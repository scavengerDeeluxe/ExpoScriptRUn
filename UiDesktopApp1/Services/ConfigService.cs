using System.Text.Json;
using UiDesktopApp1.Models;

namespace UiDesktopApp1.Services
{
    public class ConfigService
    {
        private readonly string _configPath;

        public AppConfig Config { get; private set; } = new();

        public ConfigService()
        {
            _configPath = Path.Combine(AppContext.BaseDirectory, "config.json");
            Load();
        }

        private void Load()
        {
            if (File.Exists(_configPath))
            {
                try
                {
                    var json = File.ReadAllText(_configPath);
                    var cfg = JsonSerializer.Deserialize<AppConfig>(json);
                    if (cfg != null)
                        Config = cfg;
                }
                catch
                {
                    Config = new AppConfig();
                }
            }
            else
            {
                Config = new AppConfig();
                Save();
            }
        }

        public void Save()
        {
            var json = JsonSerializer.Serialize(Config, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_configPath, json);
        }
    }
}
