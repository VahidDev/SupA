using SupA.Lib.CoordinateAndAngleManipulation;
using SupA.Lib.Core;
using SupA.Lib.DataManipulation;
using SupA.Lib.Initialization;

namespace SupA.Lib.FrameCreator
{
    public class mDiscretiseBeamsforFrames
    {
        public static List<cSteelDisc> DiscretiseBeamsforFrames(List<cSteel> CollBeamstoDiscretise)
        {
            // This function uses the public variable IntDiscretisationStepSize and IntDiscretisationStepDecPlaces

            // Definition of all function specific private variables
            var CollExistingSteelDisc = new List<cSteelDisc>();

            // Work through all the beams to discretise in our input collection
            foreach (cSteel BeamtoDiscretise in CollBeamstoDiscretise)
            {
                // For each line start at its start and keep moving along it, and discretising new points until
                // we we are longer within the bounds of the start and end points which define our line.
                int LoopCounter = 0;
                while (IsCurrentCoordinateInsideLine(mDecomposeThreeDVectorintoENUCoords.DecomposeThreeDVectorintoENUCoords(BeamtoDiscretise.Dir, mSubInitializationSupA.pubIntDiscretisationStepSize * LoopCounter), BeamtoDiscretise))
                {
                    // First make a copy of the current beamToDiscretise
                    var BeamtoDiscretiseCE = new cSteel();
                    BeamtoDiscretiseCE.CopyClassInstance(BeamtoDiscretise);

                    // then revise BeamtoDiscretiseCE start points to current node co-ordinates and convert it to ExistingSteelDisc and add it to the discretised points collection
                    CalculateNodeCoordinates(BeamtoDiscretiseCE, mDecomposeThreeDVectorintoENUCoords.DecomposeThreeDVectorintoENUCoords(BeamtoDiscretise.Dir, mSubInitializationSupA.pubIntDiscretisationStepSize * LoopCounter));
                    var ExistingSteelDisc = new cSteelDisc();
                    ExistingSteelDisc.CreateInstancefromBeam(BeamtoDiscretiseCE);
                    // Specify whether we have preceding / succeeding nodes or if we are at the end (or start) of a beam
                    if (mCoordinateCheck.CoordinateCheck("nodepoint", mCreateCoordArray.CreateCoordArray(BeamtoDiscretiseCE, "cSteel", "start", 0, 0, 0, null, true), mCreateCoordArray.CreateCoordArray(BeamtoDiscretise, "cSteel", "start", 0, 0, 0, null, true)))
                    {
                        ExistingSteelDisc.HasPrecedingNode = false;
                        ExistingSteelDisc.HasSucceedingNode = true;
                    }
                    else if (mCoordinateCheck.CoordinateCheck("nodepoint", mCreateCoordArray.CreateCoordArray(BeamtoDiscretiseCE, "cSteel", "start", 0, 0, 0, null, true), mCreateCoordArray.CreateCoordArray(BeamtoDiscretise, "cSteel", "end", 0, 0, 0, null, true)))
                    {
                        ExistingSteelDisc.HasPrecedingNode = true;
                        ExistingSteelDisc.HasSucceedingNode = false;
                    }
                    else
                    {
                        ExistingSteelDisc.HasPrecedingNode = true;
                        ExistingSteelDisc.HasSucceedingNode = true;
                    }
                    CollExistingSteelDisc.Add(ExistingSteelDisc);
                    // next point
                    LoopCounter++;
                }
            }

            return CollExistingSteelDisc;
        }

        private static bool IsCurrentCoordinateInsideLine(float[] CoordinatestoMove, cSteel BeamtoDiscretise)
        {
            if (Math.Abs(BeamtoDiscretise.EndERounded - BeamtoDiscretise.StartERounded) >= Math.Abs(CoordinatestoMove[0]) &&
                Math.Abs(BeamtoDiscretise.EndNRounded - BeamtoDiscretise.StartNRounded) >= Math.Abs(CoordinatestoMove[1]) &&
                Math.Abs(BeamtoDiscretise.EndURounded - BeamtoDiscretise.StartURounded) >= Math.Abs(CoordinatestoMove[2]))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private static void CalculateNodeCoordinates(cSteel BeamtoDiscretiseCE, float[] CoordinateTransform)
        {
            BeamtoDiscretiseCE.StartERounded = mPublicVarDefinitions.RoundDecPlc(BeamtoDiscretiseCE.StartE + CoordinateTransform[0], mSubInitializationSupA.pubIntDiscretisationStepDecPlaces);
            BeamtoDiscretiseCE.StartNRounded = mPublicVarDefinitions.RoundDecPlc(BeamtoDiscretiseCE.StartN + CoordinateTransform[1], mSubInitializationSupA.pubIntDiscretisationStepDecPlaces);
            BeamtoDiscretiseCE.StartURounded = mPublicVarDefinitions.RoundDecPlc(BeamtoDiscretiseCE.StartU + CoordinateTransform[2], mSubInitializationSupA.pubIntDiscretisationStepDecPlaces);

            BeamtoDiscretiseCE.StartE += CoordinateTransform[0];
            BeamtoDiscretiseCE.StartN += CoordinateTransform[1];
            BeamtoDiscretiseCE.StartU += CoordinateTransform[2];

            // Don't forget to discretise these!!!!
        }
    }
}
