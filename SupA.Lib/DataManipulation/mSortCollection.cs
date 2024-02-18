using System.Collections.ObjectModel;
using SupA.Lib.Core;
using SupA.Lib.Utils;

namespace SupA.Lib.DataManipulation
{
    public class mSortCollection
    {
        public Collection<object> SortCollection(Collection<object> C, string ColtoSort)
        {
            int n = C.Count;
            // Special case - sorting by PathsUnTravelledCount
            if (ColtoSort == "PathsUnTravelledCount")
            {
                return SortCollectionbyPathsunT(C);
            }
            if (n == 0) return new Collection<object>();

            int[] Index = new int[n]; // allocate index array
            for (int I = 0; I < n; I++) Index[I] = I + 1; // fill index array

            for (int I = n / 2 - 1; I >= 0; I--) // generate ordered heap
            {
                Heapify(C, ColtoSort, Index, I, n);
            }

            for (int M = n; M >= 2; M--) // sort the index array
            {
                Exchange(Index, 0, M - 1); // move highest element to top
                Heapify(C, ColtoSort, Index, 0, M - 1);
            }

            Collection<object> C2 = new Collection<object>();
            for (int I = 0; I < n; I++) C2.Add(C[Index[I] - 1]); // fill output collection

            return C2;
        }

        private void Exchange(int[] Index, long I, long J)
        {
            int Temp = Index[I];
            Index[I] = Index[J];
            Index[J] = Temp;
        }

        private void Heapify(Collection<object> c, string colToSort, int[] index, int i, int n)
        {
            // Heap order rule: a[i] >= a[2*i+1] and a[i] >= a[2*i+2]
            long nDiv2 = n / 2;
            long I = i;
            while (I < nDiv2)
            {
                long K = 2 * I + 1;
                if (K + 1 < n)
                {
                    if ((dynamic)VbaInterop.CallByName(c[index[K]], colToSort, VbCallType.Get) < (dynamic)VbaInterop.CallByName(c[index[K + 1]], colToSort, VbCallType.Get))
                    {
                        K = K + 1;
                    }
                }
                if ((dynamic)VbaInterop.CallByName(c[index[I]], colToSort, VbCallType.Get) >= (dynamic)VbaInterop.CallByName(c[index[K]], colToSort, VbCallType.Get))
                {
                    break;
                }
                Exchange(index, I, K);
                I = K;
            }
        }

        public Collection<object> SortCollectionbyPathsunT(Collection<object> C)
        {
            cPotlSupt M;
            int MaxPathsunT = 0;
            int I;
            int I2;
            Collection<object> C2 = new Collection<object>();

            // See what the maximum pathsuntravelled count value is in our entire list
            foreach (cPotlSupt item in C)
            {
                MaxPathsunT = Math.Max(MaxPathsunT, item.PathsUnTravelledCount);
            }

            for (I = 0; I <= MaxPathsunT; I++)
            {
                I2 = 1;
                while (I2 <= C.Count)
                {
                    if (((cPotlSupt)C[I2]).PathsUnTravelledCount == I)
                    {
                        C2.Add(C[I2]);
                        C.RemoveAt(I2);
                        I2--;
                    }
                    I2++;
                }
            }

            return C2;
        }
    }
}
