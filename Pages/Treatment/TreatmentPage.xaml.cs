namespace MelodiaTherapy.Pages;

public partial class TreatmentPage : ContentView
{
	public TreatmentPage()
	{
		InitializeComponent();
		TitleLabel.Text = "Choisissez votre soin";
		SubtitleLabel.Text = "Chaque soin propose une fréquence spécifique de battement binaural et de tons isochrones, adaptée pour agir sur votre cerveau.";
	}
}