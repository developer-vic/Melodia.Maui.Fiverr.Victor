using MelodiaTherapy.Helpers;

namespace MelodiaTherapy.Widgets;

public partial class TimerWidget : ContentView
{
	private System.Timers.Timer? _timer;
	private readonly TimeSpan _inactivityDuration = TimeSpan.FromMinutes(5); // Or use Constants.InactivityDuration

	public TimerWidget()
	{
		InitializeComponent();
		StartTimer();
	}

	private void StartTimer()
	{
		_timer?.Stop();

		_timer = new System.Timers.Timer(_inactivityDuration.TotalMilliseconds)
		{
			AutoReset = false
		};
		_timer.Elapsed += (s, e) =>
		{
			MainThread.BeginInvokeOnMainThread(async () =>
			{
				await NavigationService.DisplayAlert("Session expirée",
					$"Vous avez été inactif pendant {_inactivityDuration.TotalMinutes} minutes",
					"OK");
				NavigationService.NavigateToStartPageAsync();
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