namespace SupA.Lib.Core
{
    public class CLogEntry
    {
        // Fields should be private and use camelCase
        private string activityLogTxt;
        private DateTime activityStart;
        private DateTime activityEnd;
        private TimeSpan activityDuration;

        // Let Statement for initial population of Classes and for copying a class instance
        public void ReadAll(object[] arrToRead)
        {
            activityLogTxt = Convert.ToString(arrToRead[0]);
            activityStart = Convert.ToDateTime(arrToRead[1]);
            activityEnd = Convert.ToDateTime(arrToRead[2]);
            activityDuration = Convert.ToDateTime(arrToRead[3]).TimeOfDay;
        }

        // WriteAll method
        public string WriteAll(float rowNo = default)
        {
            return $"{activityLogTxt},{activityStart},{activityEnd},{activityDuration}";
        }

        // Get Properties
        public string GetActivityLogTxt() => activityLogTxt;
        public DateTime GetActivityStart() => activityStart;
        public DateTime GetActivityEnd() => activityEnd;
        public TimeSpan GetActivityDuration() => activityDuration;

        // Let Properties
        public void SetActivityLogTxt(string value) => activityLogTxt = value;
        public void SetActivityStart(DateTime value) => activityStart = value;
        public void SetActivityEnd(DateTime value) => activityEnd = value;
        public void SetActivityDuration(TimeSpan value) => activityDuration = value;
    }
}
