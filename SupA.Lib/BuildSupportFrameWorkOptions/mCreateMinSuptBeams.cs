using SupA.Lib.CoordinateAndAngleManipulation;
using SupA.Lib.Core;
using SupA.Lib.DataManipulation;
using SupA.Lib.Initialization;

namespace SupA.Lib.BuildSupportFrameWorkOptions
{
    public class mCreateMinSuptBeams
    {
        public static List<cSteel> CreateMinSuptBeams(List<cSuptPoints> CollAdjacentSuptPoints, int[,] arrNoofLevels, out int NoofSuptBeamEndCoords, int SuptGroupNo)
        {
            var CollSuptBeams = new List<cSteel>();
            int LLC1, LLC2;
            int BeamModelName = 1;
            float[] CoordArrayStart = new float[3];
            float[] CoordArrayEnd = new float[3];
            float[] CoordArrayCurrStart = new float[3];
            float[] CoordArrayCurrEnd = new float[3];
            string strCollofParlCoords;
            string ListofSupts;
            string PerpAxis1, PerpAxis2;
            int PerpAxis1No, PerpAxis2No, ParlAxisNo;

            mDefinePerpsandParls.DefinePerpsandParls(ref CollAdjacentSuptPoints[0].Tubidir, out PerpAxis1, out PerpAxis2, out PerpAxis1No, out PerpAxis2No, out ParlAxisNo);

            for (LLC1 = 0; LLC1 < arrNoofLevels.GetLength(0); LLC1++)
            {
                for (LLC2 = 1; LLC2 <= Convert.ToInt32(arrNoofLevels[LLC1, 0]); LLC2++)
                {
                    foreach (cSuptPoints Supt in CollAdjacentSuptPoints)
                    {
                        if (SuptLevelinPerpDir(Supt, LLC1 + 1) == LLC2)
                        {
                            CoordArrayStart = mCreateCoordArray.CreateCoordArray(Supt, "cSuptPoints");
                            CoordArrayEnd = CoordArrayStart;
                            break;
                        }
                    }
                    ListofSupts = "";

                    strCollofParlCoords = "";
                    foreach (cSuptPoints Supt in CollAdjacentSuptPoints)
                    {
                        if (SuptLevelinPerpDir(Supt, LLC1 + 1) == LLC2)
                        {
                            CoordArrayCurrStart = mCreateCoordArray.CreateCoordArray(Supt, "cSuptPoints", null, LLC1 + 1, PerpAxis1No, PerpAxis2No, mDecomposeThreeDVectorintoENUCoords.DecomposeThreeDVectorintoENUCoords(PerpAxis1, Supt.BeamLenReqd / 2, "allnegative"), true);
                            CoordArrayCurrEnd = mCreateCoordArray.CreateCoordArray(Supt, "cSuptPoints", null, LLC1 + 1, PerpAxis1No, PerpAxis2No, mDecomposeThreeDVectorintoENUCoords.DecomposeThreeDVectorintoENUCoords(PerpAxis1, Supt.BeamLenReqd / 2, "allpositive"), true);

                            CoordArrayStart[PerpAxis1No] = Math.Min(CoordArrayStart[PerpAxis1No], CoordArrayCurrStart[PerpAxis1No]);
                            CoordArrayEnd[PerpAxis1No] = Math.Max(CoordArrayEnd[PerpAxis1No], CoordArrayCurrEnd[PerpAxis1No]);

                            if (LLC1 == 1 || LLC1 == 3)
                            {
                                CoordArrayStart[PerpAxis2No] = Math.Min(CoordArrayStart[PerpAxis2No], CoordArrayCurrStart[PerpAxis2No]);
                                CoordArrayEnd[PerpAxis2No] = Math.Min(CoordArrayEnd[PerpAxis2No], CoordArrayCurrStart[PerpAxis2No]);
                            }
                            else if (LLC1 == 0 || LLC1 == 2)
                            {
                                CoordArrayStart[PerpAxis2No] = Math.Max(CoordArrayStart[PerpAxis2No], CoordArrayCurrStart[PerpAxis2No]);
                                CoordArrayEnd[PerpAxis2No] = Math.Max(CoordArrayEnd[PerpAxis2No], CoordArrayCurrStart[PerpAxis2No]);
                            }

                            if (strCollofParlCoords == "")
                                strCollofParlCoords = CoordArrayCurrStart[ParlAxisNo].ToString();
                            else
                                strCollofParlCoords += "," + CoordArrayCurrStart[ParlAxisNo];

                            if (ListofSupts == "")
                                ListofSupts = string.Join(",", Supt.CollSuptPointsinAreaRowNo);
                            else
                                ListofSupts += "|" + string.Join(",", Supt.CollSuptPointsinAreaRowNo);
                        }
                    }
                    var Beam = new cSteel();
                    float[] CollofParlCoords = Array.ConvertAll(strCollofParlCoords.Split(','), float.Parse);
                    CoordArrayStart[ParlAxisNo] = CollofParlCoords[0];
                    CoordArrayEnd[ParlAxisNo] = CollofParlCoords[0];
                    WriteCollSuptBeamsData(ref Beam, LLC1 + 1, LLC2, CoordArrayStart, CoordArrayEnd, PerpAxis1, PerpAxis2, ListofSupts, CollAdjacentSuptPoints[0], BeamModelName, mSubInitializationSupA.pubIntDiscretisationStepDecPlaces);
                    BeamModelName++;
                    CollSuptBeams.Add(Beam);
                }
            }

            if (mSubInitializationSupA.pubBOOLTraceOn)
            {
                mExportColltoCSVFile<cSteel>.ExportColltoCSVFile(CollSuptBeams, "CollSuptBeams-F" + (SuptGroupNo + mSubInitializationSupA.SuptGroupNoMod).ToString(), "csv");
                mExportArrtoCSVFile.ExportArrtoCSVFile(arrNoofLevels, "ArrNoofLevels-F" + (SuptGroupNo + mSubInitializationSupA.SuptGroupNoMod).ToString(), "csv");
            }

            var CollUniENCoords = new HashSet<string>();
            foreach (cSteel Beam in CollSuptBeams)
            {
                bool UniStartFlag = true;
                bool UniEndFlag = true;
                string Mem;

                foreach (string mem in CollUniENCoords)
                {
                    if ((Beam.StartE.ToString() + Beam.StartN.ToString()) != mem)
                        UniStartFlag = false;
                    if ((Beam.EndE.ToString() + Beam.EndN.ToString()) != mem)
                        UniStartFlag = false;
                }
                if (UniStartFlag)
                    CollUniENCoords.Add(Beam.StartE.ToString() + Beam.StartN.ToString());
                if (UniEndFlag)
                    CollUniENCoords.Add(Beam.EndE.ToString() + Beam.EndN.ToString());
            }
            NoofSuptBeamEndCoords = CollUniENCoords.Count;
            return CollSuptBeams;
        }

