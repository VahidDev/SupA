using SupA.Lib.Core;

namespace SupA.Lib.DataManipulation
{
    public class mWriteActivityLog
    {
        public static List<cLogEntry> ActivityLog = new List<cLogEntry>();
        public static cLogEntry CurrentLogEntry;

        public static void WriteActivityLog(string activityLogTxt, DateTime timeNow, string runMode = "")
        {
            // Check if it's the first entry
            if (runMode == "first entry")
            {
                // Create a new log entry
                CurrentLogEntry = new cLogEntry
                {
                    ActivityLogTxt = activityLogTxt,
                    ActivityStart = timeNow
                };
            }
            else
            {
                // Set end time to now
                CurrentLogEntry.ActivityEnd = timeNow;

                // Calculate activity duration
                CurrentLogEntry.ActivityDuration = CurrentLogEntry.ActivityEnd - CurrentLogEntry.ActivityStart;

                // Add the log entry to the activity log
                ActivityLog.Add(CurrentLogEntry);

                // Start a new log entry
                CurrentLogEntry = new cLogEntry
                {
                    ActivityLogTxt = activityLogTxt,
                    ActivityStart = timeNow
                };
            }
        }
    }
}
