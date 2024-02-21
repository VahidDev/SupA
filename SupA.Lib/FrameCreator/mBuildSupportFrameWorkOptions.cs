﻿using SupA.Lib.BuildSupportFrameWorkOptions;
using SupA.Lib.Core;
using SupA.Lib.DataManipulation;
using SupA.Lib.Initialization;

namespace SupA.Lib.FrameCreator
{
    public class mBuildSupportFrameWorkOptions
    {
        public List<cPotlSupt> BuildSupportFrameWorkOptions(
        List<cSuptPoints> collAdjacentSuptPoints,
        object[] arrNoofLevels,
        List<object> collLocalClashData,
        List<cSteelDisc> collLocalExistingSteelDisc,
        int noofSuptBeamEndCoords,
        int suptGroupNo)
        {
            List<cSteel> collSuptBeams;
            List<object> collExtendedBeams;
            List<object> collTrimSteels;
            List<object> collVerticalCols;
            List<cSteelDisc> collSuptBeamsDisc;
            List<cSteelDisc> collExtendedBeamsDisc;
            List<cSteelDisc> collTrimSteelsDisc;
            List<object> collVerticalColsDisc;
            List<cSteelDisc> collAllDiscBeams;
            List<cSteelDisc> collAllDiscBeamsforNodeMap;
            List<cRouteNode> collFrameNodeMap;
            List<cGroupNode> collFrameNodeMapGrouped;
            List<cGroupNode> collIntersectionGroupNodes;
            List<cPotlSupt> collPotlSuptFrameDetails;
            List<object> collGroupNodeBeams;

            // Calculate the centre point of our supports.
            // This is used to make sure there is a group and intersection Node at this point
            // so that we can correctly detail T-posts.
            cSuptPoints suptPointEffectiveCentre = mGroupSimilarFrameNodes.SetEffectiveCentreofAdjacentSupts(collAdjacentSuptPoints);

            // Now create the collection of horizontal beams required to support these pipes
            collSuptBeams = CreateMinSuptBeams(collAdjacentSuptPoints, arrNoofLevels, noofSuptBeamEndCoords, suptGroupNo);

            // Now create extensions to the minimum length support beams and store these in CreateExtendedBeams
            collExtendedBeams = mCreateExtendedBeams.CreateExtendedBeams(collSuptBeams, collLocalExistingSteelDisc, collLocalClashData, suptGroupNo);

            // Now create all potential trim steel which could be part of our support
            collTrimSteels = CreateTrimSteel(collSuptBeams, collAdjacentSuptPoints, collLocalExistingSteelDisc, collLocalClashData, suptGroupNo);

            collSuptBeamsDisc = DiscretiseBeamsforFrames(collSuptBeams);
            mExportColltoCSVFile<cSteelDisc>.ExportColltoCSVFile(collSuptBeamsDisc, "CollSuptBeamsDisc-F" + (suptGroupNo + mSubInitializationSupA.SuptGroupNoMod), "csv");

            collExtendedBeamsDisc = DiscretiseBeamsforFrames(collExtendedBeams);
            mExportColltoCSVFile<cSteelDisc>.ExportColltoCSVFile(collExtendedBeamsDisc, "CollExtendedBeamsDisc-F" + (suptGroupNo + mSubInitializationSupA.SuptGroupNoMod), "csv");

            collTrimSteelsDisc = DiscretiseBeamsforFrames(collTrimSteels);
            mExportColltoCSVFile<cSteelDisc>.ExportColltoCSVFile(collTrimSteelsDisc, "CollTrimSteelsDisc-F" + (suptGroupNo + mSubInitializationSupA.SuptGroupNoMod), "csv");

            // Now combine all discretised beam points into a single collection for use in CreateVerticalCols
            collAllDiscBeams = MergeCollection(collSuptBeamsDisc, collExtendedBeamsDisc, collTrimSteelsDisc, collLocalExistingSteelDisc);

            // now delete all the collLocalExistingSteelDisc which is parallel to our pipe to reduce the size of our problem space
            RationaliseLocalExistingSteel(collAllDiscBeams, collAdjacentSuptPoints);

            mExportColltoCSVFile<cSteelDisc>.ExportColltoCSVFile(collAllDiscBeams, "CollAllDiscBeams-F" + (suptGroupNo + mSubInitializationSupA.SuptGroupNoMod), "csv");

            // Use collAllDiscBeams to create vertical columns which could be part of our support
            collVerticalCols = CreateVerticalCols(collAllDiscBeams, collLocalClashData, suptGroupNo);
            collVerticalColsDisc = DiscretiseBeamsforFrames(collVerticalCols);
            mExportColltoCSVFile<object>.ExportColltoCSVFile(collVerticalColsDisc, "CollVerticalColsDisc-F" + (suptGroupNo + mSubInitializationSupA.SuptGroupNoMod), "csv");

            // Now combine all discretised beams and columns (excluding existing steel discretised nodes) into a single collection
            // for use as the basis of creating frame options
            collAllDiscBeamsforNodeMap = MergeCollection(collSuptBeamsDisc, collExtendedBeamsDisc, collTrimSteelsDisc, collVerticalColsDisc);
            mExportColltoCSVFile<cSteelDisc>.ExportColltoCSVFile(collAllDiscBeamsforNodeMap, "CollAllDiscBeamsforNodeMap-F" + (suptGroupNo + mSubInitializationSupA.SuptGroupNoMod), "csv");

            // Create a frame node map for use in frame option creation
            collFrameNodeMap = mCreateCompleteNodemap.CreateCompleteNodemap(collLocalExistingSteelDisc, collAllDiscBeamsforNodeMap, suptGroupNo);

            // Now Group the nodes which can be treated as a single entity
            collFrameNodeMapGrouped = new List<cGroupNode>();
            collIntersectionGroupNodes = new List<cGroupNode>();
            mGroupSimilarFrameNodes.GroupSimilarFrameNodes(collFrameNodeMap, collVerticalCols, collFrameNodeMapGrouped, collIntersectionGroupNodes, collGroupNodeBeams, collAdjacentSuptPoints, suptGroupNo, suptPointEffectiveCentre);

            // Now build collPotlSuptFrameDetails with start points
            collPotlSuptFrameDetails = PopulateStartsinPotlFrameColl(collIntersectionGroupNodes, collGroupNodeBeams, suptPointEffectiveCentre);

            // Now start PopulateStartsinPotlFrameColl
            mRoutePotentialSuptFrames.RoutePotentialSuptFrames(collPotlSuptFrameDetails, collIntersectionGroupNodes, collGroupNodeBeams, noofSuptBeamEndCoords, suptGroupNo);

            return collPotlSuptFrameDetails;
        }

        public void RationaliseLocalExistingSteel(List<cSteelDisc> collAllDiscBeams, List<cSuptPoints> collAdjacentSuptPoints)
        {
            cSuptPoints suptPoint;
            cSteelDisc discSteel;
            bool removeDiscPoint;
            float LL1c = 1;
            while (LL1c <= collAllDiscBeams.Count)
            {
                discSteel = collAllDiscBeams[(int)LL1c];
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
                    collAllDiscBeams.Remove(discSteel);
                    LL1c--;
                }
                LL1c++;
            }
        }

        public static List<cSteelDisc> MergeCollection(List<cSteelDisc> col1 = null, List<cSteelDisc> col2 = null, List<cSteelDisc> col3 = null, List<cSteelDisc> col4 = null, List<cSteelDisc> col5 = null, List<cSteelDisc> col6 = null)
        {
            // Add items from col2 to col1 and return the result
            // The function returns a NEW merged list
            var colNew = new List<cSteelDisc>();

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
