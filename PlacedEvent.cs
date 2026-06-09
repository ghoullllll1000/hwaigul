namespace hwmarch
{
    internal class PlacedEvent: IJournalEntry
    {
        private string shelf;
        private int slot;
        private string item;

        public string Shelf { get; }
        public int Slot { get; }
        public string Item { get; }

        public PlacedEvent(string shelf, int slot, string item)
        {
            Shelf = shelf;
            Slot = slot;
            Item = item;
        }

        public string ToLogLine() => $"{Shelf}|{Slot}|{Item}";
        public string ToScreenLine() => $"Размещение | полка {Shelf} | слот {Slot} | товар «{Item}»";

        public static PlacedEvent FromLogLine(string line)
        {
            var parts = line.Split('|');
            return new PlacedEvent(parts[0], int.Parse(parts[1]), parts[2]);
        }
    }
}
