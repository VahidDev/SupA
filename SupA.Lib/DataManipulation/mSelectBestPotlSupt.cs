using SupA.Lib.Core;

namespace SupA.Lib.DataManipulation
{
    public class mSelectBestPotlSupt
    {
        private List<TTblSectionProperties> TblPreferredSctnsProps; // Assuming this list is initialized elsewhere

        public void SelectBestPotlSupt(List<TTblSectionProperties> collAcceptableSctnsProps, PotlSupt frame)
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
                    frame.WeightCalculated += beam.Length * preferredSctnProp.Weight;
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
                    frame.WeightCalculated += beam.Length * tblLightestSctnProp.Weight;
                }
            }
        }

        private TTblSectionProperties InitPreferredSctnProp(string currentSection)
        {
            return TblPreferredSctnsProps.FirstOrDefault(x => x.STAADSctnname == currentSection);
        }

        private void WriteSctnPropToBeam(cSteel beam, TTblSectionProperties preferredSctnProp)
        {
            beam.MemType = preferredSctnProp.ProfileName;
            beam.MemTypeGeneric = preferredSctnProp.ProfileFamily;
            beam.MemTypeModelRef = preferredSctnProp.ProfileThreeDModelNm;
            beam.SctnDepth = preferredSctnProp.Depth;
            beam.SctnWidth = preferredSctnProp.Width;
            beam.STAADSctnName = preferredSctnProp.STAADSctnname;
            beam.UnitWeightKgm = preferredSctnProp.Weight * 1000;
        }
    }
}
