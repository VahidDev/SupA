using SupA.Lib.CoordinateAndAngleManipulation;
using SupA.Lib.Core;
using SupA.Lib.DataManipulation;
using SupA.Lib.Initialization;
using SupA.Lib.Utils;
using static SupA.Lib.CoordinateAndAngleManipulation.mCalculateDirBasedonCoords;

namespace SupA.Lib.BuildSupportFrameWorkOptions
{
    public class mGroupSimilarFrameNodes
    {
        public void GroupSimilarFrameNodes(List<cRouteNode> CollFrameNodeMap, List<object> CollVerticalCols, List<cGroupNode> CollFrameNodeMapGrouped, List<cGroupNode> CollIntersectionGroupNodes, List<cSteel> CollGroupNodeBeams, List<cSuptPoints> CollAdjacentSuptPoints, int SuptGroupNo, cSuptPoints suptpointEffectiveCentre)
        {
            int LL1c = 0;
            bool ContinueLL2Flag;
            while (LL1c < CollFrameNodeMapGrouped.Count)
            {
                ContinueLL2Flag = false;

                int NoofRouteNodesAdded = 0;
                int LL3C = 0;
                while (LL3C < CollFrameNodeMapGrouped[LL1c].GroupedNodes.Count)
                {
                    cRouteNode RouteNodeLL3 = CollFrameNodeMapGrouped[LL1c].GroupedNodes[LL3C];
                    int LL4C = 0;
                    while (LL4C < CollFrameNodeMap.Count)
                    {
                        cRouteNode RoutenodeLL4 = CollFrameNodeMap[LL4C] as cRouteNode;
                        if (CheckIfNodeShouldBeInGroup(RouteNodeLL3, RoutenodeLL4, CollVerticalCols, suptpointEffectiveCentre))
                        {
                            CollFrameNodeMapGrouped[LL1c].GroupedNodes.Add(RoutenodeLL4);
                            CollFrameNodeMap.RemoveAt(LL4C);
                            LL4C--;
                            NoofRouteNodesAdded++;
                        }
                        LL4C++;
                    }
                    LL3C++;
                }

                if (NoofRouteNodesAdded == 0)
                    ContinueLL2Flag = true;

                while (!ContinueLL2Flag)
                {
                    if (CollFrameNodeMap.Count > 0)
                    {
                        var GroupNodeLL3 = new cGroupNode();
                        GroupNodeLL3.GroupName = "groupnode" + (LL1c + 1);
                        GroupNodeLL3.AssocExistingSteel = (CollFrameNodeMap[0] as cRouteNode)?.AssocExistingSteel;
                        GroupNodeLL3.AssocExistingSteelFace = (CollFrameNodeMap[0] as cRouteNode)?.AssocExistingSteelFace;
                        GroupNodeLL3.AssocSuptBeam = (CollFrameNodeMap[0] as cRouteNode)?.AssocSuptBeam;
                        GroupNodeLL3.GroupedNodes = new List<cRouteNode> { CollFrameNodeMap[0] as cRouteNode };
                        GroupNodeLL3.SelectedRouteNode = CollFrameNodeMap[0] as cRouteNode;
                        CollFrameNodeMapGrouped.Add(GroupNodeLL3);
                        CollFrameNodeMap.RemoveAt(0);
                    }

                    LL1c++;
                }
            }

            ReworkBaseCollFrameNodeMapGrouped(CollFrameNodeMapGrouped, CollAdjacentSuptPoints);

            CollIntersectionGroupNodes = IdentifyIntersectionGroupNodes(CollFrameNodeMapGrouped, suptpointEffectiveCentre);

            PopulateConnectingIntersectionNodesinDir(CollIntersectionGroupNodes);

            CollGroupNodeBeams = CreateGroupNodeBeams(CollIntersectionGroupNodes);

            if (mSubInitializationSupA.pubBOOLTraceOn)
            {
                mExportColltoCSVFile<cGroupNode>.ExportColltoCSVFile(CollIntersectionGroupNodes, "CollIntersectionGroupNodes-F" + (SuptGroupNo + mSubInitializationSupA.SuptGroupNoMod), "csv");
                mExportColltoCSVFile<cGroupNode>.ExportColltoCSVFile(CollFrameNodeMapGrouped, "CollFrameNodeMapGrouped-F" + (SuptGroupNo + mSubInitializationSupA.SuptGroupNoMod), "csv");
                mExportColltoCSVFile<cSteel>.ExportColltoCSVFile(CollGroupNodeBeams, "CollGroupNodeBeams-F" + (SuptGroupNo + mSubInitializationSupA.SuptGroupNoMod), "csv");
            }
        }

