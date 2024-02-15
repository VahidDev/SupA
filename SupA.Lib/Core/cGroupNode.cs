namespace SupA.Lib.Core
{
    public class cGroupNode
    {
        private string pGroupName;
        private List<cRouteNode> pGroupedNodes = new List<cRouteNode>();
        private cRouteNode pSelectedRouteNode;
        private string pConnEDir;
        private string pConnNDir;
        private string pConnUDir;
        private string pConnWDir;
        private string pConnSDir;
        private string pConnDDir;
        private string pAssocSuptBeam;
        private string pAssocExistingSteel;
        private string pAssocExistingSteelFace;

        public object[] WriteAll(float RowNo = 0)
        {
            List<object> Arr = new List<object>();

            foreach (cRouteNode Mem in pGroupedNodes)
            {
                string data = pGroupName + "," + pAssocSuptBeam + "," + pAssocExistingSteel + "," +
                              pAssocExistingSteelFace + "," + pConnEDir + "," + pConnNDir + "," +
                              pConnUDir + "," + pConnWDir + "," + pConnSDir + "," + pConnDDir + "," +
                              Mem.WriteAll(RowNo);

                Arr.Add(data);
            }

            return Arr.ToArray();
        }

        public string GroupName
        {
            get { return pGroupName; }
            set { pGroupName = value; }
        }

        public cRouteNode SelectedRouteNode
        {
            get { return pSelectedRouteNode; }
            set { pSelectedRouteNode = value; }
        }

        public List<cRouteNode> GroupedNodes
        {
            get { return pGroupedNodes; }
            set { pGroupedNodes = value; }
        }

        public string AssocSuptBeam
        {
            get { return pAssocSuptBeam; }
            set { pAssocSuptBeam = value; }
        }

        public string AssocExistingSteel
        {
            get { return pAssocExistingSteel; }
            set { pAssocExistingSteel = value; }
        }

        public string AssocExistingSteelFace
        {
            get { return pAssocExistingSteelFace; }
            set { pAssocExistingSteelFace = value; }
        }

        public string ConnEDir
        {
            get { return pConnEDir; }
            set { pConnEDir = value; }
        }

        public string ConnNDir
        {
            get { return pConnNDir; }
            set { pConnNDir = value; }
        }

        public string ConnUDir
        {
            get { return pConnUDir; }
            set { pConnUDir = value; }
        }

        public string ConnWDir
        {
            get { return pConnWDir; }
            set { pConnWDir = value; }
        }

        public string ConnSDir
        {
            get { return pConnSDir; }
            set { pConnSDir = value; }
        }

        public string ConnDDir
        {
            get { return pConnDDir; }
            set { pConnDDir = value; }
        }
    }
}
