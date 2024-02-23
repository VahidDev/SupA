using SupA.Lib.Core;
using SupA.Lib.Initialization;

namespace SupA.Lib.FrameCreator
{
    public class mDetailSuptFrame
    {
        public static void DetailSuptFrameFunc(List<cPotlSupt> CollPotlSuptFrameDetails, List<cSuptPoints> CollAdjacentSuptPoints, List<cSteel> CollExistingSteel)
        {
            // Clear all beam features
            TTblBeamDetailing[] BeamFeat = new TTblBeamDetailing[1];
            TTblConnDetailing[] BeamConnFeat = new TTblConnDetailing[1];
            BeamFeat[0] = new TTblBeamDetailing();
            cBasePlate BasePlate = new cBasePlate();

            string[] arrStartEnd = ["Start", "End"];

            foreach (var Frame in CollPotlSuptFrameDetails)
            {
                var CollBasePlate = new List<cBasePlate>();

                // Reset fabrication and material cost to zero
                Frame.FabricationCost = 0;
                Frame.MatlCost = 0;
                Frame.TotalCost = 0;

                for (int BeamLLC = 0; BeamLLC < Frame.BeamsinFrameratlized.Count; BeamLLC++)
                {
                    var Beam = Frame.BeamsinFrameratlized[BeamLLC];

                    // Do twice - once for the start and once for the end
                    for (int i = 0; i < 2; i++)
                    {
                        // Reset fabrication and material cost to zero
                        float FabricationCost = 0;
                        float MatlCost = 0;

                        

                        // Identify beam features
                        // This function is broken down into parts.
                        // In the first part we use some code to define our beam details, together with start and end definition.
                        // we identify the following features on each beam:
                        // 1. Profile family    ' 2.Beam Direction and Type    ' 3. Beam Start / End    ' 4.Conn Type at This End of Beam
                        // 5. Existing Steel Face (if applicable).    ' 6.Beam in Relation to Force Centroid(Perp.Dir)
                        IdentifyBeamFeatures(Beam, Frame, ref BeamFeat, ref BeamConnFeat, arrStartEnd[i], arrStartEnd, CollAdjacentSuptPoints, CollExistingSteel);

                        // Adjust beam ends
                        // Based on the features identified in part 1 of the code, we then set adjustments to the start and end adjustments of our beams
                        // together with beam rotations:


                        //7. Beam Start / End Dimn adjustment part 1 ( proportion of profile width) - in global coord
                        // 8. Beam Start / End Dimn adjustment part 1 ( proportion of profile depth) - in global coord
                        // 9. Beam Start / End Dimn adjustment part 1 ( proportion of profile depth) - in global coord
                        // 10. Beam Start / End Dimn adjustment part 1 ( proportion of existing Steel Width)
                        // 11. Beam Start / End Dimn adjustment part 1 ( proportion of existing Steel Depth
                        // 12. Beam Translation (in dir of pipe) - prop. of beam width
                        // 13. Beam Translation (in dir of pipe) - prop. of beam depth
                        // 14. Beam Translation (perp to pipe) - prop. of beam width
                        // 15. Beam Translation (U/D)  - prop. of beam depth
                        // 16. Rotation
                        AdjustBeamEnds(Beam, BeamFeat, CollAdjacentSuptPoints, ref FabricationCost, ref MatlCost);

                        // Detail beam connections
                        DetailBeamConns(Beam, BeamConnFeat, CollAdjacentSuptPoints, FabricationCost, MatlCost, arrStartEnd[i], BasePlate, ref CollBasePlate);

                        // Update frame fabrication, material, and total cost
                        Frame.FabricationCost += FabricationCost;
                        Frame.MatlCost += MatlCost;
                        Frame.TotalCost = Frame.FabricationCost + Frame.MatlCost;
                    }

                    // Remove beams with zero length
                    // I need to find a way of reversing the fabrication costs associated with zero length beams XYZ
                    // I haven't incorporated this for now
                    if (Beam.Length == 0)
                    {
                        Frame.BeamsinFrameratlized.RemoveAt(BeamLLC);
                        BeamLLC--;
                    }
                }

                Frame.BasePlateDetailsinFrame = CollBasePlate;
            }
        }

