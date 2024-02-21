using Microsoft.VisualBasic;
using Microsoft.Office.Interop.Excel;
using SupA.Lib.Initialization;
using SupA.Lib.Core;
using SupA.Lib.DataManipulation;

namespace SupA.Lib.FEA
{
    public class mDefineSuptFramesSections
    {
        public static TTblSectionProperties[] TblPreferedSctnsProps;
        public static TTblSectionProperties[] TblLightestSctnProp;
        public static TTblSectionProperties[] pubTblSectionProperties;
        public static bool pubBOOLTraceOn;
        public static int pubIntMaxSuptDeflection;
        public static string pubstrFolderPath;

        public static void Init()
        {
            // Initialize necessary variables and collections
            List<cPotlSupt> CollPotlSuptFrameDetails = new List<cPotlSupt>();
            var CollAdjacentSuptPoints = new List<cSuptPoints>();
            int SuptGroupNo = 0;

            // Call the method to define support frame sections
            DefineSuptFramesSections(CollPotlSuptFrameDetails, CollAdjacentSuptPoints, SuptGroupNo);

            // Iterate over CollPotlSuptFrameDetails and export FEM results for each frame
            foreach (cPotlSupt frame in CollPotlSuptFrameDetails)
            {
                ExportFEMresults(frame, SuptGroupNo);
            }
        }

        static void DefineSuptFramesSections(List<cPotlSupt> collPotlSuptFrameDetails, List<cSuptPoints> collAdjacentSuptPoints, int suptGroupNo)
        {
            cPotlSupt frame;
            int frameC = 0;
            int noofBeamGroups = 0;
            string stdFile;

            // Define your file paths
            string pubstrFolderPath = @"C:\Your\Public\Folder\Path\"; // Update with your actual folder path
            string supOutputFolder = Path.Combine(pubstrFolderPath, "SupAOutput");

            Directory.CreateDirectory(Path.Combine(supOutputFolder, $"Frame{suptGroupNo + mSubInitializationSupA.SuptGroupNoMod}", "STAAD"));

            // Loop through each frame
            while (frameC < collPotlSuptFrameDetails.Count)
            {
                frame = collPotlSuptFrameDetails[frameC];
                List<TTblSectionProperties> collAcceptableSctnsProps = new List<TTblSectionProperties>();
                bool beamAvailableforLoads = false;

                // Create the FEM Model Geometry
                mCreateFEModelGeometry.CreateFEModelGeometry(frame, collAdjacentSuptPoints, out noofBeamGroups, beamAvailableforLoads, suptGroupNo, out stdFile);

                // Open and analyze the STAAD file
                using (var objOpenSTAAD = new OpenSTAAD())
                {
                    objOpenSTAAD.OpenSTAADFile(stdFile);

                    // Wait until the results are available
                    while (!File.Exists(Path.ChangeExtension(stdFile, ".png")) && !File.Exists(Path.ChangeExtension(stdFile, ".emf")))
                    {
                        Thread.Sleep(1000); // Wait for 1 second
                    }

                    objOpenSTAAD.GetSTAADFile(stdFile, true);

                    // Run FEA analysis within STAAD
                    int retVal = objOpenSTAAD.AnalyzeEx(1, 0, 1);

                    // Check if results are available
                    if (objOpenSTAAD.Output.AreResultsAvailable == 0)
                    {
                        Thread.Sleep(4000); // Wait for 4 seconds
                        continue; // Retry analysis
                    }

                    // Analyze results for each beam in the frame
                    foreach (var beam in frame.BeamsinFrame)
                    {
                        AnalyseFEAResults(objOpenSTAAD, beam, collAcceptableSctnsProps);
                    }

                    // If no sections were acceptable, remove the frame from the collection
                    if (collAcceptableSctnsProps.Count == 0)
                    {
                        collPotlSuptFrameDetails.RemoveAt(frameC);
                        frameC--;
                    }
                    else
                    {
                        // Select the best potential support
                        mSelectBestPotlSupt.SelectBestPotlSupt(collAcceptableSctnsProps, frame);

                        // Create STAAD input file with selected section
                        mCreateFEModelGeometry.CreateFEModelGeometry(frame, collAdjacentSuptPoints, out noofBeamGroups, beamAvailableforLoads, suptGroupNo, out stdFile);
                    }
                }

                frameC++;
            }

            // Export frames that didn't pass
            mExportColltoCSVFile<cPotlSupt>.ExportColltoCSVFile(collPotlSuptFrameDetails, $"CollFEAFailPotlSuptFrameDetails-Beams-F{suptGroupNo + mSubInitializationSupA.SuptGroupNoMod}.csv", "WriteAllBeamsinFrame");
        }