        public void ReworkBaseCollFrameNodeMapGrouped(List<cGroupNode> CollFrameNodeMapGrouped, List<cSuptPoints> CollAdjacentSuptPoints)
        {
            float CalculatedEasting = 0;
            float CalculatedNorthing = 0;

            foreach (var supt in CollAdjacentSuptPoints)
            {
                CalculatedEasting += supt.EastingSuptPoint;
                CalculatedNorthing += supt.NorthingSuptPoint;
            }

            CalculatedEasting /= CollAdjacentSuptPoints.Count;
            CalculatedNorthing /= CollAdjacentSuptPoints.Count;

            foreach (var GroupNodeLL1 in CollFrameNodeMapGrouped)
            {
                if (Math.Abs(GroupNodeLL1.GroupedNodes[0].Easting - CalculatedEasting) +
                    Math.Abs(GroupNodeLL1.GroupedNodes[0].Northing - CalculatedNorthing) >
                    Math.Abs(GroupNodeLL1.GroupedNodes[GroupNodeLL1.GroupedNodes.Count - 1].Easting - CalculatedEasting) +
                    Math.Abs(GroupNodeLL1.GroupedNodes[GroupNodeLL1.GroupedNodes.Count - 1].Northing - CalculatedNorthing))
                {
                    // Reverse the order of the route nodes inside the group node
                    var GroupedNodesTmp = new List<cRouteNode>();
                    for (int i = GroupNodeLL1.GroupedNodes.Count - 1; i >= 0; i--)
                    {
                        GroupedNodesTmp.Add(GroupNodeLL1.GroupedNodes[i]);
                    }
                    GroupNodeLL1.GroupedNodes = GroupedNodesTmp;
                    GroupNodeLL1.SelectedRouteNode = GroupNodeLL1.GroupedNodes[0];
                }
            }
        }

        public List<cSteel> CreateGroupNodeBeams(List<cGroupNode> CollIntersectionGroupNodes)
        {
            List<cSteel> CollGroupNodeBeams = new List<cSteel>();

            string[,] ArrDirns = { { "E", "W" }, { "N", "S" }, { "U", "D" }, { "W", "E" }, { "S", "N" }, { "D", "U" } };

            int perpAxis1No;
            int perpAxis2No;
            int parlAxisNo;

            foreach (var GroupNodeLL1 in CollIntersectionGroupNodes)
            {
                foreach (var GroupNodeLL2 in CollIntersectionGroupNodes)
                {
                    for (int LL3C = 0; LL3C < ArrDirns.GetLength(0); LL3C++)
                    {
                        string NodeConnTofromGNLL1 = VbaInterop.GetPropertyValue(GroupNodeLL1, "Conn" + ArrDirns[LL3C, 0] + "Dir");
                        if (NodeConnTofromGNLL1 == GroupNodeLL2.GroupName && NodeConnTofromGNLL1 != "")
                        {
                            // Create and add beam
                            cSteel Beam = new cSteel();
                            Beam.ExistingSteelConnNameStart = GroupNodeLL1.GroupName;
                            Beam.ExistingSteelConnNameEnd = GroupNodeLL2.GroupName;
                            Beam.StartE = GroupNodeLL1.GroupedNodes[0].Easting;
                            Beam.StartN = GroupNodeLL1.GroupedNodes[0].Northing;
                            Beam.StartU = GroupNodeLL1.GroupedNodes[0].Upping;
                            Beam.EndE = GroupNodeLL2.GroupedNodes[0].Easting;
                            Beam.EndN = GroupNodeLL2.GroupedNodes[0].Northing;
                            Beam.EndU = GroupNodeLL2.GroupedNodes[0].Upping;
                            Beam.Dir = CoordinateCalculator.CalculateDirBasedonCoords(Beam.StartE, Beam.StartN, Beam.StartU, Beam.EndE, Beam.EndN, Beam.EndU);
                            Beam.Length = (float)Math.Sqrt(Math.Pow(Beam.EndE - Beam.StartE, 2) + Math.Pow(Beam.EndN - Beam.StartN, 2) + Math.Pow(Beam.EndU - Beam.StartU, 2));
                            mDefinePerpsandParls.DefinePerpsandParls(ref Beam.Dir, out Beam.MajorAxisGlobaldir, out Beam.MinorAxisGlobaldir, out perpAxis1No, out perpAxis2No, out parlAxisNo);
                            Beam.Jusline = "CTOP";
                            Beam.FeatureDesc = "TOPC";
                            Beam.ModelName = "GroupBeam" + (CollGroupNodeBeams.Count + 1);
                            Beam.SuptSteelFunction = AssignGroupBeamSteelType(GroupNodeLL1, GroupNodeLL2);

                            CollGroupNodeBeams.Add(Beam);
                        }
                    }
                }
            }

            // Remove duplicate beams
            for (int BeamLL1C = 0; BeamLL1C < CollGroupNodeBeams.Count; BeamLL1C++)
            {
                cSteel beamLL1 = CollGroupNodeBeams[BeamLL1C];
                for (int BeamLL2C = CollGroupNodeBeams.Count - 1; BeamLL2C >= 0; BeamLL2C--)
                {
                    cSteel BeamLL2 = CollGroupNodeBeams[BeamLL2C];

                    if (beamLL1.ModelName != BeamLL2.ModelName &&
                        ((beamLL1.ExistingSteelConnNameStart == BeamLL2.ExistingSteelConnNameStart && beamLL1.ExistingSteelConnNameEnd == BeamLL2.ExistingSteelConnNameEnd) ||
                        (beamLL1.ExistingSteelConnNameStart == BeamLL2.ExistingSteelConnNameEnd && beamLL1.ExistingSteelConnNameEnd == BeamLL2.ExistingSteelConnNameStart)))
                    {
                        CollGroupNodeBeams.RemoveAt(BeamLL1C);
                        break;
                    }
                }
            }

            // Update ModelName for uniqueness
            for (int BeamLL1C = 0; BeamLL1C < CollGroupNodeBeams.Count; BeamLL1C++)
            {
                CollGroupNodeBeams[BeamLL1C].ModelName = "GroupBeam" + (BeamLL1C + 1);
            }

            return CollGroupNodeBeams;
        }

