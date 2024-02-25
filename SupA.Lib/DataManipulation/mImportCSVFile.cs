namespace SupA.Lib.DataManipulation
{
    public class mImportCSVFile
    {
        public static string[,] ImportCSVFile(string folderPath, string filename, string fileExtension, string variableSeparator)
        {
            // Define function-specific private variables
            string line;
            int txtFileRowCount = 0;
            int txtFileColCount = 0;
            int lineNumberFile = 1;
            int lineNumberArray = 0;

            // Set complete file path
            string completeFilePath = Path.Combine(folderPath, filename + fileExtension);

            // Open the text file, check its length, and redefine ArrElements accordingly
            using (StreamReader reader = new StreamReader(completeFilePath))
            {
                // Check the length of the file
                while ((line = reader.ReadLine()) != null)
                {
                    txtFileRowCount++;
                }
            }

            // Adjusting the row count to exclude header
            txtFileRowCount--;

            // Check the width of the file
            using (StreamReader reader = new StreamReader(completeFilePath))
            {
                if ((line = reader.ReadLine()) != null)
                {
                    string[] arrElementsRow = line.Split(',');
                    txtFileColCount = arrElementsRow.Length;
                }
            }

            string[,] arrElements = new string[txtFileRowCount, txtFileColCount];

            // Fill out ArrElements
            using (StreamReader reader = new StreamReader(completeFilePath))
            {
                while ((line = reader.ReadLine()) != null)
                {
                    if (lineNumberFile != 1)
                    {
                        string[] arrElementsRow = line.Split(new string[] { variableSeparator }, StringSplitOptions.None);
                        for (int elementNumber = 0; elementNumber < txtFileColCount; elementNumber++)
                        {
                            arrElements[lineNumberArray, elementNumber] = arrElementsRow[elementNumber];
                        }
                    }
                    lineNumberFile++;
                    lineNumberArray = lineNumberFile - 2;
                }
            }

            return arrElements;
        }
    }
}
