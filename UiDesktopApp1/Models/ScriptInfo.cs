using Newtonsoft.Json.Linq;

namespace UiDesktopApp1.Models;

public class ScriptInfo
{
    public string Name { get; set; } = string.Empty;
    public string SourceUrl { get; set; } = string.Empty;
    public JObject? Definition { get; set; }
}
