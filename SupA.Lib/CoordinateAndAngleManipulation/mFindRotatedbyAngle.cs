namespace SupA.Lib.CoordinateAndAngleManipulation
{
    public class mFindRotatedbyAngle
    {
        public static string FindRotatedbyAngle(string OGdirection, float RotationAngle, string RotationAxis)
        {
            string newDirection = string.Empty;

            switch (OGdirection)
            {
                case "N":
                    newDirection = "E";
                    break;
                case "E":
                    newDirection = "S";
                    break;
                case "S":
                    newDirection = "W";
                    break;
                case "W":
                    newDirection = "N";
                    break;
                default:
                    // Handle invalid input
                    break;
            }

            return newDirection;
        }
    }
}
