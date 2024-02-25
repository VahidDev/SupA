using System.Collections.ObjectModel;

namespace SupA.Lib.Core
{
    public class cPotlSupt
    {
        private bool pisClosed;
        private bool pisInvalid;
        private List<cGroupNode> pPathsUnTravelled;
        private int pPathsUnTravelledCount;
        private List<cSteel> pBeamsinFrame;
        private List<cSteel> pBeamsExcluded;
        private List<object> pDirnsUntravelled;
        private List<string> pGroupNodesTravelled;
        private List<cGroupNode> pGroupNodesinFrame;
        private List<object> pLoadsonFrame;
        private float pWeightPrelim;
        private float pWeightCalculated;
        private float pLength;
        private List<cGroupNode> pConntoExistingSteelC;
        private string pPotlSuptNo;
        private string pPotlSuptChildof;
        private List<object> pCoordStringENColl;
        private List<cSteel> pBeamsinFrameratlized;
        private List<cBasePlate> pBasePlateDetailsinFrame;
        private float pFabricationCost;
        private float pMatlCost;
        private float pTotalCost;
        private float pFrameRanking;
        private string pPreferredOption;

        public object WriteAllPathsUnTravelled(float RowNo)
        {
            List<cGroupNode> Group = pPathsUnTravelled;
            string[] Arr = new string[Group.Count + 1];
            if (Group.Count == 0)
                Arr[1] = "potlsupt" + RowNo + "," + "," + pLength + "," + pConntoExistingSteelC.Count + "," + pisClosed + "," + pPotlSuptNo + "," + pPotlSuptChildof;
            for (int MemC = 1; MemC <= Group.Count; MemC++)
            {
                cGroupNode Mem = Group[MemC];
                Arr[MemC] = "potlsupt" + RowNo + "," + Mem.GroupName + "," + pLength + "," + pConntoExistingSteelC.Count + "," + pisClosed + "," + pPotlSuptNo + "," + pPotlSuptChildof;
            }
            return Arr;
        }

        public object WriteAllFrameSummaryInfo(float RowNo)
        {
            string Str = pPotlSuptNo + "," + pisClosed + "," + pisInvalid + "," + pBeamsinFrame.Count + "," + pGroupNodesinFrame.Count + "," + pWeightPrelim + "," + pWeightCalculated + "," + pLength + "," + pConntoExistingSteelC.Count + "," + pPotlSuptNo + "," + pCoordStringENColl.Count + "," + pFabricationCost + "," + pMatlCost + "," + pTotalCost + "," + (pBeamsinFrameratlized == null ? 0 : pBeamsinFrameratlized.Count) + "," + pFrameRanking + "," + pPreferredOption;
            return Str;
        }

        public object WriteAllGroupNodesinFrame(float RowNo)
        {
            List<cGroupNode> Group = pGroupNodesinFrame;
            string[] Arr = new string[Group.Count + 1];
            string InternalPotlSuptNo = pPotlSuptNo;
            if (Group.Count == 0)
                Arr[1] = "potlsupt" + RowNo;
            for (int MemC = 1; MemC <= Group.Count; MemC++)
            {
                cGroupNode Mem = (cGroupNode)Group[MemC];
                Arr[MemC] = "potlsupt" + RowNo + "," + Mem.GroupName + "," + pisClosed + "," + pisInvalid + "," + InternalPotlSuptNo;
            }
            return Arr;
        }

        public object WriteAllConntoExistingSteelCNodesinFrame(float RowNo)
        {
            List<cGroupNode> Group = pConntoExistingSteelC;
            string[] Arr = new string[Group.Count + 1];
            string InternalPotlSuptNo = pPotlSuptNo;
            if (Group.Count == 0)
                Arr[1] = "potlsupt" + RowNo;
            for (int MemC = 1; MemC <= Group.Count; MemC++)
            {
                cGroupNode Mem = (cGroupNode)Group[MemC];
                Arr[MemC] = "potlsupt" + RowNo + "," + Mem.GroupName + "," + Mem.AssocExistingSteel + "," + pisClosed + "," + pisInvalid + "," + InternalPotlSuptNo;
            }
            return Arr;
        }

        public object WriteAllBeamsinFrame(float RowNo)
        {
            List<cSteel> Group = pBeamsinFrame;
            string[] Arr = new string[Group.Count + 1];
            string InternalPotlSuptNo = pPotlSuptNo;
            if (Group.Count == 0)
                Arr[1] = "potlsupt" + RowNo;
            for (int MemC = 1; MemC <= Group.Count; MemC++)
            {
                cSteel Mem = (cSteel)Group[MemC];
                Arr[MemC] = "potlsupt" + RowNo + "," + Mem.WriteAll + "," + InternalPotlSuptNo;
            }
            return Arr;
        }

