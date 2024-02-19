using SupA.Lib.Core;
using SupA.Lib.DataManipulation;
using SupA.Lib.Initialization;
using SupA.Lib.Utils;
using System.Collections.ObjectModel;

namespace SupA.Lib.RoutePotentialSuptFrames
{
    public class mAddRouteBranchesandOptions
    {
        public void AddRouteBranchesandOptions(object[,] ArrDirnstoAttempt, cPotlSupt CurrPotlSuptFrame, Collection<object> CollFrameNodeMapGrouped, Collection<object> CollPotlSuptFrameDetails, Collection<object> CollGroupNodeBeams, int NoofSuptBeamEndCoords, int NoofClosedRoutesWithTieIns, Collection<object> CollInvalidPotlSuptFrameDetails)
        {
            int LL2C;
            int LL3C;
            string BinarySeqToCompare;
            bool ValidBinary;
            int ValidBinaryCount;
            int NewSliceArrayNo;
            cPotlSupt NewPotlSuptFrame;
            int I;
            object DirnUntravelled;
            string CurrentDirnofTravel;
            int NodeFromNameNo;
            string NodefromName;
            int NodeToNameNo;
            string NodeToName;
            cGroupNode? NodeTo = null;
            bool InvalidRouteExistsFlag;
            string CurrentBeamStatus;
            cSteel? Beam = null;
            bool DuplicateTieIn;
            bool AdditionalFrameFeasibileCheck;
            float NoOfInvalidRoutesIgnored;
            Collection<object> ColltoExport;
            bool NodeUniqueFlag;

            ValidBinaryCount = 0;
            for (LL2C = 0; LL2C <= 63; LL2C++)
            {
                BinarySeqToCompare = mDec2Bin.Dec2Bin(LL2C, 6);
                ValidBinary = false;
                ValidBinary = CheckifBinaryRepresentsValidCombination(ArrDirnstoAttempt, BinarySeqToCompare);

                if (ValidBinary == true)
                {
                    ValidBinaryCount = ValidBinaryCount + 1;
                    for (I = 1; I <= 6; I++)
                    {
                        if (Convert.ToString(BinarySeqToCompare[I - 1]) == "1" && Convert.ToInt32(ArrDirnstoAttempt[I, 1]) != 0)
                        {
                            ArrDirnstoAttempt[I, 3] = "V";
                        }
                        else if (Convert.ToString(BinarySeqToCompare[I - 1]) == "0" && Convert.ToInt32(ArrDirnstoAttempt[I, 1]) != 0)
                        {
                            ArrDirnstoAttempt[I, 3] = "I";
                        }
                        else
                        {
                            ArrDirnstoAttempt[I, 3] = "";
                        }
                    }

                    NewPotlSuptFrame = new cPotlSupt();
                    NewPotlSuptFrame = CopyCurrPotlSuptFrame(CurrPotlSuptFrame);
                    SupAProgressBar.SupATotalRoutesExploredCount = SupAProgressBar.SupATotalRoutesExploredCount + 1;

                    if (ValidBinaryCount == 1 || ValidBinaryCount > 1)
                    {
                        NewPotlSuptFrame.PotlSuptNo = "potlsupt" + Convert.ToString(SupAProgressBar.SupATotalRoutesExploredCount);
                    }
                    else if (ValidBinaryCount == 0)
                    {
                        Console.WriteLine("this doesn't feel right - check me out!");
                    }

                    for (LL3C = 1; LL3C <= ArrDirnstoAttempt.GetUpperBound(0); LL3C++)
                    {
                        if (Convert.ToString(ArrDirnstoAttempt[LL3C, 3]) != "")
                        {
                            NewPotlSuptFrame.DirnsUntravelled.Add(Convert.ToString(ArrDirnstoAttempt[LL3C, 1]) + Convert.ToString(ArrDirnstoAttempt[LL3C, 3]));
                        }
                    }

                    InvalidRouteExistsFlag = false;
                    foreach (object DirnUntravelled_loopVariable in NewPotlSuptFrame.DirnsUntravelled)
                    {
                        DirnUntravelled = DirnUntravelled_loopVariable;
                        CurrentDirnofTravel = Convert.ToString(DirnUntravelled).Substring(Convert.ToString(DirnUntravelled).Length - 2, 1);
                        CurrentBeamStatus = Convert.ToString(DirnUntravelled).Substring(Convert.ToString(DirnUntravelled).Length - 1, 1);
                        NodefromName = Convert.ToString(DirnUntravelled).Substring(0, Convert.ToString(DirnUntravelled).Length - 2);

                        for (NodeFromNameNo = 1; NodeFromNameNo <= CollFrameNodeMapGrouped.Count; NodeFromNameNo++)
                        {
                            if (NodefromName == ((cGroupNode)CollFrameNodeMapGrouped[NodeFromNameNo]).GroupName)
                            {
                                break;
                            }
                        }

                        NodeToName = Convert.ToString(VbaInterop.CallByName(CollFrameNodeMapGrouped[NodeFromNameNo], "Conn" + CurrentDirnofTravel + "Dir", VbGet));

                        for (NodeToNameNo = 1; NodeToNameNo <= CollFrameNodeMapGrouped.Count; NodeToNameNo++)
                        {
                            if (NodeToName == ((cGroupNode)CollFrameNodeMapGrouped[NodeToNameNo]).GroupName)
                            {
                                NodeTo = (cGroupNode)CollFrameNodeMapGrouped[NodeToNameNo];
                                break;
                            }
                        }

                        foreach (cSteel Beam_loopVariable in CollGroupNodeBeams)
                        {
                            Beam = Beam_loopVariable;
                            if ((Beam.ExistingSteelConnNameEnd == NodefromName && Beam.ExistingSteelConnNameStart == NodeToName) || (Beam.ExistingSteelConnNameEnd == NodeToName && Beam.ExistingSteelConnNameStart == NodefromName))
                            {
                                break;
                            }
                        }

                        InvalidRouteExistsFlag = CheckifBeamisAlreadyInFrame(NewPotlSuptFrame, Beam, CurrentBeamStatus);
                        if (InvalidRouteExistsFlag == true)
                            break;

                        if (CurrentBeamStatus == "V")
                        {
                            NewPotlSuptFrame.BeamsinFrame.Add(Beam);
                            NodeUniqueFlag = true;
                            if (EnsureNodetoIsUniqueinPathsUntravelled(NewPotlSuptFrame.PathsUnTravelled, NodeTo) == true)
                            {
                                NewPotlSuptFrame.PathsUnTravelled.Add(NodeTo);
                            }
                            else
                            {
                                NodeUniqueFlag = false;
                            }
                            NewPotlSuptFrame.Length = NewPotlSuptFrame.Length + Beam.Length;
                            DuplicateTieIn = false;
                            foreach (var ExistingSteelTieIn in NewPotlSuptFrame.ConntoExistingSteelC)
                            {
                                if (ExistingSteelTieIn.GroupedNodes[1].AssocVerticalCol != "")
                                {
                                    if (ExistingSteelTieIn.GroupedNodes[1].Easting == NodeTo.GroupedNodes[1].Easting && ExistingSteelTieIn.GroupedNodes[1].Northing == NodeTo.GroupedNodes[1].Northing)
                                    {
                                        if (Math.Abs(NodeTo.GroupedNodes[1].Upping - ExistingSteelTieIn.GroupedNodes[1].Upping) < 400)
                                        {
                                            DuplicateTieIn = true;
                                        }
                                    }
                                }
                            }

                            if (DuplicateTieIn == false && NodeTo.GroupedNodes[1].AssocExistingSteel != "" && NodeUniqueFlag == true)
                            {
                                NewPotlSuptFrame.ConntoExistingSteelC.Add(NodeTo);
                            }
                        }
                        else if (CurrentBeamStatus == "I")
                        {
                            NewPotlSuptFrame.BeamsExcluded.Add(Beam);
                        }
                    }

                    AdditionalFrameFeasibileCheck = AdditionalFrameFeasibilityCheck(NewPotlSuptFrame, CollFrameNodeMapGrouped, NoofSuptBeamEndCoords);

                    if (InvalidRouteExistsFlag == false && AdditionalFrameFeasibileCheck == true)
                    {
                        CollPotlSuptFrameDetails.Add(NewPotlSuptFrame);
                    }
                    else if (InvalidRouteExistsFlag == true || AdditionalFrameFeasibileCheck == false)
                    {
                        NewPotlSuptFrame.IsClosed = true;
                        NewPotlSuptFrame.IsInvalid = true;
                        NoOfInvalidRoutesIgnored = SupAProgressBar.SupAInvalidRoutesBypassedCount + 1;
                        SupAProgressBar.SupAInvalidRoutesBypassedCount = NoOfInvalidRoutesIgnored;
                        CollInvalidPotlSuptFrameDetails.Add(NewPotlSuptFrame);
                        DoEvents();
                    }
                    else
                    {
                        Console.WriteLine("error in addroutebranchesandoptions");
                    }

                    if (NewPotlSuptFrame.PathsUnTravelled.Count == 0)
                    {
                        NewPotlSuptFrame.IsClosed = true;
                        if (NewPotlSuptFrame.ConntoExistingSteelC.Count > 1)
                        {
                            NoofClosedRoutesWithTieIns = NoofClosedRoutesWithTieIns + 1;
                            SupAProgressBar.SupANoofClosed = NoofClosedRoutesWithTieIns;
                            DoEvents();
                        }
                    }
                    NewPotlSuptFrame.DirnsUntravelled = new Collection<object>();
                }
            }
        }

