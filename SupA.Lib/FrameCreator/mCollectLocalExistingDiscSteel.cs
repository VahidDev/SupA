using Microsoft.VisualBasic;
using SupA.Lib.CoordinateAndAngleManipulation;
using SupA.Lib.Core;
using SupA.Lib.DataManipulation;
using SupA.Lib.Initialization;

namespace SupA.Lib.FrameCreator
{
    public class mCollectLocalExistingDiscSteel
    {
        public List<object> CollectLocalExistingDiscSteel(List<cSuptPoints> CollAdjacentSuptPoints, List<object> CollExistingSteelDisc)
        {
            // Definition of all function specific private variables
            List<object> coll = new List<object>();
            float[] SuptCoordArrLL;
            float[] SuptCoordArrUL;
            float[] CoordArrLL;
            float[] CoordArrUL;

            // These define what directions are perpendicular and parallel to our pipe
            string PerpAxis1;
            string PerpAxis2;
            int PerpAxis1No;
            int PerpAxis2No;
            int ParlAxisNo;
            mDefinePerpsandParls.DefinePerpsandParls(ref (CollAdjacentSuptPoints[1]).Tubidir, out PerpAxis1, out PerpAxis2, out PerpAxis1No, out PerpAxis2No, out ParlAxisNo);

            // Also define starting coordinates for our search box
            CoordArrLL = mCreateCoordArray.CreateCoordArray(CollAdjacentSuptPoints[1], "cSuptPoints");
            CoordArrUL = mCreateCoordArray.CreateCoordArray(CollAdjacentSuptPoints[1], "cSuptPoints");

            // Now work through all the supports in the group and identify what are the lower and upper limits of a box surrounding them
            // (and extended by the search limits defined in pubTblDefinitionofLocalExistingSteel)
            foreach (cSuptPoints Supt in CollAdjacentSuptPoints)
            {
                SuptCoordArrLL = mCreateCoordArray.CreateCoordArray(Supt, "cSuptPoints", null, 0, 0, 0, ReturnAdjStlSearchBoxDefinedinENU("negative", PerpAxis1No, PerpAxis2No, ParlAxisNo));
                SuptCoordArrUL = mCreateCoordArray.CreateCoordArray(Supt, "cSuptPoints", null, 0, 0, 0, ReturnAdjStlSearchBoxDefinedinENU("positive", PerpAxis1No, PerpAxis2No, ParlAxisNo));

                CoordArrLL[0] = Math.Min(SuptCoordArrLL[0], CoordArrLL[0]);
                CoordArrLL[1] = Math.Min(SuptCoordArrLL[1], CoordArrLL[1]);
                CoordArrLL[2] = Math.Min(SuptCoordArrLL[2], CoordArrLL[2]);

                CoordArrUL[0] = Math.Max(SuptCoordArrUL[0], CoordArrUL[0]);
                CoordArrUL[1] = Math.Max(SuptCoordArrUL[1], CoordArrUL[1]);
                CoordArrUL[2] = Math.Max(SuptCoordArrUL[2], CoordArrUL[2]);
            }

            // Now add all discretised steel inside the limits defined in the previous loop into a local discretised steel collection
            foreach (cSteelDisc DiscSteel in CollExistingSteelDisc)
            {
                if (mCoordinateCheck.CoordinateCheck("withinbounds", mCreateCoordArray.CreateCoordArray(DiscSteel, "cSteelDisc"), CoordArrLL, CoordArrUL))
                {
                    coll.Add(DiscSteel);
                }
            }

            // Now remove all disc steel which is in the shadow of another disc steel. This reduces the number of invalid routes I have to travel / weed out
            RemoveinShadowDiscSteel(CollAdjacentSuptPoints, coll);

            // Now call the function which works on concrete to remove all concrete grids that are not perpendicular to the support direction
            RemoveNonPerpdincularConcreteGrid(CollAdjacentSuptPoints, coll);

            // If pubBOOLTraceOn = True Then Call ExportColltoCSVFile(Coll, "CollLocalExistingSteelDisc", "csv")
            return coll;
        }

