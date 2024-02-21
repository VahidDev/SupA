using SupA.Lib.Core;

namespace SupA.Lib.DataManipulation
{
    public class mRedEntireStruModtoLocal
    {
        public static void RedEntireStruModtoLocal(List<cSteel> collExistingSteel, List<cSuptPoints> collSuptPointsinArea)
        {
            cSteel existingSteel;
            cSuptPoints suptPoint;
            double chkcoordE, chkcoordN, chkcoordU;
            bool keepStl;
            int i, j;

            // Work through all the steels
            i = 0;
            while (i < collExistingSteel.Count)
            {
                existingSteel = collExistingSteel[i];
                // Initially set to false; if it never becomes true, then the steel doesn't
                // run close to any supports, and we can delete it
                keepStl = false;

                if (existingSteel.ModelName == "Import File Line 74")
                {
                    // Placeholder for debugging or specific logic
                    // TestVar = TestVar;
                }

                // Work along the entire length of each beam to decide whether to keep it
                j = 0;
                while (j <= Math.Abs(existingSteel.Length / 1000))
                {
                    chkcoordE = j * (existingSteel.EndE - existingSteel.StartE) / Math.Abs(existingSteel.Length / 1000) + existingSteel.StartE;
                    chkcoordN = j * (existingSteel.EndN - existingSteel.StartN) / Math.Abs(existingSteel.Length / 1000) + existingSteel.StartN;
                    chkcoordU = j * (existingSteel.EndU - existingSteel.StartU) / Math.Abs(existingSteel.Length / 1000) + existingSteel.StartU;

                    foreach (var suptPointItem in collSuptPointsinArea)
                    {
                        if (Math.Abs(suptPointItem.EastingSuptPoint - chkcoordE) <= 4000 &&
                            Math.Abs(suptPointItem.NorthingSuptPoint - chkcoordN) <= 4000 &&
                            Math.Abs(suptPointItem.ElSuptPoint - chkcoordU) <= 5000)
                        {
                            keepStl = true;
                            break; // Exit the foreach loop
                        }
                    }

                    if (keepStl)
                    {
                        break; // Exit the while loop
                    }

                    j++;
                }

                if (!keepStl)
                {
                    collExistingSteel.RemoveAt(i);
                }
                else
                {
                    i++;
                }
            }
        }
    }
}
