using SupA.Lib.Core;
using SupA.Lib.Models;

namespace SupA.Lib.DataManipulation
{
    public class mImportCSVFiletoColl<T> where T : class
    {
        public static List<T> ImportCSVFiletoColl(string folderPath, string filename, string fileExtension, string variableSeparator, dynamic classVarToCreate, string classType)
        {
            string line;
            string[] arrElementsRow;
            string completeFilePath = Path.Combine(folderPath, filename + fileExtension);
            int txtFileColCount;
            int lineNumberFile = 0;
            var collToReturn = new List<T>();

            // Open file for input and check width of file
            using (StreamReader reader = new StreamReader(completeFilePath))
            {
                // Read the header row to determine the number of columns
                line = reader.ReadLine();
                arrElementsRow = line.Split(',');
                txtFileColCount = arrElementsRow.Length;

                // Reset the reader to start from the beginning
                reader.BaseStream.Seek(0, SeekOrigin.Begin);
                reader.DiscardBufferedData();

                while ((line = reader.ReadLine()) != null)
                {
                    lineNumberFile++;

                    // Skip processing if it's the header row
                    if (lineNumberFile != 1)
                    {
                        arrElementsRow = line.Split(variableSeparator);

                        // Instantiate object based on classType
                        if (classType == "cSuptPoints")
                        {
                            classVarToCreate = new cSuptPoints();
                        }
                        else if (classType == "cSteelDisc")
                        {
                            classVarToCreate = new cSteelDisc();
                        }
                        else if (classType == "cSteel")
                        {
                            classVarToCreate = new cSteel();
                        }
                        else if (classType == "cClashData")
                        {
                            classVarToCreate = new cClashData();
                        }
                        else if (classType == "cTubeDef")
                        {
                            classVarToCreate = new cTubeDef();
                        }
                        else if (classType.ToLower() == "string")
                        {
                            // No need to create an object for string type
                        }
                        else
                        {
                            throw new Exception("The definition of ImportCSVFiletoColl needs to be updated to include class " + classType);
                        }

                        // Add object to the collection
                        if (classType.ToLower() != "string")
                        {
                            classVarToCreate.ReadAll(arrElementsRow);
                            collToReturn.Add(classVarToCreate);
                        }
                        else
                        {
                            (collToReturn as List<string>).Add(arrElementsRow[0]);
                        }
                    }
                }
            }

            return collToReturn;
        }
    }
}
