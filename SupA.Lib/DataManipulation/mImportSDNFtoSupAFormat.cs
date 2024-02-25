using SupA.Lib.CoordinateAndAngleManipulation;
using SupA.Lib.Core;

namespace SupA.Lib.DataManipulation
{
    public class mImportSDNFtoSupAFormat
    {
        public static void ImportSDNFtoSupA(string FolderPath, string FilenameIn, string FilenameOut)
        {
            string[,] arrSDNF;
            int I;
            cSteel sctnImport;
            List<cSteel> CollSupAStlImport = new ();
            bool flgSteelSection = false;
            int Ecounter, Ncounter, Ucounter;

            arrSDNF = mImportCSVFile.ImportCSVFile(FolderPath, FilenameIn, ".csv", ",");

            for (I = 2; I <= arrSDNF.GetUpperBound(0); I++)
            {
                if (arrSDNF[I, 1].ToString() == "Packet 10" && arrSDNF[I - 1, 1].ToString() == "#")
                {
                    I++;
                    flgSteelSection = true;
                }
                else if ((arrSDNF[I, 1].ToString() == "#" && arrSDNF[I + 1, 1].ToString() == "#############") ||
                         (arrSDNF[I, 1].ToString() == "#" && arrSDNF[I + 1, 1].ToString() == "Packet 20"))
                {
                    break;
                }
                else if (flgSteelSection)
                {
                    if (arrSDNF[I + 2, 1].ToString().Split(' ')[3] == arrSDNF[I + 2, 1].ToString().Split(' ')[6])
                        Ecounter = 1;
                    else
                        Ecounter = 0;

                    if (arrSDNF[I + 2, 1].Split(' ')[4] == arrSDNF[I + 2, 1].Split(' ')[7])
                        Ncounter = 1;
                    else
                        Ncounter = 0;

                    if (arrSDNF[I + 2, 1].Split(' ')[5] == arrSDNF[I + 2, 1].Split(' ')[8])
                        Ucounter = 1;
                    else
                        Ucounter = 0;

                    if (Ecounter + Ncounter + Ucounter == 2)
                    {
                        sctnImport = returnSupAImportFormat(I, arrSDNF);
                        CollSupAStlImport.Add(sctnImport);
                    }
                    I += 5;
                }
            }

            mExportColltoCSVFile<cSteel>.ExportColltoCSVFile(CollSupAStlImport, FilenameOut, "csv", "writeall");
        }

