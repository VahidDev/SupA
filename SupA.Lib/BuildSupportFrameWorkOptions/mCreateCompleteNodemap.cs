using Microsoft.VisualBasic;
using SupA.Lib.CoordinateAndAngleManipulation;
using SupA.Lib.Core;
using SupA.Lib.DataManipulation;
using SupA.Lib.Initialization;

namespace SupA.Lib.BuildSupportFrameWorkOptions
{
    public class mCreateCompleteNodemap
    {
        public static List<cRouteNode> CreateCompleteNodemap(List<cSteelDisc> CollLocalExistingSteelDisc, List<cSteelDisc> CollAllDiscBeamsforNodeMap, int SuptGroupNo)
        {
            string Var;
            int LL2C;
            cRouteNode RouteNode = null;
            bool CoordMatch;
            var CollFrameNodeMap = new List<cRouteNode>();

            // For every potential node in our node map which could be a route node
            foreach (cSteelDisc DiscSteelPotl in CollAllDiscBeamsforNodeMap)
            {
                CoordMatch = false;

                // Do the node co-ordinates already exist in CollFrameNodeMap?
                for (LL2C = 1; LL2C <= CollFrameNodeMap.Count; LL2C++)
                {
                    RouteNode = CollFrameNodeMap[LL2C];
                    CoordMatch = mCoordinateCheck.CoordinateCheck("nodepoint", mCreateCoordArray.CreateCoordArray(DiscSteelPotl, "cSteelDisc", rounded: true), mCreateCoordArray.CreateCoordArray(RouteNode, "cRouteNode", rounded: true));
                    if (CoordMatch)
                        break;
                }

                if (!CoordMatch) // Add new node to CollFrameNodeMap and populate with details from the alldiscretisedbeams
                {
                    RouteNode = new cRouteNode();
                    WriteDetailsToRouteNode(RouteNode, DiscSteelPotl);
                    CollFrameNodeMap.Add(RouteNode);
                    RouteNode.RouteNodeNo = "RN-" + CollFrameNodeMap.Count;
                }
                else // Populate arrframenodemap node with any new details which need to be copied across from this duplicate co-ordinate node.
                     // These are any new flags that need to be set and any new "has connection in direction" information.
                {
                    RouteNode = CollFrameNodeMap[LL2C];
                    WriteDetailsToRouteNode(RouteNode, DiscSteelPotl);
                }
            }

            // Then add which of these route nodes is also a tie-in to existing steel
            foreach (cSteelDisc DiscSteelActl in CollLocalExistingSteelDisc)
            {
                CoordMatch = false;
                for (LL2C = 1; LL2C <= CollFrameNodeMap.Count; LL2C++)
                {
                    RouteNode = CollFrameNodeMap[LL2C];
                    CoordMatch = mCoordinateCheck.CoordinateCheck("nodepoint", mCreateCoordArray.CreateCoordArray(DiscSteelActl, "cSteelDisc", rounded: true), mCreateCoordArray.CreateCoordArray(RouteNode, "cRouteNode", rounded: true));
                    if (CoordMatch)
                        break;
                }
                if (CoordMatch) // Add new node to CollFrameNodeMap and populate with details from the alldiscretisedbeams
                {
                    WriteDetailsToRouteNode(RouteNode, DiscSteelActl);
                }
            }

            // Sort the collection based on various criteria
            CollFrameNodeMap = mSortCollection<cRouteNode>.SortCollection(CollFrameNodeMap, "Upping");
            CollFrameNodeMap = mSortCollection<cRouteNode>.SortCollection(CollFrameNodeMap, "Northing");
            CollFrameNodeMap = mSortCollection<cRouteNode>.SortCollection(CollFrameNodeMap, "Easting");
            CollFrameNodeMap = mSortCollection<cRouteNode>.SortCollection(CollFrameNodeMap, "Northing");

            // Now convert all ConnXxxxDir information into route node numbers
            FilloutConnsinRouteNode(CollFrameNodeMap);

            if (mSubInitializationSupA.pubBOOLTraceOn)
                mExportColltoCSVFile<cRouteNode>.ExportColltoCSVFile(CollFrameNodeMap, "CollFrameNodeMap-F" + (SuptGroupNo + mSubInitializationSupA.SuptGroupNoMod).ToString(), "csv");

            return CollFrameNodeMap;
        }

