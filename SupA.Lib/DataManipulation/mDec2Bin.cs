namespace SupA.Lib.DataManipulation
{
    public class mDec2Bin
    {
        public static string Dec2Bin(object DecimalIn, int NumberOfBits = -1)
        {
            string Dec2BinResult = "";
            decimal decimalInput = Convert.ToDecimal(DecimalIn);
            decimalInput = Math.Truncate(decimalInput);

            while (decimalInput != 0)
            {
                Dec2BinResult = (decimalInput - 2 * Math.Truncate(decimalInput / 2)) + Dec2BinResult;
                decimalInput = Math.Truncate(decimalInput / 2);
            }

            if (NumberOfBits != -1)
            {
                if (Dec2BinResult.Length > NumberOfBits)
                {
                    Dec2BinResult = "Error - Number exceeds specified bit size";
                }
                else
                {
                    Dec2BinResult = (new string('0', NumberOfBits) + Dec2BinResult).Substring(Dec2BinResult.Length - NumberOfBits);
                }
            }

            return Dec2BinResult;
        }
    }
}