        public void RemoveNonPerpdincularConcreteGrid(List<cSuptPoints> CollAdjacentSuptPoints, List<object> CollExistingSteelDisc)
        {
            // This part of the logic removes any concrete grids which are parallel to our pipe to leave only the perpendicular ones
            // which is important for later parts of the code
            // The assumption for now is that adjacent supports are oriented in the same direction

            int LL1;
            int LL2;
            cSteelDisc SteelLL1Disc;
            cSuptPoints SuptLL2;
            int GridisParallelCount;

            // The steel in LL1 is the one we are deciding to remove if it is not perpendicular
            LL1 = 1;
            while (LL1 <= CollExistingSteelDisc.Count)
            {
                SteelLL1Disc = (cSteelDisc)CollExistingSteelDisc[LL1];
                LL2 = 1;
                // then we are checking for every support point
                GridisParallelCount = 0;
                while (LL2 <= CollAdjacentSuptPoints.Count)
                {
                    SuptLL2 = (cSuptPoints)CollAdjacentSuptPoints[LL2];
                    if (mDirectionCheck.DirectionCheck(SuptLL2.Tubidir, SteelLL1Disc.DirnofBeam) == "parallel")
                    {
                        GridisParallelCount++;
                    }
                    LL2++;
                }

                // and if LineofSightBlockCount matches the number of supports we have then none of them have line of sight to that steel
                // and the LL1 steel disc can go
                if (GridisParallelCount == CollAdjacentSuptPoints.Count)
                {
                    CollExistingSteelDisc.Remove(LL1);
                    LL1--;
                }
                LL1++;
            }
        }

        public void RemoveinShadowDiscSteel(List<cSuptPoints> CollAdjacentSuptPoints, List<object> CollExistingSteelDisc)
        {
            // This part of the logic removes any trim steels that are hidden along their entire length from our suptbeam by existing steel

            int LL1;
            int LL2;
            int LL3;
            cSteelDisc SteelLL1Disc;
            cSteelDisc SteelLL3Disc;
            cSuptPoints SuptLL2;
            float AveSuptU;
            bool LineofSightBlockBool;
            int LineofSightBlockCount;

            // The steel in LL1 is the one we are deciding to remove if it is blocked from
            // all supports (in the shadow of beams when viewed from any supports
            LL1 = 1;
            while (LL1 <= CollExistingSteelDisc.Count)
            {
                SteelLL1Disc = (cSteelDisc)CollExistingSteelDisc[LL1];
                LL2 = 1;
                // then we are checking for every support point
                LineofSightBlockCount = 0;
                while (LL2 <= CollAdjacentSuptPoints.Count)
                {
                    SuptLL2 = (cSuptPoints)CollAdjacentSuptPoints[LL2];
                    // any for each streel disc in LL3
                    LL3 = 1;
                    while (LL3 <= CollExistingSteelDisc.Count)
                    {
                        SteelLL3Disc = (cSteelDisc)CollExistingSteelDisc[LL3];
                        // if the LL3 disc steel blocks LL1 then increase LineofSightBlockCount by 1
                        // and exit do because we have done enough for this support point (only looking
                        // for one block per support point
                        LineofSightBlockBool = false;
                        LineofSightBlockBool = CheckShadowLogic(SteelLL1Disc, SuptLL2, SteelLL3Disc);
                        if (LineofSightBlockBool == true)
                        {
                            LineofSightBlockCount++;
                            break; // Exit the loop, we found a blocking steel
                        }
                        LL3++;
                    }
                    LL2++;
                }
                // and if LineofSightBlockCount matches the number of supports we have then none of them have line of sight to that steel
                // and the LL1 steel disc can go
                if (LineofSightBlockCount == CollAdjacentSuptPoints.Count)
                {
                    CollExistingSteelDisc.Remove(LL1);
                    LL1--;
                }
                LL1++;
            }
        }

