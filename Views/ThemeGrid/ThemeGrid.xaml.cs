using MelodiaTherapy.Controllers;
using MelodiaTherapy.Helpers;
using MelodiaTherapy.Models;
using MelodiaTherapy.Pages;
using Microsoft.Maui.Controls.Shapes;

namespace MelodiaTherapy.Views
{
    public class ThemeGrid : ContentView
    {
        private MelodiaController? _melodiaController;
        private ThemeController? _themeController;
        private LanguageController? _languageController;

        private Grid? _grid;
        private ActivityIndicator? _loadingIndicator;

        public ThemeGrid()
        {
            Task.Factory.StartNew(() =>
            {
                _melodiaController = ServiceHelper.GetService<MelodiaController>();
                _themeController = ServiceHelper.GetService<ThemeController>();
                _languageController = ServiceHelper.GetService<LanguageController>();
                InitData();
            });
        }

        private void InitData()
        {
            _grid = new Grid
            {
                Padding = 10,
                ColumnSpacing = 8,
                RowSpacing = 8,
            };
            _loadingIndicator = new ActivityIndicator
            {
                IsVisible = false,
                IsRunning = false,
                Color = Colors.Purple
            };

            int columnCount = 3; // DeviceDisplay.MainDisplayInfo.Width < 600 ? 3 : 4;
            for (int i = 0; i < columnCount; i++)
                _grid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));

            LoadGridItems();

            var layout = new Grid();
            layout.Children.Add(_grid);
            layout.Children.Add(_loadingIndicator);
            Content = layout;
        }

        private void LoadGridItems()
        {
            if (_themeController == null || _grid == null)
                return;

            _grid.Children.Clear();

            if (_themeController.Themes == null || _themeController.Themes.Count == 0)
            {
                Task.Factory.StartNew(async () =>
                {
                    _themeController.Themes = await _themeController.LoadDemoThemes();
                    await Task.Factory.StartNew(() => InitThemesUI());
                });
            }
            else Task.Factory.StartNew(() => InitThemesUI());
        }

        private void InitThemesUI()
        {
            if (_themeController == null || _grid == null)
                return;

            int row = 0, col = 0;
            int countToUse = _themeController.Themes.Count;

            for (int i = 0; i < countToUse; i++)
            {
                var theme = _themeController.Themes[i];
                var item = CreateThemeGridItem(theme, i);

                Grid.SetRow(item, row);
                Grid.SetColumn(item, col);
                _grid.Children.Add(item);

                col++;
                if (col >= _grid.ColumnDefinitions.Count)
                {
                    col = 0;
                    row++;
                    _grid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
                }
            }
        }


        private View CreateThemeGridItem(ThemeModel theme, int index)
        {
            //var imagePath = ThemeController.GetLocalImagePath(theme);
            var image = new Image
            {
                Source = //File.Exists(imagePath) ? ImageSource.FromFile(imagePath) :
                    ServiceHelper.FixMalformedUrl(theme.ImageUrl),
                Aspect = Aspect.AspectFill,
                HeightRequest = 150
            };

            var tapGesture = new TapGestureRecognizer();
            tapGesture.Tapped += (s, e) => OnThemeTapped(theme, index);

            var frame = new Border
            {
                Padding = 5,
                Stroke = Colors.Transparent,
                StrokeShape = new RoundRectangle { CornerRadius = 12 },
                Content = new StackLayout
                {
                    Children =
                    {
                        new Grid
                        {
                            Children = { image }
                        },
                        new Label
                        {
                            Text = theme.Name,
                            FontSize = 12,
                            TextColor = theme.IsPremium && !AppData.EntitlementIsActive ? Colors.Gray : Colors.White
                        },
                        new BoxView
                        {
                            HeightRequest = 2,
                            WidthRequest = 50,
                            Color = Color.FromArgb("#ac7cea"),
                            HorizontalOptions = LayoutOptions.Start
                        }
                    }
                },
                GestureRecognizers = { tapGesture }
            };

            return frame;
        }

        private async void OnThemeTapped(ThemeModel theme, int index)
        {
            if (theme.IsPremium && !AppData.EntitlementIsActive)
            {
                NavigationService.PushPage(new PaywallPage());
                return;
            }

            if (_melodiaController == null || _languageController == null)
                return;

            if (theme == ThemeController.DefaultThemeModel &&
                _melodiaController.SelectedTreatment == TreatmentController.DefaultTreatmentModel &&
                _melodiaController.SelectedAmbiance == AmbianceController.DefaultAmbianceModel)
            {
                await NavigationService.DisplayAlert(
                    _languageController.GetLabel("Aucun son sélectionné"),
                    _languageController.GetLabel("Sélectionnez un traitement, une ambiance ou un thème."),
                    "OK");
                return;
            }

            if (_melodiaController.SelectedTreatment != TreatmentController.DefaultTreatmentModel &&
                _melodiaController.SelectedListeningMode == ListenTypeController.DefaultListenTypeModel)
            {
                await NavigationService.DisplayAlert(
                    _languageController.GetLabel("Mode d'écoute non sélectionné"),
                    _languageController.GetLabel("Sélectionnez un mode d'écoute à la page 3."),
                    "OK");
                return;
            }

            await Task.Factory.StartNew(() =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    _melodiaController.SelectedTheme = theme;
                    Preferences.Set("themeId", index);
                    _melodiaController.SetSoundsValue(null, 100);
                });
            });

            _melodiaController.GotoPlayerPage();
        }
    }
}