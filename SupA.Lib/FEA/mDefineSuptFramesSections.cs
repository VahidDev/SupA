using Microsoft.VisualBasic;
using Microsoft.Office.Interop.Excel;
using SupA.Lib.Initialization;

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
            List<object> CollAdjacentSuptPoints = new List<object>();
            int SuptGroupNo = 0;

            // Call the method to define support frame sections
            DefineSuptFramesSections(CollPotlSuptFrameDetails, CollAdjacentSuptPoints, SuptGroupNo);

            // Iterate over CollPotlSuptFrameDetails and export FEM results for each frame
            foreach (cPotlSupt frame in CollPotlSuptFrameDetails)
            {
                ExportFEMresults(frame, SuptGroupNo);
            }
        }

        public static void DefineSuptFramesSections(List<cPotlSupt> CollPotlSuptFrameDetails, List<object> CollAdjacentSuptPoints, int SuptGroupNo)
        {
            cPotlSupt Frame;
            int FrameC;
            int NoofBeamGroups;
            object TmpExportArr;
            int SctnTblC;
            int AccSctnsC;
            int BeamsinFrmC;
            bool UnityCheckAcceptable;
            bool DeflectionAcceptable;
            cSteel Beam;

            double RangeRowEnd;
            bool BeamAvailableforLoads;
            TTblSectionProperties PreferedSctnProp;
            var TblLightestSctnProp = new TTblSectionProperties[1];
            var CollAcceptableSctnsProps = new Collection();
            var CollInvalidPotlSuptFrameDetails = new Collection();
            CollInvalidPotlSuptFrameDetails = new Collection();

            // First create an array containing only my preferred section sizes, and then sort this array by weight.
            CreateTblPreferedSctnsProps();

            // Create a variable to hold your OpenSTAAD object(s) and set this
            dynamic objOpenSTAAD = Interaction.GetObject(null, "StaadPro.OpenSTAAD");
            objOpenSTAAD.SetSilentMode(1);

            // Other STAAD Variables
            int RetVal;
            string StdFile;

            // Create the parent folder for all of our STAAD calculations
            var FSO = new FileSystemObject();

            // First create the high level folder that will house all the potl support runs for our frame
            FSO.CreateFolder(pubstrFolderPath + "SupAOutput\\Frame" + (SuptGroupNo + SuptGroupNoMod) + "\\STAAD");

            // Now work through every "potlsupt" frame routing option in my frame
            FrameC = 1;
            while (FrameC <= CollPotlSuptFrameDetails.Count)
            {
                Frame = CollPotlSuptFrameDetails[FrameC];
                CollAcceptableSctnsProps = new Collection();
                BeamAvailableforLoads = false;

                // Now go and create the FEM Model Geometry. A copy of the FEM Model Geometry is created for
                // every potential preferedsctnprop and these are all run as a single batch in STAAD
                // as this is the most time efficient way of doing things.
                CreateFEModelGeometry(Frame, CollAdjacentSuptPoints, NoofBeamGroups, BeamAvailableforLoads, SuptGroupNo, StdFile);

                objOpenSTAAD.OpenSTAADFile(StdFile);
                // Now we need to make sure that the file is opened before we move on
                while (!(FSO.FileExists(Strings.Replace(StdFile, ".std", ".png")) && FSO.FileExists(Strings.Replace(StdFile, ".emf", ".png"))))
                {
                    Interaction.Wait(DateAndTime.Now + TimeSpan.FromDays(1)); // Adjust the waiting time as necessary
                }

                // Load your STAAD file - make sure you have successfully run the file
                objOpenSTAAD.GetSTAADFile(StdFile, "TRUE");

            ReturnToAnalysis:
                // Run FEA analysis within STAAD
                RetVal = objOpenSTAAD.AnalyzeEx(1, 0, 1); // For some reason (1,1,1) does not work as an option file

                // Check if results are available or not
                if (objOpenSTAAD.Output.AreResultsAvailable == 0)
                {
                    Interaction.Wait(DateAndTime.Now + TimeSpan.FromDays(1)); // Adjust the waiting time as necessary
                    goto ReturnToAnalysis;
                    Interaction.MsgBox("Results Unavailable");
                    objOpenSTAAD = null;
                    return;
                }

                if (BeamAvailableforLoads == true)
                {
                    // Now do loops on 2 levels. Level 1 loop through our TblPreferedSctnsProps one section type at a time.
                    // Level 2 loops through our potlsupt frame one beam at a time and checks if the beam passed or failed.
                    // If any beams have failed then the entire frame should be assigned a fail.
                    // Based on this loop a new table of only the PreferedSctnsProps that passed the code checks is created
                    // Then we
                    SctnTblC = 1;
                    while (SctnTblC <= Information.UBound(TblPreferedSctnsProps))
                    {
                        UnityCheckAcceptable = true;
                        DeflectionAcceptable = true;
                        PreferedSctnProp = TblPreferedSctnsProps[SctnTblC];
                        // This used to be required but not any more as we are now using STAAD
                        // Call AssignSuptFrameSectionSizes(Frame, NoofBeamGroups, PreferedSctnProp);
                        for (BeamsinFrmC = 1; BeamsinFrmC <= Frame.BeamsinFrame.Count; BeamsinFrmC++)
                        {
                            Beam = (cSteel)Frame.BeamsinFrame[BeamsinFrmC];
                            // Analyse Results
                            AnalyseFEAResults(objOpenSTAAD, Beam, SctnTblC, BeamsinFrmC, PreferedSctnProp, ref UnityCheckAcceptable, ref DeflectionAcceptable);
                        }

                        // if all the beams with this section are code compliant then add it to our table of acceptable sections
                        if (UnityCheckAcceptable == true && DeflectionAcceptable == true)
                        {
                            CollAcceptableSctnsProps.Add(PreferedSctnProp.STAADSctnname);
                        }

                        SctnTblC = SctnTblC + 1;
                    }

                    // If CollAcceptableSctnsProps is empty then this means that no sections were acceptable
                    // Then remove this potlsupt from CollPotlSuptFrameDetails and add it to CollInvalidPotlSuptFrameDetails
                    // So that it can be written out as an un-acceptable route
                    if (CollAcceptableSctnsProps.Count == 0)
                    {
                        // MsgBox("No sections pass for this frame alternative");
                        CollInvalidPotlSuptFrameDetails.Add(Frame);
                        CollPotlSuptFrameDetails.Remove(FrameC);
                        FrameC = FrameC - 1;
                    }
                    else
                    {
                        // Now that we have a reduced list of acceptable sections in CollAcceptableSctnsProps then do a cost analysis for each alternative
                        TblLightestSctnProp = new TTblSectionProperties[1];
                        SelectBestPotlSupt(CollAcceptableSctnsProps, Frame);
                        // And finally create a STAAD input file with just the section selected for use in the frame
                        CreateFEModelGeometry(Frame, CollAdjacentSuptPoints, NoofBeamGroups, BeamAvailableforLoads, SuptGroupNo, StdFile);
                    }
                }
                else
                {
                    // the below is basically a fudge that removes any supports that don't have a support beam included in their definition
                    // these shouldn't have got to this point, but a couple are arriving.
                    CollPotlSuptFrameDetails.Remove(FrameC);
                    FrameC = FrameC - 1;
                }

                TblLightestSctnProp = new TTblSectionProperties[1];
                FrameC = FrameC + 1;
            }

            // Export the collection of those potlsupport routes that didn't pass
            if (pubBOOLTraceOn == true)
            {
                ExportColltoCSVFile(CollInvalidPotlSuptFrameDetails, "CollFEAFailPotlSuptFrameDetails-Beams-F" + (SuptGroupNo + SuptGroupNoMod), "csv", "WriteAllBeamsinFrame");
            }

            // At the end of your application, remember to terminate the OpenSTAAD objects.
            objOpenSTAAD = null;
        }


        public static void ExportFEMresults(cPotlSupt Frame, int SuptGroupNo)
        {
            object TmpExportArr;
            int RangeRowEnd;

            // Export beam property types
            RangeRowEnd = FindLastFullCell("Output", 11, 6);
            TmpExportArr = Interaction.CallByName(Sheets("Output").Range("E11:K" + RangeRowEnd), "value", CallType.Get);
            ExportArrtoCSVFile(TmpExportArr, "Frame" + (SuptGroupNo + SuptGroupNoMod) + "\\EndofBeamOutput-" + Frame.PotlSuptNo, "csv", true);

            // Export node coordinates
            RangeRowEnd = FindLastFullCell("Output", 10, 14);
            TmpExportArr = Interaction.CallByName(Sheets("Output").Range("N10:AA" + RangeRowEnd), "value", CallType.Get);
            ExportArrtoCSVFile(TmpExportArr, "Frame" + (SuptGroupNo + SuptGroupNoMod) + "\\AlongBeamOutput-" + Frame.PotlSuptNo, "csv", true);
        }

        public static int FindLastFullCell(string Sheetname, int RowNo, int ColNo)
        {
            Worksheet sheet = (Worksheet)Globals.ThisAddIn.Application.ActiveWorkbook.Sheets[Sheetname];
            Microsoft.Office.Interop.Excel.Range cell = null;

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
        public string ProfileName;
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
        public double Rotation;
    }
}
