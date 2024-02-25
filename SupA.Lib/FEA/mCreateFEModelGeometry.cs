using Microsoft.Office.Interop.Excel;
using SupA.Lib.Core;
using SupA.Lib.DataManipulation;
using SupA.Lib.Initialization;

using Excel = Microsoft.Office.Interop.Excel;

namespace SupA.Lib.FEA
{
    public class mCreateFEModelGeometry
    {
        public static void CreateFEModelGeometry(cPotlSupt Frame, List<cSuptPoints> CollAdjacentSuptPoints, out int NoofBeamGroups, bool BeamAvailableforLoads, int SuptGroupNo, out string StdFile)
        {
            int RowNo = 0;
            cGroupNode GroupNode = null;
            cSteel Beam = null;
            int Counter;
            int BeamNo;
            object[,] ArrGroupNodetoSeq;
            int I;
            var collSTAADStdinput = new List<string>();
            TTblSectionProperties[] TblSctnPropsforFEMfile;

            NoofBeamGroups = 0;

            if (mDefineSuptFramesSections.TblLightestSctnProp[0].ProfileName == "")
            {
                TblSctnPropsforFEMfile = mDefineSuptFramesSections.TblPreferedSctnsProps;
                StdFile = $"Frame{SuptGroupNo + mSubInitializationSupA.SuptGroupNoMod}\\STAAD\\{Frame.PotlSuptNo}\\input";
                // Create the high-level folder that will house all the potl support runs for our frame
                Directory.CreateDirectory(Path.Combine(mSubInitializationSupA.pubstrFolderPath, $"SupAOutput\\Frame{SuptGroupNo + mSubInitializationSupA.SuptGroupNoMod}\\STAAD\\{Frame.PotlSuptNo}"));
            }
            else
            {
                TblSctnPropsforFEMfile = mDefineSuptFramesSections.TblLightestSctnProp;
                StdFile = $"Frame{SuptGroupNo + mSubInitializationSupA.SuptGroupNoMod}\\STAAD\\{Frame.PotlSuptNo}\\SelectedSctnInput";
            }

            collSTAADStdinput = mImportCSVFiletoColl.ImportCSVFiletoColl(Path.Combine(mSubInitializationSupA.pubstrFolderPath, "Vars\\"), "STDfiletemplate", ".csv", ",", "String", "String");

            // collSTAADStdinput.Add("ASD",,,)
            I = 1;
            while (I <= TblSctnPropsforFEMfile.Length)
            {
                // Create Array mapping groupnodeNos to sequential Numbers
                ArrGroupNodetoSeq = CreateGroupNodetoSeqArr(Frame, GroupNode);

                // Clear and then fill out our node co-ordinates information
                CreateFEModelGeometryNodes(Frame, GroupNode, RowNo, ArrGroupNodetoSeq, I, collSTAADStdinput);

                // Clear and then fill our which nodes are connections into existing steel
                CreateFEModelGeometryRestraints(Frame, GroupNode, RowNo, ArrGroupNodetoSeq, I, collSTAADStdinput);

                // Clear and then fill out our beam definitions
                CreateFEModelGeometryBeams(Frame, Beam, RowNo, out NoofBeamGroups, ArrGroupNodetoSeq, I, collSTAADStdinput, TblSctnPropsforFEMfile[I]);

                // Clear and then fill out load information
                CreateFEModelLoadings(Frame, CollAdjacentSuptPoints, GroupNode, Beam, RowNo, ref BeamAvailableforLoads, I, ref collSTAADStdinput);

                I++;
            }

            mExportColltoCSVFile<string>.ExportColltoCSVFile(collSTAADStdinput, StdFile, "std", "String", true);
            StdFile = Path.Combine(mSubInitializationSupA.pubstrFolderPath, $"SupAOutput\\{StdFile}.std");

            // If pubBOOLTraceOn = True Then
            //     Call exportFEMinput(SuptGroupNo, Frame)
            // End If
        }

