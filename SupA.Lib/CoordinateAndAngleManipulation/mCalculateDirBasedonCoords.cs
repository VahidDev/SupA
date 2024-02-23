namespace SupA.Lib.CoordinateAndAngleManipulation
{
    public class mCalculateDirBasedonCoords
    {
        public struct CoordinateChangeInfo
        {
            public int HasDir { get; set; }
            public float DirChange { get; set; }
        }

        public static string CalculateDirBasedonCoords(float StartE = 0, float StartN = 0, float StartU = 0,
                                                     float EndE = 0, float EndN = 0, float EndU = 0,
                                                     float[] StartArr = null, float[] EndArr = null)
        {
            CoordinateChangeInfo HasE = new CoordinateChangeInfo();
            CoordinateChangeInfo HasN = new CoordinateChangeInfo();
            CoordinateChangeInfo HasU = new CoordinateChangeInfo();
            string Eledir = "";
            string EledirE = "";
            string EledirN = "";
            string EledirU = "";
            float AngleRotDeg;
            float AngleRotRad;

            if (StartE == 0 && StartN == 0 && StartU == 0 && EndE == 0 && EndN == 0 && EndU == 0)
            {
                StartE = StartArr[0];
                StartN = StartArr[1];
                StartU = StartArr[2];
                EndE = EndArr[0];
                EndN = EndArr[1];
                EndU = EndArr[2];
            }

            if (StartE != EndE)
            {
                HasE.HasDir = 1;
                HasE.DirChange = EndE - StartE;
            }
            if (StartN != EndN)
            {
                HasN.HasDir = 1;
                HasN.DirChange = EndN - StartN;
            }
            if (StartU != EndU)
            {
                HasU.HasDir = 1;
                HasU.DirChange = EndU - StartU;
            }

            if (HasE.HasDir + HasN.HasDir + HasU.HasDir == 1)
            {
                if (HasE.DirChange > 0) Eledir = "E";
                if (HasE.DirChange < 0) Eledir = "W";
                if (HasN.DirChange > 0) Eledir = "N";
                if (HasN.DirChange < 0) Eledir = "S";
                if (HasU.DirChange > 0) Eledir = "U";
                if (HasU.DirChange < 0) Eledir = "D";
            }
            else if (HasE.HasDir + HasN.HasDir + HasU.HasDir == 2)
            {
                if (HasE.DirChange > 0) EledirE = "E";
                if (HasE.DirChange < 0) EledirE = "W";
                if (HasN.DirChange > 0) EledirN = "N";
                if (HasN.DirChange < 0) EledirN = "S";
                if (HasU.DirChange > 0) EledirU = "U";
                if (HasU.DirChange < 0) EledirU = "D";

                if (HasN.HasDir == 1)
                {
                    if (HasE.HasDir == 1)
                    {
                        AngleRotRad = (float)Math.Atan(HasE.DirChange / HasN.DirChange);
                        AngleRotDeg = AngleRotRad * 180 / (float)Math.PI;
                        Eledir = EledirN + " " + AngleRotDeg + " " + EledirE;
                    }
                    else if (HasU.HasDir == 1)
                    {
                        AngleRotRad = (float)Math.Atan(HasU.DirChange / HasN.DirChange);
                        AngleRotDeg = Math.Abs(AngleRotRad * 180 / (float)Math.PI);
                        Eledir = EledirN + " " + AngleRotDeg + " " + EledirU;
                    }
                }
                else if (HasE.HasDir == 1)
                {
                    if (HasU.HasDir == 1)
                    {
                        AngleRotRad = (float)Math.Atan(HasU.DirChange / HasE.DirChange);
                        AngleRotDeg = AngleRotRad * 180 / (float)Math.PI;
                        Eledir = EledirE + " " + AngleRotDeg + " " + EledirU;
                    }
                    else
                    {
                        Console.WriteLine("Some sort of error");
                    }
                }
                else
                {
                    Console.WriteLine("Some sort of error");
                }
            }
            else if (HasE.HasDir + HasN.HasDir + HasU.HasDir == 3)
            {
                Console.WriteLine("The function is unable to evaluate directions which are not on a single plane");
            }

            return Eledir;
        }
    }
}