        public string AssignGroupBeamSteelType(cGroupNode GroupNodeLL1, cGroupNode GroupNodeLL2)
        {
            if (!string.IsNullOrEmpty(GroupNodeLL2.GroupedNodes[0].AssocExtendedBeam) && !string.IsNullOrEmpty(GroupNodeLL1.GroupedNodes[0].AssocExtendedBeam))
            {
                return "extendedbeam";
            }
            else if (!string.IsNullOrEmpty(GroupNodeLL2.GroupedNodes[0].AssocSuptBeam) && !string.IsNullOrEmpty(GroupNodeLL1.GroupedNodes[0].AssocSuptBeam))
            {
                return "suptbeam";
            }
            else if (!string.IsNullOrEmpty(GroupNodeLL2.GroupedNodes[0].AssocTrim) && !string.IsNullOrEmpty(GroupNodeLL1.GroupedNodes[0].AssocTrim))
            {
                return "trim";
            }
            else if (!string.IsNullOrEmpty(GroupNodeLL2.GroupedNodes[0].AssocVerticalCol) && !string.IsNullOrEmpty(GroupNodeLL1.GroupedNodes[0].AssocVerticalCol))
            {
                return "verticalcol";
            }

            // Return null or throw an exception depending on your requirement
            return null;
        }

