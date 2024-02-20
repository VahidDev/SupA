using SupA.Lib.Core;

namespace SupA.Lib.DataManipulation
{
    public class mRedEntireStruModtoLocaltoTubi
    {
        public List<cSteel> RedEntireStruModtoLocaltoTubi(List<cSteel> collExistingSteel, cTubeDef tubiForLocalSteelColl)
        {
            cSteel existingSteel;
            cSuptPoints suptPoint; // Assuming this is used somewhere not shown in the snippet
            double stlchkcoordE, stlchkcoordN, stlchkcoordU;
            double tubichkcoordE, tubichkcoordN, tubichkcoordU;
            bool keepStl;

            List<cSteel> collExistingSteeltoRtn = new List<cSteel>();

            // Header row to keep the format working - assuming cSteel has a parameterless constructor
            cSteel headerrowsteel = new cSteel();
            collExistingSteeltoRtn.Add(headerrowsteel);

            // Iterating through all the steels
            for (int i = 0; i < collExistingSteel.Count; i++)
            {
                existingSteel = collExistingSteel[i];
                keepStl = false; // Reset for each steel

                // Work along the entire length of each beam
                for (int j = 0; j <= Math.Abs(existingSteel.Length / 1000); j++)
                {
                    if (existingSteel.Length != 0)
                    {
                        stlchkcoordE = j * (existingSteel.EndE - existingSteel.StartE) / Math.Abs(existingSteel.Length / 1000) + existingSteel.StartE;
                        stlchkcoordN = j * (existingSteel.EndN - existingSteel.StartN) / Math.Abs(existingSteel.Length / 1000) + existingSteel.StartN;
                        stlchkcoordU = j * (existingSteel.EndU - existingSteel.StartU) / Math.Abs(existingSteel.Length / 1000) + existingSteel.StartU;

                        // Work along the entire length of the tube
                        for (int k = 0; k <= Math.Abs(tubiForLocalSteelColl.TubeLength / 1000); k++)
                        {
                            tubichkcoordE = k * (tubiForLocalSteelColl.LEast - tubiForLocalSteelColl.AEast) / Math.Abs(tubiForLocalSteelColl.TubeLength / 1000) + tubiForLocalSteelColl.AEast;
                            tubichkcoordN = k * (tubiForLocalSteelColl.LNorth - tubiForLocalSteelColl.ANorth) / Math.Abs(tubiForLocalSteelColl.TubeLength / 1000) + tubiForLocalSteelColl.ANorth;
                            tubichkcoordU = k * (tubiForLocalSteelColl.LUpping - tubiForLocalSteelColl.AUpping) / Math.Abs(tubiForLocalSteelColl.TubeLength / 1000) + tubiForLocalSteelColl.AUpping;

                            if (Math.Abs(tubichkcoordE - stlchkcoordE) <= 4000 && Math.Abs(tubichkcoordN - stlchkcoordN) <= 4000 && Math.Abs(tubichkcoordU - stlchkcoordU) <= 5000)
                            {
                                keepStl = true;
                                break; // Exit the loop early if condition is met
                            }
                        }

                        if (keepStl)
                        {
                            collExistingSteeltoRtn.Add(existingSteel);
                            break; // Exit the loop early if steel is kept
                        }
                    }
                }
            }

            return collExistingSteeltoRtn;
        }
    }
}
