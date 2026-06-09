using System.Globalization;

namespace hwmarch
{
    internal class TakenEvent: IJournalEntry
    {
        private string shelf;
        private int slot;
        private string item;

        public string Shelf { get; }
        public int Slot { get; }
        public string Item { get; }

        public TakenEvent(string shelf, int slot, string item)
        {
            Shelf = shelf;
            Slot = slot;
            Item = item;
        }

        public string ToLogLine() => $"{Shelf}|{Slot}|{Item}";
        public string ToScreenLine() => $"Изъятие | полка {Shelf} | слот {Slot} | товар «{Item}»";

        public static TakenEvent FromLogLine(string line)
        {
            var parts = line.Split('|');
            return new TakenEvent(parts[0], int.Parse(parts[1]), parts[2]);
        }
    }
}
