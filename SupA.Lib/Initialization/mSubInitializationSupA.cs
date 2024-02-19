using Microsoft.VisualBasic;
using SupA.Lib.Core;
using SupA.Lib.DataManipulation;
using System.Collections.ObjectModel;

namespace SupA.Lib.Initialization
{
    public class mSubInitializationSupA
    {
        // Public Variables that will be used in multiple software functions
        public static TTblExtendedPHASST[] pubTblExtendedPHASST;
        public static TTblDefinitionofAdjacentSupport pubTblDefinitionofAdjacentSupport;
        public static TTblDefinitionofLocalExistingSteel pubTblDefinitionofLocalExistingSteel;
        public static TTblElementTypesandHierarchies[] pubTblElementTypesandHierarchies;
        public static TTblNodeMapArrayNametoColumnMapping[] pubTblNodeMapArrayNametoColumnMapping;
        public static TTblStandardClearancesbyElementType[] pubTblStandardClearancesbyElementType;
        public static TTblPipeDimns[] pubTblPipeDimns;
        public static TTblElementLevelReportedinClashManager[] pubTblElementLevelReportedinClashManager;
        public static TTblSectionProperties[] pubTblSectionProperties;
        public static TTblMaterialProperties[] pubTblMaterialProperties;
        public static TTblBeamJusLinetoFacesDef[] pubTblBeamJusLinetoFacesDef;
        public static TTblSDNFBeamTypetoSupABeamType[] pubTblSDNFBeamTypetoSupABeamType;
        public static TTblBeamDetailing[] pubTblBeamDetailing;
        public static TTblConnDetailing[] pubTblConnDetailing;
        public static TTblBasePlateDef[] pubTblBasePlateDef;
        public static float pubIntMaxSuptBeamExtension;
        public static float pubIntDiscretisationStepSize;
        public static int pubIntDiscretisationStepDecPlaces;
        public static float pubIntDiscStepforPlateAxisTwo;
        public static bool pubBOOLUseExisting3DData;
        public static bool pubBOOLTraceOn;
        public static string pubStrSuptSelectionMode;
        public static string pubstrFolderPath;
        public static string pubAutodeskProjPath;
        public static double PiVal;
        public static string pubThreeDModelSoftware;
        public static int SuptGroupNo;
        public static object TestVar; // Variant in VB.NET is closest to object in C#
        public static float pubIntMaxSuptDeflection;
        public static float pubMatlCostperkg;
        public static float pubFabricationHours;
        public static float pubmanHourCost;
        public static int SuptGroupNoMod;
        public static CLogEntry pubLogEntry;
        public static Collection<object> pubActivityLog; // Assuming Collection is a custom type or using System.Collections.ObjectModel

        // Functions for supt point selection
        public static TTblSuptSpanRules[] pubTblSuptSpanRules;
        public static TTblSuptScoreCat[] pubTblSuptScoreCat;
        public static object[,] pubArrSuptPointEvalMatrix; // Variant array in VB.NET is closest to object[,] in C#
        public static int pubNoofCategories;

        public void SubInitilisationSupA(string StartMode = "")
        {
            string strDirFolder;
            string strRunName;
            string strProjName;
            string str3DEnv;

            // Activity Log Tracking
            WriteActivityLog("SupA Run Started. See Time Stamp for Further Information", DateTime.Now, "first entry");

            // Set all project wide and public variables
            Excel.Workbook activeWorkbook = ((Excel.Application)Excel.Application).ActiveWorkbook;
            strDirFolder = Path.GetDirectoryName(activeWorkbook.FullName) + @"\Xl\SupA.xlsm";
            strRunName = ImportFlagTxt(strDirFolder + @"\PowerShell\", "SupARunName", ".txt", ",");
            strProjName = ImportFlagTxt(strDirFolder + @"\PowerShell\", "SupAProjName", ".txt", ",");
            str3DEnv = ImportFlagTxt(strDirFolder + @"\PowerShell\", "SupA3DEnv", ".txt", ",");
            strDirFolder = strDirFolder + @"\Dirs\";
            pubstrFolderPath = ImportFlagTxt(strDirFolder, "StrDataFolder", ".csv", ",") + str3DEnv + @"\" + strProjName + @"\" + strRunName + @"\";
            pubThreeDModelSoftware = str3DEnv;
            pubBOOLTraceOn = Convert.ToBoolean(activeWorkbook.Sheets["SheetName"].Range["pubBOOLTraceOn"].Value); // Replace "SheetName" with actual sheet name
            pubStrSuptSelectionMode = !string.IsNullOrEmpty(StartMode) ? StartMode : Convert.ToString(activeWorkbook.Sheets["SheetName"].Range["pubStrSuptSelectionMode"].Value); // Replace "SheetName" with actual sheet name
            PiVal = Math.PI;
            pubMatlCostperkg = 2;
            pubFabricationHours = 3.5f;
            pubmanHourCost = 80;

            // Start Activity Log
            pubActivityLog = new Collection<object>();

            // This variable is specific to the support point selector functions
            pubNoofCategories = Convert.ToInt32(activeWorkbook.Sheets["SheetName"].Range["NoofCategories"].Value); // Replace "SheetName" with actual sheet name

            // This is temporary code
            SupAProgressBar.Show(); // Commented out as WinForms ProgressBar is not directly equivalent to VBA UserForm
            Application.DoEvents();

            // Activity Log Tracking
            WriteActivityLog("Load all Variables for SupA", DateTime.Now);

            // Now call the relevant SupA module based on the value of StrSuptSelectionMode
            switch (pubStrSuptSelectionMode)
            {
                case "StatusdefinedList":
                    Interaction.MsgBox("error"); // Call RunInStatusdefinedlistMode (future functionality requirement logged in Jira)
                    break;
                case "UserCreatedSuptPoints":
                    RunInUserCreatedSuptPointsMode();
                    break;
                case "UserDefinedPipeList":
                    RunInUserDefinedPipeListMode();
                    break;
                case "ClearPreviousRun":
                    mArchiveRun.ArchiveRun(strRunName, str3DEnv);
                    break;
                default:
                    Interaction.MsgBox("error");
                    break;
            }
        }
    }
}