        public static void ExportFEMresults(cPotlSupt Frame, int SuptGroupNo)
        {
            object[,] TmpExportArr;
            int RangeRowEnd;
            
            Application excelApp = new Application();
            Workbook workbook = excelApp.Workbooks.Open("YourWorkbookPath.xlsx");
            Worksheet outputWorksheet = (Worksheet)workbook.Worksheets["Output"];

            // Export beam property types
            RangeRowEnd = FindLastFullCell(outputWorksheet, 11, 6);
            TmpExportArr = (object[,])Interaction.CallByName(outputWorksheet.Range["E11:K" + RangeRowEnd], "value", CallType.Get);
            mExportArrtoCSVFile.ExportArrtoCSVFile(TmpExportArr, "Frame" + (SuptGroupNo + mSubInitializationSupA.SuptGroupNoMod) + "\\EndofBeamOutput-" + Frame.PotlSuptNo, "csv", true);

            // Export node coordinates
            RangeRowEnd = FindLastFullCell(outputWorksheet, 10, 14);
            TmpExportArr = (object[,])Interaction.CallByName(outputWorksheet.Range["N10:AA" + RangeRowEnd], "value", CallType.Get);
            mExportArrtoCSVFile.ExportArrtoCSVFile(TmpExportArr, "Frame" + (SuptGroupNo + mSubInitializationSupA.SuptGroupNoMod) + "\\AlongBeamOutput-" + Frame.PotlSuptNo, "csv", true);
        }

        public static int FindLastFullCell(Worksheet sheet, int RowNo, int ColNo)
        {
            Microsoft.Office.Interop.Excel.Range cell;

            do
            {
                RowNo++;
                cell = sheet.Cells[RowNo, ColNo] as Microsoft.Office.Interop.Excel.Range;
            } while (cell.Value != null && !string.IsNullOrEmpty(cell.Value.ToString()));

            return RowNo - 1;
        }

        public static double[,] ExportMaxFEAResultsperBeam(cPotlSupt Frame, int NoofBeamGroups, TTblSectionProperties PreferedSctnProp)
        {
            Worksheet sheet = Globals.ThisAddIn.Application.ActiveWorkbook.Sheets["Output"];
            sheet.Activate();

            double[,] ArrtoRtn = new double[NoofBeamGroups, 6];

            for (int I = 0; I < NoofBeamGroups; I++)
            {
                int RowNo = 10;
                while (sheet.Cells[RowNo, 18] != null && sheet.Cells[RowNo, 18].ToString() != "")
                {
                    if (I + 1 == (int)sheet.Cells[RowNo, 14])
                    {
                        // max stress in major axis
                        ArrtoRtn[I, 0] = Math.Max(ArrtoRtn[I, 0], Math.Abs((double)sheet.Cells[RowNo, 19] * (PreferedSctnProp.Depth / 2000) / (PreferedSctnProp.MajAxisSecondMofA / 1000000000000)));

                        // max stress in minor axis
                        ArrtoRtn[I, 1] = Math.Max(ArrtoRtn[I, 1], Math.Abs((double)sheet.Cells[RowNo, 19] * (PreferedSctnProp.Width / 2000) / (PreferedSctnProp.MinAxisSecondMofA / 1000000000000)));

                        // x defl
                        ArrtoRtn[I, 2] = Math.Max(ArrtoRtn[I, 2], Math.Abs((double)sheet.Cells[RowNo, 22]));

                        // y defl
                        ArrtoRtn[I, 3] = Math.Max(ArrtoRtn[I, 3], Math.Abs((double)sheet.Cells[RowNo, 23]));

                        // Z defl
                        ArrtoRtn[I, 4] = Math.Max(ArrtoRtn[I, 4], Math.Abs((double)sheet.Cells[RowNo, 24]));

                        // unity check
                        double UnityCheckVal = Math.Abs((double)sheet.Cells[RowNo, 18] / (PreferedSctnProp.Area / 1000)) / 165000000 +
                                                Math.Abs((double)sheet.Cells[RowNo, 19] * (PreferedSctnProp.Depth / 2000) / (PreferedSctnProp.MajAxisSecondMofA / 1000000000000)) / 165000000 +
                                                Math.Abs((double)sheet.Cells[RowNo, 20] * (PreferedSctnProp.Width / 2000) / (PreferedSctnProp.MinAxisSecondMofA / 1000000000000)) / 165000000;

                        ArrtoRtn[I, 5] = Math.Max(ArrtoRtn[I, 5], UnityCheckVal);
                    }

                    RowNo++;
                }
            }

            return ArrtoRtn;
        }