        private static void DetailBeamConns(cSteel Beam, TTblConnDetailing[] BeamConnFeat, List<cSuptPoints> CollAdjacentSuptPoints, float FabricationCost, float MatlCost, string StartEnd, cBasePlate BasePlate, ref List<cBasePlate> CollBasePlate)
        {
            int I = 1;
            string DirnOfPipe;
            string DirnPerptoPipe;
            double coordMods;
            float modNorthing;
            float modUpping;
            string PipDir = "";
            string PerpDir;
            bool MatchFoundinTbl = false;

            // Set up our pipedir and perpdir variables
            if (CollAdjacentSuptPoints[1].Tubidir == "E" || CollAdjacentSuptPoints[1].Tubidir == "W")
                PipDir = "E";
            if (CollAdjacentSuptPoints[1].Tubidir == "N" || CollAdjacentSuptPoints[1].Tubidir == "S")
                PipDir = "N";

            if (PipDir == "E")
                PerpDir = "N";
            if (PipDir == "N")
                PerpDir = "E";

            // This first section (the loop and if statement) is to help us find the
            // pubTblBeamDetailing entry that tells us what coordinate modifications to make
            // to our beam start / end.
            for (I = 1; I <= mSubInitializationSupA.pubTblConnDetailing.Length; I++)
            {
                if ((mSubInitializationSupA.pubTblConnDetailing[I].ProfileFamily == BeamConnFeat[1].ProfileFamily || mSubInitializationSupA.pubTblConnDetailing[1].ProfileFamily == "ANY") &&
                    (mSubInitializationSupA.pubTblConnDetailing[I].BeamDirAndDesc == BeamConnFeat[1].BeamDirAndDesc || mSubInitializationSupA.pubTblConnDetailing[1].BeamDirAndDesc == "ANY") &&
                    (mSubInitializationSupA.pubTblConnDetailing[I].BeamStartorEnd == BeamConnFeat[1].BeamStartorEnd || mSubInitializationSupA.pubTblConnDetailing[1].BeamStartorEnd == "Any") &&
                    mSubInitializationSupA.pubTblConnDetailing[I].ConnTypeAtCurrEndofBeam == BeamConnFeat[1].ConnTypeAtCurrEndofBeam &&
                    mSubInitializationSupA.pubTblConnDetailing[I].ExistingSteelFace == BeamConnFeat[1].ExistingSteelFace)
                {
                    // This flag is included to confirm that we have found a match in pubTblBeamDetailing.
                    // If not it means we need to expand our table definition
                    MatchFoundinTbl = true;

                    if (mSubInitializationSupA.pubTblConnDetailing[I].ReinforcingGussetReqd == "YES")
                    {
                        if (Beam.ConnDetailing == "")
                            Beam.ConnDetailing = StartEnd.ToString() + "-GUSSET";
                        else
                            Beam.ConnDetailing = Beam.ConnDetailing + "|" + StartEnd.ToString() + "-GUSSET";
                    }

                    if (mSubInitializationSupA.pubTblConnDetailing[I].BasePlateReqd == "YES")
                    {
                        BasePlate = new cBasePlate();
                        if (Beam.ConnDetailing == "")
                        {
                            Beam.ConnDetailing = StartEnd.ToString() + "-BASEPLATE";
                            WriteBasePlateDetails(StartEnd, BasePlate, Beam);
                        }
                        else
                        {
                            Beam.ConnDetailing = Beam.ConnDetailing + "|" + StartEnd.ToString() + "-BASEPLATE";
                            WriteBasePlateDetails(StartEnd, BasePlate, Beam);
                        }
                        CollBasePlate.Add(BasePlate);
                    }
                }
            }

            // Before exiting this sub we add a message box to alert if a match was not found
            if (!MatchFoundinTbl)
            {
                // XYZ MessageBox.Show("pubTblBeamDetailing missing relevant entry");
            }
        }

        private static void WriteBasePlateDetails(object StartEnd, cBasePlate BasePlate, cSteel Beam)
        {
            int I;

            // First write our start and end coordinate details
            if (StartEnd.ToString() == "Start")
            {
                BasePlate.East = Beam.StartE;
                BasePlate.North = Beam.StartN;
                BasePlate.Up = Beam.StartU;
                BasePlate.GroupNodeNo = Beam.ExistingSteelConnNameStart;
            }
            else if (StartEnd.ToString() == "End")
            {
                BasePlate.East = Beam.EndE;
                BasePlate.North = Beam.EndN;
                BasePlate.Up = Beam.EndU;
                BasePlate.GroupNodeNo = Beam.ExistingSteelConnNameEnd;
            }
            else
            {
                // XYZ MessageBox.Show("StartEnd Not correctly Defined");
            }

            // Now write our baseplate details
            for (I = 1; I <= mSubInitializationSupA.pubTblBasePlateDef.Length; I++)
            {
                if (mSubInitializationSupA.pubTblBasePlateDef[I].ProfileName == Beam.MemType)
                {
                    BasePlate.BasePlateThk = mSubInitializationSupA.pubTblBasePlateDef[I].BasePlateThk;
                    BasePlate.BasePlateWidth = mSubInitializationSupA.pubTblBasePlateDef[I].BasePlateWidth;
                    BasePlate.BasePlateLength = mSubInitializationSupA.pubTblBasePlateDef[I].BasePlateLength;
                    BasePlate.BasePlateBoltingReqd = mSubInitializationSupA.pubTblBasePlateDef[I].BasePlateBoltingReqd;
                    BasePlate.BoltSpacing = mSubInitializationSupA.pubTblBasePlateDef[I].BoltSpacing;
                    BasePlate.BoltSize = mSubInitializationSupA.pubTblBasePlateDef[I].BoltSize;
                    BasePlate.PlinthThk = mSubInitializationSupA.pubTblBasePlateDef[I].PlinthThk;
                    BasePlate.PlinthWidth = mSubInitializationSupA.pubTblBasePlateDef[I].PlinthWidth;
                    BasePlate.PlinthLength = mSubInitializationSupA.pubTblBasePlateDef[I].PlinthLength;
                    break;
                }
            }
        }

