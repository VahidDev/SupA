using SupA.Lib.CoordinateAndAngleManipulation;
using SupA.Lib.Core;
using SupA.Lib.Initialization;

namespace SupA.Lib.DataManipulation
{
    public class mMasterSuptPointCreatorSub
    {
        public static List<object> MasterSuptPointCreatorSub(List<cTubeDef> CollPipeforSupporting, List<cSteelDisc> CollExistingSteelDisc, List<cSteel> CollExistingSteel, List<cTubeDefDisc> CollSelectedSuptLocns, List<object> CollAllSuptPointScores, List<object> CollAllSelectedSuptLocns)
        {
            bool SuptsReqdFlag = false;
            List<cTubeDefDisc> CollSuptPointScores = new List<cTubeDefDisc>();
            // Assuming cTubeDefDisc to be a class defined elsewhere
            cTubeDefDisc tmpTubeDefDisc;

            // Activity Log Tracking
            mWriteActivityLog.WriteActivityLog("Define Approx Supt Count Per Tube - All Pipes", DateTime.Now);

            // Now we work through the cable tray and identify how many supports are required for each part of the cable tray
            DefineApproxSuptCountPerTube(CollPipeforSupporting, ref SuptsReqdFlag);

            // Activity Log Tracking
            mWriteActivityLog.WriteActivityLog("Score All Supt Points Along Tubes  - All Pipes", DateTime.Now);

            // We work through and score every support point - the higher the score the better the support point
            ScoreSuptPoints(CollPipeforSupporting, CollExistingSteelDisc, CollSuptPointScores);
            CollAllSuptPointScores.AddRange(CollSuptPointScores); // Assuming MergeCollection's equivalent

            // Activity Log Tracking
            mWriteActivityLog.WriteActivityLog("Select Best Supt Points Along Tubes  - All Pipes", DateTime.Now);

            if (mSubInitializationSupA.pubBOOLTraceOn)
            {
                mExportColltoCSVFile<cTubeDefDisc>.ExportColltoCSVFile(CollSuptPointScores, "CollSuptPointScores7407", "csv");
            }

            // And finally choose the best support point by evaluating the output of arraysupportsreqd against the suitability of every supt point along the pipe suptpointscores
            CollSelectedSuptLocns = SelectSupportPoints(CollPipeforSupporting, CollSuptPointScores);
            CollAllSelectedSuptLocns.AddRange(CollSelectedSuptLocns); // Assuming MergeCollection's equivalent

            // And there is a little bit of post processing now - delete all the "empty supt lines" which were put in there as placeholders
            int i = 0;
            while (i < CollAllSelectedSuptLocns.Count)
            {
                tmpTubeDefDisc = (cTubeDefDisc)CollAllSelectedSuptLocns[i];
                if (tmpTubeDefDisc.East == 0)
                {
                    CollAllSelectedSuptLocns.RemoveAt(i);
                    i--;
                }
                i++;
            }

            return CollAllSelectedSuptLocns; // Assuming this is the intended return, adjusted for C# syntax
        }

        private static void DefineApproxSuptCountPerTube(List<cTubeDef> CollPipeforSupporting, ref bool SuptsReqdFlag)
        {
            foreach (var Tube in CollPipeforSupporting)
            {
                TTblSuptSpanRules? SuptingRuleforTube = null;

                // Finding the matching rule for the current tube
                foreach (var SuptingRule in mSubInitializationSupA.pubTblSuptSpanRules)
                {
                    if (SuptingRule.PipeBorOrRackSize == Tube.ABor && SuptingRule.RuleDiscApplicability == Tube.DiscClassification)
                    {
                        SuptingRuleforTube = SuptingRule;
                        break; // Exit the loop once the matching rule is found
                    }
                }

                if (SuptingRuleforTube.HasValue)
                {
                    // Calculating the number of supports required based on the tube length and the applicable rule
                    if (Tube.TubeLength <= SuptingRuleforTube.Value.MaxDistBetweenBendsWoutSupt)
                    {
                        Tube.NoOfEstSuptsReqdonTube = 0;
                    }
                    else if (Tube.TubeLength <= SuptingRuleforTube.Value.MaxDistBetweenBendsWoutSupt * 2)
                    {
                        Tube.NoOfEstSuptsReqdonTube = 1;
                        SuptsReqdFlag = true;
                    }
                    else if (Tube.TubeLength > SuptingRuleforTube.Value.MaxDistBetweenBendsWoutSupt * 2)
                    {
                        // Assuming a method to round numbers similar to VBA's Application.WorksheetFunction.Round
                        // You might need to implement or find a suitable C# equivalent
                        Tube.NoOfEstSuptsReqdonTube = 2 + (int)Math.Round((Tube.TubeLength - 2 * SuptingRuleforTube.Value.MaxDistBetweenBendsWoutSupt) / SuptingRuleforTube.Value.MaxSpan);
                        SuptsReqdFlag = true;
                    }
                    else
                    {
                        Tube.NoOfEstSuptsReqdonTube = 0;
                    }
                }
            }
        }

