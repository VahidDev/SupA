using SupA.Lib.Core;
using SupA.Lib.FEA;

namespace SupA.Lib.DataManipulation
{
    public class mSelectBestPotlSupt
    {
        private static List<TTblSectionProperties> TblPreferredSctnsProps; // Assuming this list is initialized elsewhere

        public static void SelectBestPotlSupt(List<TTblSectionProperties> collAcceptableSctnsProps, cPotlSupt frame)
        {
            float snglLightestWeight = 999999;
            TTblSectionProperties tblLightestSctnProp = null;

            foreach (var accSctnProp in collAcceptableSctnsProps)
            {
                var preferredSctnProp = InitPreferredSctnProp(accSctnProp.STAADSctnname);

                frame.WeightCalculated = 0;
                foreach (var beam in frame.BeamsinFrame)
                {
                    WriteSctnPropToBeam(beam, preferredSctnProp);
                    frame.WeightCalculated += (float)beam.Length * (float)preferredSctnProp.Weight;
                }

                if (frame.WeightCalculated <= snglLightestWeight)
                {
                    snglLightestWeight = frame.WeightCalculated;
                    tblLightestSctnProp = preferredSctnProp;
                }
            }

            if (tblLightestSctnProp != null)
            {
                frame.WeightCalculated = 0;
                foreach (var beam in frame.BeamsinFrame)
                {
                    WriteSctnPropToBeam(beam, tblLightestSctnProp);
                    frame.WeightCalculated += (float)beam.Length * (float)tblLightestSctnProp.Weight;
                }
            }
        }

        private static TTblSectionProperties InitPreferredSctnProp(string currentSection)
        {
            return TblPreferredSctnsProps.FirstOrDefault(x => x.STAADSctnname == currentSection);
        }

        private static void WriteSctnPropToBeam(cSteel beam, TTblSectionProperties preferredSctnProp)
        {
            beam.MemType = preferredSctnProp.ProfileName;
            beam.MemTypeGeneric = preferredSctnProp.ProfileFamily;
            beam.MemTypeModelRef = preferredSctnProp.ProfileThreeDModelNm;
            beam.SctnDepth = (float)preferredSctnProp.Depth;
            beam.SctnWidth = (float)preferredSctnProp.Width;
            beam.STAADSctnName = preferredSctnProp.STAADSctnname;
            beam.UnitWeightKgm = (float)(preferredSctnProp.Weight * 1000);
        }
    }
}
