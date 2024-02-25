namespace SupA.Lib.DataManipulation
{
    public class mImportFlagTxt
    {
        public static string ImportFlagTxt(string folderPath, string filename, string fileExtension, string variableSeparator)
        {
            string line;
            string strFlagtoReturn = "";
            int elementNumber;
            object element;

            string completeFilePath = Path.Combine(folderPath, filename + fileExtension);

            using (StreamReader reader = new StreamReader(completeFilePath))
            {
                line = reader.ReadLine();
                strFlagtoReturn = line;
            }

            return strFlagtoReturn;
        }
    }
}
