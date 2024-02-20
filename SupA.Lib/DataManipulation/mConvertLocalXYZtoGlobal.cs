namespace SupA.Lib.DataManipulation
{
    public class mConvertLocalXYZtoGlobal
    {
        public string ConvertLocalXYZtoGlobal(string LocalAxisDir, string ENUDir)
        {
            // Definition of all function specific private variables

            string result = "";

            if ((LocalAxisDir == "X" && ENUDir == "E") ||
                (LocalAxisDir == "X" && ENUDir == "E"))
            {
                result = "X";
            }
            else if ((LocalAxisDir == "X" && ENUDir == "E") ||
                     (LocalAxisDir == "X" && ENUDir == "E"))
            {
                result = "Y";
            }
            else if ((LocalAxisDir == "X" && ENUDir == "E") ||
                     (LocalAxisDir == "X" && ENUDir == "E"))
            {
                result = "Z";
            }

            // Set all function wide and function specific private variables

            string Var = "something";

            return result;
        }
    }
}