        public void ExportFEMInput(int SuptGroupNo, cPotlSupt Frame)
        {
            Application excelApp = new Application();
            Workbook workbook = excelApp.Workbooks.Open("YourWorkbookPath.xlsx");
            Worksheet input1Sheet = (Worksheet)workbook.Sheets["Input1"];
            Worksheet input2Sheet = (Worksheet)workbook.Sheets["Input2"];

            // Export node coordinates
            int rangeRowEnd = mDefineSuptFramesSections.FindLastFullCell(input1Sheet, 7, 12);
            Excel.Range range = input1Sheet.Range["L7", $"O{rangeRowEnd}"];
            int[,] nodeCoordinates = (int[,])range.Value;
            mExportArrtoCSVFile.ExportArrtoCSVFile(nodeCoordinates, $"Frame{SuptGroupNo + mSubInitializationSupA.SuptGroupNoMod}\\NodeCoordinates-{Frame.PotlSuptNo}", "csv", true);

            // Export restraints
            rangeRowEnd = mDefineSuptFramesSections.FindLastFullCell(input1Sheet, 7, 17);
            range = input1Sheet.Range["Q7", $"W{rangeRowEnd}"];
            int[,] restraints = (int[,])range.Value;
            mExportArrtoCSVFile.ExportArrtoCSVFile(restraints, $"Frame{SuptGroupNo + mSubInitializationSupA.SuptGroupNoMod}\\RestraintDetails-{Frame.PotlSuptNo}", "csv", true);

            // Export BeamDef
            rangeRowEnd = mDefineSuptFramesSections.FindLastFullCell(input1Sheet, 7, 25);
            range = input1Sheet.Range["Y7", $"AC{rangeRowEnd}"];
            int[,] beamDef = (int[,])range.Value;
            mExportArrtoCSVFile.ExportArrtoCSVFile(beamDef, $"Frame{SuptGroupNo + mSubInitializationSupA.SuptGroupNoMod}\\BeamDefinition-{Frame.PotlSuptNo}", "csv", true);

            // Export loads
            rangeRowEnd = mDefineSuptFramesSections.FindLastFullCell(input2Sheet, 12, 2);
            range = input2Sheet.Range["B12", $"J{rangeRowEnd}"];
            int[,] loads = (int[,])range.Value;
            mExportArrtoCSVFile.ExportArrtoCSVFile(loads, $"Frame{SuptGroupNo + mSubInitializationSupA.SuptGroupNoMod}\\LoadsonFrame-{Frame.PotlSuptNo}", "csv", true);

            workbook.Close(false);
            excelApp.Quit();

            // Release COM objects
            ReleaseObject(input1Sheet);
            ReleaseObject(input2Sheet);
            ReleaseObject(workbook);
            ReleaseObject(excelApp);
        }

        public static object[,] CreateGroupNodetoSeqArr(cPotlSupt Frame, cGroupNode GroupNode)
        {
            object[,] ArrTmp = new object[2, 1];
            int ArrC;
            bool NodeinArrFlag;
            int NewArrSize;

            foreach (cGroupNode node in Frame.GroupNodesinFrame)
            {
                NodeinArrFlag = false;
                ArrC = 0;
                while (ArrC <= ArrTmp.GetUpperBound(1))
                {
                    if ((string)ArrTmp[0, ArrC] == node.GroupName)
                    {
                        NodeinArrFlag = true;
                    }
                    ArrC++;
                }

                if (!NodeinArrFlag)
                {
                    NewArrSize = ArrTmp.GetUpperBound(1) + 1;
                    object[,] newTmp = new object[2, NewArrSize + 1];
                    Array.Copy(ArrTmp, newTmp, ArrTmp.Length);
                    ArrTmp = newTmp;
                    ArrTmp[0, NewArrSize] = node.GroupName;
                    ArrTmp[1, NewArrSize] = ArrTmp.GetUpperBound(1);
                }
            }

            return ArrTmp;
        }

        public static void CreateFEModelLoadings(cPotlSupt Frame, List<cSuptPoints> CollAdjacentSuptPoints, cGroupNode GroupNode, cSteel Beam, int RowNo, ref bool BeamAvailableforLoads, int I, ref List<string> collSTAADStdinput)
        {
            foreach (cSuptPoints Supt in CollAdjacentSuptPoints)
            {
                for (int BeamC = 0; BeamC < Frame.BeamsinFrame.Count; BeamC++)
                {
                    Beam = Frame.BeamsinFrame[BeamC];

                    // If the supt is along the current beam, then fill out load details
                    if (mCoordinateCheck.CoordinateCheck("PointAlongLine", mCreateCoordArray.CreateCoordArray(Supt, "cSuptPoints", "TOS", rounded: true), mCreateCoordArray.CreateCoordArray(Beam, "cSteel", "Start", rounded: true), mCreateCoordArray.CreateCoordArray(Beam, "cSteel", "End", rounded: true)))
                    {
                        // The below flag basically deletes any frame descriptions that are somehow missing the
                        // suptbeam for loading up from CollAdjacentSuptPoints
                        BeamAvailableforLoads = true;

                        // Complete the loading information for all 3 directions
                        for (int LoadC = 1; LoadC <= 3; LoadC++)
                        {
                            // Return the Fx, Fy, and Fz load information for the current direction under consideration in
                            // terms of global coordinates (Fx = 1, Fy = 2, and Fz = 3)
                            string LoadDir = LoadC == 1 ? "X" : LoadC == 2 ? "Y" : "Z";
                            float Load = LoadC == 1 ? Supt.SuptLoadFx / 1000f : LoadC == 2 ? Supt.SuptLoadFy / 1000f : Supt.SuptLoadFz / 1000f;

                            if (Load != 0)
                            {
                                // Calculate the distance along the beam that corresponds to the load point
                                float DistAlongBeam = Math.Abs(CalculateDistAlongBeam(mCreateCoordArray.CreateCoordArray(Beam, "cSteel", "Start", rounded: true), mCreateCoordArray.CreateCoordArray(Supt, "cSuptPoints", "TOS", rounded: true)));

                                int collRowInputno = rtnCollRowNumber(collSTAADStdinput, "MEMBER LOAD");
                                string LoadDef = $"{I}0{BeamC} CON G{LoadDir} {Load} {DistAlongBeam} 0";
                                collSTAADStdinput.Add(LoadDef);
                            }
                        }
                    }
                }
            }
        }

