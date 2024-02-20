using SupA.Lib.Core;
using SupA.Lib.Initialization;

namespace SupA.Lib.FrameCreator
{
    public class mCorrectSuptFrameCoords
    {
        public void CorrectSuptFrameCoords(List<cSuptPoints> CollAdjacentSuptPoints, List<cSteelDisc> CollLocalExistingSteelDisc, List<cPotlSupt> CollPotlSuptFrameDetails, Array arrNoofLevels)
        {
            cPotlSupt Frame;
            cSuptPoints Supt;
            cGroupNode GroupNode;
            cSteel Beam;
            int LevNo;
            double RoundedLevel = 0;
            double AccurateLevel = 0;
            cSteelDisc SteelPoint;
            cSteelDisc MatchingSteelPoint;

            // First correct all support beam elevations so that these match the lowest bottom of shoe on the
            // relevant support level.
            CorrectSuptFrameCoordsSuptFrameTOS(arrNoofLevels, CollAdjacentSuptPoints, CollPotlSuptFrameDetails);

            // Then work through the frame and wherever our frame ties into existing steel, then
            // correct all coordinates through out the frame to match the actual existing steel coordinates
            // E.g. - an elevation discrepancy (rounding error) noted at the a steel tie-in will get corrected
            // through out the frame.
            CorrectSuptFrameCoordsSteelConns(CollAdjacentSuptPoints, CollPotlSuptFrameDetails, CollLocalExistingSteelDisc);

            // Then fix anything that is T-post to be located underneath the point of action of action of the combined support load
            // (If this flexibilty exists) is an option in the layout.
            CorrectSuptFrameCoordsTPosts(CollAdjacentSuptPoints, CollPotlSuptFrameDetails);
        }

        public void CorrectSuptFrameCoordsTPosts(List<cSuptPoints> CollAdjacentSuptPoints, List<cPotlSupt> CollPotlSuptFrameDetails)
        {
            // This function might require some new definitions in the cpotlsupt class or the groupnode class
            // as we are now choosing a route node that isn't necessarily the first route note in the groupednodes list.
            // Perhaps what we need is a property called "selectedroutenode".
            // Earlier in the algorithm, this can be normally preset to the first routenode but it can be over-ridden when we
            // are making modifications at this point.
            // The FEA algorithm can use this

            // Need to set up the FEA algorithm to refer to the "selectedroutenode" and also to refer to "accurate Coordinates"
        }

