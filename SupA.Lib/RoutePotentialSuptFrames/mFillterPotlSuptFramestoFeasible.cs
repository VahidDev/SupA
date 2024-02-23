using SupA.Lib.Core;
using SupA.Lib.Initialization;

namespace SupA.Lib.RoutePotentialSuptFrames
{
    public class mFillterPotlSuptFramestoFeasible
    {
        public static void FilterPotlSuptFramestoFeasible(List<cPotlSupt> CollPotlSuptFrameDetails, List<cGroupNode> CollFrameNodeMapGrouped, List<cSteel> CollGroupNodeBeams)
        {
            int PotlSuptFrameLL1C;
            cPotlSupt PotlSuptFrameLL1;
            cPotlSupt PotlSuptFrameLL1Tmp;
            bool TieinSteelFlag;
            cGroupNode GroupNode;
            cSteel Beam;
            int GroupNodeLL3C;
            bool IncompleteRouteFlag;
            bool InvalidColumnsFlag;
            int VertBeamCount;
            float Beamlevel = 0f;
            bool FirstHoriLevel;
            int NoOfHoriLevels;
            bool HorizontalBeamsExistFlag;
            int[] ArrExistingSteelTieInC = new int[5];

            PotlSuptFrameLL1C = 0;
            while (PotlSuptFrameLL1C < CollPotlSuptFrameDetails.Count)
            {
                PotlSuptFrameLL1 = CollPotlSuptFrameDetails[PotlSuptFrameLL1C];
                if (PotlSuptFrameLL1.IsInvalid)
                {
                    CollPotlSuptFrameDetails.RemoveAt(PotlSuptFrameLL1C);
                    PotlSuptFrameLL1C--;
                }
                PotlSuptFrameLL1C++;
            }

            PotlSuptFrameLL1C = 0;
            while (PotlSuptFrameLL1C < CollPotlSuptFrameDetails.Count)
            {
                PotlSuptFrameLL1 = CollPotlSuptFrameDetails[PotlSuptFrameLL1C];
                if (!PotlSuptFrameLL1.IsClosed)
                {
                    CollPotlSuptFrameDetails.RemoveAt(PotlSuptFrameLL1C);
                    PotlSuptFrameLL1C--;
                }
                PotlSuptFrameLL1C++;
            }

            PotlSuptFrameLL1C = 0;
            while (PotlSuptFrameLL1C < CollPotlSuptFrameDetails.Count)
            {
                PotlSuptFrameLL1 = CollPotlSuptFrameDetails[PotlSuptFrameLL1C];
                int LL2C = 0;
                while (LL2C < PotlSuptFrameLL1.ConntoExistingSteelC.Count)
                {
                    if (PotlSuptFrameLL1.ConntoExistingSteelC[LL2C].AssocExistingSteel == "")
                    {
                        PotlSuptFrameLL1.ConntoExistingSteelC.RemoveAt(LL2C);
                        LL2C--;
                    }
                    LL2C++;
                }
                PotlSuptFrameLL1C++;
            }

            PotlSuptFrameLL1C = 0;
            while (PotlSuptFrameLL1C < CollPotlSuptFrameDetails.Count)
            {
                PotlSuptFrameLL1 = CollPotlSuptFrameDetails[PotlSuptFrameLL1C];
                TieinSteelFlag = false;
                if (PotlSuptFrameLL1.ConntoExistingSteelC.Count >= 1)
                    TieinSteelFlag = true;
                if (!TieinSteelFlag)
                {
                    CollPotlSuptFrameDetails.RemoveAt(PotlSuptFrameLL1C);
                    PotlSuptFrameLL1C--;
                }
                PotlSuptFrameLL1C++;
            }

            PotlSuptFrameLL1C = 0;
            while (PotlSuptFrameLL1C < CollPotlSuptFrameDetails.Count)
            {
                PotlSuptFrameLL1 = CollPotlSuptFrameDetails[PotlSuptFrameLL1C];
                InvalidColumnsFlag = false;
                foreach (var beam in PotlSuptFrameLL1.BeamsinFrame)
                {
                    foreach (var beamLL2 in PotlSuptFrameLL1.BeamsinFrame)
                    {
                        if ((beam.Dir == "U" || beam.Dir == "D") && (beamLL2.Dir == "U" || beamLL2.Dir == "D"))
                        {
                            if (Math.Abs(beam.StartE - beamLL2.StartE) == mSubInitializationSupA.pubIntDiscretisationStepSize ||
                                Math.Abs(beam.StartN - beamLL2.StartN) == mSubInitializationSupA.pubIntDiscretisationStepSize)
                            {
                                InvalidColumnsFlag = true;
                            }
                        }
                    }
                }
                if (InvalidColumnsFlag)
                {
                    CollPotlSuptFrameDetails.RemoveAt(PotlSuptFrameLL1C);
                    PotlSuptFrameLL1C--;
                }
                PotlSuptFrameLL1C++;
            }

            PotlSuptFrameLL1C = 0;
            while (PotlSuptFrameLL1C < CollPotlSuptFrameDetails.Count)
            {
                PotlSuptFrameLL1 = CollPotlSuptFrameDetails[PotlSuptFrameLL1C];
                VertBeamCount = 0;
                NoOfHoriLevels = 0;
                FirstHoriLevel = true;
                foreach (var beam in PotlSuptFrameLL1.BeamsinFrame)
                {
                    if (beam.Dir == "U" || beam.Dir == "D")
                    {
                        VertBeamCount++;
                    }
                    if (beam.Dir != "U" && beam.Dir != "D")
                    {
                        if (FirstHoriLevel)
                        {
                            Beamlevel = beam.EndURounded;
                            NoOfHoriLevels = 1;
                            FirstHoriLevel = false;
                        }
                        else if (Beamlevel != beam.EndURounded)
                        {
                            NoOfHoriLevels++;
                        }
                    }
                }
                if (VertBeamCount >= 3 && NoOfHoriLevels == 1)
                {
                    CollPotlSuptFrameDetails.RemoveAt(PotlSuptFrameLL1C);
                    PotlSuptFrameLL1C--;
                }
                PotlSuptFrameLL1C++;
            }

            PotlSuptFrameLL1C = 0;
            while (PotlSuptFrameLL1C < CollPotlSuptFrameDetails.Count)
            {
                PotlSuptFrameLL1 = CollPotlSuptFrameDetails[PotlSuptFrameLL1C];
                IncompleteRouteFlag = false;
                foreach (var groupNode in PotlSuptFrameLL1.GroupNodesinFrame)
                {
                    int NoofBeamsNodeisOn = 0;
                    foreach (var beam in PotlSuptFrameLL1.BeamsinFrame)
                    {
                        if (beam.ExistingSteelConnNameStart == groupNode.GroupName || beam.ExistingSteelConnNameEnd == groupNode.GroupName)
                        {
                            NoofBeamsNodeisOn++;
                        }
                    }
                    if (NoofBeamsNodeisOn <= 1 && groupNode.AssocExistingSteel == "" && groupNode.AssocSuptBeam == "")
                    {
                        IncompleteRouteFlag = true;
                    }
                }
                if (IncompleteRouteFlag)
                {
                    CollPotlSuptFrameDetails.RemoveAt(PotlSuptFrameLL1C);
                    PotlSuptFrameLL1C--;
                }
                PotlSuptFrameLL1C++;
            }

            PotlSuptFrameLL1C = 0;
            while (PotlSuptFrameLL1C < CollPotlSuptFrameDetails.Count)
            {
                PotlSuptFrameLL1 = CollPotlSuptFrameDetails[PotlSuptFrameLL1C];
                HorizontalBeamsExistFlag = false;
                foreach (var beam in PotlSuptFrameLL1.BeamsinFrame)
                {
                    if (beam.Dir != "U" && beam.Dir != "D")
                    {
                        HorizontalBeamsExistFlag = true;
                    }
                }
                if (!HorizontalBeamsExistFlag)
                {
                    CollPotlSuptFrameDetails.RemoveAt(PotlSuptFrameLL1C);
                    PotlSuptFrameLL1C--;
                }
                PotlSuptFrameLL1C++;
            }

            PotlSuptFrameLL1C = 0;
            while (PotlSuptFrameLL1C < CollPotlSuptFrameDetails.Count)
            {
                if (mFindSpecificPotlRoute.FindSpecificPotlRoute(CollPotlSuptFrameDetails[PotlSuptFrameLL1C]))
                {
                    Console.WriteLine("stop and see what happens next");
                }

                PotlSuptFrameLL1Tmp = new cPotlSupt();
                PotlSuptFrameLL1Tmp = mAddRouteBranchesandOptions.CopyCurrPotlSuptFrame(CollPotlSuptFrameDetails[PotlSuptFrameLL1C]);
                List<cSteel> BeamsCollTemp = new List<cSteel>();

                int BeaminFrameLL1C = 0;
                while (BeaminFrameLL1C < PotlSuptFrameLL1Tmp.BeamsinFrame.Count)
                {
                    cSteel BeaminFrameLL1 = PotlSuptFrameLL1Tmp.BeamsinFrame[BeaminFrameLL1C];
                    foreach (var GroupNode_2 in PotlSuptFrameLL1Tmp.GroupNodesinFrame)
                    {
                        if ((BeaminFrameLL1.ExistingSteelConnNameStart == GroupNode_2.GroupName && GroupNode_2.AssocExistingSteel != "") ||
                            (BeaminFrameLL1.ExistingSteelConnNameEnd == GroupNode_2.GroupName && GroupNode_2.AssocExistingSteel != ""))
                        {
                            BeamsCollTemp.Add(BeaminFrameLL1);
                            PotlSuptFrameLL1Tmp.BeamsinFrame.RemoveAt(BeaminFrameLL1C);
                            BeaminFrameLL1C--;
                            break;
                        }
                    }
                    BeaminFrameLL1C++;
                }

                bool ChangeSinceLast = true;
                while (ChangeSinceLast)
                {
                    ChangeSinceLast = false;
                    foreach (var beam in BeamsCollTemp)
                    {
                        BeaminFrameLL1C = 0;
                        while (BeaminFrameLL1C < PotlSuptFrameLL1Tmp.BeamsinFrame.Count)
                        {
                            cSteel BeaminFrameLL1 = PotlSuptFrameLL1Tmp.BeamsinFrame[BeaminFrameLL1C];
                            if (BeaminFrameLL1.ExistingSteelConnNameEnd == beam.ExistingSteelConnNameEnd ||
                                BeaminFrameLL1.ExistingSteelConnNameStart == beam.ExistingSteelConnNameStart ||
                                BeaminFrameLL1.ExistingSteelConnNameEnd == beam.ExistingSteelConnNameStart ||
                                BeaminFrameLL1.ExistingSteelConnNameStart == beam.ExistingSteelConnNameEnd)
                            {
                                ChangeSinceLast = true;
                                BeamsCollTemp.Add(PotlSuptFrameLL1Tmp.BeamsinFrame[BeaminFrameLL1C]);
                                PotlSuptFrameLL1Tmp.BeamsinFrame.RemoveAt(BeaminFrameLL1C);
                                BeaminFrameLL1C--;
                            }
                            BeaminFrameLL1C++;
                        }
                    }
                }

                if (PotlSuptFrameLL1Tmp.BeamsinFrame.Count != 0)
                {
                    CollPotlSuptFrameDetails.RemoveAt(PotlSuptFrameLL1C);
                    PotlSuptFrameLL1C--;
                }
                PotlSuptFrameLL1C++;
            }

            PotlSuptFrameLL1C = 0;
            while (PotlSuptFrameLL1C < CollPotlSuptFrameDetails.Count)
            {
                PotlSuptFrameLL1 = CollPotlSuptFrameDetails[PotlSuptFrameLL1C];
                if (!PotlSuptFrameLL1.BeamsinFrame.Any(beam => beam.Dir != "U" && beam.Dir != "D"))
                {
                    CollPotlSuptFrameDetails.RemoveAt(PotlSuptFrameLL1C);
                    PotlSuptFrameLL1C--;
                }
                PotlSuptFrameLL1C++;
            }

            // Now reduce the list of frame routes to the 25 shortest routes with:
            // one steel tie-in
            // two / three / four steel tie-ins

            // Sort CollPotlSuptFrameDetails by frame steel length
            CollPotlSuptFrameDetails = CollPotlSuptFrameDetails.OrderBy(frame => frame.Length).ToList();

            List<cPotlSupt> CollPotlSuptFrameDetailsNew = new List<cPotlSupt>();
            foreach (var PotlSuptFrameLL1_2 in CollPotlSuptFrameDetails)
            {
                int tieInCount = PotlSuptFrameLL1_2.ConntoExistingSteelC.Count;
                if (tieInCount >= 1 && ArrExistingSteelTieInC[0] <= 50 ||
                    tieInCount >= 2 && ArrExistingSteelTieInC[1] <= 50 ||
                    tieInCount >= 3 && ArrExistingSteelTieInC[2] <= 50 ||
                    tieInCount >= 4 && ArrExistingSteelTieInC[3] <= 50 ||
                    tieInCount >= 5 && ArrExistingSteelTieInC[4] <= 50)
                {
                    CollPotlSuptFrameDetailsNew.Add(PotlSuptFrameLL1_2);
                    ArrExistingSteelTieInC[tieInCount - 1]++;
                }
            }

            CollPotlSuptFrameDetails = CollPotlSuptFrameDetailsNew;
        }

        public static List<cSteel> CopyCollBeamsinFrame(List<cSteel> CollBeamsinFrame)
        {
            List<cSteel> CollBeamsinFrameTmp = new List<cSteel>();

            foreach (var steel in CollBeamsinFrame)
            {
                CollBeamsinFrameTmp.Add(steel);
            }

            return CollBeamsinFrameTmp;
        }

        public static List<cSteel> CopyCollSuptBeams(List<cSteel> CollSuptBeamNos)
        {
            List<cSteel> CollSuptBeamNosTmp = new List<cSteel>();

            foreach (var steel in CollSuptBeamNos)
            {
                CollSuptBeamNosTmp.Add(steel);
            }

            return CollSuptBeamNosTmp;
        }
    }
}
