using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace hwmarch
{
    internal class Journal<T> where T : IJournalEntry
    {
        private List<T> entries = new List<T>();

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
            var lines = entries.Select(e => e.ToLogLine()).ToList();
            File.WriteAllLines(path, lines);
        }
    }
}
