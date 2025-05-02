using Microsoft.Maui.Controls;
using System.Windows.Input;

namespace MelodiaTherapy.Views
{
    public partial class SocialCircleView : ContentView
    {
        public static readonly BindableProperty IconGlyphProperty =
            BindableProperty.Create(nameof(IconGlyph), typeof(string), typeof(SettingsTile), string.Empty);

        public static readonly BindableProperty CommandProperty =
            BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(SettingsTile), null);

        public static readonly BindableProperty CommandParameterProperty =
            BindableProperty.Create(nameof(CommandParameter), typeof(string), typeof(SettingsTile), string.Empty);

        public string IconGlyph
        {
            get => (string)GetValue(IconGlyphProperty);
            set => SetValue(IconGlyphProperty, value);
        }

        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }
        
        public string CommandParameter
        {
            get => (string)GetValue(CommandParameterProperty);
            set => SetValue(CommandParameterProperty, value);
        } 


        public SocialCircleView()
        {
            InitializeComponent();
        }
    }
}