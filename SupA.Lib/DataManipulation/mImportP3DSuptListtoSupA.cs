namespace SupA.Lib.DataManipulation
{
    public class mImportP3DSuptListtoSupA
    {
        public static void ImportP3DSuptListtoSupA(string folderPath, string filenameIn, string filenameOut)
        {
            string oldName = Path.Combine(folderPath, filenameIn + ".csv");
            string newName = Path.Combine(folderPath, filenameOut + ".csv");

            File.Copy(oldName, newName);
        }
    }
}
