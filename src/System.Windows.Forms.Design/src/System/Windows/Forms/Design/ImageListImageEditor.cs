// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.ComponentModel;
using System.Drawing.Design;
using System.Reflection;

namespace System.Windows.Forms.Design;

/// <summary>
///  Provides an editor that can perform default file searching for bitmap (.bmp) files.
/// </summary>
[CLSCompliant(false)]
public class ImageListImageEditor : ImageEditor
{
    // Metafile types are not supported in the ImageListImageEditor and should not be displayed as an option.
    internal static Type[] s_imageExtenders = [typeof(BitmapEditor)];
    private OpenFileDialog? _fileDialog;

    // Derived classes can implement a different list of supported image types.
    protected override Type[] GetImageExtenders() => s_imageExtenders;

    public override object? EditValue(ITypeDescriptorContext? context, IServiceProvider provider, object? value)
    {
        if (provider is null)
        {
            return value;
        }

        ArrayList images = [];
        if (!provider.TryGetService(out IWindowsFormsEditorService? _))
        {
            return images;
        }

        if (_fileDialog is null)
        {
            _fileDialog = new OpenFileDialog
            {
                Multiselect = true
            };

            string filter = CreateFilterEntry(this);
            foreach (Type extender in GetImageExtenders())
            {
                Type myClass = GetType();
                var editor = (ImageEditor?)Activator.CreateInstance(
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
                foreach (string name in _fileDialog.FileNames)
                {
                    using FileStream file = new(name, FileMode.Open, FileAccess.Read, FileShare.Read);
                    ImageListImage image = LoadImageFromStream(file, name.EndsWith(".ico", StringComparison.Ordinal));
                    image.Name = Path.GetFileName(name);
                    images.Add(image);
                }
            }
        }
        finally
        {
            if (!hwndFocus.IsNull)
            {
                PInvoke.SetFocus(hwndFocus);
            }
        }

        return images;
    }

    protected override string GetFileDialogDescription() => SR.imageFileDescription;

    /// <summary>
    ///  Determines if this editor supports the painting of a representation of an object's value.
    /// </summary>
    public override bool GetPaintValueSupported(ITypeDescriptorContext? context) => true;

    private static ImageListImage LoadImageFromStream(Stream stream, bool imageIsIcon)
    {
        // Copy the original stream to a buffer, then wrap a memory stream around it to avoid locking the file.
        byte[] buffer = new byte[stream.Length];
        stream.ReadExactly(buffer, 0, (int)stream.Length);

        // The created image will take over ownership of the stream.
        MemoryStream ms = new(buffer);
        return ImageListImage.ImageListImageFromStream(ms, imageIsIcon);
    }

    /// <summary>
    ///  Paints a representative value of the given object to the provided canvas.
    ///  Painting should be done within the boundaries of the provided rectangle.
    /// </summary>
    public override void PaintValue(PaintValueEventArgs e)
    {
        if (e.Value is ImageListImage image)
        {
            e = new PaintValueEventArgs(e.Context, image.Image, e.Graphics, e.Bounds);
        }

        base.PaintValue(e);
    }
}
