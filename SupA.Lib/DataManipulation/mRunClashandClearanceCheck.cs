using SupA.Lib.Models;

namespace SupA.Lib.DataManipulation
{
    public class mRunClashandClearanceCheck
    {
        public bool RunClashandClearanceCheck(List<cClashData> collLocalClashData, float[] eleBBoxLLCoordArray, float[] eleBBoxULCoordArray, string[] disciplinesToIgnore = null, string[] elementsToIgnore = null, string[] typesToIgnore = null)
        {
            disciplinesToIgnore ??= new string[] { "XYZ" }; // Default if null
            elementsToIgnore ??= new string[] { "XYZ" };
            typesToIgnore ??= new string[] { "XYZ" };

            bool clashFlag = false;
            int noOfClashes = 0;

            List<cClashData> clashDataCollection = new List<cClashData>();

            foreach (var clashData in collLocalClashData)
            {
                var clashBoxCoordArray = CreateCoordArray(clashData); // Implement CreateCoordArray accordingly
                clashFlag = CoordinateCheck(clashBoxCoordArray, eleBBoxLLCoordArray, eleBBoxULCoordArray); // Implement CoordinateCheck accordingly

                if (clashFlag)
                {
                    CreateClashDataEleDiscTypeCollection(clashData, clashDataCollection);
                    bool clashMemToBeConsidered = ClashReview(clashDataCollection, disciplinesToIgnore, elementsToIgnore, typesToIgnore);

                    if (clashMemToBeConsidered)
                    {
                        noOfClashes++;
                    }
                }
            }

            return noOfClashes > 0;
        }

        private void CreateClashDataEleDiscTypeCollection(cClashData clashData, List<cClashData> clashDataCollection)
        {
            clashDataCollection.Clear();

            var discsInClashData = string.IsNullOrEmpty(clashData.DiscsWithinClashBox) ? new[] { "" } : clashData.DiscsWithinClashBox.Split('|');
            var elesInClashData = string.IsNullOrEmpty(clashData.ElesWithinClashBox) ? new[] { "" } : clashData.ElesWithinClashBox.Split('|');
            var typesInClashData = string.IsNullOrEmpty(clashData.TypesWithinClashBox) ? new[] { "" } : clashData.TypesWithinClashBox.Split('|');

            for (int i = 0; i < discsInClashData.Length; i++)
            {
                cClashData clashInfo = new cClashData
                {
                    DiscsWithinClashBox = discsInClashData[i],
                    ElesWithinClashBox = elesInClashData[i],
                    TypesWithinClashBox = typesInClashData[i]
                };
                clashDataCollection.Add(clashInfo);
            }
        }

        private bool ClashReview(List<cClashData> clashDataCollection, string[] disciplinesToIgnore, string[] elementsToIgnore, string[] typesToIgnore)
        {
            bool clashReviewResult = true;

            for (int i = clashDataCollection.Count - 1; i >= 0; i--)
            {
                var clashItem = clashDataCollection[i];
                if (disciplinesToIgnore.Contains(clashItem.DiscsWithinClashBox) || elementsToIgnore.Contains(clashItem.ElesWithinClashBox) || typesToIgnore.Contains(clashItem.TypesWithinClashBox))
                {
                    clashDataCollection.RemoveAt(i);
                }
            }

            if (clashDataCollection.Count == 0)
            {
                clashReviewResult = false;
            }

            return clashReviewResult;
        }
    }
}
