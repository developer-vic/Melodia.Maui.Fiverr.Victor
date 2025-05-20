using MelodiaTherapy.Controllers;
using MelodiaTherapy.Helpers;
using MelodiaTherapy.Models;
using MelodiaTherapy.Pages;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Shapes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace MelodiaTherapy.Views
{
    public class ThemeGrid : ContentView
    {
        private readonly MelodiaController? _melodiaController;
        private readonly ThemeController? _themeController;
        private readonly LanguageController? _languageController;

        private Grid _grid;
        private ActivityIndicator _loadingIndicator;
        private bool _isLoading = false;

        public ThemeGrid()
        {
            _melodiaController = ServiceHelper.GetService<MelodiaController>();
            _themeController = ServiceHelper.GetService<ThemeController>();
            _languageController = ServiceHelper.GetService<LanguageController>();

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

        private async void LoadGridItems()
        {
            _grid.Children.Clear();
            int row = 0, col = 0;

            if (_themeController == null)
                return;

            if (_themeController.Themes == null || _themeController.Themes.Count == 0)
                _themeController.Themes = await _themeController.LoadDemoThemes();

            int countToUse = _themeController.Themes.Count;
            //if (countToUse > 1) countToUse = 10;

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

        string FixMalformedUrl(string? urlSent)
        {
            if (string.IsNullOrWhiteSpace(urlSent)) return "image_not_supported.png";

            urlSent = urlSent.ToLower(); // Fix common mistake: duplicated 'therapy'
            string correctUrl = urlSent.Replace("melodiatherapytherapy.com", "melodiatherapy.com");

            // var uri = new Uri(correctUrl); var encodedPath = string.Join("/", uri.AbsolutePath
            //     .Split('/', StringSplitOptions.RemoveEmptyEntries)
            //     .Select(Uri.EscapeDataString));
            // correctUrl = $"{uri.Scheme}://{uri.Host}/{encodedPath}";

            correctUrl = Uri.EscapeUriString(correctUrl);
            return correctUrl;
        }


        private View CreateThemeGridItem(ThemeModel theme, int index)
        {
            var imagePath = ThemeController.GetLocalImagePath(theme);
            var image = new Image
            {
                Source = //File.Exists(imagePath) ? ImageSource.FromFile(imagePath) :
                    FixMalformedUrl(theme.ImageUrl),
                Aspect = Aspect.AspectFill,
                HeightRequest = 150
            };

            var tapGesture = new TapGestureRecognizer();
            tapGesture.Tapped += async (s, e) => await OnThemeTapped(theme, index);

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

        private async Task OnThemeTapped(ThemeModel theme, int index)
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

            _melodiaController.SelectedTheme = theme;
            Preferences.Set("themeId", index);

            ShowLoading(true);
            await Task.Delay(1000);
            ShowLoading(false);
            _melodiaController.SetSoundsValue(null, 100);
            _melodiaController.GotoPlayerPage();
        }

        private void ShowLoading(bool show)
        {
            _loadingIndicator.IsVisible = show;
            _loadingIndicator.IsRunning = show;
            _isLoading = show;
        }
    }
}