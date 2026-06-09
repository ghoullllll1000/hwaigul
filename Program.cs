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

            LoadAllJournals(placedJournal, takenJournal, movedJournal, failedJournal);

            RestoreState(shelfA, shelfB, placedJournal, takenJournal, movedJournal);

            bool running = true;
            while (running)
            {
                Console.WriteLine("\nСклад");
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
                        {
                            PlaceItem(shelfA, shelfB, placedJournal, failedJournal);
                            break;
                        }
                    case 2:
                        {
                            TakeItem(shelfA, shelfB, takenJournal, failedJournal);
                            break;
                        }
                    case 3:
                        {
                            MoveItem(shelfA, shelfB, movedJournal, failedJournal);
                            break;
                        }
                    case 4:
                        {
                            ShowJournals(placedJournal, takenJournal, movedJournal, failedJournal);
                            break;
                        }
                    case 5:
                        {
                            running = false;
                            break;
                        }
                    default:
                        {
                            Console.WriteLine("Неверный пункт меню. Повторите ввод.");
                            break;
                        }
                }
            }

            SaveJournals(placedJournal, takenJournal, movedJournal, failedJournal);
        }

        static void LoadAllJournals(
            Journal<PlacedEvent> placed,
            Journal<TakenEvent> taken,
            Journal<MovedEvent> moved,
            Journal<FailedAttemptEvent> failed)
        {
            if (File.Exists("placed.log"))
            {
                foreach (var line in File.ReadAllLines("placed.log"))
                    placed.Add(PlacedEvent.FromLogLine(line));
            }

            if (File.Exists("taken.log"))
            {
                foreach (var line in File.ReadAllLines("taken.log"))
                    taken.Add(TakenEvent.FromLogLine(line));
            }

            if (File.Exists("moved.log"))
            {
                foreach (var line in File.ReadAllLines("moved.log"))
                    moved.Add(MovedEvent.FromLogLine(line));
            }

            if (File.Exists("failed.log"))
            {
                foreach (var line in File.ReadAllLines("failed.log"))
                    failed.Add(FailedAttemptEvent.FromLogLine(line));
            }
        }

        static void RestoreState(
            Shelf shelfA, Shelf shelfB,
            Journal<PlacedEvent> placedJournal,
            Journal<TakenEvent> takenJournal,
            Journal<MovedEvent> movedJournal)
        {
            foreach (var entry in placedJournal.GetAll())
            {
                Shelf shelf = entry.Shelf == "A" ? shelfA : shelfB;
                shelf.PlaceItem(entry.Slot, entry.Item);
            }

            foreach (var entry in takenJournal.GetAll())
            {
                Shelf shelf = entry.Shelf == "A" ? shelfA : shelfB;
                shelf.TakeItem(entry.Slot, out _);
            }

            foreach (var entry in movedJournal.GetAll())
            {
                Shelf fromShelf = entry.FromShelf == "A" ? shelfA : shelfB;
                Shelf toShelf = entry.ToShelf == "A" ? shelfA : shelfB;
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
            Console.WriteLine("Сохранение выполнено.");
        }

        static void PlaceItem(Shelf shelfA, Shelf shelfB, Journal<PlacedEvent> placedJournal, Journal<FailedAttemptEvent> failedJournal)
        {
            Console.Write("Полка (A или B): ");
            string shelfName = Console.ReadLine().ToUpper();

            if (shelfName != "A" && shelfName != "B")
            {
                Console.WriteLine("Неверное имя полки.");
                failedJournal.Add(new FailedAttemptEvent("Положить", $"Полка {shelfName}", null, "неверное имя полки"));
                return;
            }

            Console.Write("Номер слота (1-5): ");
            if (!int.TryParse(Console.ReadLine(), out int slot) || slot < 1 || slot > 5)
            {
                Console.WriteLine("Неверный номер слота.");
                failedJournal.Add(new FailedAttemptEvent("Положить", $"Полка {shelfName}", null, "неверный номер слота"));
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
                string reason = "слот занят";
                failedJournal.Add(new FailedAttemptEvent("Положить", $"полка {shelfName}", slot, reason));
                Console.WriteLine($"Нельзя положить: {reason}.");
            }
        }

        static void TakeItem(Shelf shelfA, Shelf shelfB, Journal<TakenEvent> takenJournal, Journal<FailedAttemptEvent> failedJournal)
        {
            Console.Write("Полка (A или B): ");
            string shelfName = Console.ReadLine().ToUpper();

            if (shelfName != "A" && shelfName != "B")
            {
                Console.WriteLine("Неверное имя полки.");
                failedJournal.Add(new FailedAttemptEvent("Забрать", $"Полка {shelfName}", null, "неверное имя полки"));
                return;
            }

            Console.Write("Номер слота (1-5): ");
            if (!int.TryParse(Console.ReadLine(), out int slot) || slot < 1 || slot > 5)
            {
                Console.WriteLine("Неверный номер слота.");
                failedJournal.Add(new FailedAttemptEvent("Забрать", $"Полка {shelfName}", null, "неверный номер слота"));
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
                string reason = "слот пуст";
                failedJournal.Add(new FailedAttemptEvent("Забрать", $"полка {shelfName}", slot, reason));
                Console.WriteLine($"Нельзя забрать: {reason}.");
            }
        }

        static void MoveItem(Shelf shelfA, Shelf shelfB, Journal<MovedEvent> movedJournal, Journal<FailedAttemptEvent> failedJournal)
        {
            Console.Write("Полка-источник (A или B): ");
            string fromShelfName = Console.ReadLine().ToUpper();
            if (fromShelfName != "A" && fromShelfName != "B") 
            { 
                Console.WriteLine("Неверная полка.");
                return;
            }

            Console.Write("Слот-источник (1-5): ");
            if (!int.TryParse(Console.ReadLine(), out int fromSlot) || fromSlot < 1 || fromSlot > 5) 
            { 
                Console.WriteLine("Неверный слот."); 
                return; 
            }

            Console.Write("Полка-назначение (A или B): ");
            string toShelfName = Console.ReadLine().ToUpper();
            if (toShelfName != "A" && toShelfName != "B") 
            { 
                Console.WriteLine("Неверная полка."); 
                return; 
            }

            Console.Write("Слот-назначение (1-5): ");
            if (!int.TryParse(Console.ReadLine(), out int toSlot) || toSlot < 1 || toSlot > 5) 
            { 
                Console.WriteLine("Неверный слот."); 
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
                failedJournal.Add(new FailedAttemptEvent("Перенести", $"с {fromShelfName}:{fromSlot} на {toShelfName}:{toSlot}", null, reason));
                Console.WriteLine($"Нельзя перенести: {reason}.");
            }
        }

        static void ShowJournals(
            Journal<PlacedEvent> placedJournal,
            Journal<TakenEvent> takenJournal,
            Journal<MovedEvent> movedJournal,
            Journal<FailedAttemptEvent> failedJournal)
        {
            Console.WriteLine("\nРазмещения");
            foreach (var entry in placedJournal.GetAll())
            {
                Console.WriteLine(entry.ToScreenLine());
            }

            Console.WriteLine("\nИзъятия");
            foreach (var entry in takenJournal.GetAll())
            {
                Console.WriteLine(entry.ToScreenLine());
            }

            Console.WriteLine("\nПереносы");
            foreach (var entry in movedJournal.GetAll())
            {
                Console.WriteLine(entry.ToScreenLine());
            }

            Console.WriteLine("\nНеуспешные попытки");
            foreach (var entry in failedJournal.GetAll())
            { 
                Console.WriteLine(entry.ToScreenLine());
            }
        }
    }
}