        public List<cGroupNode> IdentifyIntersectionGroupNodes(List<cGroupNode> CollFrameNodeMapGrouped, cSuptPoints suptpointEffectiveCentre)
        {
            List<cGroupNode> CollIntersectionGroupNodes = new List<cGroupNode>();

            foreach (var GroupNode in CollFrameNodeMapGrouped)
            {
                int PerpBeamDirns = 0;
                List<string> ConnectedRoutenodes = new List<string>();

                if (!string.IsNullOrEmpty(GroupNode.GroupedNodes[0].ConnEDir)) ConnectedRoutenodes.Add(GroupNode.GroupedNodes[0].ConnEDir);
                if (!string.IsNullOrEmpty(GroupNode.GroupedNodes[0].ConnWDir)) ConnectedRoutenodes.Add(GroupNode.GroupedNodes[0].ConnWDir);
                if (!string.IsNullOrEmpty(GroupNode.GroupedNodes[0].ConnNDir)) ConnectedRoutenodes.Add(GroupNode.GroupedNodes[0].ConnNDir);
                if (!string.IsNullOrEmpty(GroupNode.GroupedNodes[0].ConnSDir)) ConnectedRoutenodes.Add(GroupNode.GroupedNodes[0].ConnSDir);
                if (!string.IsNullOrEmpty(GroupNode.GroupedNodes[0].ConnUDir)) ConnectedRoutenodes.Add(GroupNode.GroupedNodes[0].ConnUDir);
                if (!string.IsNullOrEmpty(GroupNode.GroupedNodes[0].ConnDDir)) ConnectedRoutenodes.Add(GroupNode.GroupedNodes[0].ConnDDir);

                if (!string.IsNullOrEmpty(GroupNode.GroupedNodes[0].ConnEDir) || !string.IsNullOrEmpty(GroupNode.GroupedNodes[0].ConnWDir)) PerpBeamDirns++;
                if (!string.IsNullOrEmpty(GroupNode.GroupedNodes[0].ConnNDir) || !string.IsNullOrEmpty(GroupNode.GroupedNodes[0].ConnSDir)) PerpBeamDirns++;
                if (!string.IsNullOrEmpty(GroupNode.GroupedNodes[0].ConnUDir) || !string.IsNullOrEmpty(GroupNode.GroupedNodes[0].ConnDDir)) PerpBeamDirns++;
                int NoofDirsfromCE = ConnectedRoutenodes.Count;

                if (!string.IsNullOrEmpty(GroupNode.GroupedNodes[0].AssocSuptBeam) && !string.IsNullOrEmpty(GroupNode.GroupedNodes[0].AssocExtendedBeam))
                {
                    CollIntersectionGroupNodes.Add(GroupNode);
                }
                else if (GroupedNodeonSuptPointEffectiveCentre(GroupNode.GroupedNodes[0], suptpointEffectiveCentre) && !string.IsNullOrEmpty(GroupNode.GroupedNodes[0].AssocSuptBeam))
                {
                    CollIntersectionGroupNodes.Add(GroupNode);
                }
                else if (!string.IsNullOrEmpty(GroupNode.GroupedNodes[0].AssocSuptBeam) && NoofDirsfromCE == 1)
                {
                    CollIntersectionGroupNodes.Add(GroupNode);
                }
                else if (PerpBeamDirns >= 2 || !string.IsNullOrEmpty(GroupNode.GroupedNodes[0].AssocExistingSteel))
                {
                    int RouteNodenameC = 0;
                    while (RouteNodenameC < ConnectedRoutenodes.Count)
                    {
                        bool removed = false;
                        foreach (var GroupNodeLL2 in CollFrameNodeMapGrouped)
                        {
                            if (GroupNodeLL2.GroupedNodes[0].RouteNodeNo == ConnectedRoutenodes[RouteNodenameC])
                            {
                                ConnectedRoutenodes.RemoveAt(RouteNodenameC);
                                RouteNodenameC--;
                                removed = true;
                                break;
                            }
                        }
                        if (!removed)
                            RouteNodenameC++;
                    }

                    if (ConnectedRoutenodes.Count == 0)
                        CollIntersectionGroupNodes.Add(GroupNode);
                }
                else if (!string.IsNullOrEmpty(GroupNode.GroupedNodes[0].AssocExistingSteel) && NoofDirsfromCE >= 1)
                {
                    CollIntersectionGroupNodes.Add(GroupNode);
                }
            }

            return CollIntersectionGroupNodes;
        }

