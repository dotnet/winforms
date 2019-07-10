//------------------------------------------------------------------------------
// <copyright file="ImageListImageEditor.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>                                                                
//------------------------------------------------------------------------------

namespace System.Windows.Forms.Design {
    using System.Runtime.InteropServices;

    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System;
    using System.IO;
    using System.Collections;
    using System.ComponentModel;
    using System.Windows.Forms;
    using System.Drawing;
    using System.Reflection;
    using System.Drawing.Design;
    using System.Windows.Forms.ComponentModel;


    /// <include file='doc\ImageList.uex' path='docs/doc[@for="ImageListImageEditor"]/*' />
    /// <devdoc>
    ///    <para>Provides an editor that can perform default file searching for bitmap (.bmp)
    ///       files.</para>
    /// </devdoc>
    public class ImageListImageEditor : ImageEditor {

        // VSWhidbey 95227: Metafile types are not supported in the ImageListImageEditor and should
        // not be displayed as an option.
        internal static Type[] imageExtenders = new Type[] { typeof(BitmapEditor)/*, gpr typeof(Icon), typeof(MetafileEditor)*/};
        private OpenFileDialog fileDialog = null;

        // VSWhidbey 95227: accessor needed into the static field so that derived classes
        // can implement a different list of supported image types.
        [SuppressMessage("Microsoft.Security", "CA2123:OverrideLinkDemandsShouldBeIdenticalToBase")] // everything in this assembly is full trust.
        protected override Type[] GetImageExtenders() {
                return imageExtenders;
        }

        /// <include file='doc\ImageListImageEditor.uex' path='docs/doc[@for="ImageEditor.EditValue"]/*' />
        /// <devdoc>
        ///      Edits the given object value using the editor style provided by
        ///      GetEditorStyle.  A service provider is provided so that any
        ///      required editing services can be obtained.
        /// </devdoc>
        [SuppressMessage("Microsoft.Security", "CA2123:OverrideLinkDemandsShouldBeIdenticalToBase")] // everything in this assembly is full trust.
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
            ArrayList images = new ArrayList();
            if (provider != null) {
                IWindowsFormsEditorService edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
                if (edSvc != null) {
                    if (fileDialog == null) {
                        fileDialog = new OpenFileDialog();
                        fileDialog.Multiselect = true;
                        string filter = CreateFilterEntry(this);
                        for (int i = 0; i < GetImageExtenders().Length; i++) {
                            ImageEditor e = (ImageEditor) Activator.CreateInstance(GetImageExtenders()[i], BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.CreateInstance, null, null, null); //.CreateInstance();
                            Type myClass = this.GetType();
                            Type editorClass = e.GetType();
                            if (!myClass.Equals(editorClass) && e != null && myClass.IsInstanceOfType(e))
                                filter += "|" + CreateFilterEntry(e);
                        }
                        fileDialog.Filter = filter;
                    }

                    IntPtr hwndFocus = UnsafeNativeMethods.GetFocus();
                    try {
                        if (fileDialog.ShowDialog() == DialogResult.OK) {
                            foreach (string name in fileDialog.FileNames) {
                                
                                ImageListImage image;
                                FileStream file = new FileStream(name, FileMode.Open, FileAccess.Read, FileShare.Read);
                                image = LoadImageFromStream(file, name.EndsWith(".ico"));
                                image.Name = System.IO.Path.GetFileName(name);
                                images.Add(image);
                            }
                        }
                    }
                    finally {
                        if (hwndFocus != IntPtr.Zero) {
                            UnsafeNativeMethods.SetFocus(new HandleRef(null, hwndFocus));
                        }
                    }
                }
                return images;
            }
            return value;
        }

        /// <include file='doc\ImageListImageEditor.uex' path='docs/doc[@for="ImageListImageEditor.GetFileDialogDescription"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        [SuppressMessage("Microsoft.Security", "CA2123:OverrideLinkDemandsShouldBeIdenticalToBase")] // everything in this assembly is full trust.
        protected override string GetFileDialogDescription() {
            return SR.imageFileDescription;
        }

        /// <include file='doc\ImageListImageEditor.uex' path='docs/doc[@for="ImageListImageEditor.GetPaintValueSupported"]/*' />
        /// <devdoc>
        ///      Determines if this editor supports the painting of a representation
        ///      of an object's value.
        /// </devdoc>
        [SuppressMessage("Microsoft.Security", "CA2123:OverrideLinkDemandsShouldBeIdenticalToBase")] // everything in this assembly is full trust.
        public override bool GetPaintValueSupported(ITypeDescriptorContext context) {
            return true;
        }

        /// <include file='doc\ImageListImageEditor.uex' path='docs/doc[@for="ImageListImageEditor.LoadFromStream"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        private ImageListImage LoadImageFromStream(Stream stream, bool imageIsIcon) {
            //Copy the original stream to a buffer, then wrap a
            //memory stream around it.  This way we can avoid
            //locking the file
            byte[] buffer = new byte[stream.Length];
            stream.Read(buffer, 0, (int)stream.Length);
            MemoryStream ms = new MemoryStream(buffer);

            return (ImageListImage)ImageListImage.ImageListImageFromStream(ms, imageIsIcon);
        }


        /// <include file='doc\ImageListImageEditor.uex' path='docs/doc[@for="ImageListImageEditor.PaintValue"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Paints a representative value of the given object to the provided
        ///       canvas. Painting should be done within the boundaries of the
        ///       provided rectangle.
        ///    </para>
        /// </devdoc>
        [SuppressMessage("Microsoft.Security", "CA2123:OverrideLinkDemandsShouldBeIdenticalToBase")] // everything in this assembly is full trust.
        public override void PaintValue(PaintValueEventArgs e) {
            if (e.Value is ImageListImage) {
                e= new PaintValueEventArgs(e.Context, ((ImageListImage)e.Value).Image, e.Graphics, e.Bounds);
            }
            base.PaintValue(e);
            
        }
    }
    
    }

