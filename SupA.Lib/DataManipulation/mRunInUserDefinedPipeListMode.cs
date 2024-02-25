using SupA.Lib.Core;
using SupA.Lib.FrameCreator;
using SupA.Lib.Initialization;

namespace SupA.Lib.DataManipulation
{
    public class mRunInUserDefinedPipeListMode
    {
        // Global variables equivalent
        private List<cTubeDef> CollPipeforSupporting = new List<cTubeDef>();
        private List<cSteel> CollExistingSteel = new List<cSteel>();
        private List<cSteelDisc> CollExistingSteelDisc = new List<cSteelDisc>();
        private List<cSteel> CollExistingConcrete = new List<cSteel>();
        private List<object> CollSelectedSuptLocns = new List<object>();
        private List<object> CollAllSuptPointScores = new List<object>();
        private List<object> CollAllSelectedSuptLocns = new List<object>();
        private List<object> CollPipeforSupportingInd = new List<object>();
        private List<object> CollExistingSteelforTubi = new List<object>();
        private cTubeDef TubiforLocalSteelColl;

        public static void RunInUserDefinedPipeListMode()
        {
            List<cTubeDef> CollPipeforSupporting = new List<cTubeDef>();
            List<cSteel> CollExistingSteel = new List<cSteel>();
            List<cSteelDisc> CollExistingSteelDisc = new List<cSteelDisc>();
            List<cSteel> CollExistingConcrete = new List<cSteel>();
            List<cTubeDefDisc> CollSelectedSuptLocns = new List<cTubeDefDisc>();
            List<object> CollAllSuptPointScores = new List<object>();
            List<object> CollAllSelectedSuptLocns = new List<object>();
            List<cTubeDef> CollPipeforSupportingInd;
            List<cSteel> CollExistingSteelforTubi;
            cTubeDef TubiforLocalSteelColl;

            // Activity Log Tracking
            mWriteActivityLog.WriteActivityLog("Import and Prepare 3D Data for SupA to Run On", DateTime.Now);

            // If using P3D output then convert this to the standard import format first
            if (mSubInitializationSupA.pubThreeDModelSoftware == "P3D")
            {
                mImportSDNFtoSupAFormat.ImportSDNFtoSupA(mSubInitializationSupA.pubstrFolderPath + "3DOutSupAIn\\SuptPointSelMode\\", "P3D-ExistingSteelData", "SuptPointSelMode\\Area-ExistingSteelData");
                CollExistingSteel = mImportCSVFiletoColl.ImportCSVFiletoColl<cSteel>(mSubInitializationSupA.pubstrFolderPath + "3DOutSupAIn\\SuptPointSelMode\\", "Area-ExistingSteelData", ',', new cSteel());

                CollPipeforSupporting = mImportCSVFiletoColl.ImportCSVFiletoColl<cTubeDef>(mSubInitializationSupA.pubstrFolderPath + "3DOutSupAIn\\SuptPointSelMode\\", "Area-PipeData", ',', new cTubeDef());

                // Then split the Area-ExistingSteelData into a collection of csv files (one per tubi)
                for (var I = 1; I <= CollPipeforSupporting.Count; I++)
                {
                    CollExistingSteelforTubi = mRedEntireStruModtoLocaltoTubi.RedEntireStruModtoLocaltoTubi(CollExistingSteel, CollPipeforSupporting[I - 1]);
                    mExportColltoCSVFile<cSteel>.ExportColltoCSVFile(CollExistingSteelforTubi, "SuptPointSelMode\\Area-ExistingSteelData-" + I, "csv");
                }
            }

            // Now collect data from the relevant CSV files
            // First we collect data from the tubes that need supporting because what we want to do
            // is collect existing steel and existing concrete specific to each of these tubes as this is a more efficient way of executing the code

            var PipeforSupporting = new cTubeDef();
            CollPipeforSupporting = mImportCSVFiletoColl.ImportCSVFiletoColl(mSubInitializationSupA.pubstrFolderPath + "3DOutSupAIn\\SuptPointSelMode\\", "Area-PipeData",
            ".csv", ",", PipeforSupporting, nameof(cTubeDef));

            // Let's loop through and support one tube at a time
            for (int I = 1; I <= CollPipeforSupporting.Count; I++)
            {
                CollPipeforSupportingInd = new List<cTubeDef>
                {
                    CollPipeforSupporting[I]
                };

                // Import the steel specific concrete definition file
                var Existingsteel = new cSteel();
                CollExistingSteel = mImportCSVFiletoColl.ImportCSVFiletoColl(mSubInitializationSupA.pubstrFolderPath + "3DOutSupAIn\\SuptPointSelMode\\", "Area-ExistingSteelData-" + I,
                ".csv", ",", Existingsteel, nameof(cSteel));

                mExportColltoCSVFile<object>.ExportColltoCSVFile(CollAllSelectedSuptLocns, "CollSelectedSuptLocns", "csv", StdSupAOutput: true);

                // Import the tubi specific concrete definition file
                var ExistingConcrete = new cSteel();
                CollExistingConcrete = mImportCSVFiletoColl.ImportCSVFiletoColl(mSubInitializationSupA.pubstrFolderPath + "3DOutSupAIn\\SuptPointSelMode\\", "Area-ExistingConcreteData-" + I,
                ".csv", ",", ExistingConcrete, nameof(cSteel));

                // Convert CollExistingSteel to CollExistingSteelDisc
                CollExistingSteel = mManipulateStrutoSupAFormat.ManipulateStrutoSupAFormat(CollExistingSteel, mSubInitializationSupA.pubThreeDModelSoftware);
                if (mSubInitializationSupA.pubBOOLTraceOn)
                {
                    mExportColltoCSVFile<cSteel>.ExportColltoCSVFile(CollExistingSteel, "CollExistingSteelwithFaces", "csv");
                }

                // Merge the concrete and existing steel collections
                CollExistingSteel = mBuildSupportFrameWorkOptions<cSteel>.MergeCollection<cSteel, cSteel, object, object, object, object>(CollExistingSteel, CollExistingConcrete);

                CollExistingSteelDisc = mDiscretiseBeamsforFrames.DiscretiseBeamsforFrames(CollExistingSteel);
                // If pubBOOLTraceOn then ExportColltoCSVFile(CollExistingSteelDisc, "CollExistingSteelDisc", "csv");

                // The optimisation code below is no longer really required as it's baked into the input
                // RedEntireStruDiscModtoLocal(CollExistingSteelDisc, CollPipeforSupporting);
                // If pubBOOLTraceOn then ExportColltoCSVFile(CollExistingSteelDisc, "CollExistingSteelDisc", "csv");

                mMasterSuptPointCreatorSub.MasterSuptPointCreatorSub(CollPipeforSupportingInd, CollExistingSteelDisc, CollExistingSteel, CollSelectedSuptLocns, CollAllSuptPointScores, CollAllSelectedSuptLocns);

                mSelectandDetailAncilliary.SelectandDetailAncilliary(CollSelectedSuptLocns);
            }
        }
    }
}
