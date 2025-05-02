namespace MelodiaTherapy.Dialogs;

public partial class MyIdDialog : ContentPage
{
	public MyIdDialog(string deviceIdInfo)
	{
		InitializeComponent();
		idEntry.Text = deviceIdInfo;
	}
	private async void OnCloseClicked(object sender, EventArgs e)
	{
		await Navigation.PopModalAsync();
	}
}