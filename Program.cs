using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace hwmarch
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var shelfA = new Shelf();
            var shelfB = new Shelf();

            var placedJournal = new Journal<PlacedEvent>();
            var takenJournal = new Journal<TakenEvent>();
            var movedJournal = new Journal<MovedEvent>();
            var failedJournal = new Journal<FailedAttemptEvent>();

            LoadJournals(placedJournal, takenJournal, movedJournal, failedJournal);
            RestoreState(shelfA, shelfB, placedJournal, movedJournal);

            bool running = true;
            while (running)
            {
                Console.WriteLine("=== Склад ===");
                shelfA.PrintState("Полка A");
                shelfB.PrintState("Полка B");
                Console.WriteLine("1 - Положить товар");
                Console.WriteLine("2 - Забрать товар");
                Console.WriteLine("3 - Перенести товар");
                Console.WriteLine("4 - Показать журналы");
                Console.WriteLine("5 - Выход");
                Console.Write("Ваш выбор: ");

                if (!int.TryParse(Console.ReadLine(), out int choice))
                {
                    Console.WriteLine("Введите число от 1 до 5.");
                    continue;
                }

                switch (choice)
                {
                    case 1:
                        PlaceItem(shelfA, shelfB, placedJournal, failedJournal);
                        break;
                    case 2:
                        TakeItem(shelfA, shelfB, takenJournal, failedJournal);
                        break;
                    case 3:
                        MoveItem(shelfA, shelfB, movedJournal, failedJournal);
                        break;
                    case 4:
                        ShowJournals(placedJournal, takenJournal, movedJournal, failedJournal);
                        break;
                    case 5:
                        running = false;
                        break;
                    default:
                        Console.WriteLine("Неверный пункт меню. Повторите ввод.");
                        break;
                }
            }

            SaveJournals(placedJournal, takenJournal, movedJournal, failedJournal);
        }

        static void LoadJournals(Journal<PlacedEvent> placedJournal, Journal<TakenEvent> takenJournal, Journal<MovedEvent> movedJournal, Journal<FailedAttemptEvent> failedJournal)
        {
            LoadJournal("placed.log", placedJournal, PlacedEvent.FromLogLine);
            LoadJournal("taken.log", takenJournal, TakenEvent.FromLogLine);
            LoadJournal("moved.log", movedJournal, MovedEvent.FromLogLine);
            LoadJournal("failed.log", failedJournal, FailedAttemptEvent.FromLogLine);
        }
        static void LoadJournal<T>(string path, Journal<T> journal, Func<string, T> fromLogLine) where T : IJournalEntry
        {
            if (!File.Exists(path))
                return;

            var lines = File.ReadAllLines(path);
            foreach (var line in lines)
            {
                journal.Add(fromLogLine(line));
            }
        }
        static void RestoreState(
        Shelf shelfA, Shelf shelfB,
        Journal<PlacedEvent> placedJournal,
        Journal<MovedEvent> movedJournal)
        {
            foreach (var entry in placedJournal.GetAll())
            {
                var shelf = entry.Shelf == "A" ? shelfA : shelfB;
                shelf.PlaceItem(entry.Slot, entry.Item);
            }

            foreach (var entry in movedJournal.GetAll())
            {
                var fromShelf = entry.FromShelf == "A" ? shelfA : shelfB;
                var toShelf = entry.ToShelf == "A" ? shelfA : shelfB;
                fromShelf.MoveItem(entry.FromSlot, toShelf, entry.ToSlot, out _);
            }
        }
        static void SaveJournals(
        Journal<PlacedEvent> placedJournal,
        Journal<TakenEvent> takenJournal,
        Journal<MovedEvent> movedJournal,
        Journal<FailedAttemptEvent> failedJournal)
        {
            placedJournal.SaveToFile("placed.log");
            takenJournal.SaveToFile("taken.log");
            movedJournal.SaveToFile("moved.log");
            failedJournal.SaveToFile("failed.log");
            Console.WriteLine("Сохранение журналов в placed.log, taken.log, moved.log, failed.log…");
        }
        static void PlaceItem(Shelf shelfA, Shelf shelfB, Journal<PlacedEvent> placedJournal, Journal<FailedAttemptEvent> failedJournal)
        {
            Console.Write("Полка (A или B): ");
            string shelfName = Console.ReadLine().ToUpper();
            Console.Write("Номер слота (1-5): ");
            if (!int.TryParse(Console.ReadLine(), out int slot))
            {
                Console.WriteLine("Неверный номер слота.");
                return;
            }
            Console.Write("Название товара: ");
            string item = Console.ReadLine();

            Shelf shelf = shelfName == "A" ? shelfA : shelfB;
            if (shelf.PlaceItem(slot, item))
            {
                placedJournal.Add(new PlacedEvent(shelfName, slot, item));
                Console.WriteLine("Операция выполнена.");
            }
            else
            {
                string reason = string.IsNullOrEmpty(shelf.GetItem(slot)) ? "слот пуст" : "слот занят";
                failedJournal.Add(new FailedAttemptEvent("Положить", shelfName, slot, reason));
                Console.WriteLine($"Нельзя положить: {reason}.");
            }
        }
        static void TakeItem(Shelf shelfA, Shelf shelfB, Journal<TakenEvent> takenJournal, Journal<FailedAttemptEvent> failedJournal)
        {
            Console.Write("Полка (A или B): ");
            string shelfName = Console.ReadLine().ToUpper();
            Console.Write("Номер слота (1-5): ");
            if (!int.TryParse(Console.ReadLine(), out int slot))
            {
                Console.WriteLine("Неверный номер слота.");
                return;
            }

            Shelf shelf = shelfName == "A" ? shelfA : shelfB;
            if (shelf.TakeItem(slot, out string item))
            {
                takenJournal.Add(new TakenEvent(shelfName, slot, item));
                Console.WriteLine($"Забран товар: {item}");
            }
            else
            {
                string reason = string.IsNullOrEmpty(shelf.GetItem(slot)) ? "слот пуст" : "неизвестная ошибка";
                failedJournal.Add(new FailedAttemptEvent("Забрать", shelfName, slot, reason));
                Console.WriteLine($"Нельзя забрать: {reason}.");
            }
        }
        static void MoveItem(Shelf shelfA, Shelf shelfB, Journal<MovedEvent> movedJournal, Journal<FailedAttemptEvent> failedJournal)
        {
            Console.Write("Полка-источник (A или B): ");
            string fromShelfName = Console.ReadLine().ToUpper();
            Console.Write("Слот-источник (1-5): ");
            if (!int.TryParse(Console.ReadLine(), out int fromSlot))
            {
                Console.WriteLine("Неверный номер слота.");
                return;
            }
            Console.Write("Полка-назначение (A или B): ");
            string toShelfName = Console.ReadLine().ToUpper();
            Console.Write("Слот-назначение (1-5): ");
            if (!int.TryParse(Console.ReadLine(), out int toSlot))
            {
                Console.WriteLine("Неверный номер слота.");
                return;
            }

            Shelf fromShelf = fromShelfName == "A" ? shelfA : shelfB;
            Shelf toShelf = toShelfName == "A" ? shelfA : shelfB;

            if (fromShelf.MoveItem(fromSlot, toShelf, toSlot, out string item))
            {
                movedJournal.Add(new MovedEvent(fromShelfName, fromSlot, toShelfName, toSlot, item));
                Console.WriteLine("Операция выполнена.");
            }
            else
            {
                string reason = string.IsNullOrEmpty(fromShelf.GetItem(fromSlot)) ? "слот пуст" : "слот назначения занят";
                failedJournal.Add(new FailedAttemptEvent("Перенести", $"{fromShelfName}:{fromSlot} на {toShelfName}:{toSlot}", null, reason));
                Console.WriteLine($"Нельзя перенести: {reason}.");
            }
        }
        static void ShowJournals(Journal<PlacedEvent> placedJournal,Journal<TakenEvent> takenJournal, Journal<MovedEvent> movedJournal,Journal<FailedAttemptEvent> failedJournal)
        {
            Console.WriteLine("--- Размещения ---");
            foreach (var entry in placedJournal.GetAll())
                Console.WriteLine(entry.ToScreenLine());

            Console.WriteLine("--- Изъятия ---");
            foreach (var entry in takenJournal.GetAll())
                Console.WriteLine(entry.ToScreenLine());

            Console.WriteLine("--- Переносы ---");
            foreach (var entry in movedJournal.GetAll())
                Console.WriteLine(entry.ToScreenLine());

            Console.WriteLine("--- Неуспешные попытки ---");
            foreach (var entry in failedJournal.GetAll())
                Console.WriteLine(entry.ToScreenLine());
        }
    }
}
