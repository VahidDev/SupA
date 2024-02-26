using SupA.Lib.Core;

namespace SupA.Lib.DataManipulation
{
    public class mTravelCurrRtandNdtoNextIntsctn
    {
        public void TravelCurrRtandNdtoNextIntsctn(List<cPotlSupt> collPotlSuptFrameDetails, List<cGroupNode> collFrameNodeMapGrouped, ref bool untravelledPathsOutstanding)
        {
            untravelledPathsOutstanding = false;

            foreach (var potlSuptFrameLL2 in collPotlSuptFrameDetails)
            {
                foreach (var direction in potlSuptFrameLL2.DirnsUntravelled)
                {
                    var dirnofTravelLL3 = direction; // Simplified for C#
                    var currentDirnofTravelLL3 = dirnofTravelLL3.ToString().Last().ToString();

                    var nodeMapRowNoFromName = dirnofTravelLL3.ToString().Substring(0, dirnofTravelLL3.ToString().Length - 1);
                    var nodeMapRowNoFrom = collFrameNodeMapGrouped.FindIndex(x => x.GroupName == nodeMapRowNoFromName) + 1;

                    var pathsPartTravelledNo = potlSuptFrameLL2.PathsPartTravelled.FindIndex(x => x.GroupName == nodeMapRowNoFromName) + 1;

                    var nodeMapRowNoToName = collFrameNodeMapGrouped[nodeMapRowNoFrom - 1].Connections[$"Conn{currentDirnofTravelLL3}Dir"];
                    var nodeMapRowNoTo = collFrameNodeMapGrouped.FindIndex(x => x.GroupName == nodeMapRowNoToName) + 1;

                    var routeAlreadyTravelledFlag = CheckIfRouteAlreadyTravelled(potlSuptFrameLL2, collFrameNodeMapGrouped[nodeMapRowNoFrom - 1], collFrameNodeMapGrouped[nodeMapRowNoTo - 1]);
                    if (!routeAlreadyTravelledFlag)
                    {
                        AddDetailstoCurrPotlSuptRoute(potlSuptFrameLL2, collFrameNodeMapGrouped[nodeMapRowNoTo - 1], collFrameNodeMapGrouped[nodeMapRowNoTo - 1].GroupedNodes[0], pathsPartTravelledNo);
                    }
                }

                if (!potlSuptFrameLL2.IsClosed)
                {
                    ClearCurrPotlSuptRoute(potlSuptFrameLL2);
                }

                if (potlSuptFrameLL2.PathsUnTravelled.Count >= 1)
                {
                    untravelledPathsOutstanding = true;
                }
            }
        }

        public static void AddDetailstoCurrPotlSuptRoute(cPotlSupt PotlSuptFrameLL2, cGroupNode GroupNodeLL5, cRouteNode RoutenodeLL5b, int PathsPartTravelledNo)
        {
            cRouteNode RouteNodeLL2 = new cRouteNode();
            RouteNodeLL2 = PotlSuptFrameLL2.PathsPartTravelled[PathsPartTravelledNo].GroupedNodes[1];
            bool DuplicateTieIn;
            cRouteNode ExistingSteelTieIn;
            cGroupNode DupGroupNodeChk;
            bool DupGroupNodeChkFlag;

            // this is a new check to prevent nodes being covered multiple times. The sequencing of this is important (in comparison to the subsequent code).
            // as we are allowed to add a node we are arriving at for the first time into paths untravelled.
            // we just want to prevent subsequent arrivals from becoming departures.
            DupGroupNodeChkFlag = false;
            foreach (cGroupNode PathsUnTravelled in PotlSuptFrameLL2.PathsUnTravelled)
            {
                if (PathsUnTravelled.GroupName == GroupNodeLL5.GroupName)
                {
                    DupGroupNodeChkFlag = true;
                    break;
                }
            }

            foreach (cGroupNode GroupNodesinFrame in PotlSuptFrameLL2.GroupNodesinFrame)
            {
                if (GroupNodesinFrame.GroupName == GroupNodeLL5.GroupName)
                {
                    DupGroupNodeChkFlag = true;
                    break;
                }
            }

            if (DupGroupNodeChkFlag == false)
            {
                PotlSuptFrameLL2.PathsUnTravelled.Add(GroupNodeLL5);
            }

            // Add travelled node and beam details into our potlsuptframe definition.
            // Also add our current intersection node to our pathsuntravelled collection
            PotlSuptFrameLL2.GroupNodesinFrame.Add(GroupNodeLL5);
            PotlSuptFrameLL2.GroupNodesTravelled.Add(GroupNodeLL5.GroupName);
        }
    }
}
