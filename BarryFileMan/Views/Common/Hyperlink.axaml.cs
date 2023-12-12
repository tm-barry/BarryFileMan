using Avalonia;
using Avalonia.Controls;
using System;
using System.Diagnostics;

namespace BarryFileMan.Views.Common;

public partial class Hyperlink : Button
{
    public static readonly StyledProperty<Uri?> UriProperty =
        AvaloniaProperty.Register<Hyperlink, Uri?>(nameof(Uri));

    public Uri? Uri
    {
        get => GetValue(UriProperty);
        set => SetValue(UriProperty, value);
    }

    public Hyperlink()
    {
        InitializeComponent();
    }

    private void OpenHyperlink(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        // Open hyperlink if no Command binding
        if (Command == null)
        {
            var hyperlink = Uri?.ToString() ?? Content?.ToString() ?? string.Empty;
            Process.Start(new ProcessStartInfo() { FileName = hyperlink, UseShellExecute = true });
        }
    }
}