        public bool EnsureNodetoIsUniqueinPathsUntravelled(Collection<object> PathsUnTravelled, cGroupNode NodeTo)
        {
            bool isUnique = true;
            foreach (cGroupNode GroupNode in PathsUnTravelled)
            {
                if (GroupNode.GroupName == NodeTo.GroupName)
                {
                    isUnique = false;
                    break;
                }
            }
            return isUnique;
        }

        public int CheckHowManyColumnsinFrame(cPotlSupt NewPotlSuptFrame, cSteel Beam, string CurrentBeamStatus)
        {
            var CoordStringENColl = NewPotlSuptFrame.CoordStringENColl;
            if (CurrentBeamStatus == "V")
            {
                bool CooordAlreadyExists = false;
                if (Beam.Dir == "U" || Beam.Dir == "D")
                {
                    string CoordStringENNew = Beam.StartE.ToString() + Beam.StartN.ToString();
                    foreach (string CoordStringENExisting in CoordStringENColl)
                    {
                        if (CoordStringENExisting == CoordStringENNew)
                        {
                            CooordAlreadyExists = true;
                            break;
                        }
                    }

                    if (!CooordAlreadyExists)
                    {
                        CoordStringENColl.Add(CoordStringENNew);
                    }
                }
            }
            return CoordStringENColl.Count;
        }

