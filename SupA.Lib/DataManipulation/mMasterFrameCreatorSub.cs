using SupA.Lib.Core;
using SupA.Lib.FEA;
using SupA.Lib.FrameCreator;
using SupA.Lib.Initialization;
using SupA.Lib.Models;
using SupA.Lib.Utils;
using static SupA.Lib.FrameCreator.mCollectLocalClashData;

namespace SupA.Lib.DataManipulation
{
    public class mMasterFrameCreatorSub
    {
        public static void MasterFrameCreatorSub(List<cSuptPoints> CollSuptPointsinArea, List<cSteelDisc> CollExistingSteelDisc, List<cClashData> CollAllClashData, List<cSteel> CollExistingSteel)
        {
            int IntNoofSuptsGrouped;
            List<cSuptPoints> CollAdjacentSuptPoints = new List<cSuptPoints>(); ;
            List<cSteelDisc> CollLocalExistingSteelDisc = new List<cSteelDisc>(); ;
            List<cSteel> CollExtendedBeams = new List<cSteel>(); ;
            List<cSteel> CollSuptBeams = new List<cSteel>(); ;
            CollectLocalClashDataResult CollLocalClashData = new CollectLocalClashDataResult(); ;
            List<cPotlSupt> CollPotlSuptFrameDetails = new List<cPotlSupt>();
            int NoofSuptBeamEndCoords = 0;
            int SuptGroupNo;
            var FSO = new FileSystemObject();
            bool SuptGroupNoModFound;
            int NewFolderNo;
            int[,] arrNoofLevels = null;

            IntNoofSuptsGrouped = 0;

            mAddDatatoSuptPoints.AddDatatoSuptPoints(CollSuptPointsinArea);

            SuptGroupNo = 1;
            while (IntNoofSuptsGrouped < CollSuptPointsinArea.Count)
            {
                mWriteActivityLog.WriteActivityLog("Running of Support Frame Generation - Frame " + SuptGroupNo, DateTime.Now);

                if (mSubInitializationSupA.pubBOOLTraceOn == true)
                {
                    SuptGroupNoModFound = false;
                    NewFolderNo = SuptGroupNo;
                    while (SuptGroupNoModFound == false)
                    {
                        if (!(FSO.FolderExists(mSubInitializationSupA.pubstrFolderPath + "SupAOutput\\Frame" + NewFolderNo)))
                        {
                            FSO.CreateFolder(mSubInitializationSupA.pubstrFolderPath + "SupAOutput\\Frame" + NewFolderNo);
                            break;
                        }
                        NewFolderNo = NewFolderNo + 1;
                    }
                    mSubInitializationSupA.SuptGroupNoMod = NewFolderNo - SuptGroupNo;
                }

                CollAdjacentSuptPoints = mCreateAdjacentSuptpointsArray.CreateAdjacentSuptpointsArray(ref IntNoofSuptsGrouped, CollSuptPointsinArea, SuptGroupNo);

                CollLocalClashData = mCollectLocalClashData.CollectLocalClashData(CollAdjacentSuptPoints, CollAllClashData, SuptGroupNo);
                if (mSubInitializationSupA.pubBOOLTraceOn == true)
                    mExportColltoCSVFile<cClashData>.ExportColltoCSVFile(CollLocalClashData.ClashDataCollection, "CollLocalClashData-F" + (SuptGroupNo + mSubInitializationSupA.SuptGroupNoMod), "csv");

                mCategoriseSuptpoints.CategoriseSuptpoints(CollAdjacentSuptPoints, arrNoofLevels, SuptGroupNo);

                CollLocalExistingSteelDisc = mCollectLocalExistingDiscSteel.CollectLocalExistingDiscSteel(CollAdjacentSuptPoints, CollExistingSteelDisc);
                if (mSubInitializationSupA.pubBOOLTraceOn == true)
                    mExportColltoCSVFile<cSteelDisc>.ExportColltoCSVFile(CollLocalExistingSteelDisc, "CollLocalExistingSteelDisc-F" + (SuptGroupNo + mSubInitializationSupA.SuptGroupNoMod), "csv");

                SupAProgressBar.SupATotalRoutesExploredCount = 0;

                CollPotlSuptFrameDetails = mBuildSupportFrameWorkOptions<cSuptPoints>.BuildSupportFrameWorkOptions(CollAdjacentSuptPoints, arrNoofLevels, CollLocalClashData.ClashDataCollection, CollLocalExistingSteelDisc, NoofSuptBeamEndCoords, SuptGroupNo);

                if (mSubInitializationSupA.pubBOOLTraceOn == true)
                    mExportColltoCSVFile<cPotlSupt>.ExportColltoCSVFile(CollPotlSuptFrameDetails, "CollPotlSuptFrameDetailsCooordsNotCorrected-Beams-F" + (SuptGroupNo + mSubInitializationSupA.SuptGroupNoMod), "csv", "WriteAllBeamsinFrame");

                mCorrectSuptFrameCoords.CorrectSuptFrameCoords(CollAdjacentSuptPoints, CollLocalExistingSteelDisc, CollPotlSuptFrameDetails, arrNoofLevels);

                mWriteActivityLog.WriteActivityLog("Running of Support Frame FEA Calcs - Frame " + SuptGroupNo, DateTime.Now);

                mDefineSuptFramesSections.DefineSuptFramesSections(CollPotlSuptFrameDetails, CollAdjacentSuptPoints, SuptGroupNo);
                if (mSubInitializationSupA.pubBOOLTraceOn == true)
                    mExportColltoCSVFile<cPotlSupt>.ExportColltoCSVFile(CollPotlSuptFrameDetails, "CollPotlSuptFrameDetails-UnrationlistedBeams-F" + (SuptGroupNo + mSubInitializationSupA.SuptGroupNoMod), "csv", "WriteAllBeamsinFrame");

                mWriteActivityLog.WriteActivityLog("Running of Support Frame Detailing - Frame " + SuptGroupNo, DateTime.Now);

                mRationaliseBeamDefinitions.RationaliseBeamDefinitions(CollPotlSuptFrameDetails);
                if (mSubInitializationSupA.pubBOOLTraceOn == true)
                    mExportColltoCSVFile<cPotlSupt>.ExportColltoCSVFile(CollPotlSuptFrameDetails, "CollPotlSuptFrameDetails-RationalisdeBeams(predetailing)-F" + (SuptGroupNo + mSubInitializationSupA.SuptGroupNoMod), "csv", "WriteAllDetailedBeamsinFrame");

                mDetailSuptFrame.DetailSuptFrameFunc(CollPotlSuptFrameDetails, CollAdjacentSuptPoints, CollExistingSteel);

                mSelectBestSuptFrame.SelectBestSuptFrame(CollPotlSuptFrameDetails);

                if (mSubInitializationSupA.pubBOOLTraceOn == true)
                    mExportColltoCSVFile<cPotlSupt>.ExportColltoCSVFile(CollPotlSuptFrameDetails, "CollPotlSuptFrameDetails-PathsunT-F" + (SuptGroupNo + mSubInitializationSupA.SuptGroupNoMod), "csv", "WriteAllPathsUnTravelled");
                if (mSubInitializationSupA.pubBOOLTraceOn == true)
                    mExportColltoCSVFile<cPotlSupt>.ExportColltoCSVFile(CollPotlSuptFrameDetails, "CollPotlSuptFrameDetails-GroupNodes-F" + (SuptGroupNo + mSubInitializationSupA.SuptGroupNoMod), "csv", "WriteAllGroupNodesinFrame");
                if (mSubInitializationSupA.pubBOOLTraceOn == true)
                    mExportColltoCSVFile<cPotlSupt>.ExportColltoCSVFile(CollPotlSuptFrameDetails, "CollPotlSuptFrameDetails-Beams-F" + (SuptGroupNo + mSubInitializationSupA.SuptGroupNoMod), "csv", "WriteAllBeamsinFrame");
                mExportColltoCSVFile<cPotlSupt>.ExportColltoCSVFile(CollPotlSuptFrameDetails, "CollPotlSuptFrameDetails-DetailedBeams-F" + (SuptGroupNo + mSubInitializationSupA.SuptGroupNoMod), "csv", "WriteAllDetailedBeamsinFrame", true);
                mExportColltoCSVFile<cPotlSupt>.ExportColltoCSVFile(CollPotlSuptFrameDetails, "CollPotlSuptFrameDetails-DetailedBasePlates-F" + (SuptGroupNo + mSubInitializationSupA.SuptGroupNoMod), "csv", "WriteAllBasePlateDetailsinFrame", true);
                mExportColltoCSVFile<cPotlSupt>.ExportColltoCSVFile(CollPotlSuptFrameDetails, "CollPotlSuptFrameDetails-Summary-F" + (SuptGroupNo + mSubInitializationSupA.SuptGroupNoMod), "csv", "WriteAllFrameSummaryInfo", true);
                if (mSubInitializationSupA.pubBOOLTraceOn == true)
                    mExportColltoCSVFile<cPotlSupt>.ExportColltoCSVFile(CollPotlSuptFrameDetails, "CollPotlSuptFrameDetails-ConntoExistingSteelNodes-F" + (SuptGroupNo + mSubInitializationSupA.SuptGroupNoMod), "csv", "WriteAllConntoExistingSteelCNodesinFrame");

                SuptGroupNo = SuptGroupNo + 1;
            }

            mWriteActivityLog.WriteActivityLog("Close the log", DateTime.Now);

            if (mSubInitializationSupA.pubBOOLTraceOn == true)
                mExportColltoCSVFile<cSuptPoints>.ExportColltoCSVFile(CollSuptPointsinArea, "CollGroupedSuptPointsinArea", "csv");
            mExportColltoCSVFile<cPotlSupt>.ExportColltoCSVFile(mSubInitializationSupA.pubActivityLog, "ActivityLog", "csv");
        }
    }
}
