using System.Collections.ObjectModel;
using MelodiaTherapy.Models;

namespace MelodiaTherapy.Pages;

public partial class DirectoryExplorerPage : ContentPage
{
	public ObservableCollection<FileItem> DirectoryContents { get; set; } = new();

	private string _currentPath = "";
	private int _nbFolders = 0;
	private int _nbFiles = 0;

	public DirectoryExplorerPage()
	{
		InitializeComponent();
		BindingContext = this;
		LoadDocumentsDirectory();
	}

	private void LoadDocumentsDirectory()
	{
		var docDir = FileSystem.AppDataDirectory;
		_currentPath = docDir;
		LoadDirectoryContents(docDir);
	}

	private void LoadDirectoryContents(string path)
	{
		if (!Directory.Exists(path)) return;

		DirectoryContents.Clear();
		var contents = Directory.GetFileSystemEntries(path);

		_nbFiles = 0;
		_nbFolders = 0;

		foreach (var item in contents)
		{
			bool isDir = Directory.Exists(item);
			if (isDir) _nbFolders++; else _nbFiles++;

			DirectoryContents.Add(new FileItem
			{
				Path = item,
				IsDirectory = isDir,
				Icon = isDir ? "folder.png" : "file.png"
			});
		}

		StatsLabel.Text = $"Contents (nb folders: {_nbFolders}, nb files: {_nbFiles}):";
		StatsLabel.IsVisible = true;
	}

	private void OnRefreshClicked(object sender, EventArgs e)
	{
		LoadDirectoryContents(_currentPath);
	}

	private void OnItemTapped(object sender, TappedEventArgs e)
	{
		var binidngObj = (Grid)sender;
		var tapped = binidngObj.BindingContext as FileItem;
		if (tapped != null && tapped.IsDirectory)
		{
			_currentPath = tapped.Path;
			LoadDirectoryContents(tapped.Path);
		}
	}
}