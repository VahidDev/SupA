using SupA.Lib.Core;

namespace SupA.Lib.DataManipulation
{
    public class mWriteActivityLog
    {
        private CLogEntry pubLogEntry;
        private List<CLogEntry> pubActivityLog = new List<CLogEntry>();

        public void WriteActivityLog(string activityLogTxt, DateTime timeNow, string runMode = "")
        {
            if (runMode == "first entry")
            {
                pubLogEntry = new CLogEntry
                {
                    ActivityLogTxt = activityLogTxt,
                    ActivityStart = timeNow
                };
            }
            else
            {
                if (pubLogEntry != null)
                {
                    pubLogEntry.ActivityEnd = timeNow;
                    // Automatically calculates duration within the LogEntry class

                    pubActivityLog.Add(pubLogEntry);

                    // Start a new log entry
                    pubLogEntry = new CLogEntry
                    {
                        ActivityLogTxt = activityLogTxt,
                        ActivityStart = timeNow
                    };
                }
            }
        }
    }
}
