using SupA.Lib.Core;

namespace SupA.Lib.DataManipulation
{
    public class mSelectandDetailAncilliary
    {
        public void SelectandDetailAncilliary(IEnumerable<cTubeDefDisc> CollSelectedSuptLocns)
        {
            foreach (var SelSuptLocnTube in CollSelectedSuptLocns)
            {
                var Supt = SelectSuptTypefromPhaast(SelSuptLocnTube);
                // Implement further steps as needed
            }
        }

        public cSuptPoints SelectSuptTypefromPhaast(cTubeDefDisc SelSuptLocnTube)
        {
            var Supt = new cSuptPoints();
            WriteDatatoSuptLocn(Supt, SelSuptLocnTube);

            for (int i = 0; i < pubTblExtendedPHASST.GetLength(0); i++)
            {
                if (Supt.Bore <= pubTblExtendedPHASST[i, 0].PipeRangeMaxNB && Supt.Bore >= pubTblExtendedPHASST[i, 0].PipeRangeMinNB
                    && Supt.InsuThk <= pubTblExtendedPHASST[i, 0].InsuThkMax && Supt.InsuThk >= pubTblExtendedPHASST[i, 0].InsuThkMin
                    && Supt.DesTempMin >= pubTblExtendedPHASST[i, 0].MinDesTemp
                    && Supt.DesTempMax <= pubTblExtendedPHASST[i, 0].MaxDesTemp)
                {
                    Supt.WritePhasstData = pubTblExtendedPHASST[i, 0];
                    break;
                }
            }

            foreach (var pipeDimn in pubTblPipeDimns)
            {
                if (Supt.Bore == pipeDimn.NB)
                {
                    Supt.PipeOD = pipeDimn.OD;
                    break;
                }
            }

            return Supt;
        }

        public void WriteDatatoSuptLocn(cSuptPoints Supt, cTubeDefDisc SelSuptLocnTube)
        {
            Supt.EastingSuptPointProperty = SelSuptLocnTube.East;
            Supt.NorthingSuptPointProperty = SelSuptLocnTube.North;
            Supt.ElSuptPointProperty = SelSuptLocnTube.Upping;
            Supt.BoreProperty = SelSuptLocnTube.abor;
            Supt.TubidirProperty = SelSuptLocnTube.Dir;
            Supt.PipeNameProperty = SelSuptLocnTube.NameofPipe;
            Supt.TubiNameProperty = SelSuptLocnTube.TubeName;
        }
    }
}