        public bool CheckShadowLogic(cSteelDisc SteelLL1Disc, cSuptPoints SuptLL2, cSteelDisc SteelLL3Disc)
        {
            bool LineofSightBlockBool = false;

            // Check for Vertical Beams to see if they're in the shadow
            if ((SteelLL1Disc.DirnofBeam == "U" || SteelLL1Disc.DirnofBeam == "D") &&
                (SteelLL3Disc.DirnofBeam == "U" || SteelLL3Disc.DirnofBeam == "D"))
            {
                if (SteelLL1Disc.Easting == SteelLL3Disc.Easting ||
                    SteelLL1Disc.Northing == SteelLL3Disc.Northing)
                {
                    if (SteelLL1Disc.Upping == SteelLL3Disc.Upping)
                    {
                        if (SuptLL2.Tubidir == "N" || SuptLL2.Tubidir == "S")
                        {
                            if (((SuptLL2.EastingSuptPointRounded - SteelLL1Disc.Easting) > 0) &&
                                (SuptLL2.EastingSuptPointRounded > SteelLL3Disc.Easting) &&
                                (SteelLL3Disc.Easting > SteelLL1Disc.Easting))
                            {
                                LineofSightBlockBool = true;
                            }
                            if (((SteelLL1Disc.Easting - SuptLL2.EastingSuptPointRounded) > 0) &&
                                (SuptLL2.EastingSuptPointRounded < SteelLL3Disc.Easting) &&
                                (SteelLL3Disc.Easting < SteelLL1Disc.Easting))
                            {
                                LineofSightBlockBool = true;
                            }
                        }

                        if (SuptLL2.Tubidir == "E" || SuptLL2.Tubidir == "W")
                        {
                            if (((SuptLL2.NorthingSuptPointRounded - SteelLL1Disc.Northing) > 0) &&
                                (SuptLL2.NorthingSuptPointRounded > SteelLL3Disc.Northing) &&
                                (SteelLL3Disc.Northing > SteelLL1Disc.Northing))
                            {
                                LineofSightBlockBool = true;
                            }
                            if (((SteelLL1Disc.Northing - SuptLL2.NorthingSuptPointRounded) > 0) &&
                                (SuptLL2.NorthingSuptPointRounded < SteelLL3Disc.Northing) &&
                                (SteelLL3Disc.Northing < SteelLL1Disc.Northing))
                            {
                                LineofSightBlockBool = true;
                            }
                        }
                    }
                }
            }

            // Check for horizontal Beams. This is done by seeing if LL1 and LL2 eastings and northings match
            if ((SteelLL1Disc.DirnofBeam == "E" || SteelLL1Disc.DirnofBeam == "W" ||
                 SteelLL1Disc.DirnofBeam == "N" || SteelLL1Disc.DirnofBeam == "S") &&
                (SteelLL3Disc.DirnofBeam == "E" || SteelLL3Disc.DirnofBeam == "W" ||
                 SteelLL3Disc.DirnofBeam == "N" || SteelLL3Disc.DirnofBeam == "S"))
            {
                if (SteelLL1Disc.Easting == SteelLL3Disc.Easting ||
                    SteelLL1Disc.Northing == SteelLL3Disc.Northing)
                {
                    if ((SuptLL2.ElSuptPointRounded - SteelLL1Disc.Upping) > 0 &&
                        (SuptLL2.ElSuptPointRounded > SteelLL3Disc.Upping) &&
                        (SteelLL3Disc.Upping > SteelLL1Disc.Upping))
                    {
                        LineofSightBlockBool = true;
                    }
                    if ((SteelLL1Disc.Upping - SuptLL2.ElSuptPointRounded) > 0 &&
                        (SuptLL2.ElSuptPointRounded < SteelLL3Disc.Upping) &&
                        (SteelLL3Disc.Upping < SteelLL1Disc.Upping))
                    {
                        LineofSightBlockBool = true;
                    }
                }
            }

            return LineofSightBlockBool;
        }

        public static float[] ReturnAdjStlSearchBoxDefinedinENU(string LimitDirection, int PerpAxis1No, int PerpAxis2No, int ParlAxisNo)
        {
            // This function uses the public variable TblDefinitionofAdjacentSupport
            float[] CoordArray = new float[3];

            if (LimitDirection == "positive")
            {
                CoordArray[PerpAxis1No] = mSubInitializationSupA.pubTblDefinitionofLocalExistingSteel.DistPerp1toS;
                CoordArray[PerpAxis2No] = mSubInitializationSupA.pubTblDefinitionofLocalExistingSteel.DistPerp2toS;
                CoordArray[ParlAxisNo] = mSubInitializationSupA.pubTblDefinitionofLocalExistingSteel.DistParltoS;
            }
            else if (LimitDirection == "negative")
            {
                CoordArray[PerpAxis1No] = -1 * mSubInitializationSupA.pubTblDefinitionofLocalExistingSteel.DistPerp1toS;
                CoordArray[PerpAxis2No] = -1 * mSubInitializationSupA.pubTblDefinitionofLocalExistingSteel.DistPerp2toS;
                CoordArray[ParlAxisNo] = -1 * mSubInitializationSupA.pubTblDefinitionofLocalExistingSteel.DistParltoS;
            }
            else
            {
                Interaction.MsgBox("Error");
            }

            return CoordArray;
        }
    }
}
