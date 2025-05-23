using System.Windows.Input;
using MelodiaTherapy.Dialogs;
using MelodiaTherapy.Helpers;
using MelodiaTherapy.Interfaces;
using MelodiaTherapy.Pages;
using MelodiaTherapy.Services;

namespace MelodiaTherapy.Views;

public partial class MenuDrawerView : ContentView
{
	public MenuDrawerView()
	{
		InitializeComponent();
		BindingContext = new MenuDrawerViewModel();
	}
}

public class MenuDrawerViewModel : BaseViewModel
{
	private string? versionInfo;
	private string? deviceIdInfo;

	public string? VersionInfo { get => versionInfo; set { SetProperty(ref versionInfo, value); } }
	public string? DeviceIdInfo { get => deviceIdInfo; set { SetProperty(ref deviceIdInfo, value); } }

	public ICommand MyCommand { get; set; }

	public MenuDrawerViewModel()
	{
		MyCommand = new Command<string>(OnButtonClicked);
		InitData();
	}

	private void InitData()
	{
		Task.Factory.StartNew(() =>
		{
			var deviceIdService = ServiceHelper.GetService<IDeviceIdService>();

			MainThread.BeginInvokeOnMainThread(() =>
			{
				VersionInfo = $"{AppInfo.VersionString} ({AppInfo.BuildString})";
				DeviceIdInfo = deviceIdService?.GetDeviceId() ?? Guid.NewGuid().ToString().ToUpper();
			});
		});
	}

	private async void OnButtonClicked(string par)
	{
		switch (par)
		{
			case "CloseDrawer":
				NavigationService.CloseDrawer();
				break;
			case "NavigateToLanguage":
				NavigationService.PushPage(new LanguagePage());
				break;
			case "NavigateToPremium":
				if (PremiumService.IsEntitlementActive)
					NavigationService.PushPage(new PremiumPage());
				else
					NavigationService.PushPage(new PaywallPage());
				break;
			case "NavigateToStorage":
				NavigationService.PushPage(new StoragePage());
				break;
			// case "NavigateToSaved":
			// 	NavigationService.NavigateTo(typeof(SavedPage));
			// 	break;
			case "OpenInstagram":
				await Browser.OpenAsync(new Uri("https://www.instagram.com/melodiatherapy"), BrowserLaunchMode.External);
				break;
			case "OpenFacebook":
				await Browser.OpenAsync(new Uri("https://www.facebook.com/MelodiaTherapy"), BrowserLaunchMode.External);
				break;
			case "ContactUs":
				await Browser.OpenAsync(new Uri("https://www.melodiatherapy.com/contact/"), BrowserLaunchMode.External);
				break;
			case "RateApp":
				string storeUrl = DeviceInfo.Platform == DevicePlatform.Android
					? "https://play.google.com/store/apps/details?id=com.app.melodiatherapy"
					: "https://apps.apple.com/app/melodia-therapy/id6448510044";
				await Browser.OpenAsync(new Uri(storeUrl), BrowserLaunchMode.External);
				break;
			case "OpenFaq":
				string faqUrl = LanguageService.CurrentLanguage switch
				{
					"fr" => "https://www.melodiatherapy.com/fr/faq-fr/",
					"es" => "https://www.melodiatherapy.com/es/faq-es/",
					_ => "https://www.melodiatherapy.com/faq/"
				};
				await Browser.OpenAsync(new Uri(faqUrl), BrowserLaunchMode.External);
				break;
			case "OpenTerms":
				await Browser.OpenAsync(new Uri(Constants.TermsAndConditionsLink), BrowserLaunchMode.External);
				break;
			case "OpenLegalNotices":
				await Browser.OpenAsync(new Uri(Constants.LegalNoticesLink), BrowserLaunchMode.External);
				break;
			case "OpenPrivacyPolicy":
				await Browser.OpenAsync(new Uri(Constants.PrivacyPolicyLink), BrowserLaunchMode.External);
				break;
			case "ShowAboutDialog":
				NavigationService.PushPage(new AboutPage());
				break;
			case "CopyId":
				await Clipboard.SetTextAsync(DeviceIdInfo);
				NavigationService.OpenDialog(new MyIdDialog(DeviceIdInfo));
				break;
		}
	}
}
