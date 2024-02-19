namespace SupA.Lib.Core
{
    public class CLogEntry
    {
        // Fields should be private and use camelCase
        private string activityLogTxt;
        private DateTime activityStart;
        private DateTime activityEnd;
        private TimeSpan activityDuration;

        public string ActivityLogTxt
        {
            get { return activityLogTxt; }
            set { activityLogTxt = value; }
        }

        public DateTime ActivityStart
        {
            get { return activityStart; }
            set { activityStart = value; }
        }

        public DateTime ActivityEnd
        {
            get { return activityEnd; }
            set { activityEnd = value; }
        }

        public TimeSpan ActivityDuration
        {
            get { return activityDuration; }
            set { activityDuration = value; }
        }

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
    }
}
