namespace SupA.Lib.Initialization
{
    public struct TTblExtendedPHASST
    {
        public float PipeRangeMinNB { get; set; }
        public float PipeRangeMaxNB { get; set; }
        public string ApplicableAreaTypes { get; set; }
        public float InsuThkMin { get; set; }
        public float InsuThkMax { get; set; }
        public float OpTemp { get; set; }
        public float MinDesTemp { get; set; }
        public float MaxDesTemp { get; set; }
        public float BlowDownDuratn { get; set; }
        public string AnciType { get; set; }
        public float AnciHeightStd { get; set; }
        public float AnciHeightMin { get; set; }
        public float AnciHeightMax { get; set; }
        public float BeamLenReqd { get; set; }
    }

    public struct TTblDefinitionofAdjacentSupport
    {
        public float DistPerp1toS { get; set; }
        public float DistPerp2toS { get; set; }
        public float DistParltoS { get; set; }
        public int NoofLoopIterations { get; set; }
        public float AllowableTOSDiff { get; set; }
    }

    public struct TTblDefinitionofLocalExistingSteel
    {
        public float DistPerp1toS { get; set; }
        public float DistPerp2toS { get; set; }
        public float DistParltoS { get; set; }
    }

    public struct TTblElementTypesandHierarchies
    {
        public string Parent { get; set; }
        public string Child { get; set; }
    }

    public struct TTblNodeMapArrayNametoColumnMapping
    {
        public int ArrFrameNodeMapColNo { get; set; }
        public string AssociatedDiscretisedArrNm { get; set; }
    }

    public struct TTblStandardClearancesbyElementType
    {
        public string Element1 { get; set; }
        public string Element2 { get; set; }
        public float ClearanceReqd { get; set; }
        public string ActionifClashFound { get; set; }
        public string ActionifClearanceIssueFound { get; set; }
        public string OverrideFunction1 { get; set; }
        public string OverrideFunction2 { get; set; }
        public string ExceptionRule1 { get; set; }
        public string ExceptionRule2 { get; set; }
    }

    public struct TTblPipeDimns
    {
        public float NB { get; set; }
        public float OD { get; set; }
    }

    public struct TTblElementLevelReportedinClashManager
    {
        public string Element { get; set; }
    }

    public struct TTblSectionProperties
    {
        public string ProfileStandard { get; set; }
        public string ProfileName { get; set; }
        public string ProfileFamily { get; set; }
        public string ProfileThreeDModelNm { get; set; }
        public bool PreferredSctnFlag { get; set; }
        public float Depth { get; set; }
        public float Width { get; set; }
        public float Weight { get; set; }
        public float Area { get; set; }
        public float MinAxisShearArea { get; set; }
        public float MajAxisShearArea { get; set; }
        public float MajAxisSecondMofA { get; set; }
        public float MinAxisSecondMofA { get; set; }
        public float TorsionConstant { get; set; }
        public float Rotation { get; set; }
        public string STAADSctnname { get; set; }
    }

    public struct TTblMaterialProperties
    {
        public string MatlRef { get; set; }
        public string MatlDesc { get; set; }
        public string ThreeDModelMatlDesc { get; set; }
        public float YoungsMod { get; set; }
        public float ShearMod { get; set; }
        public float AllowableStress { get; set; }
    }

    public struct TTblBeamJusLinetoFacesDef
    {
        public string BeamType { get; set; }
        public string Jusline { get; set; }
        public string FaceDesc { get; set; }
        public float DistanceinDepth { get; set; }
        public float DistanceinWidth { get; set; }
    }

    public struct TTblSDNFBeamTypetoSupABeamType
    {
        public string P3DType { get; set; }
        public string SupAType { get; set; }
    }

