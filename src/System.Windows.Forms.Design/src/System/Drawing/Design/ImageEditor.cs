// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.ComponentModel;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Design;

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
            ArgumentNullException.ThrowIfNull(e);

            string[] extenders = e.GetExtensions();
            string description = e.GetFileDialogDescription();
            string extensions = CreateExtensionsString(extenders, ",");
            string extensionsWithSemicolons = CreateExtensionsString(extenders, ";");
            return $"{description}({extensions})|{extensionsWithSemicolons}";
        }

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (!provider.TryGetService(out IWindowsFormsEditorService editorService))
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

                    ImageEditor e = (ImageEditor)Activator.CreateInstance(
                        extender,
                        BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.CreateInstance,
                        null,
                        null,
                        null);

                    Type myClass = GetType();
                    if (!myClass.Equals(e.GetType()) && e != null && myClass.IsInstanceOfType(e))
                    {
                        filter += $"|{CreateFilterEntry(e)}";
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

        /// <inheritdoc />
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) => UITypeEditorEditStyle.Modal;

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

                ImageEditor e = (ImageEditor)Activator.CreateInstance(
                    extender,
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.CreateInstance,
                    null,
                    null,
                    null);

                if (e.GetType() != typeof(ImageEditor))
                {
                    string[] extensions = e.GetExtensions();
                    if (extensions is not null)
                    {
                        list.AddRange(extensions);
                    }
                }
            }

            return (string[])list.ToArray(typeof(string));
        }

        /// <inheritdoc />
        public override bool GetPaintValueSupported(ITypeDescriptorContext context) => true;

        protected virtual Image LoadFromStream(Stream stream)
        {
            ArgumentNullException.ThrowIfNull(stream);

            // Copy the original stream to a new memory stream to avoid locking the file.
            // The created image will take over ownership of the stream.
            var memoryStream = new MemoryStream();
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
}
