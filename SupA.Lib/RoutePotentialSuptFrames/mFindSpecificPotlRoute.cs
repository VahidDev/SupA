using SupA.Lib.Core;

namespace SupA.Lib.RoutePotentialSuptFrames
{
    public class mFindSpecificPotlRoute
    {
        public static bool FindSpecificPotlRoute(cPotlSupt PotlSupttoChk)
        {
            List<string> FrameDef = new List<string>
            {
                "GroupBeam14",
                "GroupBeam16",
                "GroupBeam18",
                "GroupBeam23",
                "GroupBeam19",
                "GroupBeam20"
            };

            List<string> beamsToRemove = new List<string>();
            List<cSteel> beamsToRemoveTmp = new List<cSteel>();

            foreach (var mem in FrameDef)
            {
                foreach (var mem2 in PotlSupttoChk.BeamsinFrame)
                {
                    if (mem == mem2.ModelName)
                    {
                        beamsToRemove.Add(mem);
                        beamsToRemoveTmp.Add(mem2);
                        break;
                    }
                }
            }

            foreach (var mem in beamsToRemove)
            {
                FrameDef.Remove(mem);
            }

            foreach (var mem2 in beamsToRemoveTmp)
            {
                PotlSupttoChk.BeamsinFrame.Remove(mem2);
            }

            if (FrameDef.Count == 0 && PotlSupttoChk.BeamsinFrame.Count == 0 && PotlSupttoChk.PathsUnTravelledCount == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