        private static void AdjustBeamEnds(cSteel Beam, TTblBeamDetailing[] BeamFeat, List<cSuptPoints> CollAdjacentSuptPoints, ref float FabricationCost, ref float MatlCost)
        {
            int I = 1;
            string DirnOfPipe;
            string DirnPerptoPipe;
            double coordMods;
            float modNorthing;
            float modUpping;
            string PipDir = "";
            string PerpDir = "";
            bool MatchFoundinTbl = false;

            // Set up our pipedir and perpdir variables
            if (CollAdjacentSuptPoints[1].Tubidir == "E" || CollAdjacentSuptPoints[1].Tubidir == "W") PipDir = "E";
            if (CollAdjacentSuptPoints[1].Tubidir == "N" || CollAdjacentSuptPoints[1].Tubidir == "S") PipDir = "N";
            if (PipDir == "E") PerpDir = "N";
            if (PipDir == "N") PerpDir = "E";

            // This first section (the loop and if statement) is to help us find the
            // pubTblBeamDetailing entry that tells us what coordinate modifications to make
            // to our beam start / end.
            for (I = 1; I <= mSubInitializationSupA.pubTblBeamDetailing.Length; I++)
            {
                if (mSubInitializationSupA.pubTblBeamDetailing[I].ProfileFamily == BeamFeat[1].ProfileFamily &&
                    mSubInitializationSupA.pubTblBeamDetailing[I].BeamDirAndDesc == BeamFeat[1].BeamDirAndDesc &&
                    mSubInitializationSupA.pubTblBeamDetailing[I].BeamStartorEnd == BeamFeat[1].BeamStartorEnd &&
                    mSubInitializationSupA.pubTblBeamDetailing[I].ConnTypeAtCurrEndofBeam == BeamFeat[1].ConnTypeAtCurrEndofBeam &&
                    mSubInitializationSupA.pubTblBeamDetailing[I].ExistingSteelFace == BeamFeat[1].ExistingSteelFace &&
                    mSubInitializationSupA.pubTblBeamDetailing[I].BeaminRelationtoAvPosofSupts == BeamFeat[1].BeaminRelationtoAvPosofSupts)
                {
                    // This flag is included to confirm that we have found a match in pubTblBeamDetailing.
                    // If not, it means we need to expand our table definition
                    MatchFoundinTbl = true;

                    // First, let's define those coordMods that need to be applied in the beam direction
                    coordMods = 0 +
                        mSubInitializationSupA.pubTblBeamDetailing[I].BeamAdjustpart1BMwidth * Beam.SctnWidth +
                        mSubInitializationSupA.pubTblBeamDetailing[I].BeamAdjustpart1BMdepth * Beam.SctnDepth +
                        mSubInitializationSupA.pubTblBeamDetailing[I].BeamAdjustpart2const +
                        mSubInitializationSupA.pubTblBeamDetailing[I].BeamAdjustpart1ESdepth * BeamFeat[1].ExistingSteelBMDepth +
                        mSubInitializationSupA.pubTblBeamDetailing[I].BeamAdjustpart1ESwidth * BeamFeat[1].ExistingSteelBMWidth;

                    // Then make the relevant coordinate changes to the relevant end.
                    if (BeamFeat[1].BeamStartorEnd == "Start")
                    {
                        if (Beam.Dir == "E") Beam.StartE += (float)coordMods;
                        if (Beam.Dir == "N") Beam.StartN += (float)coordMods;
                        if (Beam.Dir == "U") Beam.StartU += (float)coordMods;
                        Beam.StartERounded = mPublicVarDefinitions.RoundDecPlc(Beam.StartE, mSubInitializationSupA.pubIntDiscretisationStepDecPlaces);
                        Beam.StartNRounded = mPublicVarDefinitions.RoundDecPlc(Beam.StartN, mSubInitializationSupA.pubIntDiscretisationStepDecPlaces);
                        Beam.StartURounded = mPublicVarDefinitions.RoundDecPlc(Beam.StartU, mSubInitializationSupA.pubIntDiscretisationStepDecPlaces);
                    }
                    else if (BeamFeat[1].BeamStartorEnd == "End")
                    {
                        if (Beam.Dir == "E") Beam.EndE += (float)coordMods;
                        if (Beam.Dir == "N") Beam.EndN += (float)coordMods;
                        if (Beam.Dir == "U") Beam.EndU += (float)coordMods;
                        Beam.EndERounded = mPublicVarDefinitions.RoundDecPlc(Beam.EndE, mSubInitializationSupA.pubIntDiscretisationStepDecPlaces);
                        Beam.EndNRounded = mPublicVarDefinitions.RoundDecPlc(Beam.EndN, mSubInitializationSupA.pubIntDiscretisationStepDecPlaces);
                        Beam.EndURounded = mPublicVarDefinitions.RoundDecPlc(Beam.EndU, mSubInitializationSupA.pubIntDiscretisationStepDecPlaces);
                    }

                    // Now let's calculate those coordMods which need to be made in the direction of the pipe
                    coordMods = 0 + mSubInitializationSupA.pubTblBeamDetailing[I].BeamTraninDirofPipeBMwidth * Beam.SctnWidth +
                        mSubInitializationSupA.pubTblBeamDetailing[I].BeamTraninDirofPipeBMdepth * Beam.SctnDepth;
                    if (PipDir == "E") coordMods += mSubInitializationSupA.pubTblBeamDetailing[I].CorrforEWPipesXofBeamDepth * Beam.SctnDepth;

                    if (BeamFeat[1].BeamStartorEnd == "Start")
                    {
                        if (PipDir == "E") Beam.StartE += (float)coordMods;
                        if (PipDir == "N") Beam.StartN += (float)coordMods;
                        if (PipDir == "U") Beam.StartU += (float)coordMods;
                        Beam.StartERounded = mPublicVarDefinitions.RoundDecPlc(Beam.StartE, mSubInitializationSupA.pubIntDiscretisationStepDecPlaces);
                        Beam.StartNRounded = mPublicVarDefinitions.RoundDecPlc(Beam.StartN, mSubInitializationSupA.pubIntDiscretisationStepDecPlaces);
                        Beam.StartURounded = mPublicVarDefinitions.RoundDecPlc(Beam.StartU, mSubInitializationSupA.pubIntDiscretisationStepDecPlaces);
                    }
                    else if (BeamFeat[1].BeamStartorEnd == "End")
                    {
                        if (PipDir == "E") Beam.EndE += (float)coordMods;
                        if (PipDir == "N") Beam.EndN += (float)coordMods;
                        if (PipDir == "U") Beam.EndU += (float)coordMods;
                        Beam.EndERounded = mPublicVarDefinitions.RoundDecPlc(Beam.EndE, mSubInitializationSupA.pubIntDiscretisationStepDecPlaces);
                        Beam.EndNRounded = mPublicVarDefinitions.RoundDecPlc(Beam.EndN, mSubInitializationSupA.pubIntDiscretisationStepDecPlaces);
                        Beam.EndURounded = mPublicVarDefinitions.RoundDecPlc(Beam.EndU, mSubInitializationSupA.pubIntDiscretisationStepDecPlaces);
                    }

                    // Then those coordMods which need to be made perpendicular to the pipe (horizontally)
                    // To be honest, I don't understand why this section is required as I reckon it's a duplicate of
                    // our beam end adjustments ? XYZ
                    coordMods = 0 + mSubInitializationSupA.pubTblBeamDetailing[I].BeamTranPerpToPipeBMwidth * Beam.SctnWidth +
                        mSubInitializationSupA.pubTblBeamDetailing[I].BeamTranPerpToPipeBMdepth * Beam.SctnDepth;

                    if (BeamFeat[1].BeamStartorEnd == "Start")
                    {
                        if (PerpDir == "E") Beam.StartE += (float)coordMods;
                        if (PerpDir == "N") Beam.StartN += (float)coordMods;
                        if (PerpDir == "U") Beam.StartU += (float)coordMods;
                        Beam.StartERounded = mPublicVarDefinitions.RoundDecPlc(Beam.StartE, mSubInitializationSupA.pubIntDiscretisationStepDecPlaces);
                        Beam.StartNRounded = mPublicVarDefinitions.RoundDecPlc(Beam.StartN, mSubInitializationSupA.pubIntDiscretisationStepDecPlaces);
                        Beam.StartURounded = mPublicVarDefinitions.RoundDecPlc(Beam.StartU, mSubInitializationSupA.pubIntDiscretisationStepDecPlaces);
                    }
                    else if (BeamFeat[1].BeamStartorEnd == "End")
                    {
                        if (PerpDir == "E") Beam.EndE += (float)coordMods;
                        if (PerpDir == "N") Beam.EndN += (float)coordMods;
                        if (PerpDir == "U") Beam.EndU += (float)coordMods;
                        Beam.EndERounded = mPublicVarDefinitions.RoundDecPlc(Beam.EndE, mSubInitializationSupA.pubIntDiscretisationStepDecPlaces);
                        Beam.EndNRounded = mPublicVarDefinitions.RoundDecPlc(Beam.EndN, mSubInitializationSupA.pubIntDiscretisationStepDecPlaces);
                        Beam.EndURounded = mPublicVarDefinitions.RoundDecPlc(Beam.EndU, mSubInitializationSupA.pubIntDiscretisationStepDecPlaces);
                    }

                    // And then finally those which need to be made in the U / D direction
                    coordMods = 0 + mSubInitializationSupA.pubTblBeamDetailing[I].BeamTranUDBMdepth * Beam.SctnDepth;
                    if (BeamFeat[1].BeamStartorEnd == "Start")
                    {
                        Beam.StartU += (float)coordMods;
                        Beam.StartURounded = mPublicVarDefinitions.RoundDecPlc(Beam.StartU, mSubInitializationSupA.pubIntDiscretisationStepDecPlaces);
                    }
                    else if (BeamFeat[1].BeamStartorEnd == "End")
                    {
                        Beam.EndU += (float)coordMods;
                        Beam.EndURounded = mPublicVarDefinitions.RoundDecPlc(Beam.EndU, mSubInitializationSupA.pubIntDiscretisationStepDecPlaces);
                    }
                    // The below assumes that all beams are orthogonal
                    if (BeamFeat[1].BeamStartorEnd == "End")
                    {
                        Beam.Length = (float)Math.Sqrt(Math.Pow(Beam.StartE - Beam.EndE, 2) + Math.Pow(Beam.StartN - Beam.EndN, 2) + Math.Pow(Beam.StartU - Beam.EndU, 2));
                    }
                    Beam.Bangle = mSubInitializationSupA.pubTblBeamDetailing[I].Rotation.ToString();
                    if (PipDir == "E" && Beam.Dir == "U") Beam.Bangle += 90;
                    // else if (PipDir == "E" && (Beam.Dir == "N" || Beam.Dir == "S")) Beam.Bangle += 180;

                    // Then fill in all of our simple stuff
                    Beam.Jusline = mSubInitializationSupA.pubTblBeamDetailing[I].Jusline.ToString();

                    // Finally, calculate the fabrication cost of this end connection.
                    // This calculated number then rolls up into a fabrication cost per potlsupt.
                    CalculateBeamCosts(ref FabricationCost, ref MatlCost, BeamFeat, Beam, BeamFeat[1].BeamStartorEnd);
                }
            }

            // Before exiting this sub, we add a message box to alert if a match was not found
            if (!MatchFoundinTbl)
            {
                // XYZ MsgBox "pubTblBeamDetailing missing relevant entry"
            }
        }