        private static List<cTubeDefDisc> ScoreSuptPoints(List<cTubeDef> collPipeforSupporting, List<cSteelDisc> collExistingSteelDisc, List<cTubeDefDisc> collSuptPointScores)
        {
            double pubIntDiscretisationStepSize = 1.0; // Define this based on your actual discretisation step size
            float horiDistCheck =0f;
            float vertDistCheck = 0f;

            foreach (var tube in collPipeforSupporting)
            {
                if (new[] { "E", "W", "N", "S", "U", "D" }.Contains(tube.ADir))
                {
                    int intLenDisc = 0;
                    while (intLenDisc <= tube.TubeLength / pubIntDiscretisationStepSize)
                    {
                        var tubeDisc = new cTubeDefDisc
                        {
                            // Assuming WritefromcTubeDef is a method to copy properties from cTubeDef to cTubeDefDisc
                            // This needs to be replaced or implemented according to your actual object model
                            // For demonstration, direct property assignments are shown
                            Dir = tube.ADir,
                            East = tube.AEast + mDecomposeThreeDVectorintoENUCoords.DecomposeThreeDVectorintoENUCoords(tube.ADir, (float)(intLenDisc * pubIntDiscretisationStepSize))[0],
                            North = tube.ANorth + mDecomposeThreeDVectorintoENUCoords.DecomposeThreeDVectorintoENUCoords(tube.ADir, (float)(intLenDisc * pubIntDiscretisationStepSize))[1],
                            Upping = tube.AUpping + mDecomposeThreeDVectorintoENUCoords.DecomposeThreeDVectorintoENUCoords(tube.ADir, (float)(intLenDisc * pubIntDiscretisationStepSize))[2]
                        };

                        foreach (var discSteel in collExistingSteelDisc)
                        {
                            horiDistCheck = 10000000;
                            vertDistCheck = 10000000;

                            // Logic to calculate horiDistCheck and vertDistCheck based on tubeDisc and discSteel positions
                            // Implement this based on your actual logic and method availability

                            if (horiDistCheck != 10000000 && vertDistCheck != 10000000)
                            {
                                GetSuptScoreofPoint(horiDistCheck, vertDistCheck, tubeDisc);
                            }
                        }

                        if (tubeDisc.SuptPointScore <= 0.002)
                        {
                            GetSuptScoreofPoint(horiDistCheck, vertDistCheck, tubeDisc);
                        }

                        collSuptPointScores.Add(tubeDisc);
                        intLenDisc++;
                    }
                }
            }

            return collSuptPointScores;
        }

        private static void GetSuptScoreofPoint(float HoriDistCheck, float VertDistCheck, cTubeDefDisc TubeDisc)
        {
            for (int CountH = 0; CountH < mSubInitializationSupA.pubArrSuptPointEvalMatrix.GetLength(0); CountH++) // Rows
            {
                for (int CountI = 0; CountI < mSubInitializationSupA.pubArrSuptPointEvalMatrix.GetLength(1); CountI++) // Columns
                {
                    // Check if the horizontal and vertical distances match the criteria in the evaluation matrix
                    if (HoriDistCheck < mSubInitializationSupA.pubArrSuptPointEvalMatrix[0, CountI] && VertDistCheck < mSubInitializationSupA.pubArrSuptPointEvalMatrix[CountH, 0])
                    {
                        // Update the support point score if it's less than the current matrix value
                        if (TubeDisc.SuptPointScore < mSubInitializationSupA.pubArrSuptPointEvalMatrix[CountH, CountI])
                        {
                            TubeDisc.SuptPointScore = mSubInitializationSupA.pubArrSuptPointEvalMatrix[CountH, CountI];
                            // Assuming CategorisePointSuit is a method that categorizes the support point based on its score
                            TubeDisc.SuptPointScoreCat = CategorisePointSuit(TubeDisc.SuptPointScore);
                        }
                        return; // Exit the method once a match is found and processed
                    }
                }
            }
        }

        private static int CategorisePointSuit(float pointScore)
        {
            int suptCat = 0; // Default or unknown category

            foreach (var category in mSubInitializationSupA.pubTblSuptScoreCat)
            {
                if (pointScore >= category.MinValforPointScore && pointScore <= category.MaxValforPointScore)
                {
                    suptCat = category.PointCat;
                    break; // Exit the loop once a matching category is found
                }
            }

            return suptCat;
        }

        private static TTblSuptSpanRules? SelectSupportPointsDefineVariables(cTubeDef tubeForSupting)
        {
            // Loop through all the entries in pubTblSuptSpanRules
            foreach (var selectedSuptSpanRule in mSubInitializationSupA.pubTblSuptSpanRules)
            {
                if (selectedSuptSpanRule.PipeBorOrRackSize == tubeForSupting.ABor &&
                    selectedSuptSpanRule.RuleDiscApplicability == tubeForSupting.DiscClassification)
                {
                    // Return the first matching rule
                    return selectedSuptSpanRule;
                }
            }

            // If no matching rule is found, return null or a default TTblSuptSpanRules instance
            // This depends on how you want to handle the case where no rule matches
            return null;
        }

