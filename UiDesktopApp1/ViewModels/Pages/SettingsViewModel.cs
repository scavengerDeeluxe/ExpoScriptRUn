using Wpf.Ui.Abstractions.Controls;
using Wpf.Ui.Appearance;
using UiDesktopApp1.Services;

namespace UiDesktopApp1.ViewModels.Pages
{
    public partial class SettingsViewModel : ObservableObject, INavigationAware
    {
        private bool _isInitialized = false;

        private readonly ConfigService _config;
        private readonly PowerShellService _ps;

        [ObservableProperty]
        private string _appVersion = String.Empty;

        [ObservableProperty]
        private string _repositoryUrl = string.Empty;

        [ObservableProperty]
        private ApplicationTheme _currentTheme = ApplicationTheme.Unknown;

        public SettingsViewModel(ConfigService config, PowerShellService ps)
        {
            _config = config;
            _ps = ps;
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
            CurrentTheme = ApplicationThemeManager.GetAppTheme();
            AppVersion = $"UiDesktopApp1 - {GetAssemblyVersion()}";
            RepositoryUrl = _config.Config.ScriptRepository;

            _isInitialized = true;
        }

        private string GetAssemblyVersion()
        {
            return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version?.ToString()
                ?? String.Empty;
        }

        [RelayCommand]
        private void OnChangeTheme(string parameter)
        {
            switch (parameter)
            {
                case "theme_light":
                    if (CurrentTheme == ApplicationTheme.Light)
                        break;

                    ApplicationThemeManager.Apply(ApplicationTheme.Light);
                    CurrentTheme = ApplicationTheme.Light;

                    break;

                default:
                    if (CurrentTheme == ApplicationTheme.Dark)
                        break;

                    ApplicationThemeManager.Apply(ApplicationTheme.Dark);
                    CurrentTheme = ApplicationTheme.Dark;

                    break;
            }
        }

        [RelayCommand]
        private async Task SaveRepositoryAsync()
        {
            _config.Config.ScriptRepository = RepositoryUrl;
            _config.Save();
            await _ps.RefreshScriptsAsync();
        }
    }
}