    public struct TTblBeamDetailing
    {
        public string ProfileFamily { get; set; }
        public string BeamDir { get; set; }
        public string BeamDirAndDesc { get; set; }
        public string BeamStartorEnd { get; set; }
        public string ConnTypeAtCurrEndofBeam { get; set; }
        public string ExistingSteelFace { get; set; }
        public string BeaminRelationtoAvPosofSupts { get; set; }
        public float BeamAdjustpart1BMwidth { get; set; }
        public float BeamAdjustpart1BMdepth { get; set; }
        public float BeamAdjustpart2const { get; set; }
        public float BeamAdjustpart1ESwidth { get; set; }
        public float BeamAdjustpart1ESdepth { get; set; }
        public float BeamTraninDirofPipeBMwidth { get; set; }
        public float BeamTraninDirofPipeBMdepth { get; set; }
        public float BeamTranPerpToPipeBMwidth { get; set; }
        public float BeamTranPerpToPipeBMdepth { get; set; }
        public float BeamTranUDBMdepth { get; set; }
        public float Rotation { get; set; }
        public float ExistingSteelBMDepth { get; set; }
        public float ExistingSteelBMWidth { get; set; }
        public float Jusline { get; set; }
        public float CorrforEWPipesXofBeamDepth { get; set; }
    }

    public struct TTblConnDetailing
    {
        public string ProfileFamily { get; set; }
        public string ProfileFamilyConnBeam { get; set; }
        public string BeamDirAndDesc { get; set; }
        public string BeamStartorEnd { get; set; }
        public string ConnTypeAtCurrEndofBeam { get; set; }
        public string ExistingSteelFace { get; set; }
        public string ReinforcingGussetReqd { get; set; }
        public string BasePlateReqd { get; set; }
    }

    public struct TTblBasePlateDef
    {
        public string ProfileStandard;
        public string ProfileName;
        public string ProfileFamily;
        public string ProfileThreeDModelNm;
        public float Depth;
        public float Width;
        public float BasePlateThk;
        public float BasePlateWidth;
        public float BasePlateLength;
        public string BasePlateBoltingReqd;
        public float BoltSpacing;
        public float BoltSize;
        public float PlinthThk;
        public float PlinthWidth;
        public float PlinthLength;
    }

    public struct TTblSuptSpanRules
    {
        public int PipeBorOrRackSize;
        public float MaxSuptDistfromBends;
        public float MaxDistBetweenBendsWoutSupt;
        public float MaxSpan;
        public bool MidPointSuptReqdonBend;
        public string RuleDiscApplicability;
        public float MinSuptDistfromElbo;
        public float MinSpanBtwnSuptsRatio;
        public bool OptionBSuptsforAllDirChanges;
        public float MaxSuptDistfromHeaderEqSizeBra;
        public float MaxSuptDistfromHeaderRedSizeBra;
        public float MaxSuptDistfromUnrestrainedEnd;
        public float PipeOD;
        public float ShoeHeight;
        public float SuptSteelMinWidthReqd;
    }

    public struct TTblSuptScoreCat
    {
        public float MaxValforPointScore;
        public float MinValforPointScore;
        public string CodeAction;
        public int PointCat;
    }

    public class mPublicVarDefinitions
    {
        public static TTblExtendedPHASST[] CreateTblExtendedPHASST(object[,] InputArray)
        {
            TTblExtendedPHASST[] NewTblExtendedPHASST = new TTblExtendedPHASST[InputArray.GetLength(0)];

            for (int i = 0; i < InputArray.GetLength(0); i++)
            {
                NewTblExtendedPHASST[i] = new TTblExtendedPHASST
                {
                    PipeRangeMinNB = Convert.ToSingle(InputArray[i + 1, 1]),
                    PipeRangeMaxNB = Convert.ToSingle(InputArray[i + 1, 2]),
                    ApplicableAreaTypes = Convert.ToString(InputArray[i + 1, 3]),
                    InsuThkMin = Convert.ToSingle(InputArray[i + 1, 4]),
                    InsuThkMax = Convert.ToSingle(InputArray[i + 1, 5]),
                    OpTemp = Convert.ToSingle(InputArray[i + 1, 6]),
                    MinDesTemp = Convert.ToSingle(InputArray[i + 1, 7]),
                    MaxDesTemp = Convert.ToSingle(InputArray[i + 1, 8]),
                    BlowDownDuratn = Convert.ToSingle(InputArray[i + 1, 9]),
                    AnciType = Convert.ToString(InputArray[i + 1, 10]),
                    AnciHeightStd = Convert.ToSingle(InputArray[i + 1, 11]),
                    AnciHeightMin = Convert.ToSingle(InputArray[i + 1, 12]),
                    AnciHeightMax = Convert.ToSingle(InputArray[i + 1, 13]),
                    BeamLenReqd = Convert.ToSingle(InputArray[i + 1, 14])
                };
            }

            return NewTblExtendedPHASST;
        }

