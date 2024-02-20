using SupA.Lib.Core;

namespace SupA.Lib.DataManipulation
{
    public class mRoutePotentialSuptFrames
    {
        public void RoutePotentialSuptFrames(List<cPotlSupt> collPotlSuptFrameDetails, List<object> collFrameNodeMapGrouped, List<object> collGroupNodeBeams, int noOfSuptBeamEndCoords, int suptGroupNo)
        {
            var arrDirnsToAttempt = new string[,] { { "", "E" }, { "", "N" }, { "", "U" }, { "", "W" }, { "", "S" }, { "", "D" } };
            int noOfLoops = 0;
            int noOfClosedRoutesWithTieIns = 0;
            bool untravelledPathsOutstanding = true;
            int sinceSorting = 1;

            List<cPotlSupt> collInvalidPotlSuptFrameDetails = new List<cPotlSupt>();

            int ll1c = 1;
            while (ll1c <= collPotlSuptFrameDetails.Count && noOfClosedRoutesWithTieIns < 2000)
            {
                var currPotlSuptFrame = collPotlSuptFrameDetails[ll1c - 1];
                if (!currPotlSuptFrame.IsClosed)
                {
                    // IDPotTravelDirnsFromCENode, AddRouteBranchesandOptions, and SortCollection are placeholder methods for the actual logic to be implemented
                    currPotlSuptFrame.GroupNodesinFrame.Add(currPotlSuptFrame.PathsUnTravelled[0]);
                    currPotlSuptFrame.PathsUnTravelled.RemoveAt(0);

                    // Simulating the removal and processing logic as per VBA
                    collPotlSuptFrameDetails.RemoveAt(ll1c - 1);
                    ll1c--;

                    if (sinceSorting >= 1000)
                    {
                        collPotlSuptFrameDetails = SortCollection(collPotlSuptFrameDetails); // Placeholder for actual sorting logic
                        sinceSorting = 1;
                        ll1c = 0;
                    }
                    sinceSorting++;
                }

                ll1c++;
                // Simulating the update of a progress bar or similar UI element
            }

            // Conditional exports based on `pubBOOLTraceOn`
            if (pubBOOLTraceOn)
            {
                ExportColltoCSVFile(collPotlSuptFrameDetails, $"CollPotlSuptFrameDetails-unfiltered-GroupNodes-F{suptGroupNo}", "WriteAllGroupNodesInFrame");
                ExportColltoCSVFile(collInvalidPotlSuptFrameDetails, $"CollInvalidPotlSuptFrameDetails-Beams-F{suptGroupNo}", "WriteAllBeamsinFrame");
            }

            // Further processing steps as per original VBA code
            FilterPotlSuptFramestoFeasible(collPotlSuptFrameDetails, collFrameNodeMapGrouped, collGroupNodeBeams);
            MakePotlSuptFramesBeamsUnique(collPotlSuptFrameDetails);
        }

        public void MakePotlSuptFramesBeamsUnique(List<cPotlSupt> collPotlSuptFrameDetails)
        {
            foreach (var potlFrame in collPotlSuptFrameDetails)
            {
                List<cSteel> newBeamsInFrame = new List<cSteel>();
                foreach (var beam in potlFrame.BeamsInFrame)
                {
                    cSteel newBeam = new cSteel
                    {
                        // Assuming CopyClassInstance method or similar logic to clone or copy beam details
                    };
                    newBeamsInFrame.Add(newBeam);
                }
                potlFrame.BeamsInFrame = newBeamsInFrame;
            }
        }
    }
}
