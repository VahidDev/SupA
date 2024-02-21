using SupA.Lib.Core;
using SupA.Lib.FrameCreator;
using SupA.Lib.Initialization;
using SupA.Lib.Models;

namespace SupA.Lib.DataManipulation
{
    public class mRunInUserCreatedSuptPointsMode
    {
        public static void RunInUserCreatedSuptPointsMode()
        {
            // Define and set all function wide and function specific private variables
            cTubeDef CollSuptPointsinArea ;
            var CollExistingSteel = new List<cSteel>();
            var CollExistingConcrete = new List<cSteel>();
            var CollExistingSteelDisc = new List<cSteelDisc>();
            var CollClashData = new List<cClashData>();

            // Create and run the E3D Calling Script
            if (!mSubInitializationSupA.pubBOOLUseExisting3DData)
            {
                // Calls to CreateSubInitilisation3DBat and Run3DDataCollectionRoutines would be implemented here
            }

            // Activity Log Tracking
            mWriteActivityLog.WriteActivityLog("Import and Prepare 3D Support Data for SupA to Run On", DateTime.Now);

            // If using P3D output then convert this to the standard import format first
            if (mSubInitializationSupA.pubThreeDModelSoftware == "P3D")
            {
                ImportSDNFtoSupA(mSubInitializationSupA.pubstrFolderPath + "P3D-ExistingSteelData", "FrameCreationMode\\Area-ExistingSteelData");
                ImportNavisClashtoSupA(mSubInitializationSupA.pubstrFolderPath + "Navis-Clashdata", "Area-ClashData");
                // Comment about P3D-SuptPointData post-processing being unnecessary for now
            }

            // Now collect data from the relevant CSV files
            CollSuptPointsinArea = ImportCSVFiletoColl<cSuptPoints>(mSubInitializationSupA.pubstrFolderPath + "Area-SuptPointData.csv");
            CollClashData = ImportCSVFiletoColl<cClashData>(mSubInitializationSupA.pubstrFolderPath + "Area-ClashData.csv");
            CollExistingSteel = ImportCSVFiletoColl<cSteel>(mSubInitializationSupA.pubstrFolderPath + "Area-ExistingSteelData.csv");
            CollExistingConcrete = ImportCSVFiletoColl<cSteel>(mSubInitializationSupA.pubstrFolderPath + "Area-ExistingConcreteData.csv");

            // Convert CollExistingSteel to CollExistingSteelDisc
            ManipulateStrutoSupAFormat(CollExistingSteel, mSubInitializationSupA.pubThreeDModelSoftware);
            if (mSubInitializationSupA.pubBOOLTraceOn) mExportColltoCSVFile<cSteel>.ExportColltoCSVFile(CollExistingSteel, "CollExistingSteelwithFaces", "csv");

            // Merge the concrete and existing steel collections
            mBuildSupportFrameWorkOptions<cSteel>.MergeCollection<cSteel, cSteel, object, object, object, object>(CollExistingSteel, CollExistingConcrete);

            // Then get rid of all those existing steel faces which aren't within 3500 of any support point
            mRedEntireStruModtoLocaltoTubi.RedEntireStruModtoLocaltoTubi(CollExistingSteel, CollSuptPointsinArea);
            if (mSubInitializationSupA.pubBOOLTraceOn) mExportColltoCSVFile<cSteel>.ExportColltoCSVFile(CollExistingSteel, "CollExistingSteelwithFacesRed", "csv");

            CollExistingSteelDisc = DiscretiseBeamsforFrames(CollExistingSteel);

            // Reduce concrete points to local concrete points and append these to CollExistingSteelDisc
            if (mSubInitializationSupA.pubBOOLTraceOn) mExportColltoCSVFile<cSteelDisc>.ExportColltoCSVFile(CollExistingSteelDisc, "CollExistingSteelDisc", "csv");

            // Ready to call MasterFrameCreatorSub
            MasterFrameCreatorSub(CollSuptPointsinArea, CollExistingSteelDisc, CollClashData, CollExistingSteel);
        }
    }
}
