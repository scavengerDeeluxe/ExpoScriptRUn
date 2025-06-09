namespace UiDesktopApp1.Models
{
    public class AppConfig
    {
        public string ConfigurationsFolder { get; set; }

        public string AppPropertiesFileName { get; set; }

        public string ScriptRepository { get; set; } = "https://api.github.com/repos/scavengerDeeluxe/ExpoScriptRUn/contents/Scripts?ref=main";
    }
}
