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
        public string TubeName => pTubeName;
        public string TubeType => pTubeType;
        public string ComponentRef => pComponentRef;
        public float abor => pABor;
        public float lbor => pLBor;
        public string Dir => pDir;
        public float East => pEast;
        public float North => pNorth;
        public float Upping => pUpping;
        public float EastRounded => mPublicVarDefinitions.RoundDecPlc(pEast, mSubInitializationSupA.pubIntDiscretisationStepDecPlaces);
        public float NorthRounded => mPublicVarDefinitions.RoundDecPlc(pNorth, mSubInitializationSupA.pubIntDiscretisationStepDecPlaces);
        public float UppingRounded => mPublicVarDefinitions.RoundDecPlc(pUpping, mSubInitializationSupA.pubIntDiscretisationStepDecPlaces);
        public string DiscClassification => pDiscClassification;
        public string NameofPipe => pNameofPipe;
        public float HoriOffsetfromStlMin => pHoriOffsetfromStlMin;
        public float VertOffsetfromStlMin => pVertOffsetfromStlMin;
        public float SuptPointScore => pSuptPointScore;
        public int SuptPointScoreCat => pSuptPointScoreCat;
        public string SuptRanking => pSuptRanking;

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
        public float LBorProperty
        {
            set => pLBor = value;
        }
        public string DirProperty
        {
            set => pDir = value;
        }
        public float EastProperty
        {
            set => pEast = value;
        }
        public float NorthProperty
        {
            set => pNorth = value;
        }
        public float UppingProperty
        {
            set => pUpping = value;
        }
        public string DiscClassificationProperty
        {
            set => pDiscClassification = value;
        }
        public string NameofPipeProperty
        {
            set => pNameofPipe = value;
        }
        public float HoriOffsetfromStlMinProperty
        {
            set => pHoriOffsetfromStlMin = value;
        }
        public float VertOffsetfromStlMinProperty
        {
            set => pVertOffsetfromStlMin = value;
        }
        public float SuptPointScoreProperty
        {
            set => pSuptPointScore = value;
        }
        public int SuptPointScoreCatProperty
        {
            set => pSuptPointScoreCat = value;
        }
        public string SuptRankingProperty
        {
            set => pSuptRanking = value;
        }
    }
}
