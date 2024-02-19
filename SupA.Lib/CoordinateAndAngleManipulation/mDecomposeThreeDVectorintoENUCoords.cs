namespace SupA.Lib.CoordinateAndAngleManipulation
{
    public class mDecomposeThreeDVectorintoENUCoords
    {
        public static float[] DecomposeThreeDVectorintoENUCoords(string direction, float distanceToMove, string runMode = "")
        {
            float[] distanceToMoveInCardinalDirns = new float[3];

            // Make sure that there are no spaces in the direction variable
            direction = direction.Trim();

            // Case 1. Direction to be moved is a single cardinal direction. This requires no trig to figure out
            if (direction.Length == 1)
            {
                if (direction == "E")
                    distanceToMoveInCardinalDirns[0] = distanceToMove;
                else if (direction == "N")
                    distanceToMoveInCardinalDirns[1] = distanceToMove;
                else if (direction == "U")
                    distanceToMoveInCardinalDirns[2] = distanceToMove;
                else if (direction == "W")
                    distanceToMoveInCardinalDirns[0] = -distanceToMove;
                else if (direction == "S")
                    distanceToMoveInCardinalDirns[1] = -distanceToMove;
                else if (direction == "D")
                    distanceToMoveInCardinalDirns[2] = -distanceToMove;
            }
            // Case 2. Direction to be moved is on a "cardinal plane"
            else if (double.TryParse(direction.Replace(direction[0].ToString(), "").Replace(direction[direction.Length - 1].ToString(), ""), out _))
            {
                // Set up + or - multipliers based on the direction
                int NSDirnIndicator = (direction[0] == 'N' || direction[direction.Length - 1] == 'N') ? 1 : (direction[0] == 'S' || direction[direction.Length - 1] == 'S') ? -1 : 0;
                int EWDirnIndicator = (direction[0] == 'E' || direction[direction.Length - 1] == 'E') ? 1 : (direction[0] == 'W' || direction[direction.Length - 1] == 'W') ? -1 : 0;
                int UDDirnIndicator = (direction[0] == 'U' || direction[direction.Length - 1] == 'U') ? 1 : (direction[0] == 'D' || direction[direction.Length - 1] == 'D') ? -1 : 0;

                // Pull the angle of rotation out of the direction string
                double angleOfRotation = double.Parse(direction.Replace(direction[0].ToString(), "").Replace(direction[direction.Length - 1].ToString(), ""));
                angleOfRotation *= Math.PI / 180;

                if (direction[0] == 'N' || direction[0] == 'S')
                {
                    if (direction[direction.Length - 1] == 'E' || direction[direction.Length - 1] == 'W')
                    {
                        // The first letter in the direction string gets its magnitude from cos
                        distanceToMoveInCardinalDirns[1] = distanceToMove * (float)Math.Cos(angleOfRotation) * NSDirnIndicator;
                        // The last letter in the direction string gets its magnitude from sin
                        distanceToMoveInCardinalDirns[0] = distanceToMove * (float)Math.Sin(angleOfRotation) * EWDirnIndicator;
                    }
                    else if (direction[direction.Length - 1] == 'U' || direction[direction.Length - 1] == 'D')
                    {
                        // The first letter in the direction string gets its magnitude from cos
                        distanceToMoveInCardinalDirns[1] = distanceToMove * (float)Math.Cos(angleOfRotation) * NSDirnIndicator;
                        // The last letter in the direction string gets its magnitude from sin
                        distanceToMoveInCardinalDirns[2] = distanceToMove * (float)Math.Sin(angleOfRotation) * UDDirnIndicator;
                    }
                    else
                    {
                        Console.WriteLine("Error!");
                    }
                }
                else if (direction[0] == 'E' || direction[0] == 'W')
                {
                    if (direction[direction.Length - 1] == 'U' || direction[direction.Length - 1] == 'D')
                    {
                        // The first letter in the direction string gets its magnitude from cos
                        distanceToMoveInCardinalDirns[0] = distanceToMove * (float)Math.Cos(angleOfRotation) * EWDirnIndicator;
                        // The last letter in the direction string gets its magnitude from sin
                        distanceToMoveInCardinalDirns[2] = distanceToMove * (float)Math.Sin(angleOfRotation) * UDDirnIndicator;
                    }
                    else if (direction[direction.Length - 1] == 'N' || direction[direction.Length - 1] == 'S')
                    {
                        // The first letter in the direction string gets its magnitude from cos
                        distanceToMoveInCardinalDirns[0] = distanceToMove * (float)Math.Cos(angleOfRotation) * EWDirnIndicator;
                        // The last letter in the direction string gets its magnitude from sin
                        distanceToMoveInCardinalDirns[1] = distanceToMove * (float)Math.Sin(angleOfRotation) * NSDirnIndicator;
                    }
                    else
                    {
                        Console.WriteLine("Error!");
                    }
                }
                else
                {
                    Console.WriteLine("Error!");
                }
            }
            else
            {
                Console.WriteLine("Error!");
            }

            if (runMode == "allpositive")
            {
                distanceToMoveInCardinalDirns[0] = Math.Abs(distanceToMoveInCardinalDirns[0]);
                distanceToMoveInCardinalDirns[1] = Math.Abs(distanceToMoveInCardinalDirns[1]);
                distanceToMoveInCardinalDirns[2] = Math.Abs(distanceToMoveInCardinalDirns[2]);
            }
            else if (runMode == "allnegative")
            {
                distanceToMoveInCardinalDirns[0] = -Math.Abs(distanceToMoveInCardinalDirns[0]);
                distanceToMoveInCardinalDirns[1] = -Math.Abs(distanceToMoveInCardinalDirns[1]);
                distanceToMoveInCardinalDirns[2] = -Math.Abs(distanceToMoveInCardinalDirns[2]);
            }

            return distanceToMoveInCardinalDirns;
        }
    }
}