        public static void CreateTblDefinitionofAdjacentSupport(object[,] InputArray, out TTblDefinitionofAdjacentSupport pubTblDefinitionofAdjacentSupport)
        {
            pubTblDefinitionofAdjacentSupport = new TTblDefinitionofAdjacentSupport
            {
                DistPerp1toS = Convert.ToSingle(InputArray[1, 1]),
                DistPerp2toS = Convert.ToSingle(InputArray[1, 2]),
                DistParltoS = Convert.ToSingle(InputArray[1, 3]),
                NoofLoopIterations = Convert.ToInt32(InputArray[1, 4]),
                AllowableTOSDiff = Convert.ToSingle(InputArray[1, 5])
            };
        }

        public static void CreateTblDefinitionofLocalExistingSteel(object[,] InputArray, out TTblDefinitionofLocalExistingSteel pubTblDefinitionofLocalExistingSteel)
        {
            pubTblDefinitionofLocalExistingSteel = new TTblDefinitionofLocalExistingSteel
            {
                DistPerp1toS = Convert.ToSingle(InputArray[1, 1]),
                DistPerp2toS = Convert.ToSingle(InputArray[1, 2]),
                DistParltoS = Convert.ToSingle(InputArray[1, 3])
            };
        }

        public static void CreateTblElementTypesandHierarchies(object[,] InputArray, out TTblElementTypesandHierarchies[] pubTblElementTypesandHierarchies)
        {
            pubTblElementTypesandHierarchies = new TTblElementTypesandHierarchies[InputArray.GetLength(0)];

            for (int ArrayRowCounter = 0; ArrayRowCounter < InputArray.GetLength(0); ArrayRowCounter++)
            {
                pubTblElementTypesandHierarchies[ArrayRowCounter].Parent = Convert.ToString(InputArray[ArrayRowCounter + 1, 1]);
                pubTblElementTypesandHierarchies[ArrayRowCounter].Child = Convert.ToString(InputArray[ArrayRowCounter + 1, 2]);
            }
        }

        public static void CreateTblNodeMapArrayNametoColumnMapping(object[,] InputArray, out TTblNodeMapArrayNametoColumnMapping[] pubTblNodeMapArrayNametoColumnMapping)
        {
            pubTblNodeMapArrayNametoColumnMapping = new TTblNodeMapArrayNametoColumnMapping[InputArray.GetLength(0)];

            for (int ArrayRowCounter = 0; ArrayRowCounter < InputArray.GetLength(0); ArrayRowCounter++)
            {
                pubTblNodeMapArrayNametoColumnMapping[ArrayRowCounter].ArrFrameNodeMapColNo = Convert.ToInt32(InputArray[ArrayRowCounter + 1, 1]);
                pubTblNodeMapArrayNametoColumnMapping[ArrayRowCounter].AssociatedDiscretisedArrNm = Convert.ToString(InputArray[ArrayRowCounter + 1, 2]);
            }
        }

