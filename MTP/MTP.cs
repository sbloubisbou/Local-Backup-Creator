using MediaDevices;
using Microsoft.VisualBasic;
using System.Diagnostics;
using System.Net.Security;
using System.Runtime.InteropServices;
using System.Text;
namespace Local_Backup_Creator.MTP
{
    #region MTP Connection
    internal static class Device
    {
        internal static MediaDevice srcDevice = null!;
        internal static MediaDevice dstDevice = null!;
        internal static MediaDevice logDevice = null!;
        //Program won't do the backup if no device is appropriately selected.

        internal static void SelectDevice(string deviceType)// "src" | "dst" | "log"
        {
            List<MediaDevice> devices = [.. MediaDevice.GetDevices()];

            if (devices.Count == 0)
            {
                MessageBox.Show($"There are no media devices connected!\n" +
                    $"Be sure that you have plugged correctly your device" +
                    $" and have granted the right for this PC to access the storage of your device."
                    , "No device connected!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            //Get the devices' name by briefly connecting to it.
            List<string> deviceNames = [];
            foreach (MediaDevice d in devices)
            {
                try
                {
                    d.Connect();
                    string name = !string.IsNullOrWhiteSpace(d.FriendlyName) ? d.FriendlyName : d.Model;
                    deviceNames.Add(name ?? "Unknown MTP Device");
                }
                catch
                {
                    deviceNames.Add("Unknown MTP Device");
                }
                finally
                {
                    if (d.IsConnected)
                        d.Disconnect();
                }
            }

            //Always show a list, even for one device
            string deviceList = string.Join("\n", deviceNames.Select((n, i) => $"{i + 1}. {n}"));
            string input = Interaction.InputBox(
                $"Select a device by number:\n\n{deviceList}",
                "Select an MTP Device", "1");

            if (int.TryParse(input, out int index) && index >= 1 && index <= devices.Count)
            {
                MediaDevice selectedDevice = devices[index - 1];

                //If a new device is selected, check if it is the same as dst and log.
                //Do this vice versa using references.

                ref MediaDevice target = ref srcDevice;
                if (deviceType == "dst") target = ref dstDevice;
                if (deviceType == "log") target = ref logDevice;

                //Check if this device is already assigned to another role
                MediaDevice? existing = new[] { srcDevice, dstDevice, logDevice }
                    .FirstOrDefault(d => d?.DeviceId == selectedDevice.DeviceId);

                if (existing != null)
                    target = existing;
                //This points the selected device to the same device selected at another place.

                else
                {
                    target = selectedDevice;
                    try
                    {
                        target.Connect();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(
                            $"Failed to connect to {srcDevice.Model}.\n{ex.Message}",
                            "MTP Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    finally
                    {
                        target?.Disconnect();
                        //Only at the start of the backup they will be connected.
                    }
                }
            }
            else
            {
                MessageBox.Show("Invalid MTP device selection!",
                "Wrong MTP device selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        #endregion MTP Connection

        #region MTP Operations

        internal static string ListRoots(MediaDevice currentDevice)
        {

            currentDevice.Connect();
            List<MediaDriveInfo> drives = [.. currentDevice.GetDrives()];

            StringBuilder sb = new();

            if (drives.Count == 0)
            {
                MessageBox.Show($"This current device has no accessible storage! " +
                    $"Be sure to have unlocked your device and have allowed your PC to access its files when prompted."
                    , "No storage detected!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                currentDevice.Disconnect();
                return "";
            }

            for (int i = 0; i < drives.Count; i++)
            {
                sb.AppendLine($"{i + 1}. {drives[i].Name}");

                //Show top-level dirs inside the drive
                string[] dirs = currentDevice.GetDirectories(drives[i].Name);
                foreach (string? dir in dirs.Take(4))
                //Limit for readability, 2-3 max root directory displayed or window will be too big
                {
                    sb.AppendLine($"   - {dir}");
                }
            }

            currentDevice.Disconnect();

            string input = Interaction.InputBox(
                $"Select a drive by number:\n\n{sb}",
                "Select MTP Storage", "1");

            if (int.TryParse(input, out int choice) && choice > 0 && choice <= drives.Count)
            {
                return drives[choice - 1].Name; //Ex. "\Internal shared storage" or "\Card"
            }
            return "";
        }

        internal static void CreateFolder(string path, MediaDevice currentDevice)
        {
            currentDevice.CreateDirectory(path);
        }

        internal static void DeleteFolder(string path, MediaDevice currentDevice)
        {
            if (currentDevice.DirectoryExists(path))
                currentDevice.DeleteDirectory(path, recursive: true);
        }

        internal static void CopyFileFromMTP(string sourcePath, string dstPath
            , MediaDevice currentDevice)
        {
            if (!File.Exists(Path.Combine(dstPath, Path.GetFileName(sourcePath))))
            {
                try
                {
                    currentDevice.DownloadFile(sourcePath, dstPath);
                }
                catch (COMException)
                {
                    //Happens if there is a transfer in progress and the user closes the app.
                }
            }
                
        }

        internal static void CopyFileToMTP(string sourcePath, string dstPath
            , MediaDevice currentDevice)
        {
            if (!currentDevice.FileExists(dstPath))
            {
                try
                {
                    currentDevice.UploadFile(sourcePath, dstPath);
                }
                catch (COMException)
                {
                    //Hey there!
                }
            }
        }

        internal static void CopyFileMtpToMtpBuffered(string srcPath, string dstPath,
            MediaDevice srcDevice, MediaDevice dstDevice)
        {
            //Get the unique id for openRead
            MediaFileInfo srcFileInfo = srcDevice.GetFileInfo(srcPath);

            // Check if the source and destination are actually the same physical phone
            if (srcDevice.DeviceId == dstDevice.DeviceId)
            {
                //Same device : Download, store in temp, then upload.
                string tempFile = Path.GetTempFileName();
                try
                {
                    srcDevice.DownloadFile(srcPath, tempFile);
                    //Will crash if not enough space
                    using var fs = File.OpenRead(tempFile);
                    dstDevice.UploadFile(fs, dstPath);
                    //This last line may crash if the program is closed while doing a backup
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        "This computer doesn't have enough storage" +
                        " to temporarily store this file onto the destination device : " +
                        $"{srcPath}\nPlease copy it manually.\n{ex}", "Not enough storage on your computer!"
                    , MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    if (File.Exists(tempFile)) File.Delete(tempFile);
                }
            }
            else
            {
                //Different devices : use a 'stream pipe'
                using Stream srcStream = 
                    srcDevice.OpenReadFromPersistentUniqueId(srcFileInfo.PersistentUniqueId);
                try
                {
                    dstDevice.UploadFile(srcStream, dstPath);
                }
                catch (COMException)
                {
                    //Happens if there is a transfer in progress and the user closes the app.
                }
            }
        }

        internal static void DeleteFile(string path, MediaDevice currentDevice)
        {
            if (currentDevice.FileExists(path))
                currentDevice.DeleteFile(path);
        }
    }
    #endregion MTP Operations
}