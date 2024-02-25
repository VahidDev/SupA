using SupA.Lib.Core;
using SupA.Lib.Initialization;

namespace SupA.Lib.DataManipulation
{
    public class mSelectandDetailAncilliary
    {
        public static void SelectandDetailAncilliary(IEnumerable<cTubeDefDisc> CollSelectedSuptLocns)
        {
            foreach (var SelSuptLocnTube in CollSelectedSuptLocns)
            {
                var Supt = SelectSuptTypefromPhaast(SelSuptLocnTube);
                // Implement further steps as needed
            }
        }

        public static cSuptPoints SelectSuptTypefromPhaast(cTubeDefDisc SelSuptLocnTube)
        {
            var Supt = new cSuptPoints();
            WriteDatatoSuptLocn(Supt, SelSuptLocnTube);

            for (int i = 0; i < mSubInitializationSupA.pubTblExtendedPHASST.GetLength(0); i++)
            {
                if (Supt.Bore <= mSubInitializationSupA.pubTblExtendedPHASST[i].PipeRangeMaxNB && Supt.Bore >= mSubInitializationSupA.pubTblExtendedPHASST[i].PipeRangeMinNB
                    && Supt.InsuThk <= mSubInitializationSupA.pubTblExtendedPHASST[i].InsuThkMax && Supt.InsuThk >= mSubInitializationSupA.pubTblExtendedPHASST[i].InsuThkMin
                    && Supt.DesTempMin >= mSubInitializationSupA.pubTblExtendedPHASST[i].MinDesTemp
                    && Supt.DesTempMax <= mSubInitializationSupA.pubTblExtendedPHASST[i].MaxDesTemp)
                {
                    Supt.WritePhasstData(mSubInitializationSupA.pubTblExtendedPHASST[i]);
                    break;
                }
            }

            foreach (var pipeDimn in mSubInitializationSupA.pubTblPipeDimns)
            {
                if (Supt.Bore == pipeDimn.NB)
                {
                    Supt.PipeOD = pipeDimn.OD;
                    break;
                }
            }

            return Supt;
        }

        public static void WriteDatatoSuptLocn(cSuptPoints Supt, cTubeDefDisc SelSuptLocnTube)
        {
            Supt.EastingSuptPointProperty = SelSuptLocnTube.East;
            Supt.NorthingSuptPointProperty = SelSuptLocnTube.North;
            Supt.ElSuptPointProperty = SelSuptLocnTube.Upping;
            Supt.BoreProperty = SelSuptLocnTube.ABor;
            Supt.TubidirProperty = SelSuptLocnTube.Dir;
            Supt.PipeNameProperty = SelSuptLocnTube.NameofPipe;
            Supt.TubiNameProperty = SelSuptLocnTube.TubeName;
        }
    }
}
