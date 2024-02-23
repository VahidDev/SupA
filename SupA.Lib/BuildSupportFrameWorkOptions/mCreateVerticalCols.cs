using SupA.Lib.Core;
using SupA.Lib.DataManipulation;
using SupA.Lib.Initialization;
using SupA.Lib.Models;

namespace SupA.Lib.BuildSupportFrameWorkOptions
{
    public class mCreateVerticalCols
    {
        public static List<cSteel> CreateVerticalCols(List<cSteelDisc> CollAllDiscBeams, List<cClashData> CollLocalClashData, int SuptGroupNo)
        {
            List<cSteel> CollVerticalCols = new List<cSteel>();
            int PerpAxis1No = 1;
            int PerpAxis2No = 2;
            int ParlAxisNo = 3;

            int BeamModelName = 1;

            foreach (cSteelDisc SteelNodeLL1 in CollAllDiscBeams)
            {
                foreach (cSteelDisc SteelNodeLL2 in CollAllDiscBeams)
                {
                    float[] CoordArrayColEnd1 = mCreateCoordArray.CreateCoordArray(SteelNodeLL1, "cSteelDisc");
                    float[] CoordArrayColEnd2 = mCreateCoordArray.CreateCoordArray(SteelNodeLL2, "cSteelDisc");

                    if (mPublicVarDefinitions.RoundDecPlc(CoordArrayColEnd1[PerpAxis1No], mSubInitializationSupA.pubIntDiscretisationStepDecPlaces) == mPublicVarDefinitions.RoundDecPlc(CoordArrayColEnd2[PerpAxis1No], mSubInitializationSupA.pubIntDiscretisationStepDecPlaces) &&
                    mPublicVarDefinitions.RoundDecPlc(CoordArrayColEnd1[PerpAxis2No], mSubInitializationSupA.pubIntDiscretisationStepDecPlaces) == mPublicVarDefinitions.RoundDecPlc(CoordArrayColEnd2[PerpAxis2No], mSubInitializationSupA.pubIntDiscretisationStepDecPlaces) &&
                    mPublicVarDefinitions.RoundDecPlc(CoordArrayColEnd1[ParlAxisNo], mSubInitializationSupA.pubIntDiscretisationStepDecPlaces) != mPublicVarDefinitions.RoundDecPlc(CoordArrayColEnd2[ParlAxisNo], mSubInitializationSupA.pubIntDiscretisationStepDecPlaces))
                    {
                        string ConnectingSteelEnd1 = SteelNodeLL1.Name;
                        string ConnectingSteelEnd2 = SteelNodeLL2.Name;
                        string ConnectingSteelEnd1Dir = SteelNodeLL1.DirnofBeam;
                        string ConnectingSteelEnd2Dir = SteelNodeLL2.DirnofBeam;

                        bool ClashFlag = false;
                        ClashFlag = mRunClashandClearanceCheck.RunClashandClearanceCheck(CollLocalClashData, CoordArrayColEnd1, CoordArrayColEnd2, new string[] { "S", "X", "C" });

                        if (!ClashFlag)
                        {
                            bool VerticalColValidFlag = true;
                            foreach (cSteel VerticalCol in CollVerticalCols)
                            {
                                if (mCoordinateCheck.CoordinateCheck("nodepoint", mCreateCoordArray.CreateCoordArray(VerticalCol, "cSteel", "start"), CoordArrayColEnd1) && mCoordinateCheck.CoordinateCheck("nodepoint", mCreateCoordArray.CreateCoordArray(VerticalCol, "cSteel", "end"), CoordArrayColEnd2) ||
                                    mCoordinateCheck.CoordinateCheck("nodepoint", mCreateCoordArray.CreateCoordArray(VerticalCol, "cSteel", "start"), CoordArrayColEnd2) && mCoordinateCheck.CoordinateCheck("nodepoint", mCreateCoordArray.CreateCoordArray(VerticalCol, "cSteel", "end"), CoordArrayColEnd1))
                                {
                                    VerticalColValidFlag = false;

                                    // if there is an existing vertical column and its start and end is tying into an existing steel but this is not where we
                                    // are getting the end information from then update this


                                    // Set the end at smaller co-ordinates to be our start, as per standard convention
                                    string StartDef = "";
                                    if ((CoordArrayColEnd1[0] + CoordArrayColEnd1[1] + CoordArrayColEnd1[2]) < (CoordArrayColEnd2[0] + CoordArrayColEnd2[1] + CoordArrayColEnd2[2]))
                                    {
                                        StartDef = "End1";
                                    }

                                    if (SteelNodeLL1.SteelFunction == "existingsteel")
                                    {
                                        VerticalCol.StartConnStlDir = SteelNodeLL1.DirnofBeam;
                                        VerticalCol.ExistingSteelConnNameStart = SteelNodeLL1.Name;
                                    }
                                    if (SteelNodeLL2.SteelFunction == "existingsteel")
                                    {
                                        VerticalCol.StartConnStlDir = SteelNodeLL2.DirnofBeam;
                                        VerticalCol.ExistingSteelConnNameStart = SteelNodeLL2.Name;
                                    }
                                    break;
                                }

                                if (mCoordinateCheck.CoordinateCheck("tocheckwithinlineagainst", CoordArrayColEnd1, mCreateCoordArray.CreateCoordArray(VerticalCol, "cSteel", "start"), mCreateCoordArray.CreateCoordArray(VerticalCol, "cSteel", "end"), CoordArrayColEnd2))
                                {
                                    CollVerticalCols.Remove(VerticalCol);
                                    break;
                                }

                                if (mCoordinateCheck.CoordinateCheck("tocheckwithinlineagainst", mCreateCoordArray.CreateCoordArray(VerticalCol, "cSteel", "start"), CoordArrayColEnd1, CoordArrayColEnd2, mCreateCoordArray.CreateCoordArray(VerticalCol, "cSteel", "end")))
                                {
                                    VerticalColValidFlag = false;
                                    break;
                                }
                            }

                            if (ConnectingSteelEnd1 == ConnectingSteelEnd2 && !string.IsNullOrEmpty(ConnectingSteelEnd1) && !string.IsNullOrEmpty(ConnectingSteelEnd2))
                            {
                                VerticalColValidFlag = false;
                            }

                            if (VerticalColValidFlag)
                            {
                                cSteel VerticalCol = WriteVerticalColData(CoordArrayColEnd1, CoordArrayColEnd2, ConnectingSteelEnd1, ConnectingSteelEnd2, "E", "N", "U", BeamModelName, ConnectingSteelEnd1Dir, ConnectingSteelEnd2Dir);
                                BeamModelName++;
                                CollVerticalCols.Add(VerticalCol);
                            }
                        }
                    }
                }
            }

            ReduceCollVerticalSteeltoShortestComponents(CollVerticalCols);
            if (mSubInitializationSupA.pubBOOLTraceOn)
            {
                mExportColltoCSVFile<cSteel>.ExportColltoCSVFile(CollVerticalCols, "CollVerticalCols-F" + (SuptGroupNo + mSubInitializationSupA.SuptGroupNoMod).ToString(), "csv");
            }
            return CollVerticalCols;
        }

