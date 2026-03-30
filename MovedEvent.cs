using System.Globalization;

namespace hwmarch
{
    internal class MovedEvent: IJournalEntry
    {
        private string fromShelf;
        private int fromSlot;
        private string toShelf;
        private int toSlot;
        private string item;

        public string FromShelf
        {
            get { return fromShelf; }
        }
        public int FromSlot
        {
            get { return fromSlot; }
        }
        public string ToShelf
        {
            get { return toShelf; }
        }
        public int ToSlot
        {
            get { return toSlot; }
        }
        public string Item
        {
            get { return item; }
        }

        public MovedEvent(string fromShelf, int fromSlot, string toShelf, int toSlot, string item)
        {
            this.fromShelf = fromShelf;
            this.fromSlot = fromSlot;
            this.toShelf = toShelf;
            this.toSlot = toSlot;
            this.item = item;
        }

        public string ToLogLine()
        {
            return $"{fromShelf}|{fromSlot}|{toShelf}|{toSlot}|{item}";
        }
        public string ToScreenLine()
        {
            return $"Перенос | с {fromShelf}:{fromSlot} на {toShelf}:{toSlot} | товар {item}";
        }

        public static MovedEvent FromLogLine(string line)
        {
            var parts = line.Split('|');
            return new MovedEvent(parts[1], int.Parse(parts[2]), parts[3], int.Parse(parts[4]), parts[5]);
        }
    }
}
