namespace SupA.Lib.Models
{
    // Use PascalCase for class names
    public class cClashData
    {

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

        private float eastingClashBoxCentre;
        public float EastingClashBoxCentre
        {
            get => eastingClashBoxCentre;
            set => eastingClashBoxCentre = value;
        }
        public float NorthingClashBoxCentre
        {
            get => northingClashBoxCentre;
            set => northingClashBoxCentre = value;
        }
        public float UppingClashBoxCentre
        {
            get => uppingClashBoxCentre;
            set => uppingClashBoxCentre = value;
        }
        public float ClashBoxSize
        {
            get => clashBoxSize;
            set => clashBoxSize = value;
        }
        public string DiscsWithinClashBox { get => discsWithinClashBox; set => discsWithinClashBox = value; }

        private float northingClashBoxCentre;
        public float NorthingClashBoxCentreProp
        {
            get => northingClashBoxCentre;
            set => northingClashBoxCentre = value;
        }

        private float uppingClashBoxCentre;
        public float UppingClashBoxCentreProp
        {
            get => uppingClashBoxCentre;
            set => uppingClashBoxCentre = value;
        }

        private float clashBoxSize;
        public float ClashBoxSizeProp
        {
            get => clashBoxSize;
            set => clashBoxSize = value;
        }

        private string elesWithinClashBox;
        public string ElesWithinClashBox
        {
            get => elesWithinClashBox;
            set => elesWithinClashBox = value;
        }

        private string typesWithinClashBox;
        public string TypesWithinClashBox
        {
            get => typesWithinClashBox;
            set => typesWithinClashBox = value;
        }

        private string discsWithinClashBox;
        public string DiscsWithinClashBoxProp
        {
            get => discsWithinClashBox;
            set => discsWithinClashBox = value;
        }

    }
}
