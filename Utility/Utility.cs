using Local_Backup_Creator.MTP;
using MediaDevices;
using System.Collections.Concurrent;
namespace Local_Backup_Creator.Utility
{
    #region Config Section

    internal static class Config
    {
        //Variables that will be used by different methods between different files
        //So the frontend can know the status of the backend.

        //settings.cfg and instructions.md
        internal static string settingsPath = "", instructionsPath = "";

        internal static bool isLoading = true;

        //Parameters of settings.cfg
        internal static bool backgroundLaunch = false, autoClose = false;
        //The number of threads used by the backup, default value is the # of cores
        internal static UInt16 pcThreads = 1;

        //Variables that need to be accessed by the "backend" and not vie the checkbox's value
        //Will be modified at Section 2 in startBackup

        internal static bool createLOG = false, logDone = false;
        //If the user cancels the backup midway, create a log report.

        internal static bool compMetadata = true;
        internal static string srcFolder = "", dstFolder = "", logFolder = "";

        internal static string[] subfolders = [];

        //Updated by updateBackup() in utility.cs, read by showProgression() in form1.cs
        internal static string currentlyAnalysed = "";
        internal static int foldersRead = 0;
        //Threads that are busy 
        internal static int activeThreads = 0;

        //The program will try to copy system + temporary files/folders
        internal static bool includeSysTemp = false;

        //Hardcoded finish instead of cancellationToken (cause of laziness, works just fine!)
        internal static bool finished = false;
    }

    #endregion Config Section
    #region Logging Section

    internal static class Logging
    {
        //Text containing the .LOG file
        internal static string textLOG = $"Local Backup Creator — Log of " +
            $"{DateTime.Now:yyyy-MM-dd — HH:mm:ss}\n";

