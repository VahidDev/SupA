namespace SupA.Lib.Core
{
    public class cBasePlate
    {
        private string pGroupNodeNo;
        private double pEast;
        private double pNorth;
        private double pUp;
        private double pBasePlateThk;
        private double pBasePlateWidth;
        private double pBasePlateLength;
        private string pBasePlateBoltingReqd;
        private double pBoltSpacing;
        private double pBoltSize;
        private double pPlinthThk;
        private double pPlinthWidth;
        private double pPlinthLength;

        public string GroupNodeNo
        {
            get { return pGroupNodeNo; }
            set { pGroupNodeNo = value; }
        }

        public double East
        {
            get { return pEast; }
            set { pEast = value; }
        }

        public double North
        {
            get { return pNorth; }
            set { pNorth = value; }
        }

        public double Up
        {
            get { return pUp; }
            set { pUp = value; }
        }

        public double BasePlateThk
        {
            get { return pBasePlateThk; }
            set { pBasePlateThk = value; }
        }

        public double BasePlateWidth
        {
            get { return pBasePlateWidth; }
            set { pBasePlateWidth = value; }
        }

        public double BasePlateLength
        {
            get { return pBasePlateLength; }
            set { pBasePlateLength = value; }
        }

        public string BasePlateBoltingReqd
        {
            get { return pBasePlateBoltingReqd; }
            set { pBasePlateBoltingReqd = value; }
        }

        public double BoltSpacing
        {
            get { return pBoltSpacing; }
            set { pBoltSpacing = value; }
        }

        public double BoltSize
        {
            get { return pBoltSize; }
            set { pBoltSize = value; }
        }

        public double PlinthThk
        {
            get { return pPlinthThk; }
            set { pPlinthThk = value; }
        }

        public double PlinthWidth
        {
            get { return pPlinthWidth; }
            set { pPlinthWidth = value; }
        }

        public double PlinthLength
        {
            get { return pPlinthLength; }
            set { pPlinthLength = value; }
        }

        public void ReadAll(object[] ArrtoRead)
        {
            pGroupNodeNo = (string)ArrtoRead[0];
            pEast = (double)ArrtoRead[1];
            pNorth = (double)ArrtoRead[2];
            pUp = (double)ArrtoRead[3];
            pBasePlateThk = (double)ArrtoRead[4];
            pBasePlateWidth = (double)ArrtoRead[5];
            pBasePlateLength = (double)ArrtoRead[6];
            pBasePlateBoltingReqd = (string)ArrtoRead[7];
            pBoltSpacing = (double)ArrtoRead[8];
            pBoltSize = (double)ArrtoRead[9];
            pPlinthThk = (double)ArrtoRead[10];
            pPlinthWidth = (double)ArrtoRead[11];
            pPlinthLength = (double)ArrtoRead[12];
        }

        public string WriteAll(float RowNo = 0)
        {
            string result = pGroupNodeNo;
            result += "," + pEast;
            result += "," + pNorth;
            result += "," + pUp;
            result += "," + pBasePlateThk;
            result += "," + pBasePlateWidth;
            result += "," + pBasePlateLength;
            result += "," + pBasePlateBoltingReqd;
            result += "," + pBoltSpacing;
            result += "," + pBoltSize;
            result += "," + pPlinthThk;
            result += "," + pPlinthWidth;
            result += "," + pPlinthLength;
            return result;
        }
    }
}
