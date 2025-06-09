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
            LoadScriptsAsync();

        }

        private async void LoadScriptsAsync()
        {
            Scripts.Clear();
            try
            {
                var data = await _repo.GetScriptsAsync();
                foreach (var s in data)
                    Scripts.Add(s);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load scripts: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