        private static int SetupforSupportonShortRun(cTubeDef tubeForSupting, List<cTubeDefDisc> collSuptPointScores, float allowableSpan, float allowableAfterBend, out int goForwardOrBack, out double noOfLoops, out int counterStartingPoint)
        {
            double suptLocnForEvalE, suptLocnForEvalN, suptLocnForEvalU;

            // Assuming DecomposeThreeDVectorintoENUCoords returns a double[] with E, N, U coordinates
            var decomposedVector = mDecomposeThreeDVectorintoENUCoords.DecomposeThreeDVectorintoENUCoords(tubeForSupting.ADir, tubeForSupting.TubeLength / 2);
            suptLocnForEvalE = mPublicVarDefinitions.RoundDecPlc(tubeForSupting.AEast + decomposedVector[0], mSubInitializationSupA.pubIntDiscretisationStepDecPlaces);
            suptLocnForEvalN = mPublicVarDefinitions.RoundDecPlc(tubeForSupting.ANorth + decomposedVector[1], mSubInitializationSupA.pubIntDiscretisationStepDecPlaces);
            suptLocnForEvalU = mPublicVarDefinitions.RoundDecPlc(tubeForSupting.AUpping + decomposedVector[2], mSubInitializationSupA.pubIntDiscretisationStepDecPlaces);

            counterStartingPoint = -1; // Default to an invalid value

            // Loop through support point scores to find the matching location
            for (int i = 0; i < collSuptPointScores.Count; i++)
            {
                if (collSuptPointScores[i].EastRounded == suptLocnForEvalE &&
                    collSuptPointScores[i].NorthRounded == suptLocnForEvalN &&
                    collSuptPointScores[i].UppingRounded == suptLocnForEvalU)
                {
                    counterStartingPoint = i; // Found the matching point
                    break;
                }
            }

            // Calculate the number of loops and the direction to go forward or back
            noOfLoops = Math.Min(mPublicVarDefinitions.RoundDecPlc(tubeForSupting.TubeLength, -2) / 100,
                                 -mPublicVarDefinitions.RoundDecPlc(tubeForSupting.TubeLength, mSubInitializationSupA.pubIntDiscretisationStepDecPlaces) / 100 + 2 * allowableAfterBend / 100);
            goForwardOrBack = 0; // Assuming this means stay in place, adjust based on actual logic

            return counterStartingPoint; // Assuming a return is needed; adjust as necessary
        }

        private static cTubeDefDisc SelectBestSuptPointinRange(double noOfLoops, ref int counterSuptPointEval, int counterStartingPoint, int goForwardOrBack, List<cTubeDefDisc> collSuptPointScores, string suptRank)
        {
            cTubeDefDisc selectedSuptPoint = new cTubeDefDisc();

            // Assuming pubNoofCategories is defined and accessible within this context.
            for (int countF = 1; countF <= mSubInitializationSupA.pubNoofCategories; countF++)
            {
                if (goForwardOrBack != 0)
                {
                    for (int countE = 1; countE <= noOfLoops; countE++)
                    {
                        counterSuptPointEval = counterStartingPoint + countE * goForwardOrBack;
                        if (IsValidIndex(counterSuptPointEval, collSuptPointScores) &&
                            countF == collSuptPointScores[counterSuptPointEval - 1].SuptPointScoreCat)
                        {
                            AssignPropertiesFromSourceToDestination(collSuptPointScores[counterSuptPointEval - 1], ref selectedSuptPoint, suptRank);
                            return selectedSuptPoint;
                        }
                    }
                }
                else
                {
                    // Handle the case where GoForwardorBack == 0 (central point first, then alternate)
                    int direction = -1; // Start with -1 to alternate direction
                    for (int countE = 0; countE < noOfLoops; countE++)
                    {
                        counterSuptPointEval = counterStartingPoint + (direction * ((countE + 1) / 2));
                        direction *= -1; // Alternate direction

                        if (IsValidIndex(counterSuptPointEval, collSuptPointScores) &&
                            countF == collSuptPointScores[counterSuptPointEval - 1].SuptPointScoreCat)
                        {
                            AssignPropertiesFromSourceToDestination(collSuptPointScores[counterSuptPointEval - 1], ref selectedSuptPoint, suptRank);
                            return selectedSuptPoint;
                        }
                    }
                }
            }

            return selectedSuptPoint; // Return the selected support point, if any
        }

        private static bool IsValidIndex(int index, List<cTubeDefDisc> list)
        {
            return index > 0 && index <= list.Count; // Ensure the index is within the bounds of the list
        }

