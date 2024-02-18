using SupA.Lib.Core;

namespace SupA.Lib.FEA
{
    public class mAssignSuptFrameSectionSizes
    {
        public void AssignSuptFrameSectionSizes(cPotlSupt frame, int noOfBeamGroups)
        {
            TTblSectionProperties[] tblPreferedSctnsProps = CreateTblPreferedSctnsProps();
            foreach (var beam in frame.BeamsinFrame)
            {
                // Implementation for each beam
            }
        }

        private TTblSectionProperties[] CreateTblPreferedSctnsProps()
        {
            int tblLen = 0;
            // Check how long to make tblPreferedSctnsProps
            foreach (var sectionProp in mDefineSuptFramesSections.pubTblSectionProperties)
            {
                if (sectionProp.PreferredSctnFlag)
                {
                    tblLen++;
                    if (sectionProp.MajAxisSecondMofA != sectionProp.MinAxisSecondMofA)
                    {
                        tblLen++;
                    }
                }
            }

            // Now populate tblPreferedSctnsProps
            var tblPreferedSctnsProps = new TTblSectionProperties[tblLen];
            tblLen = 0;
            foreach (var sectionProp in mDefineSuptFramesSections.pubTblSectionProperties)
            {
                if (sectionProp.PreferredSctnFlag)
                {
                    tblPreferedSctnsProps[tblLen] = sectionProp;
                    tblLen++;
                    if (sectionProp.MajAxisSecondMofA != sectionProp.MinAxisSecondMofA)
                    {
                        var newSectionProp = new TTblSectionProperties
                        {
                            MajAxisSecondMofA = sectionProp.MinAxisSecondMofA,
                            MinAxisSecondMofA = sectionProp.MajAxisSecondMofA,
                            MajAxisShearArea = sectionProp.MinAxisShearArea,
                            MinAxisShearArea = sectionProp.MajAxisShearArea,
                            Width = sectionProp.Width,
                            Depth = sectionProp.Depth,
                            Rotation = 90,
                            STAADSctnname = sectionProp.STAADSctnname
                        };
                        tblPreferedSctnsProps[tblLen] = newSectionProp;
                        tblLen++;
                    }
                }
            }

            // Sort the array by weight
            Array.Sort(tblPreferedSctnsProps, (x, y) => x.MajAxisSecondMofA.CompareTo(y.MajAxisSecondMofA)); // Assuming sorting by MajAxisSecondMofA as a placeholder for weight
            return tblPreferedSctnsProps;
        }
    }
}
