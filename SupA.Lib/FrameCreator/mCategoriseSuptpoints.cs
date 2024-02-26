using SupA.Lib.CoordinateAndAngleManipulation;
using SupA.Lib.Core;
using SupA.Lib.DataManipulation;
using SupA.Lib.Initialization;

namespace SupA.Lib.FrameCreator
{
    public class mCategoriseSuptpoints
    {
        public static void CategoriseSuptpoints(List<cSuptPoints> CollAdjacentSuptPoints, ref int[,] arrNoofLevels, int SuptGroupNo)
        {
            cSuptPoints Supt;
            float[] TOSCoordArray;
            int[] SharedLevelNo = new int[4];

            string PerpAxis1;
            string PerpAxis2;
            int PerpAxis1No;
            int PerpAxis2No;
            int ParlAxisNo;

            for (int i = 0; i < CollAdjacentSuptPoints.Count; i++)
            {
                Supt = CollAdjacentSuptPoints[i];
                mDefinePerpsandParls.DefinePerpsandParls(ref Supt.Tubidir, out PerpAxis1, out PerpAxis2, out PerpAxis1No, out PerpAxis2No, out ParlAxisNo);

                TOSCoordArray = mCreateCoordArray.CreateCoordArray(Supt, "cSuptPoints", null, 0, 0, 0, mDecomposeThreeDVectorintoENUCoords.DecomposeThreeDVectorintoENUCoords(PerpAxis1, Supt.AnciHeiStd + Supt.PipeOD / 2));
                Supt.TOSPerpDir1pos = TOSCoordArray[PerpAxis1No];
                TOSCoordArray = mCreateCoordArray.CreateCoordArray(Supt, "cSuptPoints", null, 0, 0, 0, mDecomposeThreeDVectorintoENUCoords.DecomposeThreeDVectorintoENUCoords(PerpAxis1, -(Supt.AnciHeiStd + Supt.PipeOD / 2)));
                Supt.TOSPerpDir1neg = TOSCoordArray[PerpAxis1No];
                TOSCoordArray = mCreateCoordArray.CreateCoordArray(Supt, "cSuptPoints", null, 0, 0, 0, mDecomposeThreeDVectorintoENUCoords.DecomposeThreeDVectorintoENUCoords(PerpAxis2, Supt.AnciHeiStd + Supt.PipeOD / 2));
                Supt.TOSPerpDir2pos = TOSCoordArray[PerpAxis2No];
                TOSCoordArray = mCreateCoordArray.CreateCoordArray(Supt, "cSuptPoints", null, 0, 0, 0, mDecomposeThreeDVectorintoENUCoords.DecomposeThreeDVectorintoENUCoords(PerpAxis2, -(Supt.AnciHeiStd + Supt.PipeOD / 2)));
                Supt.TOSPerpDir2neg = TOSCoordArray[PerpAxis2No];
            }

            for (int LL1c = 0; LL1c < 4; LL1c++)
            {
                SharedLevelNo[LL1c] = 0;
                foreach (var SuptLL2 in CollAdjacentSuptPoints)
                {
                    if (!CheckifSharedLevelisSet(SuptLL2, LL1c))
                    {
                        SharedLevelNo[LL1c]++;
                        SetSharedLevel(SuptLL2, LL1c, SharedLevelNo[LL1c]);
                        foreach (var SuptLL3 in CollAdjacentSuptPoints)
                        {
                            if (!CheckifSharedLevelisSet(SuptLL3, LL1c) && mCoordinateCheck.CoordinateCheck("alongsingleaxis", mCreateCoordArray.CreateCoordArray(ReturnSharedLevelEltoCheck(SuptLL3, LL1c), "float"), mCreateCoordArray.CreateCoordArray(ReturnSharedLevelEltoCheck(SuptLL2, LL1c) + mSubInitializationSupA.pubTblDefinitionofAdjacentSupport.AllowableTOSDiff, "float"), mCreateCoordArray.CreateCoordArray(ReturnSharedLevelEltoCheck(SuptLL2, LL1c) - mSubInitializationSupA.pubTblDefinitionofAdjacentSupport.AllowableTOSDiff, "float")))
                            {
                                SetSharedLevel(SuptLL3, LL1c, SharedLevelNo[LL1c]);
                            }
                        }
                    }
                }
            }

            if (CollAdjacentSuptPoints[0].Tubidir == "N" || CollAdjacentSuptPoints[0].Tubidir == "S" || CollAdjacentSuptPoints[0].Tubidir == "W" || CollAdjacentSuptPoints[0].Tubidir == "E")
            {
                //arrNoofLevels[0] = 0; arrNoofLevels[1] = 0; arrNoofLevels[2] = 0; arrNoofLevels[3] = SharedLevelNo[3];
                // Converted for [,] instead of []:
                arrNoofLevels[0, 0] = 0; arrNoofLevels[0, 1] = 0; arrNoofLevels[0, 2] = 0; arrNoofLevels[0, 3] = SharedLevelNo[3];
            }
            else if (CollAdjacentSuptPoints[0].Tubidir == "U" || CollAdjacentSuptPoints[0].Tubidir == "D")
            {
                Console.WriteLine("The categorisesupportpoints functionality for dealing with non horizontal pipes has not yet been created");
            }
            else
            {
                Console.WriteLine("Error in categorisesupportpoints function");
            }

            //if (mSubInitializationSupA.pubBOOLTraceOn)
            mExportColltoCSVFile<cSuptPoints>.ExportColltoCSVFile(CollAdjacentSuptPoints, "CollAdjacentSuptPointswithLevels-F" + (SuptGroupNo + mSubInitializationSupA.SuptGroupNoMod).ToString(), "csv", null, true);
        }

