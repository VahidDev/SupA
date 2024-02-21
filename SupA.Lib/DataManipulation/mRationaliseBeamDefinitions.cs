using SupA.Lib.Core;

namespace SupA.Lib.DataManipulation
{
    public class mRationaliseBeamDefinitions
    {
        public void RationaliseBeamDefinitions(List<cPotlSupt> CollPotlSuptFrameDetails)
        {
            foreach (var Frame in CollPotlSuptFrameDetails)
            {
                Frame.BeamsinFrameratlized = new List<cSteel>();
                foreach (var Beam in Frame.BeamsinFrame)
                {
                    var BeamtoAdd = new cSteel();
                    BeamtoAdd.CopyClassInstance(Beam);
                    Frame.BeamsinFrameratlized.Add(BeamtoAdd);
                }

                int ExternalLoopLL1 = 1;
                while (ExternalLoopLL1 <= 10)
                {
                    for (int ratBeamLL1C = 0; ratBeamLL1C < Frame.BeamsinFrameratlized.Count; ratBeamLL1C++)
                    {
                        var ratBeamLL1 = Frame.BeamsinFrameratlized[ratBeamLL1C];
                        for (int ratBeamLL2C = 0; ratBeamLL2C < Frame.BeamsinFrameratlized.Count; ratBeamLL2C++)
                        {
                            if (ratBeamLL2C != ratBeamLL1C)
                            {
                                var ratBeamLL2 = Frame.BeamsinFrameratlized[ratBeamLL2C];
                                bool BeamcontInratlzedColl = false; // Assume CoordinateCheck and DirectionCheck logic
                                                                    // CoordinateCheck and DirectionCheck method calls replaced with inline logic
                                if ((Math.Abs(ratBeamLL1.StartE - ratBeamLL2.EndE) <= 0.001 && Math.Abs(ratBeamLL1.StartN - ratBeamLL2.EndN) <= 0.001 && Math.Abs(ratBeamLL1.StartU - ratBeamLL2.EndU) <= 0.001) || (Math.Abs(ratBeamLL1.EndE - ratBeamLL2.StartE) <= 0.001 && Math.Abs(ratBeamLL1.EndN - ratBeamLL2.StartN) <= 0.001 && Math.Abs(ratBeamLL1.EndU - ratBeamLL2.StartU) <= 0.001))
                                {
                                    BeamcontInratlzedColl = true;
                                }

                                if (BeamcontInratlzedColl)
                                {
                                    UpdateRatBeamDefinition(ratBeamLL1, ratBeamLL2);
                                    Frame.BeamsinFrameratlized.RemoveAt(ratBeamLL2C);
                                    ratBeamLL2C--;
                                }
                            }
                        }
                    }
                    ExternalLoopLL1++;
                }
            }
        }

        public static void UpdateRatBeamDefinition(cSteel ratBeamLL1, cSteel ratBeamLL2)
        {
            // Assuming that the comparison of magnitudes would determine how to adjust the start and end of ratBeamLL1
            // to cover the combined length or extents of both beams when they are aligned or continuous.

            // Determine the combined or furthest extents of the beams
            double startE = Math.Min(ratBeamLL1.StartE, ratBeamLL2.StartE);
            double startN = Math.Min(ratBeamLL1.StartN, ratBeamLL2.StartN);
            double startU = Math.Min(ratBeamLL1.StartU, ratBeamLL2.StartU);
            double endE = Math.Max(ratBeamLL1.EndE, ratBeamLL2.EndE);
            double endN = Math.Max(ratBeamLL1.EndN, ratBeamLL2.EndN);
            double endU = Math.Max(ratBeamLL1.EndU, ratBeamLL2.EndU);

            // Check if the beams are considered continuous or aligned in any way that would necessitate adjustment
            if (DirectionCheck(ratBeamLL1.Dir, ratBeamLL2.Dir) == "parallel")
            {
                // Update ratBeamLL1 to the combined or furthest extents
                ratBeamLL1.StartE = startE;
                ratBeamLL1.StartN = startN;
                ratBeamLL1.StartU = startU;
                ratBeamLL1.EndE = endE;
                ratBeamLL1.EndN = endN;
                ratBeamLL1.EndU = endU;

                // Optionally, update the connection names if relevant to the logic
                // This part is assumed and would depend on specific requirements
            }

            // Update Length based on the new start and end points; this simplistic approach may need refinement
            ratBeamLL1.Length = Math.Sqrt(Math.Pow(ratBeamLL1.EndE - ratBeamLL1.StartE, 2) +
                                          Math.Pow(ratBeamLL1.EndN - ratBeamLL1.StartN, 2) +
                                          Math.Pow(ratBeamLL1.EndU - ratBeamLL1.StartU, 2));
        }

        public static void UpdateRatBeamDefinitionEnds(cSteel tempBeam, cSteel ratBeam, string endToCopy, string endToCopyTo)
        {
            if (endToCopy == "start")
            {
                if (endToCopyTo == "start")
                {
                    tempBeam.StartE = ratBeam.StartE;
                    tempBeam.StartN = ratBeam.StartN;
                    tempBeam.StartU = ratBeam.StartU;
                    // Assuming ExistingSteelConnNameStart is a property that needs to be copied
                    tempBeam.ExistingSteelConnNameStart = ratBeam.ExistingSteelConnNameStart;
                }
                else if (endToCopyTo == "end")
                {
                    tempBeam.EndE = ratBeam.StartE;
                    tempBeam.EndN = ratBeam.StartN;
                    tempBeam.EndU = ratBeam.StartU;
                    tempBeam.ExistingSteelConnNameEnd = ratBeam.ExistingSteelConnNameStart;
                }
            }
            else if (endToCopy == "end")
            {
                if (endToCopyTo == "start")
                {
                    tempBeam.StartE = ratBeam.EndE;
                    tempBeam.StartN = ratBeam.EndN;
                    tempBeam.StartU = ratBeam.EndU;
                    tempBeam.ExistingSteelConnNameStart = ratBeam.ExistingSteelConnNameEnd;
                }
                else if (endToCopyTo == "end")
                {
                    tempBeam.EndE = ratBeam.EndE;
                    tempBeam.EndN = ratBeam.EndN;
                    tempBeam.EndU = ratBeam.EndU;
                    tempBeam.ExistingSteelConnNameEnd = ratBeam.ExistingSteelConnNameEnd;
                }
            }
            else if (endToCopy == "both" && endToCopyTo == "both")
            {
                tempBeam.StartE = ratBeam.StartE;
                tempBeam.StartN = ratBeam.StartN;
                tempBeam.StartU = ratBeam.StartU;
                tempBeam.EndE = ratBeam.EndE;
                tempBeam.EndN = ratBeam.EndN;
                tempBeam.EndU = ratBeam.EndU;
                tempBeam.ExistingSteelConnNameStart = ratBeam.ExistingSteelConnNameStart;
                tempBeam.ExistingSteelConnNameEnd = ratBeam.ExistingSteelConnNameEnd;
            }
        }

        public static string DirectionCheck(string DirectionOne, string DirectionTwo)
        {
            if ((DirectionOne == "N" || DirectionOne == "S") && (DirectionTwo == "N" || DirectionTwo == "S"))
            {
                return "parallel";
            }
            else if ((DirectionOne == "E" || DirectionOne == "W") && (DirectionTwo == "E" || DirectionTwo == "W"))
            {
                return "parallel";
            }
            else if ((DirectionOne == "U" || DirectionOne == "D") && (DirectionTwo == "U" || DirectionTwo == "D"))
            {
                return "parallel";
            }
            else
            {
                return "not parallel";
            }
        }
    }
}
