﻿using SupA.Lib.Core;
using SupA.Lib.Initialization;

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

        public void RunInUserDefinedPipeListMode()
        {
            List<cTubeDef> CollPipeforSupporting = new List<cTubeDef>();
            List<cSteel> CollExistingSteel = new List<cSteel>();
            List<cSteelDisc> CollExistingSteelDisc = new List<cSteelDisc>();
            List<cSteel> CollExistingConcrete = new List<cSteel>();
            List<object> CollSelectedSuptLocns = new List<object>();
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
                ImportSDNFtoSupA(mSubInitializationSupA.pubstrFolderPath + "3DOutSupAIn\\SuptPointSelMode\\", "P3D-ExistingSteelData", "SuptPointSelMode\\Area-ExistingSteelData");
                CollExistingSteel = ImportCSVFiletoColl<cSteel>(mSubInitializationSupA.pubstrFolderPath + "3DOutSupAIn\\SuptPointSelMode\\", "Area-ExistingSteelData", ',', new cSteel());

                CollPipeforSupporting = ImportCSVFiletoColl<cTubeDef>(mSubInitializationSupA.pubstrFolderPath + "3DOutSupAIn\\SuptPointSelMode\\", "Area-PipeData", ',', new cTubeDef());

                // Then split the Area-ExistingSteelData into a collection of csv files (one per tubi)
                for (I = 1; I <= CollPipeforSupporting.Count; I++)
                {
                    CollExistingSteelforTubi = mRedEntireStruModtoLocaltoTubi.RedEntireStruModtoLocaltoTubi(CollExistingSteel, CollPipeforSupporting[I - 1]);
                    mExportColltoCSVFile<cSteel>.ExportColltoCSVFile(CollExistingSteelforTubi, "SuptPointSelMode\\Area-ExistingSteelData-" + I, "csv");
                }
            }

            // Now collect data from the relevant CSV files
            // First we collect data from the tubes that need supporting because what we want to do
            // is collect existing steel and existing concrete specific to each of these tubes as this is a more efficient way of executing the code

            var PipeforSupporting = new cTubeDef();
            CollPipeforSupporting = ImportCSVFiletoColl(mSubInitializationSupA.pubstrFolderPath + "3DOutSupAIn\\SuptPointSelMode\\", "Area-PipeData",
            ".csv", ",", PipeforSupporting, typeof(cTubeDef));

            // Let's loop through and support one tube at a time
            for (int I = 1; I <= CollPipeforSupporting.Count; I++)
            {
                CollPipeforSupportingInd = new Collection<cTubeDef>();
                CollPipeforSupportingInd.Add(CollPipeforSupporting[I]);

                // Import the steel specific concrete definition file
                var Existingsteel = new cSteel();
                CollExistingSteel = ImportCSVFiletoColl(mSubInitializationSupA.pubstrFolderPath + "3DOutSupAIn\\SuptPointSelMode\\", "Area-ExistingSteelData-" + I,
                ".csv", ",", Existingsteel, typeof(cSteel));

                mExportColltoCSVFile<object>.ExportColltoCSVFile(CollAllSelectedSuptLocns, "CollSelectedSuptLocns", "csv", true);

                // Import the tubi specific concrete definition file
                var ExistingConcrete = new cSteel();
                CollExistingConcrete = ImportCSVFiletoColl(mSubInitializationSupA.pubstrFolderPath + "3DOutSupAIn\\SuptPointSelMode\\", "Area-ExistingConcreteData-" + I,
                ".csv", ",", ExistingConcrete, typeof(cSteel));

                // Convert CollExistingSteel to CollExistingSteelDisc
                CollExistingSteel = ManipulateStrutoSupAFormat(CollExistingSteel, mSubInitializationSupA.pubThreeDModelSoftware);
                if (mSubInitializationSupA.pubBOOLTraceOn)
                {
                    mExportColltoCSVFile<cSteel>.ExportColltoCSVFile(CollExistingSteel, "CollExistingSteelwithFaces", "csv");
                }

                // Merge the concrete and existing steel collections
                CollExistingSteel = MergeCollection(CollExistingSteel, CollExistingConcrete);

                CollExistingSteelDisc = DiscretiseBeamsforFrames(CollExistingSteel);
                // If pubBOOLTraceOn then ExportColltoCSVFile(CollExistingSteelDisc, "CollExistingSteelDisc", "csv");

                // The optimisation code below is no longer really required as it's baked into the input
                // RedEntireStruDiscModtoLocal(CollExistingSteelDisc, CollPipeforSupporting);
                // If pubBOOLTraceOn then ExportColltoCSVFile(CollExistingSteelDisc, "CollExistingSteelDisc", "csv");

                MasterSuptPointCreatorSub(CollPipeforSupportingInd, CollExistingSteelDisc, CollExistingSteel, CollSelectedSuptLocns, CollAllSuptPointScores, CollAllSelectedSuptLocns);

                SelectandDetailAncilliary(CollSelectedSuptLocns);
            }
        }
    }
}