        public static void FilloutConnsinRouteNode(List<cRouteNode> CollFrameNodeMap)
        {
            string[] ArrDirnstoAttempt = { "E", "N", "U", "W", "S", "D" };

            // Work through every route node in CollFrameNodeMap
            foreach (cRouteNode RouteNodeLL1 in CollFrameNodeMap)
            {
                // And every direction for every route node
                for (int i = 0; i < 6; i++)
                {
                    // And now look for the matching route node in CollFrameNodeMap
                    string DirntoSearchforRN = ArrDirnstoAttempt[i];
                    foreach (cRouteNode RouteNodeLL2 in CollFrameNodeMap)
                    {
                        // Check if the connection ConnXDir is true
                        if (string.Equals(((string)RouteNodeLL1.GetType().GetProperty("Conn" + DirntoSearchforRN + "Dir").GetValue(RouteNodeLL1, null)).ToLower(), "true"))
                        {
                            // Check if coordinates match for the respective directions
                            if (mCoordinateCheck.CoordinateCheck("nodepoint", mCreateCoordArray.CreateCoordArray(RouteNodeLL1, "cRouteNode", rounded: true),
                                mCreateCoordArray.CreateCoordArray(RouteNodeLL2, "cRouteNode", null, 0, 0, 0, mDecomposeThreeDVectorintoENUCoords.DecomposeThreeDVectorintoENUCoords(DirntoSearchforRN, -mSubInitializationSupA.pubIntDiscretisationStepSize), true)))
                            {
                                // If coordinates match, update the connection direction with the RouteNodeNo of RouteNodeLL2
                                switch (DirntoSearchforRN)
                                {
                                    case "E":
                                        RouteNodeLL1.ConnEDir = RouteNodeLL2.RouteNodeNo;
                                        break;
                                    case "N":
                                        RouteNodeLL1.ConnNDir = RouteNodeLL2.RouteNodeNo;
                                        break;
                                    case "U":
                                        RouteNodeLL1.ConnUDir = RouteNodeLL2.RouteNodeNo;
                                        break;
                                    case "W":
                                        RouteNodeLL1.ConnWDir = RouteNodeLL2.RouteNodeNo;
                                        break;
                                    case "S":
                                        RouteNodeLL1.ConnSDir = RouteNodeLL2.RouteNodeNo;
                                        break;
                                    case "D":
                                        RouteNodeLL1.ConnDDir = RouteNodeLL2.RouteNodeNo;
                                        break;
                                }
                                // Exit the loop as we found the matching route node
                                break;
                            }
                        }
                    }
                }
            }
        }

        static void WriteDetailsToRouteNode(cRouteNode RouteNode, cSteelDisc DiscSteel)
        {
            RouteNode.Easting = DiscSteel.Easting;
            RouteNode.Northing = DiscSteel.Northing;
            RouteNode.Upping = DiscSteel.Upping;

            if (DiscSteel.SteelFunction == "trimsteel")
            {
                RouteNode.AssocTrim = DiscSteel.Name;
            }
            else if (DiscSteel.SteelFunction == "suptbeam")
            {
                RouteNode.AssocSuptBeam = DiscSteel.Name;
                RouteNode.PSuptBeamPerpDirFromPipeAxis = DiscSteel.SuptBeamPerpDirnfromPipeAxis;
            }
            else if (DiscSteel.SteelFunction == "extendedbeam")
            {
                RouteNode.AssocExtendedBeam = DiscSteel.Name;
            }
            else if (DiscSteel.SteelFunction == "verticalcol")
            {
                RouteNode.AssocVerticalCol = DiscSteel.Name;
            }
            else if (DiscSteel.SteelFunction == "existingsteel" || DiscSteel.ExistingConnType == "CONCRETE")
            {
                RouteNode.AssocExistingSteel = DiscSteel.Name;
                RouteNode.AssocExistingSteelFace = DiscSteel.SteelFunction == "existingsteel" ? DiscSteel.FeatureDesc : "Concrete";
            }
            else
            {
                Interaction.MsgBox("Incorrectly defined steel function in WriteDetailsToRouteNode");
            }

            if (DiscSteel.SteelFunction != "existingsteel")
            {
                if (DiscSteel.DirnofBeam == "E")
                {
                    if (DiscSteel.HasPrecedingNode) RouteNode.ConnWDir = "True";
                    if (DiscSteel.HasSucceedingNode) RouteNode.ConnEDir = "True";
                }
                else if (DiscSteel.DirnofBeam == "W")
                {
                    if (DiscSteel.HasPrecedingNode) RouteNode.ConnEDir = "True";
                    if (DiscSteel.HasSucceedingNode) RouteNode.ConnWDir = "True";
                }
                else if (DiscSteel.DirnofBeam == "N")
                {
                    if (DiscSteel.HasPrecedingNode) RouteNode.ConnSDir = "True";
                    if (DiscSteel.HasSucceedingNode) RouteNode.ConnNDir = "True";
                }
                else if (DiscSteel.DirnofBeam == "S")
                {
                    if (DiscSteel.HasPrecedingNode) RouteNode.ConnNDir = "True";
                    if (DiscSteel.HasSucceedingNode) RouteNode.ConnSDir = "True";
                }
                else if (DiscSteel.DirnofBeam == "U")
                {
                    if (DiscSteel.HasPrecedingNode) RouteNode.ConnDDir = "True";
                    if (DiscSteel.HasSucceedingNode) RouteNode.ConnUDir = "True";
                }
                else if (DiscSteel.DirnofBeam == "D")
                {
                    if (DiscSteel.HasPrecedingNode) RouteNode.ConnUDir = "True";
                    if (DiscSteel.HasSucceedingNode) RouteNode.ConnDDir = "True";
                }
                else
                {
                    Interaction.MsgBox("Check Directions in WriteDetailsToRouteNode");
                }
            }
        }
    }
}
