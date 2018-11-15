// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope="member", Target="System.Drawing.Design.Com2ExtendedUITypeEditor..ctor(System.Type)")]


namespace System.Drawing.Design {

    using System.Diagnostics;

    using System.Collections;
    using Microsoft.Win32;
    using System.ComponentModel;
    using System.ComponentModel.Design;
    using System.Drawing;
    using System.Drawing.Design;

    /// <include file='doc\COM2ExtendedUITypeEditor.uex' path='docs/doc[@for="Com2ExtendedUITypeEditor"]/*' />
    /// <internalonly/>
    /// <devdoc>
    ///    <para>Provides an editor that provides a way to visually edit the values of a COM2 
    ///       type.</para>
    /// </devdoc>
    internal class Com2ExtendedUITypeEditor : UITypeEditor {
    
        private UITypeEditor innerEditor;
        
        public Com2ExtendedUITypeEditor(UITypeEditor baseTypeEditor) {
            this.innerEditor = baseTypeEditor;
        }
        
        public Com2ExtendedUITypeEditor(Type baseType) {
            this.innerEditor = (UITypeEditor)TypeDescriptor.GetEditor(baseType, typeof(UITypeEditor));
        }
        
        public UITypeEditor InnerEditor {
            get {
               return innerEditor;
            }
        }
  
        /// <include file='doc\COM2ExtendedUITypeEditor.uex' path='docs/doc[@for="Com2ExtendedUITypeEditor.EditValue"]/*' />
        /// <devdoc>
        ///      Edits the given object value using the editor style provided by
        ///      GetEditorStyle.  A service provider is provided so that any
        ///      required editing services can be obtained.
        /// </devdoc>
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
            if (innerEditor != null)  {
               return innerEditor.EditValue(context, provider, value);
            }
            else {
               return base.EditValue(context, provider, value);
            }
        }
  
        /// <include file='doc\COM2ExtendedUITypeEditor.uex' path='docs/doc[@for="Com2ExtendedUITypeEditor.GetPaintValueSupported"]/*' />
        /// <devdoc>
        ///      Determines if this editor supports the painting of a representation
        ///      of an object's value.
        /// </devdoc>
        public override bool GetPaintValueSupported(ITypeDescriptorContext context) {
            if (innerEditor != null) {
               return innerEditor.GetPaintValueSupported(context);
            }
            return base.GetPaintValueSupported(context);
        }

        /// <include file='doc\COM2ExtendedUITypeEditor.uex' path='docs/doc[@for="Com2ExtendedUITypeEditor.GetEditStyle"]/*' />
        /// <devdoc>
        ///      Retrieves the editing style of the Edit method.  If the method
        ///      is not supported, this will return None.
        /// </devdoc>
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
            if (innerEditor != null) {
               return innerEditor.GetEditStyle(context);
            }
            return base.GetEditStyle(context);
        }

        /// <include file='doc\COM2ExtendedUITypeEditor.uex' path='docs/doc[@for="Com2ExtendedUITypeEditor.PaintValue"]/*' />
        /// <devdoc>
        ///      Paints a representative value of the given object to the provided
        ///      canvas.  Painting should be done within the boundaries of the
        ///      provided rectangle.
        /// </devdoc>
        public override void PaintValue(PaintValueEventArgs e) {
            if (innerEditor != null) {
               innerEditor.PaintValue(e);
            }
            base.PaintValue(e);
        }
    }
}

