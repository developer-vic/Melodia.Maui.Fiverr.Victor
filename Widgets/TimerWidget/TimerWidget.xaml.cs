using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using MelodiaTherapy.Helpers;

namespace MelodiaTherapy.Widgets;

public partial class TimerWidget : ContentView
{
	private System.Timers.Timer? _timer;
	public TimerWidget()
	{
		InitializeComponent();
		StartTimer();
	}

	private void StartTimer()
	{
		_timer?.Stop();

		_timer = new System.Timers.Timer(Constants.InactivityDuration.TotalMilliseconds)
		{
			AutoReset = false
		};
		_timer.Elapsed += (s, e) =>
		{
			Console.WriteLine("Timer Widget expired");
			MainThread.BeginInvokeOnMainThread(async () =>
			{
				var snackbarOptions = new SnackbarOptions
				{
					BackgroundColor = Color.FromArgb("#76cec5"),
					TextColor = Colors.White,
					ActionButtonTextColor = Colors.Black,
					CornerRadius = 8
				};

				var snackbar = Snackbar.Make(
					$"Vous avez été inactif pendant {Constants.InactivityDuration.TotalMilliseconds} minutes",
					action: () => { /* optional dismiss action */ },
					actionButtonText: "OK",
					duration: TimeSpan.FromSeconds(5),
					visualOptions: snackbarOptions
				);

				NavigationService.NavigateToStartPageAsync();
				
				await snackbar.Show();
				
			});
		};
		_timer.Start();
		Console.WriteLine("Timer Widget started");
	}

	// protected override void OnDisappearing()
	// {
	// 	base.OnDisappearing();
	// 	_timer?.Stop();
	// 	_timer?.Dispose();
	// 	Console.WriteLine("Timer Widget disposed");
	// }
}