        private static void AssignPropertiesFromSourceToDestination(cTubeDefDisc source, ref cTubeDefDisc destination, string suptRank)
        {
            // Assuming cTubeDefDisc has properties like East, North, Upping, TubeName, etc.
            destination.East = source.East;
            destination.North = source.North;
            destination.Upping = source.Upping;
            destination.TubeName = source.TubeName;
            destination.HoriOffsetfromStlMin = source.HoriOffsetfromStlMin;
            destination.VertOffsetfromStlMin = source.VertOffsetfromStlMin;
            destination.SuptPointScore = source.SuptPointScore;
            destination.SuptPointScoreCat = source.SuptPointScoreCat;
            destination.ABor = source.ABor;
            destination.LBor = source.LBor;
            destination.Dir = source.Dir;
            destination.SuptRanking = suptRank;
            destination.NameofPipe = source.NameofPipe;
            destination.DiscClassification = source.DiscClassification;
        }

        private static void GetBackwithinArrayandFtub(int Counterstartingpoint, List<cTubeDefDisc> CollSuptPointScores, ref double Suptlocnforeval, int Dirnftub, cTubeDef TubeforSupting)
        {
            // re-adjust for when we're at the very end of the array and suptpointscores has run out of spare entries at the edge to
            // use as a fudge factor
            if (Counterstartingpoint > CollSuptPointScores.Count)
            {
                Suptlocnforeval += Dirnftub * 100 * (CollSuptPointScores.Count - Counterstartingpoint);
                Counterstartingpoint = CollSuptPointScores.Count;
            }

        AmIBacktoMyFtub:
            if (CollSuptPointScores[Counterstartingpoint].TubeName == TubeforSupting.TubeName)
            {
                // do nothing if I am on my ftub
            }
            else
            {
                Counterstartingpoint -= 1;
                Suptlocnforeval -= 100 * Dirnftub;
                goto AmIBacktoMyFtub; // C# supports goto for jumping to labels, though its use is generally discouraged
            }

            // surely I then need to redefine my number of loops and dirn of travel XYZ
        }

        private static void SetupforSupportonStraightRun(cTubeDef TubeforSupting, List<cTubeDefDisc> CollSuptPointScores, float AllowableSpan, float AllowableAfterBend, ref int GoForwardorBack, ref double NoofLoops, ref int Counterstartingpoint, ref bool FullySuptedFlag, float MinSpanRatioBetweenSupts)
        {
            double Suptlocnforeval = 0;
            double SuptLocnForEvalE = 0;
            double SuptLocnForEvalN = 0;
            double SuptLocnForEvalU = 0;
            float Dirnftub = 0;
            float CoordVis = 0;
            float DistfromEnd = 0;
            float CountfromEnd = 0;

            SuptLocnForEvalE = Math.Round(TubeforSupting.AEast, mSubInitializationSupA.pubIntDiscretisationStepDecPlaces);
            SuptLocnForEvalN = Math.Round(TubeforSupting.ANorth, mSubInitializationSupA.pubIntDiscretisationStepDecPlaces);
            SuptLocnForEvalU = Math.Round(TubeforSupting.AUpping, mSubInitializationSupA.pubIntDiscretisationStepDecPlaces);

            if (TubeforSupting.ADir == "E" || TubeforSupting.ADir == "W")
            {
                SuptLocnForEvalE = Math.Round(CollSuptPointScores[Counterstartingpoint].East + mDecomposeThreeDVectorintoENUCoords.DecomposeThreeDVectorintoENUCoords(CollSuptPointScores[Counterstartingpoint].Dir, AllowableSpan)[1], mSubInitializationSupA.pubIntDiscretisationStepDecPlaces);
                Suptlocnforeval = SuptLocnForEvalE;
            }
            else if (TubeforSupting.ADir == "N" || TubeforSupting.ADir == "S")
            {
                SuptLocnForEvalN = Math.Round(CollSuptPointScores[Counterstartingpoint].North + mDecomposeThreeDVectorintoENUCoords.DecomposeThreeDVectorintoENUCoords(CollSuptPointScores[Counterstartingpoint].Dir, AllowableSpan)[2], mSubInitializationSupA.pubIntDiscretisationStepDecPlaces);
                Suptlocnforeval = SuptLocnForEvalN;
            }
            else if (TubeforSupting.ADir == "U" || TubeforSupting.ADir == "D")
            {
                SuptLocnForEvalU = Math.Round(CollSuptPointScores[Counterstartingpoint].Upping + mDecomposeThreeDVectorintoENUCoords.DecomposeThreeDVectorintoENUCoords(CollSuptPointScores[Counterstartingpoint].Dir, AllowableSpan)[3], mSubInitializationSupA.pubIntDiscretisationStepDecPlaces);
                Suptlocnforeval = SuptLocnForEvalU;
            }

            if (TubeforSupting.ADir == "E" || TubeforSupting.ADir == "N" || TubeforSupting.ADir == "U")
            {
                Dirnftub = 1;
            }
            else
            {
                Dirnftub = -1;
            }

            Counterstartingpoint = (int)(Counterstartingpoint + (AllowableSpan / 100));

            GetBackwithinArrayandFtub(Counterstartingpoint, CollSuptPointScores, ref Suptlocnforeval, (int)Dirnftub, TubeforSupting);

            CoordVis = (float)(Dirnftub * (Math.Round(ReturnRelevantCoordinate(TubeforSupting) + Dirnftub * TubeforSupting.TubeLength, mSubInitializationSupA.pubIntDiscretisationStepDecPlaces) - Suptlocnforeval));

            if (CoordVis < AllowableAfterBend)
            {
                FullySuptedFlag = true;

                DistfromEnd = (float)(Dirnftub * (ReturnRelevantCoordinate(TubeforSupting) + Dirnftub * Math.Round(TubeforSupting.TubeLength, mSubInitializationSupA.pubIntDiscretisationStepDecPlaces) - Suptlocnforeval));
                CountfromEnd = DistfromEnd / 100;
                Suptlocnforeval = Math.Round(Suptlocnforeval - (AllowableAfterBend - DistfromEnd) * Dirnftub, mSubInitializationSupA.pubIntDiscretisationStepDecPlaces);
                Counterstartingpoint = (int)(Counterstartingpoint - ((AllowableAfterBend / 100) - CountfromEnd));
                GoForwardorBack = 1;
                NoofLoops = (int)((AllowableAfterBend / 100) - CountfromEnd);
            }
            else
            {
                GoForwardorBack = -1;
                NoofLoops = (int)(MinSpanRatioBetweenSupts * AllowableSpan / 100);
            }
        }

