namespace SupA.Lib.Core
{
    public class cRouteNode
    {
        private string pRouteNodeNo;
        private float pEasting;
        private float pNorthing;
        private float pUpping;
        private string pAssocTrim;
        private string pAssocSuptBeam;
        private string pAssocExtendedBeam;
        private string pAssocVerticalCol;
        private string pAssocExistingSteel;
        private string pAssocExistingSteelFace;
        private string pAssocBraceEPlane;
        private string pAssocBraceNPlane;
        private string pAssocBraceUPlane;
        private string pAssocTieBack;
        private string pConnEDir;
        private string pConnNDir;
        private string pConnUDir;
        private string pConnWDir;
        private string pConnSDir;
        private string pConnDDir;
        private string pConnBrace1Dir;
        private string pConnBrace2Dir;
        private string pConnBrace3Dir;
        private string pConnBrace4Dir;
        private string pConnBrace5Dir;
        private string pConnBrace6Dir;
        private string pConnBrace7Dir;
        private string pConnBrace8Dir;
        private string pConnBrace9Dir;
        private string pConnBrace10Dir;
        private string pConnBrace11Dir;
        private string pConnBrace12Dir;
        private string pSuptBeamPerpDirFromPipeAxis;

        public string WriteAll(float RowNo = 0)
        {
            string allData = pRouteNodeNo + "," + pEasting + "," + pNorthing + "," + pUpping + "," +
                             pAssocTrim + "," + pAssocSuptBeam + "," + pAssocExtendedBeam + "," +
                             pAssocVerticalCol + "," + pAssocExistingSteel + "," + pAssocExistingSteelFace +
                             "," + pAssocBraceEPlane + "," + pAssocBraceNPlane + "," + pAssocBraceUPlane +
                             "," + pAssocTieBack + "," + pConnEDir + "," + pConnNDir + "," + pConnUDir +
                             "," + pConnWDir + "," + pConnSDir + "," + pConnDDir + "," + pConnBrace1Dir +
                             "," + pConnBrace2Dir + "," + pConnBrace3Dir + "," + pConnBrace4Dir +
                             "," + pConnBrace5Dir + "," + pConnBrace6Dir + "," + pConnBrace7Dir +
                             "," + pConnBrace8Dir + "," + pConnBrace9Dir + "," + pConnBrace10Dir +
                             "," + pConnBrace11Dir + "," + pConnBrace12Dir + "," + pSuptBeamPerpDirFromPipeAxis;

            return allData;
        }

        public string RouteNodeNo
        {
            get { return pRouteNodeNo; }
            set { pRouteNodeNo = value; }
        }

        public float Easting
        {
            get { return pEasting; }
            set { pEasting = value; }
        }

        public float Northing
        {
            get { return pNorthing; }
            set { pNorthing = value; }
        }

        public float Upping
        {
            get { return pUpping; }
            set { pUpping = value; }
        }

        public string AssocTrim
        {
            get { return pAssocTrim; }
            set { pAssocTrim = value; }
        }

        public string AssocSuptBeam
        {
            get { return pAssocSuptBeam; }
            set { pAssocSuptBeam = value; }
        }

        public string AssocExtendedBeam
        {
            get { return pAssocExtendedBeam; }
            set { pAssocExtendedBeam = value; }
        }

        public string AssocVerticalCol
        {
            get { return pAssocVerticalCol; }
            set { pAssocVerticalCol = value; }
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

        public string AssocBraceEPlane
        {
            get { return pAssocBraceEPlane; }
            set { pAssocBraceEPlane = value; }
        }

        public string AssocBraceNPlane
        {
            get { return pAssocBraceNPlane; }
            set { pAssocBraceNPlane = value; }
        }

        public string AssocBraceUPlane
        {
            get { return pAssocBraceUPlane; }
            set { pAssocBraceUPlane = value; }
        }

        public string AssocTieBack
        {
            get { return pAssocTieBack; }
            set { pAssocTieBack = value; }
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

        public string ConnBrace1Dir
        {
            get { return pConnBrace1Dir; }
            set { pConnBrace1Dir = value; }
        }

        public string ConnBrace2Dir
        {
            get { return pConnBrace2Dir; }
            set { pConnBrace2Dir = value; }
        }

        public string ConnBrace3Dir
        {
            get { return pConnBrace3Dir; }
            set { pConnBrace3Dir = value; }
        }

        public string ConnBrace4Dir
        {
            get { return pConnBrace4Dir; }
            set { pConnBrace4Dir = value; }
        }

        public string ConnBrace5Dir
        {
            get { return pConnBrace5Dir; }
            set { pConnBrace5Dir = value; }
        }

        public string ConnBrace6Dir
        {
            get { return pConnBrace6Dir; }
            set { pConnBrace6Dir = value; }
        }

        public string ConnBrace7Dir
        {
            get { return pConnBrace7Dir; }
            set { pConnBrace7Dir = value; }
        }

        public string PConnBrace8Dir { get => pConnBrace8Dir; set => pConnBrace8Dir = value; }
        public string PConnBrace9Dir { get => pConnBrace9Dir; set => pConnBrace9Dir = value; }
        public string PConnBrace10Dir { get => pConnBrace10Dir; set => pConnBrace10Dir = value; }
        public string PConnBrace11Dir { get => pConnBrace11Dir; set => pConnBrace11Dir = value; }
        public string PConnBrace12Dir { get => pConnBrace12Dir; set => pConnBrace12Dir = value; }
        public string PSuptBeamPerpDirFromPipeAxis { get => pSuptBeamPerpDirFromPipeAxis; set => pSuptBeamPerpDirFromPipeAxis = value; }
    }
}