        private static void CalculateBeamCosts(ref float FabricationCost, ref float MatlCost, TTblBeamDetailing[] BeamFeat, cSteel Beam, string StartEnd)
        {
            // At the current level of complexity, the fabrication cost of any beam connection is taken to be the same at 2 x beam width & 2 x beam depth
            // this assumption becomes less accurate as the need for stiffeners  arises at larger pipe sizes but it is a starting point for now
            // 1/2 of the above is applied each time this function is passed through. This adds up to one complete consideration per beam connection.
            if (BeamFeat[1].ConnTypeAtCurrEndofBeam == "Suptbeam" ||
                BeamFeat[1].ConnTypeAtCurrEndofBeam == "Trim" ||
                BeamFeat[1].ConnTypeAtCurrEndofBeam == "ExistingSteel" ||
                BeamFeat[1].ConnTypeAtCurrEndofBeam == "Vertical (Start)" ||
                BeamFeat[1].ConnTypeAtCurrEndofBeam == "Vertical (End)")
            {
                FabricationCost = mSubInitializationSupA.pubmanHourCost * mSubInitializationSupA.pubFabricationHours * (Beam.SctnDepth + Beam.SctnWidth) / 1000;
            }
            else if (BeamFeat[1].ConnTypeAtCurrEndofBeam == "Free")
            {
                FabricationCost = 0;
            }
            else
            {
                // This part seems to indicate an error condition where the beam end type is not defined.
                // You might want to handle this case appropriately in your C# code.
                // For now, I'll just print an error message similar to the VBA code.
                Console.WriteLine("Beam end not defined");
            }

            // Set up our beam material cost so that it is only calculated once per beam (on the end)
            // the calculation is done on the end as this is when all the adjustments have been applied so is when the detailed length
            // is fully calculated
            if (StartEnd == "End")
            {
                MatlCost = (float)(Beam.Length * Beam.UnitWeightKgm * mSubInitializationSupA.pubMatlCostperkg / 1000);
            }
            else
            {
                MatlCost = 0;
            }
        }

