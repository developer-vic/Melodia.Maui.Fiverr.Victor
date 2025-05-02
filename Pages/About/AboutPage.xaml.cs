using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;
using Microsoft.Maui.Controls;

namespace MelodiaTherapy.Pages;

public partial class AboutPage : ContentPage
{
	public ObservableCollection<CollaboratorViewModel> Collaborators { get; set; }
	public string AppVersion => $"Version {AppInfo.VersionString} ({AppInfo.BuildString})";

	public AboutPage()
	{
		InitializeComponent();
		Collaborators =
        [
            new("Mohamed Aanan", "Développeur", linkedIn: "https://www.linkedin.com/in/mohamed-aanan/"),
			new("Tommy Chevalier", "Développeur", linkedIn: "https://www.linkedin.com/in/tommy-chevalier/"),
			new("Samuel Lambert", "Développeur", linkedIn: "https://www.linkedin.com/in/samuel-lambert/"),
			new("FUSOR", "Entreprise", website: "https://www.fusor.be"),
			new("Bruno Vandamme", "CTO", linkedIn: "https://www.linkedin.com/in/bruno-vandamme-4099672/"),
			new("Martin Lecapitaine", "CEO", linkedIn: "https://www.linkedin.com/in/martin-lecapitaine-4555ba14b/"),
			new("Nathalie Garcia", "Artiste sonore", linkedIn: "https://www.linkedin.com/in/nathalie-garcia-kruger-sound-10470b3b/"),
			new("Jared Blake", "Captations sons de nature", linkedIn: "https://www.linkedin.com/in/jared-blake/", website: "https://acousticnature.com/"),
			new("Hans Peter Beck", "Captations sons de nature", youtube: "https://www.youtube.com/@naturescriticalear"),
			new("Aakash Agrawal", "Chercheur neuroscientifique", linkedIn: "https://www.linkedin.com/in/aakashagr/"),
			new("Velmurugan Jayabal", "Neuroscientifique", linkedIn: "https://www.linkedin.com/in/velmurugan-jayabal-07253b6a/"),
			new("Maria Isabel Granados", "Docteure MTC", linkedIn: "https://www.linkedin.com/in/maria-isabel-b%C3%BChler-granados-b894951a9/"),
			new("Calm Whale", "Producteur de musique", website: "https://whaleloryb.bandcamp.com/"),
			new("Sophie Holemans", "Editorialiste", linkedIn: "https://www.linkedin.com/in/sophie-holemans-a4451017"),
			new("Matias Romero", "Producteur de musique, Sono thérapie", linkedIn: "https://www.linkedin.com/in/mat%C3%ADas-romero-acu%C3%B1a-b9865b1b1/"),
			new("Asif Shareef", "Producteur de musique", linkedIn: "https://www.linkedin.com/in/asif-shareef-71779b187/")
		];
		BindingContext = this;
	}

	public class CollaboratorViewModel
	{
		public string Name { get; }
		public string Role { get; }
		public string? LinkedIn { get; }
		public string? Website { get; }
		public string? YouTube { get; }

		public bool HasLinkedIn => !string.IsNullOrEmpty(LinkedIn);
		public bool HasWebsite => !string.IsNullOrEmpty(Website);
		public bool HasYouTube => !string.IsNullOrEmpty(YouTube);

		public ICommand OpenLinkCommand => new Command<string>((url) =>
		{
			if (!string.IsNullOrEmpty(url))
				Launcher.Default.OpenAsync(new Uri(url));
		});

		public CollaboratorViewModel(string name, string role, string? linkedIn = null, string? website = null, string? youtube = null)
		{
			Name = name;
			Role = role;
			LinkedIn = linkedIn;
			Website = website;
			YouTube = youtube;
		}
	}
}