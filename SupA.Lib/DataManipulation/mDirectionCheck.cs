namespace SupA.Lib.DataManipulation
{
    public class mDirectionCheck
    {
        public static string DirectionCheck(string DirectionOne, string DirectionTwo)
        {
            if ((DirectionOne == "N" || DirectionOne == "S") && (DirectionTwo == "N" || DirectionTwo == "S"))
            {
                return "parallel";
            }
            else if ((DirectionOne == "E" || DirectionOne == "W") && (DirectionTwo == "E" || DirectionTwo == "W"))
            {
                return "parallel";
            }
            else if ((DirectionOne == "U" || DirectionOne == "D") && (DirectionTwo == "U" || DirectionTwo == "D"))
            {
                return "parallel";
            }
            else
            {
                return "not parallel";
            }
        }
    }
}