        public bool CheckifBeamisAlreadyInFrame(cPotlSupt NewPotlSuptFrame, cSteel Beam, string CurrentBeamStatus)
        {
            bool isAlreadyInFrame = false;
            foreach (cSteel ExistingBeam in NewPotlSuptFrame.BeamsinFrame)
            {
                if (Beam.ModelName == ExistingBeam.ModelName && CurrentBeamStatus == "V")
                {
                    isAlreadyInFrame = true;
                    break;
                }
            }

            if (!isAlreadyInFrame)
            {
                foreach (cSteel ExistingBeam in NewPotlSuptFrame.BeamsExcluded)
                {
                    if (Beam.ModelName == ExistingBeam.ModelName && CurrentBeamStatus == "V")
                    {
                        isAlreadyInFrame = true;
                        break;
                    }
                }
            }

            return isAlreadyInFrame;
        }

        public cPotlSupt CopyCurrPotlSuptFrame(cPotlSupt CurrPotlSuptFrame)
        {
            cPotlSupt NewPotlSuptFrame = new cPotlSupt();

            foreach (object mem in CurrPotlSuptFrame.PathsUnTravelled)
            {
                NewPotlSuptFrame.PathsUnTravelled.Add(mem);
            }

            foreach (object mem in CurrPotlSuptFrame.BeamsExcluded)
            {
                NewPotlSuptFrame.BeamsExcluded.Add(mem);
            }

            foreach (object mem in CurrPotlSuptFrame.DirnsUntravelled)
            {
                NewPotlSuptFrame.DirnsUntravelled.Add(mem);
            }

            foreach (object mem in CurrPotlSuptFrame.GroupNodesTravelled)
            {
                NewPotlSuptFrame.GroupNodesTravelled.Add(mem);
            }

            foreach (object mem in CurrPotlSuptFrame.GroupNodesinFrame)
            {
                NewPotlSuptFrame.GroupNodesinFrame.Add(mem);
            }

            foreach (object mem in CurrPotlSuptFrame.BeamsinFrame)
            {
                NewPotlSuptFrame.BeamsinFrame.Add(mem);
            }

            foreach (object mem in CurrPotlSuptFrame.LoadsonFrame)
            {
                NewPotlSuptFrame.LoadsonFrame.Add(mem);
            }

            foreach (cGroupNode mem in CurrPotlSuptFrame.ConntoExistingSteelC)
            {
                NewPotlSuptFrame.ConntoExistingSteelC.Add(mem);
            }

            foreach (object mem in CurrPotlSuptFrame.CoordStringENColl)
            {
                NewPotlSuptFrame.CoordStringENColl.Add(mem);
            }

            NewPotlSuptFrame.WeightPrelim = CurrPotlSuptFrame.WeightPrelim;
            NewPotlSuptFrame.WeightCalculated = CurrPotlSuptFrame.WeightCalculated;
            NewPotlSuptFrame.Length = CurrPotlSuptFrame.Length;
            NewPotlSuptFrame.IsClosed = CurrPotlSuptFrame.IsClosed;
            NewPotlSuptFrame.PotlSuptChildof = CurrPotlSuptFrame.PotlSuptNo;

            return NewPotlSuptFrame;
        }

