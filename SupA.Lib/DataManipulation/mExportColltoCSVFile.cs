using SupA.Lib.Initialization;
using SupA.Lib.Utils;

namespace SupA.Lib.DataManipulation
{
    public class mExportColltoCSVFile<T> where T : class
    {
        public static void ExportColltoCSVFile(List<T> coll, string Filename, string FileExtension, string CallProperty = "", bool StdSupAOutput = false)
        {
            // Set the folder and file name
            string myfile;

            if (Filename == "ActivityLog")
                myfile = mSubInitializationSupA.pubstrFolderPath + Filename + "." + FileExtension;
            else if (Filename == "Area-ExistingSteelData")
                myfile = mSubInitializationSupA.pubstrFolderPath + "3DOutSupAIn\\" + Filename + "." + FileExtension;
            else if (Filename == "C:\\SupA\\Data\\STAAD\\test")
                myfile = "C:\\SupA\\Data\\STAAD\\test" + "." + FileExtension;
            else if (Filename.Contains("FrameCreationMode\\Area-ExistingSteelData", StringComparison.OrdinalIgnoreCase))
                myfile = mSubInitializationSupA.pubstrFolderPath + "3DOutSupAIn\\" + Filename + "." + FileExtension;
            else if (Filename.Contains("SuptPointSelMode\\Area-ExistingSteelData", StringComparison.OrdinalIgnoreCase))
                myfile = mSubInitializationSupA.pubstrFolderPath + "3DOutSupAIn\\" + Filename + "." + FileExtension;
            else if (StdSupAOutput)
                myfile = mSubInitializationSupA.pubstrFolderPath + "SupAOutput\\" + Filename + "." + FileExtension;
            else
                myfile = mSubInitializationSupA.pubstrFolderPath + "VBATrace\\" + Filename + "." + FileExtension;

            // Open the file for writing
            using (StreamWriter writer = new StreamWriter(myfile))
            {
                for (int MemC = 1; MemC <= coll.Count; MemC++)
                {
                    object Mem = coll[MemC];
                    string Printstring;

                    if (CallProperty == "String")
                        Printstring = Mem.ToString();
                    else
                    {
                        var propertyValue = (dynamic)VbaInterop.CallByName(Mem, CallProperty, VbCallType.Get, MemC);
                        Printstring = propertyValue ?? ""; // If propertyValue is null, assign an empty string
                    }

                    // Writing Printstring to the file
                    writer.WriteLine(Printstring);
                }
            }
        }
    }
}
