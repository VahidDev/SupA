using SupA.Lib.Core;
using SupA.Lib.DataManipulation;
using SupA.Lib.Initialization;

namespace SupA.Lib.FrameCreator
{
    public class mBuildSupportFrameWorkOptions
    {
        public List<cSuptPoints> BuildSupportFrameWorkOptions(
        List<cSuptPoints> collAdjacentSuptPoints,
        object[] arrNoofLevels,
        List<object> collLocalClashData,
        List<object> collLocalExistingSteelDisc,
        int noofSuptBeamEndCoords,
        int suptGroupNo)
        {
            List<object> collSuptBeams;
            List<object> collExtendedBeams;
            List<object> collTrimSteels;
            List<object> collVerticalCols;
            List<object> collSuptBeamsDisc;
            List<object> collExtendedBeamsDisc;
            List<object> collTrimSteelsDisc;
            List<object> collVerticalColsDisc;
            List<object> collAllDiscBeams;
            List<object> collAllDiscBeamsforNodeMap;
            List<object> collFrameNodeMap;
            List<object> collFrameNodeMapGrouped;
            List<object> collIntersectionGroupNodes;
            List<cSuptPoints> collPotlSuptFrameDetails;
            List<object> collGroupNodeBeams;

            // Calculate the centre point of our supports.
            // This is used to make sure there is a group and intersection Node at this point
            // so that we can correctly detail T-posts.
            cSuptPoints suptPointEffectiveCentre = SetEffectiveCentreofAdjacentSupts(collAdjacentSuptPoints);

            // Now create the collection of horizontal beams required to support these pipes
            collSuptBeams = CreateMinSuptBeams(collAdjacentSuptPoints, arrNoofLevels, noofSuptBeamEndCoords, suptGroupNo);

            // Now create extensions to the minimum length support beams and store these in CreateExtendedBeams
            collExtendedBeams = CreateExtendedBeams(collSuptBeams, collLocalExistingSteelDisc, collLocalClashData, suptGroupNo);

            // Now create all potential trim steel which could be part of our support
            collTrimSteels = CreateTrimSteel(collSuptBeams, collAdjacentSuptPoints, collLocalExistingSteelDisc, collLocalClashData, suptGroupNo);

            collSuptBeamsDisc = DiscretiseBeamsforFrames(collSuptBeams);
            mExportColltoCSVFile<object>.ExportColltoCSVFile(collSuptBeamsDisc, "CollSuptBeamsDisc-F" + (suptGroupNo + mSubInitializationSupA.SuptGroupNoMod), "csv");

            collExtendedBeamsDisc = DiscretiseBeamsforFrames(collExtendedBeams);
            mExportColltoCSVFile<object>.ExportColltoCSVFile(collExtendedBeamsDisc, "CollExtendedBeamsDisc-F" + (suptGroupNo + mSubInitializationSupA.SuptGroupNoMod), "csv");

            collTrimSteelsDisc = DiscretiseBeamsforFrames(collTrimSteels);
            mExportColltoCSVFile<object>.ExportColltoCSVFile(collTrimSteelsDisc, "CollTrimSteelsDisc-F" + (suptGroupNo + mSubInitializationSupA.SuptGroupNoMod), "csv");

            // Now combine all discretised beam points into a single collection for use in CreateVerticalCols
            collAllDiscBeams = MergeCollection(collSuptBeamsDisc, collExtendedBeamsDisc, collTrimSteelsDisc, collLocalExistingSteelDisc);

            // now delete all the collLocalExistingSteelDisc which is parallel to our pipe to reduce the size of our problem space
            RationaliseLocalExistingSteel(collAllDiscBeams, collAdjacentSuptPoints);

            mExportColltoCSVFile<object>.ExportColltoCSVFile(collAllDiscBeams, "CollAllDiscBeams-F" + (suptGroupNo + mSubInitializationSupA.SuptGroupNoMod), "csv");

            // Use collAllDiscBeams to create vertical columns which could be part of our support
            collVerticalCols = CreateVerticalCols(collAllDiscBeams, collLocalClashData, suptGroupNo);
            collVerticalColsDisc = DiscretiseBeamsforFrames(collVerticalCols);
            mExportColltoCSVFile<object>.ExportColltoCSVFile(collVerticalColsDisc, "CollVerticalColsDisc-F" + (suptGroupNo + mSubInitializationSupA.SuptGroupNoMod), "csv");

            // Now combine all discretised beams and columns (excluding existing steel discretised nodes) into a single collection
            // for use as the basis of creating frame options
            collAllDiscBeamsforNodeMap = MergeCollection(collSuptBeamsDisc, collExtendedBeamsDisc, collTrimSteelsDisc, collVerticalColsDisc);
            mExportColltoCSVFile<object>.ExportColltoCSVFile(collAllDiscBeamsforNodeMap, "CollAllDiscBeamsforNodeMap-F" + (suptGroupNo + mSubInitializationSupA.SuptGroupNoMod), "csv");

            // Create a frame node map for use in frame option creation
            collFrameNodeMap = CreateCompleteNodemap(collLocalExistingSteelDisc, collAllDiscBeamsforNodeMap, suptGroupNo);

            // Now Group the nodes which can be treated as a single entity
            collFrameNodeMapGrouped = new List<FrameNodeMapGrouped>();
            collIntersectionGroupNodes = new List<IntersectionGroupNodes>();
            GroupSimilarFrameNodes(collFrameNodeMap, collVerticalCols, collFrameNodeMapGrouped, collIntersectionGroupNodes, collGroupNodeBeams, collAdjacentSuptPoints, suptGroupNo, suptPointEffectiveCentre);

            // Now build collPotlSuptFrameDetails with start points
            collPotlSuptFrameDetails = PopulateStartsinPotlFrameColl(collIntersectionGroupNodes, collGroupNodeBeams, suptPointEffectiveCentre);

            // Now start PopulateStartsinPotlFrameColl
            RoutePotentialSuptFrames(collPotlSuptFrameDetails, collIntersectionGroupNodes, collGroupNodeBeams, noofSuptBeamEndCoords, suptGroupNo);

            return collPotlSuptFrameDetails;
        }