        private static void IdentifyBeamFeatures(cSteel Beam, cPotlSupt Frame, ref TTblBeamDetailing[] BeamFeat, ref TTblConnDetailing[] BeamConnFeat, string StartEnd, string[] arrStartEnd, List<cSuptPoints> CollAdjacentSuptPoints, List<cSteel> CollExistingSteel)
        {
            // 1. Profile family
            BeamFeat[0].ProfileFamily = "";
            BeamFeat[0].ProfileFamily = Beam.MemTypeGeneric;
            BeamConnFeat[0].ProfileFamily = "";
            BeamConnFeat[0].ProfileFamily = Beam.MemTypeGeneric;

            // 2. Beam Direction and Type
            BeamFeat[0].BeamDirAndDesc = "";
            BeamConnFeat[0].BeamDirAndDesc = "";

            if (Beam.Dir == "U")
            {
                BeamFeat[0].BeamDirAndDesc = "VERT";
                BeamConnFeat[0].BeamDirAndDesc = "VERT";
            }

            if (Beam.Dir == "E" || Beam.Dir == "W" || Beam.Dir == "N" || Beam.Dir == "S")
            {
                if (IdentifyBeamFeaturesTrimBeam(Beam, Frame, arrStartEnd) == true)
                {
                    BeamFeat[0].BeamDirAndDesc = "PERP (TRIM)";
                    BeamConnFeat[0].BeamDirAndDesc = "PERP (TRIM)";
                }
                else
                {
                    BeamFeat[0].BeamDirAndDesc = "PERP (NOT TRIM)";
                    BeamConnFeat[0].BeamDirAndDesc = "PERP (NOT TRIM)";
                }
            }

            // 3. Beam Start / End
            BeamFeat[0].BeamStartorEnd = "";
            BeamFeat[0].BeamStartorEnd = StartEnd.ToString();
            BeamConnFeat[0].BeamStartorEnd = "";
            BeamConnFeat[0].BeamStartorEnd = StartEnd.ToString();

            // 4. Conn Type at This End of Beam
            BeamFeat[0].ConnTypeAtCurrEndofBeam = "";
            BeamFeat[0].ConnTypeAtCurrEndofBeam = IdentifyBeamFeaturesEndConn(Beam, Frame, StartEnd);
            BeamConnFeat[0].ConnTypeAtCurrEndofBeam = "";
            BeamConnFeat[0].ConnTypeAtCurrEndofBeam = IdentifyBeamFeaturesEndConn(Beam, Frame, StartEnd);

            // 5. Existing Steel Face (if applicable). And also the existing steel beam depth and width
            BeamFeat[0].ExistingSteelFace = "";
            BeamFeat[0].ExistingSteelFace = IdentifyBeamFeaturesExistingSteelFace(Beam, Frame, StartEnd, BeamFeat, CollExistingSteel);
            BeamConnFeat[0].ExistingSteelFace = "";
            BeamConnFeat[0].ExistingSteelFace = IdentifyBeamFeaturesExistingSteelFace(Beam, Frame, StartEnd, BeamFeat, CollExistingSteel);

            // 6. Beam "End" in Relation to Average Position of Supports (Perp. Dir)
            BeamFeat[0].BeaminRelationtoAvPosofSupts = "";
            BeamFeat[0].BeaminRelationtoAvPosofSupts = IdentifyBeamFeaturesEndinRelationtoAvofCentrefofSupts(Beam, Frame, StartEnd, CollAdjacentSuptPoints);
        }

