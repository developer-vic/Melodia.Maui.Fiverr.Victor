using System.Windows.Input;
using MelodiaTherapy.Helpers;
using MelodiaTherapy.Services;

namespace MelodiaTherapy.Pages
{
	public partial class StartPage : ContentPage, IMenuDrawerVM
	{
		public StartPage()
		{
			InitializeComponent();
			BindingContext = new StartPageVM();
		}

		public void OnDrawerCloseClicked()
		{
			HideDrawer();
		}

		private void OnDrawerClickOut(object sender, EventArgs e)
		{
			HideDrawer();
		}

		private void ShowDrawer()
		{
			// Animate drawer to slide in from right
			menuDrawerContainer.TranslationX = this.Width;
			menuDrawerContainer.IsVisible = true;

			menuDrawerContainer.FadeTo(1, 250, Easing.CubicOut);
			menuDrawerContainer.TranslateTo(0, 0, 250, Easing.CubicOut);
		}

		public async void HideDrawer()
		{
			// Animate drawer to slide out to right
			await Task.WhenAll(
				menuDrawerContainer.FadeTo(0, 250, Easing.CubicIn),
				menuDrawerContainer.TranslateTo(this.Width, 0, 250, Easing.CubicIn)
			);

			menuDrawerContainer.IsVisible = false;
		}

		private void OnOpenMenuClicked(object sender, EventArgs e)
		{
			ShowDrawer();
		}

		private async void OnStartClicked(object sender, EventArgs e)
		{
			try
			{
				await PremiumService.CheckPremium();

				if (DataService.IsReady)
				{
					NavigationService.SetAsMainPage(new ProgressPage());
				}
				else
				{
					await DisplayAlert("Info", "Données pas prêtes, patientez svp...", "OK");
				}
			}
			catch (Exception ex)
			{
				await DisplayAlert("Erreur", ex.Message, "OK");
			}
		}
	}

	internal class StartPageVM : BaseViewModel
	{
		public ICommand OpenSocialMediaCommand { get; set; }
		public StartPageVM()
		{
			OpenSocialMediaCommand = new Command<string>(OpenSocialMediaClicked);
		}

		private async void OpenSocialMediaClicked(string platform)
		{
			string? url = platform switch
			{
				"facebook" => "https://www.facebook.com/sharer/sharer.php?u=https%3A//www.melodiatherapy.com/",
				"messenger" => "fb-messenger://share/?link=www.melodiatherapy.com",
				"twitter" => "https://twitter.com/intent/tweet?text=https%3A//www.melodiatherapy.com/",
				"linkedin" => "https://www.linkedin.com/shareArticle?mini=true&url=https%3A//www.melodiatherapy.com/",
				"whatsapp" => "https://wa.me/?text=https://www.melodiatherapy.com",
				"email" => "mailto:?subject=Melodia Therapy&body=https://www.melodiatherapy.com",
				"copy" => null, // handle separately
				"share" => null, // handle separately
				_ => null
			};

			if (platform == "copy")
			{
				await Clipboard.Default.SetTextAsync("https://melodiatherapy.com/");
				NavigationService.ToastText("url copied");
				return;
			}

			if (platform == "share")
			{
				await Share.Default.RequestAsync(new ShareTextRequest
				{
					Title = "Melodia Therapy",
					Text = "https://www.melodiatherapy.com",
					Subject = "Melodia Therapy"
				});
				return;
			}

			if (!string.IsNullOrEmpty(url))
			{
				await Launcher.Default.OpenAsync(new Uri(url));
			}
		}

	}
}