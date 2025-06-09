using System.Collections.ObjectModel;
using UiDesktopApp1.Models;
using UiDesktopApp1.Services;
using Wpf.Ui.Abstractions.Controls;

namespace UiDesktopApp1.ViewModels.Pages
{
    public partial class DataViewModel : ObservableObject, INavigationAware
    {
        private bool _isInitialized = false;
        private readonly HistoryService _history;

        [ObservableProperty]
        private ObservableCollection<HistoryEntry> _entries = new();

        public DataViewModel(HistoryService history)
        {
            _history = history;
        }

        public Task OnNavigatedToAsync()
        {
            if (!_isInitialized)
                InitializeViewModel();

            return Task.CompletedTask;
        }

        public Task OnNavigatedFromAsync() => Task.CompletedTask;

        private void InitializeViewModel()
        {
            Entries = new ObservableCollection<HistoryEntry>(_history.Entries);
            _isInitialized = true;
        }

        public void Save()
        {
            _history.SaveChanges();
        }
    }
}
