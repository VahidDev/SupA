using SupA.Lib.Core;

namespace SupA.Lib.DataManipulation
{
    public class mRedEntireStruDiscModtoLocal
    {
        public static void RedEntireStruDiscModtoLocal(List<cSteelDisc> collExistingSteelDisc, List<cTubeDef> collPipeforSupporting)
        {
            cTubeDef existingTube;
            cSteelDisc discSteel;
            double chkcoordE, chkcoordN, chkcoordU;
            bool keepStl;
            int k, i;
            double discretCount;

            // Work through all of our discretised steel
            k = 0;
            while (k < collExistingSteelDisc.Count)
            {
                discSteel = collExistingSteelDisc[k];
                keepStl = false; // Initially false; if it never becomes true, then the steel doesn't run close to any pipes, and we can delete it

                // Work through all the pipe tubes
                for (i = 0; i < collPipeforSupporting.Count; i++)
                {
                    existingTube = collPipeforSupporting[i];

                    // Work along the entire length of each beam to decide whether to keep it
                    discretCount = existingTube.TubeLength / 1000;
                    for (int j = 0; j <= Math.Abs(discretCount); j++)
                    {
                        chkcoordE = j * (existingTube.LEast - existingTube.AEast) / discretCount + existingTube.AEast;
                        chkcoordN = j * (existingTube.LNorth - existingTube.ANorth) / discretCount + existingTube.ANorth;
                        chkcoordU = j * (existingTube.LUpping - existingTube.AUpping) / discretCount + existingTube.AUpping;

                        if (Math.Abs(discSteel.Easting - chkcoordE) <= 3000 &&
                            Math.Abs(discSteel.Northing - chkcoordN) <= 3000 &&
                            Math.Abs(discSteel.Upping - chkcoordU) <= 6000)
                        {
                            keepStl = true;
                            break; // Exit the for loop
                        }
                    }

                    if (keepStl)
                    {
                        break; // Exit the outer for loop if we decide to keep the steel
                    }
                }

                if (!keepStl)
                {
                    collExistingSteelDisc.RemoveAt(k); // Remove the item if it's not kept
                }
                else
                {
                    k++; // Only increment if we didn't remove the item, to avoid skipping elements
                }
            }
        }
    }
}