        private static cSteel WriteVerticalColData(float[] CoordArrayColEnd1, float[] CoordArrayColEnd2, string ConnectingSteelEnd1, string ConnectingSteelEnd2,
                                    string PerpAxis1, string PerpAxis2, string ParlAxis, int BeamModelName, string ConnectingSteelEnd1Dir, string ConnectingSteelEnd2Dir)
        {
            string StartDef = "";
            cSteel VerticalCol = new cSteel();

            // Set the end at smaller co-ordinates to be our start, as per standard convention
            if ((CoordArrayColEnd1[0] + CoordArrayColEnd1[1] + CoordArrayColEnd1[2]) < (CoordArrayColEnd2[0] + CoordArrayColEnd2[1] + CoordArrayColEnd2[2]))
            {
                StartDef = "End1";
            }

            // Then add all attributes to the trim steel
            if (StartDef == "End1")
            {
                VerticalCol.StartE = CoordArrayColEnd1[0];
                VerticalCol.StartN = CoordArrayColEnd1[1];
                VerticalCol.StartU = CoordArrayColEnd1[2];
                VerticalCol.EndE = CoordArrayColEnd2[0];
                VerticalCol.EndN = CoordArrayColEnd2[1];
                VerticalCol.EndU = CoordArrayColEnd2[2];
                VerticalCol.ExistingSteelConnNameStart = ConnectingSteelEnd1;
                VerticalCol.ExistingSteelConnNameEnd = ConnectingSteelEnd2;
                VerticalCol.StartConnStlDir = ConnectingSteelEnd1Dir;
                VerticalCol.EndConnStlDir = ConnectingSteelEnd2Dir;
            }
            else
            {
                VerticalCol.StartE = CoordArrayColEnd2[0];
                VerticalCol.StartN = CoordArrayColEnd2[1];
                VerticalCol.StartU = CoordArrayColEnd2[2];
                VerticalCol.EndE = CoordArrayColEnd1[0];
                VerticalCol.EndN = CoordArrayColEnd1[1];
                VerticalCol.EndU = CoordArrayColEnd1[2];
                VerticalCol.ExistingSteelConnNameStart = ConnectingSteelEnd2;
                VerticalCol.ExistingSteelConnNameEnd = ConnectingSteelEnd1;
                VerticalCol.StartConnStlDir = ConnectingSteelEnd2Dir;
                VerticalCol.EndConnStlDir = ConnectingSteelEnd1Dir;
            }

            VerticalCol.StartERounded = mPublicVarDefinitions.RoundDecPlc(VerticalCol.StartE, mSubInitializationSupA.pubIntDiscretisationStepDecPlaces);
            VerticalCol.StartNRounded = mPublicVarDefinitions.RoundDecPlc(VerticalCol.StartN, mSubInitializationSupA.pubIntDiscretisationStepDecPlaces);
            VerticalCol.StartURounded = mPublicVarDefinitions.RoundDecPlc(VerticalCol.StartU, mSubInitializationSupA.pubIntDiscretisationStepDecPlaces);
            VerticalCol.EndERounded = mPublicVarDefinitions.RoundDecPlc(VerticalCol.EndE, mSubInitializationSupA.pubIntDiscretisationStepDecPlaces);
            VerticalCol.EndNRounded = mPublicVarDefinitions.RoundDecPlc(VerticalCol.EndN, mSubInitializationSupA.pubIntDiscretisationStepDecPlaces);
            VerticalCol.EndURounded = mPublicVarDefinitions.RoundDecPlc(VerticalCol.EndU, mSubInitializationSupA.pubIntDiscretisationStepDecPlaces);
            VerticalCol.Length = (float)Math.Sqrt(Math.Pow(VerticalCol.StartE - VerticalCol.EndE, 2) + Math.Pow(VerticalCol.StartN - VerticalCol.EndN, 2) + Math.Pow(VerticalCol.StartU - VerticalCol.EndU, 2));

            VerticalCol.Dir = ParlAxis;
            VerticalCol.MinorAxisGlobaldir = PerpAxis2;
            VerticalCol.MajorAxisGlobaldir = PerpAxis1;

            VerticalCol.FeatureDesc = "TOPC";
            VerticalCol.Jusline = "CTOP";
            VerticalCol.SuptSteelFunction = "verticalcol";
            VerticalCol.ModelName = "verticalcol" + BeamModelName;

            return VerticalCol;
        }

