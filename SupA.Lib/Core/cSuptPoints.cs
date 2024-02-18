namespace SupA.Lib.Core
{
    public class TTblExtendedPHASST
    {
        public string AnciType { get; set; }
        public float AnciHeightStd { get; set; }
        public float AnciHeightMin { get; set; }
        public float AnciHeightMax { get; set; }
        public float BeamLenReqd { get; set; }
    }

    public class cSuptPoints
    {
        private float pEastingSuptPoint;
        private float pNorthingSuptPoint;
        private float pElSuptPoint;
        private string pTubiName;
        private string pSuptCat;
        private string pCollSuptPointsinAreaRowNo;
        private string pSupportOpt;
        private string pTubidir;
        private string pBore;
        private float pEastingSuptPointRounded;
        private float pNorthingSuptPointRounded;
        private float pElSuptPointRounded;
        private float pSuptLoadFx;
        private float pSuptLoadFy;
        private float pSuptLoadFz;
        private string pSuptFuncDirX;
        private string pSuptFuncDirY;
        private string pSuptFuncDirZ;
        private bool pIncorpintoFrameFlag;
        private string pPipeName;
        private string pSuptMaturityFlag1;
        private string pSuptMaturityFlag2;
        private float pPipeOD;
        private string pAnciType;
        private float pAnciHeiStd;
        private float pAnciHeiMin;
        private float pAnciHeiMax;
        private string pInsuType;
        private float pInsuThk;
        private float pOpTemp;
        private float pDesTempMin;
        private float pDesTempMax;
        private float pBlowdownDurn;
        private float pDesPressMax;
        private string pAreaSuptSeqNumber;
        private string pTOSPerpDir1pos;
        private string pTOSPerpDir2pos;
        private string pTOSPerpDir1neg;
        private string pTOSPerpDir2neg;
        private int pShrdLvlNoinPerpDir1pos;
        private int pShrdLvlNoinPerpDir2pos;
        private int pShrdLvlNoinPerpDir1neg;
        private int pShrdLvlNoinPerpDir2neg;
        private string pChosenShrdLvlDir;
        private string pBeamLenReqd;

        // Let Statement for initial population of Classes
        public void ReadAll(object[] arrToRead)
        {
            pEastingSuptPoint = Convert.ToSingle(arrToRead[0]);
            pNorthingSuptPoint = Convert.ToSingle(arrToRead[1]);
            pElSuptPoint = Convert.ToSingle(arrToRead[2]);
            pTubiName = Convert.ToString(arrToRead[3]);
            pSuptCat = Convert.ToString(arrToRead[4]);
            pCollSuptPointsinAreaRowNo = Convert.ToString(arrToRead[5]);
            pSupportOpt = Convert.ToString(arrToRead[6]);
            pTubidir = Convert.ToString(arrToRead[7]);
            pBore = Convert.ToString(arrToRead[8]);
            pEastingSuptPointRounded = Convert.ToSingle(arrToRead[9]);
            pNorthingSuptPointRounded = Convert.ToSingle(arrToRead[10]);
            pElSuptPointRounded = Convert.ToSingle(arrToRead[11]);
            pSuptLoadFx = Convert.ToSingle(arrToRead[12]);
            pSuptLoadFy = Convert.ToSingle(arrToRead[13]);
            pSuptLoadFz = Convert.ToSingle(arrToRead[14]);
            pSuptFuncDirX = Convert.ToString(arrToRead[15]);
            pSuptFuncDirY = Convert.ToString(arrToRead[16]);
            pSuptFuncDirZ = Convert.ToString(arrToRead[17]);
            pIncorpintoFrameFlag = Convert.ToBoolean(arrToRead[18]);
            pPipeName = Convert.ToString(arrToRead[19]);
            pSuptMaturityFlag1 = Convert.ToString(arrToRead[20]);
            pSuptMaturityFlag2 = Convert.ToString(arrToRead[21]);
            pPipeOD = Convert.ToSingle(arrToRead[22]);
            pAnciType = Convert.ToString(arrToRead[23]);
            pAnciHeiStd = Convert.ToSingle(arrToRead[24]);
            pAnciHeiMin = Convert.ToSingle(arrToRead[25]);
            pAnciHeiMax = Convert.ToSingle(arrToRead[26]);
            pInsuType = Convert.ToString(arrToRead[27]);
            pInsuThk = Convert.ToSingle(arrToRead[28]);
            pOpTemp = Convert.ToSingle(arrToRead[29]);
            pDesTempMin = Convert.ToSingle(arrToRead[30]);
            pDesTempMax = Convert.ToSingle(arrToRead[31]);
            pBlowdownDurn = Convert.ToSingle(arrToRead[32]);
            pDesPressMax = Convert.ToSingle(arrToRead[33]);
            pAreaSuptSeqNumber = Convert.ToString(arrToRead[34]);
            pTOSPerpDir1pos = Convert.ToString(arrToRead[35]);
            pTOSPerpDir2pos = Convert.ToString(arrToRead[36]);
            pTOSPerpDir1neg = Convert.ToString(arrToRead[37]);
            pTOSPerpDir2neg = Convert.ToString(arrToRead[38]);
            pShrdLvlNoinPerpDir1pos = Convert.ToInt32(arrToRead[39]);
            pShrdLvlNoinPerpDir2pos = Convert.ToInt32(arrToRead[40]);
            pShrdLvlNoinPerpDir1neg = Convert.ToInt32(arrToRead[41]);
            pShrdLvlNoinPerpDir2neg = Convert.ToInt32(arrToRead[42]);
            pChosenShrdLvlDir = Convert.ToString(arrToRead[43]);
            pBeamLenReqd = Convert.ToString(arrToRead[44]);
        }

        // WritePhasstData method
        public void WritePhasstData(TTblExtendedPHASST tblExtendedPHASST)
        {
            pAnciType = tblExtendedPHASST.AnciType;
            pAnciHeiStd = tblExtendedPHASST.AnciHeightStd;
            pAnciHeiMin = tblExtendedPHASST.AnciHeightMin;
            pAnciHeiMax = tblExtendedPHASST.AnciHeightMax;
            pBeamLenReqd = tblExtendedPHASST.BeamLenReqd;
        }

        // WriteAll Property
        public string WriteAll(float rowNo = 0)
        {
            return $"{pEastingSuptPoint},{pNorthingSuptPoint},{pElSuptPoint},{pTubiName},{pSuptCat},{pCollSuptPointsinAreaRowNo}," +
                   $"{pSupportOpt},{pTubidir},{pBore},{pEastingSuptPointRounded},{pNorthingSuptPointRounded},{pElSuptPointRounded}," +
                   $"{pSuptLoadFx},{pSuptLoadFy},{pSuptLoadFz},{pSuptFuncDirX},{pSuptFuncDirY},{pSuptFuncDirZ},{pIncorpintoFrameFlag}," +
                   $"{pPipeName},{pSuptMaturityFlag1},{pSuptMaturityFlag2},{pPipeOD},{pAnciType},{pAnciHeiStd},{pAnciHeiMin},{pAnciHeiMax}," +
                   $"{pInsuType},{pInsuThk},{pOpTemp},{pDesTempMin},{pDesTempMax},{pBlowdownDurn},{pDesPressMax},{pAreaSuptSeqNumber}," +
                   $"{pTOSPerpDir1pos},{pTOSPerpDir2pos},{pTOSPerpDir1neg},{pTOSPerpDir2neg},{pShrdLvlNoinPerpDir1pos},{pShrdLvlNoinPerpDir2pos}," +
                   $"{pShrdLvlNoinPerpDir1neg},{pShrdLvlNoinPerpDir2neg},{pChosenShrdLvlDir},{pBeamLenReqd},{pTubiName}";
        }

        // Get Properties
        public float EastingSuptPointRounded => pEastingSuptPointRounded;
        public float NorthingSuptPointRounded => pNorthingSuptPointRounded;
        public float ElSuptPointRounded => pElSuptPointRounded;
        public string TubiName => pTubiName;
        public string SuptCat => pSuptCat;
        public string CollSuptPointsinAreaRowNo => pCollSuptPointsinAreaRowNo;
        public string SupportOpt => pSupportOpt;
        public string Tubidir => pTubidir;
        public string Bore => pBore;
        public float EastingSuptPoint => pEastingSuptPoint;
        public float NorthingSuptPoint => pNorthingSuptPoint;
        public float ElSuptPoint => pElSuptPoint;
        public float SuptLoadFx => pSuptLoadFx;
        public float SuptLoadFy => pSuptLoadFy;
        public float SuptLoadFz => pSuptLoadFz;
        public string SuptFuncDirX => pSuptFuncDirX;
        public string SuptFuncDirY => pSuptFuncDirY;
        public string SuptFuncDirZ => pSuptFuncDirZ;
        public bool IncorpintoFrameFlag => pIncorpintoFrameFlag;
        public string PipeName => pPipeName;
        public string SuptMaturityFlag1 => pSuptMaturityFlag1;
        public string SuptMaturityFlag2 => pSuptMaturityFlag2;
        public float PipeOD => pPipeOD;
        public string AnciType => pAnciType;
        public float AnciHeiStd => pAnciHeiStd;
        public float AnciHeiMin => pAnciHeiMin;
        public float AnciHeiMax => pAnciHeiMax;
        public string InsuType => pInsuType;
        public float InsuThk => pInsuThk;
        public float OpTemp => pOpTemp;
        public float DesTempMin => pDesTempMin;
        public float DesTempMax => pDesTempMax;
        public float BlowdownDurn => pBlowdownDurn;
        public float DesPressMax => pDesPressMax;
        public string AreaSuptSeqNumber => pAreaSuptSeqNumber;
        public string TOSPerpDir1pos => pTOSPerpDir1pos;
        public string TOSPerpDir2pos => pTOSPerpDir2pos;
        public string TOSPerpDir1neg => pTOSPerpDir1neg;
        public string TOSPerpDir2neg => pTOSPerpDir2neg;
        public int ShrdLvlNoinPerpDir1pos => pShrdLvlNoinPerpDir1pos;
        public int ShrdLvlNoinPerpDir2pos => pShrdLvlNoinPerpDir2pos;
        public int ShrdLvlNoinPerpDir1neg => pShrdLvlNoinPerpDir1neg;
        public int ShrdLvlNoinPerpDir2neg => pShrdLvlNoinPerpDir2neg;
        public string ChosenShrdLvlDir => pChosenShrdLvlDir;
        public string BeamLenReqd => pBeamLenReqd;

        // Individual Let Properties
        public float EastingSuptPointRoundedProperty
        {
            set => pEastingSuptPointRounded = value;
        }

        public float NorthingSuptPointRoundedProperty
        {
            set => pNorthingSuptPointRounded = value;
        }

        public float ElSuptPointRoundedProperty
        {
            set => pElSuptPointRounded = value;
        }

        public string TubiNameProperty
        {
            set => pTubiName = value;
        }

        public string SuptCatProperty
        {
            set => pSuptCat = value;
        }

        public string CollSuptPointsinAreaRowNoProperty
        {
            set => pCollSuptPointsinAreaRowNo = value;
        }

        public string SupportOptProperty
        {
            set => pSupportOpt = value;
        }

        public string TubidirProperty
        {
            set => pTubidir = value;
        }

        public string BoreProperty
        {
            set => pBore = value;
        }

        public float EastingSuptPointProperty
        {
            set => pEastingSuptPoint = value;
        }

        public float NorthingSuptPointProperty
        {
            set => pNorthingSuptPoint = value;
        }

        public float ElSuptPointProperty
        {
            set => pElSuptPoint = value;
        }

        public float SuptLoadFxProperty
        {
            set => pSuptLoadFx = value;
        }

        public float SuptLoadFyProperty
        {
            set => pSuptLoadFy = value;
        }

        public float SuptLoadFzProperty
        {
            set => pSuptLoadFz = value;
        }

        public string SuptFuncDirXProperty
        {
            set => pSuptFuncDirX = value;
        }

        public string SuptFuncDirYProperty
        {
            set => pSuptFuncDirY = value;
        }

        public string SuptFuncDirZProperty
        {
            set => pSuptFuncDirZ = value;
        }

        public bool IncorpintoFrameFlagProperty
        {
            set => pIncorpintoFrameFlag = value;
        }

        public string PipeNameProperty
        {
            set => pPipeName = value;
        }

        public string SuptMaturityFlag1Property
        {
            set => pSuptMaturityFlag1 = value;
        }

        public string SuptMaturityFlag2Property
        {
            set => pSuptMaturityFlag2 = value;
        }

        public float PipeODProperty
        {
            set => pPipeOD = value;
        }

        public string AnciTypeProperty
        {
            set => pAnciType = value;
        }

        public float AnciHeiStdProperty
        {
            set => pAnciHeiStd = value;
        }

        public float AnciHeiMinProperty
        {
            set => pAnciHeiMin = value;
        }

        public float AnciHeiMaxProperty
        {
            set => pAnciHeiMax = value;
        }

        public string InsuTypeProperty
        {
            set => pInsuType = value;
        }

        public float InsuThkProperty
        {
            set => pInsuThk = value;
        }

        public float OpTempProperty
        {
            set => pOpTemp = value;
        }

        public float DesTempMinProperty
        {
            set => pDesTempMin = value;
        }

        public float DesTempMaxProperty
        {
            set => pDesTempMax = value;
        }

        public float BlowdownDurnProperty
        {
            set => pBlowdownDurn = value;
        }

        public float DesPressMaxProperty
        {
            set => pDesPressMax = value;
        }

        public string AreaSuptSeqNumberProperty
        {
            set => pAreaSuptSeqNumber = value;
        }

        public string TOSPerpDir1posProperty
        {
            set => pTOSPerpDir1pos = value;
        }

        public string TOSPerpDir2posProperty
        {
            set => pTOSPerpDir2pos = value;
        }

        public string TOSPerpDir1negProperty
        {
            set => pTOSPerpDir1neg = value;
        }

        public string TOSPerpDir2negProperty
        {
            set => pTOSPerpDir2neg = value;
        }

        public int ShrdLvlNoinPerpDir1posProperty
        {
            set => pShrdLvlNoinPerpDir1pos = value;
        }

        public int ShrdLvlNoinPerpDir2posProperty
        {
            set => pShrdLvlNoinPerpDir2pos = value;
        }

        public int ShrdLvlNoinPerpDir1negProperty
        {
            set => pShrdLvlNoinPerpDir1neg = value;
        }

        public int ShrdLvlNoinPerpDir2negProperty
        {
            set => pShrdLvlNoinPerpDir2neg = value;
        }

        public string ChosenShrdLvlDirProperty
        {
            set => pChosenShrdLvlDir = value;
        }

        public string BeamLenReqdProperty
        {
            set => pBeamLenReqd = value;
        }
    }
}
