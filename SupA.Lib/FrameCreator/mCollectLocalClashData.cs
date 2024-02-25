using SupA.Lib.CoordinateAndAngleManipulation;
using SupA.Lib.Core;
using SupA.Lib.DataManipulation;
using SupA.Lib.Initialization;
using SupA.Lib.Models;

namespace SupA.Lib.FrameCreator
{
    public class mCollectLocalClashData
    {
        public class CollectLocalClashDataResult
        {
            public List<cClashData> ClashDataCollection { get; set; }
        }

        public static CollectLocalClashDataResult CollectLocalClashData(List<cSuptPoints> collAdjacentSuptPoints, List<cClashData> collAllClashData, int suptGroupNo)
        {
            // Definition of all function specific private variables
            cSuptPoints supt;
            cClashData clashData;
            List<cClashData> coll = new List<cClashData>();
            float[] suptCoordArrLL;
            float[] suptCoordArrUL;
            float[] coordArrLL;
            float[] coordArrUL;
            // These define what directions are perpendicular and parallel to our pipe
            string perpAxis1;
            string perpAxis2;
            int perpAxis1No;
            int perpAxis2No;
            int parlAxisNo;
            mDefinePerpsandParls.DefinePerpsandParls(ref collAdjacentSuptPoints[0].Tubidir, out perpAxis1, out perpAxis2, out perpAxis1No, out perpAxis2No, out parlAxisNo);
            // Also define starting coordinates for our search box
            coordArrLL = mCreateCoordArray.CreateCoordArray(collAdjacentSuptPoints[0], "cSuptPoints");
            coordArrUL = mCreateCoordArray.CreateCoordArray(collAdjacentSuptPoints[0], "cSuptPoints");
            // Now work through all the supports in the group and identify what are the lower and upper limits of a box surrounding them
            foreach (var suptItem in collAdjacentSuptPoints)
            {
                suptCoordArrLL = mCreateCoordArray.CreateCoordArray(suptItem, "cSuptPoints", null, 0, 0, 0, mCollectLocalExistingDiscSteel.ReturnAdjStlSearchBoxDefinedinENU("negative", perpAxis1No, perpAxis2No, parlAxisNo));
                suptCoordArrUL = mCreateCoordArray.CreateCoordArray(suptItem, "cSuptPoints", null, 0, 0, 0, mCollectLocalExistingDiscSteel.ReturnAdjStlSearchBoxDefinedinENU("positive", perpAxis1No, perpAxis2No, parlAxisNo));
                coordArrLL[0] = Math.Min(suptCoordArrLL[0], coordArrLL[0]);
                coordArrLL[1] = Math.Min(suptCoordArrLL[1], coordArrLL[1]);
                coordArrLL[2] = Math.Min(suptCoordArrLL[2], coordArrLL[2]);
                coordArrUL[0] = Math.Max(suptCoordArrUL[0], coordArrUL[0]);
                coordArrUL[1] = Math.Max(suptCoordArrUL[1], coordArrUL[1]);
                coordArrUL[2] = Math.Max(suptCoordArrUL[2], coordArrUL[2]);
            }
            // Now add all discretised steel inside the limits defined in the previous loop into a local discretised steel collection
            foreach (var clashDataItem in collAllClashData)
            {
                if (mCoordinateCheck.CoordinateCheck("withinbounds", mCreateCoordArray.CreateCoordArray(clashDataItem, "cClashData"), coordArrLL, coordArrUL))
                {
                    coll.Add(clashDataItem);
                }
            }
            if (mSubInitializationSupA.pubBOOLTraceOn)
            {
                mExportColltoCSVFile<cClashData>.ExportColltoCSVFile(coll, "CollAllClashData-F" + (suptGroupNo + mSubInitializationSupA.SuptGroupNoMod).ToString(), "csv");
            }
            return new CollectLocalClashDataResult { ClashDataCollection = coll };
        }
    }
}