        public static void CreateTblStandardClearancesbyElementType(object[,] InputArray, out TTblStandardClearancesbyElementType[] pubTblStandardClearancesbyElementType)
        {
            pubTblStandardClearancesbyElementType = new TTblStandardClearancesbyElementType[InputArray.GetLength(0)];

            for (int ArrayRowCounter = 0; ArrayRowCounter < InputArray.GetLength(0); ArrayRowCounter++)
            {
                pubTblStandardClearancesbyElementType[ArrayRowCounter].Element1 = Convert.ToString(InputArray[ArrayRowCounter + 1, 1]);
                pubTblStandardClearancesbyElementType[ArrayRowCounter].Element2 = Convert.ToString(InputArray[ArrayRowCounter + 1, 2]);
                pubTblStandardClearancesbyElementType[ArrayRowCounter].ClearanceReqd = Convert.ToSingle(InputArray[ArrayRowCounter + 1, 3]);
                pubTblStandardClearancesbyElementType[ArrayRowCounter].ActionifClashFound = Convert.ToString(InputArray[ArrayRowCounter + 1, 4]);
                pubTblStandardClearancesbyElementType[ArrayRowCounter].ActionifClearanceIssueFound = Convert.ToString(InputArray[ArrayRowCounter + 1, 5]);
                pubTblStandardClearancesbyElementType[ArrayRowCounter].OverrideFunction1 = Convert.ToString(InputArray[ArrayRowCounter + 1, 6]);
                pubTblStandardClearancesbyElementType[ArrayRowCounter].OverrideFunction2 = Convert.ToString(InputArray[ArrayRowCounter + 1, 7]);
                pubTblStandardClearancesbyElementType[ArrayRowCounter].ExceptionRule1 = Convert.ToString(InputArray[ArrayRowCounter + 1, 8]);
                pubTblStandardClearancesbyElementType[ArrayRowCounter].ExceptionRule2 = Convert.ToString(InputArray[ArrayRowCounter + 1, 9]);
            }
        }

        public static void CreateTblPipeDimns(object[,] InputArray, out TTblPipeDimns[] pubTblPipeDimns)
        {
            pubTblPipeDimns = new TTblPipeDimns[InputArray.GetLength(0)];

            for (int ArrayRowCounter = 0; ArrayRowCounter < InputArray.GetLength(0); ArrayRowCounter++)
            {
                pubTblPipeDimns[ArrayRowCounter].NB = Convert.ToSingle(InputArray[ArrayRowCounter + 1, 1]);
                pubTblPipeDimns[ArrayRowCounter].OD = Convert.ToSingle(InputArray[ArrayRowCounter + 1, 2]);
            }
        }

        public static void CreateTblElementLevelReportedinClashManager(object[,] InputArray, out TTblElementLevelReportedinClashManager[] pubTblElementLevelReportedinClashManager)
        {
            pubTblElementLevelReportedinClashManager = new TTblElementLevelReportedinClashManager[InputArray.GetLength(0)];

            for (int ArrayRowCounter = 0; ArrayRowCounter < InputArray.GetLength(0); ArrayRowCounter++)
            {
                pubTblElementLevelReportedinClashManager[ArrayRowCounter].Element = Convert.ToString(InputArray[ArrayRowCounter + 1, 1]);
            }
        }

        public static void CreateTblSectionProperties(object[,] InputArray, out TTblSectionProperties[] pubTblSectionProperties)
        {
            pubTblSectionProperties = new TTblSectionProperties[InputArray.GetLength(0)];

            for (int i = 0; i < InputArray.GetLength(0); i++)
            {
                pubTblSectionProperties[i] = new TTblSectionProperties
                {
                    ProfileStandard = Convert.ToString(InputArray[i + 1, 1]),
                    ProfileName = Convert.ToString(InputArray[i + 1, 2]),
                    ProfileFamily = Convert.ToString(InputArray[i + 1, 3]),
                    ProfileThreeDModelNm = Convert.ToString(InputArray[i + 1, 4]),
                    PreferredSctnFlag = Convert.ToBoolean(InputArray[i + 1, 5]),
                    Depth = Convert.ToSingle(InputArray[i + 1, 6]),
                    Width = Convert.ToSingle(InputArray[i + 1, 7]),
                    Weight = Convert.ToSingle(InputArray[i + 1, 8]),
                    Area = Convert.ToSingle(InputArray[i + 1, 9]),
                    MinAxisShearArea = Convert.ToSingle(InputArray[i + 1, 10]),
                    MajAxisShearArea = Convert.ToSingle(InputArray[i + 1, 11]),
                    MajAxisSecondMofA = Convert.ToSingle(InputArray[i + 1, 12]),
                    MinAxisSecondMofA = Convert.ToSingle(InputArray[i + 1, 13]),
                    TorsionConstant = Convert.ToSingle(InputArray[i + 1, 14]),
                    Rotation = Convert.ToSingle(InputArray[i + 1, 15]),
                    STAADSctnname = Convert.ToString(InputArray[i + 1, 16])
                };
            }
        }

