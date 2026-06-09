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

        public string FromShelf { get; }
        public int FromSlot { get; }
        public string ToShelf { get; }
        public int ToSlot { get; }
        public string Item { get; }

        public MovedEvent(string fromShelf, int fromSlot, string toShelf, int toSlot, string item)
        {
            FromShelf = fromShelf;
            FromSlot = fromSlot;
            ToShelf = toShelf;
            ToSlot = toSlot;
            Item = item;
        }

        public string ToLogLine() => $"{FromShelf}|{FromSlot}|{ToShelf}|{ToSlot}|{Item}";
        public string ToScreenLine() => $"Перенос | с {FromShelf}:{FromSlot} на {ToShelf}:{ToSlot} | товар «{Item}»";

        public static MovedEvent FromLogLine(string line)
        {
            var parts = line.Split('|');
            return new MovedEvent(parts[0], int.Parse(parts[1]), parts[2], int.Parse(parts[3]), parts[4]);
        }
    }
}
