namespace SupA.Lib
{
    public class mQSortSctns
    {
        private static object Temp;

        public static bool SortSections(ref object[] inputArray, bool descending = false)
        {
            bool result = false;

            // Begin the actual sorting process.
            int lb = 0;
            int ub = inputArray.Length - 1;
            int curLow = lb;
            int curHigh = ub;
            int curMidpoint = lb + (ub - lb) / 2; // note integer division

            Temp = inputArray[curMidpoint];

            while (curLow <= curHigh)
            {
                while (QSortCompare(inputArray[curLow], Temp) < 0)
                {
                    curLow++;
                    if (curLow == ub)
                        break;
                }

                while (QSortCompare(Temp, inputArray[curHigh]) < 0)
                {
                    curHigh--;
                    if (curHigh == lb)
                        break;
                }

                if (curLow <= curHigh)
                {
                    object buffer = inputArray[curLow];
                    inputArray[curLow] = inputArray[curHigh];
                    inputArray[curHigh] = buffer;
                    curLow++;
                    curHigh--;
                }
            }

            if (lb < curHigh)
                SortSections(ref inputArray, descending);

            if (curLow < ub)
                SortSections(ref inputArray, descending);

            // If Descending is true, reverse the order of the array
            if (descending)
            {
                Array.Reverse(inputArray, lb, ub - lb + 1);
            }

            result = true;
            return result;
        }

        private static int QSortCompare(object v1, object v2)
        {
            if (IsSimpleNumericType(v1) && IsSimpleNumericType(v2))
            {
                double d1 = Convert.ToDouble(v1);
                double d2 = Convert.ToDouble(v2);
                return d1.CompareTo(d2);
            }
            else if (IsNumericString(v1) && IsNumericString(v2))
            {
                double d1 = Convert.ToDouble(v1);
                double d2 = Convert.ToDouble(v2);
                return d1.CompareTo(d2);
            }
            else
            {
                string s1 = v1.ToString();
                string s2 = v2.ToString();
                return string.Compare(s1, s2, StringComparison.OrdinalIgnoreCase);
            }
        }

        private static bool IsSimpleNumericType(object obj)
        {
            return obj is int || obj is long || obj is float || obj is double || obj is decimal;
        }

        private static bool IsNumericString(object obj)
        {
            string str = obj.ToString();
            double dummy;
            return double.TryParse(str, out dummy);
        }
    }
}
