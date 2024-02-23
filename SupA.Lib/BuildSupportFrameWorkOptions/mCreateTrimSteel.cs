using SupA.Lib.CoordinateAndAngleManipulation;
using SupA.Lib.Core;
using SupA.Lib.DataManipulation;
using SupA.Lib.Initialization;
using SupA.Lib.Models;

namespace SupA.Lib.BuildSupportFrameWorkOptions
{
    public class mCreateTrimSteel
    {
        public static List<cSteel> CreateTrimSteel(List<cSteel> CollSuptBeams, List<cSuptPoints> CollAdjacentSuptPoints, List<cSteelDisc> CollLocalExistingSteelDisc, List<cClashData> CollLocalClashData, int SuptGroupNo)
        {
            var CollTrimSteel = new List<cSteel>();
            cSteel TrimSteel;
            string ConnectingSteelEnd1, ConnectingSteelEnd2;

            foreach (cSteel SuptBeam in CollSuptBeams)
            {
                string PerpAxis1, PerpAxis2;
                int PerpAxis1No, PerpAxis2No, ParlAxisNo;
                mDefinePerpsandParls.DefinePerpsandParls(ref SuptBeam.Dir, out PerpAxis1, out PerpAxis2, out PerpAxis1No, out PerpAxis2No, out ParlAxisNo);

                foreach (cSteelDisc ExistingSteelNodeLL2 in CollLocalExistingSteelDisc)
                {
                    foreach (cSteelDisc ExistingSteelNodeLL3 in CollLocalExistingSteelDisc)
                    {
                        if (ExistingSteelNodeLL2.ExistingConnType == "CONCRETE" || ExistingSteelNodeLL3.ExistingConnType == "CONCRETE")
                        {
                            // Do nothing as trim steel is not required
                        }
                        else
                        {
                            float[] CoordArrayTrimEnd1 = mCreateCoordArray.CreateCoordArray(ExistingSteelNodeLL2, "cSteelDisc");
                            float[] CoordArrayTrimEnd2 = mCreateCoordArray.CreateCoordArray(ExistingSteelNodeLL3, "cSteelDisc");
                            float[] CoordArraySuptLocn = mCreateCoordArray.CreateCoordArray(CollAdjacentSuptPoints[1], "cSuptPoints");

                            if (Math.Round(CoordArrayTrimEnd1[PerpAxis1No], mSubInitializationSupA.pubIntDiscretisationStepDecPlaces) == Math.Round(CoordArrayTrimEnd2[PerpAxis1No], mSubInitializationSupA.pubIntDiscretisationStepDecPlaces) &&
                                Math.Round(CoordArrayTrimEnd1[PerpAxis2No], mSubInitializationSupA.pubIntDiscretisationStepDecPlaces) == Math.Round(CoordArrayTrimEnd2[PerpAxis2No], mSubInitializationSupA.pubIntDiscretisationStepDecPlaces) &&
                                Math.Round(CoordArrayTrimEnd1[ParlAxisNo], mSubInitializationSupA.pubIntDiscretisationStepDecPlaces) != Math.Round(CoordArrayTrimEnd2[ParlAxisNo], mSubInitializationSupA.pubIntDiscretisationStepDecPlaces))
                            {
                                if (Math.Round(CoordArraySuptLocn[PerpAxis1No], mSubInitializationSupA.pubIntDiscretisationStepDecPlaces) == Math.Round(CoordArrayTrimEnd1[PerpAxis1No], mSubInitializationSupA.pubIntDiscretisationStepDecPlaces))
                                {
                                    if (mDirectionCheck.DirectionCheck(ExistingSteelNodeLL2.DirnofBeam, mCalculateDirBasedonCoords.CalculateDirBasedonCoords(0, 0, 0, 0, 0, 0, CoordArrayTrimEnd1, CoordArrayTrimEnd2)) != "parallel" &&
                                        mDirectionCheck.DirectionCheck(ExistingSteelNodeLL3.DirnofBeam, mCalculateDirBasedonCoords.CalculateDirBasedonCoords(0, 0, 0, 0, 0, 0, CoordArrayTrimEnd1, CoordArrayTrimEnd2)) != "parallel")
                                    {
                                        ConnectingSteelEnd1 = ExistingSteelNodeLL2.Name;
                                        ConnectingSteelEnd2 = ExistingSteelNodeLL3.Name;

                                        bool ClashFlag = false;
                                        ClashFlag = mRunClashandClearanceCheck.RunClashandClearanceCheck(CollLocalClashData, CoordArrayTrimEnd1, CoordArrayTrimEnd2, new string[] { "S", "X", "C" });

                                        if (!ClashFlag)
                                        {
                                            bool TrimSteelValidFlag = true;
                                            foreach (cSteel existingTrimSteel in CollTrimSteel)
                                            {
                                                if ((mCoordinateCheck.CoordinateCheck("nodepoint", mCreateCoordArray.CreateCoordArray(existingTrimSteel, "cSteel", "start"), CoordArrayTrimEnd1) == true &&
                                                     mCoordinateCheck.CoordinateCheck("nodepoint", mCreateCoordArray.CreateCoordArray(existingTrimSteel, "cSteel", "end"), CoordArrayTrimEnd2)) == true ||
                                                    (mCoordinateCheck.CoordinateCheck("nodepoint", mCreateCoordArray.CreateCoordArray(existingTrimSteel, "cSteel", "start"), CoordArrayTrimEnd2) == true &&
                                                     mCoordinateCheck.CoordinateCheck("nodepoint", mCreateCoordArray.CreateCoordArray(existingTrimSteel, "cSteel", "end"), CoordArrayTrimEnd1)) == true)
                                                {
                                                    TrimSteelValidFlag = false;
                                                }
                                            }

                                            if (ConnectingSteelEnd1 == ConnectingSteelEnd2)
                                            {
                                                TrimSteelValidFlag = false;
                                            }

                                            if (TrimSteelValidFlag)
                                            {
                                                TrimSteel = WriteTrimSteelData(CoordArrayTrimEnd1, CoordArrayTrimEnd2, ConnectingSteelEnd1, ConnectingSteelEnd2, SuptBeam);
                                                CollTrimSteel.Add(TrimSteel);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            // Rationalize trim steel collection
            RationaliseTrimSteelColl(CollTrimSteel, CollLocalExistingSteelDisc, CollAdjacentSuptPoints);

            if (mSubInitializationSupA.pubBOOLTraceOn == true)
            {
                mExportColltoCSVFile<cSteel>.ExportColltoCSVFile(CollTrimSteel, "CollTrimSteel-F" + (SuptGroupNo + mSubInitializationSupA.SuptGroupNoMod), "csv");
            }

            return CollTrimSteel;
        }

        private static cSteel WriteTrimSteelData(float[] CoordArrayTrimEnd1, float[] CoordArrayTrimEnd2, string ConnectingSteelEnd1, string ConnectingSteelEnd2, cSteel SuptBeam)
        {
            cSteel TrimSteel = new cSteel();

            // Pull out perpAxis information to write to our beam
            string PerpAxis1, PerpAxis2;
            int PerpAxis1No, PerpAxis2No, ParlAxisNo;
            mDefinePerpsandParls.DefinePerpsandParls(ref SuptBeam.Dir, out PerpAxis1, out PerpAxis2, out PerpAxis1No, out PerpAxis2No, out ParlAxisNo);

            // Set the end at smaller co-ordinates to be our start, as per standard convention
            string StartDef = (CoordArrayTrimEnd1[0] + CoordArrayTrimEnd1[1] + CoordArrayTrimEnd1[2]) < (CoordArrayTrimEnd2[0] + CoordArrayTrimEnd2[1] + CoordArrayTrimEnd2[2]) ? "End1" : "End2";

            // Then add all attributes to the trim steel
            if (StartDef == "End1")
            {
                TrimSteel.StartE = CoordArrayTrimEnd1[0];
                TrimSteel.StartN = CoordArrayTrimEnd1[1];
                TrimSteel.StartU = CoordArrayTrimEnd1[2];
                TrimSteel.EndE = CoordArrayTrimEnd2[0];
                TrimSteel.EndN = CoordArrayTrimEnd2[1];
                TrimSteel.EndU = CoordArrayTrimEnd2[2];
                TrimSteel.ExistingSteelConnNameStart = ConnectingSteelEnd1;
                TrimSteel.ExistingSteelConnNameEnd = ConnectingSteelEnd2;
            }
            else
            {
                TrimSteel.StartE = CoordArrayTrimEnd2[0];
                TrimSteel.StartN = CoordArrayTrimEnd2[1];
                TrimSteel.StartU = CoordArrayTrimEnd2[2];
                TrimSteel.EndE = CoordArrayTrimEnd1[0];
                TrimSteel.EndN = CoordArrayTrimEnd1[1];
                TrimSteel.EndU = CoordArrayTrimEnd1[2];
                TrimSteel.ExistingSteelConnNameStart = ConnectingSteelEnd2;
                TrimSteel.ExistingSteelConnNameEnd = ConnectingSteelEnd1;
            }

            TrimSteel.StartERounded = mPublicVarDefinitions.RoundDecPlc(TrimSteel.StartE, mSubInitializationSupA.pubIntDiscretisationStepDecPlaces);
            TrimSteel.StartNRounded = mPublicVarDefinitions.RoundDecPlc(TrimSteel.StartN, mSubInitializationSupA.pubIntDiscretisationStepDecPlaces);
            TrimSteel.StartURounded = mPublicVarDefinitions.RoundDecPlc(TrimSteel.StartU, mSubInitializationSupA.pubIntDiscretisationStepDecPlaces);
            TrimSteel.EndERounded = mPublicVarDefinitions.RoundDecPlc(TrimSteel.EndE, mSubInitializationSupA.pubIntDiscretisationStepDecPlaces);
            TrimSteel.EndNRounded = mPublicVarDefinitions.RoundDecPlc(TrimSteel.EndN, mSubInitializationSupA.pubIntDiscretisationStepDecPlaces);
            TrimSteel.EndURounded = mPublicVarDefinitions.RoundDecPlc(TrimSteel.EndU, mSubInitializationSupA.pubIntDiscretisationStepDecPlaces);
            TrimSteel.Length = (float)Math.Pow(Math.Pow(TrimSteel.StartE - TrimSteel.EndE, 2) + Math.Pow(TrimSteel.StartN - TrimSteel.EndN, 2) + Math.Pow(TrimSteel.StartU - TrimSteel.EndU, 2), 0.5);

            TrimSteel.Dir = SuptBeam.Dir;
            TrimSteel.MinorAxisGlobaldir = PerpAxis2;
            TrimSteel.MajorAxisGlobaldir = PerpAxis1;

            TrimSteel.FeatureDesc = "TOPC";
            TrimSteel.Jusline = "CTOP";
            TrimSteel.SuptSteelFunction = "trimsteel";

            return TrimSteel;
        }

        public static void RationaliseTrimSteelColl(List<cSteel> CollSteel, List<cSteelDisc> CollLocalExistingSteelDisc, List<cSuptPoints> CollAdjacentSuptPoints)
        {
            cSteel Steel;
            cSteel SteelLL1;
            cSteelDisc SteelDiscLL3;
            int LL1, LL2, LL3, CollMemCount;

            // Logic for leaving only the shortest pieces of each trim steel in the collection
            CollMemCount = CollSteel.Count;
            LL1 = 1;
            while (LL1 <= CollMemCount)
            {
                Steel = CollSteel[LL1];
                for (LL2 = 1; LL2 <= CollSteel.Count; LL2++)
                {
                    SteelLL1 = CollSteel[LL2];
                    if (LL2 != LL1)
                    {
                        if (mCoordinateCheck.CoordinateCheck("withinbounds", mCreateCoordArray.CreateCoordArray(SteelLL1, "cSteel", "start", 0, 0, 0, null, true), mCreateCoordArray.CreateCoordArray(Steel, "cSteel", "start", 0, 0, 0, null, true), mCreateCoordArray.CreateCoordArray(Steel, "cSteel", "end", 0, 0, 0, null, true))
                            && mCoordinateCheck.CoordinateCheck("withinbounds", mCreateCoordArray.CreateCoordArray(SteelLL1, "cSteel", "end", 0, 0, 0, null, true), mCreateCoordArray.CreateCoordArray(Steel, "cSteel", "start", 0, 0, 0, null, true), mCreateCoordArray.CreateCoordArray(Steel, "cSteel", "end", 0, 0, 0, null, true)))
                        {
                            CollSteel.RemoveAt(LL1);
                            LL1--;
                            CollMemCount = CollSteel.Count;
                            break;
                        }
                    }
                }
                LL1++;
            }

            // Logic for leaving only one trim steel for every trim steel which has matching start and end beam references
            CollMemCount = CollSteel.Count;
            LL1 = 1;
            while (LL1 <= CollMemCount)
            {
                Steel = (cSteel)CollSteel[LL1];

                // Define perpendicular axes
                string PerpAxis1, PerpAxis2;
                int PerpAxis1No, PerpAxis2No, ParlAxisNo;
                mDefinePerpsandParls.DefinePerpsandParls(ref Steel.Dir, out PerpAxis1, out PerpAxis2, out PerpAxis1No, out PerpAxis2No, out ParlAxisNo);

                for (LL2 = 1; LL2 <= CollSteel.Count; LL2++)
                {
                    SteelLL1 = CollSteel[LL2];

                    if (mCreateCoordArray.CreateCoordArray(SteelLL1, "cSteel", "start", 0, 0, 0, null, true)[PerpAxis1No] == mCreateCoordArray.CreateCoordArray(Steel, "cSteel", "start", 0, 0, 0, null, true)[PerpAxis1No]
                        && Steel.Dir == SteelLL1.Dir
                        && (Steel.ExistingSteelConnNameEnd == SteelLL1.ExistingSteelConnNameEnd || Steel.ExistingSteelConnNameStart == SteelLL1.ExistingSteelConnNameStart))
                    {
                        if (LL2 != LL1)
                        {
                            // Remove trim steel if its length is less than 300
                            if (Steel.Length <= 300)
                            {
                                CollSteel.RemoveAt(LL1);
                                LL1--;
                                CollMemCount = CollSteel.Count;
                                break;
                            }
                            // Remove other trim steel if its length is less than 300
                            else if (SteelLL1.Length <= 300)
                            {
                                CollSteel.RemoveAt(LL2);
                                LL1 = 1;
                                CollMemCount = CollSteel.Count;
                                break;
                            }
                            // Remove trim steel if its length is shorter or if it ties into the same steel on both ends and its perpAxis2No coordinate is lower
                            else if (mCreateCoordArray.CreateCoordArray(Steel, "cSteel", "start", 0, 0, 0, null, true)[PerpAxis2No] <= mCreateCoordArray.CreateCoordArray(SteelLL1, "cSteel", "start", 0, 0, 0, null, true)[PerpAxis2No]
                                && (Steel.Length >= SteelLL1.Length || (Steel.ExistingSteelConnNameEnd == SteelLL1.ExistingSteelConnNameEnd && Steel.ExistingSteelConnNameStart == SteelLL1.ExistingSteelConnNameStart)))
                            {
                                CollSteel.RemoveAt(LL1);
                                LL1--;
                                CollMemCount = CollSteel.Count;
                                break;
                            }
                            // Remove other trim steel if its length is shorter or if it ties into the same steel on both ends and its perpAxis2No coordinate is higher
                            else if (mCreateCoordArray.CreateCoordArray(Steel, "cSteel", "start", 0, 0, 0, null, true)[PerpAxis2No] >= mCreateCoordArray.CreateCoordArray(SteelLL1, "cSteel", "start", 0, 0, 0, null, true)[PerpAxis2No]
                                && (Steel.Length <= SteelLL1.Length || (Steel.ExistingSteelConnNameEnd == SteelLL1.ExistingSteelConnNameEnd && Steel.ExistingSteelConnNameStart == SteelLL1.ExistingSteelConnNameStart)))
                            {
                                CollSteel.RemoveAt(LL2);
                                LL1 = 1;
                                CollMemCount = CollSteel.Count;
                                break;
                            }
                            else
                            {
                                Console.WriteLine("Error in rationalising trim steel");
                            }
                        }
                    }
                }
                LL1++;
            }

            int EntireLenShadowedCnt;
            float trimE, trimN, trimU, AveSuptU = 0;

            // Calculate average support elevation
            for (LL1 = 0; LL1 < CollAdjacentSuptPoints.Count; LL1++)
            {
                AveSuptU = (CollAdjacentSuptPoints[LL1].ElSuptPoint + ((LL1 - 1) * AveSuptU)) / (LL1 + 1);
                if (LL1 == 1)
                {
                    // Check condition if needed
                }
            }

            // Iterate over CollSteel
            for (LL1 = 0; LL1 < CollSteel.Count; LL1++)
            {
                SteelLL1 = CollSteel[LL1];
                EntireLenShadowedCnt = 0;

                // Check for each discrete point along the length of SteelLL1
                for (LL2 = 0; LL2 <= SteelLL1.Length / mSubInitializationSupA.pubIntDiscretisationStepSize; LL2++)
                {
                    trimE = SteelLL1.StartERounded;
                    trimN = SteelLL1.StartNRounded;
                    trimU = SteelLL1.StartURounded;

                    if (SteelLL1.Dir == "N") trimN += mSubInitializationSupA.pubIntDiscretisationStepSize * LL2;
                    if (SteelLL1.Dir == "E") trimE += mSubInitializationSupA.pubIntDiscretisationStepSize * LL2;

                    // Check against existing steel discs
                    for (LL3 = 0; LL3 < CollLocalExistingSteelDisc.Count; LL3++)
                    {
                        SteelDiscLL3 = CollLocalExistingSteelDisc[LL3];

                        // Check if trim is hidden from support along its entire length by an existing steel
                        if (SteelDiscLL3.Easting == trimE && SteelDiscLL3.Northing == trimN)
                        {
                            // Beams only work as shadows if they are between the shoe and the trim steel location
                            if (((trimU - AveSuptU) > 0 && (SteelDiscLL3.Upping - AveSuptU) < (trimU - AveSuptU)) ||
                                ((trimU - AveSuptU) < 0 && (SteelDiscLL3.Upping - AveSuptU) > (trimU - AveSuptU)))
                            {
                                EntireLenShadowedCnt++;
                                break;
                            }
                        }
                    }
                }

                // If EntireLenShadowedCnt equals the number of discrete points along the beam, remove the trim definition
                if (EntireLenShadowedCnt == (SteelLL1.Length / mSubInitializationSupA.pubIntDiscretisationStepSize) + 1)
                {
                    CollSteel.RemoveAt(LL1);
                    LL1--;
                }
            }

            // Renaming trim steel models
            for (LL1 = 1; LL1 <= CollSteel.Count; LL1++)
            {
                Steel = CollSteel[LL1];
                Steel.ModelName = "trimsteel" + LL1;
            }
        }
    }
}