        public static void CreateTblMaterialProperties(object[,] InputArray, out TTblMaterialProperties[] pubTblMaterialProperties)
        {
            pubTblMaterialProperties = new TTblMaterialProperties[InputArray.GetLength(0)];

            for (int i = 0; i < InputArray.GetLength(0); i++)
            {
                pubTblMaterialProperties[i] = new TTblMaterialProperties
                {
                    MatlRef = Convert.ToString(InputArray[i + 1, 1]),
                    MatlDesc = Convert.ToString(InputArray[i + 1, 2]),
                    ThreeDModelMatlDesc = Convert.ToString(InputArray[i + 1, 3]),
                    YoungsMod = Convert.ToSingle(InputArray[i + 1, 4]),
                    ShearMod = Convert.ToSingle(InputArray[i + 1, 5]),
                    AllowableStress = Convert.ToSingle(InputArray[i + 1, 6])
                };
            }
        }

        public static void CreateTblBeamJusLinetoFacesDef(object[,] InputArray, out TTblBeamJusLinetoFacesDef[] pubTblBeamJusLinetoFacesDef)
        {
            pubTblBeamJusLinetoFacesDef = new TTblBeamJusLinetoFacesDef[InputArray.GetLength(0)];

            for (int i = 0; i < InputArray.GetLength(0); i++)
            {
                pubTblBeamJusLinetoFacesDef[i] = new TTblBeamJusLinetoFacesDef
                {
                    BeamType = Convert.ToString(InputArray[i + 1, 1]),
                    Jusline = Convert.ToString(InputArray[i + 1, 2]),
                    FaceDesc = Convert.ToString(InputArray[i + 1, 3]),
                    DistanceinDepth = Convert.ToSingle(InputArray[i + 1, 4]),
                    DistanceinWidth = Convert.ToSingle(InputArray[i + 1, 5])
                };
            }
        }

        public static void CreateTblSDNFBeamTypetoSupABeamType(object[,] InputArray, out TTblSDNFBeamTypetoSupABeamType[] pubTblSDNFBeamTypetoSupABeamType)
        {
            pubTblSDNFBeamTypetoSupABeamType = new TTblSDNFBeamTypetoSupABeamType[InputArray.GetLength(0)];

            for (int i = 0; i < InputArray.GetLength(0); i++)
            {
                pubTblSDNFBeamTypetoSupABeamType[i] = new TTblSDNFBeamTypetoSupABeamType
                {
                    P3DType = Convert.ToString(InputArray[i + 1, 1]),
                    SupAType = Convert.ToString(InputArray[i + 1, 2])
                };
            }
        }

        public static float[] RoundCoordArr(float[] arrayToRound, int pubIntDiscretisationStepDecPlaces)
        {
            float[] roundedArr = new float[3];

            roundedArr[0] = RoundDecPlc(arrayToRound[0], pubIntDiscretisationStepDecPlaces);
            roundedArr[1] = RoundDecPlc(arrayToRound[1], pubIntDiscretisationStepDecPlaces);
            roundedArr[2] = RoundDecPlc(arrayToRound[2], pubIntDiscretisationStepDecPlaces);

            return roundedArr;
        }

        public static float RoundDecPlc(float numberToRound, int decPlacesToRound)
        {
            return (float)Math.Round(numberToRound, decPlacesToRound);
        }

