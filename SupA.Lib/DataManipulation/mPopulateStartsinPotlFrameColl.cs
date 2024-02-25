using SupA.Lib.Core;

namespace SupA.Lib.DataManipulation
{
    public class mPopulateStartsinPotlFrameColl
    {
        public static List<cPotlSupt> PopulateStartsinPotlFrameColl(List<cGroupNode> CollFrameNodeMapGrouped, List<cSteel> CollGroupNodeBeams, cSuptPoints suptpointEffectiveCentre)
        {
            // Definition of all function specific private variables

            string Var;
            cPotlSupt potlsupt;
            List<cPotlSupt> CollPotlSuptFrameDetails = new List<cPotlSupt>();
            List<cGroupNode> GroupNodesTravelled;
            List<cGroupNode> CollPathsUnTravelled;
            cPotlSupt LL2CPotlStartP;
            cGroupNode LL3PathunT;
            bool LL2MatchingSuptBeamFlag;
            List<cSteel> CollSuptBeams;
            List<cGroupNode> CollSuptBeamEndGroupNodes;
            double Length = 0.0D;
            // Work through all the group nodes

            foreach (var GroupedNode in CollFrameNodeMapGrouped)
            {
                var RouteNode = GroupedNode.GroupedNodes[0]; // Adjusted for zero-based indexing

                if (RouteNode.AssocSuptBeam != "" &&
                    (RouteNode.AssocExistingSteel != "" ||
                     RouteNode.AssocExtendedBeam != "" ||
                     RouteNode.AssocTrim != "" ||
                     RouteNode.AssocVerticalCol != ""))
                {
                    // This little section is inserted so that only one end of each support beam is considered as a starting point for our frame.
                    // This halves the number of potential routes that need to be searched to find our design solution.
                    LL2MatchingSuptBeamFlag = false;
                    foreach (var item in CollPotlSuptFrameDetails)
                    {
                        foreach (var LL3PathunTItem in item.PathsUnTravelled)
                        {
                            if (RouteNode.AssocSuptBeam == LL3PathunTItem.GroupedNodes[0].AssocSuptBeam)
                            {
                                LL2MatchingSuptBeamFlag = true;
                                break; // Exit the inner loop early
                            }
                        }
                        if (LL2MatchingSuptBeamFlag) break; // Exit the outer loop early
                    }

                    if (!LL2MatchingSuptBeamFlag)
                    {
                        // Then we either create a new potlsuptframe if the collection is empty or add the current group node
                        // to the paths untravelled list of our potl supt
                        if (CollPotlSuptFrameDetails.Count == 0)
                        {
                            potlsupt = new cPotlSupt
                            {
                                PathsUnTravelled = new List<cGroupNode>(),
                                BeamsExcluded = new List<cSteel>(),
                                DirnsUntravelled = new List<object>(),
                                GroupNodesinFrame = new List<cGroupNode>(),
                                GroupNodesTravelled = new List<string>(),
                                BeamsinFrame = new List<cSteel>(),
                                LoadsonFrame = new List<object>(), // Assuming object type, adjust as necessary
                                ConntoExistingSteelC = new List<cGroupNode>(), // Assuming object type, adjust as necessary
                                CoordStringENColl = new List<object>(),
                                PotlSuptNo = "potlsupt1"
                            };

                            CollSuptBeams = new List<cSteel>();
                            CollSuptBeamEndGroupNodes = new List<cGroupNode>();
                            AddSuptBeamstoPotlFrame(CollGroupNodeBeams, CollFrameNodeMapGrouped, ref CollSuptBeams, ref CollSuptBeamEndGroupNodes, ref Length);
                            potlsupt.BeamsinFrame = CollSuptBeams;
                            potlsupt.Length = (float)Length;
                            potlsupt.PathsUnTravelled = CollSuptBeamEndGroupNodes;
                            CollPotlSuptFrameDetails.Add(potlsupt);
                        }
                    }
                }
            }

            return CollPotlSuptFrameDetails;
        }

        public static void AddSuptBeamstoPotlFrame(List<cSteel> CollGroupNodeBeams, List<cGroupNode> CollFrameNodeMapGrouped, ref List<cSteel> CollSuptBeams, ref List<cGroupNode> CollSuptBeamEndGroupNodes, ref double Length)
        {
            cSteel Beam;
            int SuptBeamConnCount;
            cGroupNode GroupNode;
            cGroupNode GroupNodeLL2;
            string AssocSuptBeamNameStart = string.Empty;
            string AssocSuptBeamNameEnd = string.Empty;

            // Add all the beams which are support beams to CollSuptBeams and all the group nodes which are part of supt beams to CollSuptBeamEndGroupNodes
            foreach (var beam in CollGroupNodeBeams)
            {
                SuptBeamConnCount = 0;
                foreach (var groupNode in CollFrameNodeMapGrouped)
                {
                    if (groupNode.GroupName == beam.ExistingSteelConnNameEnd && groupNode.AssocSuptBeam != "")
                    {
                        SuptBeamConnCount++;
                        AssocSuptBeamNameStart = groupNode.AssocSuptBeam;
                        if (!CollSuptBeamEndGroupNodes.Contains(groupNode))
                        {
                            CollSuptBeamEndGroupNodes.Add(groupNode);
                        }
                    }

                    if (groupNode.GroupName == beam.ExistingSteelConnNameStart && groupNode.AssocSuptBeam != "")
                    {
                        SuptBeamConnCount++;
                        AssocSuptBeamNameEnd = groupNode.AssocSuptBeam;
                        if (!CollSuptBeamEndGroupNodes.Contains(groupNode))
                        {
                            CollSuptBeamEndGroupNodes.Add(groupNode);
                        }
                    }

                    if (SuptBeamConnCount == 2) break;
                }

                if (SuptBeamConnCount == 2 && AssocSuptBeamNameEnd == AssocSuptBeamNameStart)
                {
                    CollSuptBeams.Add(beam);
                    Length += beam.Length;
                }
            }

            // Review CollSuptBeamEndGroupNodes and make sure each group node is only listed once in the collection
            // In C#, this is easily done using Distinct(). However, ensure your cGroupNode class implements IEquatable<cGroupNode> or has overridden Equals() and GetHashCode() if necessary.
            CollSuptBeamEndGroupNodes = CollSuptBeamEndGroupNodes.Distinct().ToList();
        }
    }
}
