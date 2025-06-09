using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using UiDesktopApp1.Services;

namespace UiDesktopApp1.ViewModels.Pages
{
    public partial class DashboardViewModel : ObservableObject
    {
        private readonly PowerShellService _ps;

        public ObservableCollection<string> Scripts { get; } = new();
        public ObservableCollection<ScriptParameter> Parameters { get; } = new();

        [ObservableProperty]
        private string? _selectedScript;

        [ObservableProperty]
        private string _log = string.Empty;

        public DashboardViewModel(PowerShellService ps)
        {
            _ps = ps;
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
            Log += await _ps.RunScriptAsync(SelectedScript, Parameters);
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
