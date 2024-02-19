using SupA.Lib.CoordinateAndAngleManipulation;
using SupA.Lib.Core;
using SupA.Lib.DataManipulation;
using SupA.Lib.Initialization;

namespace SupA.Lib.FrameCreator
{
    public class mCreateAdjacentSuptpointsArray
    {
        public static List<cSuptPoints> CreateAdjacentSuptpointsArray(ref int IntNoofSuptsGrouped, List<cSuptPoints> CollSuptPointsinArea, int SuptGroupNo)
        {
            // Definition of all function specific private variables
            List<cSuptPoints> SuptsCol = new List<cSuptPoints>();
            cSuptPoints StartingSupt = null;

            // These define what directions are perpendicular and parallel to our pipe
            string PerpAxis1, PerpAxis2;
            int PerpAxis1No, PerpAxis2No, ParlAxisNo;

            // First find our starting point which is the first support in CollSuptPointsinArea
            // that has not already been grouped into a frame
            foreach (cSuptPoints Supt in CollSuptPointsinArea)
            {
                if (!Supt.IncorpintoFrameFlag)
                {
                    Supt.IncorpintoFrameFlag = true;
                    Supt.AreaSuptSeqNumber = SuptGroupNo;
                    SuptsCol.Add(Supt);
                    StartingSupt = Supt;
                    IntNoofSuptsGrouped++;
                    break;
                }
            }

            // Lets define our perpendiculars and parallels for use in the search functions
            mDefinePerpsandParls.DefinePerpsandParls(ref StartingSupt.Tubidir, out PerpAxis1, out PerpAxis2, out PerpAxis1No, out PerpAxis2No, out ParlAxisNo);

            // Now that we have our starting point, look for any other points in the array which are close to this starting point.
            // We do this by looking for anything for any supports close to any supports already in our SuptsCol
            // We do this loop the number of times defined in TblDefinitionofAdjacentSupport(4)
            for (int LL1c = 1; LL1c <= mSubInitializationSupA.pubTblDefinitionofAdjacentSupport.NoofLoopIterations; LL1c++)
            {
                foreach (cSuptPoints Supt in CollSuptPointsinArea)
                {
                    // Look for any supports within the area bounded by pubTblDefinitionofAdjacentSupport values and add to group for supporting
                    // the criteria is: 1) hasn't been supported as part of a frame group.
                    // 2) co-ordinates are within the definition of pubTblDefinitionofAdjacentSupport
                    // 3) Tubidir directions match
                    if (!Supt.IncorpintoFrameFlag &&
                        mDirectionCheck.DirectionCheck(Supt.Tubidir, StartingSupt.Tubidir) == "parallel" &&
                        mCoordinateCheck.CoordinateCheck("withinbounds", mCreateCoordArray.CreateCoordArray(Supt, "cSuptPoints"),
                            mCreateCoordArray.CreateCoordArray(StartingSupt, "cSuptPoints", null, 0, 0, 0,
                                ReturnAdjSuptSearchBoxDefinedinENU("positive", PerpAxis1No, PerpAxis2No, ParlAxisNo)),
                            mCreateCoordArray.CreateCoordArray(StartingSupt, "cSuptPoints", null, 0, 0, 0,
                                ReturnAdjSuptSearchBoxDefinedinENU("negative", PerpAxis1No, PerpAxis2No, ParlAxisNo))))
                    {
                        Supt.IncorpintoFrameFlag = true;
                        Supt.AreaSuptSeqNumber = SuptGroupNo;
                        SuptsCol.Add(Supt);
                        IntNoofSuptsGrouped++;
                    }
                }
            }

            return SuptsCol;
        }

        public static float[] ReturnAdjSuptSearchBoxDefinedinENU(string LimitDirection, int PerpAxis1No, int PerpAxis2No, int ParlAxisNo)
        {
            // This function uses the public variable TblDefinitionofAdjacentSupport
            float[] CoordArray = new float[3];

            if (LimitDirection == "positive")
            {
                CoordArray[PerpAxis1No - 1] = mSubInitializationSupA.pubTblDefinitionofAdjacentSupport.DistPerp1toS;
                CoordArray[PerpAxis2No - 1] = mSubInitializationSupA.pubTblDefinitionofAdjacentSupport.DistPerp2toS;
                CoordArray[ParlAxisNo - 1] = mSubInitializationSupA.pubTblDefinitionofAdjacentSupport.DistParltoS;
            }
            else if (LimitDirection == "negative")
            {
                CoordArray[PerpAxis1No - 1] = -1 * mSubInitializationSupA.pubTblDefinitionofAdjacentSupport.DistPerp1toS;
                CoordArray[PerpAxis2No - 1] = -1 * mSubInitializationSupA.pubTblDefinitionofAdjacentSupport.DistPerp2toS;
                CoordArray[ParlAxisNo - 1] = -1 * mSubInitializationSupA.pubTblDefinitionofAdjacentSupport.DistParltoS;
            }
            else
            {
                // Handle error condition
                Console.WriteLine("Error");
            }

            return CoordArray;
        }
    }
}
