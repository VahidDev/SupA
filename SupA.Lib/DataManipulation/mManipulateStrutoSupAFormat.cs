using SupA.Lib.CoordinateAndAngleManipulation;
using SupA.Lib.Core;
using SupA.Lib.Initialization;

namespace SupA.Lib.DataManipulation
{
    public class mManipulateStrutoSupAFormat
    {
        public static List<cSteel> ManipulateStrutoSupAFormat(List<cSteel> CollExistingSteel, string ThreeDModelSoftware)
        {
            List<cSteel> CollExistingSteelFaceLines = new List<cSteel>();

            if (ThreeDModelSoftware == "E3D" || ThreeDModelSoftware == "P3D")
            {
                foreach (cSteel Existingsteel in CollExistingSteel)
                {
                    CallDatafromTblSectionProperties(Existingsteel);

                    for (int LL2 = 1; LL2 <= mSubInitializationSupA.pubTblBeamJusLinetoFacesDef.GetUpperBound(0); LL2++)
                    {
                        if (mSubInitializationSupA.pubTblBeamJusLinetoFacesDef[LL2].BeamType == Existingsteel.MemTypeGeneric && mSubInitializationSupA.pubTblBeamJusLinetoFacesDef[LL2].Jusline == Existingsteel.Jusline)
                        {
                            if (!((mSubInitializationSupA.pubTblBeamJusLinetoFacesDef[LL2].BeamType == "IBEAM" && mSubInitializationSupA.pubTblBeamJusLinetoFacesDef[LL2].FaceDesc == "LEFTC" && (Existingsteel.Dir != "U" && Existingsteel.Dir != "D")) ||
                                  (mSubInitializationSupA.pubTblBeamJusLinetoFacesDef[LL2].BeamType == "IBEAM" && mSubInitializationSupA.pubTblBeamJusLinetoFacesDef[LL2].FaceDesc == "RIGHTC" && (Existingsteel.Dir != "U" && Existingsteel.Dir != "D"))))
                            {
                                cSteel ExistingSteelFacelines = new cSteel();
                                ExistingSteelFacelines.CopyClassInstance(Existingsteel);
                                ExistingSteelFacelines.FeatureDesc = mSubInitializationSupA.pubTblBeamJusLinetoFacesDef[LL2].FaceDesc;
                                ExistingSteelFacelines.BeamNo = CollExistingSteelFaceLines.Count + 1;
                                ExistingSteelFacelines.SuptSteelFunction = "existingsteel";
                                SetFaceLineCoordinates(ExistingSteelFacelines, mSubInitializationSupA.pubTblBeamJusLinetoFacesDef[LL2].DistanceinWidth, mSubInitializationSupA.pubTblBeamJusLinetoFacesDef[LL2].DistanceinDepth);

                                CollExistingSteelFaceLines.Add(ExistingSteelFacelines);
                            }
                        }
                    }
                }
            }

            return CollExistingSteelFaceLines;
        }

        public static void CallDatafromTblSectionProperties(cSteel Existingsteel)
        {
            for (int ArrayRowCounter = 1; ArrayRowCounter <= mSubInitializationSupA.pubTblSectionProperties.GetUpperBound(0); ArrayRowCounter++)
            {
                if (mSubInitializationSupA.pubTblSectionProperties[ArrayRowCounter].ProfileThreeDModelNm == Existingsteel.MemTypeModelRef)
                {
                    Existingsteel.MemTypeGeneric = mSubInitializationSupA.pubTblSectionProperties[ArrayRowCounter].ProfileFamily;
                    Existingsteel.MemType = mSubInitializationSupA.pubTblSectionProperties[ArrayRowCounter].ProfileName;
                    Existingsteel.SctnDepth = mSubInitializationSupA.pubTblSectionProperties[ArrayRowCounter].Depth;
                    Existingsteel.SctnWidth = mSubInitializationSupA.pubTblSectionProperties[ArrayRowCounter].Width;
                    Existingsteel.STAADSctnName = mSubInitializationSupA.pubTblSectionProperties[ArrayRowCounter].STAADSctnname;
                }
            }
        }

        public static void SetFaceLineCoordinates(cSteel ExistingSteelFacelines, float FactortoTravelWidth, float FactortoTravelDepth)
        {
            float[] CoordinatestoAdjustMinorAxis = mDecomposeThreeDVectorintoENUCoords.DecomposeThreeDVectorintoENUCoords(ExistingSteelFacelines.MinorAxisGlobaldir, ExistingSteelFacelines.SctnDepth * FactortoTravelDepth);
            float[] CoordinatestoAdjustMajorAxis = mDecomposeThreeDVectorintoENUCoords.DecomposeThreeDVectorintoENUCoords(ExistingSteelFacelines.MajorAxisGlobaldir, ExistingSteelFacelines.SctnWidth * FactortoTravelWidth);

            ExistingSteelFacelines.StartE += CoordinatestoAdjustMinorAxis[0] + CoordinatestoAdjustMajorAxis[0];
            ExistingSteelFacelines.StartN += CoordinatestoAdjustMinorAxis[1] + CoordinatestoAdjustMajorAxis[1];
            ExistingSteelFacelines.StartU += CoordinatestoAdjustMinorAxis[2] + CoordinatestoAdjustMajorAxis[2];
            ExistingSteelFacelines.EndE += CoordinatestoAdjustMinorAxis[0] + CoordinatestoAdjustMajorAxis[0];
            ExistingSteelFacelines.EndN += CoordinatestoAdjustMinorAxis[1] + CoordinatestoAdjustMajorAxis[1];
            ExistingSteelFacelines.EndU += CoordinatestoAdjustMinorAxis[2] + CoordinatestoAdjustMajorAxis[2];

            ExistingSteelFacelines.StartERounded = mPublicVarDefinitions.RoundDecPlc(ExistingSteelFacelines.StartE, mSubInitializationSupA.pubIntDiscretisationStepDecPlaces);
            ExistingSteelFacelines.StartNRounded = mPublicVarDefinitions.RoundDecPlc(ExistingSteelFacelines.StartN, mSubInitializationSupA.pubIntDiscretisationStepDecPlaces);
            ExistingSteelFacelines.StartURounded = mPublicVarDefinitions.RoundDecPlc(ExistingSteelFacelines.StartU, mSubInitializationSupA.pubIntDiscretisationStepDecPlaces);
            ExistingSteelFacelines.EndERounded = mPublicVarDefinitions.RoundDecPlc(ExistingSteelFacelines.EndE, mSubInitializationSupA.pubIntDiscretisationStepDecPlaces);
            ExistingSteelFacelines.EndNRounded = mPublicVarDefinitions.RoundDecPlc(ExistingSteelFacelines.EndN, mSubInitializationSupA.pubIntDiscretisationStepDecPlaces);
            ExistingSteelFacelines.EndURounded = mPublicVarDefinitions.RoundDecPlc(ExistingSteelFacelines.EndU, mSubInitializationSupA.pubIntDiscretisationStepDecPlaces);
        }
    }
}