        private static float ReturnRelevantCoordinate(cTubeDef TubeforSupting)
        {
            float relevantCoordinate = 0;

            if (TubeforSupting.ADir == "E" || TubeforSupting.ADir == "W")
            {
                relevantCoordinate = (float)Math.Round(TubeforSupting.AEast, mSubInitializationSupA.pubIntDiscretisationStepDecPlaces);
            }
            else if (TubeforSupting.ADir == "N" || TubeforSupting.ADir == "S")
            {
                relevantCoordinate = (float)Math.Round(TubeforSupting.ANorth, mSubInitializationSupA.pubIntDiscretisationStepDecPlaces);
            }
            else if (TubeforSupting.ADir == "U" || TubeforSupting.ADir == "D")
            {
                relevantCoordinate = (float)Math.Round(TubeforSupting.AUpping, mSubInitializationSupA.pubIntDiscretisationStepDecPlaces);
            }

            return relevantCoordinate;
        }

        private static void SetupforFirstAfterBend(cTubeDef TubeforSupting, List<cTubeDefDisc> CollSuptPointScores, float AllowableAfterBend, ref int GoForwardorBack, ref double NoofLoops, ref int Counterstartingpoint)
        {
            int CountG;
            double Suptlocnforeval = 0;
            double SuptLocnForEvalE = 0;
            double SuptLocnForEvalN = 0;
            double SuptLocnForEvalU = 0;

            SuptLocnForEvalE = Math.Round(TubeforSupting.AEast, mSubInitializationSupA.pubIntDiscretisationStepDecPlaces);
            SuptLocnForEvalN = Math.Round(TubeforSupting.ANorth, mSubInitializationSupA.pubIntDiscretisationStepDecPlaces);
            SuptLocnForEvalU = Math.Round(TubeforSupting.AUpping, mSubInitializationSupA.pubIntDiscretisationStepDecPlaces);

            if (TubeforSupting.ADir == "E" || TubeforSupting.ADir == "W")
            {
                SuptLocnForEvalE = Math.Round(TubeforSupting.AEast + mDecomposeThreeDVectorintoENUCoords.DecomposeThreeDVectorintoENUCoords(TubeforSupting.ADir, (float)AllowableAfterBend)[1], mSubInitializationSupA.pubIntDiscretisationStepDecPlaces);
                Suptlocnforeval = SuptLocnForEvalE;
            }
            else if (TubeforSupting.ADir == "N" || TubeforSupting.ADir == "S")
            {
                SuptLocnForEvalN = Math.Round(TubeforSupting.ANorth + mDecomposeThreeDVectorintoENUCoords.DecomposeThreeDVectorintoENUCoords(TubeforSupting.ADir, (float)AllowableAfterBend)[2], mSubInitializationSupA.pubIntDiscretisationStepDecPlaces);
                Suptlocnforeval = SuptLocnForEvalN;
            }
            else if (TubeforSupting.ADir == "U" || TubeforSupting.ADir == "D")
            {
                SuptLocnForEvalU = Math.Round(TubeforSupting.AUpping + mDecomposeThreeDVectorintoENUCoords.DecomposeThreeDVectorintoENUCoords(TubeforSupting.ADir, (float)AllowableAfterBend)[3], mSubInitializationSupA.pubIntDiscretisationStepDecPlaces);
                Suptlocnforeval = SuptLocnForEvalU;
            }

            for (CountG = 1; CountG <= CollSuptPointScores.Count; CountG++)
            {
                if (CollSuptPointScores[CountG].EastRounded == SuptLocnForEvalE && CollSuptPointScores[CountG].NorthRounded == SuptLocnForEvalN && CollSuptPointScores[CountG].UppingRounded == SuptLocnForEvalU)
                {
                    Counterstartingpoint = CountG;
                    break;
                }
            }

            NoofLoops = (int)(AllowableAfterBend / 100);
            GoForwardorBack = -1;
        }

