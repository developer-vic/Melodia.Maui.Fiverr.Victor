namespace MelodiaTherapy.Dialogs;

public partial class ConfirmModalPage : ContentPage
{
	private readonly TaskCompletionSource<bool> _taskCompletionSource = new();

	public ConfirmModalPage(string title, string message, string yesText, string noText)
	{
		InitializeComponent();
		TitleLabel.Text = title;
		MessageLabel.Text = message;
		btnYes.Text = yesText;
		btnNo.Text = noText;
	}

	public Task<bool> ShowAsync()
	{
		return _taskCompletionSource.Task;
	}

	private async void OnYesClicked(object sender, EventArgs e)
	{
		_taskCompletionSource.TrySetResult(true);
		await Navigation.PopModalAsync();
	}

	private async void OnNoClicked(object sender, EventArgs e)
	{
		_taskCompletionSource.TrySetResult(false);
		await Navigation.PopModalAsync();
	}
}