        public bool CheckifBinaryRepresentsValidCombination(object[,] ArrDirnstoAttempt, string BinarySeqToCompare)
        {
            int CountertoTrue = 0;
            bool ValidBinary = false;

            for (int LL3C = 1; LL3C <= ArrDirnstoAttempt.Length; LL3C++)
            {
                if (((int)ArrDirnstoAttempt[LL3C, 1] == 0 && BinarySeqToCompare[LL3C - 1] == '0') ||
                    ((int)ArrDirnstoAttempt[LL3C, 1] != 0 && BinarySeqToCompare[LL3C - 1] == '0') ||
                    ((int)ArrDirnstoAttempt[LL3C, 1] != 0 && BinarySeqToCompare[LL3C - 1] == '1'))
                {
                    CountertoTrue++;
                }
            }

            if (CountertoTrue == 6)
            {
                ValidBinary = true;
            }

            return ValidBinary;
        }

        public bool AdditionalFrameFeasibilityCheck(cPotlSupt NewPotlSuptFrame, Collection<object> CollFrameNodeMapGrouped, int NoofSuptBeamEndCoords)
        {
            bool ValidFrame = true;
            bool InvalidColumnsFlag = false;
            bool IncompleteRouteFlag = false;

            foreach (cSteel Beam in NewPotlSuptFrame.BeamsinFrame)
            {
                foreach (cSteel BeamLL2 in NewPotlSuptFrame.BeamsinFrame)
                {
                    if ((Beam.Dir == "U" || Beam.Dir == "D") && (BeamLL2.Dir == "U" || BeamLL2.Dir == "D"))
                    {
                        if (Math.Abs(Beam.StartE - BeamLL2.StartE) == mSubInitializationSupA.pubIntDiscretisationStepSize ||
                            Math.Abs(Beam.StartN - BeamLL2.StartN) == mSubInitializationSupA.pubIntDiscretisationStepSize)
                        {
                            InvalidColumnsFlag = true;
                        }
                    }
                }
            }

            if (InvalidColumnsFlag)
            {
                ValidFrame = false;
            }

            foreach (cGroupNode GroupNode in NewPotlSuptFrame.GroupNodesinFrame)
            {
                int NoofBeamsNodeisOn = 0;

                foreach (cSteel Beam in NewPotlSuptFrame.BeamsinFrame)
                {
                    if (Beam.ExistingSteelConnNameStart == GroupNode.GroupName || Beam.ExistingSteelConnNameEnd == GroupNode.GroupName)
                    {
                        NoofBeamsNodeisOn++;
                    }
                }

                foreach (cGroupNode GroupNodeLL3 in NewPotlSuptFrame.PathsUnTravelled)
                {
                    if (GroupNodeLL3.GroupName == GroupNode.GroupName)
                    {
                        NoofBeamsNodeisOn++;
                    }
                }

                if (NoofBeamsNodeisOn <= 1)
                {
                    if (GroupNode.AssocExistingSteel == "" && GroupNode.AssocSuptBeam == "")
                    {
                        IncompleteRouteFlag = true;
                    }
                }
            }

            if (IncompleteRouteFlag)
            {
                ValidFrame = false;
            }

            if (NewPotlSuptFrame.CoordStringENColl.Count > NoofSuptBeamEndCoords)
            {
                ValidFrame = false;
            }

            return ValidFrame;
        }
    }
}