        public void SetupforSupportonShortRun(cTubeDef TubeforSupting, List<cTubeDefDisc> CollSuptPointScores, float AllowableSpan, float AllowableAfterBend, int GoForwardorBack, double NoofLoops, ref long Counterstartingpoint)
        {
            int CountG;
            double SuptLocnForEvalE;
            double SuptLocnForEvalN;
            double SuptLocnForEvalU;

            SuptLocnForEvalE = Math.Round(TubeforSupting.AEast + mDecomposeThreeDVectorintoENUCoords.DecomposeThreeDVectorintoENUCoords(TubeforSupting.ADir, TubeforSupting.TubeLength / 2)[1], mSubInitializationSupA.pubIntDiscretisationStepDecPlaces);
            SuptLocnForEvalN = Math.Round(TubeforSupting.ANorth + mDecomposeThreeDVectorintoENUCoords.DecomposeThreeDVectorintoENUCoords(TubeforSupting.ADir, TubeforSupting.TubeLength / 2)[2], mSubInitializationSupA.pubIntDiscretisationStepDecPlaces);
            SuptLocnForEvalU = Math.Round(TubeforSupting.AUpping + mDecomposeThreeDVectorintoENUCoords.DecomposeThreeDVectorintoENUCoords(TubeforSupting.ADir, TubeforSupting.TubeLength / 2)[3], mSubInitializationSupA.pubIntDiscretisationStepDecPlaces);

            // then get to that point in the suptpointscores array so we can work from that point
            for (CountG = 1; CountG <= CollSuptPointScores.Count; CountG++)
            {
                if (CollSuptPointScores[CountG].EastRounded == SuptLocnForEvalE && CollSuptPointScores[CountG].NorthRounded == SuptLocnForEvalN && CollSuptPointScores[CountG].UppingRounded == SuptLocnForEvalU)
                {
                    Counterstartingpoint = CountG;
                    break;
                }
            }

            // XYZ we have defined our starting point but what about number of loops?
            NoofLoops = Math.Min(Math.Round(TubeforSupting.TubeLength, -2) / 100, -Math.Round(TubeforSupting.TubeLength, mSubInitializationSupA.pubIntDiscretisationStepDecPlaces) / 100 + 2 * (AllowableAfterBend / 100));
            GoForwardorBack = 0;
        }