        private static float CalculateDistAlongBeam(float[] BeamStartPos, float[] SuptPos)
        {
            float DistAlongBeam = SuptPos[0] - BeamStartPos[0];
            DistAlongBeam += SuptPos[1] - BeamStartPos[1];
            DistAlongBeam += SuptPos[2] - BeamStartPos[2];
            return DistAlongBeam;
        }

        public static void CreateFEModelGeometryNodes(cPotlSupt Frame, cGroupNode GroupNode, int RowNo, dynamic ArrGroupNodetoSeq, int I, List<string> collSTAADStdinput)
        {
            int GroupNodeC;
            int LookupTblC;
            var CollNodesAlreadyDefined = new List<object>();
            int AlreadyDefinedC;
            bool DuplicateNodeDef;
            int collRowInputno = rtnCollRowNumber(collSTAADStdinput, "JOINT COORDINATES");

            for (GroupNodeC = 1; GroupNodeC <= Frame.GroupNodesinFrame.Count; GroupNodeC++)
            {
                GroupNode = Frame.GroupNodesinFrame[GroupNodeC];

                // This extra piece of the code is for converting the group node numbers used within the wider SupA algorithm
                // to sequential numbered nodes for use in the FEA code
                for (LookupTblC = 1; LookupTblC <= ArrGroupNodetoSeq.GetLength(2); LookupTblC++)
                {
                    if (GroupNode.GroupName == ArrGroupNodetoSeq[1, LookupTblC])
                    {
                        break;
                    }
                }

                // This makes sure that each node is only ever defined once in our input.
                DuplicateNodeDef = false;
                for (AlreadyDefinedC = 1; AlreadyDefinedC <= CollNodesAlreadyDefined.Count; AlreadyDefinedC++)
                {
                    if (CollNodesAlreadyDefined[AlreadyDefinedC] == ArrGroupNodetoSeq[2, LookupTblC])
                    {
                        DuplicateNodeDef = true;
                    }
                }

                if (!DuplicateNodeDef)
                {
                    CollNodesAlreadyDefined.Add(ArrGroupNodetoSeq[2, LookupTblC]);
                    collSTAADStdinput.Add(I + "0" + ArrGroupNodetoSeq[2, LookupTblC] + " " + GroupNode.GroupedNodes[1].Easting + " " + GroupNode.GroupedNodes[1].Northing + " " + GroupNode.GroupedNodes[1].Upping + $" {Type.Missing} ${collRowInputno}");
                }
            }
        }

        public static void CreateFEModelGeometryRestraints(cPotlSupt Frame, cGroupNode GroupNode, int RowNo, object[,] ArrGroupNodetoSeq, int I, List<string> collSTAADStdinput)
        {
            int LookupTblC;
            float collRowInputno = rtnCollRowNumber(collSTAADStdinput, "SUPPORTS");

            foreach (cGroupNode groupNode in Frame.GroupNodesinFrame)
            {
                GroupNode = groupNode;
                if (GroupNode.AssocExistingSteel != "")
                {
                    for (LookupTblC = 1; LookupTblC <= ArrGroupNodetoSeq.GetLength(1); LookupTblC++)
                    {
                        if (GroupNode.GroupName == (string)ArrGroupNodetoSeq[0, LookupTblC - 1])
                        {
                            break;
                        }
                    }

                    collSTAADStdinput.Add(I + "0" + ArrGroupNodetoSeq[1, LookupTblC - 1] + $" FIXED {collRowInputno}");
                }
            }
        }

