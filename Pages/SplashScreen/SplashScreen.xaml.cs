using System.Diagnostics;
using MelodiaTherapy.Services;
using MelodiaTherapy.Stores;

namespace MelodiaTherapy.Pages
{
	public partial class SplashScreen : ContentPage
	{
		private System.Timers.Timer? retryTimer;
		private bool isConnected = false;
		private bool retry = false;

		public SplashScreen()
		{
			InitializeComponent();
			CheckInternet();
			InitDataStream();
		}

		private async void CheckInternet()
		{
			var current = Connectivity.NetworkAccess;
			isConnected = current == NetworkAccess.Internet;
			OnPropertyChanged(nameof(isConnected));
			UpdateUI();

			if (isConnected)
			{
				await InitStartData();
			}
		}

		private async Task InitStartData()
		{
			try
			{
				DataService.DataReadyChanged += OnDataReadyChanged;
				await DataService.InitStartData();
				DataService.DataReadyChanged -= OnDataReadyChanged;
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex);
				await DisplayAlert("Error", ex.Message, "OK");
			}
		}

		private void OnDataReadyChanged(bool ready)
		{
			if (ready)
			{
				retry = false;
				retryTimer?.Stop();
				if (Application.Current != null)
				{
					Application.Current.Windows[0].Page = LanguageStore.LanguageChosen
						? new NavigationPage(new StartPage())
						: new LanguagePage();
				}
			}
		}

		private void InitDataStream()
		{ 
			retryTimer = new System.Timers.Timer(20000);
			retryTimer.Elapsed += (s, e) =>
			{
				retry = true;
				MainThread.BeginInvokeOnMainThread(() => UpdateUI());
			};
			retryTimer.Start();
		}

		private void UpdateUI()
		{
			RetryButton.IsVisible = retry;
			ConnectionMessage.Text = isConnected
				? "Loading..."
				: "Connect to internet to continue ...";
			ProgressIndicator.IsVisible = isConnected;
			ErrorBox.IsVisible = !isConnected;
		}

		private void RetryButton_Clicked(object sender, EventArgs e)
		{
			CheckInternet();
		}
	}
}