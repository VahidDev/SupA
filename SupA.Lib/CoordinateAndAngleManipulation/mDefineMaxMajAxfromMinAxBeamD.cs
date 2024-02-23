using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupA.Lib.CoordinateAndAngleManipulation
{
    public class mDefineMaxMajAxfromMinAxBeamD
    {
        public static string DefineMajorAxisfromMinorAxisandBeamDir(string MinorAxis, string BeamDir)
        {
            string MajorAxis = "";

            if (BeamDir == "N")
            {
                if (MinorAxis == "E")
                {
                    MajorAxis = "U";
                }
                else if (MinorAxis == "W")
                {
                    MajorAxis = "D";
                }
                else if (MinorAxis == "U")
                {
                    MajorAxis = "W";
                }
                else if (MinorAxis == "D")
                {
                    MajorAxis = "E";
                }
            }

            if (BeamDir == "S")
            {
                if (MinorAxis == "E")
                {
                    MajorAxis = "D";
                }
                else if (MinorAxis == "W")
                {
                    MajorAxis = "U";
                }
                else if (MinorAxis == "U")
                {
                    MajorAxis = "E";
                }
                else if (MinorAxis == "D")
                {
                    MajorAxis = "W";
                }
            }

            if (BeamDir == "E")
            {
                if (MinorAxis == "N")
                {
                    MajorAxis = "D";
                }
                else if (MinorAxis == "S")
                {
                    MajorAxis = "U";
                }
                else if (MinorAxis == "U")
                {
                    MajorAxis = "N";
                }
                else if (MinorAxis == "D")
                {
                    MajorAxis = "S";
                }
            }

            if (BeamDir == "W")
            {
                if (MinorAxis == "N")
                {
                    MajorAxis = "D";
                }
                else if (MinorAxis == "S")
                {
                    MajorAxis = "U";
                }
                else if (MinorAxis == "U")
                {
                    MajorAxis = "S";
                }
                else if (MinorAxis == "D")
                {
                    MajorAxis = "N";
                }
            }

            if (BeamDir == "U")
            {
                if (MinorAxis == "E")
                {
                    MajorAxis = "S";
                }
                else if (MinorAxis == "W")
                {
                    MajorAxis = "N";
                }
                else if (MinorAxis == "N")
                {
                    MajorAxis = "E";
                }
                else if (MinorAxis == "S")
                {
                    MajorAxis = "W";
                }
            }

            if (BeamDir == "D")
            {
                if (MinorAxis == "E")
                {
                    MajorAxis = "N";
                }
                else if (MinorAxis == "W")
                {
                    MajorAxis = "S";
                }
                else if (MinorAxis == "N")
                {
                    MajorAxis = "W";
                }
                else if (MinorAxis == "S")
                {
                    MajorAxis = "E";
                }
            }

            return MajorAxis;
        }
    }
}