        public cTubeDefDisc SelectBestSuptPointinRange(long NoofLoops, ref int CounterSuptPointEval, int Counterstartingpoint, int GoForwardorBack, List<cTubeDefDisc> CollSuptPointScores, string SuptRank)
        {
            cTubeDefDisc SelectedSuptPoint = new cTubeDefDisc();

            // Try find a category 1 support within those limits.
            // If no cat 1 cat 2 then 3 then 4
            for (int CountF = 1; CountF <= mSubInitializationSupA.pubNoofCategories; CountF++)
            {
                if (GoForwardorBack != 0)
                {
                    // un-directional loop case
                    for (int CountE = 1; CountE <= NoofLoops; CountE++)
                    {
                        // this indexes the support point we are currently evaluating
                        CounterSuptPointEval = Counterstartingpoint + CountE * GoForwardorBack;

                        if (CountF == CollSuptPointScores[CounterSuptPointEval].SuptPointScoreCat)
                        {
                            SelectedSuptPoint.East = CollSuptPointScores[CounterSuptPointEval].East;
                            SelectedSuptPoint.North = CollSuptPointScores[CounterSuptPointEval].North;
                            SelectedSuptPoint.Upping = CollSuptPointScores[CounterSuptPointEval].Upping;
                            SelectedSuptPoint.TubeName = CollSuptPointScores[CounterSuptPointEval].TubeName;
                            SelectedSuptPoint.HoriOffsetfromStlMin = CollSuptPointScores[CounterSuptPointEval].HoriOffsetfromStlMin;
                            SelectedSuptPoint.VertOffsetfromStlMin = CollSuptPointScores[CounterSuptPointEval].VertOffsetfromStlMin;
                            SelectedSuptPoint.SuptPointScore = CollSuptPointScores[CounterSuptPointEval].SuptPointScore;
                            SelectedSuptPoint.SuptPointScoreCat = CollSuptPointScores[CounterSuptPointEval].SuptPointScoreCat;
                            SelectedSuptPoint.ABor = CollSuptPointScores[CounterSuptPointEval].ABor;
                            SelectedSuptPoint.LBor = CollSuptPointScores[CounterSuptPointEval].LBor;
                            SelectedSuptPoint.Dir = CollSuptPointScores[CounterSuptPointEval].Dir;
                            SelectedSuptPoint.SuptRanking = SuptRank;
                            SelectedSuptPoint.NameofPipe = CollSuptPointScores[CounterSuptPointEval].NameofPipe;
                            SelectedSuptPoint.DiscClassification = CollSuptPointScores[CounterSuptPointEval].DiscClassification;

                            // as soon as we have one good support point then we can exit this
                            return SelectedSuptPoint;
                        }
                    }
                }
                else
                {
                    // alternating loop direction case
                    for (int CountE = 0; CountE < NoofLoops; CountE++)
                    {
                        // this is the logic for deciding how far away from our ideal support point we are evaluating first
                        int tmpCounterSuptPointEval = 0;
                        if (CountE == 0)
                        {
                            tmpCounterSuptPointEval = Counterstartingpoint;
                        }
                        if (mSubInitializationSupA.Application.Worksheets.Application.WorksheetFunction.IsEven(CountE))
                        {
                            tmpCounterSuptPointEval = Counterstartingpoint - CountE / 2;
                        }
                        else if (mSubInitializationSupA.Application.Worksheets.Application.WorksheetFunction.IsOdd(CountE))
                        {
                            tmpCounterSuptPointEval = Counterstartingpoint + (1 + (CountE - 1) / 2);
                        }

                        // This if statement added as on short tubi's an error was throwing when
                        // CounterSuptPointEval = 0 as 0 doesn't exist in a collection
                        if (tmpCounterSuptPointEval != 0)
                        {
                            // this is the logic for making sure we only evaluate supts in current category
                            // also we only do this loop if countersuptpointeval <= ubound of array (the numbering of loops defined above is a little bit dodgy)
                            if (tmpCounterSuptPointEval <= CollSuptPointScores.Count)
                            {
                                if (CountF == CollSuptPointScores[tmpCounterSuptPointEval].SuptPointScoreCat)
                                {
                                    SelectedSuptPoint.East = CollSuptPointScores[tmpCounterSuptPointEval].East;
                                    SelectedSuptPoint.North = CollSuptPointScores[tmpCounterSuptPointEval].North;
                                    SelectedSuptPoint.Upping = CollSuptPointScores[tmpCounterSuptPointEval].Upping;
                                    SelectedSuptPoint.TubeName = CollSuptPointScores[tmpCounterSuptPointEval].TubeName;
                                    SelectedSuptPoint.HoriOffsetfromStlMin = CollSuptPointScores[tmpCounterSuptPointEval].HoriOffsetfromStlMin;
                                    SelectedSuptPoint.VertOffsetfromStlMin = CollSuptPointScores[tmpCounterSuptPointEval].VertOffsetfromStlMin;
                                    SelectedSuptPoint.SuptPointScore = CollSuptPointScores[tmpCounterSuptPointEval].SuptPointScore;
                                    SelectedSuptPoint.SuptPointScoreCat = CollSuptPointScores[tmpCounterSuptPointEval].SuptPointScoreCat;
                                    SelectedSuptPoint.ABor = CollSuptPointScores[tmpCounterSuptPointEval].ABor;
                                    SelectedSuptPoint.LBor = CollSuptPointScores[tmpCounterSuptPointEval].LBor;
                                    SelectedSuptPoint.SuptRanking = SuptRank;
                                    SelectedSuptPoint.NameofPipe = CollSuptPointScores[tmpCounterSuptPointEval].NameofPipe;
                                    SelectedSuptPoint.Dir = CollSuptPointScores[tmpCounterSuptPointEval].Dir;
                                    SelectedSuptPoint.DiscClassification = CollSuptPointScores[tmpCounterSuptPointEval].DiscClassification;

                                    // as soon as we have one good support point then we can exit this
                                    return SelectedSuptPoint;
                                }
                            }
                        }
                    }
                }
            }

            // once we've found a support point we either loop or finish, depending if it was the last one required
            return SelectedSuptPoint;
        }

