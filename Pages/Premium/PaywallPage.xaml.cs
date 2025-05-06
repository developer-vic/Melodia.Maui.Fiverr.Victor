using System.Collections.ObjectModel;
using System.Windows.Input;
using MelodiaTherapy.Models;
using MelodiaTherapy.Services; 
using MelodiaTherapy.Helpers;
using MelodiaTherapy.Controllers;

namespace MelodiaTherapy.Pages;

public partial class PaywallPage : ContentPage
{
	public PaywallPage()
	{
		InitializeComponent();
		//BindingContext = new PaywallViewModel();
	}
	protected override async void OnAppearing()
	{
		base.OnAppearing();
		if (BindingContext is PaywallViewModel viewModel)
		{
			await viewModel.InitializeAsync();
		}
	}
}

public class PaywallViewModel : BaseViewModel
{
	private readonly LanguageController l = new LanguageController();

	private ObservableCollection<PackageViewModel> _displayPackages = [];
	private PackageViewModel _selectedPackage;
	private bool _isLoading;
	private bool _isConnected;
	private string _promoCode;
	private Dictionary<string, string> _languageResources;
	private ObservableCollection<string> _features;
	private double _discount;

	public ObservableCollection<PackageViewModel> DisplayPackages
	{
		get => _displayPackages;
		set => SetProperty(ref _displayPackages, value);
	}

	public PackageViewModel SelectedPackage
	{
		get => _selectedPackage;
		set => SetProperty(ref _selectedPackage, value);
	}

	public bool IsLoading
	{
		get => _isLoading;
		set => SetProperty(ref _isLoading, value);
	}

	public bool IsConnected
	{
		get => _isConnected;
		set => SetProperty(ref _isConnected, value);
	}

	public bool IsNotConnected => !IsConnected;

	public string PromoCode
	{
		get => _promoCode;
		set => SetProperty(ref _promoCode, value);
	}

	public Dictionary<string, string> LanguageResources
	{
		get => _languageResources;
		set => SetProperty(ref _languageResources, value);
	}

	public ObservableCollection<string> Features
	{
		get => _features;
		set => SetProperty(ref _features, value);
	}

	public double Discount
	{
		get => _discount;
		set => SetProperty(ref _discount, value);
	}

	public ICommand SelectPackageCommand { get; private set; }
	public ICommand ApplyPromoCodeCommand { get; private set; }
	public ICommand RetryConnectionCommand { get; private set; }
	public ICommand OpenTermsCommand { get; private set; }
	public ICommand OpenPrivacyCommand { get; private set; }

	public PaywallViewModel()
	{
		DisplayPackages = new ObservableCollection<PackageViewModel>();
		Features = new ObservableCollection<string>();
		LanguageResources = new Dictionary<string, string>();

		SelectPackageCommand = new Command<PackageViewModel>(async (package) => await PurchasePackageAsync(package));
		ApplyPromoCodeCommand = new Command(async () => await ApplyPromoCodeAsync());
		RetryConnectionCommand = new Command(async () => await InitializeAsync());
		OpenTermsCommand = new Command(async () => await OpenTermsAndConditionsAsync());
		OpenPrivacyCommand = new Command(async () => await OpenPrivacyPolicyAsync());

		LoadLanguageResources();
		LoadFeatures();
	}

	private async void LoadLanguageResources()
	{
		await l.InitializeAsync();
		LanguageResources = l.GetAllLabels();
	}

	private void LoadFeatures()
	{
		Features = new ObservableCollection<string>
			{
				l.GetLabel("Fonctionne hors ligne"),
				l.GetLabel("Accès illimité aux musiques thérapeutiques"),
				l.GetLabel("Choix de la durée")
			};
	}

	public async Task InitializeAsync()
	{
		await CheckInternetConnectionAsync();

		if (IsConnected)
		{
			await GetPackagesAsync();
		}
	}

	private async Task CheckInternetConnectionAsync()
	{
		IsConnected = await InternetService.IsConnectedAsync();
	}