        public void RationaliseLocalExistingSteel(List<object> collAllDiscBeams, List<cSuptPoints> collAdjacentSuptPoints)
        {
            cSuptPoints suptPoint;
            cSteelDisc discSteel;
            bool removeDiscPoint;
            float LL1c = 1;
            while (LL1c <= collAllDiscBeams.Count)
            {
                discSteel = (cSteelDisc)collAllDiscBeams[(int)LL1c];
                removeDiscPoint = false;
                foreach (cSuptPoints item in collAdjacentSuptPoints)
                {
                    suptPoint = item;
                    if (mDirectionCheck.DirectionCheck(suptPoint.Tubidir, discSteel.DirnofBeam) == "parallel" && discSteel.ExistingConnType == "CONCRETE")
                    {
                        removeDiscPoint = true;
                    }
                }
                if (removeDiscPoint)
                {
                    collAllDiscBeams.Remove((int)LL1c);
                    LL1c--;
                }
                LL1c++;
            }
        }

        public static List<object> MergeCollection(List<object> col1 = null, List<object> col2 = null, List<object> col3 = null, List<object> col4 = null, List<object> col5 = null, List<object> col6 = null)
        {
            // Add items from col2 to col1 and return the result
            // The function returns a NEW merged list
            List<object> colNew = new List<object>();

            if (col1 != null)
            {
                foreach (var item in col1)
                {
                    colNew.Add(item);
                }
            }
            if (col2 != null)
            {
                foreach (var item in col2)
                {
                    colNew.Add(item);
                }
            }
            if (col3 != null)
            {
                foreach (var item in col3)
                {
                    colNew.Add(item);
                }
            }
            if (col4 != null)
            {
                foreach (var item in col4)
                {
                    colNew.Add(item);
                }
            }
            if (col5 != null)
            {
                foreach (var item in col5)
                {
                    colNew.Add(item);
                }
            }
            if (col6 != null)
            {
                foreach (var item in col6)
                {
                    colNew.Add(item);
                }
            }

            return colNew; // return the new merged list
        }
    }
}
