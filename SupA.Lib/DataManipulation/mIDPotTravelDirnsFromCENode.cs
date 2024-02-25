using SupA.Lib.Core;

namespace SupA.Lib.DataManipulation
{
    public class mIDPotTravelDirnsFromCENode
    {
        public static void IDPotTravelDirnsFromCENode(object[,] arrDirnstoAttempt, List<cGroupNode> collPotlSuptFrameDetails, cPotlSupt currPotlSuptFrame, List<cGroupNode> collFrameNodeMapGrouped)
        {
            cGroupNode groupNodeForCheck;
            string groupNodetoReturn;
            cRouteNode routeNodeLL4;
            bool connInDir;
            bool existingSteelConn;
            string searchNode;

            int ll2C;
            string ll2Dir;

            for (ll2C = 0; ll2C <= arrDirnstoAttempt.GetUpperBound(0); ll2C++)
            {
                ll2Dir = (string)arrDirnstoAttempt[ll2C, 1];

                groupNodeForCheck = currPotlSuptFrame.PathsUnTravelled[0];
                connInDir = CheckConnExistsInTravelDir(groupNodeForCheck, ll2Dir);

                existingSteelConn = !string.IsNullOrEmpty(groupNodeForCheck.AssocExistingSteel);

                if (!connInDir || existingSteelConn)
                {
                    arrDirnstoAttempt[ll2C, 0] = 0;
                }
                else
                {
                    arrDirnstoAttempt[ll2C, 0] = groupNodeForCheck.GroupName + ll2Dir;
                }
            }
        }

        public static bool CheckConnExistsInTravelDir(cGroupNode checkGroupNode, string dirn)
        {
            if (dirn == "E" && !string.IsNullOrEmpty(checkGroupNode.ConnEDir)) return true;
            if (dirn == "W" && !string.IsNullOrEmpty(checkGroupNode.ConnWDir)) return true;
            if (dirn == "N" && !string.IsNullOrEmpty(checkGroupNode.ConnNDir)) return true;
            if (dirn == "S" && !string.IsNullOrEmpty(checkGroupNode.ConnSDir)) return true;
            if (dirn == "U" && !string.IsNullOrEmpty(checkGroupNode.ConnUDir)) return true;
            if (dirn == "D" && !string.IsNullOrEmpty(checkGroupNode.ConnDDir)) return true;

            return false;
        }
    }
}