        public void PopulateConnectingIntersectionNodesinDir(List<cGroupNode> CollIntersectionGroupNodes)
        {
            string[] ArrDirnstoAttempt = new string[] { "E", "N", "U", "W", "S", "D" };

            foreach (var GroupNodeLL1 in CollIntersectionGroupNodes)
            {
                cRouteNode RoutenodeLL1b = GroupNodeLL1.GroupedNodes[0];

                foreach (var direction in ArrDirnstoAttempt)
                {
                    string ConninDir = (string)RoutenodeLL1b.GetType().GetProperty("Conn" + direction + "Dir").GetValue(RoutenodeLL1b);

                    if (!string.IsNullOrEmpty(ConninDir))
                    {
                        int StepCounterLL3 = 1;
                        bool MatchingCoordinateFoundFlag = false;

                        while (!MatchingCoordinateFoundFlag && (mSubInitializationSupA.pubIntDiscretisationStepSize * StepCounterLL3) < 4500)
                        {
                            foreach (var GroupNodeLL4 in CollIntersectionGroupNodes)
                            {
                                cRouteNode RoutenodeLL4b = GroupNodeLL4.GroupedNodes[0];
                                MatchingCoordinateFoundFlag = mCoordinateCheck.CoordinateCheck("nodepoint", mCreateCoordArray.CreateCoordArray(RoutenodeLL4b, "cRouteNode"),
                                    mCreateCoordArray.CreateCoordArray(RoutenodeLL1b, "cRouteNode", null, 0, 0, 0,
                                    mDecomposeThreeDVectorintoENUCoords.DecomposeThreeDVectorintoENUCoords(direction, StepCounterLL3 * mSubInitializationSupA.pubIntDiscretisationStepSize)));

                                if (MatchingCoordinateFoundFlag)
                                {
                                    switch (direction)
                                    {
                                        case "E":
                                            GroupNodeLL1.ConnEDir = GroupNodeLL4.GroupName;
                                            break;
                                        case "N":
                                            GroupNodeLL1.ConnNDir = GroupNodeLL4.GroupName;
                                            break;
                                        case "U":
                                            GroupNodeLL1.ConnUDir = GroupNodeLL4.GroupName;
                                            break;
                                        case "W":
                                            GroupNodeLL1.ConnWDir = GroupNodeLL4.GroupName;
                                            break;
                                        case "S":
                                            GroupNodeLL1.ConnSDir = GroupNodeLL4.GroupName;
                                            break;
                                        case "D":
                                            GroupNodeLL1.ConnDDir = GroupNodeLL4.GroupName;
                                            break;
                                    }
                                    break;
                                }
                            }

                            if (MatchingCoordinateFoundFlag)
                                break;

                            StepCounterLL3++;
                        }
                    }
                }
            }
        }

