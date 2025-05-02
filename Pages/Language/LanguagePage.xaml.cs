using MelodiaTherapy.Services;
using System.Collections.ObjectModel;
using MelodiaTherapy.Models;
using System.Windows.Input;
using Microsoft.Maui.Platform;

namespace MelodiaTherapy.Pages
{
    public partial class LanguagePage : ContentPage
    {
        public LanguagePage()
        {
            InitializeComponent();
            BindingContext = new LanguagePageVM();
        }

        private void OnStartClicked(object sender, EventArgs e)
        {
            if (Application.Current != null)
            {
                Application.Current.Windows[0].Page = new NavigationPage(new StartPage());
            }
        }
    }

    internal class LanguagePageVM : BaseViewModel
    {
        private bool showLoadingOverlay = true;
        private ObservableCollection<MobileLanguageVM> languages = [];
        private MobileLanguageVM? selectedLanguage;

        public MobileLanguageVM? SelectedLanguage { get => selectedLanguage; set { SetProperty(ref selectedLanguage, value); } }
        public ObservableCollection<MobileLanguageVM> Languages { get => languages; set { SetProperty(ref languages, value); } }
        public bool ShowLoadingOverlay { get => showLoadingOverlay; set { SetProperty(ref showLoadingOverlay, value); } }

        public ICommand PickerCommand { get; }

        public LanguagePageVM()
        {
            PickerCommand = new Command<Picker>(FocusPicker);
            InitData();
        }

        private void FocusPicker(Picker picker)
        {
            picker.Unfocus(); picker.Focus();
#if ANDROID
            var nativePicker = picker.Handler?.PlatformView as MauiPicker;
            nativePicker?.PerformClick();
#endif
        }

        private async void InitData()
        {
            DataService.DataReadyChanged += OnDataReadyChanged;
            await DataService.InitStartData();
            DataService.DataReadyChanged -= OnDataReadyChanged;
        }

        private async void OnDataReadyChanged(bool ready)
        {
            if (ready)
            {
                var langs = await MobileServices.GetData<MobileLanguageVM>("languages.json");
                Languages = new ObservableCollection<MobileLanguageVM>(langs);
                SelectedLanguage = Languages.Count > 0 ? Languages[0] : new MobileLanguageVM() { Name = "Select Language" };
                MainThread.BeginInvokeOnMainThread(() => ShowLoadingOverlay = false);
            }
        }
    }
}