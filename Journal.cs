using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace hwmarch
{
    internal class Journal<T> where T : IJournalEntry
    {
        private readonly List<T> entries = new List<T>();

        public void Add(T entry)
        {
            entries.Add(entry);
        }

        public IEnumerable<T> GetAll()
        {
            return entries.AsReadOnly();
        }

        public void SaveToFile(string path)
        {
            string[] lines = new string[entries.Count];
            for (int i = 0; i < entries.Count; i++)
            {
                lines[i] = entries[i].ToLogLine();
            }
            File.WriteAllLines(path, lines);
        }
    }
}
