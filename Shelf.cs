using System;

namespace hwmarch
{
    internal class Shelf
    {
        private const int S = 5;
        private readonly string[] slots = new string[S];

        public string GetItem(int slot)
        {
            if (slot < 1 || slot > S) return null;
            return slots[slot - 1];
        }

        public bool PlaceItem(int slot, string item)
        {
            if (slot < 1 || slot > S || !string.IsNullOrEmpty(slots[slot - 1]))
                return false;

            slots[slot - 1] = item;
            return true;
        }

        public bool TakeItem(int slot, out string item)
        {
            item = null;
            if (slot < 1 || slot > S || string.IsNullOrEmpty(slots[slot - 1]))
                return false;

            item = slots[slot - 1];
            slots[slot - 1] = null;
            return true;
        }

        public bool MoveItem(int fromSlot, Shelf toShelf, int toSlot, out string item)
        {
            item = null;
            if (string.IsNullOrEmpty(GetItem(fromSlot)) || !string.IsNullOrEmpty(toShelf.GetItem(toSlot)))
                return false;

            TakeItem(fromSlot, out item);
            toShelf.PlaceItem(toSlot, item);
            return true;
        }

        public void PrintState(string label)
        {
            Console.Write($"{label}: ");
            for (int i = 0; i < S; i++)
            {
                string status = string.IsNullOrEmpty(slots[i]) ? "пусто" : slots[i];
                Console.Write($"{i + 1} {status}   ");
            }
            Console.WriteLine();
        }
    }
}
