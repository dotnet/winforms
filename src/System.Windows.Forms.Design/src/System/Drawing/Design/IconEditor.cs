// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace System.Drawing.Design;

// NOTE: this class should be almost identical to ImageEditor. The main exception is PaintValue, which has logic
// that should probably be copied into ImageEditor.

/// <summary>
/// Provides an editor for visually picking an icon.
/// </summary>
public class IconEditor : UITypeEditor
{
    private static readonly List<string> s_iconExtensions = ["ico"];
    private static readonly Type[] s_imageExtenders = [];
    private FileDialog? _fileDialog;

    protected static string? CreateExtensionsString(string?[]? extensions, string sep)
    {
        if (extensions is null || extensions.Length == 0)
        {
            return null;
        }

        string text = string.Empty;
        for (int i = 0; i < extensions.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(extensions[i]))
            {
                continue;
            }

            text = $"{text}*.{extensions[i]}";
            if (i < extensions.Length - 1)
            {
                text += sep;
            }
        }

        return text;
    }

    protected static string CreateFilterEntry(IconEditor editor)
    {
        string description = editor.GetFileDialogDescription();
        string? extensions = CreateExtensionsString(editor.GetExtensions(), ",");
        string? extensionsWithSemicolons = CreateExtensionsString(editor.GetExtensions(), ";");
        return $"{description}({extensions})|{extensionsWithSemicolons}";
    }

    public override object? EditValue(ITypeDescriptorContext? context, IServiceProvider provider, object? value)
    {
        // Even though we don't use the editor service this is historically what we did.
        if (!provider.TryGetService(out IWindowsFormsEditorService? _))
        {
            return value;
        }

        if (_fileDialog is null)
        {
            _fileDialog = new OpenFileDialog();
            string filter = CreateFilterEntry(this);

            Debug.Assert(s_imageExtenders.Length <= 0, "Why does IconEditor have subclasses if Icon doesn't?");

            _fileDialog.Filter = filter;
        }

        HWND hwndFocus = PInvoke.GetFocus();
        try
        {
            if (_fileDialog.ShowDialog() == DialogResult.OK)
            {
                using FileStream stream = new(_fileDialog.FileName, FileMode.Open, FileAccess.Read, FileShare.Read);
                value = LoadFromStream(stream);
            }
        }
        finally
        {
            if (!hwndFocus.IsNull)
            {
                PInvoke.SetFocus(hwndFocus);
            }
        }

        return value;
    }

    /// <summary>
    /// Retrieves the editing style of the Edit method. If the method
    /// is not supported, this will return None.
    /// </summary>
    public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext? context)
        => UITypeEditorEditStyle.Modal;

    protected virtual string GetFileDialogDescription() => SR.iconFileDescription;

    protected virtual string[] GetExtensions() => [.. s_iconExtensions];

    /// <inheritdoc />
    public override bool GetPaintValueSupported(ITypeDescriptorContext? context) => true;

    protected virtual Icon LoadFromStream(Stream stream) => new(stream);

    /// <inheritdoc />
    public override void PaintValue(PaintValueEventArgs e)
    {
        if (e?.Value is not Icon icon)
        {
            return;
        }

        // If icon is smaller than rectangle, just center it unscaled in the rectangle.
        Rectangle rectangle = e.Bounds;

        if (icon.Width < rectangle.Width)
        {
            rectangle.X += (rectangle.Width - icon.Width) / 2;
            rectangle.Width = icon.Width;
        }

        if (icon.Height < rectangle.Height)
        {
            rectangle.Y += (rectangle.Height - icon.Height) / 2;
            rectangle.Height = icon.Height;
        }

        e.Graphics.DrawIcon(icon, rectangle);
    }
}