	private async Task GetPackagesAsync(string? promoCode = null)
	{
		if (!IsConnected)
			return;

		try
		{
			IsLoading = true;

			// Get available packages from IAP service
			var packages = await InAppPurchaseService.GetAvailablePackagesAsync();

			// Get subscriptions with promo code applied if specified
			var subscriptions = await SubscriptionService.GetSubscriptionsAsync(promoCode);

			DisplayPackages.Clear();

			// Create package view models from subscription and package data
			var packageViewModels = new List<PackageViewModel>();
			foreach (var subscription in subscriptions)
			{
				var offeringId = subscription.SubscriptionCode.Split('/')[0];
				var packageId = subscription.SubscriptionCode.Split('/')[1];

				string? parentCode = subscription.ParentSubscriptionCode;
				Package? parentPackage = null;

				if (!string.IsNullOrEmpty(parentCode))
				{
					var parentPackageId = parentCode.Split('/')[1];
					var parentOfferingId = parentCode.Split('/')[0];
					parentPackage = packages.FirstOrDefault(p =>
						p.Identifier == parentPackageId && p.OfferingIdentifier == parentOfferingId);
				}

				var package = packages.FirstOrDefault(p =>
					p.Identifier == packageId && p.OfferingIdentifier == offeringId);

				if (package != null)
				{
					var packageViewModel = new PackageViewModel
					{
						Package = package,
						Description = l.GetLabel(subscription.Description ?? string.Empty),
						OldPrice = parentPackage?.Price,
						IsSelected = false
					};

					packageViewModels.Add(packageViewModel);
				}
			}

			// Sort by price
			packageViewModels = packageViewModels
				.OrderBy(p => p.Package.Price)
				.ToList();

			// Mark the last package as popular (highest price)
			if (packageViewModels.Count > 0)
			{
				packageViewModels.Last().IsPopular = true;
			}

			// Calculate discount for popular package
			if (packageViewModels.Count >= 2)
			{
				double price1 = packageViewModels.First().Package.Price;
				double price2 = packageViewModels.Last().Package.Price;
				double valueRatio = ValueInMonths(packageViewModels.Last().Package) /
								   ValueInMonths(packageViewModels.First().Package);

				Discount = valueRatio * price1 - price2;
				packageViewModels.Last().Discount = Discount;
				packageViewModels.Last().CurrencyCode = packageViewModels.Last().Package.CurrencyCode;
			}

			// Add to UI collection
			foreach (var packageViewModel in packageViewModels)
			{
				DisplayPackages.Add(packageViewModel);
			}

			// Select last package by default
			if (DisplayPackages.Count > 0)
			{
				SelectedPackage = DisplayPackages.Last();
				SelectedPackage.IsSelected = true;
			}
		}
		catch (Exception ex)
		{
			await NavigationService.DisplayAlert("Error", ex.Message, l.GetLabel("OK"));
		}
		finally
		{
			IsLoading = false;
		}
	}

	private double ValueInMonths(Package package)
	{
		// W = weeks, M = months, Y = years
		// P1W = 1 week, P1M = 1 month, P1Y = 1 year
		string period = package.SubscriptionPeriod;

		if (period.Contains("W"))
		{
			string numStr = new string(period.Where(char.IsDigit).ToArray());
			if (double.TryParse(numStr, out double num))
			{
				return num / 4.34524;
			}
		}
		else if (period.Contains("M"))
		{
			string numStr = new string(period.Where(char.IsDigit).ToArray());
			if (double.TryParse(numStr, out double num))
			{
				return num;
			}
		}
		else if (period.Contains("Y"))
		{
			string numStr = new string(period.Where(char.IsDigit).ToArray());
			if (double.TryParse(numStr, out double num))
			{
				return num * 12;
			}
		}

		return 0.0;
	}

	private async Task PurchasePackageAsync(PackageViewModel package)
	{
		if (package == null)
			return;

		try
		{
			IsLoading = true;

			foreach (var item in DisplayPackages)
			{
				item.IsSelected = (item == package);
			}

			SelectedPackage = package;

			// Perform purchase
			var purchaseResult = await InAppPurchaseService.PurchasePackageAsync(package.Package);

			if (purchaseResult.IsSuccessful)
			{
				// Update application premium status
				AppData.IsPremium = purchaseResult.IsPremiumActive;
				AppData.SubscriptionCode = $"{package.Package.OfferingIdentifier}/{package.Package.Identifier}";

				// Save subscription code in preferences
				Preferences.Set("subscriptionCode", AppData.SubscriptionCode);

				// Send device info to server
				await DataService.SendDeviceInfoAsync();

				// Navigate to start page if premium is active
				if (AppData.IsPremium)
				{
					NavigationService.NavigateToStartPageAsync();
				}
			}
			else
			{
				await NavigationService.DisplayAlert("Purchase Failed", purchaseResult.ErrorMessage, l.GetLabel("OK"));
			}
		}
		catch (Exception ex)
		{
			await NavigationService.DisplayAlert("Error", ex.Message, l.GetLabel("OK"));
		}
		finally
		{
			IsLoading = false;
		}
	}

	private async Task ApplyPromoCodeAsync()
	{
		if (string.IsNullOrWhiteSpace(PromoCode))
		{
			await NavigationService.DisplayAlert(
				"",
				l.GetLabel("Veuillez entrer un code promo"),
				l.GetLabel("OK"));
			return;
		}

		PromoCode = PromoCode.ToUpper();
		await GetPackagesAsync(PromoCode);
	}

	private async Task OpenTermsAndConditionsAsync()
	{
		await Browser.OpenAsync(Constants.TermsAndConditionsLink, BrowserLaunchMode.External);
	}

	private async Task OpenPrivacyPolicyAsync()
	{
		await Browser.OpenAsync(Constants.PrivacyPolicyLink, BrowserLaunchMode.External);
	}
}
