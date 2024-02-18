
namespace SupA.Lib.Core
{
    public class cSteelDisc
    {
        private float pEasting;
        private float pEastingaccurate;
        private float pNorthing;
        private float pNorthingaccurate;
        private float pUpping;
        private float pUppingaccurate;
        private string pMemType;
        private string pParameters;
        private string pName;
        private string pMemTypeGeneric;
        private string pSteelFunction;
        private float pRowNoinParentArray;
        private string pDirnofBeam;
        private string pMinorAxisGlobaldir;
        private string pMajorAxisGlobaldir;
        private string pFeatureDesc;
        private string pSpare;
        private string pOwningDiscipline;
        private string pSuptBeamPerpDirnfromPipeAxis;
        private bool pHasPrecedingNode;
        private bool pHasSucceedingNode;
        private string pExistingConnType;

        public void CreateInstancefromBeam(cSteel BeamInstance)
        {
            pEasting = BeamInstance.StartERounded;
            pEastingaccurate = BeamInstance.StartE;
            pNorthing = BeamInstance.StartNRounded;
            pNorthingaccurate = BeamInstance.StartN;
            pUpping = BeamInstance.StartURounded;
            pUppingaccurate = BeamInstance.StartU;
            pMemType = BeamInstance.MemType;
            pParameters = BeamInstance.MemModelParam;
            pName = BeamInstance.ModelName;
            pMemTypeGeneric = BeamInstance.MemTypeGeneric;
            pSteelFunction = BeamInstance.SuptSteelFunction;
            pRowNoinParentArray = BeamInstance.BeamNo;
            pDirnofBeam = BeamInstance.Dir;
            pMinorAxisGlobaldir = BeamInstance.MinorAxisGlobaldir;
            pMajorAxisGlobaldir = BeamInstance.MajorAxisGlobaldir;
            pFeatureDesc = BeamInstance.FeatureDesc;
            pSpare = "";
            pOwningDiscipline = BeamInstance.OwningDisc;
            pSuptBeamPerpDirnfromPipeAxis = BeamInstance.SuptBeamPerpDirFromPipeAxis;
            pHasPrecedingNode = false;
            pHasSucceedingNode = false;
            pExistingConnType = BeamInstance.ExistingConnType;
        }

        public string WriteAll(float RowNo = 0)
        {
            return $"{pEasting},{pNorthing},{pUpping},{pEastingaccurate},{pNorthingaccurate},{pUppingaccurate}," +
                   $"{pMemType},{pParameters},{pName},{pMemTypeGeneric},{pSteelFunction},{pRowNoinParentArray}," +
                   $"{pDirnofBeam},{pMinorAxisGlobaldir},{pMajorAxisGlobaldir},{pFeatureDesc},{pSpare}," +
                   $"{pOwningDiscipline},{pSuptBeamPerpDirnfromPipeAxis},{pHasPrecedingNode},{pHasSucceedingNode},{pExistingConnType}";
        }

        public float Easting
        {
            get => pEasting;
            set => pEasting = value;
        }

        public float Northing
        {
            get => pNorthing;
            set => pNorthing = value;
        }

        public float Upping
        {
            get => pUpping;
            set => pUpping = value;
        }

        public float Eastingaccurate
        {
            get => pEastingaccurate;
            set => pEastingaccurate = value;
        }

        public float Northingaccurate
        {
            get => pNorthingaccurate;
            set => pNorthingaccurate = value;
        }

        public float Uppingaccurate
        {
            get => pUppingaccurate;
            set => pUppingaccurate = value;
        }

        public string MemType
        {
            get => pMemType;
            set => pMemType = value;
        }

        public string Parameters
        {
            get => pParameters;
            set => pParameters = value;
        }

        public string Name
        {
            get => pName;
            set => pName = value;
        }

        public string MemTypeGeneric
        {
            get => pMemTypeGeneric;
            set => pMemTypeGeneric = value;
        }

        public string SteelFunction
        {
            get => pSteelFunction;
            set => pSteelFunction = value;
        }

        public float RowNoinParentArray
        {
            get => pRowNoinParentArray;
            set => pRowNoinParentArray = value;
        }

        public string DirnofBeam
        {
            get => pDirnofBeam;
            set => pDirnofBeam = value;
        }

        public string MinorAxisGlobaldir
        {
            get => pMinorAxisGlobaldir;
            set => pMinorAxisGlobaldir = value;
        }

        public string MajorAxisGlobaldir
        {
            get => pMajorAxisGlobaldir;
            set => pMajorAxisGlobaldir = value;
        }

        public string FeatureDesc
        {
            get => pFeatureDesc;
            set => pFeatureDesc = value;
        }

        public string Spare
        {
            get => pSpare;
            set => pSpare = value;
        }

        public string OwningDiscipline
        {
            get => pOwningDiscipline;
            set => pOwningDiscipline = value;
        }

        public string SuptBeamPerpDirnfromPipeAxis
        {
            get => pSuptBeamPerpDirnfromPipeAxis;
            set => pSuptBeamPerpDirnfromPipeAxis = value;
        }

        public bool HasPrecedingNode
        {
            get => pHasPrecedingNode;
            set => pHasPrecedingNode = value;
        }

        public bool HasSucceedingNode
        {
            get => pHasSucceedingNode;
            set => pHasSucceedingNode = value;
        }

        public string ExistingConnType
        {
            get => pExistingConnType;
            set => pExistingConnType = value;
        }
    }
}
