using Local_Backup_Creator.MTP;
using Local_Backup_Creator.Utility;
using System.Diagnostics;
namespace Local_Backup_Creator
{
    public partial class MainForm : Form
    {
        #region Code Structure
        /*
            --- Local Backup Creator ---
            
            Hey there, here is an explanation of the code I made to help me remember my project!
            In case I jump back in after decades (?) or show to a future employer.
            This project was started in August 2025 and will be completed as of January 2026.
            I don't think there will be any new addons.
            95% of the code is fully thought and written by me, 
            the 5% being chatGPT for educationnal and learning purposes.
            I Used Visual Studio 2022 + 2026 for the UI + coding (all in C# + DotNet 9 -> 10).

            This program is based off 4 crucial pieces, and I will go in details for every one of them.


            1. MediaDevices Library (as a nuget package)

                I don't even know what it does in depth.
                It is only used as the low-level intermediary between my code and the MTP devices.
                It offers methods for handling connections and file transfer. The MTP.cs file implements them.
                It is all synchronous so I can't multithread.

            
            2. MTP.cs
                
                This file is divided into two sections: 
                The Connection and the Operation part for the MTP devices.
                
                Connection : For this program to work, we need a source, destination and logging filepath.
                The path can come from Windows (C or D disk + online), 
                Linux (/home) and an MTP device (Like \Card\ or \Internal Integrated Storage\ for Android).
                So i've created 3 variables : srcDevice, dstDevice, logDevice. 
                They will contain the status of the corresponding device. If there's no appropriate MTP device connected,
                the value is null. Every method that works in-between the UI and MediaDevice library will receive
                the appropriate device variable as a parameter so they can differentiate and execute the matching operation.
                A connection is established for every variable. 
                Nothing will stop the program from connecting to the same device many times (for soure and destination).
                This may cause bugs.
                
                MTP.cs also contains the methods for file transferring, 
                not including path manipulation. File transfer includes 
                downloading + uploading between MTP devices and the program's device.

            3. Utility.cs
                
                This file first contains many variables that are used for the transfer of information 
                from the UI to many methods that haven't access to the interface. # of threads, logging, etc...
                There is a logging section that is used to create the log file (duh).
                Next, the most important part is the Backup class. It uses two methods :

                the Analyse method : It is a recursive method that scans a specified folder from a Queue (folderListToAnalyse())
                and operates by comparing the differences from the folder both in the source and destination.
        (eg.)       C:\Backup\folder1
                    D:\My files\Backup\folder1
                Files and folders differing in name (+ size and date if "compare file metadata" is turned on) will be deleted then
                copied from the source to the destination to recreate the exact content of the source. 
                Subfolders that are in the source will then be added to the queue.

                For the Action method, it is the one that gets called by Analyse and will copy | delete | create a folder | remove it
                accordigly to the state of the media devices. It also handles errors and notes it on the logging.

            4. Form1.cs
                
                Yup, most of the code isn't on the program.cs, its here and it is divided in three sections.
                Onload, Backup Configuration and Running.

                    Onload: This part reads the settings.cfg and apply the choices to the UI.
                    It is also here where the UI is updated a bit and paths for resources are created.
        
                    Backup Configuration: This is where actions resulting of button clicks happens,
                    such as selecting a folder, saving the configuration, enabling MTP for a device, etc...
                    
                    Running: It starts when the user press start. the method called calls other methods for validating
                    the user's choice, saving the config automatically, starting the async threads on the starting folders
                    and updating the UI to monitor the progress. Then there is the cancel button in the same section.


            Every thread created by the main thread are async. 
            The main thread only incorporates the UI interactions, 
            the progress (using ShowProgression()). and the MTP device selection.
            The main thread then creates a specific number of threads for the file IO matching maxThreads on settings.cfg
            or the # of physical cores this computer has.

        Every section is delimited by #regions to improve readability. Delve in and enjoy :)
        */
        #endregion Code Structure
        #region Onload
        //Below are every methods that will be executed at the start of the program.

