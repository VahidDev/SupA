namespace SupA.Lib.Core
{
    public class cTubeDef
    {
        // Fields
        private string pTubeName;
        private string pTubeType;
        private string pComponentRef;
        private float pABor;
        private string pADir;
        private float pAEast;
        private float pANorth;
        private float pAUpping;
        private float pLBor;
        private string pLDir;
        private float pLEast;
        private float pLNorth;
        private float pLUpping;
        private float pTubeLength;
        private string pDiscClassification;
        private string pNameofPipe;
        private string pFlagConnBraAtStart;
        private string pFlagConnBraAtEnd;
        private float pConnBraAtStartBor;
        private float pConnBraAtEndBor;
        private float pNoOfEstSuptsReqdonTube;

        // Property for reading and writing all properties
        public object ReadAll
        {
            set
            {
                var arrToRead = (object[])value;
                pTubeName = (string)arrToRead[0];
                pTubeType = (string)arrToRead[1];
                pComponentRef = (string)arrToRead[2];
                pABor = Convert.ToSingle(arrToRead[3]);
                pADir = (string)arrToRead[4];
                pAEast = Convert.ToSingle(arrToRead[5]);
                pANorth = Convert.ToSingle(arrToRead[6]);
                pAUpping = Convert.ToSingle(arrToRead[7]);
                pLBor = Convert.ToSingle(arrToRead[8]);
                pLDir = (string)arrToRead[9];
                pLEast = Convert.ToSingle(arrToRead[10]);
                pLNorth = Convert.ToSingle(arrToRead[11]);
                pLUpping = Convert.ToSingle(arrToRead[12]);
                pTubeLength = Convert.ToSingle(arrToRead[13]);
                pDiscClassification = (string)arrToRead[14];
                pNameofPipe = (string)arrToRead[15];
                pFlagConnBraAtStart = (string)arrToRead[16];
                pFlagConnBraAtEnd = (string)arrToRead[17];
                pConnBraAtStartBor = Convert.ToSingle(arrToRead[18]);
                pConnBraAtEndBor = Convert.ToSingle(arrToRead[19]);
            }
        }

        public string WriteAll(float rowNo = 0)
        {
            return $"{pTubeName},{pTubeType},{pComponentRef},{pABor},{pADir},{pAEast},{pANorth},{pAUpping},{pLBor},{pLDir}," +
                   $"{pLEast},{pLNorth},{pLUpping},{pTubeLength},{pDiscClassification},{pNameofPipe},{pFlagConnBraAtStart}," +
                   $"{pFlagConnBraAtEnd},{pConnBraAtStartBor},{pConnBraAtEndBor},{pNoOfEstSuptsReqdonTube}";
        }

        // Get Properties
        public string TubeName => pTubeName;
        public string TubeType => pTubeType;
        public string ComponentRef => pComponentRef;
        public float ABor => pABor;
        public string ADir => pADir;
        public float AEast => pAEast;
        public float ANorth => pANorth;
        public float AUpping => pAUpping;
        public float LBor => pLBor;
        public string LDir => pLDir;
        public float LEast => pLEast;
        public float LNorth => pLNorth;
        public float LUpping => pLUpping;
        public float TubeLength => pTubeLength;
        public string DiscClassification => pDiscClassification;
        public string NameofPipe => pNameofPipe;
        public string FlagConnBraAtStart => pFlagConnBraAtStart;
        public string FlagConnBraAtEnd => pFlagConnBraAtEnd;
        public float ConnBraAtStartBor => pConnBraAtStartBor;
        public float ConnBraAtEndBor => pConnBraAtEndBor;
        public float NoOfEstSuptsReqdonTube => pNoOfEstSuptsReqdonTube;

        // Let Properties
        public string TubeNameProperty
        {
            set => pTubeName = value;
        }
        public string TubeTypeProperty
        {
            set => pTubeType = value;
        }
        public string ComponentRefProperty
        {
            set => pComponentRef = value;
        }
        public float ABorProperty
        {
            set => pABor = value;
        }
        public string ADirProperty
        {
            set => pADir = value;
        }
        public float AEastProperty
        {
            set => pAEast = value;
        }
        public float ANorthProperty
        {
            set => pANorth = value;
        }
        public float AUppingProperty
        {
            set => pAUpping = value;
        }
        public float LBorProperty
        {
            set => pLBor = value;
        }
        public string LDirProperty
        {
            set => pLDir = value;
        }
        public float LEastProperty
        {
            set => pLEast = value;
        }
        public float LNorthProperty
        {
            set => pLNorth = value;
        }
        public float LUppingProperty
        {
            set => pLUpping = value;
        }
        public float TubeLengthProperty
        {
            set => pTubeLength = value;
        }
        public string DiscClassificationProperty
        {
            set => pDiscClassification = value;
        }
        public string NameofPipeProperty
        {
            set => pNameofPipe = value;
        }
        public string FlagConnBraAtStartProperty
        {
            set => pFlagConnBraAtStart = value;
        }
        public string FlagConnBraAtEndProperty
        {
            set => pFlagConnBraAtEnd = value;
        }
        public float ConnBraAtStartBorProperty
        {
            set => pConnBraAtStartBor = value;
        }
        public float ConnBraAtEndBorProperty
        {
            set => pConnBraAtEndBor = value;
        }
        public float NoOfEstSuptsReqdonTubeProperty
        {
            set => pNoOfEstSuptsReqdonTube = value;
        }
    }
}
