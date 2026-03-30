using System.Globalization;

namespace hwmarch
{
    internal class TakenEvent: IJournalEntry
    {
        private string shelf;
        private int slot;
        private string item;

        public string Shelf
        {
            get {  return shelf; }
        }
        public int Slot
        {
            get { return slot; }
        }
        public string Item
        {
            get { return item; }
        }

        public TakenEvent(string shelf, int slot, string item)
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
            return $"Изъятие | полка {shelf} | слот {slot} | товар {item}";
        }

        public static TakenEvent FromLogLine(string line)
        {
            var parts = line.Split('|');
            return new TakenEvent(parts[1], int.Parse(parts[2]), parts[3]);
        }
    }
}
