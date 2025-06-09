using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using UiDesktopApp1.Models;

namespace UiDesktopApp1.Services;

public class ScriptRepositoryService
{
    private readonly HttpClient _httpClient;
    private readonly ConfigService _config;

    public ScriptRepositoryService(ConfigService config)
    {
        _config = config;
        _httpClient = new HttpClient();
        _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("App");
    }

    public async Task<IEnumerable<ScriptInfo>> GetScriptsAsync()
    {
        var results = new List<ScriptInfo>();
        try
        {
            var response = await _httpClient.GetStringAsync(_config.Config.ScriptRepository);
            var files = JArray.Parse(response);
            foreach (var file in files)
            {
                var downloadUrl = file.Value<string>("download_url");
                var name = file.Value<string>("name");
                if (string.IsNullOrEmpty(downloadUrl) || string.IsNullOrEmpty(name))
                    continue;

                var scriptInfo = new ScriptInfo
                {
                    Name = name,
                    SourceUrl = downloadUrl
                };

                var jsonUrl = downloadUrl.EndsWith(".ps1") ? downloadUrl[..^4] + ".json" : null;
                if (!string.IsNullOrEmpty(jsonUrl))
                {
                    try
                    {
                        var jsonText = await _httpClient.GetStringAsync(jsonUrl);
                        scriptInfo.Definition = JObject.Parse(jsonText);
                    }
                    catch
                    {
                        // ignore if json missing
                    }
                }

                results.Add(scriptInfo);
            }
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Unable to load scripts from '{_config.Config.ScriptRepository}'", ex);
        }

        return results;
    }
}
