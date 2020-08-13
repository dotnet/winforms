// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using static Interop;

namespace System.Drawing.Design
{
    /// <summary>
    ///  Provides an editor for visually picking an image.
    /// </summary>
    [CLSCompliant(false)]
    public class ImageEditor : UITypeEditor
    {
        private static readonly Type[] s_imageExtenders = new Type[] { typeof(BitmapEditor), typeof(MetafileEditor) };
        private FileDialog _fileDialog;

        // Accessor needed into the static field so that derived classes
        // can implement a different list of supported image types.
        protected virtual Type[] GetImageExtenders() => s_imageExtenders;

        protected static string CreateExtensionsString(string[] extensions, string sep)
        {
            if (extensions is null || extensions.Length == 0)
            {
                return null;
            }

            var text = new StringBuilder();
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
            if (e is null)
            {
                throw new ArgumentNullException(nameof(e));
            }

            string[] extenders = e.GetExtensions();
            string desc = e.GetFileDialogDescription();
            string exts = CreateExtensionsString(extenders, ",");
            string extsSemis = CreateExtensionsString(extenders, ";");
            return desc + "(" + exts + ")|" + extsSemis;
        }

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (provider != null)
            {
                if (provider.GetService(typeof(IWindowsFormsEditorService)) is IWindowsFormsEditorService edSvc)
                {
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

                            ImageEditor e = (ImageEditor)Activator.CreateInstance(extender, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.CreateInstance, null, null, null);
                            Type myClass = GetType();
                            if (!myClass.Equals(e.GetType()) && e != null && myClass.IsInstanceOfType(e))
                            {
                                filter += "|" + CreateFilterEntry(e);
                            }
                        }

                        _fileDialog.Filter = filter;
                    }

                    IntPtr hwndFocus = User32.GetFocus();
                    try
                    {
                        if (_fileDialog.ShowDialog() == DialogResult.OK)
                        {
                            using (var file = new FileStream(_fileDialog.FileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                            {
                                value = LoadFromStream(file);
                            }
                        }
                    }
                    finally
                    {
                        if (hwndFocus != IntPtr.Zero)
                        {
                            User32.SetFocus(hwndFocus);
                        }
                    }
                }
            }

            return value;
        }

        /// <summary>
        ///  Retrieves the editing style of the Edit method. If the method is not supported,
        ///  this will return None.
        /// </summary>
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }

        protected virtual string GetFileDialogDescription() => SR.imageFileDescription;

        protected virtual string[] GetExtensions()
        {
            ArrayList list = new ArrayList();
            foreach (Type extender in GetImageExtenders())
            {
                // Skip invalid extenders.
                if (extender is null || !typeof(ImageEditor).IsAssignableFrom(extender))
                {
                    continue;
                }

                ImageEditor e = (ImageEditor)Activator.CreateInstance(extender, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.CreateInstance, null, null, null);
                if (e.GetType() != typeof(ImageEditor))
                {
                    string[] extensions = e.GetExtensions();
                    if (extensions != null)
                    {
                        list.AddRange(extensions);
                    }
                }
            }

            return (string[])list.ToArray(typeof(string));
        }

        /// <summary>
        ///  Determines if this editor supports the painting of a representation
        ///  of an object's value.
        /// </summary>
        public override bool GetPaintValueSupported(ITypeDescriptorContext context) => true;

        protected virtual Image LoadFromStream(Stream stream)
        {
            if (stream is null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            // Copy the original stream to a new memory stream to avoid locking the file.
            // The created image will take over ownership of the stream.
            var memoryStream = new MemoryStream();
            stream.CopyTo(memoryStream);
            return Image.FromStream(memoryStream);
        }

        /// <summary>
        ///  Paints a representative value of the given object to the provided
        ///  canvas. Painting should be done within the boundaries of the
        ///  provided rectangle.
        /// </summary>
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
}
