using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using MelodiaTherapy.Controllers;
using MelodiaTherapy.Helpers;
using MelodiaTherapy.Models;
using MelodiaTherapy.Pages;
using Microsoft.Maui.Controls.Shapes;

namespace MelodiaTherapy.Views
{
    public class ListeningDurationGrid : ContentView
    {
        private readonly MelodiaController? melodia;
        private readonly ListenDurationController? ldcontroller;

        private ObservableCollection<ListenDurationModel>? _listenDurations;
        private ListenDurationModel? _selectedDuration;
        private readonly Grid _grid;

        public ListeningDurationGrid()
        {
            melodia = ServiceHelper.GetService<MelodiaController>();
            ldcontroller = ServiceHelper.GetService<ListenDurationController>();

            _grid = new Grid
            {
                ColumnSpacing = 12,
                RowSpacing = 12
            };

            InitiateData();
        }

        private async void InitiateData()
        {
            if (ldcontroller == null || melodia == null) return;

            var tempDurations = await ldcontroller.LoadDemoListeningDuration();
            _listenDurations = new ObservableCollection<ListenDurationModel>(tempDurations);
            _selectedDuration = _listenDurations.FirstOrDefault();

            int columns = 2;
            for (int i = 0; i < columns; i++)
                _grid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));

            int rows = (_listenDurations.Count + 1) / columns;
            for (int i = 0; i < rows; i++)
                _grid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));

            for (int i = 0; i < _listenDurations.Count; i++)
            {
                var item = _listenDurations[i];
                var view = CreateGridItem(item, i);
                int row = i / columns;
                int col = i % columns;

                Grid.SetRow(view, row);
                Grid.SetColumn(view, col);
                _grid.Children.Add(view);
            }

            Content = _grid;
        }

        private View CreateGridItem(ListenDurationModel item, int index)
        {
            var isSelected = item.Duration == _selectedDuration?.Duration;

            var frame = new Border
            {
                StrokeShape = new RoundRectangle { CornerRadius = 12 },
                Padding = 12,
                Stroke = Colors.Transparent,
                BackgroundColor = isSelected ? Color.FromArgb("#76cec5") : Color.FromArgb("#41404E"),
                Content = new VerticalStackLayout
                {
                    Padding = 12,
                    Spacing = 10,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                    Children =
                    {
                        new Image
                        {
                            Source = "schedule_icon.png",
                            WidthRequest = 28,
                            HeightRequest = 28,
                            Opacity = item.IsPremium && !AppData.EntitlementIsActive ? 0.4 : 1.0
                        },
                        new Label
                        {
                            Text = item.Description,
                            FontSize = 14,
                            HorizontalTextAlignment = TextAlignment.Center,
                            TextColor = item.IsPremium && !AppData.EntitlementIsActive
                                ? Colors.LightGray
                                : isSelected ? Colors.Black : Colors.LightGray
                        }
                    }
                }
            };

            var lockIcon = new Label
            {
                Text = item.IsPremium && !AppData.EntitlementIsActive ? "\uD83D\uDD12" : "", // 🔒
                TextColor = Colors.Orange,
                FontSize = 16,
                HorizontalOptions = LayoutOptions.End,
                VerticalOptions = LayoutOptions.Start,
                Margin = new Thickness(0, 0, 5, 0)
            };

            var tapGesture = new TapGestureRecognizer();
            tapGesture.Tapped += async (s, e) =>
            {
                if (item.IsPremium && !AppData.EntitlementIsActive)
                {
                    NavigationService.PushPage(new PaywallPage());
                    return;
                }

                _selectedDuration = item;
                Preferences.Set("listenDurationId", index);
                melodia?.NextPage();
            };

            var gridOverlay = new Grid();
            gridOverlay.Children.Add(frame);
            gridOverlay.Children.Add(lockIcon);
            gridOverlay.GestureRecognizers.Add(tapGesture);

            return gridOverlay;
        }
    }
}