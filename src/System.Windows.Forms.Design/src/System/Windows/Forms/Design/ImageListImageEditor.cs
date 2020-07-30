// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.ComponentModel;
using System.Drawing.Design;
using System.IO;
using System.Reflection;
using static Interop;

namespace System.Windows.Forms.Design
{
    /// <summary>
    ///  Provides an editor that can perform default file searching for bitmap (.bmp) files.
    /// </summary>
    [CLSCompliant(false)]
    public class ImageListImageEditor : ImageEditor
    {
        // VSWhidbey 95227: Metafile types are not supported in the ImageListImageEditor and should
        // not be displayed as an option.
        internal static Type[] s_imageExtenders = new Type[] { typeof(BitmapEditor)/*, gpr typeof(Icon), typeof(MetafileEditor)*/};
        private OpenFileDialog _fileDialog;

        // VSWhidbey 95227: accessor needed into the static field so that derived classes
        // can implement a different list of supported image types.
        protected override Type[] GetImageExtenders() => s_imageExtenders;

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (provider is null)
            {
                return value;
            }

            var images = new ArrayList();
            var edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
            if (edSvc is null)
            {
                return images;
            }

            if (_fileDialog is null)
            {
                _fileDialog = new OpenFileDialog();
                _fileDialog.Multiselect = true;
                string filter = CreateFilterEntry(this);
                Type[] imageExtenders = GetImageExtenders();
                for (int i = 0; i < imageExtenders.Length; i++)
                {
                    var myClass = this.GetType();
                    var editor = (ImageEditor)Activator.CreateInstance(imageExtenders[i],
                                                                       BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.CreateInstance,
                                                                       null, null, null);
                    var editorClass = editor.GetType();

                    if (!myClass.Equals(editorClass) && editor != null && myClass.IsInstanceOfType(editor))
                    {
                        filter += "|" + CreateFilterEntry(editor);
                    }
                }
                _fileDialog.Filter = filter;
            }

            IntPtr hwndFocus = User32.GetFocus();
            try
            {
                if (_fileDialog.ShowDialog() == DialogResult.OK)
                {
                    foreach (string name in _fileDialog.FileNames)
                    {
                        ImageListImage image;
                        using (FileStream file = new FileStream(name, FileMode.Open, FileAccess.Read, FileShare.Read))
                        {
                            image = LoadImageFromStream(file, name.EndsWith(".ico"));
                            image.Name = Path.GetFileName(name);
                            images.Add(image);
                        }
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

            return images;
        }

        protected override string GetFileDialogDescription() => SR.imageFileDescription;

        /// <summary>
        ///  Determines if this editor supports the painting of a representation of an object's value.
        /// </summary>
        public override bool GetPaintValueSupported(ITypeDescriptorContext context) => true;

        private ImageListImage LoadImageFromStream(Stream stream, bool imageIsIcon)
        {
            // Copy the original stream to a buffer, then wrap a
            // memory stream around it.  This way we can avoid locking the file
            byte[] buffer = new byte[stream.Length];
            stream.Read(buffer, 0, (int)stream.Length);

            // The created image will take over ownership of the stream.
            MemoryStream ms = new MemoryStream(buffer);
            return ImageListImage.ImageListImageFromStream(ms, imageIsIcon);
        }

        /// <summary>
        ///  Paints a representative value of the given object to the provided canvas.
        ///  Painting should be done within the boundaries of the provided rectangle.
        /// </summary>
        public override void PaintValue(PaintValueEventArgs e)
        {
            if (e.Value is ImageListImage)
            {
                e = new PaintValueEventArgs(e.Context, ((ImageListImage)e.Value).Image, e.Graphics, e.Bounds);
            }

            base.PaintValue(e);
        }
    }
}