        private static List<cTubeDefDisc> SelectSupportPoints(List<cTubeDef> CollPipeforSupporting, List<cTubeDefDisc> CollSuptPointScores)
        {
            bool FullySuptedFlag;
            int GoForwardorBack = 0;
            double NoofLoops = 0;
            bool FullySuptflag;
            string Loopstrategy;
            string SuptRank = string.Empty;
            cTubeDef TubeforSupting;
            TTblSuptSpanRules? SelectedSuptSpanRule;
            int Counterstartingpoint = 0;
            int CounterSuptPointEval = 0;
            bool PipOrthFlag;
            var CollSelectedSuptLocns = new List<cTubeDefDisc>
            {
                new()
            };

            // Loop through all of our pipe tubes
            int CountD = 1;
            while (CountD <= CollPipeforSupporting.Count)
            {
            // We progress along the Tube adding supports.
            // We keep looping through it until all supports are added and the Tube is fully supported
            // However, we do need to find some way of identifying if we are stuck in a truly endless loop.
            // How is TBC still? XYZ
            // this will not be a do loop, but every time we get to the end we start again, unless fullysuptedflag = true
            notfullysupported:
                FullySuptedFlag = false;
                // Just a little error catching over here
                if (CountD > CollPipeforSupporting.Count)
                    break;
                TubeforSupting = CollPipeforSupporting[CountD];

                // We set up our selected support span rule. This is the rule which matches our bores and discipline being supported.
                SelectedSuptSpanRule = SelectSupportPointsDefineVariables(TubeforSupting);
                PipOrthFlag = false;
                if (TubeforSupting.ADir == "E" || TubeforSupting.ADir == "W" || TubeforSupting.ADir == "N" || TubeforSupting.ADir == "S")
                {
                    PipOrthFlag = true;
                }

                // at the beginning of every loop, we reset CounterStartingPoint = CounterSuptPointEval
                Counterstartingpoint = CounterSuptPointEval;

                // if we are dealing with an orthogonal pipe
                if (TubeforSupting.TubeType == "TUBI" && PipOrthFlag == true)
                {
                    // All of the below is setting up various starting point
                    // variables which then feed into the function SelectBestSuptPointinRange
                    // If this is a pipe
                    if (TubeforSupting.DiscClassification == "P")
                    {
                        // The below hasn't been incorporated into the re-write
                        // this below code changes the generic "allowable after bend distance" to a number
                        // of specific allowables to take account of various alternative layouts nuances
                        // I should change the variable name from allowableafterbend to something else to indicate that XYZ
                        // if this is the first support on a straight run of pipe which requires no supports then
                        if (TubeforSupting.NoOfEstSuptsReqdonTube == 0 && TubeforSupting.TubeType == "TUBI")
                        {
                            // there needs to be a piece of code in here for option B supports to be added along each section XYZ
                            // To work this requires mods to other parts of the code
                            SetupforSupportonShortRun(TubeforSupting, CollSuptPointScores, SelectedSuptSpanRule.Value.MaxSpan, SelectedSuptSpanRule.Value.MaxSuptDistfromBends, out GoForwardorBack, out NoofLoops, out Counterstartingpoint);
                            SuptRank = "B";
                            FullySuptedFlag = true;
                        }
                        else if ((string)CollSelectedSuptLocns[CollSelectedSuptLocns.Count].TubeName != TubeforSupting.TubeName && TubeforSupting.NoOfEstSuptsReqdonTube > 1)
                        {
                            SetupforFirstAfterBend(TubeforSupting, CollSuptPointScores, SelectedSuptSpanRule.Value.MaxSuptDistfromBends, ref GoForwardorBack, ref NoofLoops, ref Counterstartingpoint);
                            SuptRank = "A";
                        }
                        else if ((string)CollSelectedSuptLocns[CollSelectedSuptLocns.Count].TubeName == TubeforSupting.TubeName && TubeforSupting.NoOfEstSuptsReqdonTube > 1)
                        {
                            SetupforSupportonStraightRun(TubeforSupting, CollSuptPointScores, SelectedSuptSpanRule.Value.MaxSpan, SelectedSuptSpanRule.Value.MaxSuptDistfromBends, ref GoForwardorBack, ref NoofLoops, ref Counterstartingpoint, ref FullySuptedFlag, SelectedSuptSpanRule.Value.MinSpanBtwnSuptsRatio);
                            SuptRank = "A";
                        }
                        else if (TubeforSupting.NoOfEstSuptsReqdonTube == 1)
                        {
                            SetupforSupportonShortRun(TubeforSupting, CollSuptPointScores, SelectedSuptSpanRule.Value.MaxSpan, SelectedSuptSpanRule.Value.MaxSuptDistfromBends, out GoForwardorBack, out NoofLoops, out Counterstartingpoint);
                            SuptRank = "A";
                            FullySuptedFlag = true;
                        }
                        else
                        {
                            // no cases met
                            //MessageBox.Show("no cases met");
                        }
                    }

                    // XYZ. This explanation is pants and needs re-writing
                    // This code loops through the section of pipe designed in a number of loops and tries to find the best support point in
                    // cat 1 , then 2  , then 3 , up to no of categories
                    // It does this based on the starting parameters kicked out in the preceding if statements.
                    var SelectedSuptPoint = new cTubeDefDisc();
                    SelectedSuptPoint = SelectBestSuptPointinRange(NoofLoops, ref CounterSuptPointEval, Counterstartingpoint, GoForwardorBack, CollSuptPointScores, SuptRank);
                    CollSelectedSuptLocns.Add(SelectedSuptPoint);

                    if (FullySuptedFlag == false)
                    {
                        // This CountD = CountD + 1 is confusing, but it is here because the goto notfullysupported
                        // label means we don't get to the counter at the end of the loop except when FullySuptedFlag = True
                        // (or in the other conditions iff-ed above (error catchers)
                        // CountD = CountD + 1;
                        goto notfullysupported;
                    }
                }

                CountD++;
            }

            // Return CollSelectedSuptLocns;
            return CollSelectedSuptLocns;
        }
    }
}
