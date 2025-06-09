using System.Management.Automation;
using System.Management.Automation.Language;
using System.Text;

namespace UiDesktopApp1.Services
{
    public class ScriptParameter
    {
        public string Name { get; set; } = string.Empty;
        public bool Mandatory { get; set; }
        public string? Value { get; set; }
    }

    public class PowerShellService
    {
        private readonly string _scriptDirectory;

        public PowerShellService()
        {
            _scriptDirectory = Path.Combine(AppContext.BaseDirectory, "Scripts");
            Directory.CreateDirectory(_scriptDirectory);
        }

        public IEnumerable<string> ListScripts()
        {
            if (!Directory.Exists(_scriptDirectory))
                return Enumerable.Empty<string>();
            return Directory.GetFiles(_scriptDirectory, "*.ps1")
                .Select(Path.GetFileNameWithoutExtension);
        }

        public IEnumerable<ScriptParameter> GetParameters(string scriptName)
        {
            var path = Path.Combine(_scriptDirectory, scriptName + ".ps1");
            if (!File.Exists(path))
                return Enumerable.Empty<ScriptParameter>();

            var ast = Parser.ParseFile(path, out _, out _);
            var parameters = ast.ParamBlock?.Parameters;
            if (parameters == null)
                return Enumerable.Empty<ScriptParameter>();

            var result = new List<ScriptParameter>();
            foreach (var p in parameters)
            {
                var attr = p.Attributes
                    .OfType<AttributeAst>()
                    .Select(a => a.GetReflectionAttribute())
                    .OfType<ParameterAttribute>()
                    .FirstOrDefault();
                bool mandatory = attr != null && attr.Mandatory;
                result.Add(new ScriptParameter
                {
                    Name = p.Name.VariablePath.UserPath,
                    Mandatory = mandatory
                });
            }
            return result;
        }

        public async Task<string> RunScriptAsync(string scriptName, IEnumerable<ScriptParameter> parameters)
        {
            var path = Path.Combine(_scriptDirectory, scriptName + ".ps1");
            using var ps = PowerShell.Create();
            ps.AddCommand(path);
            foreach (var param in parameters)
            {
                if (!string.IsNullOrWhiteSpace(param.Value))
                {
                    ps.AddParameter(param.Name, param.Value);
                }
            }

            var results = await Task.Factory.FromAsync(ps.BeginInvoke(), ps.EndInvoke);
            var sb = new StringBuilder();
            foreach (var r in results)
                sb.AppendLine(r?.ToString());
            return sb.ToString();
        }

        public async Task<string> GetHelpAsync(string scriptName)
        {
            var path = Path.Combine(_scriptDirectory, scriptName + ".ps1");
            using var ps = PowerShell.Create();
            ps.AddCommand("Get-Help").AddParameter("Path", path).AddParameter("Full", true);
            var results = await Task.Factory.FromAsync(ps.BeginInvoke(), ps.EndInvoke);
            var sb = new StringBuilder();
            foreach (var r in results)
                sb.AppendLine(r?.ToString());
            return sb.ToString();
        }
    }
}
