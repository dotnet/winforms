// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace System.Drawing.Design;

/// <summary>
///  Provides an editor for visually picking an image.
/// </summary>
[CLSCompliant(false)]
public class ImageEditor : UITypeEditor
{
    private static readonly Type[] s_imageExtenders = [typeof(BitmapEditor), typeof(MetafileEditor)];
    private FileDialog? _fileDialog;

    // Accessor needed into the static field so that derived classes
    // can implement a different list of supported image types.
    protected virtual Type[] GetImageExtenders() => s_imageExtenders;

    protected static string? CreateExtensionsString(string?[]? extensions, string sep)
    {
        if (extensions is null || extensions.Length == 0)
        {
            return null;
        }

        StringBuilder text = new();
        for (int i = 0; i < extensions.Length; i++)
        {
            // Skip empty extensions.
            if (string.IsNullOrEmpty(extensions[i]))
            {
                continue;
            }

            text.Append("*.");
            text.Append(extensions[i]);
            if (i != extensions.Length - 1)
            {
                text.Append(sep);
            }
        }

        return text.ToString();
    }

    protected static string CreateFilterEntry(ImageEditor e)
    {
        ArgumentNullException.ThrowIfNull(e);

        string[] extenders = e.GetExtensions();
        string description = e.GetFileDialogDescription();
        string? extensions = CreateExtensionsString(extenders, ",");
        string? extensionsWithSemicolons = CreateExtensionsString(extenders, ";");
        return $"{description}({extensions})|{extensionsWithSemicolons}";
    }

    public override object? EditValue(ITypeDescriptorContext? context, IServiceProvider provider, object? value)
    {
        if (!provider.TryGetService(out IWindowsFormsEditorService? _))
        {
            return value;
        }

        if (_fileDialog is null)
        {
            _fileDialog = new OpenFileDialog();
            string filter = CreateFilterEntry(this);
            foreach (Type extender in GetImageExtenders())
            {
                // Skip invalid extenders.
                if (extender is null || !typeof(ImageEditor).IsAssignableFrom(extender))
                {
                    continue;
                }

                Type myClass = GetType();
                ImageEditor? editor = (ImageEditor?)Activator.CreateInstance(
                    extender,
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.CreateInstance,
                    binder: null,
                    args: null,
                    culture: null);

                if (editor is not null
                    && editor.GetType() is Type editorClass
                    && !myClass.Equals(editorClass)
                    && myClass.IsInstanceOfType(editor))
                {
                    filter += $"|{CreateFilterEntry(editor)}";
                }
            }

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

    public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext? context) => UITypeEditorEditStyle.Modal;

    protected virtual string GetFileDialogDescription() => SR.imageFileDescription;

    protected virtual string[] GetExtensions()
    {
        List<string> list = [];
        foreach (Type extender in GetImageExtenders())
        {
            // Skip invalid extenders.
            if (extender is null || !typeof(ImageEditor).IsAssignableFrom(extender))
            {
                continue;
            }

            ImageEditor? editor = (ImageEditor?)Activator.CreateInstance(
                extender,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.CreateInstance,
                binder: null,
                args: null,
                culture: null);

            if (editor is not null && editor.GetType() != typeof(ImageEditor))
            {
                string[] extensions = editor.GetExtensions();
                if (extensions is not null)
                {
                    list.AddRange(extensions);
                }
            }
        }

        return [.. list];
    }

    /// <inheritdoc />
    public override bool GetPaintValueSupported(ITypeDescriptorContext? context) => true;

    protected virtual Image LoadFromStream(Stream stream)
    {
        ArgumentNullException.ThrowIfNull(stream);

        // Copy the original stream to a new memory stream to avoid locking the file.
        // The created image will take over ownership of the stream.
        MemoryStream memoryStream = new();
        stream.CopyTo(memoryStream);
        return Image.FromStream(memoryStream);
    }

    /// <inheritdoc />
    public override void PaintValue(PaintValueEventArgs e)
    {
        if (e?.Value is Image image)
        {
            Rectangle r = e.Bounds;
            r.Width--;
            r.Height--;
            e.Graphics.DrawRectangle(SystemPens.WindowFrame, r);
            e.Graphics.DrawImage(image, e.Bounds);
        }
    }
}