        public static void ReduceCollVerticalSteeltoShortestComponents(List<cSteel> CollSteel)
        {
            int LL1, LL2;
            LL1 = 0;
            while (LL1 < CollSteel.Count)
            {
                cSteel Steel = CollSteel[LL1];
                LL2 = 0;
                while (LL2 < CollSteel.Count)
                {
                    if (LL2 != LL1)
                    {
                        cSteel SteelLL2 = CollSteel[LL2];
                        if (mCoordinateCheck.CoordinateCheck("withinbounds", mCreateCoordArray.CreateCoordArray(SteelLL2, "cSteel", "start", 0, 0, 0, null, true), mCreateCoordArray.CreateCoordArray(Steel, "cSteel", "start", 0, 0, 0, null, true), mCreateCoordArray.CreateCoordArray(Steel, "cSteel", "end", 0, 0, 0, null, true)) &&
                            mCoordinateCheck.CoordinateCheck("withinbounds", mCreateCoordArray.CreateCoordArray(SteelLL2, "cSteel", "end", 0, 0, 0, null, true), mCreateCoordArray.CreateCoordArray(Steel, "cSteel", "start", 0, 0, 0, null, true), mCreateCoordArray.CreateCoordArray(Steel, "cSteel", "end", 0, 0, 0, null, true)))
                        {
                            CollSteel.RemoveAt(LL1);
                            LL1--;
                            break;
                        }
                    }
                    LL2++;
                }
                LL1++;
            }

            for (int i = 0; i < CollSteel.Count; i++)
            {
                cSteel Steel = CollSteel[i];
                Steel.ModelName = "verticalcol" + (i + 1);
            }
        }
    }
}