        private static string IdentifyBeamFeaturesExistingSteelFace(cSteel Beam, cPotlSupt Frame, object StartEnd, TTblBeamDetailing[] BeamFeat, List<cSteel> CollExistingSteel)
        {
            cGroupNode GroupN;
            string GroupNtoMtch = "";
            string ExistingSteelFace = "";

            if (StartEnd.ToString() == "Start")
            {
                GroupNtoMtch = Beam.ExistingSteelConnNameStart;
            }
            else if (StartEnd.ToString() == "End")
            {
                GroupNtoMtch = Beam.ExistingSteelConnNameEnd;
            }

            // Find group node which matches our end and then get the relevant assocexistingsteelface
            foreach (cGroupNode GroupN_2 in Frame.GroupNodesinFrame)
            {
                if (GroupN_2.GroupName == GroupNtoMtch)
                {
                    // Our existing steel face is a simple look up across from routenode 1 of our group node
                    ExistingSteelFace = GroupN_2.GroupedNodes[1].AssocExistingSteelFace;

                    // While we are here we go and loop find our existing steel depth and width from CollExistingSteel
                    // So if we have an existingsteelface in our lookup (that is, if we are tying in to existing steel
                    // / not returning NA from this function)
                    if (ExistingSteelFace != "")
                    {
                        foreach (cSteel ExistingSteel in CollExistingSteel)
                        {
                            if (ExistingSteel.ModelName == GroupN_2.GroupedNodes[1].AssocExistingSteel)
                            {
                                BeamFeat[0].ExistingSteelBMDepth = ExistingSteel.SctnDepth;
                                BeamFeat[0].ExistingSteelBMWidth = ExistingSteel.SctnWidth;
                                break;
                            }
                        }
                    }
                    break;
                }
            }

            if (ExistingSteelFace == "") ExistingSteelFace = "NA";

            return ExistingSteelFace;
        }