        public static TTblBeamDetailing[] CreateTblBeamDetailing(object[,] inputArray)
        {
            int rowCount = inputArray.GetLength(0);
            TTblBeamDetailing[] newTblBeamDetailing = new TTblBeamDetailing[rowCount];

            for (int i = 0; i < rowCount; i++)
            {
                newTblBeamDetailing[i] = new TTblBeamDetailing
                {
                    ProfileFamily = Convert.ToString(inputArray[i, 0]),
                    BeamDir = Convert.ToString(inputArray[i, 1]),
                    BeamDirAndDesc = Convert.ToString(inputArray[i, 2]),
                    BeamStartorEnd = Convert.ToString(inputArray[i, 3]),
                    ConnTypeAtCurrEndofBeam = Convert.ToString(inputArray[i, 4]),
                    ExistingSteelFace = Convert.ToString(inputArray[i, 5]),
                    BeaminRelationtoAvPosofSupts = Convert.ToString(inputArray[i, 6]),
                    BeamAdjustpart1BMwidth = Convert.ToSingle(inputArray[i, 7]),
                    BeamAdjustpart1BMdepth = Convert.ToSingle(inputArray[i, 8]),
                    BeamAdjustpart2const = Convert.ToSingle(inputArray[i, 9]),
                    BeamAdjustpart1ESwidth = Convert.ToSingle(inputArray[i, 10]),
                    BeamAdjustpart1ESdepth = Convert.ToSingle(inputArray[i, 11]),
                    BeamTraninDirofPipeBMwidth = Convert.ToSingle(inputArray[i, 12]),
                    BeamTraninDirofPipeBMdepth = Convert.ToSingle(inputArray[i, 13]),
                    BeamTranPerpToPipeBMwidth = Convert.ToSingle(inputArray[i, 14]),
                    BeamTranPerpToPipeBMdepth = Convert.ToSingle(inputArray[i, 15]),
                    BeamTranUDBMdepth = Convert.ToSingle(inputArray[i, 16]),
                    Rotation = Convert.ToSingle(inputArray[i, 17]),
                    ExistingSteelBMDepth = Convert.ToSingle(inputArray[i, 18]),
                    ExistingSteelBMWidth = Convert.ToSingle(inputArray[i, 19]),
                    Jusline = Convert.ToSingle(inputArray[i, 20]),
                    CorrforEWPipesXofBeamDepth = Convert.ToSingle(inputArray[i, 21])
                };
            }

            return newTblBeamDetailing;
        }

        public static TTblConnDetailing[] CreateTblConnDetailing(object[,] inputArray)
        {
            TTblConnDetailing[] newTblConnDetailing = new TTblConnDetailing[inputArray.GetLength(0)];

            for (int arrayRowCounter = 0; arrayRowCounter < inputArray.GetLength(0); arrayRowCounter++)
            {
                newTblConnDetailing[arrayRowCounter] = new TTblConnDetailing
                {
                    ProfileFamily = Convert.ToString(inputArray[arrayRowCounter + 1, 0]),
                    ProfileFamilyConnBeam = Convert.ToString(inputArray[arrayRowCounter + 1, 1]),
                    BeamDirAndDesc = Convert.ToString(inputArray[arrayRowCounter + 1, 2]),
                    BeamStartorEnd = Convert.ToString(inputArray[arrayRowCounter + 1, 3]),
                    ConnTypeAtCurrEndofBeam = Convert.ToString(inputArray[arrayRowCounter + 1, 4]),
                    ExistingSteelFace = Convert.ToString(inputArray[arrayRowCounter + 1, 5]),
                    ReinforcingGussetReqd = Convert.ToString(inputArray[arrayRowCounter + 1, 6]),
                    BasePlateReqd = Convert.ToString(inputArray[arrayRowCounter + 1, 7])
                };
            }

            return newTblConnDetailing;
        }

        public static TTblBasePlateDef[] CreateTblBasePlateDef(object[,] inputArray)
        {
            TTblBasePlateDef[] newTblBasePlateDef = new TTblBasePlateDef[inputArray.GetLength(0)];

            for (int arrayRowCounter = 0; arrayRowCounter < inputArray.GetLength(0); arrayRowCounter++)
            {
                newTblBasePlateDef[arrayRowCounter] = new TTblBasePlateDef
                {
                    ProfileStandard = Convert.ToString(inputArray[arrayRowCounter + 1, 0]),
                    ProfileName = Convert.ToString(inputArray[arrayRowCounter + 1, 1]),
                    ProfileFamily = Convert.ToString(inputArray[arrayRowCounter + 1, 2]),
                    ProfileThreeDModelNm = Convert.ToString(inputArray[arrayRowCounter + 1, 3]),
                    Depth = Convert.ToSingle(inputArray[arrayRowCounter + 1, 4]),
                    Width = Convert.ToSingle(inputArray[arrayRowCounter + 1, 5]),
                    BasePlateThk = Convert.ToSingle(inputArray[arrayRowCounter + 1, 6]),
                    BasePlateWidth = Convert.ToSingle(inputArray[arrayRowCounter + 1, 7]),
                    BasePlateLength = Convert.ToSingle(inputArray[arrayRowCounter + 1, 8]),
                    BasePlateBoltingReqd = Convert.ToString(inputArray[arrayRowCounter + 1, 9]),
                    BoltSpacing = Convert.ToSingle(inputArray[arrayRowCounter + 1, 10]),
                    BoltSize = Convert.ToSingle(inputArray[arrayRowCounter + 1, 11]),
                    PlinthThk = Convert.ToSingle(inputArray[arrayRowCounter + 1, 12]),
                    PlinthWidth = Convert.ToSingle(inputArray[arrayRowCounter + 1, 13]),
                    PlinthLength = Convert.ToSingle(inputArray[arrayRowCounter + 1, 14])
                };
            }

            return newTblBasePlateDef;
        }

