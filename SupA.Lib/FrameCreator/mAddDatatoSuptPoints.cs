using SupA.Lib.Core;
using SupA.Lib.Initialization;

namespace SupA.Lib.FrameCreator
{
    public class mAddDatatoSuptPoints
    {
        public struct SuptPoints
        {
            public double Bore { get; set; }
            public double InsuThk { get; set; }
            public double DesTempMin { get; set; }
            public double DesTempMax { get; set; }
            public object WritePhasstData { get; set; }
            public double PipeOD { get; set; }
        }

        public struct ExtendedPHASST
        {
            public double PipeRangeMaxNB { get; set; }
            public double PipeRangeMinNB { get; set; }
            public double InsuThkMax { get; set; }
            public double InsuThkMin { get; set; }
            public double MinDesTemp { get; set; }
            public double MaxDesTemp { get; set; }
        }

        public struct PipeDimns
        {
            public double NB { get; set; }
            public double OD { get; set; }
        }

        public void AddDatatoSuptPoints(List<cSuptPoints> collSuptPointsinArea)
        {
            // Write all data from pubTblExtendedPHASST to the Supt objects in collSuptPointsinArea
            foreach (var supt in collSuptPointsinArea)
            {
                foreach (var item in mSubInitializationSupA.pubTblExtendedPHASST)
                {
                    if (supt.Bore <= item.PipeRangeMaxNB && supt.Bore >= item.PipeRangeMinNB &&
                        supt.InsuThk <= item.InsuThkMax && supt.InsuThk >= item.InsuThkMin &&
                        supt.DesTempMin >= item.MinDesTemp &&
                        supt.DesTempMax <= item.MaxDesTemp)
                    {
                        supt.WritePhasstData(item);
                        break;
                    }
                }
            }

            // Write the pipe OD from pubTblPipeDimns to the Supt objects in collSuptPointsinArea
            foreach (var supt in collSuptPointsinArea)
            {
                foreach (var item in mSubInitializationSupA.pubTblPipeDimns)
                {
                    if (supt.Bore == item.NB)
                    {
                        supt.PipeOD = item.OD;
                        break;
                    }
                }
            }
        }
    }
}
