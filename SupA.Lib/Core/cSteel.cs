namespace SupA.Lib.Core
{
    public class cSteel
    {
        private float pBeamNo;
        private float pStartE;
        private float pStartN;
        private float pStartU;
        private float pEndE;
        private float pEndN;
        private float pEndU;
        private double pLength;
        private string pDir;
        private string pSuptNosonBeam;
        private string pPrelimSctnReqd;
        private float pSctnDepth;
        private float pSctnWidth;
        private int pLevelNo;
        private int pSuptBeamPerpDirFromPipeAxis;
        private string pExistingSteelConnNameStart;
        private string pExistingSteelConnNameEnd;
        private bool pClashonBeam;
        private string pSuptSteelFunction;
        private string pModelName;
        private string pOwningDisc;
        private string pMemTypeModelRef;
        private string pMemType;
        private string pMemTypeGeneric;
        private string pMemModelParam;
        private string pJusline;
        private string pBangle;
        private string pOri;
        private string pMinorAxisGlobaldir;
        private string pMajorAxisGlobaldir;
        private float pStartERounded;
        private float pStartNRounded;
        private float pStartURounded;
        private float pEndERounded;
        private float pEndNRounded;
        private float pEndURounded;
        private string pFeatureDesc;
        private float pUnitWeightKgm;
        private string pStartConnStlDir;
        private string pEndConnStlDir;
        private string pExistingConnType;
        private string pConnDetailing;
        private string pSTAADSctnName;

        public float BeamNo { get => pBeamNo; set => pBeamNo = value; }
        public float StartE { get => pStartE; set => pStartE = value; }
        public float StartN { get => pStartN; set => pStartN = value; }
        public float StartU { get => pStartU; set => pStartU = value; }
        public float EndE { get => pEndE; set => pEndE = value; }
        public float EndN { get => pEndN; set => pEndN = value; }
        public float EndU { get => pEndU; set => pEndU = value; }
        public double Length { get => pLength; set => pLength = value; }
        public string Dir;
        public string SuptNosonBeam { get => pSuptNosonBeam; set => pSuptNosonBeam = value; }
        public string PrelimSctnReqd { get => pPrelimSctnReqd; set => pPrelimSctnReqd = value; }
        public float SctnDepth { get => pSctnDepth; set => pSctnDepth = value; }
        public float SctnWidth { get => pSctnWidth; set => pSctnWidth = value; }
        public int LevelNo { get => pLevelNo; set => pLevelNo = value; }
        public int SuptBeamPerpDirFromPipeAxis { get => pSuptBeamPerpDirFromPipeAxis; set => pSuptBeamPerpDirFromPipeAxis = value; }
        public string ExistingSteelConnNameStart { get => pExistingSteelConnNameStart; set => pExistingSteelConnNameStart = value; }
        public string ExistingSteelConnNameEnd { get => pExistingSteelConnNameEnd; set => pExistingSteelConnNameEnd = value; }
        public bool ClashonBeam { get => pClashonBeam; set => pClashonBeam = value; }
        public string SuptSteelFunction { get => pSuptSteelFunction; set => pSuptSteelFunction = value; }
        public string ModelName { get => pModelName; set => pModelName = value; }
        public string OwningDisc { get => pOwningDisc; set => pOwningDisc = value; }
        public string MemTypeModelRef { get => pMemTypeModelRef; set => pMemTypeModelRef = value; }
        public string MemType { get => pMemType; set => pMemType = value; }
        public string MemTypeGeneric { get => pMemTypeGeneric; set => pMemTypeGeneric = value; }
        public string MemModelParam { get => pMemModelParam; set => pMemModelParam = value; }
        public string Jusline { get => pJusline; set => pJusline = value; }
        public string Bangle { get => pBangle; set => pBangle = value; }
        public string Ori { get => pOri; set => pOri = value; }
        public string MinorAxisGlobaldir;
        public string MajorAxisGlobaldir;
        public float StartERounded { get => pStartERounded; set => pStartERounded = value; }
        public float StartNRounded { get => pStartNRounded; set => pStartNRounded = value; }
        public float StartURounded { get => pStartURounded; set => pStartURounded = value; }
        public float EndERounded { get => pEndERounded; set => pEndERounded = value; }
        public float EndNRounded { get => pEndNRounded; set => pEndNRounded = value; }
        public float EndURounded { get => pEndURounded; set => pEndURounded = value; }
        public string FeatureDesc { get => pFeatureDesc; set => pFeatureDesc = value; }
        public float UnitWeightKgm { get => pUnitWeightKgm; set => pUnitWeightKgm = value; }
        public string StartConnStlDir { get => pStartConnStlDir; set => pStartConnStlDir = value; }
        public string EndConnStlDir { get => pEndConnStlDir; set => pEndConnStlDir = value; }
        public string ExistingConnType { get => pExistingConnType; set => pExistingConnType = value; }
        public string ConnDetailing { get => pConnDetailing; set => pConnDetailing = value; }
        public string STAADSctnName { get => pSTAADSctnName; set => pSTAADSctnName = value; }

        public void ReadAll(object[] arrtoRead)
        {
            pBeamNo = (float)arrtoRead[0];
            pStartE = (float)arrtoRead[1];
            pStartN = (float)arrtoRead[2];
            pStartU = (float)arrtoRead[3];
            pEndE = (float)arrtoRead[4];
            pEndN = (float)arrtoRead[5];
            pEndU = (float)arrtoRead[6];
            pLength = (float)arrtoRead[7];
            pDir = (string)arrtoRead[8];
            pSuptNosonBeam = (string)arrtoRead[9];
            pPrelimSctnReqd = (string)arrtoRead[10];
            pSctnDepth = (float)arrtoRead[11];
            pSctnWidth = (float)arrtoRead[12];
            pLevelNo = (int)arrtoRead[13];
            pSuptBeamPerpDirFromPipeAxis = (int)arrtoRead[14];
            pExistingSteelConnNameStart = (string)arrtoRead[15];
            pExistingSteelConnNameEnd = (string)arrtoRead[16];
            pClashonBeam = (bool)arrtoRead[17];
            pSuptSteelFunction = (string)arrtoRead[18];
            pModelName = (string)arrtoRead[19];
            pOwningDisc = (string)arrtoRead[20];
            pMemTypeModelRef = (string)arrtoRead[21];
            pMemType = (string)arrtoRead[22];
            pMemTypeGeneric = (string)arrtoRead[23];
            pMemModelParam = (string)arrtoRead[24];
            pJusline = (string)arrtoRead[25];
            pBangle = (string)arrtoRead[26];
            pOri = (string)arrtoRead[27];
            pMinorAxisGlobaldir = (string)arrtoRead[28];
            pMajorAxisGlobaldir = (string)arrtoRead[29];
            pStartERounded = (float)arrtoRead[30];
            pStartNRounded = (float)arrtoRead[31];
            pStartURounded = (float)arrtoRead[32];
            pEndERounded = (float)arrtoRead[33];
            pEndNRounded = (float)arrtoRead[34];
            pEndURounded = (float)arrtoRead[35];
            pFeatureDesc = (string)arrtoRead[36];
            pUnitWeightKgm = (float)arrtoRead[37];
            pExistingConnType = (string)arrtoRead[38];
            pConnDetailing = (string)arrtoRead[39];
            pSTAADSctnName = (string)arrtoRead[40];
        }

        public void CopyClassInstance(cSteel instanceToCopy)
        {
            pBeamNo = instanceToCopy.BeamNo;
            pStartE = instanceToCopy.StartE;
            pStartN = instanceToCopy.StartN;
            pStartU = instanceToCopy.StartU;
            pEndE = instanceToCopy.EndE;
            pEndN = instanceToCopy.EndN;
            pEndU = instanceToCopy.EndU;
            pLength = instanceToCopy.Length;
            pDir = instanceToCopy.Dir;
            pSuptNosonBeam = instanceToCopy.SuptNosonBeam;
            pPrelimSctnReqd = instanceToCopy.PrelimSctnReqd;
            pSctnDepth = instanceToCopy.SctnDepth;
            pSctnWidth = instanceToCopy.SctnWidth;
            pLevelNo = instanceToCopy.LevelNo;
            pSuptBeamPerpDirFromPipeAxis = instanceToCopy.SuptBeamPerpDirFromPipeAxis;
            pExistingSteelConnNameStart = instanceToCopy.ExistingSteelConnNameStart;
            pExistingSteelConnNameEnd = instanceToCopy.ExistingSteelConnNameEnd;
            pClashonBeam = instanceToCopy.ClashonBeam;
            pSuptSteelFunction = instanceToCopy.SuptSteelFunction;
            pModelName = instanceToCopy.ModelName;
            pOwningDisc = instanceToCopy.OwningDisc;
            pMemTypeModelRef = instanceToCopy.MemTypeModelRef;
            pMemType = instanceToCopy.MemType;
            pMemTypeGeneric = instanceToCopy.MemTypeGeneric;
            pMemModelParam = instanceToCopy.MemModelParam;
            pJusline = instanceToCopy.Jusline;
            pBangle = instanceToCopy.Bangle;
            pOri = instanceToCopy.Ori;
            pMinorAxisGlobaldir = instanceToCopy.MinorAxisGlobaldir;
            pMajorAxisGlobaldir = instanceToCopy.MajorAxisGlobaldir;
            pStartERounded = instanceToCopy.StartERounded;
            pStartNRounded = instanceToCopy.StartNRounded;
            pStartURounded = instanceToCopy.StartURounded;
            pEndERounded = instanceToCopy.EndERounded;
            pEndNRounded = instanceToCopy.EndNRounded;
            pEndURounded = instanceToCopy.EndURounded;
            pFeatureDesc = instanceToCopy.FeatureDesc;
            pUnitWeightKgm = instanceToCopy.UnitWeightKgm;
            pStartConnStlDir = instanceToCopy.StartConnStlDir;
            pEndConnStlDir = instanceToCopy.EndConnStlDir;
            pExistingConnType = instanceToCopy.ExistingConnType;
            pConnDetailing = instanceToCopy.ConnDetailing;
            pSTAADSctnName = instanceToCopy.STAADSctnName;
        }

        public string WriteAll(float RowNo = 0)
        {
            string result = pBeamNo.ToString();
            result += "," + pStartE.ToString();
            result += "," + pStartN.ToString();
            result += "," + pStartU.ToString();
            result += "," + pEndE.ToString();
            result += "," + pEndN.ToString();
            result += "," + pEndU.ToString();
            result += "," + pLength.ToString();
            result += "," + pDir;
            result += "," + pSuptNosonBeam;
            result += "," + pPrelimSctnReqd;
            result += "," + pSctnDepth.ToString();
            result += "," + pSctnWidth.ToString();
            result += "," + pLevelNo.ToString();
            result += "," + pSuptBeamPerpDirFromPipeAxis.ToString();
            result += "," + pExistingSteelConnNameStart;
            result += "," + pExistingSteelConnNameEnd;
            result += "," + pClashonBeam.ToString();
            result += "," + pSuptSteelFunction;
            result += "," + pModelName;
            result += "," + pOwningDisc;
            result += "," + pMemTypeModelRef;
            result += "," + pMemType;
            result += "," + pMemTypeGeneric;
            result += "," + pMemModelParam;
            result += "," + pJusline;
            result += "," + pBangle;
            result += "," + pOri;
            result += "," + pMinorAxisGlobaldir;
            result += "," + pMajorAxisGlobaldir;
            result += "," + pStartERounded.ToString();
            result += "," + pStartNRounded.ToString();
            result += "," + pStartURounded.ToString();
            result += "," + pEndERounded.ToString();
            result += "," + pEndNRounded.ToString();
            result += "," + pEndURounded.ToString();
            result += "," + pFeatureDesc;
            result += "," + pUnitWeightKgm.ToString();
            result += "," + pStartConnStlDir;
            result += "," + pEndConnStlDir;
            result += "," + pExistingConnType;
            result += "," + pConnDetailing;
            result += "," + pSTAADSctnName;

            return result;
        }
    }
}
