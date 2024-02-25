using Microsoft.VisualBasic;
using SupA.Lib.Initialization;

namespace SupA.Lib.DataManipulation
{
    public class mExportArrtoCSVFile
    {
        public static void ExportArrtoCSVFile(int[,] arr, string filename, string fileExtension, bool stdSupAOutput = false, string overRideFolderPath = "")
        {
            // Set the folder and file name
            string myfile;
            string printstring;
            int I, J;
            int dimnum = 0;

            try
            {
                // Determine the number of dimensions in the array
                for (dimnum = 1; dimnum <= 60; dimnum++) // 60 being the absolute dimensions limitation
                {
                    int errorCheck = arr.GetLowerBound(dimnum);
                }
                dimnum--;
            }
            catch { }

            if (!string.IsNullOrEmpty(overRideFolderPath))
                myfile = overRideFolderPath + filename + "." + fileExtension;
            else if (stdSupAOutput)
                myfile = mSubInitializationSupA.pubstrFolderPath + "SupAOutput\\" + filename + "." + fileExtension;
            else
                myfile = mSubInitializationSupA.pubstrFolderPath + "VBATrace\\" + filename + "." + fileExtension;

            // Open the file for writing
            using (StreamWriter writer = new StreamWriter(myfile))
            {
                if (dimnum == 1)
                {
                    for (I = 0; I < arr.GetLength(0); I++)
                    {
                        printstring = arr[I, 0].ToString();
                        writer.WriteLine(printstring);
                    }
                }
                else if (dimnum == 2)
                {
                    for (I = 0; I < arr.GetLength(0); I++)
                    {
                        printstring = arr[I, 0].ToString();
                        for (J = 1; J < arr.GetLength(1); J++)
                        {
                            printstring += "," + arr[I, J];
                        }
                        writer.WriteLine(printstring);
                    }
                }
                else
                {
                    // Error in exportarrtocsv
                    Interaction.MsgBox("Error in exportarrtocsv");
                }
            }
        }
    }
}
