using MelodiaTherapy.Controllers;
using MelodiaTherapy.Helpers;
using MelodiaTherapy.Models;
using MelodiaTherapy.Pages;

namespace MelodiaTherapy.Views;

public class AmbianceGrid : ContentView
{
	private readonly MelodiaController? melodia;
	private readonly AmbianceController? acontroller;
	private readonly bool isMobile;
	private readonly Grid grid;

	public AmbianceGrid()
	{
		melodia = ServiceHelper.GetService<MelodiaController>();
		acontroller = ServiceHelper.GetService<AmbianceController>();
		isMobile = DeviceInfo.Idiom == DeviceIdiom.Phone;

		grid = new Grid
		{
			ColumnSpacing = 12,
			RowSpacing = 12,
		};

		Content = new ScrollView
		{
			Content = grid
		};

		LoadAmbiances();
	}

	private void LoadAmbiances()
	{
		int columns = isMobile ? 2 : 3;
		grid.ColumnDefinitions.Clear();
		
		for (int i = 0; i < columns; i++)
			grid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));

		if (acontroller == null || melodia == null)
			return; 

		if (acontroller.Ambiances == null || acontroller.Ambiances.Count == 0)
		{
			Task.Factory.StartNew(async () =>
			{
				acontroller.Ambiances = await acontroller.LoadDemoAmbiances();
				MainThread.BeginInvokeOnMainThread(() => InitAmbianceUI(columns));
			});
		}
		else InitAmbianceUI(columns);
	}

	private void InitAmbianceUI(int columns)
	{
		if (acontroller?.Ambiances == null || melodia == null) return;

		grid.RowDefinitions.Clear();
		int rows = (int)Math.Ceiling(acontroller.Ambiances.Count / (double)columns);
		
		for (int i = 0; i < rows; i++)
			grid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));

		for (int index = 0; index < acontroller.Ambiances.Count; index++)
		{
			var ambiance = acontroller.Ambiances[index];

			GridModel gridModel = new GridModel(
				ambiance.Guid,
				ambiance.Guid,
				ambiance.IsPremium,
				ambiance.IconCode,
				ambiance.Icon.ToString(),
				ambiance.Name,
				ambiance.Description);
			GridModel selectedGridModel = new GridModel(
				melodia.SelectedAmbiance.Guid,
				melodia.SelectedAmbiance.Guid,
				melodia.SelectedAmbiance.IsPremium,
				melodia.SelectedAmbiance.IconCode,
				melodia.SelectedAmbiance.Icon.ToString(),
				melodia.SelectedAmbiance.Name,
				melodia.SelectedAmbiance.Description);

			var item = new GridItem(gridModel, selectedGridModel, show: true);
			item.OnTapped = () =>
			{
				if (ambiance.IsPremium && !AppData.EntitlementIsActive)
				{
					NavigationService.PushPage(new PaywallPage());
					return;
				}

				melodia.SelectedAmbiance = ambiance;
				Preferences.Default.Set("ambianceId", index);
				melodia.NextPage();
			};

			int row = index / columns;
			int col = index % columns;

			Grid.SetRow(item, row);
			Grid.SetColumn(item, col);
			grid.Children.Add(item);
		}
	}
}