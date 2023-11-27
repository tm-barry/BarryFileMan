using Avalonia.Controls;
using System;
using System.Threading.Tasks;

namespace BarryFileMan.Views.Common;

public partial class MessageBox : Window
{
    MsgBoxResult _msgBoxResult = MsgBoxResult.Ok;

    public MessageBox()
    {
        InitializeComponent();
    }

    public static Task<MsgBoxResult> ShowAsync(Window? parent, string title, string message, 
        MsgBoxButtons buttons, MsgBoxIcons? icon = null, WindowStartupLocation windowStartupLocation = WindowStartupLocation.CenterScreen)
    {
        var msgbox = new MessageBox()
        {
            Title = title,
            WindowStartupLocation = windowStartupLocation,
        };

        msgbox.MsgText.Text = message;
        
        var materialIconKind = GetMaterialIconKind(icon);
        msgbox.MsgIcon.Kind = materialIconKind.GetValueOrDefault();
        msgbox.MsgIcon.IsVisible = materialIconKind.HasValue;

        if (buttons == MsgBoxButtons.Ok || buttons == MsgBoxButtons.OkCancel)
            msgbox.AddButton("Ok", MsgBoxResult.Ok, true);

        if (buttons == MsgBoxButtons.YesNo || buttons == MsgBoxButtons.YesNoCancel)
        {
            msgbox.AddButton("Yes", MsgBoxResult.Yes);
            msgbox.AddButton("No", MsgBoxResult.No, true);
        }

        if (buttons == MsgBoxButtons.OkCancel || buttons == MsgBoxButtons.YesNoCancel)
            msgbox.AddButton("Cancel", MsgBoxResult.Cancel, true);

        var tcs = new TaskCompletionSource<MsgBoxResult>();
        msgbox.Closed += delegate { tcs.TrySetResult(msgbox._msgBoxResult); };

        if (parent != null)
            msgbox.ShowDialog(parent);
        else 
            msgbox.Show();

        return tcs.Task;
    }

    private static Material.Icons.MaterialIconKind? GetMaterialIconKind(MsgBoxIcons? icon)
    {
        switch(icon)
        {
            case MsgBoxIcons.Alert:
                return Material.Icons.MaterialIconKind.AlertCircle;
            case MsgBoxIcons.Error:
                return Material.Icons.MaterialIconKind.CloseCircle;
            case MsgBoxIcons.Info:
                return Material.Icons.MaterialIconKind.InfoCircle;
            case MsgBoxIcons.Question:
                return Material.Icons.MaterialIconKind.QuestionMarkCircle;
            case MsgBoxIcons.Success:
                return Material.Icons.MaterialIconKind.SuccessCircle;
            default:
                return null;
        }
    }

    private void AddButton(string content, MsgBoxResult result, bool defaultValue = false)
    {
        var btn = new Button
        {
            Content = new TextBlock 
            { 
                Text = content,
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center
            }
        };

        btn.Click += (_sender, _eventArgs) => {
            _msgBoxResult = result;
            Close();
        };

        Buttons.Children.Add(btn);

        if (defaultValue)
            _msgBoxResult = result;
    }
}

public enum MsgBoxButtons
{
    Ok,
    OkCancel,
    YesNo,
    YesNoCancel
}

public enum MsgBoxIcons
{
    Alert,
    Error,
    Info,
    Question,
    Success
}

public enum MsgBoxResult
{
    Ok,
    Cancel,
    Yes,
    No
}