        public object WriteAllDetailedBeamsinFrame(float RowNo)
        {
            List<cSteel> Group = pBeamsinFrameratlized;
            string[] Arr = new string[Group.Count + 1];
            string InternalPotlSuptNo = pPotlSuptNo;
            float tmpFrameRanking = pFrameRanking;
            string tmppreferredoption = pPreferredOption;
            if (Group.Count == 0)
                Arr[1] = "potlsupt" + RowNo;
            for (int MemC = 1; MemC <= Group.Count; MemC++)
            {
                cSteel Mem = (cSteel)Group[MemC];
                Arr[MemC] = "potlsupt" + RowNo + "," + Mem.WriteAll + "," + InternalPotlSuptNo + "," + tmpFrameRanking + "," + tmppreferredoption;
            }
            return Arr;
        }

        public object WriteAllBasePlateDetailsinFrame(float RowNo)
        {
            List<cBasePlate> Group = pBasePlateDetailsinFrame;
            string[] Arr = new string[Group.Count + 1];
            string InternalPotlSuptNo = pPotlSuptNo;
            float tmpFrameRanking = pFrameRanking;
            for (int MemC = 1; MemC <= Group.Count; MemC++)
            {
                cBasePlate Mem = (cBasePlate)Group[MemC];
                Arr[MemC] = Mem.WriteAll + "," + InternalPotlSuptNo + "," + tmpFrameRanking;
            }
            if (Group.Count != 0)
                return Arr;
            return null;
        }

        public bool IsClosed
        {
            get { return pisClosed; }
            set { pisClosed = value; }
        }

        public bool IsInvalid
        {
            get { return pisInvalid; }
            set { pisInvalid = value; }
        }

        public int PathsUnTravelledCount
        {
            get { return pPathsUnTravelledCount; }
            set { pPathsUnTravelledCount = value; }
        }

        public List<cGroupNode> PathsUnTravelled
        {
            get { return pPathsUnTravelled; }
            set { pPathsUnTravelled = value; }
        }

        public List<cSteel> BeamsExcluded
        {
            get { return pBeamsExcluded; }
            set { pBeamsExcluded = value; }
        }

        public List<object> DirnsUntravelled
        {
            get { return pDirnsUntravelled; }
            set { pDirnsUntravelled = value; }
        }

        public List<string> GroupNodesTravelled
        {
            get { return pGroupNodesTravelled; }
            set { pGroupNodesTravelled = value; }
        }

        public List<cGroupNode> GroupNodesinFrame
        {
            get { return pGroupNodesinFrame; }
            set { pGroupNodesinFrame = value; }
        }

        public List<cSteel> BeamsinFrame
        {
            get { return pBeamsinFrame; }
            set { pBeamsinFrame = value; }
        }

        public List<object> LoadsonFrame
        {
            get { return pLoadsonFrame; }
            set { pLoadsonFrame = value; }
        }

        public float WeightPrelim
        {
            get { return pWeightPrelim; }
            set { pWeightPrelim = value; }
        }

        public float WeightCalculated
        {
            get { return pWeightCalculated; }
            set { pWeightCalculated = value; }
        }

        public float Length
        {
            get { return pLength; }
            set { pLength = value; }
        }

        public List<cGroupNode> ConntoExistingSteelC
        {
            get { return pConntoExistingSteelC; }
            set { pConntoExistingSteelC = value; }
        }

        public string PotlSuptNo
        {
            get { return pPotlSuptNo; }
            set { pPotlSuptNo = value; }
        }

        public string PotlSuptChildof
        {
            get { return pPotlSuptChildof; }
            set { pPotlSuptChildof = value; }
        }

        public List<object> CoordStringENColl
        {
            get { return pCoordStringENColl; }
            set { pCoordStringENColl = value; }
        }

        public List<cSteel> BeamsinFrameratlized
        {
            get { return pBeamsinFrameratlized; }
            set { pBeamsinFrameratlized = value; }
        }

        public List<cBasePlate> BasePlateDetailsinFrame
        {
            get { return pBasePlateDetailsinFrame; }
            set { pBasePlateDetailsinFrame = value; }
        }

        public float FabricationCost
        {
            get { return pFabricationCost; }
            set { pFabricationCost = value; }
        }

        public float MatlCost
        {
            get { return pMatlCost; }
            set { pMatlCost = value; }
        }

        public float TotalCost
        {
            get { return pTotalCost; }
            set { pTotalCost = value; }
        }

        public float FrameRanking
        {
            get { return pFrameRanking; }
            set { pFrameRanking = value; }
        }

        public string PreferredOption
        {
            get { return pPreferredOption; }
            set { pPreferredOption = value; }
        }
    }
}
