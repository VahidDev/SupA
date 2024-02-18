namespace SupA.Lib.Models
{
    // Use PascalCase for class names
    public class CClashData
    {
        // Fields should be private and use camelCase
        private float eastingClashBoxCentre;
        private float northingClashBoxCentre;
        private float uppingClashBoxCentre;
        private float clashBoxSize;
        private string elesWithinClashBox;
        private string typesWithinClashBox;
        private string discsWithinClashBox;

        // Let Properties
        public void ReadAll(object[] arrToRead)
        {
            eastingClashBoxCentre = Convert.ToSingle(arrToRead[0]);
            northingClashBoxCentre = Convert.ToSingle(arrToRead[1]);
            uppingClashBoxCentre = Convert.ToSingle(arrToRead[2]);
            clashBoxSize = Convert.ToSingle(arrToRead[3]);
            elesWithinClashBox = Convert.ToString(arrToRead[4]);
            typesWithinClashBox = Convert.ToString(arrToRead[5]);
            discsWithinClashBox = Convert.ToString(arrToRead[6]);
        }

        // WriteAll method
        public string WriteAll(float rowNo = default)
        {
            return $"{eastingClashBoxCentre},{northingClashBoxCentre},{uppingClashBoxCentre},{clashBoxSize},{elesWithinClashBox},{typesWithinClashBox},{discsWithinClashBox}";
        }

        // Get Properties
        public float EastingClashBoxCentre => eastingClashBoxCentre;
        public float NorthingClashBoxCentre => northingClashBoxCentre;
        public float UppingClashBoxCentre => uppingClashBoxCentre;
        public float ClashBoxSize => clashBoxSize;
        public string ElesWithinClashBox => elesWithinClashBox;
        public string TypesWithinClashBox => typesWithinClashBox;
        public string DiscsWithinClashBox => discsWithinClashBox;

        // Let Properties
        public float EastingClashBoxCentreProp { set => eastingClashBoxCentre = value; }
        public float NorthingClashBoxCentreProp { set => northingClashBoxCentre = value; }
        public float UppingClashBoxCentreProp { set => uppingClashBoxCentre = value; }
        public float ClashBoxSizeProp { set => clashBoxSize = value; }
        public string ElesWithinClashBoxProp { set => elesWithinClashBox = value; }
        public string TypesWithinClashBoxProp { set => typesWithinClashBox = value; }
        public string DiscsWithinClashBoxProp { set => discsWithinClashBox = value; }
    }
}
