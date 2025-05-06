using System.Windows.Input;
using MelodiaTherapy.Dialogs;
using MelodiaTherapy.Helpers;

namespace MelodiaTherapy.Pages;

public partial class StoragePage : ContentPage
{
	public StoragePage()
	{
		InitializeComponent();
		BindingContext = new StoragePageVM();
	}
}

internal class StoragePageVM
{
	public ICommand MyCommand { get; set; }
	public StoragePageVM()
	{
		MyCommand = new Command<string>(OnButtonClicked);
	}

	private void OnButtonClicked(string par)
	{
		switch (par)
		{
			case "UsedSpace":
				OnUsedSpaceClicked();
				break;
			case "ExploreFolder":
				OnExploreFolderClicked();
				break;
			case "DownloadCenter":
				OnDownloadCenterClicked();
				break;
			case "DownloadAll":
				OnDownloadAllClicked();
				break;
			case "CancelAllDownload":
				OnCancelAllDownloadClicked();
				break;
			case "DeleteFiles":
				OnDeleteFilesClicked();
				break;
			case "ResetApp":
				OnResetAppClicked();
				break;
		}
	}

	private void OnUsedSpaceClicked()
	{
		// used space logic
	}

	private void OnExploreFolderClicked()
	{
		NavigationService.PushPage(new DirectoryExplorerPage());
	}

	private void OnDownloadCenterClicked()
	{
		NavigationService.PushPage(new DownloadCenterPage());
	}

	private void OnDownloadAllClicked()
	{
		// download all logic 
	}

	private async void OnCancelAllDownloadClicked()
	{
		var modal = new ConfirmModalPage("Cancel all Downloads?", "Are you sure you want to cancel all downloads?", "Supprimer", "Cancel");
		NavigationService.OpenDialog(modal);

		bool confirmed = await modal.ShowAsync();
		if (confirmed)
		{
			// Perform cancel download
		}
	}

	private async void OnDeleteFilesClicked()
	{
		var modal = new ConfirmModalPage("Delete files?", "Are you sure you want to delete all downloaded files?", "Supprimer", "Cancel");
		NavigationService.OpenDialog(modal);

		bool confirmed = await modal.ShowAsync();
		if (confirmed)
		{
			// Perform delete files
		}
	}

	private async void OnResetAppClicked()
	{
		var modal = new ConfirmModalPage("Reset", "Are you sure you want to delete all the data from the appllication?", "Confirm", "Cancel");
		NavigationService.OpenDialog(modal);

		bool confirmed = await modal.ShowAsync();
		if (confirmed)
		{
			// Perform reset app
		}
	}
}