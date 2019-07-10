//------------------------------------------------------------------------------
// <copyright file="ImageCollectionEditor.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>                                                                
//------------------------------------------------------------------------------

[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope="member", Target="System.Windows.Forms.Design.ImageCollectionEditor..ctor(System.Type)")]

namespace System.Windows.Forms.Design {
    
    using System.Runtime.InteropServices;

    using System.Diagnostics;
    using System;
    using System.IO;
    using System.Collections;
    using System.ComponentModel;
    using System.ComponentModel.Design;
    using System.Windows.Forms;
    using System.Drawing;
    using System.Drawing.Design;
    using System.Windows.Forms.ComponentModel;

    /// <include file='doc\ImageCollectionEditor.uex' path='docs/doc[@for="ImageCollectionEditor"]/*' />
    /// <devdoc>
    ///    <para> 
    ///       Provides an editor for an image collection.</para>
    /// </devdoc>
    internal class ImageCollectionEditor : CollectionEditor {
    
        /// <include file='doc\ImageCollectionEditor.uex' path='docs/doc[@for="ImageCollectionEditor.ImageCollectionEditor"]/*' />
        /// <devdoc>
        /// <para>Initializes a new instance of the <see cref='System.Windows.Forms.Design.ImageCollectionEditor'/> class.</para>
        /// </devdoc>
        public ImageCollectionEditor(Type type) : base(type){
        }

        /// <include file='doc\ImageCollectionEditor.uex' path='docs/doc[@for="ImageCollectionEditor.GetDisplayText"]/*' />
        /// <devdoc>
        ///      Retrieves the display text for the given list item.
        /// </devdoc>
        protected override string GetDisplayText(object value) {
            string text;
            
            if (value == null) {
                return string.Empty;
            }

            PropertyDescriptor prop = TypeDescriptor.GetProperties(value)["Name"];
            if (prop != null) {
                text = (string) prop.GetValue( value );
                if (text != null && text.Length > 0) {
                    return text;
                }
            }

            // If we want to show any type information - pretend we're an image.
            if (value is ImageListImage) {
                value = ((ImageListImage)value).Image;
            }
            
            text = TypeDescriptor.GetConverter(value).ConvertToString(value);

            if (text == null || text.Length == 0) {
                text = value.GetType().Name;
            }

            return text;
        }
        /// <include file='doc\ImageCollectionEditor.uex' path='docs/doc[@for="ImageCollectionEditor.CreateInstance"]/*' />
        /// <devdoc>
        ///    <para>Creates an instance of the specified type in the collection.</para>
        /// </devdoc>
        protected override object CreateInstance(Type type) {
            UITypeEditor editor = (UITypeEditor) TypeDescriptor.GetEditor(typeof(ImageListImage), typeof(UITypeEditor));
            return editor.EditValue(this.Context, null);
        }

        /// <include file='doc\CollectionEditor.uex' path='docs/doc[@for="CollectionEditor.CreateCollectionForm"]/*' />
        /// <devdoc>
        ///    <para>Creates a
        ///       new form to show the current collection.</para>
        /// </devdoc>
        protected override CollectionForm CreateCollectionForm() {
            CollectionForm form = base.CreateCollectionForm();
            
            // We want to switch the title to ImageCollection Editor instead of ImageListImage Editor.
            // The collection editor is actually using ImageListImages, while the collection we're actually editing is the Image collection.
            form.Text = SR.ImageCollectionEditorFormText;
            return form;
        }

        /// <include file='doc\ImageCollectionEditor.uex' path='docs/doc[@for="ImageCollectionEditor.GetObjectsfromInstance"]/*' />
        /// <devdoc>
        ///    <para>
        ///       This Function gets the images from the givem object. The input is an ArrayList as an object.
        ///       The output is a arraylist which contains the individual images that need to be created.
        ///    </para>
        /// </devdoc>
        protected override IList GetObjectsFromInstance(object instance) {
            ArrayList value = instance as ArrayList;
            if (value != null) {
                return value;
            }
            else return null;
        }
    }

}