        private static void WriteCollSuptBeamsData(ref cSteel Beam, int LLC1, int LLC2, float[] CoordArrayStart, float[] CoordArrayEnd,
        string PerpAxis1, string PerpAxis2, string ListofSupts, cSuptPoints FirstSuptinCol, int BeamModelName, int pubIntDiscretisationStepDecPlaces)
        {
            // Write start and end data
            Beam.StartE = CoordArrayStart[0];
            Beam.StartN = CoordArrayStart[1];
            Beam.StartU = CoordArrayStart[2];
            Beam.EndE = CoordArrayEnd[0];
            Beam.EndN = CoordArrayEnd[1];
            Beam.EndU = CoordArrayEnd[2];

            // Write rounded start and end data
            Beam.StartERounded = mPublicVarDefinitions.RoundDecPlc(Beam.StartE, pubIntDiscretisationStepDecPlaces);
            Beam.StartNRounded = mPublicVarDefinitions.RoundDecPlc(Beam.StartN, pubIntDiscretisationStepDecPlaces);
            Beam.StartURounded = mPublicVarDefinitions.RoundDecPlc(Beam.StartU, pubIntDiscretisationStepDecPlaces);
            Beam.EndERounded = mPublicVarDefinitions.RoundDecPlc(Beam.EndE, pubIntDiscretisationStepDecPlaces);
            Beam.EndNRounded = mPublicVarDefinitions.RoundDecPlc(Beam.EndN, pubIntDiscretisationStepDecPlaces);
            Beam.EndURounded = mPublicVarDefinitions.RoundDecPlc(Beam.EndU, pubIntDiscretisationStepDecPlaces);

            // Write the beam directions
            Beam.Dir = mCalculateDirBasedonCoords.CalculateDirBasedonCoords(Beam.StartE, Beam.StartN, Beam.StartU, Beam.EndE, Beam.EndN, Beam.EndU);
            string PerpAxis1Beam, PerpAxis2Beam;
            int PerpAxis1No, PerpAxis2No, PerpAxisNo;
            mDefinePerpsandParls.DefinePerpsandParls(ref Beam.Dir, out PerpAxis1Beam, out PerpAxis2Beam, out PerpAxis1No, out PerpAxis2No, out PerpAxisNo);
            Beam.MinorAxisGlobaldir = PerpAxis2Beam;
            Beam.MajorAxisGlobaldir = PerpAxis1Beam;

            // Write the beam length
            Beam.Length = (float)Math.Sqrt(Math.Pow(Beam.StartE - Beam.EndE, 2) + Math.Pow(Beam.StartN - Beam.EndN, 2) + Math.Pow(Beam.StartU - Beam.EndU, 2));

            // Write any other additional attributes that need to be filled out
            Beam.SuptNosonBeam = ListofSupts;
            Beam.LevelNo = LLC2;
            Beam.SuptBeamPerpDirFromPipeAxis = LLC1;
            Beam.SuptSteelFunction = "suptbeam";
            Beam.FeatureDesc = "TOPC";
            Beam.ModelName = "suptbeam" + BeamModelName;
        }

        private static int SuptLevelinPerpDir(cSuptPoints Supt, int LLC1)
        {
            int result = 0;
            switch (LLC1)
            {
                case 1:
                    result = Supt.ShrdLvlNoinPerpDir1pos;
                    break;
                case 2:
                    result = Supt.ShrdLvlNoinPerpDir1neg;
                    break;
                case 3:
                    result = Supt.ShrdLvlNoinPerpDir2pos;
                    break;
                case 4:
                    result = Supt.ShrdLvlNoinPerpDir2neg;
                    break;
                default:
                    // Handle invalid LLC1 value here, if necessary
                    break;
            }
            return result;
        }
    }
}
