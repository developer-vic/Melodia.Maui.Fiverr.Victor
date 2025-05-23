using MelodiaTherapy.Controllers;
using MelodiaTherapy.Helpers;
using MelodiaTherapy.Models;
using MelodiaTherapy.Pages;

namespace MelodiaTherapy.Views;

public class ListeningGrid : ContentView
{
    private MelodiaController? melodia;
    private ListenTypeController? ltcontroller;
    private bool isMobile;
    private Grid? grid;

    public ListeningGrid()
    {
        Task.Factory.StartNew(() =>
        {
            melodia = ServiceHelper.GetService<MelodiaController>();
            ltcontroller = ServiceHelper.GetService<ListenTypeController>();
            isMobile = DeviceInfo.Idiom == DeviceIdiom.Phone;
            InitData();
        });
    }

    private void InitData()
    {
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
        if (grid == null)
            return;

        int columns = isMobile ? 2 : 3;
        grid.ColumnDefinitions.Clear();
        for (int i = 0; i < columns; i++)
            grid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));

        if (ltcontroller == null)
            return;
        if (melodia == null)
            return;

        if (ltcontroller.ListenTypes == null)
            ltcontroller.ListenTypes = await ltcontroller.LoadDemoListenings();

        grid.RowDefinitions.Clear();
        int rows = (int)Math.Ceiling(ltcontroller.ListenTypes.Count / (double)columns);
        for (int i = 0; i < rows; i++)
            grid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));

        for (int index = 0; index < ltcontroller.ListenTypes.Count; index++)
        {
            var Listening = ltcontroller.ListenTypes[index];

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