using SupA.Lib.CoordinateAndAngleManipulation;
using SupA.Lib.Core;
using SupA.Lib.DataManipulation;
using SupA.Lib.Initialization;
using SupA.Lib.Models;

namespace SupA.Lib.BuildSupportFrameWorkOptions
{
    public class mCreateExtendedBeams
    {
        public static List<cSteel> CreateExtendedBeams(List<cSteel> CollSuptBeams, List<cSteelDisc> CollLocalExistingSteelDisc, List<cClashData> CollLocalClashData, int SuptGroupNo)
        {
            // Definition of all function specific private variables
            // Loop Variables
            //cSteelDisc Node;
            //cSteel SuptBeam;
            int LL2C;
            int LL3C;
            // Collections
            var CollExtendedBeams = new List<cSteel>();
            // General internal variables and flags
            string ConnectingSteelStart = "";
            string ConnectingSteelEnd = "";
            float ExtensionLen = 0f;
            cSteel ExtendedBeam;
            float[] CoordArrayBeamEnd1 = Array.Empty<float>();
            float[] CoordArrayBeamEnd2 = Array.Empty<float>();
            bool TieinFlag;
            bool ClashFlag;
            int BeamModelName = 1;

            // For every support beam option which has been identified
            foreach (cSteel SuptBeam in CollSuptBeams)
            {
                // For both the start and end of the beams ' (LL2C = 1 is always start and LL2C = 2 is always end)
                for (LL2C = 1; LL2C <= 2; LL2C++)
                {
                    // For the number of steps required to reach our maximum support beam extension when stepping at our discretisation step size
                    for (LL3C = 0; LL3C <= mSubInitializationSupA.pubIntMaxSuptBeamExtension / mSubInitializationSupA.pubIntDiscretisationStepSize; LL3C++)
                    {
                        // but first clear the variables which need to be reset at every loop
                        ConnectingSteelStart = ""; ConnectingSteelEnd = ""; ExtensionLen = 0; TieinFlag = false;

                        // Get current end of beam and start of beam co-ordinates to use in the clash check and tie-in checks (and to
                        // potentially return as our beam definition)

                        // the definition of starts and ends below are flipped round to continue the convention that start is always
                        // on smaller co-ordinates than end.
                        if (LL2C == 1)
                        {
                            CoordArrayBeamEnd1 = mCreateCoordArray.CreateCoordArray(SuptBeam, "cSteel", "start", 0, 0, 0, mDecomposeThreeDVectorintoENUCoords.DecomposeThreeDVectorintoENUCoords(SuptBeam.Dir, -(LL3C * mSubInitializationSupA.pubIntDiscretisationStepSize)));
                            CoordArrayBeamEnd2 = mCreateCoordArray.CreateCoordArray(SuptBeam, "cSteel", "start");
                            ConnectingSteelEnd = SuptBeam.ModelName;
                        }
                        else if (LL2C == 2)
                        {
                            CoordArrayBeamEnd1 = mCreateCoordArray.CreateCoordArray(SuptBeam, "cSteel", "end", 0, 0, 0, mDecomposeThreeDVectorintoENUCoords.DecomposeThreeDVectorintoENUCoords(SuptBeam.Dir, (LL3C * mSubInitializationSupA.pubIntDiscretisationStepSize)));
                            CoordArrayBeamEnd2 = mCreateCoordArray.CreateCoordArray(SuptBeam, "cSteel", "end");
                            ConnectingSteelStart = SuptBeam.ModelName;
                        }

                        // check if the extension at the current discretisation step is a tie-in to existing steel
                        foreach (cSteelDisc Node in CollLocalExistingSteelDisc)
                        {
                            if (mCoordinateCheck.CoordinateCheck("nodepoint", mPublicVarDefinitions.RoundCoordArr(CoordArrayBeamEnd1, mSubInitializationSupA.pubIntDiscretisationStepDecPlaces), mPublicVarDefinitions.RoundCoordArr(mCreateCoordArray.CreateCoordArray(Node, "cSteelDisc"), mSubInitializationSupA.pubIntDiscretisationStepDecPlaces)))
                            {
                                if (LL2C == 1) ConnectingSteelStart = Node.Name;
                                if (LL2C == 2) ConnectingSteelEnd = Node.Name;
                                ExtensionLen = LL3C * mSubInitializationSupA.pubIntDiscretisationStepSize;
                                TieinFlag = true;
                                break;
                            }
                        }

                        // check if the extension at the current discretisation step results in any clashes
                        // (but first reset the related variables)
                        ClashFlag = mRunClashandClearanceCheck.RunClashandClearanceCheck(CollLocalClashData, CoordArrayBeamEnd1, CoordArrayBeamEnd2, new string[] { "S", "X", "C" });

                        // now we exit the loop if either our clashflag or tie-in flag is true
                        if (ClashFlag || TieinFlag)
                        {
                            if (ClashFlag && LL3C != 0)
                            {
                                ExtensionLen = (LL3C - 1) * mSubInitializationSupA.pubIntDiscretisationStepSize;
                            }
                            break;
                        }
                    }

                    // now write our data about the extended beam to a new extended beam instance, and add this to our collection
                    if (LL2C == 1) // Start of suptbeam extensions
                    {
                        ExtendedBeam = new cSteel();
                        ExtendedBeam = WriteExtendedBeamData(CoordArrayBeamEnd1, CoordArrayBeamEnd2, SuptBeam, ExtensionLen, ConnectingSteelStart, ConnectingSteelEnd, "start", SuptBeam.SuptBeamPerpDirFromPipeAxis, SuptBeam.LevelNo, BeamModelName);
                        BeamModelName++;
                        CollExtendedBeams.Add(ExtendedBeam);
                    }
                    else if (LL2C == 2) // End of suptbeam extensions
                    {
                        ExtendedBeam = new cSteel();
                        ExtendedBeam = WriteExtendedBeamData(CoordArrayBeamEnd1, CoordArrayBeamEnd2, SuptBeam, ExtensionLen, ConnectingSteelStart, ConnectingSteelEnd, "end", SuptBeam.SuptBeamPerpDirFromPipeAxis, SuptBeam.LevelNo, BeamModelName);
                        BeamModelName++;
                        CollExtendedBeams.Add(ExtendedBeam);
                    }
                }
            }

            if (mSubInitializationSupA.pubBOOLTraceOn == true)
            {
                mExportColltoCSVFile<cSteel>.ExportColltoCSVFile(CollExtendedBeams, "CollExtendedBeams-F" + (SuptGroupNo + mSubInitializationSupA.SuptGroupNoMod).ToString(), "csv");
            }
            return CollExtendedBeams;
        }

