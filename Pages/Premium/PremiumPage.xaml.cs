using System.Globalization;
using System.Windows.Input;
using MelodiaTherapy.Controllers;
using MelodiaTherapy.Helpers;

namespace MelodiaTherapy.Pages;

public partial class PremiumPage : ContentPage
{
	public PremiumPage()
	{
		InitializeComponent();
		BindingContext = new PremiumViewModel();
	}

	private class PremiumViewModel : BindableObject
	{
		public bool IsTrial { get; } = true;

		public int DaysLeft { get; } = 0;

		private readonly LanguageController l = new LanguageController();

		public PremiumViewModel()
		{
			int trialDays = AppData.PeriodFreeTrial;
			DateTime createdOn = AppData.CreatedOn ?? DateTime.UtcNow;
			DaysLeft = (createdOn.AddDays(trialDays) - DateTime.UtcNow).Days + 1;
			IsTrial = AppData.IsFreeTrial ?? false;

			OpenTermsCommand = new Command(async () =>
			{
				await Launcher.Default.OpenAsync(Constants.TermsAndConditionsLink);
			});

			OpenPrivacyCommand = new Command(async () =>
			{
				await Launcher.Default.OpenAsync(Constants.PrivacyPolicyLink);
			});
			
		}

		public string PremiumTitle => l.GetLabel("Premium");

		public string TrialStatusText => IsTrial
			? l.GetLabel("Active Free Trial")
			: l.GetLabel("Abonnement Premium actif");

		public string TrialDaysLeftText => IsTrial
			? $"{l.GetLabel("Days Left for the Free Trial:")} {DaysLeft} {l.GetLabel("Days")}"
			: string.Empty;

		public string SubscriptionStartDateText =>
			$"- {l.GetLabel("Date d'abonnement:")}\n{FormatDate(AppData.OriginalPurchaseDate)} (GMT)";

		public string SubscriptionEndDateText =>
			$"- {l.GetLabel("Date d'expiration:")}\n{FormatDate(AppData.ExpirationDate)} (GMT)";

		public string TermsText => l.GetLabel("Conditions d'utilisation");
		public string PrivacyText => l.GetLabel("Politique de confidentialité");

		public ICommand OpenTermsCommand { get; }
		public ICommand OpenPrivacyCommand { get; }

		private string FormatDate(string? isoDate)
		{
			if (string.IsNullOrEmpty(isoDate)) return "/";
			return DateTime.TryParse(isoDate, out var parsedDate)
				? parsedDate.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture)
				: "/";
		}
	}
}