        public static void AnalyseFEAResults(object objOpenSTAAD, cSteel Beam, int SctnTblC, int BeamsinFrmC, TTblSectionProperties PreferedSctnProp, ref bool UnityCheckAcceptable, ref bool DeflectionAcceptable)
        {
            long varReturnVal;
            string designCode;
            string designStatus;
            double criticalRatio;
            double allowableRatio;
            long criticalLoadCase;
            double criticalSection;
            string criticalClause;
            string designSection;
            double[] designForces = new double[3];
            double KLByR;
            long memberNo;

            memberNo = SctnTblC * 10 + BeamsinFrmC;

            varReturnVal = objOpenSTAAD.Output.GetMemberSteelDesignResults(memberNo, out designCode, out designStatus, out criticalRatio, out allowableRatio, out criticalLoadCase, out criticalSection, out criticalClause, out designSection, out designForces, out KLByR);

            if (designStatus == "FAIL")
            {
                UnityCheckAcceptable = false;
            }

            long memb;
            long Lcase;
            object RetVal;
            string strDir;
            double DMax;
            double dMaxPos;

            for (int I = 1; I <= 3; I++)
            {
                Lcase = 1;
                if (I == 1)
                {
                    strDir = "X";
                }
                else if (I == 2)
                {
                    strDir = "Y";
                }
                else
                {
                    strDir = "Z";
                }
                memb = memberNo;
                RetVal = objOpenSTAAD.Output.GetMaxSectionDisplacement(memb, strDir, Lcase, out DMax, out dMaxPos);
                // Convert displacement from meters to millimeters
                DMax *= 1000;

                if (Math.Abs(DMax) >= pubIntMaxSuptDeflection)
                {
                    DeflectionAcceptable = false;
                }
                else
                {
                    // Deflection is within limits
                    Interaction.MsgBox("DEFLECTION OK");
                }
            }
        }

        public static void AssignSuptFrameSectionSizes(cPotlSupt Frame, int NoofBeamGroups, TTblSectionProperties PreferedSctnProp)
        {
            int GroupC;
            int RowNo;

            // Clear contents of the specified range
            ClearContents("a7:j70");

            RowNo = 7;
            for (GroupC = 1; GroupC <= NoofBeamGroups; GroupC++)
            {
                // Assign values to cells in the specified range
                SetCellValue(RowNo + GroupC - 1, 1, PreferedSctnProp.ProfileName);
                SetCellValue(RowNo + GroupC - 1, 2, NoofBeamGroups);
                SetCellValue(RowNo + GroupC - 1, 3, PreferedSctnProp.Area / 1000000);
                SetCellValue(RowNo + GroupC - 1, 4, PreferedSctnProp.MajAxisSecondMofA / 1000000000000.0);
                SetCellValue(RowNo + GroupC - 1, 5, PreferedSctnProp.MinAxisSecondMofA / 1000000000000.0);
                SetCellValue(RowNo + GroupC - 1, 6, PreferedSctnProp.TorsionConstant / 1000000000.0);
                SetCellValue(RowNo + GroupC - 1, 8, PreferedSctnProp.MinAxisShearArea / 1000000);
                SetCellValue(RowNo + GroupC - 1, 9, PreferedSctnProp.MajAxisShearArea / 1000000);
                SetCellValue(RowNo + GroupC - 1, 7, mSubInitializationSupA.pubTblMaterialProperties[0].YoungsMod);
                SetCellValue(RowNo + GroupC - 1, 10, mSubInitializationSupA.pubTblMaterialProperties[0].ShearMod);
            }
        }

