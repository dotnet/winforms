// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Windows.Forms.Design.Editors.Resources;

namespace System.Drawing.Design
{
    /// <summary>
    /// Provides an editor for visually picking an image.
    /// </summary>
    [CLSCompliant(false)]
    public class ImageEditor : UITypeEditor
    {
        internal static Type[] imageExtenders = new Type[] { typeof(BitmapEditor), /*gpr typeof(Icon),*/ typeof(MetafileEditor) };
        internal FileDialog fileDialog;

        // Accessor needed into the static field so that derived classes
        // can implement a different list of supported image types.
        [SuppressMessage("Microsoft.Security", "CA2123:OverrideLinkDemandsShouldBeIdenticalToBase")] // everything in this assembly is full trust.
        protected virtual Type[] GetImageExtenders()
        {
            return imageExtenders;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1818:DoNotConcatenateStringsInsideLoops")]
        protected static string CreateExtensionsString(string[] extensions, string sep)
        {
            if (extensions == null || extensions.Length == 0)
                return null;
            string text = null;
            for (int i = 0; i < extensions.Length - 1; i++)
                text = text + "*." + extensions[i] + sep;
            text = text + "*." + extensions[extensions.Length - 1];
            return text;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")] // previously shipped this way. Would be a breaking change.
        protected static string CreateFilterEntry(ImageEditor e)
        {
            string desc = e.GetFileDialogDescription();
            string exts = CreateExtensionsString(e.GetExtensions(), ",");
            string extsSemis = CreateExtensionsString(e.GetExtensions(), ";");
            return desc + "(" + exts + ")|" + extsSemis;
        }

        /// <summary>
        ///      Edits the given object value using the editor style provided by
        ///      GetEditorStyle.  A service provider is provided so that any
        ///      required editing services can be obtained.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1818:DoNotConcatenateStringsInsideLoops")]
        [SuppressMessage("Microsoft.Security", "CA2123:OverrideLinkDemandsShouldBeIdenticalToBase")] // everything in this assembly is full trust.
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (provider != null)
            {
                IWindowsFormsEditorService edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));

                if (edSvc != null)
                {
                    if (fileDialog == null)
                    {
                        fileDialog = new OpenFileDialog();
                        string filter = CreateFilterEntry(this);
                        for (int i = 0; i < GetImageExtenders().Length; i++)
                        {
                            ImageEditor e = (ImageEditor)Activator.CreateInstance(GetImageExtenders()[i], BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.CreateInstance, null, null, null); //.CreateInstance();
                            Type myClass = this.GetType();
                            Type editorClass = e.GetType();
                            if (!myClass.Equals(editorClass) && e != null && myClass.IsInstanceOfType(e))
                                filter += "|" + CreateFilterEntry(e);
                        }
                        fileDialog.Filter = filter;
                    }

                    IntPtr hwndFocus = UnsafeNativeMethods.GetFocus();
                    try
                    {
                        if (fileDialog.ShowDialog() == DialogResult.OK)
                        {
                            FileStream file = new FileStream(fileDialog.FileName, FileMode.Open, FileAccess.Read, FileShare.Read);
                            value = LoadFromStream(file);
                        }
                    }
                    finally
                    {
                        if (hwndFocus != IntPtr.Zero)
                        {
                            UnsafeNativeMethods.SetFocus(new HandleRef(null, hwndFocus));
                        }
                    }
                }
            }
            return value;
        }

        /// <summary>
        ///      Retrieves the editing style of the Edit method.  If the method
        ///      is not supported, this will return None.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2123:OverrideLinkDemandsShouldBeIdenticalToBase")] // everything in this assembly is full trust.
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }

        protected virtual string GetFileDialogDescription()
        {
            return SR.imageFileDescription;
        }

        [SuppressMessage("Microsoft.Security", "CA2123:OverrideLinkDemandsShouldBeIdenticalToBase")] // everything in this assembly is full trust.
        protected virtual string[] GetExtensions()
        {
            // We should probably smash them together...
            ArrayList list = new ArrayList();
            for (int i = 0; i < GetImageExtenders().Length; i++)
            {
                ImageEditor e = (ImageEditor)Activator.CreateInstance(GetImageExtenders()[i], BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.CreateInstance, null, null, null);   //.CreateInstance();
                if (e.GetType() != typeof(ImageEditor))
                    list.AddRange(new ArrayList(e.GetExtensions()));
            }
            return (string[])list.ToArray(typeof(string));
        }

        /// <summary>
        ///      Determines if this editor supports the painting of a representation
        ///      of an object's value.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2123:OverrideLinkDemandsShouldBeIdenticalToBase")] // everything in this assembly is full trust.
        public override bool GetPaintValueSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        [SuppressMessage("Microsoft.Security", "CA2123:OverrideLinkDemandsShouldBeIdenticalToBase")] // everything in this assembly is full trust.
        protected virtual Image LoadFromStream(Stream stream)
        {
            //Copy the original stream to a buffer, then wrap a
            //memory stream around it.  This way we can avoid
            //locking the file
            byte[] buffer = new byte[stream.Length];
            stream.Read(buffer, 0, (int)stream.Length);
            MemoryStream ms = new MemoryStream(buffer);

            return Image.FromStream(ms);
        }

        /// <summary>
        ///       Paints a representative value of the given object to the provided
        ///       canvas. Painting should be done within the boundaries of the
        ///       provided rectangle.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2109:ReviewVisibleEventHandlers")] //Benign code
        [SuppressMessage("Microsoft.Security", "CA2123:OverrideLinkDemandsShouldBeIdenticalToBase")] // everything in this assembly is full trust.
        public override void PaintValue(PaintValueEventArgs e)
        {
            Image image = e.Value as Image;
            if (image != null)
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

