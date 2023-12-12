using Avalonia;
using Avalonia.Controls;
using BarryFileMan.Enums.Help;
using BarryFileMan.Managers;
using static System.Collections.Specialized.BitVector32;

namespace BarryFileMan.Views.Help;

public partial class HelpButton : Button
{
    public static readonly StyledProperty<double?> IconHeightProperty =
        AvaloniaProperty.Register<HelpButton, double?>(nameof(IconHeight));

    public static readonly StyledProperty<double?> IconWidthProperty =
        AvaloniaProperty.Register<HelpButton, double?>(nameof(IconWidth));

    public static readonly StyledProperty<HelpSections> SectionProperty =
        AvaloniaProperty.Register<HelpButton, HelpSections>(nameof(Section), HelpSections.Help);

    public double? IconHeight
    {
        get => GetValue(IconHeightProperty);
        set => SetValue(IconHeightProperty, value);
    }

    public double? IconWidth
    {
        get => GetValue(IconWidthProperty);
        set => SetValue(IconWidthProperty, value);
    }

    public HelpSections Section
    {
        get => GetValue(SectionProperty);
        set => SetValue(SectionProperty, value);
    }

    public HelpButton()
    {
        InitializeComponent();
    }

    private void HelpButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if(Command == null)
        {
            AppManager.HelpWindowShowAsync(Section);
        }
    }
}