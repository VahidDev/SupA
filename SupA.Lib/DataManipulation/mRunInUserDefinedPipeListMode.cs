using SupA.Lib.Core;

namespace SupA.Lib.DataManipulation
{
    public class mRunInUserDefinedPipeListMode
    {
        // Placeholder class definitions to mirror VBA types
        public class Collection<T> : List<T> { }

        // Global variables equivalent
        private Collection<cTubeDef> CollPipeforSupporting = new Collection<cTubeDef>();
        private Collection<cSteel> CollExistingSteel = new Collection<cSteel>();
        private Collection<cSteelDisc> CollExistingSteelDisc = new Collection<cSteelDisc>();
        private Collection<cSteel> CollExistingConcrete = new Collection<cSteel>();
        private Collection<object> CollSelectedSuptLocns = new Collection<object>();
        private Collection<object> CollAllSuptPointScores = new Collection<object>();
        private Collection<object> CollAllSelectedSuptLocns = new Collection<object>();
        private Collection<object> CollPipeforSupportingInd = new Collection<object>();
        private Collection<object> CollExistingSteelforTubi = new Collection<object>();
        private cTubeDef TubiforLocalSteelColl;
        private int I;

        // Assuming these are defined and set elsewhere
        private string pubThreeDModelSoftware = "P3D";
        private string pubstrFolderPath = @"C:\Path\To\3DOutSupAIn\SuptPointSelMode\";
        private bool pubBOOLTraceOn = true;

        public void RunInUserDefinedPipeListMode()
        {
            Collection<cTubeDef> CollPipeforSupporting = new Collection<cTubeDef>();
            Collection<cSteel> CollExistingSteel = new Collection<cSteel>();
            Collection<cSteelDisc> CollExistingSteelDisc = new Collection<cSteelDisc>();
            Collection<cSteel> CollExistingConcrete = new Collection<cSteel>();
            Collection<object> CollSelectedSuptLocns = new Collection<object>();
            Collection<object> CollAllSuptPointScores = new Collection<object>();
            Collection<object> CollAllSelectedSuptLocns = new Collection<object>();
            Collection<cTubeDef> CollPipeforSupportingInd;
            Collection<cSteel> CollExistingSteelforTubi;
            cTubeDef TubiforLocalSteelColl;

            // Activity Log Tracking
            WriteActivityLog("Import and Prepare 3D Data for SupA to Run On", DateTime.Now);

            // If using P3D output then convert this to the standard import format first
            if (pubThreeDModelSoftware == "P3D")
            {
                ImportSDNFtoSupA(pubstrFolderPath + "3DOutSupAIn\\SuptPointSelMode\\", "P3D-ExistingSteelData", "SuptPointSelMode\\Area-ExistingSteelData");
                CollExistingSteel = ImportCSVFiletoColl<cSteel>(pubstrFolderPath + "3DOutSupAIn\\SuptPointSelMode\\", "Area-ExistingSteelData", ',', new cSteel());

                CollPipeforSupporting = ImportCSVFiletoColl<cTubeDef>(pubstrFolderPath + "3DOutSupAIn\\SuptPointSelMode\\", "Area-PipeData", ',', new cTubeDef());

                // Then split the Area-ExistingSteelData into a collection of csv files (one per tubi)
                for (I = 1; I <= CollPipeforSupporting.Count; I++)
                {
                    CollExistingSteelforTubi = RedEntireStruModtoLocaltoTubi(CollExistingSteel, CollPipeforSupporting[I - 1]);
                    ExportColltoCSVFile(CollExistingSteelforTubi, "SuptPointSelMode\\Area-ExistingSteelData-" + I, "csv");
                }
            }

            // Now collect data from the relevant CSV files
            // First we collect data from the tubes that need supporting because what we want to do
            // is collect existing steel and existing concrete specific to each of these tubes as this is a more efficient way of executing the code

            var PipeforSupporting = new cTubeDef();
            var CollPipeforSupporting = ImportCSVFiletoColl(pubstrFolderPath + "3DOutSupAIn\\SuptPointSelMode\\", "Area-PipeData",
            ".csv", ",", PipeforSupporting, typeof(cTubeDef));

            // Let's loop through and support one tube at a time
            for (int I = 1; I <= CollPipeforSupporting.Count; I++)
            {
                CollPipeforSupportingInd = new Collection<cTubeDef>();
                CollPipeforSupportingInd.Add(CollPipeforSupporting[I]);

                // Import the steel specific concrete definition file
                var Existingsteel = new cSteel();
                CollExistingSteel = ImportCSVFiletoColl(pubstrFolderPath + "3DOutSupAIn\\SuptPointSelMode\\", "Area-ExistingSteelData-" + I,
                ".csv", ",", Existingsteel, typeof(cSteel));

                ExportColltoCSVFile(CollAllSelectedSuptLocns, "CollSelectedSuptLocns", "csv", true);

                // Import the tubi specific concrete definition file
                var ExistingConcrete = new cSteel();
                CollExistingConcrete = ImportCSVFiletoColl(pubstrFolderPath + "3DOutSupAIn\\SuptPointSelMode\\", "Area-ExistingConcreteData-" + I,
                ".csv", ",", ExistingConcrete, typeof(cSteel));

                // Convert CollExistingSteel to CollExistingSteelDisc
                CollExistingSteel = ManipulateStrutoSupAFormat(CollExistingSteel, pubThreeDModelSoftware);
                if (pubBOOLTraceOn) ExportColltoCSVFile(CollExistingSteel, "CollExistingSteelwithFaces", "csv");

                // Merge the concrete and existing steel collections
                CollExistingSteel = MergeCollection(CollExistingSteel, CollExistingConcrete);

                CollExistingSteelDisc = DiscretiseBeamsforFrames(CollExistingSteel);
                // If pubBOOLTraceOn then ExportColltoCSVFile(CollExistingSteelDisc, "CollExistingSteelDisc", "csv");

                // The optimisation code below is no longer really required as it's baked into the input
                // RedEntireStruDiscModtoLocal(CollExistingSteelDisc, CollPipeforSupporting);
                // If pubBOOLTraceOn then ExportColltoCSVFile(CollExistingSteelDisc, "CollExistingSteelDisc", "csv");

                MasterSuptPointCreatorSub(CollPipeforSupportingInd, CollExistingSteelDisc, CollExistingSteel, CollSelectedSuptLocns, CollAllSuptPointScores, CollAllSelectedSuptLocns);

                SelectandDetailAncilliary(CollSelectedSuptLocns);

                if (I == 47)
                {
                    TestVar = TestVar;
                }
            }
        }
    }
}
