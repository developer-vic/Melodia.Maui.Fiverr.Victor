using System.Diagnostics;
using MelodiaTherapy.Helpers;
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

		private void CheckInternet()
		{
			var current = Connectivity.NetworkAccess;
			isConnected = current == NetworkAccess.Internet;
			OnPropertyChanged(nameof(isConnected));
			UpdateUI();

			if (isConnected)
			{
				Task.Factory.StartNew(() =>
				{
					Dispatcher.StartTimer(TimeSpan.FromSeconds(2), InitStartData);
				});
			}
		}

		private bool InitStartData()
		{
			// DataService.DataReadyChanged += OnDataReadyChanged;
			// DataService.InitStartData();
			// DataService.DataReadyChanged -= OnDataReadyChanged;

			OnDataReadyChanged(true);
			return false;
		}

		private void OnDataReadyChanged(bool ready)
		{
			if (ready)
			{
				retry = false;
				retryTimer?.Stop();

				NavigationService.SetAsMainPage(LanguageStore.LanguageChosen
					? new StartPage() : new LanguagePage());
			}
		}

		private void InitDataStream()
		{
			retryTimer = new System.Timers.Timer(5000);
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