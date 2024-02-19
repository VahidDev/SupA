namespace SupA.Lib.DataManipulation
{
    public class mArchiveRun
    {
        public static void ArchiveRun(string runName, string str3DEnv)
        {
            int runNumber = 0;
            string runFolderPath = "YourFolderPathHere";

            // Import ActivityLog from the previous run
            string[] preRunActivityLog = File.ReadAllLines(Path.Combine(runFolderPath, "ActivityLog.csv"));
            DateTime runDate = DateTime.Parse(preRunActivityLog[1].Split(',')[1]);
            string runFolderName = $"{runDate.Year}-{runDate.Month}-{runDate.Day}-Run{runNumber}";

            // Find the first unused run number for a given date
            while (Directory.Exists(Path.Combine(runFolderPath, "RunArchive", runFolderName)))
            {
                runNumber++;
                runFolderName = $"{runDate.Year}-{runDate.Month}-{runDate.Day}-Run{runNumber}";
            }

            // Create the archive folder
            string archiveFolderPath = Path.Combine(runFolderPath, "RunArchive", runFolderName);
            Directory.CreateDirectory(archiveFolderPath);

            // Copy relevant folders to the archive
            CopyFolder(Path.Combine(runFolderPath, "3DOutSupAIn"), Path.Combine(archiveFolderPath, "3DOutSupAIn"));
            CopyFolder(Path.Combine(runFolderPath, "SupAOutput"), Path.Combine(archiveFolderPath, "SupAOutput"));
            CopyFolder(Path.Combine(runFolderPath, "VBATrace"), Path.Combine(archiveFolderPath, "VBATrace"));

            // Delete original folders
            Directory.Delete(Path.Combine(runFolderPath, "3DOutSupAIn"), true);
            Directory.Delete(Path.Combine(runFolderPath, "SupAOutput"), true);
            Directory.Delete(Path.Combine(runFolderPath, "VBATrace"), true);

            // Recreate necessary empty folders
            Directory.CreateDirectory(Path.Combine(runFolderPath, "3DOutSupAIn"));
            Directory.CreateDirectory(Path.Combine(runFolderPath, "3DOutSupAIn", "FrameCreationMode"));
            Directory.CreateDirectory(Path.Combine(runFolderPath, "3DOutSupAIn", "SuptPointSelMode"));
            Directory.CreateDirectory(Path.Combine(runFolderPath, "3DOutSupAIn", "Temp"));
            Directory.CreateDirectory(Path.Combine(runFolderPath, "SupAOutput"));
            Directory.CreateDirectory(Path.Combine(runFolderPath, "VBATrace"));

            if (str3DEnv == "P3D")
            {
                // Keep the P3D SDNF export files
                string sourceFile = Path.Combine(archiveFolderPath, "3DOutSupAIn", "FrameCreationMode", "P3D-ExistingSteelData.csv");
                string destinationFile = Path.Combine(runFolderPath, "3DOutSupAIn", "FrameCreationMode", "P3D-ExistingSteelData.csv");
                File.Copy(sourceFile, destinationFile, true);

                sourceFile = Path.Combine(archiveFolderPath, "3DOutSupAIn", "SuptPointSelMode", "P3D-ExistingSteelData.csv");
                destinationFile = Path.Combine(runFolderPath, "3DOutSupAIn", "SuptPointSelMode", "P3D-ExistingSteelData.csv");
                File.Copy(sourceFile, destinationFile, true);
            }
        }

        private static void CopyFolder(string sourceFolder, string destinationFolder)
        {
            if (!Directory.Exists(destinationFolder))
            {
                Directory.CreateDirectory(destinationFolder);
            }

            string[] files = Directory.GetFiles(sourceFolder);
            foreach (string file in files)
            {
                string fileName = Path.GetFileName(file);
                string destinationFile = Path.Combine(destinationFolder, fileName);
                File.Copy(file, destinationFile, true);
            }

            string[] subFolders = Directory.GetDirectories(sourceFolder);
            foreach (string subFolder in subFolders)
            {
                string folderName = Path.GetFileName(subFolder);
                string destinationSubFolder = Path.Combine(destinationFolder, folderName);
                CopyFolder(subFolder, destinationSubFolder);
            }
        }
    }
}
