using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using UiDesktopApp1.Services;
using UiDesktopApp1.Models;

namespace UiDesktopApp1.ViewModels.Pages
{
    public partial class DashboardViewModel : ObservableObject
    {
        private readonly ScriptRepositoryService _repo;

        public ObservableCollection<ScriptInfo> Scripts { get; } = new();

        [ObservableProperty]
        private ScriptInfo? _selectedScript;

        public DashboardViewModel(ScriptRepositoryService repo)
        {
            _repo = repo;
            LoadScripts();
        }

        private void LoadScripts()
        {
            Scripts.Clear();
            var data = Task.Run(() => _repo.GetScriptsAsync()).Result;
            foreach (var s in data)
                Scripts.Add(s);
        }

        [RelayCommand]
        private void RunScript()
        {
            if (SelectedScript == null)
                return;

            var win = new Views.Windows.ScriptExecutionWindow(SelectedScript);
            win.Owner = Application.Current.MainWindow;
            win.ShowDialog();
        }

    }
}
