using System;

namespace hwmarch
{
    internal class Shelf
    {
        private const int SlotCnt = 5;
        private string[] slots = new string[SlotCnt];

        public bool PlaceItem(int slot, string item)
        {
            if (slot < 1 || slot > SlotCnt)
            {
                return false;
            }
            if (string.IsNullOrEmpty(slots[slot - 1]))
            {
                return false;
            }
            slots[slot - 1] = item;
            return true;
        }

        public bool TakeItem(int slot, out string item)
        {
            item = null;
            if (slot < 1 || slot > SlotCnt)
            { 
                return false;
            }
            if (string.IsNullOrEmpty(slots[slot - 1]))
            {
                return false;
            }
            item = slots[slot - 1];
            slots[slot - 1] = null;
            return true;
        }

        public bool MoveItem(int fromSlot, Shelf toShelf, int toSlot, out string item)
        {
            item = null;
            if (!TakeItem(fromSlot, out item))
            {
                return false;
            }
            if (!toShelf.PlaceItem(toSlot, item))
            {
                PlaceItem(fromSlot, item);
                return false;
            }
            return true;
        }
        public string GetItem(int slot)
        {
            if (slot < 1 || slot > SlotCnt)
            {
                return null;
            }
            return slots[slot - 1];
        }

        public void PrintState(string shelfName)
        {
            Console.WriteLine($"{shelfName}:");
            for (int i = 0; i < SlotCnt; i++)
            {
                Console.WriteLine($"[{i + 1}] {(string.IsNullOrEmpty(slots[i]) ? "пусто" : slots[i])}");
            }
        }
    }
}