        static void ClearContents(string range)
        {
            // Implement logic to clear contents of the specified range
        }

        static void SetCellValue(int row, int column, object value)
        {
            // Implement logic to set value in the specified cell
        }

        public static void CreateTblPreferedSctnsProps()
        {
            int TblLen = 0;
            for (int LL1c = 0; LL1c < pubTblSectionProperties.GetLength(0); LL1c++)
            {
                if (pubTblSectionProperties[LL1c].PreferredSctnFlag)
                {
                    TblLen++;
                    if (pubTblSectionProperties[LL1c].MajAxisSecondMofA != pubTblSectionProperties[LL1c].MinAxisSecondMofA)
                    {
                        TblLen++;
                    }
                }
            }

            // Now populate TblPreferedSctnsProps
            TTblSectionProperties[] TblPreferedSctnsProps = new TTblSectionProperties[TblLen];
            TblLen = 0;
            for (int LL1c = 0; LL1c < pubTblSectionProperties.GetLength(0); LL1c++)
            {
                if (pubTblSectionProperties[LL1c].PreferredSctnFlag)
                {
                    TblLen++;
                    TblPreferedSctnsProps[TblLen - 1] = pubTblSectionProperties[LL1c];
                    if (pubTblSectionProperties[LL1c].MajAxisSecondMofA != pubTblSectionProperties[LL1c].MinAxisSecondMofA)
                    {
                        TblLen++;
                        TblPreferedSctnsProps[TblLen - 1] = pubTblSectionProperties[LL1c];
                        TblPreferedSctnsProps[TblLen - 1].MajAxisSecondMofA = pubTblSectionProperties[LL1c].MinAxisSecondMofA;
                        TblPreferedSctnsProps[TblLen - 1].MinAxisSecondMofA = pubTblSectionProperties[LL1c].MajAxisSecondMofA;
                        TblPreferedSctnsProps[TblLen - 1].MajAxisShearArea = pubTblSectionProperties[LL1c].MinAxisShearArea;
                        TblPreferedSctnsProps[TblLen - 1].MinAxisShearArea = pubTblSectionProperties[LL1c].MajAxisShearArea;
                        TblPreferedSctnsProps[TblLen - 1].Width = pubTblSectionProperties[LL1c].Width;
                        TblPreferedSctnsProps[TblLen - 1].Depth = pubTblSectionProperties[LL1c].Depth;
                        TblPreferedSctnsProps[TblLen - 1].Rotation = 90;
                        TblPreferedSctnsProps[TblLen - 1].STAADSctnname = pubTblSectionProperties[LL1c].STAADSctnname;
                    }
                }
            }

            // Sorting the array (not implemented)
            // SortComplete = QSortSctns();
        }
    }

    public class TTblSectionProperties
    {
        public string ProfileStandard;
        public string ProfileName;
        public string ProfileFamily;
        public string ProfileThreeDModelNm;
        public double Area;
        public double MajAxisSecondMofA;
        public double MinAxisSecondMofA;
        public double TorsionConstant;
        public double MinAxisShearArea;
        public double MajAxisShearArea;
        public double YoungsMod;
        public double ShearMod;
        public bool PreferredSctnFlag;
        public string STAADSctnname;
        public double Width;
        public double Depth;
        public double Weight;
        public double Rotation;
    }
}