        public static TTblSuptSpanRules[] CreateTblSuptSpanRules(object[,] inputArray)
        {
            TTblSuptSpanRules[] newTblSuptSpanRules = new TTblSuptSpanRules[inputArray.GetLength(0)];

            for (int arrayRowCounter = 0; arrayRowCounter < inputArray.GetLength(0); arrayRowCounter++)
            {
                newTblSuptSpanRules[arrayRowCounter] = new TTblSuptSpanRules
                {
                    PipeBorOrRackSize = Convert.ToInt16(inputArray[arrayRowCounter + 1, 0]),
                    MaxSuptDistfromBends = Convert.ToSingle(inputArray[arrayRowCounter + 1, 1]),
                    MaxDistBetweenBendsWoutSupt = Convert.ToSingle(inputArray[arrayRowCounter + 1, 2]),
                    MaxSpan = Convert.ToSingle(inputArray[arrayRowCounter + 1, 3]),
                    MidPointSuptReqdonBend = Convert.ToBoolean(inputArray[arrayRowCounter + 1, 4]),
                    RuleDiscApplicability = Convert.ToString(inputArray[arrayRowCounter + 1, 5]),
                    MinSuptDistfromElbo = Convert.ToSingle(inputArray[arrayRowCounter + 1, 6]),
                    MinSpanBtwnSuptsRatio = Convert.ToSingle(inputArray[arrayRowCounter + 1, 7]),
                    OptionBSuptsforAllDirChanges = Convert.ToBoolean(inputArray[arrayRowCounter + 1, 8]),
                    MaxSuptDistfromHeaderEqSizeBra = Convert.ToSingle(inputArray[arrayRowCounter + 1, 9]),
                    MaxSuptDistfromHeaderRedSizeBra = Convert.ToSingle(inputArray[arrayRowCounter + 1, 10]),
                    MaxSuptDistfromUnrestrainedEnd = Convert.ToSingle(inputArray[arrayRowCounter + 1, 11]),
                    PipeOD = Convert.ToSingle(inputArray[arrayRowCounter + 1, 12]),
                    ShoeHeight = Convert.ToSingle(inputArray[arrayRowCounter + 1, 13]),
                    SuptSteelMinWidthReqd = Convert.ToSingle(inputArray[arrayRowCounter + 1, 14])
                };
            }

            return newTblSuptSpanRules;
        }

        public static TTblSuptScoreCat[] CreateTblSuptScoreCat(object[,] inputArray)
        {
            TTblSuptScoreCat[] newTblSuptScoreCat = new TTblSuptScoreCat[inputArray.GetLength(0)];

            for (int arrayRowCounter = 0; arrayRowCounter < inputArray.GetLength(0); arrayRowCounter++)
            {
                newTblSuptScoreCat[arrayRowCounter] = new TTblSuptScoreCat
                {
                    MaxValforPointScore = Convert.ToSingle(inputArray[arrayRowCounter + 1, 0]),
                    MinValforPointScore = Convert.ToSingle(inputArray[arrayRowCounter + 1, 1]),
                    CodeAction = Convert.ToString(inputArray[arrayRowCounter + 1, 2]),
                    PointCat = Convert.ToInt16(inputArray[arrayRowCounter + 1, 3])
                };
            }

            return newTblSuptScoreCat;
        }
    }
}
