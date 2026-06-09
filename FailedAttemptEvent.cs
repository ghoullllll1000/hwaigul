using System.Globalization;

namespace hwmarch
{
    internal class FailedAttemptEvent: IJournalEntry
    {
        private string operation;
        private string shelf;
        private int? slot;
        private string reason;

        public string Operation { get; }
        public string ShelfInfo { get; }
        public int? Slot { get; }
        public string Reason { get; }

        public FailedAttemptEvent(string operation, string shelfInfo, int? slot, string reason)
        {
            Operation = operation;
            ShelfInfo = shelfInfo;
            Slot = slot;
            Reason = reason;
        }

        public string ToLogLine() => $"{Operation}|{ShelfInfo}|{Slot}|{Reason}";
        public string ToScreenLine()
        {
            string slotStr = Slot.HasValue ? $" слот {Slot}" : "";
            return $"Неудача | {Operation} | {ShelfInfo}{slotStr} | причина: {Reason}";
        }

        public static FailedAttemptEvent FromLogLine(string line)
        {
            var parts = line.Split('|');
            int? slot = string.IsNullOrEmpty(parts[2]) ? null : (int?)int.Parse(parts[2]);
            return new FailedAttemptEvent(parts[0], parts[1], slot, parts[3]);
        }
    }
}