        public bool CheckIfNodeShouldBeInGroup(cRouteNode RouteNodeLL3, cRouteNode RoutenodeLL4, List<object> CollVerticalCols, cSuptPoints suptpointEffectiveCentre)
        {
            float[] CoordArrayLL3 = new float[5];
            float[] CoordArrayLL4 = new float[5];
            string StartConnLL3;
            string StartConnLL4;
            string EndConnLL3;
            string EndConnLL4;
            string StartConnDirLL4;
            string EndConnDirLL4;
            string StartConnDirLL3;
            string EndConnDirLL3;
            string PerpAxis1;
            string PerpAxis2;
            int PerpAxis1No;
            int PerpAxis2No;
            int ParlAxisNo;
            string DirnofTravel = "";
            string SignofTravel = "";

            if (RouteNodeLL3.AssocVerticalCol != "" && RoutenodeLL4.AssocVerticalCol != "" &&
                RouteNodeLL3.AssocVerticalCol != RoutenodeLL4.AssocVerticalCol &&
                RouteNodeLL3.AssocTrim == "" && RoutenodeLL4.AssocTrim == "" &&
                RouteNodeLL3.AssocSuptBeam == "" && RoutenodeLL4.AssocSuptBeam == "" &&
                RouteNodeLL3.AssocExtendedBeam == "" && RoutenodeLL4.AssocExtendedBeam == "" &&
                RouteNodeLL3.AssocExistingSteel == "" && RoutenodeLL4.AssocExistingSteel == "")
            {
                StartConnLL3 = (string)CollVerticalCols[int.Parse(RouteNodeLL3.AssocVerticalCol.Replace("verticalcol", ""))].GetType().GetProperty("ExistingSteelConnNameStart").GetValue(CollVerticalCols[int.Parse(RouteNodeLL3.AssocVerticalCol.Replace("verticalcol", ""))]);
                StartConnLL4 = (string)CollVerticalCols[int.Parse(RoutenodeLL4.AssocVerticalCol.Replace("verticalcol", ""))].GetType().GetProperty("ExistingSteelConnNameStart").GetValue(CollVerticalCols[int.Parse(RoutenodeLL4.AssocVerticalCol.Replace("verticalcol", ""))]);
                EndConnLL3 = (string)CollVerticalCols[int.Parse(RouteNodeLL3.AssocVerticalCol.Replace("verticalcol", ""))].GetType().GetProperty("ExistingSteelConnNameEnd").GetValue(CollVerticalCols[int.Parse(RouteNodeLL3.AssocVerticalCol.Replace("verticalcol", ""))]);
                EndConnLL4 = (string)CollVerticalCols[int.Parse(RoutenodeLL4.AssocVerticalCol.Replace("verticalcol", ""))].GetType().GetProperty("ExistingSteelConnNameEnd").GetValue(CollVerticalCols[int.Parse(RoutenodeLL4.AssocVerticalCol.Replace("verticalcol", ""))]);

                StartConnDirLL3 = (string)CollVerticalCols[int.Parse(RouteNodeLL3.AssocVerticalCol.Replace("verticalcol", ""))].GetType().GetProperty("StartConnStlDir").GetValue(CollVerticalCols[int.Parse(RouteNodeLL3.AssocVerticalCol.Replace("verticalcol", ""))]);
                EndConnDirLL3 = (string)CollVerticalCols[int.Parse(RouteNodeLL3.AssocVerticalCol.Replace("verticalcol", ""))].GetType().GetProperty("EndConnStlDir").GetValue(CollVerticalCols[int.Parse(RouteNodeLL3.AssocVerticalCol.Replace("verticalcol", ""))]);
                StartConnDirLL4 = (string)CollVerticalCols[int.Parse(RoutenodeLL4.AssocVerticalCol.Replace("verticalcol", ""))].GetType().GetProperty("StartConnStlDir").GetValue(CollVerticalCols[int.Parse(RoutenodeLL4.AssocVerticalCol.Replace("verticalcol", ""))]);
                EndConnDirLL4 = (string)CollVerticalCols[int.Parse(RoutenodeLL4.AssocVerticalCol.Replace("verticalcol", ""))].GetType().GetProperty("EndConnStlDir").GetValue(CollVerticalCols[int.Parse(RoutenodeLL4.AssocVerticalCol.Replace("verticalcol", ""))]);

                if (EndConnLL3 == EndConnLL4 && StartConnLL3 == StartConnLL4)
                {
                    if (!CheckifThereisaPerpBeamAtThisPoint(StartConnDirLL4, EndConnDirLL4, StartConnDirLL3, EndConnDirLL3))
                    {
                        if (!CheckifAtSuptBeamMidPoint(StartConnDirLL4, RoutenodeLL4, suptpointEffectiveCentre))
                        {
                            var parlAxis = "U";
                            mDefinePerpsandParls.DefinePerpsandParls(ref parlAxis, out PerpAxis1, out PerpAxis2, out PerpAxis1No, out PerpAxis2No, out ParlAxisNo);

                            CoordArrayLL3 = mCreateCoordArray.CreateCoordArray(RouteNodeLL3, "cRouteNode");
                            CoordArrayLL4 = mCreateCoordArray.CreateCoordArray(RoutenodeLL4, "cRouteNode");

                            if (CoordArrayLL3[ParlAxisNo] == CoordArrayLL4[ParlAxisNo])
                            {
                                for (int i = 1; i <= 4; i++)
                                {
                                    if (i == 1 || i == 3)
                                        DirnofTravel = PerpAxis1;
                                    if (i == 2 || i == 4)
                                        DirnofTravel = PerpAxis2;
                                    if (i == 1 || i == 2)
                                        SignofTravel = "allpositive";
                                    if (i == 3 || i == 4)
                                        SignofTravel = "allnegative";

                                    CoordArrayLL3 = mCreateCoordArray.CreateCoordArray(RouteNodeLL3, "cRouteNode", null, 0, 0, 0, mDecomposeThreeDVectorintoENUCoords.DecomposeThreeDVectorintoENUCoords(DirnofTravel, mSubInitializationSupA.pubIntDiscretisationStepSize, SignofTravel), true);
                                    CoordArrayLL4 = mCreateCoordArray.CreateCoordArray(RoutenodeLL4, "cRouteNode", null, 0, 0, 0, null, true);

                                    if (mCoordinateCheck.CoordinateCheck("nodepoint", CoordArrayLL3, CoordArrayLL4))
                                    {
                                        return true;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return false;
        }

        public bool CheckifThereisaPerpBeamAtThisPoint(string StartConnDirLL4, string EndConnDirLL4, string StartConnDirLL3, string EndConnDirLL3)
        {
            if ((StartConnDirLL4 == "N" || StartConnDirLL4 == "S") && (EndConnDirLL4 == "N" || EndConnDirLL4 == "S") &&
                (StartConnDirLL3 == "N" || StartConnDirLL3 == "S") && (EndConnDirLL3 == "N" || EndConnDirLL3 == "S"))
            {
                return false;
            }
            else if ((StartConnDirLL4 == "E" || StartConnDirLL4 == "W") && (EndConnDirLL4 == "E" || EndConnDirLL4 == "W") &&
                (StartConnDirLL3 == "E" || StartConnDirLL3 == "W") && (EndConnDirLL3 == "E" || EndConnDirLL3 == "W"))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public bool CheckifAtSuptBeamMidPoint(string StartConnDirLL4, cRouteNode RoutenodeLL4, cSuptPoints suptpointEffectiveCentre)
        {
            if (StartConnDirLL4 == "N" || StartConnDirLL4 == "S")
            {
                if (RoutenodeLL4.Northing == suptpointEffectiveCentre.NorthingSuptPointRounded)
                {
                    return true;
                }
            }
            else if (StartConnDirLL4 == "E" || StartConnDirLL4 == "W")
            {
                if (RoutenodeLL4.Easting == suptpointEffectiveCentre.EastingSuptPointRounded)
                {
                    return true;
                }
            }
            else
            {
                // Handle the case where the direction is neither "N" nor "S", nor "E" nor "W"
                throw new Exception("There is an uncoded case here");
            }

            return false;
        }

        public cSuptPoints SetEffectiveCentreofAdjacentSupts(List<object> CollAdjacentSuptPoints)
        {
            cSuptPoints SetEffectiveCentreofAdjacentSupts = new cSuptPoints();
            SetEffectiveCentreofAdjacentSupts.EastingSuptPoint = 0;
            SetEffectiveCentreofAdjacentSupts.NorthingSuptPoint = 0;
            SetEffectiveCentreofAdjacentSupts.ElSuptPoint = 0;

            for (int I = 1; I <= CollAdjacentSuptPoints.Count; I++)
            {
                cSuptPoints Supt = (cSuptPoints)CollAdjacentSuptPoints[I];
                SetEffectiveCentreofAdjacentSupts.EastingSuptPoint = (SetEffectiveCentreofAdjacentSupts.EastingSuptPoint * (I - 1) + Supt.EastingSuptPoint) / I;
                SetEffectiveCentreofAdjacentSupts.NorthingSuptPoint = (SetEffectiveCentreofAdjacentSupts.NorthingSuptPoint * (I - 1) + Supt.NorthingSuptPoint) / I;
                SetEffectiveCentreofAdjacentSupts.ElSuptPoint = (SetEffectiveCentreofAdjacentSupts.ElSuptPoint * (I - 1) + Supt.ElSuptPoint) / I;
            }

            SetEffectiveCentreofAdjacentSupts.EastingSuptPointRounded = (float)Math.Round(SetEffectiveCentreofAdjacentSupts.EastingSuptPoint, mSubInitializationSupA.pubIntDiscretisationStepDecPlaces);
            SetEffectiveCentreofAdjacentSupts.NorthingSuptPointRounded = (float)Math.Round(SetEffectiveCentreofAdjacentSupts.NorthingSuptPoint, mSubInitializationSupA.pubIntDiscretisationStepDecPlaces);
            SetEffectiveCentreofAdjacentSupts.ElSuptPointRounded = (float)Math.Round(SetEffectiveCentreofAdjacentSupts.ElSuptPoint, mSubInitializationSupA.pubIntDiscretisationStepDecPlaces);

            return SetEffectiveCentreofAdjacentSupts;
        }

        public bool GroupedNodeonSuptPointEffectiveCentre(cRouteNode Nodetocheck, cSuptPoints suptpointEffectiveCentre)
        {
            if (!string.IsNullOrEmpty(Nodetocheck.ConnNDir) || !string.IsNullOrEmpty(Nodetocheck.ConnSDir))
            {
                if (Nodetocheck.Northing == suptpointEffectiveCentre.NorthingSuptPointRounded)
                {
                    return true;
                }
            }
            else if (!string.IsNullOrEmpty(Nodetocheck.ConnEDir) || !string.IsNullOrEmpty(Nodetocheck.ConnWDir))
            {
                if (Nodetocheck.Easting == suptpointEffectiveCentre.EastingSuptPointRounded)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