        public MainForm()
        {
            InitializeComponent();

            // Build the paths for the settings.cfg and instructions.md and disable the GIF

            string baseDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources");
            Config.settingsPath = Path.Combine(baseDir, "settings.cfg");
            Config.instructionsPath = Path.Combine(baseDir, "instructions.md");

            pictureBoxGIF.Visible = false;

            //Semi-transparency for the panels
            panel1.BackColor = Color.FromArgb(230, 215, 228, 242);
            panel2.BackColor = Color.FromArgb(230, 255, 255, 255);
            panel3.BackColor = Color.FromArgb(230, 215, 228, 242);
            panel4.BackColor = Color.FromArgb(230, 255, 255, 255);
            panel5.BackColor = Color.FromArgb(230, 215, 228, 242);
            panel6.BackColor = Color.FromArgb(230, 215, 228, 242);

            //Ignore the background erase when resizing the Form, causing flickering!
            this.SetStyle(ControlStyles.AllPaintingInWmPaint |
              ControlStyles.UserPaint |
              ControlStyles.OptimizedDoubleBuffer, true);
        }

        private void Form1_Load(object sender, EventArgs e) //Read the settings.cfg and apply the parameters
        {
            //Can't be in a dedicated class, uses UI elements
            if (File.Exists(Config.settingsPath))
            {
                string[] lines = File.ReadAllLines(Config.settingsPath);

                foreach (string line in lines)
                {
                    if (!line.Contains('=')) 
                        continue;

                    // Split only once per line
                    string[] parts = line.Split('=', 2); //2 ensures it only splits at the first '='
                    string key = parts[0].Trim();
                    string value = parts[1].Trim();
                    bool isTrue = value.Equals("true", StringComparison.OrdinalIgnoreCase);

                    switch (key.ToLower()) // Using ToLower for case-insensitive matching
                    {
                        case "checkboxsrcmtp":
                            checkBoxSrcMTP.Checked = isTrue;
                            break;

                        case "textboxsrcfolder":
                            textBoxSrcFolder.Text = value;
                            break;

                        case "checkboxsubfolder":
                            checkBoxSubfolder.Checked = isTrue;
                            break;

                        case "textboxsubfolder":
                            textBoxSubfolder.Text = value;
                            break;

                        case "checkboxdstmtp":
                            checkBoxDstMTP.Checked = isTrue;
                            break;

                        case "textboxdstfolder":
                            textBoxDstFolder.Text = value;
                            break;

                        case "checkboxlog":
                            checkBoxLOG.Checked = isTrue;
                            break;

                        case "checkboxlogmtp":
                            checkBoxLOGMTP.Checked = isTrue;
                            break;

                        case "textboxlocationlog":
                            textBoxLocationLOG.Text = value;
                            break;

                        case "checkboxcompmeta":
                            checkBoxCompMeta.Checked = isTrue;
                            break;

                        case "autoclose":
                            Config.autoClose = isTrue;
                            break;

                        case "includesystemp":
                            Config.includeSysTemp = isTrue;
                            break;

                        case "backgroundlaunch":
                            //Automatically launch the backup process with the form1 minimised
                            Config.backgroundLaunch = isTrue;
                            if (isTrue)
                            {
                                WindowState = FormWindowState.Minimized;
                                StartBackup(buttonStart, EventArgs.Empty);
                            }
                            break;


                        case "maxthreads":
                            //The number of Threads used to analyse the backup
                            if (ushort.TryParse(value, out ushort threads))
                            {
                                if (threads == 0) //CPU gonna go BRRRR
                                    threads = (ushort)Environment.ProcessorCount;

                                if (threads > 128) // ushort is always >= 0
                                {
                                    MessageBox.Show("The value of maxThreads is incorrect!\n" +
                                        "It must be between 1 and 128 for a custom number of threads " +
                                        "or set to 0 for the default value"
                                        , "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    Application.Exit();
                                }
                                Config.pcThreads = threads;
                            }
                            break;
                    }
                }
            }
            else
            {
                MessageBox.Show("settings.cfg file not found. " +
                    "It should be in the 'Resources' folder", "Warning",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            //Check to disable elements that may not be used
            CheckBoxSubfolderChanged(checkBoxSubfolder, EventArgs.Empty);
            CheckBoxLOGChanged(checkBoxLOG, EventArgs.Empty);

            Config.isLoading = false;//The form should show up now.
        }

        private void CheckBoxSubfolderChanged(object sender, EventArgs e)
        //Grey out the textboxes if they wont be used
        //Related to UI so it stays here
        {
            if (checkBoxSubfolder.Checked)
            {
                textBoxSubfolder.Enabled = true;
                buttonSelectSubfolder.Enabled = true;
            }
            else
            {
                textBoxSubfolder.Enabled = false;
                textBoxSubfolder.Text = "";
                buttonSelectSubfolder.Enabled = false;
            }
        }

        private void CheckBoxLOGChanged(object sender, EventArgs e)
        //Grey out the textboxes if they wont be used
        {
            if (checkBoxLOG.Checked)
            {
                textBoxLocationLOG.Enabled = true;
                buttonSelectLOG.Enabled = true;
            }
            else
            {
                textBoxLocationLOG.Enabled = false;
                textBoxLocationLOG.Text = "";
                buttonSelectLOG.Enabled = false;
            }
        }

        private void CheckBoxSrcMTPChanged(object sender, EventArgs e)
        //Called onload and if the user ticks or unticks this checkBox
        {
            if (checkBoxSrcMTP.Checked)
            {
                //Select the correct MTP device only if the user check it manually,
                //or press start and hasn't chosen yet, not onload.
                if (!Config.isLoading)
                {
                    textBoxSrcFolder.Text = "";
                    textBoxSrcFolder.PlaceholderText =
                        @"Example : Apple iPhone 18 Pro or Apple iPhone;\Internal Storage\202601";

                    Device.SelectDevice("src");
                    if (Device.srcDevice != null)
                    {
                        //FriendlyName -> Model -> Description -> "Unknown Device"
                        string displayName =
                            !string.IsNullOrWhiteSpace(Device.srcDevice.FriendlyName) ? Device.srcDevice.FriendlyName :
                            !string.IsNullOrWhiteSpace(Device.srcDevice.Model) ? Device.srcDevice.Model :
                            "Unknown MTP Device";

                        textBoxSrcFolder.Text = displayName + ';' + Device.ListRoots(Device.srcDevice);
                    }
                }
            }
            else
            {
                if (!Config.isLoading)
                {//User unchecked it
                    Device.srcDevice = null!;
                    textBoxSrcFolder.Text = "";
                    textBoxSrcFolder.PlaceholderText =
                        @"Example : C:\Users\public or C:\My Backup";
                }
            }
        }

        private void CheckBoxDstMTPChanged(object sender, EventArgs e)
        {

            if (checkBoxDstMTP.Checked)
            {
                if (!Config.isLoading)
                {
                    textBoxDstFolder.Text = "";
                    textBoxDstFolder.PlaceholderText =
                        @"Example : Samsung S26 or SM-S938B;\Internal shared storage\DCIM";

                    Device.SelectDevice("dst");
                    if (Device.dstDevice != null)
                    {
                        string displayName =
                            !string.IsNullOrWhiteSpace(Device.dstDevice.FriendlyName) ? Device.dstDevice.FriendlyName :
                            !string.IsNullOrWhiteSpace(Device.dstDevice.Model) ? Device.dstDevice.Model :
                            "Unknown MTP Device";

                        textBoxDstFolder.Text = displayName + ';' + Device.ListRoots(Device.dstDevice);
                    }
                }
            }
            else
            {
                if (!Config.isLoading)
                {
                    Device.dstDevice = null!;
                    textBoxDstFolder.Text = "";
                    textBoxDstFolder.PlaceholderText =
                        @"Example : D:\My Backup or \\NAS-01\shared-files\backup";
                }
            }
        }

        private void CheckBoxLOGMTPChanged(object sender, EventArgs e)
        {
            if (checkBoxLOGMTP.Checked)
            {
                if (!Config.isLoading)
                {
                    textBoxLocationLOG.Text = "";
                    textBoxLocationLOG.PlaceholderText =
                        @"Example : Pixel 11 Pro;\Card\My backup or CANON EOS;\DCIM";

                    Device.SelectDevice("log");
                    if (Device.logDevice != null)
                    {
                        string displayName =
                            !string.IsNullOrWhiteSpace(Device.logDevice.FriendlyName) ? Device.logDevice.FriendlyName :
                            !string.IsNullOrWhiteSpace(Device.logDevice.Model) ? Device.logDevice.Model :
                            "Unknown MTP Device";

                        textBoxLocationLOG.Text = displayName + ';' + Device.ListRoots(Device.logDevice);
                    }
                }
            }
            else
            {
                if (!Config.isLoading)
                {
                    Device.logDevice = null!;
                    textBoxLocationLOG.Text = "";
                    textBoxLocationLOG.PlaceholderText =
                        @"Example : D:\ or \\192.168.0.10\data\logs";
                }
            }
        }

        #endregion Onload
        #region Backup Configuration
        //Below are methods used to prepare for the backup
        //(folder selection, instruction file)

        private void SaveConfigClick(object sender, EventArgs e)
        {
            SaveConfig(buttonStart, EventArgs.Empty);
        }

        private void OpenInstructions(object sender, LinkLabelLinkClickedEventArgs e)
        {
            //Open the instructions.md file via notepad.exe
            try
            {
                if (File.Exists(Config.instructionsPath))
                    Process.Start("notepad.exe", Config.instructionsPath);
                else
                    MessageBox.Show("instructions.md file not found.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening instructions.md file: {ex.Message}"
                    , "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SelectFolder(object sender, EventArgs e) //Used by 3 buttons: buttonSrc, Dst and LOG
        //Opens a file explorer windows where the user select the source,
        //destination and the location of the LOG file.
        {
            //Source
            if (checkBoxSrcMTP.Checked && sender == buttonSelectSrcFolder)
            {
                Device.SelectDevice("src");
                {
                    //FriendlyName -> Model -> Description -> "Unknown Device"
                    string displayName =
                        !string.IsNullOrWhiteSpace(Device.srcDevice.FriendlyName) ? Device.srcDevice.FriendlyName :
                        !string.IsNullOrWhiteSpace(Device.srcDevice.Model) ? Device.srcDevice.Model :
                        "Unknown MTP Device";

                    textBoxSrcFolder.Text = displayName + ';' + Device.ListRoots(Device.srcDevice);
                }
            }
            //Destination
            else if (checkBoxDstMTP.Checked && sender == buttonSelectDstFolder)
            {
                Device.SelectDevice("dst");
                if (Device.dstDevice != null)
                {
                    string displayName =
                        !string.IsNullOrWhiteSpace(Device.dstDevice.FriendlyName) ? Device.dstDevice.FriendlyName :
                        !string.IsNullOrWhiteSpace(Device.dstDevice.Model) ? Device.dstDevice.Model :
                        "Unknown MTP Device";

                    textBoxDstFolder.Text = displayName + ';' + Device.ListRoots(Device.dstDevice);
                }
            }
            //LOG location
            else if (checkBoxLOGMTP.Checked && sender == buttonSelectLOG)
            {
                Device.SelectDevice("log");
                if (Device.logDevice != null)
                {
                    string displayName =
                        !string.IsNullOrWhiteSpace(Device.logDevice.FriendlyName) ? Device.logDevice.FriendlyName :
                        !string.IsNullOrWhiteSpace(Device.logDevice.Model) ? Device.logDevice.Model :
                        "Unknown MTP Device";

                    textBoxLocationLOG.Text = displayName + ';' + Device.ListRoots(Device.logDevice);
                }
            }
            else
            {
                using FolderBrowserDialog dialog = new();

                dialog.Description = "Select a folder";
                dialog.UseDescriptionForTitle = true;
                String startPath;

                if (sender is Button clickedButton)
                //Verify if it's a button that called the function
                { /*Find the button that called the function and 
                1. open the FolderBrowserDialog at the corresponding place */

                    if (clickedButton == buttonSelectSrcFolder)
                    {
                        startPath = textBoxSrcFolder.Text;

                        //We put the right path into the startPath variable and check it.
                        if (!string.IsNullOrWhiteSpace(startPath) && Directory.Exists(startPath))
                        {
                            dialog.SelectedPath = startPath; //Now become the initial folder
                        }
                    }

                    else if (clickedButton == buttonSelectDstFolder) //Same thing but for the 3 buttons
                    {
                        startPath = textBoxDstFolder.Text;

                        if (!string.IsNullOrWhiteSpace(startPath) && Directory.Exists(startPath))
                        {
                            dialog.SelectedPath = startPath;
                        }
                    }

                    else if (clickedButton == buttonSelectLOG)
                    {
                        startPath = textBoxLocationLOG.Text;

                        if (!string.IsNullOrWhiteSpace(startPath) && Directory.Exists(startPath))
                        {
                            dialog.SelectedPath = startPath;
                        }
                    }
                }

                if (dialog.ShowDialog() == DialogResult.OK) //User has chosen a path
                {
                    if (sender is Button clickedButton2)//Same thing, not optimal my bad
                    {
                        if (clickedButton2 == buttonSelectSrcFolder) //2. Update the textBox w. the selected path
                        {
                            textBoxSrcFolder.Text = dialog.SelectedPath;
                        }

                        else if (clickedButton2 == buttonSelectDstFolder)
                        {
                            textBoxDstFolder.Text = dialog.SelectedPath;
                        }

                        else if (clickedButton2 == buttonSelectLOG)
                        {
                            textBoxLocationLOG.Text = dialog.SelectedPath;
                        }
                    }
                }
            }
        }

        private void SelectSubfolder(object sender, EventArgs e)//Only used by the select subfolder button
        {
            //Obtains the path of the subfolder then keeps the folder name and adds it to the textBox.
            using FolderBrowserDialog dialog = new();

            dialog.Description = "Select a subfolder";
            dialog.UseDescriptionForTitle = true;

            if (Device.srcDevice != null)
            {
                MessageBox.Show("Since MTP is enabled, you must manually write the name of the subfolders!" +
                    "\nYou can use File Explorer to find thoses folders."
                        , "MTP is enabled", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            String startPath = Backup.EnumerateFolders(textBoxSrcFolder.Text, Device.srcDevice)[0];

            if (!string.IsNullOrWhiteSpace(startPath) && Backup.PathExists(startPath, false, Device.srcDevice))
            {
                dialog.SelectedPath = startPath; //Now become the initial folder
            }

            if (dialog.ShowDialog() == DialogResult.OK) //User has chosen a path
            {
                string subfolderName = Backup.GetRelativePath(dialog.SelectedPath, "", Device.srcDevice);

                textBoxSubfolder.Text += subfolderName + ';';
            }
        }

        #endregion Backup Configuration
        #region Execution
        //Below are all the methods used to create the backup.

        private async void StartBackup(object sender, EventArgs e)
        //Contains every steps to create the backup,
        //like the main() in Python after the user clicked the start button
        {
            //Step 1 : Select the MTP device if they haven't been selected yet
            if (checkBoxSrcMTP.Checked)
            {
                try
                {
                    //Is it null? If not, connect.
                    if (Device.srcDevice == null)
                    {
                        MessageBox.Show("The source MTP device is not selected! " +
                            "Please select it first before starting the backup.",
                            "Source MTP device not found!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    Device.srcDevice.Connect();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"The source MTP device cannot be connected!" +
                        $"Please select it again.\n {ex}",
                        "Source MTP device not found!"
                        , MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    Device.srcDevice = null!;
                    return;
                }
            }
            else
            {//Just in case since every method checks if the device == null.
                if (Device.srcDevice != null)
                {
                    Device.srcDevice = null!;
                }
            }

            if (checkBoxDstMTP.Checked)
            {
                try
                {
                    if (Device.dstDevice == null)
                    {
                        MessageBox.Show("The destination MTP device is not selected! " +
                            "Please select it first before starting the backup.",
                            "Destination MTP device not found!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    Device.dstDevice.Connect();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"The destination MTP device cannot be connected!" +
                        $"Please select it again.\n {ex}",
                        "Destination MTP device not found!"
                        , MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    Device.dstDevice = null!;
                    return;
                }
            }
            else
            {
                if (Device.dstDevice != null)
                {
                    Device.dstDevice = null!;
                }
            }

            if (checkBoxLOGMTP.Checked)
            {
                try
                {
                    if (Device.logDevice == null)
                    {
                        MessageBox.Show("The log MTP device is not selected! " +
                            "Please select it first before starting the backup.",
                            "Log MTP device not found!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    Device.logDevice.Connect();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"The log MTP device cannot be connected!" +
                        $"Please select it again.\n {ex}",
                        "Log MTP device not found!"
                        , MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    Device.logDevice = null!;
                    return;
                }
            }
            else
            {
                if (Device.logDevice != null)
                {
                    Device.logDevice = null!;
                }
            }

            //Step 2: Assign the values of frequently used parameters into the Config class
            //Used by CheckChoices()

            Config.createLOG = checkBoxLOG.Checked;
            Config.logDone = false; //Is set to true (log has been made) when the backup is finished
            //If not finished (canceled or errors), still create one.

            Config.finished = false;

            Config.compMetadata = checkBoxCompMeta.Checked;

            //By sending a MediaDevice to the method, no worry for the kind of path managed.

            Config.srcFolder = GetPath(textBoxSrcFolder.Text).TrimEnd(
                Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

            Config.dstFolder = GetPath(textBoxDstFolder.Text).TrimEnd(
                Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

            Config.logFolder = GetPath(textBoxLocationLOG.Text).TrimEnd(
                Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

            if (checkBoxSubfolder.Checked)
                Config.subfolders = [.. textBoxSubfolder.Text.Trim(';').Split(';').Select(v => v.Trim())];

            if (Device.srcDevice != null || Device.dstDevice != null)
                Config.pcThreads = 1;

            //Step 3: Check if the folders and subfolders create a correct path
            //before starting the backup

            bool problem = CheckChoices(buttonStart, EventArgs.Empty);
            if (problem)
                return;

            //Step 4: Grey out everything except the Cancel button
            checkBoxSrcMTP.Enabled = false;
            buttonSelectSrcFolder.Enabled = false;
            buttonSelectDstFolder.Enabled = false;
            buttonSelectSubfolder.Enabled = false;
            buttonSelectLOG.Enabled = false;
            buttonSaveConfig.Enabled = false;
            textBoxSrcFolder.Enabled = false;
            textBoxDstFolder.Enabled = false;
            textBoxSubfolder.Enabled = false;
            textBoxLocationLOG.Enabled = false;
            checkBoxSubfolder.Enabled = false;
            checkBoxDstMTP.Enabled = false;
            checkBoxLOG.Enabled = false;
            checkBoxLOGMTP.Enabled = false;
            checkBoxCompMeta.Enabled = false;
            buttonStart.Enabled = false;

            //Step 5: Save the preferences to the config.cfg file,
            //also called before exiting the program
            SaveConfig(buttonStart, EventArgs.Empty);


            //Step 6: Animate the progressBar and Label async
            _ = ShowProgression();
            //Say shush to the compiler, the showProgression will run in parallel to updateBackup

            //Step 7: Actually analyse files and do the backup
            //It's this method creating the threads and assigning them a task from the Queue.
            await Task.Run(() => UpdateBackup(this, EventArgs.Empty));
            //The work is offloaded from the UI thread to a background thread.

            //Step 8: End of the backup.
            //Un-grey out the UI
            buttonSelectSrcFolder.Enabled = true;
            buttonSelectDstFolder.Enabled = true;
            buttonSelectSubfolder.Enabled = true;
            buttonSelectLOG.Enabled = true;
            buttonSaveConfig.Enabled = true;
            textBoxSrcFolder.Enabled = true;
            textBoxDstFolder.Enabled = true;
            textBoxSubfolder.Enabled = true;
            textBoxLocationLOG.Enabled = true;
            checkBoxSrcMTP.Enabled = true;
            checkBoxSubfolder.Enabled = true;
            checkBoxDstMTP.Enabled = true;
            checkBoxLOG.Enabled = true;
            checkBoxLOGMTP.Enabled = true;
            checkBoxCompMeta.Enabled = true;
            buttonStart.Enabled = true;

            //Regroup the results into a LOG file at the end.

            if (!Config.backgroundLaunch)
                MessageBox.Show("Backup succesfully finished"
                    , "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            Logging.textLOG += $"\n{DateTime.Now:yyyy-MM-dd - HH:mm:ss}" +
                $" | Info : Backup succesfully done!";

            if (Config.createLOG)
                Logging.CreateLOG(Config.logFolder, Device.logDevice);
            Config.logDone = true;

            Device.srcDevice?.Disconnect();
            Device.dstDevice?.Disconnect();
            Device.logDevice?.Disconnect();

            //Wait or close by itself depending on settings.cfg

            if (Config.autoClose)
                ButtonCancelClick(buttonCancel, EventArgs.Empty);

            CheckBoxSubfolderChanged(checkBoxSubfolder, EventArgs.Empty);
            CheckBoxLOGChanged(checkBoxSubfolder, EventArgs.Empty);
        }

        //Below are every methods called from above in order.

        private static string GetPath(string path)
                => path.Contains(';') ? path.Split(';')[1].Trim() : path;

        private bool CheckChoices(object sender, EventArgs e)
        {
            //Step 1: Check if the folders and subfolders create a correct path
            //with the source and destination folder.

            //iPhones deny write or update access,
            //they can only be source, not destination nor log location.

            if ((Device.dstDevice != null && Device.dstDevice.Model.Contains("Apple iPhone"))
                || (Device.logDevice != null && Device.logDevice.Model.Contains("Apple iPhone")))
            {
                MessageBox.Show($"Your iPhone cannot be the destination nor the log location of your backup" +
                    $"since this program does not have the permissions to write on this device."
                    , "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return true;
            }

            //textBoxSrcFolder:
            if (!Backup.PathExists(Config.srcFolder, false, Device.srcDevice))
            {
                MessageBox.Show($"The path in the source folder location doesn't exist :\n{Config.srcFolder}"
                    , "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return true;
            }

            //textBoxDstFolder:
            if (!Backup.PathExists(Config.dstFolder, false, Device.dstDevice))
            {
                MessageBox.Show($"The path in the destination folder location doesn't exist :\n{Config.dstFolder}"
                    , "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return true;
            }

            if (checkBoxLOG.Checked)
            {
                if (!Backup.PathExists(Config.logFolder, false, Device.logDevice))
                {
                    MessageBox.Show($"The path in the log file location doesn't exist :\n{Config.logFolder}"
                        , "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return true;
                }
            }

            if (Config.srcFolder.Contains("Local Backup Creator")
                //The folder name of the program ;-)
                || Config.dstFolder.Contains("Local Backup Creator"))
            {
                MessageBox.Show("Task failed successfully!"
                , "Local Backup Creator", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            if (checkBoxSubfolder.Checked)
            {
                //Source :

                foreach (string subfolderName in Config.subfolders)
                {
                    if (!Backup.PathExists(Config.srcFolder + "\\" + subfolderName
                        , false, Device.srcDevice)
                        || String.IsNullOrWhiteSpace(subfolderName))
                    {
                        MessageBox.Show("One or many chosen subfolders aren't valid!" +
                        "\nCheck if the subfolders are located in the source folder and " +
                        "that they are separated by a ; with no spaces."
                        , "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return true;
                    }
                }
                //No destination check, the folders will be created.
            }
            return false;
        }

        private void SaveConfig(object sender, EventArgs e)
        //Step 2: Save the preferences to the config.cfg file when everything is okay
        {
            List<string> lines =
            [
                $"#Backup Settings : ",
                $"",
                $"checkBoxSrcMTP={(checkBoxSrcMTP.Checked ? "true" : "false")}",
                $"textBoxSrcFolder={textBoxSrcFolder.Text}",
                $"",
                $"checkBoxSubfolder={(checkBoxSubfolder.Checked ? "true" : "false")}",
                $"textBoxSubfolder={textBoxSubfolder.Text}",
                $"",
                $"CheckBoxDstMTP={(checkBoxDstMTP.Checked ? "true" : "false")}",
                $"textBoxDstFolder={textBoxDstFolder.Text}",
                $"",
                $"checkBoxLOG={(checkBoxLOG.Checked ? "true" : "false")}",
                $"checkBoxLOGMTP={(checkBoxLOGMTP.Checked ? "true" : "false")}",
                $"textBoxLocationLOG={textBoxLocationLOG.Text}",
                $"",
                $"checkBoxCompMeta={(checkBoxCompMeta.Checked ? "true" : "false")}",
                $"",
                $"#Automatisation : ",
                $"",
                $"backgroundLaunch={Config.backgroundLaunch.ToString().ToLower()}",
                $"autoClose={Config.autoClose.ToString().ToLower()}",
                $"",
                $"MaxThreads={(Config.pcThreads == Environment.ProcessorCount ? "0" : "1")}",
                $"",
                $"includeSysTemp={Config.includeSysTemp.ToString().ToLower()}"
            ];
            File.WriteAllLines(Config.settingsPath, lines);
        }

        private async Task ShowProgression()
        //Creates a progressbar and updates it async using the values from the Config variables
        {
            //Silent exceptions are a thing now?
            try
            {
                pictureBoxGIF.Visible = true;
                labelProgress.Text = "The backup process is starting...";

                //The Display process will execute in two different ways
                //depending on the device being MTP or not:

                //If not MTP, start an async task that will update the number of folders async.
                //The progressBar will be constantly updated depending on the percentage folderRead / folderTotal

                //If MTP, the progressBar will be Marquee style with no info on the progression.

                //to be used as the total of folders in the progressBar
                int foldersTotal = 1;

                if (Device.srcDevice == null)
                //Manual progressive count, way faster and updates the display every 100 ms
                {
                    progressBarBackup.Minimum = 0;
                    progressBarBackup.Maximum = 100;
                    progressBarBackup.Value = 0;

                    void SafeEnumerate(string srcSubPath)
                    {
                        try
                        {
                            DirectoryInfo dirInfo = new(srcSubPath);

                            foreach (FileSystemInfo fsInfo in dirInfo.GetDirectories())
                            {
                                Interlocked.Increment(ref foldersTotal);

                                if (fsInfo is DirectoryInfo subDir)
                                {
                                    SafeEnumerate(subDir.FullName);
                                }
                            }
                        }
                        catch (Exception ex) when
                        (ex is UnauthorizedAccessException || ex is DirectoryNotFoundException)
                        {
                            //Skip the bad folder
                        }
                    }

                    Task Count(string srcSubPath) =>
                        Task.Run(() => SafeEnumerate(srcSubPath));

                    if (!checkBoxSubfolder.Checked)
                        _ = Count(Config.srcFolder);
                    else
                    {
                        foreach (string subfolderName in Config.subfolders)
                        {
                            _ = Count(Path.Join(Config.srcFolder, subfolderName));
                        }
                    }
                }

                else
                {
                    progressBarBackup.Style = ProgressBarStyle.Marquee;
                    progressBarBackup.MarqueeAnimationSpeed = 50;
                }

                int percentage = 0;

                //Translate the progressbar's value to percentages
                while (!Config.finished)
                {

                    if (Device.srcDevice == null)
                    {
                        // Int / Int == Int!!!
                        percentage = (int)Math.Floor((float)Config.foldersRead / (float)foldersTotal * 100.0f);
                        if (percentage >= 100)//Always a discrepancy, you will probably notice it on the display
                            percentage = 99;
                        if (percentage == 0)
                            percentage = 1;
                        progressBarBackup.Value = percentage;
                    }

                    labelProgress.Text =
                        $"Folders in total : {(Device.srcDevice != null ? "Unknown" : foldersTotal)} - " +
                        $"Folders read : {Config.foldersRead} - Status : " +
                        $"{(Device.srcDevice != null ? "Unknown" : progressBarBackup.Value.ToString() + '%')}" +
                        $" - Active threads : {Volatile.Read(ref Config.activeThreads)}\n" +
                        $"Processing {Config.currentlyAnalysed}";
                    //In float it's: float percentage = ((float)filesRead / foldersTotal) * 100f;
                    await Task.Delay(100);
                    //Every 0.1s, update the progressBar with higher percentage and diferent label

                }
                labelProgress.Text = "Finished!";
                progressBarBackup.Style = ProgressBarStyle.Continuous;
                progressBarBackup.Value = 100;
                pictureBoxGIF.Visible = false;
            }

            catch (Exception ex)
            {
                if (!Config.backgroundLaunch)
                    MessageBox.Show($"Silent exception: \n{ex}"
                    , "Debug 810/Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                Logging.textLOG += $"\n{DateTime.Now:yyyy-MM-dd - HH:mm:ss}" +
                            $" | Silent Exception (815) in showProgression : {ex.Message} (X)";
            }
        }

        private async Task UpdateBackup(object sender, EventArgs e)
        //Every subfolders will be analysed in a tree method from trunk to leafs passing by every branch via
        //An async method.
        {
            //Start the folder list:


            if (!checkBoxSubfolder.Checked)
                await Backup.Analyse(Config.srcFolder);
            else
            {
                foreach (string subfolderName in Config.subfolders)
                    await Backup.Analyse(
                          Backup.Combine(Config.srcFolder, subfolderName, Device.srcDevice));
            }

            ParallelOptions options = new()
            {
                MaxDegreeOfParallelism = Config.pcThreads,
                CancellationToken = CancellationToken.None
            };

            //Tells if the program is finished or not, all threads are tracked
            //Program is finished when activeThreads == 0 AND no subfolders to analyse
            Config.activeThreads = 0;

            await Parallel.ForEachAsync(Enumerable.Range(0, Config.pcThreads),
                options, async (workerId, ct) =>
            {
                while (!Backup.folderListToAnalyse.IsEmpty || Volatile.Read(ref Config.activeThreads) > 0)
                {
                    if (Backup.folderListToAnalyse.TryDequeue(out string? srcFolder))
                    {
                        Interlocked.Increment(ref Config.activeThreads);

                        try
                        {
                            await Backup.Analyse(srcFolder);
                        }
                        finally
                        {
                            Interlocked.Decrement(ref Config.activeThreads);
                        }
                    }
                    else
                        await Task.Delay(100, ct);
                }
            });
            Config.finished = true;
        }


        private void ButtonCancelClick(object sender, EventArgs e)
        //Close the program after saving the preferences and stopping the backup process
        {
            DialogResult result = DialogResult.Yes;

            if (!Config.autoClose)
                result = MessageBox.Show("Close the program?", "Local Backup Creator"
                    , MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                SaveConfig(buttonStart, EventArgs.Empty);
                if (!Config.logDone)
                    Logging.CreateLOG(Config.logFolder, Device.logDevice);

                Config.finished = true;

                Device.srcDevice?.Disconnect(); //Nullish coalescing, super cool!
                Device.dstDevice?.Disconnect();
                Device.logDevice?.Disconnect();

                //Be sure the LOG and config.cfg have been saved + threads have closed
                Thread.Sleep(100);
                Application.Exit();
            }
        }
    }
}
        #endregion Execution