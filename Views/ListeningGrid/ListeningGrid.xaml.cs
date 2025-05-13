using MelodiaTherapy.Controllers;
using MelodiaTherapy.Helpers;
using MelodiaTherapy.Models;
using MelodiaTherapy.Pages;

namespace MelodiaTherapy.Views;

public class ListeningGrid : ContentView
{
    private readonly MelodiaController? melodia;
    private readonly ListenTypeController? acontroller;
    private readonly bool isMobile;
    private readonly Grid grid;

    public ListeningGrid()
    {
        melodia = ServiceHelper.GetService<MelodiaController>();
        acontroller = ServiceHelper.GetService<ListenTypeController>();
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

        LoadListenTypes();
    }

    private async void LoadListenTypes()
    {
        int columns = isMobile ? 2 : 3;
        grid.ColumnDefinitions.Clear();
        for (int i = 0; i < columns; i++)
            grid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));

        if (acontroller == null)
            return;
        if (melodia == null)
            return;

        if (acontroller.ListenTypes == null)
            acontroller.ListenTypes = await acontroller.LoadDemoListenings();

        grid.RowDefinitions.Clear();
        int rows = (int)Math.Ceiling(acontroller.ListenTypes.Count / (double)columns);
        for (int i = 0; i < rows; i++)
            grid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));

        for (int index = 0; index < acontroller.ListenTypes.Count; index++)
        {
            var Listening = acontroller.ListenTypes[index];

            GridModel gridModel = new GridModel(
                Listening.Guid,
                Listening.Guid,
                false,
                Listening.IconCode,
                Listening.Icon.ToString(),
                Listening.Name,
                Listening.Description);
            GridModel selectedGridModel = new GridModel(
                melodia.SelectedListeningMode.Guid,
                melodia.SelectedListeningMode.Guid,
                false,
                melodia.SelectedListeningMode.IconCode,
                melodia.SelectedListeningMode.Icon.ToString(),
                melodia.SelectedListeningMode.Name,
                melodia.SelectedListeningMode.Description);

            var item = new GridItem(gridModel, selectedGridModel, show: true);
            item.OnTapped = () =>
            {
                // if (Listening.IsPremium && !AppData.EntitlementIsActive)
                // {
                // 	NavigationService.PushPage(new PaywallPage());
                // 	return;
                // }

                melodia.SelectedListeningMode = Listening;
                Preferences.Default.Set("listenTypeId", index);
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