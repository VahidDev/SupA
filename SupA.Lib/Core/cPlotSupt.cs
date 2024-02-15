using SupA.Lib.Core;
using System.Collections.ObjectModel;

namespace SupA.Lib
{
    public class cPotlSupt
    {
        private bool pisClosed;
        private bool pisInvalid;
        private Collection<object> pPathsUnTravelled;
        private int pPathsUnTravelledCount;
        private Collection<object> pBeamsinFrame;
        private Collection<object> pBeamsExcluded;
        private Collection<object> pDirnsUntravelled;
        private Collection<object> pGroupNodesTravelled;
        private Collection<object> pGroupNodesinFrame;
        private Collection<object> pLoadsonFrame;
        private float pWeightPrelim;
        private float pWeightCalculated;
        private float pLength;
        private Collection<cGroupNode> pConntoExistingSteelC;
        private string pPotlSuptNo;
        private string pPotlSuptChildof;
        private Collection<object> pCoordStringENColl;
        private Collection<object> pBeamsinFrameratlized;
        private Collection<object> pBasePlateDetailsinFrame;
        private float pFabricationCost;
        private float pMatlCost;
        private float pTotalCost;
        private float pFrameRanking;
        private string pPreferredOption;

        public object WriteAllPathsUnTravelled(float RowNo)
        {
            Collection<object> Group = pPathsUnTravelled;
            string[] Arr = new string[Group.Count + 1];
            if (Group.Count == 0)
                Arr[1] = "potlsupt" + RowNo + "," + "," + pLength + "," + pConntoExistingSteelC.Count + "," + pisClosed + "," + pPotlSuptNo + "," + pPotlSuptChildof;
            for (int MemC = 1; MemC <= Group.Count; MemC++)
            {
                cGroupNode Mem = (cGroupNode)Group[MemC];
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
            Collection<object> Group = pGroupNodesinFrame;
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
            Collection<cGroupNode> Group = pConntoExistingSteelC;
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
            Collection<object> Group = pBeamsinFrame;
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
            Collection<object> Group = pBeamsinFrameratlized;
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
            Collection<object> Group = pBasePlateDetailsinFrame;
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

        public Collection<object> PathsUnTravelled
        {
            get { return pPathsUnTravelled; }
            set { pPathsUnTravelled = value; }
        }

        public Collection<object> BeamsExcluded
        {
            get { return pBeamsExcluded; }
            set { pBeamsExcluded = value; }
        }

        public Collection<object> DirnsUntravelled
        {
            get { return pDirnsUntravelled; }
            set { pDirnsUntravelled = value; }
        }

        public Collection<object> GroupNodesTravelled
        {
            get { return pGroupNodesTravelled; }
            set { pGroupNodesTravelled = value; }
        }

        public Collection<object> GroupNodesinFrame
        {
            get { return pGroupNodesinFrame; }
            set { pGroupNodesinFrame = value; }
        }

        public Collection<object> BeamsinFrame
        {
            get { return pBeamsinFrame; }
            set { pBeamsinFrame = value; }
        }

        public Collection<object> LoadsonFrame
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

        public Collection<cGroupNode> ConntoExistingSteelC
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

        public Collection<object> CoordStringENColl
        {
            get { return pCoordStringENColl; }
            set { pCoordStringENColl = value; }
        }

        public Collection<object> BeamsinFrameratlized
        {
            get { return pBeamsinFrameratlized; }
            set { pBeamsinFrameratlized = value; }
        }

        public Collection<object> BasePlateDetailsinFrame
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
