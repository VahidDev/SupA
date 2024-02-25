using SupA.Lib.Core;
using SupA.Lib.Models;

namespace SupA.Lib.DataManipulation
{
    public class mImportNavisClashtoSupA
    {
        public static void ImportNavisClashtoSupA(string folderPath, string filenameIn, string filenameOut)
        {
            string oldName = Path.Combine(folderPath, filenameIn + ".csv");
            string newName = Path.Combine(folderPath, filenameOut + ".csv");

            File.Copy(oldName, newName);

            // Declaring variables
            List<string[]> arrNavisClash = new List<string[]>();
            string navisClashRow;
            cClashData clashImport;
            List<cClashData> collSupAClashImport = new List<cClashData>();
            int eCounter, nCounter, uCounter;

            // Importing CSV file into an array
            using (StreamReader reader = new StreamReader(newName))
            {
                while ((navisClashRow = reader.ReadLine()) != null)
                {
                    arrNavisClash.Add(navisClashRow.Split(','));
                }
            }

            // Importing headers
            clashImport = returnSupAClashImportFormatLineOne();
            collSupAClashImport.Add(clashImport);

            // Looping through all lines in the CSV file
            foreach (string[] navisClashRowArray in arrNavisClash)
            {
                clashImport = returnSupAClashImportFormat(navisClashRowArray);
                collSupAClashImport.Add(clashImport);
            }

            // Exporting steel to CSV file
            mExportColltoCSVFile<cClashData>.ExportColltoCSVFile(collSupAClashImport, filenameOut, "csv", "writeall");
        }

        static cClashData returnSupAClashImportFormat(string[] navisClashRow)
        {
            cClashData clashImport = new cClashData();

            // Copying fields with a 1:1 relationship
            clashImport.EastingClashBoxCentre = float.Parse(navisClashRow[0]);
            clashImport.NorthingClashBoxCentre = float.Parse(navisClashRow[1]);
            clashImport.UppingClashBoxCentre = float.Parse(navisClashRow[2]);
            clashImport.ClashBoxSize = float.Parse(navisClashRow[3]);
            clashImport.ElesWithinClashBox = "";
            clashImport.TypesWithinClashBox = "";
            clashImport.DiscsWithinClashBox = "";

            return clashImport;
        }

        static cClashData returnSupAClashImportFormatLineOne()
        {
            cClashData clashImport = new cClashData();

            clashImport.EastingClashBoxCentre = 0; //"EastingClashBoxCentre";
            clashImport.NorthingClashBoxCentre = 1; //"NorthingClashBoxCentre";
            clashImport.UppingClashBoxCentre = 2; //"UppingClashBoxCentre";
            clashImport.ClashBoxSize = 3; //"ClashBoxSize";
            clashImport.ElesWithinClashBox = "ElesWithinClashBox";
            clashImport.TypesWithinClashBox = "TypesWithinClashBox";
            clashImport.DiscsWithinClashBox = "DiscsWithinClashBox";

            return clashImport;
        }
    }
}
