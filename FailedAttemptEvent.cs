using System.Globalization;

namespace hwmarch
{
    internal class FailedAttemptEvent: IJournalEntry
    {
        private string operation;
        private string shelf;
        private int? slot;
        private string reason;

        public string Operation
        {
            get { return operation; }
        }
        public string Shelf
        {
            get { return shelf; }
        }
        public int? Slot
        {
            get { return slot; }
        }
        public string Reason
        {
            get { return reason; }
        }

        public FailedAttemptEvent(string operation, string shelf, int? slot, string reason)
        {
            this.operation = operation;
            this.shelf = shelf;
            this.slot = slot;
            this.reason = reason;
        }

        public string ToLogLine()
        {
            return $"{operation}|{shelf}|{slot?.ToString()}|{reason}";
        }
        public string ToScreenLine()
        {
            return $"Неудача | {operation} | полка {shelf} | слот {slot} | причина - {reason}";
        }

        public static FailedAttemptEvent FromLogLine(string line)
        {
            var parts = line.Split('|');
            int? slot = string.IsNullOrEmpty(parts[3]) ? 0 : int.Parse(parts[3]);
            return new FailedAttemptEvent(parts[1], parts[2], slot, parts[4]);
        }
    }
}
