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
            // Define and Set all function wide and function specific private variables.
            // These are CollSuptPointsinArea , CollExistingSteelDisc , CollAllClashData
            List<cSuptPoints> CollSuptPointsinArea = new List<cSuptPoints>();
            cSuptPoints SuptPointsinArea = new cSuptPoints();

            List<cSteel> CollExistingSteel = new List<cSteel>();
            cSteel Existingsteel = new cSteel();

            List<cSteel> CollExistingConcrete = new List<cSteel>();
            cSteel ExistingConcrete = new cSteel();

            List<cSteelDisc> CollExistingSteelDisc = new List<cSteelDisc>();
            cSteelDisc ExistingSteelDisc = new cSteelDisc();

            List<cClashData> CollClashData = new List<cClashData>();
            cClashData ClashData = new cClashData();

            object[] ArrFileContentsTmp;

            // Create and run the E3D Calling Script
            if (!mSubInitializationSupA.pubBOOLUseExisting3DData)
            {
                // Call CreateSubInitilisation3DBat; 
                // the way this script runs depends on the value of StrSuptSelectionMode
                // Call Run3DDataCollectionRoutines;
                // This also holds the VBA script until the return of 3D data here 
                // Call Shell(shellCommand, vbNormalFocu);
            }

            // Activity Log Tracking
            mWriteActivityLog.WriteActivityLog("Import and Prepare 3D Support Data for SupA to Run On", DateTime.Now());

            // If using P3D output then convert this to the standard import format first
            if (mSubInitializationSupA.pubThreeDModelSoftware == "P3D")
            {
                mImportSDNFtoSupAFormat.ImportSDNFtoSupA(Path.Combine(mSubInitializationSupA.pubstrFolderPath, "3DOutSupAIn", "FrameCreationMode"), "P3D-ExistingSteelData", "FrameCreationMode\\Area-ExistingSteelData");
                mImportNavisClashtoSupA.ImportNavisClashtoSupA(Path.Combine(mSubInitializationSupA.pubstrFolderPath, "3DOutSupAIn", "FrameCreationMode"), "Navis-Clashdata", "Area-ClashData");
                // This line is not required for now as the supt point format from P3D 
                // is already in the format required by SupA and no
                // post-processing is required
                // ImportP3DSuptListtoSupA(Path.Combine(pubstrFolderPath, "3DOutSupAIn", "FrameCreationMode"), "P3D-SuptPointData", "Area-SuptPointData");
            }

            // Now collect data from the relevant CSV files
            SuptPointsinArea = new cSuptPoints();
            CollSuptPointsinArea = mImportCSVFiletoColl.ImportCSVFiletoColl(Path.Combine(mSubInitializationSupA.pubstrFolderPath, "3DOutSupAIn", "FrameCreationMode"), "Area-SuptPointData", ".csv", ",", SuptPointsinArea, "cSuptPoints");

            ClashData = new cClashData();
            CollClashData = mImportCSVFiletoColl.ImportCSVFiletoColl(Path.Combine(mSubInitializationSupA.pubstrFolderPath, "3DOutSupAIn", "FrameCreationMode"), "Area-ClashData", ".csv", ",", ClashData, "cClashData");

            Existingsteel = new cSteel();
            CollExistingSteel = mImportCSVFiletoColl.ImportCSVFiletoColl(Path.Combine(mSubInitializationSupA.pubstrFolderPath, "3DOutSupAIn", "FrameCreationMode"), "Area-ExistingSteelData", ".csv", ",", Existingsteel, "cSteel");

            ExistingConcrete = new cSteel();
            CollExistingConcrete = mImportCSVFiletoColl.ImportCSVFiletoColl(Path.Combine(mSubInitializationSupA.pubstrFolderPath, "3DOutSupAIn", "FrameCreationMode"), "Area-ExistingConcreteData", ".csv", ",", Existingsteel, "cSteel");

            // Convert CollExistingSteel to CollExistingSteelDisc
            CollExistingSteel = mManipulateStrutoSupAFormat.ManipulateStrutoSupAFormat(CollExistingSteel, mSubInitializationSupA.pubThreeDModelSoftware);
            if (mSubInitializationSupA.pubBOOLTraceOn)
            {
                mExportColltoCSVFile<cSteel>.ExportColltoCSVFile(CollExistingSteel, "CollExistingSteelwithFaces", "csv");
            }

            // Merge the concrete and existing steel collections
            CollExistingSteel = mBuildSupportFrameWorkOptions<cSteel>.MergeCollection<cSteel, cSteel, object, object, object, object>(CollExistingSteel, CollExistingConcrete);

            // Then get rid of all of those existing steel faces which aren't within 3500 of any support point
            mRedEntireStruModtoLocal.RedEntireStruModtoLocal(CollExistingSteel, CollSuptPointsinArea);
            if (mSubInitializationSupA.pubBOOLTraceOn)
            {
                mExportColltoCSVFile<cSteel>.ExportColltoCSVFile(CollExistingSteel, "CollExistingSteelwithFacesRed", "csv");
            }

            CollExistingSteelDisc = mDiscretiseBeamsforFrames.DiscretiseBeamsforFrames(CollExistingSteel);

            // Reduce concrete points to local concrete points.
            // Append these to CollExistingSteelDisc
            if (mSubInitializationSupA.pubBOOLTraceOn)
            {
                mExportColltoCSVFile<cSteelDisc>.ExportColltoCSVFile(CollExistingSteelDisc, "CollExistingSteelDisc", "csv");
            }
            // I am now ready to call MasterFrameCreatorSub
            mMasterFrameCreatorSub.MasterFrameCreatorSub(CollSuptPointsinArea, CollExistingSteelDisc, CollClashData, CollExistingSteel);
        }
    }
}
