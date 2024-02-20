using SupA.Lib.Core;

namespace SupA.Lib.DataManipulation
{
    public class mSelectBestSuptFrame
    {
        public void SelectBestSuptFrame(List<cPotlSupt> collPotlSuptFrameDetails)
        {
            collPotlSuptFrameDetails = collPotlSuptFrameDetails.OrderBy(x => x.TotalCost).ToList();

            int i = 1;
            foreach (var frame in collPotlSuptFrameDetails)
            {
                frame.FrameRanking = i++;
            }

            foreach (var potlSuptFrameLL1 in collPotlSuptFrameDetails)
            {
                bool invalidTrim = false;
                if (potlSuptFrameLL1.PotlSuptNo == "potlsupt1437")
                {
                    // Debug or logging point
                }

                foreach (var beam in potlSuptFrameLL1.BeamsinFrameratlized)
                {
                    int noOfConnstoExistingSteel = 0;
                    foreach (var groupNode in potlSuptFrameLL1.GroupNodesinFrame)
                    {
                        if (beam.SuptSteelFunction == "trim" &&
                            (beam.ExistingSteelConnNameEnd == groupNode.GroupName || beam.ExistingSteelConnNameStart == groupNode.GroupName) &&
                            !string.IsNullOrEmpty(groupNode.AssocExistingSteel))
                        {
                            noOfConnstoExistingSteel++;
                        }
                    }

                    invalidTrim = noOfConnstoExistingSteel switch
                    {
                        1 => true,
                        2 => false,
                        _ => invalidTrim
                    };
                }

                potlSuptFrameLL1.PreferredOption = invalidTrim ? "nonpreferred" : "preferred";
            }
        }
    }
}
