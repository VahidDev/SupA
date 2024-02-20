using SupA.Lib.Core;
using SupA.Lib.Models;

namespace SupA.Lib.DataManipulation
{
    public class mRunInUserCreatedSuptPointsMode
    {
        public void RunInUserCreatedSuptPointsMode()
        {
            // Define and set all function wide and function specific private variables
            var CollSuptPointsinArea = new List<cSuptPoints>();
            var CollExistingSteel = new List<cSteel>();
            var CollExistingConcrete = new List<cSteel>();
            var CollExistingSteelDisc = new List<cSteelDisc>();
            var CollClashData = new List<cClashData>();

            // Create and run the E3D Calling Script
            if (!pubBOOLUseExisting3DData)
            {
                // Calls to CreateSubInitilisation3DBat and Run3DDataCollectionRoutines would be implemented here
            }

            // Activity Log Tracking
            WriteActivityLog("Import and Prepare 3D Support Data for SupA to Run On", DateTime.Now);

            // If using P3D output then convert this to the standard import format first
            if (pubThreeDModelSoftware == "P3D")
            {
                ImportSDNFtoSupA(pubstrFolderPath + "P3D-ExistingSteelData", "FrameCreationMode\\Area-ExistingSteelData");
                ImportNavisClashtoSupA(pubstrFolderPath + "Navis-Clashdata", "Area-ClashData");
                // Comment about P3D-SuptPointData post-processing being unnecessary for now
            }

            // Now collect data from the relevant CSV files
            CollSuptPointsinArea = ImportCSVFiletoColl<cSuptPoints>(pubstrFolderPath + "Area-SuptPointData.csv");
            CollClashData = ImportCSVFiletoColl<cClashData>(pubstrFolderPath + "Area-ClashData.csv");
            CollExistingSteel = ImportCSVFiletoColl<cSteel>(pubstrFolderPath + "Area-ExistingSteelData.csv");
            CollExistingConcrete = ImportCSVFiletoColl<cSteel>(pubstrFolderPath + "Area-ExistingConcreteData.csv");

            // Convert CollExistingSteel to CollExistingSteelDisc
            ManipulateStrutoSupAFormat(CollExistingSteel, pubThreeDModelSoftware);
            if (pubBOOLTraceOn) ExportColltoCSVFile(CollExistingSteel, "CollExistingSteelwithFaces.csv");

            // Merge the concrete and existing steel collections
            MergeCollection(CollExistingSteel, CollExistingConcrete);

            // Then get rid of all those existing steel faces which aren't within 3500 of any support point
            RedEntireStruModtoLocal(CollExistingSteel, CollSuptPointsinArea);
            if (pubBOOLTraceOn) ExportColltoCSVFile(CollExistingSteel, "CollExistingSteelwithFacesRed.csv");

            CollExistingSteelDisc = DiscretiseBeamsforFrames(CollExistingSteel);

            // Reduce concrete points to local concrete points and append these to CollExistingSteelDisc
            if (pubBOOLTraceOn) ExportColltoCSVFile(CollExistingSteelDisc, "CollExistingSteelDisc.csv");

            // Ready to call MasterFrameCreatorSub
            MasterFrameCreatorSub(CollSuptPointsinArea, CollExistingSteelDisc, CollClashData, CollExistingSteel);
        }
    }
}