        private static string IdentifyBeamFeaturesEndConn(cSteel Beam, cPotlSupt Frame, object StartEnd)
        {
            string GroupNtoMtch = "";

            // Based on our StartEnd variable value, set our group node to match as either the beam start or end group node.
            if (StartEnd.ToString() == "Start")
            {
                GroupNtoMtch = Beam.ExistingSteelConnNameStart;
            }
            else if (StartEnd.ToString() == "End")
            {
                GroupNtoMtch = Beam.ExistingSteelConnNameEnd;
            }

            // Find group node which matches our end
            foreach (cGroupNode GroupN in Frame.GroupNodesinFrame)
            {
                if (GroupN.GroupName == GroupNtoMtch)
                {
                    // Once we have found our matching group node then work through routenode 1 of the group node to pull out the
                    // relevant features.
                    // If our beam direction is U then it can have the following end descriptions based on the AssocXXXX attribute on the route node
                    if (Beam.Dir == "U")
                    {
                        if (!string.IsNullOrEmpty(GroupN.GroupedNodes[1].AssocExistingSteel))
                        {
                            return "ExistingSteel";
                        }
                        else if (!string.IsNullOrEmpty(GroupN.GroupedNodes[1].AssocExtendedBeam) || !string.IsNullOrEmpty(GroupN.GroupedNodes[1].AssocSuptBeam))
                        {
                            return "Suptbeam";
                        }
                        else if (!string.IsNullOrEmpty(GroupN.GroupedNodes[1].AssocTrim))
                        {
                            return "Trim";
                        }
                    }
                    // If our beam direction is E or N then it can have the following end descriptions based on the AssocXXXX attribute on the route node
                    else
                    {
                        if (!string.IsNullOrEmpty(GroupN.GroupedNodes[1].AssocExistingSteel))
                        {
                            return "ExistingSteel";
                        }
                        else if (string.IsNullOrEmpty(GroupN.GroupedNodes[1].AssocExistingSteel) && string.IsNullOrEmpty(GroupN.GroupedNodes[1].AssocExtendedBeam) && string.IsNullOrEmpty(GroupN.GroupedNodes[1].AssocSuptBeam) && string.IsNullOrEmpty(GroupN.GroupedNodes[1].AssocTrim) && string.IsNullOrEmpty(GroupN.GroupedNodes[1].AssocVerticalCol))
                        {
                            return "Free";
                        }
                        else if (IdentifyBeamFeaturesFreeEndCheck(GroupN, Frame))
                        {
                            return "Free";
                        }
                        else if (!string.IsNullOrEmpty(GroupN.GroupedNodes[1].ConnUDir) && !string.IsNullOrEmpty(GroupN.GroupedNodes[1].AssocVerticalCol))
                        {
                            return "Vertical (Start)";
                        }
                        else if (!string.IsNullOrEmpty(GroupN.GroupedNodes[1].ConnDDir) && !string.IsNullOrEmpty(GroupN.GroupedNodes[1].AssocVerticalCol))
                        {
                            return "Vertical (End)";
                        }
                    }
                }
            }

            return "";
        }

        private static bool IdentifyBeamFeaturesFreeEndCheck(cGroupNode GroupN, cPotlSupt Frame)
        {
            bool IsthisFreeEndBool = true;
            int CountofConnsAwayfromnode = 0;

            foreach (cGroupNode groupNtoCheck in Frame.GroupNodesinFrame)
            {
                if (GroupN.ConnNDir == groupNtoCheck.GroupName)
                {
                    CountofConnsAwayfromnode++;
                }
                else if (GroupN.ConnSDir == groupNtoCheck.GroupName)
                {
                    CountofConnsAwayfromnode++;
                }
                else if (GroupN.ConnEDir == groupNtoCheck.GroupName)
                {
                    CountofConnsAwayfromnode++;
                }
                else if (GroupN.ConnWDir == groupNtoCheck.GroupName)
                {
                    CountofConnsAwayfromnode++;
                }
                else if (!string.IsNullOrEmpty(GroupN.ConnUDir) && GroupN.ConnUDir == groupNtoCheck.GroupName)
                {
                    IsthisFreeEndBool = false;
                }
                else if (!string.IsNullOrEmpty(GroupN.ConnDDir) && GroupN.ConnDDir == groupNtoCheck.GroupName)
                {
                    IsthisFreeEndBool = false;
                }
            }

            if (CountofConnsAwayfromnode > 1)
            {
                IsthisFreeEndBool = false;
            }

            return IsthisFreeEndBool;
        }

