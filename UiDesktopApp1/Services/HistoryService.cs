using System.Text.Json;
using System.IO;
using UiDesktopApp1.Models;

namespace UiDesktopApp1.Services
{
    public class HistoryService
    {
        private readonly string _historyPath;
        private List<HistoryEntry> _entries = new();

        public IReadOnlyList<HistoryEntry> Entries => _entries;

        public HistoryService()
        {
            _historyPath = Path.Combine(AppContext.BaseDirectory, "history.json");
            if (File.Exists(_historyPath))
            {
                try
                {
                    var json = File.ReadAllText(_historyPath);
                    var data = JsonSerializer.Deserialize<List<HistoryEntry>>(json);
                    if (data != null)
                        _entries = data;
                }
                catch
                {
                    // ignore
                }
            }
        }

        private void Save()
        {
            var json = JsonSerializer.Serialize(_entries, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_historyPath, json);
        }

        public void AddEntry(HistoryEntry entry)
        {
            _entries.Add(entry);
            Save();
        }

        public void SaveChanges()
        {
            Save();
        }
    }
}