        public static cSteel returnSupAImportFormat(int I, string[,] arrSDNF)
        {
            // Create a new instance of cSteel
            cSteel sctnImport = new cSteel();
            float CutbackStart = 0;
            float CutbackEnd = 0;
            // Notes on the conversion from SDNF to SupA format:
            // Lots of SDNF fields haven't been included in this return function yet, including:
            // - Material Grade
            // - Mirror X Axis
            // - Mirror Y Axis
            // XYZ Offsets Start and End (linear record number 5)
            // XY Cross Section Offsets (linear record number 4)

            // And in addition, there have been some assumptions made:
            // - The end cutback has a negative sign included in this. I'm not sure if this is correct.
            // - Is the SupA bangle a 1 to 1 relationship with the linear member record 2 rotation field?

            // Also, some SupA fields have not been created correctly in the below function. For example:
            // - Ori (this has been set to blank)
            // - Jusline = (this has been set to NA)
            // - Model Name is not yet collected from Plant 3D

            // These are the values with predefined entries which need setting with integer or string types as part of the SupA import definition process
            sctnImport.ModelName = "Import File Line " + I;
            sctnImport.OwningDisc = "S";
            sctnImport.BeamNo = 0;
            sctnImport.SuptNosonBeam = "";
            sctnImport.PrelimSctnReqd = "";
            sctnImport.SctnDepth = 0;
            sctnImport.SctnWidth = 0;
            sctnImport.LevelNo = 0;
            sctnImport.SuptBeamPerpDirFromPipeAxis = 0;
            sctnImport.ExistingSteelConnNameStart = "";
            sctnImport.ExistingSteelConnNameEnd = "";
            sctnImport.ClashonBeam = false;
            sctnImport.SuptSteelFunction = "";
            sctnImport.MemType = "";
            sctnImport.MemTypeGeneric = "";
            sctnImport.MemModelParam = "";
            sctnImport.StartERounded = 0;
            sctnImport.StartNRounded = 0;
            sctnImport.StartURounded = 0;
            sctnImport.EndERounded = 0;
            sctnImport.EndNRounded = 0;
            sctnImport.EndURounded = 0;
            sctnImport.FeatureDesc = "";
            sctnImport.Ori = "";

            // These are the values which need to be set based on the SDNF data

            // Pick up the relevant fields from linear record number 2
            sctnImport.MemTypeModelRef = arrSDNF[I + 1, 0].ToString().Split(' ')[0].Replace("\"", ""); // First part of string two ("Section Size" in the SDNF notation)

            string bangleWorkings = arrSDNF[I + 1, 0].ToString().Split('"')[4];
            sctnImport.Bangle = bangleWorkings.Split(' ')[1]; // third part of string two ("rotation" in the SDNF notation)

            // Pick up the relevant fields from linear record number 3

            // Set the initial sctn starts and ends so that these can be used to calculate dir and feed back to the end sctn starts and ends with cutbacks included
            sctnImport.StartE = float.Parse(arrSDNF[I + 2, 0].ToString().Split(' ')[2]);
            sctnImport.StartN = float.Parse(arrSDNF[I + 2, 0].ToString().Split(' ')[3]);
            sctnImport.StartU = float.Parse(arrSDNF[I + 2, 0].ToString().Split(' ')[4]);
            sctnImport.EndE = float.Parse(arrSDNF[I + 2, 0].ToString().Split(' ')[5]);
            sctnImport.EndN = float.Parse(arrSDNF[I + 2, 0].ToString().Split(' ')[6]);
            sctnImport.EndU = float.Parse(arrSDNF[I + 2, 0].ToString().Split(' ')[7]);

            // Calculate the beam axis direction from the start and end coordinates
            sctnImport.Dir = mCalculateDirBasedonCoords.CalculateDirBasedonCoords(sctnImport.StartE, sctnImport.StartN, sctnImport.StartU,
                                                        sctnImport.EndE, sctnImport.EndN, sctnImport.EndU);

            // Fetch the Cut Back Dimensions from the SDNF File.
            CutbackStart = float.Parse(arrSDNF[I + 2, 0].ToString().Split(' ')[8]); // The start cutback is the first entry.
                                                                                 // Note that end cutback direction is inverted (-) 'THIS MAY NOT BE CORRECT
            CutbackEnd = float.Parse(arrSDNF[I + 2, 0].ToString().Split(' ')[9]); // The end cutback is the second entry.'Note that end cutback direction may need to be inverted (-) ????

            // Calculate start and end coordinates considering the cutback, beam dir, and start and coordinates from the SDNF file.
            sctnImport.StartE = float.Parse(arrSDNF[I + 2, 0].ToString().Split(' ')[2]) + mDecomposeThreeDVectorintoENUCoords.DecomposeThreeDVectorintoENUCoords(sctnImport.Dir, CutbackStart)[0];
            sctnImport.StartN = float.Parse(arrSDNF[I + 2, 0].ToString().Split(' ')[3]) + mDecomposeThreeDVectorintoENUCoords.DecomposeThreeDVectorintoENUCoords(sctnImport.Dir, CutbackStart)[1];
            sctnImport.StartU = float.Parse(arrSDNF[I + 2, 0].ToString().Split(' ')[4]) + mDecomposeThreeDVectorintoENUCoords.DecomposeThreeDVectorintoENUCoords(sctnImport.Dir, CutbackStart)[2];
            sctnImport.EndE = float.Parse(arrSDNF[I + 2, 0].ToString().Split(' ')[5]) + mDecomposeThreeDVectorintoENUCoords.DecomposeThreeDVectorintoENUCoords(sctnImport.Dir, CutbackEnd)[0];
            sctnImport.EndN = float.Parse(arrSDNF[I + 2, 0].ToString().Split(' ')[6]) + mDecomposeThreeDVectorintoENUCoords.DecomposeThreeDVectorintoENUCoords(sctnImport.Dir, CutbackEnd)[1];
            sctnImport.EndU = float.Parse(arrSDNF[I + 2, 0].ToString().Split(' ')[7]) + mDecomposeThreeDVectorintoENUCoords.DecomposeThreeDVectorintoENUCoords(sctnImport.Dir, CutbackEnd)[2];

            // Calculate length using a 3D vector formula
            sctnImport.Length = (float)Math.Sqrt(Math.Pow(sctnImport.StartE - sctnImport.EndE, 2) +
                                                  Math.Pow(sctnImport.StartN - sctnImport.EndN, 2) +
                                                  Math.Pow(sctnImport.StartU - sctnImport.EndU, 2));

            // Calculate the major axis global dir from the SDNF orientation vector (entries 1, 2 & 3 in linear record number 3) using the CalculateDirBasedonCoords function
            sctnImport.MinorAxisGlobaldir = mCalculateDirBasedonCoords.CalculateDirBasedonCoords(0, 0, 0,
                                                                       int.Parse(arrSDNF[I + 2, 0].ToString().Split(' ')[0]),
                                                                       int.Parse(arrSDNF[I + 2, 0].ToString().Split(' ')[1]),
                                                                       int.Parse(arrSDNF[I + 2, 0].ToString().Split(' ')[2]));

            sctnImport.MajorAxisGlobaldir = mDefineMaxMajAxfromMinAxBeamD.DefineMajorAxisfromMinorAxisandBeamDir(sctnImport.MinorAxisGlobaldir, sctnImport.Dir);

            if (sctnImport.Bangle == "90")
            {
                sctnImport.MinorAxisGlobaldir = mFindRotatedbyAngle.FindRotatedbyAngle(sctnImport.MinorAxisGlobaldir, 90, "U");
                sctnImport.MajorAxisGlobaldir = mFindRotatedbyAngle.FindRotatedbyAngle(sctnImport.MajorAxisGlobaldir, 90, "U");
            }

            // Pick up the relevant fields from linear record number 4
            // None
            sctnImport.Jusline = "NA";

            // Pick up the relevant fields from linear record number 5
            // None

            // Pick up the relevant fields from linear record number 6
            // None

            return sctnImport;
        }

