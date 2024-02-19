using SupA.Lib.Core;

namespace SupA.Lib.DataManipulation
{
    public class mCreateCoordArray
    {
        public static float[] CreateCoordArray(object coordOwningObject, string baseClass, string coordDesc = "", int perpDirNo = 0, int perpAxis1No = 0, int perpAxis2No = 0, float[] modificationArray = null, bool rounded = false)
        {
            float[] arrToReturn = new float[3];
            float coordToReturn = 0;
            int coordCol = 0;

            // Set modification array to {0, 0, 0} if it's null
            if (modificationArray == null)
            {
                modificationArray = new float[3];
            }

            // Return coordinates based on base class and coordinate description
            if (baseClass == "cSteel" && coordDesc.ToLower() == "start")
            {
                // Assuming CoordOwningObject properties are accessible directly
                arrToReturn[0] = ((cSteel)coordOwningObject).StartE + modificationArray[0];
                arrToReturn[1] = ((cSteel)coordOwningObject).StartN + modificationArray[1];
                arrToReturn[2] = ((cSteel)coordOwningObject).StartU + modificationArray[2];
            }
            else if (baseClass == "cSteel" && coordDesc.ToLower() == "end")
            {
                // Assuming CoordOwningObject properties are accessible directly
                arrToReturn[0] = ((cSteel)coordOwningObject).EndE + modificationArray[0];
                arrToReturn[1] = ((cSteel)coordOwningObject).EndN + modificationArray[1];
                arrToReturn[2] = ((cSteel)coordOwningObject).EndU + modificationArray[2];
            }
            else if (baseClass == "cSuptPoints")
            {
                // Assuming CoordOwningObject properties are accessible directly
                arrToReturn[0] = ((cSuptPoints)coordOwningObject).EastingSuptPoint + modificationArray[0];
                arrToReturn[1] = ((cSuptPoints)coordOwningObject).NorthingSuptPoint + modificationArray[1];
                arrToReturn[2] = ((cSuptPoints)coordOwningObject).ElSuptPoint + modificationArray[2];
                if (coordDesc.ToLower() == "tos")
                {
                    arrToReturn[2] = ((cSuptPoints)coordOwningObject).TOSPerpDir2neg;
                }
                if (((cSuptPoints)coordOwningObject).Tubidir == "U" || ((cSuptPoints)coordOwningObject).Tubidir == "D")
                {
                    Console.WriteLine("The above logic needs to be expanded to cover vertical supports");
                }

                // Convert support centre line to support bottom of shoe
                if (perpDirNo == 1)
                {
                    coordToReturn = ((cSuptPoints)coordOwningObject).TOSPerpDir1pos;
                    coordCol = perpAxis1No;
                }
                else if (perpDirNo == 2)
                {
                    coordToReturn = ((cSuptPoints)coordOwningObject).TOSPerpDir1neg;
                    coordCol = perpAxis1No;
                }
                else if (perpDirNo == 3)
                {
                    coordToReturn = ((cSuptPoints)coordOwningObject).TOSPerpDir2pos;
                    coordCol = perpAxis2No;
                }
                else if (perpDirNo == 4)
                {
                    coordToReturn = ((cSuptPoints)coordOwningObject).TOSPerpDir2neg;
                    coordCol = perpAxis2No;
                }
                if (coordCol != 0)
                {
                    arrToReturn[coordCol - 1] = coordToReturn;
                }
            }
            else if (baseClass == "cSteelDisc" || baseClass == "cRouteNode" || baseClass == "cClashData")
            {
                // Assuming CoordOwningObject properties are accessible directly
                arrToReturn[0] = ((cRouteNode)coordOwningObject).Easting + modificationArray[0];
                arrToReturn[1] = ((cRouteNode)coordOwningObject).Northing + modificationArray[1];
                arrToReturn[2] = ((cRouteNode)coordOwningObject).Upping + modificationArray[2];
            }
            else if (baseClass == "coordarray")
            {
                // Assuming CoordOwningObject is a float array
                for (int i = 0; i < 3; i++)
                {
                    arrToReturn[i] = ((float[])coordOwningObject)[i] + modificationArray[i];
                }
            }
            else
            {
                Console.WriteLine("Error");
            }

            // Round the coordinates if required
            if (rounded)
            {
                int pubIntDiscretisationStepDecPlaces = 2; // Assuming this value is accessible
                for (int i = 0; i < 3; i++)
                {
                    arrToReturn[i] = (float)Math.Round(arrToReturn[i], pubIntDiscretisationStepDecPlaces);
                }
            }

            return arrToReturn;
        }
    }
}
