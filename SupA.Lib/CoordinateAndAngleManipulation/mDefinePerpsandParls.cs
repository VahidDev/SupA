namespace SupA.Lib.CoordinateAndAngleManipulation
{
    public class mDefinePerpsandParls
    {
        public static void DefinePerpsandParls(ref string parlAxis, out string perpAxis1, out string perpAxis2, out int perpAxis1No, out int perpAxis2No, out int parlAxisNo)
        {
            perpAxis1 = "";
            perpAxis2 = "";
            parlAxisNo = 0;
            perpAxis1No = 0;
            perpAxis2No = 0;
            switch (parlAxis)
            {
                case "N":
                    perpAxis1 = "W";
                    perpAxis2 = "U";
                    parlAxisNo = 2;
                    perpAxis1No = 1;
                    perpAxis2No = 3;
                    break;
                case "S":
                    perpAxis1 = "E";
                    perpAxis2 = "U";
                    parlAxisNo = 2;
                    perpAxis1No = 1;
                    perpAxis2No = 3;
                    break;
                case "E":
                    perpAxis1 = "N";
                    perpAxis2 = "U";
                    parlAxisNo = 1;
                    perpAxis1No = 2;
                    perpAxis2No = 3;
                    break;
                case "W":
                    perpAxis1 = "S";
                    perpAxis2 = "U";
                    parlAxisNo = 1;
                    perpAxis1No = 2;
                    perpAxis2No = 3;
                    break;
                case "U":
                    perpAxis1 = "E";
                    perpAxis2 = "N";
                    parlAxisNo = 3;
                    perpAxis1No = 1;
                    perpAxis2No = 2;
                    break;
                case "D":
                    perpAxis1 = "W";
                    perpAxis2 = "N";
                    parlAxisNo = 3;
                    perpAxis1No = 1;
                    perpAxis2No = 2;
                    break;
                default:
                    Console.WriteLine("Error in DefinePerpsandParls");
                    break;
            }
        }
    }
}