        public cSteel ReturnSupAImportFormatLineOne()
        {
            // Create a new instance of cSteel
            cSteel sctnImport = new cSteel();

            sctnImport.BeamNo = 0; // "BeamNo"
            sctnImport.StartE = 1; // "StartE"
            sctnImport.StartN = 2; // "StartN"
            sctnImport.StartU = 3; // "StartU"
            sctnImport.EndE = 4; // "EndE"
            sctnImport.EndN = 5; // "EndN"
            sctnImport.EndU = 6; // "EndU"
            sctnImport.Length = 7; // "Length"
            sctnImport.Dir = "Dir";
            sctnImport.SuptNosonBeam = "SuptNosonBeam";
            sctnImport.PrelimSctnReqd = "PrelimSctnReqd";
            sctnImport.SctnDepth = 11; // "SctnDepth"
            sctnImport.SctnWidth = 12; // "SctnWidth"
            sctnImport.LevelNo = 13; // "LevelNo"
            sctnImport.SuptBeamPerpDirFromPipeAxis = 14; // "SuptBeamPerpDirFromPipeAxis"
            sctnImport.ExistingSteelConnNameStart = "ExistingSteelConnNameStart";
            sctnImport.ExistingSteelConnNameEnd = "ExistingSteelConnNameEnd";
            sctnImport.ClashonBeam = false; // "ClashonBeam"
            sctnImport.SuptSteelFunction = "SuptSteelFunction";
            sctnImport.ModelName = "ModelName";
            sctnImport.OwningDisc = "OwningDisc";
            sctnImport.MemTypeModelRef = "MemTypeModelRef";
            sctnImport.MemType = "MemType";
            sctnImport.MemTypeGeneric = "MemTypeGeneric";
            sctnImport.MemModelParam = "MemModelParam";
            sctnImport.Jusline = "Jusline";
            sctnImport.Bangle = "Bangle";
            sctnImport.Ori = "Ori";
            sctnImport.MinorAxisGlobaldir = "MinorAxisGlobaldir";
            sctnImport.MajorAxisGlobaldir = "MajorAxisGlobaldir";
            sctnImport.StartERounded = 30; // "StartERounded"
            sctnImport.StartNRounded = 31; // "StartNRounded"
            sctnImport.StartURounded = 32; // "StartURounded"
            sctnImport.EndERounded = 33; // "EndERounded"
            sctnImport.EndNRounded = 34; // "EndNRounded"
            sctnImport.EndURounded = 35; // "EndURounded"
            sctnImport.FeatureDesc = "36"; // "FeatureDesc"

            return sctnImport;
        }
    }
}
