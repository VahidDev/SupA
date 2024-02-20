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
    }
}
