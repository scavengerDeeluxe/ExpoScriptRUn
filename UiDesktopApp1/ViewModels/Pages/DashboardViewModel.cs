using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Linq;
using UiDesktopApp1.Services;

namespace UiDesktopApp1.ViewModels.Pages
{
    public partial class DashboardViewModel : ObservableObject
    {
        private readonly PowerShellService _ps;
        private readonly HistoryService _history;

        public ObservableCollection<string> Scripts { get; } = new();
        public ObservableCollection<ScriptParameter> Parameters { get; } = new();

        [ObservableProperty]
        private string? _selectedScript;

        [ObservableProperty]
        private string _log = string.Empty;

        public DashboardViewModel(PowerShellService ps, HistoryService history)
        {
            _ps = ps;
            _history = history;
            LoadScripts();
        }

        private void LoadScripts()
        {
            Scripts.Clear();
            foreach (var script in _ps.ListScripts())
                Scripts.Add(script);
        }

        partial void OnSelectedScriptChanged(string? value)
        {
            Parameters.Clear();
            if (value == null)
                return;
            foreach (var p in _ps.GetParameters(value))
                Parameters.Add(p);
        }

        [RelayCommand]
        private async Task RunScriptAsync()
        {
            if (SelectedScript == null)
                return;

            Log = $"Running {SelectedScript}...\n";
            var output = await _ps.RunScriptAsync(SelectedScript, Parameters);
            Log += output;

            var entry = new Models.HistoryEntry
            {
                RunAt = DateTime.Now,
                ScriptName = SelectedScript,
                Parameters = string.Join("; ", Parameters.Select(p => $"{p.Name}={p.Value}")),
                Output = output
            };
            _history.AddEntry(entry);
        }

        [RelayCommand]
        private async Task ShowHelpAsync()
        {
            if (SelectedScript == null)
                return;

            Log = await _ps.GetHelpAsync(SelectedScript);
        }
    }
}
