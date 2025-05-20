using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MelodiaTherapy.Helpers;
using MelodiaTherapy.Models;
using Microsoft.Maui.Controls.Shapes;

namespace MelodiaTherapy.Views;

public class GridItem : ContentView
{
    public static readonly BindableProperty ItemProperty = BindableProperty.Create(
        nameof(Item), typeof(GridModel), typeof(GridItem), propertyChanged: OnPropertyChanged);

    public static readonly BindableProperty SelectedItemProperty = BindableProperty.Create(
        nameof(SelectedItem), typeof(GridModel), typeof(GridItem), propertyChanged: OnPropertyChanged);

    public static readonly BindableProperty OnTappedProperty = BindableProperty.Create(
        nameof(OnTapped), typeof(Action), typeof(GridItem));

    public static readonly BindableProperty ShowInfoProperty = BindableProperty.Create(
        nameof(ShowInfo), typeof(bool), typeof(GridItem), false);

    private readonly Border container;
    private readonly Label icon;
    private readonly Label nameLabel;
    private readonly Grid overlay;

    public GridModel Item
    {
        get => (GridModel)GetValue(ItemProperty);
        set => SetValue(ItemProperty, value);
    }

    public GridModel SelectedItem
    {
        get => (GridModel)GetValue(SelectedItemProperty);
        set => SetValue(SelectedItemProperty, value);
    }

    public Action OnTapped
    {
        get => (Action)GetValue(OnTappedProperty);
        set => SetValue(OnTappedProperty, value);
    }

    public bool ShowInfo
    {
        get => (bool)GetValue(ShowInfoProperty);
        set => SetValue(ShowInfoProperty, value);
    }

    public GridItem(GridModel item, GridModel selectedItem, bool show)
    {
        container = new Border
        {
            Stroke = Colors.Transparent,
            StrokeShape = new RoundRectangle { CornerRadius = 12 },
            Padding = new Thickness(8, 15),
            BackgroundColor = Colors.Transparent,
            Content = new VerticalStackLayout
            {
                Spacing = 12,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                Children =
                {
                    (icon = new Label
                    {
                        TextColor=Color.FromArgb("#76cec5"),
                        FontSize = 30,
                        FontFamily = "FontAwesome",
                        HorizontalOptions = LayoutOptions.Center
                    }),
                    (nameLabel = new Label
                    {
                        FontSize = 12,
                        FontAttributes = FontAttributes.Bold,
                        HorizontalTextAlignment = TextAlignment.Center,
                        TextColor = Colors.White,
                        TextTransform= TextTransform.Uppercase
                    })
                }
            }
        };

        overlay = new Grid
        {
            Children =
            {
                container,
                new ImageButton
                {
                    Source = "info.png",
                    BackgroundColor = Colors.Transparent,
                    WidthRequest = 24,
                    HeightRequest = 24,
                    Aspect = Aspect.Center,
                    HorizontalOptions = LayoutOptions.End,
                    VerticalOptions = LayoutOptions.Start,
                    Margin = new Thickness(6),
                    Command = new Command(() =>
                    {
                        if (Item != null && !(Item.IsPremium && !AppData.EntitlementIsActive))
                            ShowInfoDialog();
                    }),
                    IsVisible = false // controlled later
                }
            }
        };

        var tapGesture = new TapGestureRecognizer { Command = new Command(() => OnTapped?.Invoke()) };
        overlay.GestureRecognizers.Add(tapGesture);

        Content = overlay;

        // Set the initial values to trigger UpdateView
        ShowInfo = show;
        SelectedItem = selectedItem;
        Item = item;
    }

    private static void OnPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is GridItem view)
        {
            view.UpdateView();
        }
    }

    private void UpdateView()
    {
        if (Item == null || SelectedItem == null || overlay == null)
            return;

        container.BackgroundColor =
            Item.Name == SelectedItem?.Name ? Colors.Orange : Colors.LightGray.WithAlpha(0.1f);

        icon.Text = char.ConvertFromUtf32(int.TryParse(Item.Icon, out int iconCode) ? iconCode : 0xf5dc);
        //icon.TextColor = Item.IsPremium && !AppData.EntitlementIsActive ? Color.FromArgb("#FFC107") : Colors.Orange;

        nameLabel.Text = Item.Name;
        nameLabel.TextColor =
            Item.Name == SelectedItem?.Name ? Colors.Black : Colors.White;

        var infoIcon = overlay.Children[1] as ImageButton;
        if (infoIcon != null)
        {
            infoIcon.IsVisible = Item.Name != "AUCUN" && ShowInfo;
            infoIcon.WidthRequest = 24; infoIcon.HeightRequest = 24;
            infoIcon.Aspect = Aspect.Center;
            infoIcon.Source = (Item.IsPremium && !AppData.EntitlementIsActive) ? "lock.png" : "info.png";
        }
    }

    private async void ShowInfoDialog()
    {
        await NavigationService.DisplayAlert(Item.Name ?? "Info", Item.Description ?? "No description available", "Close");
    }
}