        private static bool IdentifyBeamFeaturesTrimBeam(cSteel Beam, cPotlSupt Frame, string[] arrStartEnd)
        {
            // Set default return value
            bool isTrimBeam = false;

            // Check both the start and end of our beam
            foreach (string StartEnd in arrStartEnd)
            {
                string GroupNtoMtch = "";
                bool StartisTrim = false;
                bool EndisTrim = false;

                if (StartEnd == "Start")
                {
                    // Find group node which matches our start
                    GroupNtoMtch = Beam.ExistingSteelConnNameStart;
                    foreach (cGroupNode GroupN in Frame.GroupNodesinFrame)
                    {
                        if (GroupN.GroupName == GroupNtoMtch && GroupN.GroupedNodes.Count > 0)
                        {
                            if (!string.IsNullOrEmpty(GroupN.GroupedNodes[0].AssocTrim))
                            {
                                // And if this group node has a value for assoctrim then set startistrim is true
                                StartisTrim = true;
                            }
                        }
                    }
                }
                else if (StartEnd == "End")
                {
                    // Find group node which matches our end
                    GroupNtoMtch = Beam.ExistingSteelConnNameEnd;
                    foreach (cGroupNode GroupN in Frame.GroupNodesinFrame)
                    {
                        if (GroupN.GroupName == GroupNtoMtch && GroupN.GroupedNodes.Count > 0)
                        {
                            if (!string.IsNullOrEmpty(GroupN.GroupedNodes[0].AssocTrim))
                            {
                                // And if this group node has a value for assoctrim then set endistrim is true
                                EndisTrim = true;
                            }
                        }
                    }
                }

                // And if both start and end are trim then this must be a trim
                if (EndisTrim && StartisTrim)
                {
                    isTrimBeam = true;
                }
            }

            return isTrimBeam;
        }

        private static string IdentifyBeamFeaturesEndinRelationtoAvofCentrefofSupts(cSteel Beam, cPotlSupt Frame, string StartEnd, List<cSuptPoints> CollAdjacentSuptPoints)
        {
            double SuptEastAv = 0;
            double SuptNorthingAv = 0;
            double SuptUppingAv = 0;
            int NoofSupts = 0;
            string PipDir = "";

            // This section below considers all supts in colladjacentsuptpoints and takes an average position of these.
            // This may be simplistic for now but it will do
            foreach (cSuptPoints Supt in CollAdjacentSuptPoints)
            {
                SuptEastAv = (SuptEastAv * NoofSupts + Supt.EastingSuptPoint) / (NoofSupts + 1);
                SuptNorthingAv = (SuptNorthingAv * NoofSupts + Supt.NorthingSuptPoint) / (NoofSupts + 1);
                SuptUppingAv = (SuptUppingAv * NoofSupts + Supt.ElSuptPoint) / (NoofSupts + 1);
                NoofSupts++;
                if (NoofSupts == 2)
                {
                    //MessageBox.Show("Here is an opportunity to check this code");
                }
                PipDir = Supt.Tubidir;
            }

            // If our pipdir is E or W then
            if (PipDir == "E" || PipDir == "W")
            {
                if (StartEnd == "Start")
                {
                    if (Beam.StartN <= SuptNorthingAv)
                    {
                        return "-";
                    }
                    else
                    {
                        return "+";
                    }
                }
                else if (StartEnd == "End")
                {
                    if (Beam.EndN <= SuptNorthingAv)
                    {
                        return "-";
                    }
                    else
                    {
                        return "+";
                    }
                }
            }
            // If our pipdir is N or S then
            else if (PipDir == "N" || PipDir == "S")
            {
                if (StartEnd == "Start")
                {
                    if (Beam.StartE <= SuptEastAv)
                    {
                        return "-";
                    }
                    else
                    {
                        return "+";
                    }
                }
                else if (StartEnd == "End")
                {
                    if (Beam.EndE <= SuptEastAv)
                    {
                        return "-";
                    }
                    else
                    {
                        return "+";
                    }
                }
            }

            return "";
        }
    }
}
