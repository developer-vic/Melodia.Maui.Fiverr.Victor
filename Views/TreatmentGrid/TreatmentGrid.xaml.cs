using MelodiaTherapy.Controllers;
using MelodiaTherapy.Helpers;
using MelodiaTherapy.Models;
using MelodiaTherapy.Pages;

namespace MelodiaTherapy.Views;

public class TreatmentGrid : ContentView
{
	private readonly MelodiaController? melodia;
	private readonly TreatmentController? tcontroller;
	private readonly bool isMobile;
	private readonly Grid grid;

	public TreatmentGrid()
	{
		melodia = ServiceHelper.GetService<MelodiaController>();
		tcontroller = ServiceHelper.GetService<TreatmentController>();
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

		LoadTreatments();
	}

	private void LoadTreatments()
	{
		int columns = isMobile ? 2 : 3;
		grid.ColumnDefinitions.Clear();
		
		for (int i = 0; i < columns; i++)
			grid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));

		if (tcontroller == null)
			return;

		if (tcontroller.Treatments == null || tcontroller.Treatments.Count == 0)
		{
			Task.Factory.StartNew(async () =>
			{
				tcontroller.Treatments = await tcontroller.LoadDemoTreatments();
				MainThread.BeginInvokeOnMainThread(() => InitTreatmentUI(columns));
			});
		}
		else InitTreatmentUI(columns);
	}

	private void InitTreatmentUI(int columns)
	{
		if (tcontroller?.Treatments == null || melodia == null)
			return;

		grid.RowDefinitions.Clear();
		int rows = (int)Math.Ceiling(tcontroller.Treatments.Count / (double)columns);
		
		for (int i = 0; i < rows; i++)
			grid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));

		for (int index = 0; index < tcontroller.Treatments.Count; index++)
		{
			var treatment = tcontroller.Treatments[index];

			GridModel gridModel = new GridModel(
				treatment.Guid,
				treatment.Guid,
				treatment.IsPremium,
				treatment.IconCode,
				treatment.Icon.ToString(),
				treatment.Name,
				treatment.Description);
			GridModel selectedGridModel = new GridModel(
				melodia.SelectedTreatment.Guid,
				melodia.SelectedTreatment.Guid,
				melodia.SelectedTreatment.IsPremium,
				melodia.SelectedTreatment.IconCode,
				melodia.SelectedTreatment.Icon.ToString(),
				melodia.SelectedTreatment.Name,
				melodia.SelectedTreatment.Description);

			var item = new GridItem(gridModel, selectedGridModel, show: true);
			item.OnTapped = () =>
			{
				if (treatment.IsPremium && !AppData.EntitlementIsActive)
				{
					NavigationService.PushPage(new PaywallPage());
					return;
				}

				melodia.SelectedTreatment = treatment;
				Preferences.Default.Set("treatmentId", index);
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