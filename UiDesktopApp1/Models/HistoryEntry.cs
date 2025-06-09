using CommunityToolkit.Mvvm.ComponentModel;

namespace UiDesktopApp1.Models
{
    public partial class HistoryEntry : ObservableObject
    {
        [ObservableProperty]
        private DateTime _runAt;

        [ObservableProperty]
        private string _scriptName = string.Empty;

        [ObservableProperty]
        private string _parameters = string.Empty;

        [ObservableProperty]
        private string _output = string.Empty;

        [ObservableProperty]
        private int? _rating;
    }
}
