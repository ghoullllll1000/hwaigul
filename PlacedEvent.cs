namespace hwmarch
{
    internal class PlacedEvent: IJournalEntry
    {
        private string shelf;
        private int slot;
        private string item;

        public string Shelf
        {
            get { return shelf; }
        }
        public int Slot
        {
            get { return slot; }
        }
        public string Item
        {
            get { return item; }
        }

        public PlacedEvent(string shelf, int slot, string item)
        {
            this.shelf = shelf;
            this.slot = slot;
            this.item = item;
        }

        public string ToLogLine()
        {
            return $"{shelf}|{slot}|{item}";
        }
        public string ToScreenLine()
        {
            return $"Размещение | полка {shelf} | слот {slot} | товар {item}";
        }

        public static PlacedEvent FromLogLine(string line)
        {
            var parts = line.Split('|');
            return new PlacedEvent(parts[1], int.Parse(parts[2]), parts[3]);
        }
    }
}
