using SupA.Lib.Initialization;

namespace SupA.Lib.Core
{
    public class cTubeDefDisc
    {
        // Fields
        private string pTubeName;
        private string pTubeType;
        private string pComponentRef;
        private float pABor;
        private float pLBor;
        private string pDir;
        private float pEast;
        private float pNorth;
        private float pUpping;
        private string pDiscClassification;
        private string pNameofPipe;
        private float pHoriOffsetfromStlMin;
        private float pVertOffsetfromStlMin;
        private float pSuptPointScore;
        private int pSuptPointScoreCat;
        private string pSuptRanking;

        // Property for writing all properties
        public string WriteAll(float rowNo = 0)
        {
            return $"{pTubeName},{pTubeType},{pComponentRef},{pABor},{pLBor},{pDir},{pEast},{pNorth},{pUpping}," +
                   $"{pDiscClassification},{pNameofPipe},{pHoriOffsetfromStlMin},{pVertOffsetfromStlMin}," +
                   $"{pSuptPointScore},{pSuptPointScoreCat},{pSuptRanking}";
        }

        // Get Properties
        public string ComponentRef => pComponentRef;
        public float EastRounded => mPublicVarDefinitions.RoundDecPlc(pEast, mSubInitializationSupA.pubIntDiscretisationStepDecPlaces);
        public float NorthRounded => mPublicVarDefinitions.RoundDecPlc(pNorth, mSubInitializationSupA.pubIntDiscretisationStepDecPlaces);
        public float UppingRounded => mPublicVarDefinitions.RoundDecPlc(pUpping, mSubInitializationSupA.pubIntDiscretisationStepDecPlaces);

        // Let Properties
        public void WritefromcTubeDef(cTubeDef value)
        {
            pTubeName = value.TubeName;
            pTubeType = value.TubeType;
            pComponentRef = value.ComponentRef;
            pABor = value.ABor;
            pLBor = value.LBor;
            pDir = value.ADir;
            pDiscClassification = value.DiscClassification;
            pNameofPipe = value.NameofPipe;
        }

        public string TubeName
        {
            set => pTubeName = value;
            get=> pNameofPipe;
        }
        public string TubeType
        {
            set => pTubeType = value;
            get => pTubeType;
        }
        public string ComponentRefProperty
        {
            set => pComponentRef = value;
        }
        public float ABor
        {
            set => pABor = value; get => pABor;
        }
        public float LBor
        {
            set => pLBor = value; get => pLBor;
        }
        public string Dir
        {
            set => pDir = value;
            get => pDir;
        }
        public float East
        {
            set => pEast = value;
            get => pEast;
        }
        public float North
        {
            set => pNorth = value; get => pNorth;
        }
        public float Upping
        {
            set => pUpping = value; get => pUpping;
        }
        public string DiscClassification
        {
            set => pDiscClassification = value; get => pDiscClassification;
        }
        public string NameofPipe
        {
            set => pNameofPipe = value;
            get => pNameofPipe;
        }
        public float HoriOffsetfromStlMin
        {
            set => pHoriOffsetfromStlMin = value;
            get => pHoriOffsetfromStlMin;
        }
        public float VertOffsetfromStlMin
        {
            set => pVertOffsetfromStlMin = value; get => pVertOffsetfromStlMin;
        }
        public float SuptPointScore
        {
            set => pSuptPointScore = value;
            get => pSuptPointScore;
        }
        public int SuptPointScoreCat
        {
            set => pSuptPointScoreCat = value;
            get => pSuptPointScoreCat;
        }
        public string SuptRanking
        {
            set => pSuptRanking = value;
            get => pSuptRanking;
        }
    }
}