        public static void SetSharedLevel(cSuptPoints Supt, int LL1c, int SharedLevelNo)
        {
            switch (LL1c)
            {
                case 1:
                    Supt.ShrdLvlNoinPerpDir1pos = SharedLevelNo;
                    break;
                case 2:
                    Supt.ShrdLvlNoinPerpDir1neg = SharedLevelNo;
                    break;
                case 3:
                    Supt.ShrdLvlNoinPerpDir2pos = SharedLevelNo;
                    break;
                case 4:
                    Supt.ShrdLvlNoinPerpDir2neg = SharedLevelNo;
                    break;
                default:
                    Console.WriteLine("error");
                    break;
            }
        }

        public static bool CheckifSharedLevelisSet(cSuptPoints Supt, int LL1c)
        {
            bool isSet = false;

            switch (LL1c)
            {
                case 1:
                    if (Supt.ShrdLvlNoinPerpDir1pos != 0) isSet = true;
                    break;
                case 2:
                    if (Supt.ShrdLvlNoinPerpDir1neg != 0) isSet = true;
                    break;
                case 3:
                    if (Supt.ShrdLvlNoinPerpDir2pos != 0) isSet = true;
                    break;
                case 4:
                    if (Supt.ShrdLvlNoinPerpDir2neg != 0) isSet = true;
                    break;
                default:
                    Console.WriteLine("error");
                    break;
            }
            return isSet;
        }

        public static float ReturnSharedLevelEltoCheck(cSuptPoints Supt, int LL1c)
        {
            float sharedLevel = 0;

            switch (LL1c)
            {
                case 1:
                    sharedLevel = Supt.TOSPerpDir1pos;
                    break;
                case 2:
                    sharedLevel = Supt.TOSPerpDir1neg;
                    break;
                case 3:
                    sharedLevel = Supt.TOSPerpDir2pos;
                    break;
                case 4:
                    sharedLevel = Supt.TOSPerpDir2neg;
                    break;
                default:
                    Console.WriteLine("error");
                    break;
            }

            return sharedLevel;
        }
    }
}