        public void CorrectSuptFrameCoordsSteelConns(List<cSuptPoints> CollAdjacentSuptPoints, List<cPotlSupt> CollPotlSuptFrameDetails, List<cSteelDisc> CollLocalExistingSteelDisc)
        {
            cPotlSupt Frame;
            cGroupNode GroupNode;
            cSteel Beam;
            cSteelDisc SteelPoint;
            cSteelDisc MatchingSteelPoint = null;

            // Work through all of the frames in CollPotlSuptFrameDetails
            foreach (cPotlSupt frame in CollPotlSuptFrameDetails)
            {
                // Work through all the groupnodes that define our frame and whenever we find one that is connected to existing
                // steel then go and correct all the rounded coordinates on the associated beam
                foreach (cGroupNode groupNode in frame.GroupNodesinFrame)
                {
                    // this filter may not be required
                    if (groupNode.AssocExistingSteel != "")
                    {
                        // then go and find the steel point with matching rounded coordinates
                        // Note that for the cSteelDisc class, the standard "easting / northing / upping" attributes
                        // is rounded and the "eastingaccurate / etc" is the unrounded coordinate
                        foreach (cSteelDisc steelPoint in CollLocalExistingSteelDisc)
                        {
                            if (steelPoint.Easting == groupNode.SelectedRouteNode.Easting &&
                                steelPoint.Northing == groupNode.SelectedRouteNode.Northing &&
                                steelPoint.Upping == groupNode.SelectedRouteNode.Upping)
                            {
                                MatchingSteelPoint = steelPoint;
                                break;
                            }
                        }

                        // then go and correct these rounded coordinates to accurate coordinates from the steel point
                        // wherever you find these on a frame
                        foreach (cSteel beam in frame.BeamsinFrame)
                        {
                            // For Starts
                            if (beam.StartERounded == MatchingSteelPoint.Easting)
                            {
                                beam.StartE = MatchingSteelPoint.Eastingaccurate;
                            }
                            if (beam.StartNRounded == MatchingSteelPoint.Northing)
                            {
                                beam.StartN = MatchingSteelPoint.Northingaccurate;
                            }
                            if (beam.StartURounded == MatchingSteelPoint.Upping)
                            {
                                if ((beam.Dir == "U" || beam.Dir == "D") && MatchingSteelPoint.DirnofBeam != "D" && MatchingSteelPoint.DirnofBeam != "U")
                                {
                                    beam.StartU = MatchingSteelPoint.Uppingaccurate;
                                }
                            }
                            // And For Ends
                            if (beam.EndERounded == MatchingSteelPoint.Easting)
                            {
                                beam.EndE = MatchingSteelPoint.Eastingaccurate;
                            }
                            if (beam.EndNRounded == MatchingSteelPoint.Northing)
                            {
                                beam.EndN = MatchingSteelPoint.Northingaccurate;
                            }
                            if (beam.EndURounded == MatchingSteelPoint.Upping)
                            {
                                if ((beam.Dir == "U" || beam.Dir == "D") && MatchingSteelPoint.DirnofBeam != "D" && MatchingSteelPoint.DirnofBeam != "U")
                                {
                                    beam.EndU = MatchingSteelPoint.Uppingaccurate;
                                }
                            }
                        }
                    }
                }
            }
        }

        public void CorrectSuptFrameCoordsSuptFrameTOS(Array arrNoofLevels, List<cSuptPoints> CollAdjacentSuptPoints, List<cPotlSupt> CollPotlSuptFrameDetails)
        {
            cPotlSupt Frame;
            cSuptPoints Supt;
            cSteel Beam;
            int LevNo;
            double RoundedLevel;
            double AccurateLevel;

            // Am I looking at the right value in the arrnooflevels array?
            for (LevNo = 1; LevNo <= (int)arrNoofLevels.GetValue(3); LevNo++)
            {
                RoundedLevel = 0;
                AccurateLevel = 0;

                // Work through every supt in CollAdjacentSuptPoints and find the lowest
                // supt level on a common frame to use as my beam elevations (anything higher will be shimmed)
                foreach (cSuptPoints supt in CollAdjacentSuptPoints)
                {
                    // this only works for horizontal pipes for now - I use the perpdir2neg as the TOS elevation for my supt beam
                    // So if my current support is on the shared level then get the
                    if (supt.ShrdLvlNoinPerpDir2neg == LevNo)
                    {
                        if (supt.TOSPerpDir2neg >= RoundedLevel)
                            RoundedLevel = mPublicVarDefinitions.RoundDecPlc(supt.TOSPerpDir2neg, mSubInitializationSupA.pubIntDiscretisationStepDecPlaces);
                        if (supt.TOSPerpDir2neg >= AccurateLevel)
                            AccurateLevel = supt.TOSPerpDir2neg;
                    }
                }

                foreach (cPotlSupt frame in CollPotlSuptFrameDetails)
                {
                    foreach (cSteel beam in frame.BeamsinFrame)
                    {
                        if (beam.StartURounded == RoundedLevel)
                            beam.StartU = (float)AccurateLevel;

                        if (beam.EndURounded == RoundedLevel)
                            beam.EndU = (float)AccurateLevel;

                        if (beam.Dir == "U" || beam.Dir == "D")
                            beam.Length = Math.Abs(beam.EndU - beam.StartU);
                    }
                }
            }
        }
    }
}