        internal static void CreateLOG(string path, MediaDevice? currentDevice)
        {
            if (currentDevice != null)
            //This is how the program knows it's an MTP device or not.
            //Remember this because it is used across every method.
            {
                try
                {
                    string fullPath = Backup.Combine(
                        path, $"LBC {DateTime.Now:yyyy-MM-dd_HH.mm.ss}.log", currentDevice);

                    byte[] data = System.Text.Encoding.UTF8.GetBytes(textLOG);

                    using MemoryStream ms = new(data);
                    //Upload the stream as a new file
                    currentDevice.UploadFile(ms, fullPath);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occured while creating the .log" +
                        $" report at the MTP destination {path} : {ex.Message}",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                try
                {
                    File.WriteAllText(
                        Backup.Combine(path, $"LBC {DateTime.Now:yyyy-MM-dd_HH.mm.ss}.log"
                        , currentDevice), textLOG);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occured while creating the .log report : {ex.Message}",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            Config.logDone = true;
        }
    }

    #endregion Logging Section
    #region Global Device Operation Section
    internal static class Backup
    {
        //Create a queue that will contain the first subfolders of the source.
        //For every subfolder contained in the queue, create a multithreaded Task at updateBackup().
        internal static ConcurrentQueue<string> folderListToAnalyse = new();

        internal async static Task Analyse(string srcPath)
        //Whats to create, copy, remove and delete?
        {
            Interlocked.Exchange(ref Config.currentlyAnalysed, srcPath);

            string relativePath = GetRelativePath(Config.srcFolder, srcPath, Device.srcDevice);
            string dstSubPath = Combine(Config.dstFolder, relativePath, Device.srcDevice);

            //dstSubPath may look like this : \Card\backup\folder1/folder2/file.txt, must fix (/ -> \)
            //Since srcPath is /home/backup/folder1/folder2/file.txt (Linux)
            if (Device.dstDevice != null)
                dstSubPath = dstSubPath.Replace('/', '\\');

            else
                dstSubPath = dstSubPath.Replace('\\', Path.DirectorySeparatorChar)
                       .Replace('/', Path.DirectorySeparatorChar);


            if (!PathExists(dstSubPath, false, Device.dstDevice))
                await Action("create", dstSubPath, srcPath);

            //Files:
            try //UnauthorisedAccessException to skip due to an obligatory Directory.GetFiles
            {
                List<string> srcFilesArray = EnumerateFiles(srcPath, Device.srcDevice);

                for (Int16 i = 0; i < srcFilesArray.Count; i++)
                    srcFilesArray[i] = GetRelativePath(srcFilesArray[i], "", Device.srcDevice);
                //GetRelativePath Returns the folder's name

                List<string> dstFilesArray = EnumerateFiles(dstSubPath, Device.dstDevice);

                for (Int16 i = 0; i < dstFilesArray.Count; i++)
                    dstFilesArray[i] = GetRelativePath(dstFilesArray[i], "", Device.dstDevice);

                foreach (string srcFile in srcFilesArray)
                {
                    //Temporary, system and ReparsePoint files check first in any case
                    FileAttributes srcAttributes = FileAttributes.Normal;

                    try
                    {
                        srcAttributes = File.GetAttributes(Combine(srcPath, srcFile, Device.srcDevice));
                    }
                    catch
                    {
                        //Cursed way to know if it's MTP.
                    }

                    bool cantCopy = false;

                    if (Config.includeSysTemp && ((srcAttributes & FileAttributes.ReparsePoint) != 0))
                        cantCopy = true;

                    if (!Config.includeSysTemp &&
                        ((srcAttributes & FileAttributes.Temporary) != 0 ||
                         (srcAttributes & FileAttributes.System) != 0 ||
                         (srcAttributes & FileAttributes.ReparsePoint) != 0))
                        cantCopy = true;

                    if (cantCopy)
                    {//User wont bother to know that this file was skipped
                        Logging.textLOG += $"\nSkipped this file : " +
                            $"{Combine(srcPath, srcFile, Device.srcDevice)} - Status : " +
                            $"{((srcAttributes & FileAttributes.Temporary) != 0 ? "Temporary" : "")}" +
                            $"{((srcAttributes & FileAttributes.System) != 0 ? " System" : "")}" +
                            $"{((srcAttributes & FileAttributes.ReparsePoint) != 0 ? " ReparsePoint" : "")}";
                    }
                    else //Passed the check
                    {
                        if (dstFilesArray.Remove(srcFile))
                        //If a file is already in the backup, check size and date of last modified
                        {
                            if (Config.compMetadata && Device.srcDevice == null && Device.dstDevice == null)
                            {
                                //Size in bytes + LastWriteTimeUtc
                                FileInfo src = new(Combine(srcPath, srcFile, Device.srcDevice));

                                FileInfo dst = new(Combine(dstSubPath, srcFile, Device.dstDevice));

                                if (src.Length != dst.Length || src.LastWriteTimeUtc.ToFileTimeUtc()
                                    != dst.LastWriteTimeUtc.ToFileTimeUtc())
                                {
                                    //Overwrite the file
                                    await Action("copy", Combine(dstSubPath, srcFile, Device.dstDevice)
                                        , Combine(srcPath, srcFile, Device.srcDevice));
                                }
                            }
                        }
                        else //If a file is missing, add it
                            await Action("copy", Combine(dstSubPath, srcFile, Device.dstDevice)
                                , Combine(srcPath, srcFile, Device.dstDevice));
                    }
                }
                //Delete destination files that aren't in the source
                foreach (string dstFile in dstFilesArray)
                    await Action("delete", Combine(dstSubPath, dstFile, Device.dstDevice), "");
            }
            catch (UnauthorizedAccessException ex)
            {
                if (Config.createLOG)
                {
                    Logging.textLOG += $"\n{DateTime.Now:yyyy-MM-dd - HH:mm:ss}" +
                        $" | Error : Access to this file is denied : {ex.Message} (3)";
                }
                if (!Config.backgroundLaunch)
                {
                    MessageBox.Show($"Error : Access to this file is denied:\n{ex.Message}" +
                    $"\nMake sure to start the program with administrator priviledges",
                    "Error 188", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            //Folders:
            try
            {
                List<string> srcFoldersArray = EnumerateFolders(srcPath, Device.srcDevice);

                for (Int16 i = 0; i < srcFoldersArray.Count; i++)
                    srcFoldersArray[i] = GetRelativePath(srcFoldersArray[i], "", Device.srcDevice);

                List<string> dstFoldersArray = EnumerateFolders(dstSubPath, Device.dstDevice);

                for (Int16 i = 0; i < dstFoldersArray.Count; i++)
                    dstFoldersArray[i] = GetRelativePath(dstFoldersArray[i], "", Device.dstDevice);


                foreach (string srcFolder in srcFoldersArray)
                {
                    FileAttributes srcAttributes = FileAttributes.Normal;

                    try
                    {
                        srcAttributes = File.GetAttributes(Combine(srcPath, srcFolder, Device.srcDevice));
                    }
                    catch
                    {
                        //It's MTP
                    }

                    bool cantCopy = false;

                    if (Config.includeSysTemp && (srcAttributes & FileAttributes.ReparsePoint) != 0)
                        cantCopy = true;

                    if ((srcAttributes & FileAttributes.Temporary) != 0 ||
                            (srcAttributes & FileAttributes.System) != 0 ||
                            (srcAttributes & FileAttributes.ReparsePoint) != 0)
                        cantCopy = true;

                    if (cantCopy)
                    {
                        Logging.textLOG += $"\nSkipped this folder : " +
                            $"{Combine(srcPath, srcFolder, Device.srcDevice)} - Status : " +
                            $"{((srcAttributes & FileAttributes.Temporary) != 0 ? "Temporary" : "")} - " +
                            $"{((srcAttributes & FileAttributes.System) != 0 ? " System" : "")}" +
                            $"{((srcAttributes & FileAttributes.ReparsePoint) != 0 ? " ReparsePoint" : "")}";
                    }
                    else
                    {
                        //Note that I wont compare the metadata of a folder, 
                        //The subfolders may be currently analysed, crashing the program
                        folderListToAnalyse.Enqueue(Combine(srcPath, srcFolder, Device.srcDevice));
                        //Add folders to get analysed, recursion

                        if (!dstFoldersArray.Remove(srcFolder))
                            await Action("create", Combine(dstSubPath, srcFolder, Device.dstDevice),
                                 Combine(srcPath, srcFolder, Device.srcDevice));
                    }
                }
                //Delete destination folders that aren't in the source
                foreach (string dstFolder in dstFoldersArray)
                    await Action("remove", Combine(dstSubPath, dstFolder, Device.dstDevice)
                        , "");

                Interlocked.Increment(ref Config.foldersRead);
            }
            catch (UnauthorizedAccessException ex)
            {
                if (Config.createLOG)
                {
                    Logging.textLOG += $"\n{DateTime.Now:yyyy-MM-dd - HH:mm:ss}" +
                        $" | Error : Access to this folder is denied : {ex.Message} (3)";
                }
                if (!Config.backgroundLaunch)
                {
                    MessageBox.Show($"Error : Access to this folder is denied:\n{ex.Message}" +
                    $"\nMake sure to start the program with administrator priviledges",
                    "Error 208", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }//No finally cause it wont match the display's analysed files
        }

        //For maintanibility, sourcePath is necessary but not always used in the Action() method.
        internal async static Task Action(string action, string mainPath, string sourcePath)
        {
            //The actual creating and deleting
            {
                try
                {
                    switch (action)
                    {
                        case "copy":
                            if (Device.srcDevice != null && Device.dstDevice != null)
                            {//Making the synchronous method async to prevent blockages
                                await Task.Run(() =>
                                {
                                    Device.CopyFileMtpToMtpBuffered(
                                        sourcePath, mainPath, Device.srcDevice, Device.dstDevice);
                                });
                            }

                            else if (Device.srcDevice != null && Device.dstDevice == null)
                            {
                                await Task.Run(() =>
                                {
                                    Device.CopyFileFromMTP(sourcePath, mainPath, Device.srcDevice);
                                });
                            }

                            else if (Device.srcDevice == null && Device.dstDevice != null)
                            {
                                await Task.Run(() =>
                                {
                                    Device.CopyFileToMTP(sourcePath, mainPath, Device.dstDevice);
                                });
                            }

                            else
                            {
                                using FileStream sourceStream = new(
                                sourcePath, FileMode.Open, FileAccess.Read, FileShare.Read,
                                bufferSize: 81920, useAsync: true);

                                using FileStream destinationStream = new(
                                mainPath, FileMode.Create, FileAccess.Write, FileShare.None,
                                bufferSize: 81920, useAsync: true);

                                await sourceStream.CopyToAsync(destinationStream);
                                await destinationStream.FlushAsync();

                                //Give all of the attributes except read-only (bit operation &)
                                //Because setting a new timestamp will error out
                                FileAttributes srcAttrs = File.GetAttributes(sourcePath);
                                File.SetAttributes(mainPath, srcAttrs & ~FileAttributes.ReadOnly);

                                File.SetCreationTimeUtc(mainPath, File.GetCreationTimeUtc(sourcePath));
                                File.SetLastWriteTimeUtc(mainPath, File.GetLastWriteTimeUtc(sourcePath));
                                File.SetLastAccessTimeUtc(mainPath, File.GetLastAccessTimeUtc(sourcePath));
                            }
                            break;

                        case "delete":
                            if (Device.dstDevice != null)//Called by Analyse(), dstDevice = null
                                Device.DeleteFile(mainPath, Device.dstDevice);
                            else
                                File.Delete(mainPath);
                            break;

                        case "create":
                            if (Device.dstDevice != null)
                                Device.CreateFolder(mainPath, Device.dstDevice);

                            else if (Device.srcDevice == null && Device.dstDevice == null)
                            {

                                Directory.CreateDirectory(mainPath);

                                FileAttributes srcAttrs = File.GetAttributes(sourcePath);
                                File.SetAttributes(mainPath, srcAttrs & ~FileAttributes.ReadOnly);

                                Directory.SetCreationTimeUtc(mainPath, Directory.GetCreationTimeUtc(sourcePath));
                                Directory.SetLastWriteTimeUtc(mainPath, Directory.GetLastWriteTimeUtc(sourcePath));
                                Directory.SetLastAccessTimeUtc(mainPath, Directory.GetLastAccessTimeUtc(sourcePath));
                            }
                            else
                            {
                                Directory.CreateDirectory(mainPath);
                                //src can be on a MediaDevice, wont bother
                            }
                            break;

                        case "remove":
                            if (Device.dstDevice != null)
                                Device.DeleteFolder(mainPath, Device.dstDevice);
                            else
                                Directory.Delete(mainPath, true);
                            break;
                    }
                }
                catch (Exception ex) when (ex is FileNotFoundException || ex is DirectoryNotFoundException)
                {
                    if (Config.createLOG)
                    {
                        Logging.textLOG += $"\n{DateTime.Now:yyyy-MM-dd - HH:mm:ss}" +
                            $" | Error : file/directory cannot be found : {ex.Message} (1)";
                    }
                    if (!Config.backgroundLaunch)
                    {
                        MessageBox.Show($"Error : file/directory cannot be found : {ex.Message}",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (IOException ex)
                {
                    if (ex.HResult == -2147024864) // ERROR_SHARING_VIOLATION
                    {
                        if (Config.createLOG)
                            Logging.textLOG += $"\n{DateTime.Now:yyyy-MM-dd - HH:mm:ss}" +
                                $" | Error : A process is locking this file : {ex.Message} (2)";

                        if (!Config.backgroundLaunch)
                            MessageBox.Show($"Warning : Another process is currently " +
                                $"using this file. It has been skipped : {ex.Message}",
                            "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    else
                    {
                        if (Config.createLOG)
                            Logging.textLOG += $"\n{DateTime.Now:yyyy-MM-dd - HH:mm:ss}" +
                                $" | Error related to I/O : {ex.Message} (2)";

                        if (!Config.backgroundLaunch)
                            MessageBox.Show($"Error related to I/O : {ex.Message}",
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (UnauthorizedAccessException ex)
                {
                    if (Config.createLOG)
                    {
                        Logging.textLOG += $"\n{DateTime.Now:yyyy-MM-dd - HH:mm:ss}" +
                            $" | Error : Access to this file or directory is denied : {ex.Message} (3)";
                    }
                    if (!Config.backgroundLaunch)
                    {
                        MessageBox.Show($"Error : Access to this file or directory is denied:\n{ex.Message}" +
                        $"\nMake sure to start the program with administrator priviledges",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    if (Config.createLOG)
                    {
                        Logging.textLOG += $"\n{DateTime.Now:yyyy-MM-dd - HH:mm:ss}" +
                            $" | Error : {ex.Message} (4)";
                    }
                    if (!Config.backgroundLaunch)
                    {
                        MessageBox.Show($"Unknown error : {ex.Message}",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
    #endregion Global Device Operation Section
        #region Path Translation Layer Section

        internal static List<string> EnumerateFolders(string rootPath, MediaDevice? currentDevice, bool recursive = false)
        //Only used in DisplayProgression if the device is MTP. If not, a manual high-tech count is done.
        {
            List<string> folders = [];

            if (currentDevice == null)//I think its unused
                folders = [.. Directory.GetDirectories(rootPath)];
            else
            {
                if (!currentDevice.DirectoryExists(rootPath))
                    return folders;

                foreach (string? dir in currentDevice.GetDirectories(rootPath))
                {
                    folders.Add(dir);

                    if (recursive)
                    {
                        List<string> subFolders = EnumerateFolders(dir, currentDevice, true);
                        folders.AddRange(subFolders);
                    }
                }
            }
            return folders;
        }

        internal static List<string> EnumerateFiles(string rootPath, MediaDevice? currentDevice)
        {
            List<string> files = [];

            if (currentDevice == null)
                files = [.. Directory.GetFiles(rootPath)];
            else
            {
                if (!currentDevice.DirectoryExists(rootPath))
                    return files;

                foreach (string? file in currentDevice.GetFiles(rootPath))
                {
                    files.Add(file);
                }
            }
            return files;
        }

        internal static bool PathExists(string path, bool isFile, MediaDevice? currentDevice)
        {
            if (currentDevice == null)
            {
                if (isFile)
                    return File.Exists(path);
                else
                    return Directory.Exists(path);
            }
            else
            {
                if (isFile)
                    return currentDevice.FileExists(path);
                else
                    return currentDevice.DirectoryExists(path);
            }
        }

        internal static string Combine(string subPath, string folder, MediaDevice? currentDevice)
        {
            if (string.IsNullOrEmpty(folder))
                return subPath;

            return currentDevice == null
                ? Path.Combine(subPath, folder)
                : $"{subPath.TrimEnd('\\')}\\{folder}";
        }

        internal static string GetRelativePath(string basePath, string fullPath, MediaDevice? currentDevice)
        {
            if (currentDevice == null)
            {
                if (String.IsNullOrWhiteSpace(fullPath))
                    return Path.GetFileName(basePath);

                else if (basePath.Length == fullPath.Length)
                    return "";
                //At the same path level, ignore and add an exception at Combine()

                else
                    return Path.GetRelativePath(basePath, fullPath);
            }

            else
            {
                if (String.IsNullOrWhiteSpace(fullPath))
                    //MTP paths always ends with \\ and are not located on this device,
                    //Path.method like Path.GetFileName may not work if they are on Linux
                    //since they expect "/" as a delimiter but recieves "\\"
                    return Path.GetFileName(basePath); // \Card\DCIM\Camera -> Camera,

                else if (basePath.Length == fullPath.Length)
                    return "";

                else
                    // The [index..] syntax means "from this index to the end of the string" !
                    return fullPath[(basePath + "\\").Length..];
            }       // \Card\DCIM\Camera -> \DCIM\Camera
        }
    }
    #endregion Path Translation Layer Section
}