        public static cSteel WriteExtendedBeamData(float[] CoordArrayBeamEnd1, float[] CoordArrayBeamEnd2, cSteel SuptBeam, float ExtensionLen, string ConnectingSteelStart, string ConnectingSteelEnd, string StartorEnd, int PerpDir, int LevelNo, int BeamModelName)
        {
            cSteel ExtendedBeam = new cSteel();

            // Pull out perpAxis information to write to our beam
            string PerpAxis1, PerpAxis2;
            int PerpAxis1No, PerpAxis2No, ParlAxisNo;
            mDefinePerpsandParls.DefinePerpsandParls(ref SuptBeam.Dir, out PerpAxis1, out PerpAxis2, out PerpAxis1No, out PerpAxis2No, out ParlAxisNo);

            if (StartorEnd == "start")
            {
                ExtendedBeam.StartE = CoordArrayBeamEnd1[0];
                ExtendedBeam.StartN = CoordArrayBeamEnd1[1];
                ExtendedBeam.StartU = CoordArrayBeamEnd1[2];
                ExtendedBeam.EndE = CoordArrayBeamEnd2[0];
                ExtendedBeam.EndN = CoordArrayBeamEnd2[1];
                ExtendedBeam.EndU = CoordArrayBeamEnd2[2];
            }
            else if (StartorEnd == "end")
            {
                ExtendedBeam.StartE = CoordArrayBeamEnd2[0];
                ExtendedBeam.StartN = CoordArrayBeamEnd2[1];
                ExtendedBeam.StartU = CoordArrayBeamEnd2[2];
                ExtendedBeam.EndE = CoordArrayBeamEnd1[0];
                ExtendedBeam.EndN = CoordArrayBeamEnd1[1];
                ExtendedBeam.EndU = CoordArrayBeamEnd1[2];
            }
            else
            {
                throw new Exception("Error in writeextendedbeamdata");
            }

            ExtendedBeam.StartERounded = mPublicVarDefinitions.RoundDecPlc(ExtendedBeam.StartE, mSubInitializationSupA.pubIntDiscretisationStepDecPlaces);
            ExtendedBeam.StartNRounded = mPublicVarDefinitions.RoundDecPlc(ExtendedBeam.StartN, mSubInitializationSupA.pubIntDiscretisationStepDecPlaces);
            ExtendedBeam.StartURounded = mPublicVarDefinitions.RoundDecPlc(ExtendedBeam.StartU, mSubInitializationSupA.pubIntDiscretisationStepDecPlaces);
            ExtendedBeam.EndERounded = mPublicVarDefinitions.RoundDecPlc(ExtendedBeam.EndE, mSubInitializationSupA.pubIntDiscretisationStepDecPlaces);
            ExtendedBeam.EndNRounded = mPublicVarDefinitions.RoundDecPlc(ExtendedBeam.EndN, mSubInitializationSupA.pubIntDiscretisationStepDecPlaces);
            ExtendedBeam.EndURounded = mPublicVarDefinitions.RoundDecPlc(ExtendedBeam.EndU, mSubInitializationSupA.pubIntDiscretisationStepDecPlaces);
            ExtendedBeam.Length = (float)Math.Sqrt(Math.Pow(ExtendedBeam.StartE - ExtendedBeam.EndE, 2) + Math.Pow(ExtendedBeam.StartN - ExtendedBeam.EndN, 2) + Math.Pow(ExtendedBeam.StartU - ExtendedBeam.EndU, 2));

            ExtendedBeam.Dir = SuptBeam.Dir;
            ExtendedBeam.MinorAxisGlobaldir = PerpAxis2;
            ExtendedBeam.MajorAxisGlobaldir = PerpAxis1;

            ExtendedBeam.ExistingSteelConnNameStart = ConnectingSteelStart;
            ExtendedBeam.ExistingSteelConnNameEnd = ConnectingSteelEnd;

            ExtendedBeam.FeatureDesc = "TOPC";
            ExtendedBeam.Jusline = "CTOP";
            ExtendedBeam.LevelNo = LevelNo; // this is wrong
            ExtendedBeam.SuptBeamPerpDirFromPipeAxis = PerpDir; // this is wrong

            ExtendedBeam.SuptSteelFunction = "extendedbeam";
            ExtendedBeam.ModelName = "extendedbeam" + BeamModelName;

            return ExtendedBeam;
        }
    }
}
