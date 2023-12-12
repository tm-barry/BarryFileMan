using Avalonia;
using Avalonia.Controls;
using BarryFileMan.Enums.Help;
using BarryFileMan.Managers;

namespace BarryFileMan.Views.Help;

public partial class HelpLink : Button
{
    public static readonly StyledProperty<HelpSections> SectionProperty =
        AvaloniaProperty.Register<HelpLink, HelpSections>(nameof(Section), HelpSections.Help);

    public HelpSections Section
    {
        get => GetValue(SectionProperty);
        set => SetValue(SectionProperty, value);
    }

    public HelpLink()
    {
        InitializeComponent();
        Click += HelpLink_Click;
    }

    private void HelpLink_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (Command == null)
        {
            AppManager.HelpWindowShowAsync(Section);
        }
    }
}