        public static void CreateFEModelGeometryBeams(cPotlSupt Frame, cSteel Beam, int RowNo, out int NoofBeamGroups, object[,] ArrGroupNodetoSeq, int I, List<string> collSTAADStdinput, TTblSectionProperties CurrSctn)
        {
            int LookupTblC;
            string BeamInput;
            NoofBeamGroups = 0;
            int collRowInputnoMI = rtnCollRowNumber(collSTAADStdinput, "MEMBER INCIDENCES");
            int collRowInputnoMP = rtnCollRowNumber(collSTAADStdinput, "MEMBER PROPERTY 'EUROPE (EN 2023).DB3'");

            for (int BeamC = 0; BeamC < Frame.BeamsinFrame.Count; BeamC++)
            {
                Beam = Frame.BeamsinFrame[BeamC];

                BeamInput = I + "0" + BeamC;

                for (LookupTblC = 0; LookupTblC < ArrGroupNodetoSeq.GetLength(1); LookupTblC++)
                {
                    if (Beam.ExistingSteelConnNameStart == (string)ArrGroupNodetoSeq[0, LookupTblC])
                    {
                        break;
                    }
                }

                BeamInput += " " + I + "0" + ArrGroupNodetoSeq[1, LookupTblC];

                for (LookupTblC = 0; LookupTblC < ArrGroupNodetoSeq.GetLength(1); LookupTblC++)
                {
                    if (Beam.ExistingSteelConnNameEnd == (string)ArrGroupNodetoSeq[0, LookupTblC])
                    {
                        break;
                    }
                }

                BeamInput += " " + I + "0" + ArrGroupNodetoSeq[1, LookupTblC];
                collSTAADStdinput.Add(BeamInput + $" {collRowInputnoMI}");

                collSTAADStdinput.Add(I + "0" + BeamC + " " + CurrSctn.STAADSctnname + $" {collRowInputnoMP}");

                RowNo++;
            }
        }

        public static int AssignBeamGroupNo(cPotlSupt Frame, cSteel Beam, out int NoofBeamGroups)
        {
            cSteel CompareBeam;
            List<float> CollUppings = new List<float>();
            bool VerticalColsExist = false;

            NoofBeamGroups = 0;

            // Work through all columns,
            // first, find out all the relevant elevations for horizontal beams and add them to a collection
            // also, return whether there are any vertical columns in our support
            foreach (var beam in Frame.BeamsinFrame)
            {
                if (beam.Dir != "U" && beam.Dir != "D")
                {
                    bool DuplicateUppingFlag = false;
                    foreach (var upping in CollUppings)
                    {
                        if (beam.StartU == upping)
                        {
                            DuplicateUppingFlag = true;
                            break;
                        }
                    }
                    if (!DuplicateUppingFlag)
                    {
                        CollUppings.Add(beam.StartU);
                    }
                }
                else
                {
                    VerticalColsExist = true;
                }
            }

            // Now we assign our beam a group number based on its upping and the collingupping collection
            int UppingC;
            for (UppingC = 0; UppingC < CollUppings.Count; UppingC++)
            {
                var upping = CollUppings[UppingC];
                if (Beam.StartU == upping)
                {
                    if (VerticalColsExist)
                    {
                        return 1 + UppingC;
                    }
                    else
                    {
                        return UppingC + 1;
                    }
                }
            }

            // If we are a vertical column, then overwrite our assigned beamgroupno to 1
            if (Beam.Dir == "U" || Beam.Dir == "D")
            {
                NoofBeamGroups = 1;
                return 1;
            }

            if (VerticalColsExist)
            {
                NoofBeamGroups = 1 + CollUppings.Count;
            }
            else
            {
                NoofBeamGroups = CollUppings.Count;
            }

            // This line should never be reached, but it's needed to satisfy the compiler
            return -1;
        }

        public static int rtnCollRowNumber(List<string> collSTAADStdinput, string rtnString)
        {
            for (int i = 1; i <= collSTAADStdinput.Count; i++)
            {
                if (collSTAADStdinput[i].ToString().Replace("'", "") == rtnString.Replace("'", ""))
                {
                    return i + 1;
                }
            }
            return -1; // or any other default value to indicate not found
        }

        private void ReleaseObject(object obj)
        {
            try
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
                obj = null;
            }
            catch (Exception ex)
            {
                obj = null;
            }
            finally
            {
                GC.Collect();
            }
        }
    }
}
