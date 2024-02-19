using SupA.Lib.Initialization;

namespace SupA.Lib.DataManipulation
{
    public class mCoordinateCheck
    {
        public static bool CoordinateCheck(string runOption, float[] coordArrayToCheck, float[] coordArrayToCheckAgainstOne = null, float[] coordArrayToCheckAgainstTwo = null, float[] coordArrayToCheckTwo = null)
        {
            bool coordinateCheck = false;

            if (runOption == "withinbounds")
            {
                if ((coordArrayToCheckAgainstOne[0] >= coordArrayToCheck[0] && coordArrayToCheck[0] >= coordArrayToCheckAgainstTwo[0] ||
                     coordArrayToCheckAgainstOne[0] <= coordArrayToCheck[0] && coordArrayToCheck[0] <= coordArrayToCheckAgainstTwo[0]) &&
                    (coordArrayToCheckAgainstOne[1] >= coordArrayToCheck[1] && coordArrayToCheck[1] >= coordArrayToCheckAgainstTwo[1] ||
                     coordArrayToCheckAgainstOne[1] <= coordArrayToCheck[1] && coordArrayToCheck[1] <= coordArrayToCheckAgainstTwo[1]) &&
                    (coordArrayToCheckAgainstOne[2] >= coordArrayToCheck[2] && coordArrayToCheck[2] >= coordArrayToCheckAgainstTwo[2] ||
                     coordArrayToCheckAgainstOne[2] <= coordArrayToCheck[2] && coordArrayToCheck[2] <= coordArrayToCheckAgainstTwo[2]))
                {
                    coordinateCheck = true;
                }
            }
            else if (runOption == "PointAlongLine")
            {
                int countOfMatchingCoords = 0;

                for (int i = 0; i < 3; i++)
                {
                    if (coordArrayToCheck[i] == coordArrayToCheckAgainstOne[i] && coordArrayToCheck[i] == coordArrayToCheckAgainstTwo[i])
                    {
                        countOfMatchingCoords++;
                    }
                }

                if (countOfMatchingCoords >= 2)
                {
                    int lineParlAxisNo = -1;

                    for (int i = 0; i < 3; i++)
                    {
                        if (coordArrayToCheck[i] == coordArrayToCheckAgainstOne[i] && coordArrayToCheck[i] == coordArrayToCheckAgainstTwo[i])
                        {
                            lineParlAxisNo = i;
                            break;
                        }
                    }

                    if (lineParlAxisNo != -1)
                    {
                        float higherCAgainst = Math.Max(coordArrayToCheckAgainstOne[lineParlAxisNo], coordArrayToCheckAgainstTwo[lineParlAxisNo]);
                        float lowerCAgainst = Math.Min(coordArrayToCheckAgainstOne[lineParlAxisNo], coordArrayToCheckAgainstTwo[lineParlAxisNo]);

                        if (coordArrayToCheck[lineParlAxisNo] >= lowerCAgainst && coordArrayToCheck[lineParlAxisNo] <= higherCAgainst)
                        {
                            coordinateCheck = true;
                        }
                    }
                }
            }
            else if (runOption == "tocheckwithinlineagainst")
            {
                if (coordArrayToCheck[0] == coordArrayToCheckAgainstOne[0] &&
                    coordArrayToCheck[1] == coordArrayToCheckAgainstOne[1] &&
                    coordArrayToCheck[0] == coordArrayToCheckAgainstTwo[0] &&
                    coordArrayToCheck[1] == coordArrayToCheckAgainstTwo[1] &&
                    coordArrayToCheck[0] == coordArrayToCheckTwo[0] &&
                    coordArrayToCheck[1] == coordArrayToCheckTwo[1])
                {
                    float higherCAgainst = Math.Max(coordArrayToCheckAgainstOne[2], coordArrayToCheckAgainstTwo[2]);
                    float lowerCAgainst = Math.Min(coordArrayToCheckAgainstOne[2], coordArrayToCheckAgainstTwo[2]);
                    float higherCChk = Math.Max(coordArrayToCheck[2], coordArrayToCheckTwo[2]);
                    float lowerCChk = Math.Min(coordArrayToCheck[2], coordArrayToCheckTwo[2]);

                    if (higherCChk <= higherCAgainst && lowerCChk >= lowerCAgainst)
                    {
                        coordinateCheck = true;
                    }
                }
            }
            else if (runOption == "alongsingleaxis")
            {
                if ((coordArrayToCheckAgainstOne[0] >= coordArrayToCheck[0] && coordArrayToCheck[0] >= coordArrayToCheckAgainstTwo[0] ||
                     coordArrayToCheckAgainstOne[0] <= coordArrayToCheck[0] && coordArrayToCheck[0] <= coordArrayToCheckAgainstTwo[0]) &&
                    (coordArrayToCheckAgainstOne[1] >= coordArrayToCheck[1] && coordArrayToCheck[1] >= coordArrayToCheckAgainstTwo[1] ||
                     coordArrayToCheckAgainstOne[1] <= coordArrayToCheck[1] && coordArrayToCheck[1] <= coordArrayToCheckAgainstTwo[1]))
                {
                    coordinateCheck = true;
                }
            }
            else if (runOption == "nodepoint")
            {
                if (coordArrayToCheckAgainstOne[0] == coordArrayToCheck[0] &&
                    coordArrayToCheckAgainstOne[1] == coordArrayToCheck[1] &&
                    coordArrayToCheckAgainstOne[2] == coordArrayToCheck[2])
                {
                    coordinateCheck = true;
                }
            }
            else if (runOption == "tocheckhigherthanagainst")
            {
                if (coordArrayToCheckAgainstOne[0] == coordArrayToCheck[0] &&
                    coordArrayToCheckAgainstOne[1] == coordArrayToCheck[1] &&
                    coordArrayToCheckAgainstOne[2] <= coordArrayToCheck[2])
                {
                    coordinateCheck = true;
                }
            }
            else if (runOption == "tochecklowerthanagainst")
            {
                if (coordArrayToCheckAgainstOne[0] == coordArrayToCheck[0] &&
                    coordArrayToCheckAgainstOne[1] == coordArrayToCheck[1] &&
                    coordArrayToCheckAgainstOne[2] >= coordArrayToCheck[2])
                {
                    coordinateCheck = true;
                }
            }
            else if (runOption == "clashboxcheck")
            {
                float[] clashBoxLL = new float[3] { 0, -0.98f * mSubInitializationSupA.pubIntDiscretisationStepSize / 2, -0.98f * mSubInitializationSupA.pubIntDiscretisationStepSize / 2 };
                float[] clashBoxUL = new float[3] { 0, 0.98f * mSubInitializationSupA.pubIntDiscretisationStepSize / 2, 0.98f * mSubInitializationSupA.pubIntDiscretisationStepSize / 2 };

                bool checkInX = (clashBoxLL[0] <= coordArrayToCheckAgainstOne[0] && clashBoxUL[0] >= coordArrayToCheckAgainstTwo[0]) ||
                                (clashBoxLL[0] <= coordArrayToCheckAgainstTwo[0] && clashBoxUL[0] >= coordArrayToCheckAgainstOne[0]) ||
                                (clashBoxLL[0] >= coordArrayToCheckAgainstOne[0] && clashBoxLL[0] <= coordArrayToCheckAgainstTwo[0]) ||
                                (clashBoxUL[0] >= coordArrayToCheckAgainstOne[0] && clashBoxUL[0] <= coordArrayToCheckAgainstTwo[0]) ||
                                (clashBoxLL[0] <= coordArrayToCheckAgainstOne[0] && clashBoxLL[0] >= coordArrayToCheckAgainstTwo[0]) ||
                                (clashBoxUL[0] <= coordArrayToCheckAgainstOne[0] && clashBoxUL[0] >= coordArrayToCheckAgainstTwo[0]) ||
                                (clashBoxLL[0] >= coordArrayToCheckAgainstOne[0] && clashBoxLL[0] <= coordArrayToCheckAgainstTwo[0] &&
                                 clashBoxUL[0] >= coordArrayToCheckAgainstOne[0] && clashBoxUL[0] <= coordArrayToCheckAgainstTwo[0]) ||
                                (clashBoxLL[0] >= coordArrayToCheckAgainstTwo[0] && clashBoxLL[0] <= coordArrayToCheckAgainstOne[0] &&
                                 clashBoxUL[0] >= coordArrayToCheckAgainstTwo[0] && clashBoxUL[0] <= coordArrayToCheckAgainstOne[0]);

                bool checkInY = (clashBoxLL[1] <= coordArrayToCheckAgainstOne[1] && clashBoxUL[1] >= coordArrayToCheckAgainstOne[1]) ||
                                (clashBoxLL[1] <= coordArrayToCheckAgainstTwo[1] && clashBoxUL[1] >= coordArrayToCheckAgainstTwo[1]) ||
                                (clashBoxLL[1] >= coordArrayToCheckAgainstOne[1] && clashBoxLL[1] <= coordArrayToCheckAgainstTwo[1]) ||
                                (clashBoxUL[1] >= coordArrayToCheckAgainstOne[1] && clashBoxUL[1] <= coordArrayToCheckAgainstTwo[1]) ||
                                (clashBoxLL[1] <= coordArrayToCheckAgainstOne[1] && clashBoxLL[1] >= coordArrayToCheckAgainstTwo[1]) ||
                                (clashBoxUL[1] <= coordArrayToCheckAgainstOne[1] && clashBoxUL[1] >= coordArrayToCheckAgainstTwo[1]) ||
                                (clashBoxLL[1] >= coordArrayToCheckAgainstOne[1] && clashBoxLL[1] <= coordArrayToCheckAgainstTwo[1] &&
                                 clashBoxUL[1] >= coordArrayToCheckAgainstOne[1] && clashBoxUL[1] <= coordArrayToCheckAgainstTwo[1]) ||
                                (clashBoxLL[1] >= coordArrayToCheckAgainstTwo[1] && clashBoxLL[1] <= coordArrayToCheckAgainstOne[1] &&
                                 clashBoxUL[1] >= coordArrayToCheckAgainstTwo[1] && clashBoxUL[1] <= coordArrayToCheckAgainstOne[1]);

                bool checkInZ = (clashBoxLL[2] <= coordArrayToCheckAgainstOne[2] && clashBoxUL[2] >= coordArrayToCheckAgainstOne[2]) ||
                                (clashBoxLL[2] <= coordArrayToCheckAgainstTwo[2] && clashBoxUL[2] >= coordArrayToCheckAgainstTwo[2]) ||
                                (clashBoxLL[2] >= coordArrayToCheckAgainstOne[2] && clashBoxLL[2] <= coordArrayToCheckAgainstTwo[2]) ||
                                (clashBoxUL[2] >= coordArrayToCheckAgainstOne[2] && clashBoxUL[2] <= coordArrayToCheckAgainstTwo[2]) ||
                                (clashBoxLL[2] <= coordArrayToCheckAgainstOne[2] && clashBoxLL[2] >= coordArrayToCheckAgainstTwo[2]) ||
                                (clashBoxUL[2] <= coordArrayToCheckAgainstOne[2] && clashBoxUL[2] >= coordArrayToCheckAgainstTwo[2]) ||
                                (clashBoxLL[2] >= coordArrayToCheckAgainstOne[2] && clashBoxLL[2] <= coordArrayToCheckAgainstTwo[2] &&
                                 clashBoxUL[2] >= coordArrayToCheckAgainstOne[2] && clashBoxUL[2] <= coordArrayToCheckAgainstTwo[2]) ||
                                (clashBoxLL[2] >= coordArrayToCheckAgainstTwo[2] && clashBoxLL[2] <= coordArrayToCheckAgainstOne[2] &&
                                 clashBoxUL[2] >= coordArrayToCheckAgainstTwo[2] && clashBoxUL[2] <= coordArrayToCheckAgainstOne[2]);

                if (checkInX && checkInY && checkInZ)
                {
                    coordinateCheck = true;
                }
            }

            return coordinateCheck;
        }
    }
}
