//using SupA.Lib.Core;

//namespace SupA.Lib.DataManipulation
//{
//    public class mTravelCurrRtandNdtoNextIntsctn
//    {
//        public void TravelCurrRtandNdtoNextIntsctn(List<cPotlSupt> CollPotlSuptFrameDetails, List<cGroupNode> CollFrameNodeMapGrouped, ref bool UntravelledPathsOutstanding)
//        {
//            int LL3C;
//            bool RouteAlreadyTravelledFlag;
//            bool IntersectionFoundFlagLL4;
//            cGroupNode GroupNodeLL5;
//            cRouteNode RoutenodeLL5b;
//            cGroupNode TravelledGroupNodeLL7;
//            int StepCounterLL4;
//            string PathsPartTravelledName;
//            string NodeMapRowNoFromName;
//            int NodeMapRowNoFrom;
//            int NodeMapRowNoTo;
//            string NodeMapRowNoToName;
//            string strGroupNodeinDir;
//            int PathsPartTravelledNo;
//            bool MatchingCoordinateFoundFlag;
//            string CurrentDirnofTravelLL3;

//            UntravelledPathsOutstanding = false;

//            foreach (cPotlSupt PotlSuptFrameLL2 in CollPotlSuptFrameDetails)
//            {
//                foreach (string DirnofTravelLL3 in PotlSuptFrameLL2.DirnsUntravelled)
//                {
//                    CurrentDirnofTravelLL3 = DirnofTravelLL3.Substring(DirnofTravelLL3.Length - 1);

//                    NodeMapRowNoFromName = DirnofTravelLL3.Substring(0, DirnofTravelLL3.Length - 1);
//                    for (NodeMapRowNoFrom = 0; NodeMapRowNoFrom < CollFrameNodeMapGrouped.Count; NodeMapRowNoFrom++)
//                    {
//                        if (NodeMapRowNoFromName == CollFrameNodeMapGrouped[NodeMapRowNoFrom].GroupName)
//                        {
//                            break;
//                        }
//                    }

//                    for (PathsPartTravelledNo = 0; PathsPartTravelledNo < PotlSuptFrameLL2.PathsPartTravelled.Count; PathsPartTravelledNo++)
//                    {
//                        if (NodeMapRowNoFromName == PotlSuptFrameLL2.PathsPartTravelled[PathsPartTravelledNo].GroupName)
//                        {
//                            break;
//                        }
//                    }

//                    NodeMapRowNoToName = CollFrameNodeMapGrouped[NodeMapRowNoFrom].GetType().GetProperty("Conn" + CurrentDirnofTravelLL3 + "Dir").GetValue(CollFrameNodeMapGrouped[NodeMapRowNoFrom]).ToString();
//                    for (NodeMapRowNoTo = 0; NodeMapRowNoTo < CollFrameNodeMapGrouped.Count; NodeMapRowNoTo++)
//                    {
//                        if (NodeMapRowNoToName == CollFrameNodeMapGrouped[NodeMapRowNoTo].GroupName)
//                        {
//                            break;
//                        }
//                    }

//                    RouteAlreadyTravelledFlag = CheckIfRouteAlreadyTravelled(PotlSuptFrameLL2, CollFrameNodeMapGrouped[NodeMapRowNoFrom], CollFrameNodeMapGrouped[NodeMapRowNoTo]);
//                    if (!RouteAlreadyTravelledFlag)
//                    {
//                        AddDetailstoCurrPotlSuptRoute(PotlSuptFrameLL2, CollFrameNodeMapGrouped[NodeMapRowNoTo], CollFrameNodeMapGrouped[NodeMapRowNoTo].GroupedNodes[0], PathsPartTravelledNo);
//                    }
//                }

//                if (!PotlSuptFrameLL2.IsClosed)
//                {
//                    ClearCurrPotlSuptRoute(PotlSuptFrameLL2);
//                }
//                if (PotlSuptFrameLL2.PathsUnTravelled.Count >= 1)
//                {
//                    UntravelledPathsOutstanding = true;
//                }
//            }
//        }

//        public void AddDetailstoCurrPotlSuptRoute(cPotlSupt PotlSuptFrameLL2, cGroupNode GroupNodeLL5, cRouteNode RoutenodeLL5b, int PathsPartTravelledNo)
//        {
//            // RouteNodeLL2 is NOT used in VBA code.
//            //cRouteNode RouteNodeLL2 = PotlSuptFrameLL2.PathsPartTravelled[PathsPartTravelledNo].GroupedNodes[1];
//            bool DuplicateTieIn;
//            cRouteNode ExistingSteelTieIn;
//            cGroupNode DupGroupNodeChk;
//            bool DupGroupNodeChkFlag;

//            // This is a new check to prevent nodes being covered multiple times.
//            // The sequencing of this is important (in comparison to the subsequent code).
//            // As we are allowed to add a node we are arriving at for the first time into paths untravelled.
//            // We just want to prevent subsequent arrivals from becoming departures.
//            DupGroupNodeChkFlag = false;
//            foreach (cGroupNode groupNode in PotlSuptFrameLL2.PathsUnTravelled)
//            {
//                if (groupNode.GroupName == GroupNodeLL5.GroupName)
//                {
//                    DupGroupNodeChkFlag = true;
//                    break;
//                }
//            }
//            foreach (cGroupNode groupNode in PotlSuptFrameLL2.GroupNodesinFrame)
//            {
//                if (groupNode.GroupName == GroupNodeLL5.GroupName)
//                {
//                    DupGroupNodeChkFlag = true;
//                    break;
//                }
//            }

//            if (DupGroupNodeChkFlag == false)
//                PotlSuptFrameLL2.PathsUnTravelled.Add(GroupNodeLL5);

//            // Add travelled node and beam details into our potlsuptframe definition.
//            // Also add our current intersection node to our pathsuntravelled collection
//            PotlSuptFrameLL2.GroupNodesinFrame.Add(GroupNodeLL5);
//            PotlSuptFrameLL2.GroupNodesTravelled.Add(GroupNodeLL5.GroupName);
//        }
//    }
//}
