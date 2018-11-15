// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms.ComponentModel.Com2Interop {

    using Microsoft.Win32;
    using System;
    using System.ComponentModel;
    using System.ComponentModel.Design;
    using System.Diagnostics;
    using System.Drawing;
    using System.Drawing.Design;
    using System.Runtime.InteropServices;
    using System.Windows.Forms.Design;
    using System.Globalization;

    /// <include file='doc\COM2PropertyBuilderUITypeEditor.uex' path='docs/doc[@for="Com2PropertyBuilderUITypeEditor"]/*' />
    /// <devdoc>
    /// </devdoc>
    internal class Com2PropertyBuilderUITypeEditor : Com2ExtendedUITypeEditor {

        private Com2PropertyDescriptor propDesc;
        string  guidString;
        int     bldrType;

        public Com2PropertyBuilderUITypeEditor(Com2PropertyDescriptor pd, string guidString, int type, UITypeEditor baseEditor) : base(baseEditor) {
            propDesc = pd;
            this.guidString = guidString;
            this.bldrType = type;
        }

        /// <include file='doc\COM2PropertyBuilderUITypeEditor.uex' path='docs/doc[@for="Com2PropertyBuilderUITypeEditor.EditValue"]/*' />
        /// <devdoc>
        ///     Takes the value returned from valueAccess.getValue() and modifies or replaces
        ///     the value, passing the result into valueAccess.setValue().  This is where
        ///     an editor can launch a modal dialog or create a drop down editor to allow
        ///     the user to modify the value.  Host assistance in presenting UI to the user
        ///     can be found through the valueAccess.getService function.
        /// </devdoc>
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {

            IntPtr parentHandle = (IntPtr)UnsafeNativeMethods.GetFocus();

            IUIService uiSvc = (IUIService)provider.GetService(typeof(IUIService));
            if (uiSvc != null) {
                IWin32Window parent = uiSvc.GetDialogOwnerWindow();
                if (parent != null) {
                    parentHandle = parent.Handle;
                }
            }
            
            bool useValue = false;
            //VARIANT pValue = null;
            object pValue = value;

            try{
               object obj = propDesc.TargetObject;
                
              
               if (obj is ICustomTypeDescriptor) {
                  obj = ((ICustomTypeDescriptor)obj).GetPropertyOwner(propDesc);
               }

                
                Debug.Assert(obj is NativeMethods.IProvidePropertyBuilder, "object is not IProvidePropertyBuilder");
                NativeMethods.IProvidePropertyBuilder propBuilder = (NativeMethods.IProvidePropertyBuilder)obj;

                if (NativeMethods.Failed(propBuilder.ExecuteBuilder(propDesc.DISPID,
                                                          guidString,
                                                          null,
                                                          new HandleRef(null, parentHandle),
                                                          ref pValue, ref useValue))){
                    useValue = false;
                }

            }catch(ExternalException ex) {
                Debug.Fail("Failed to show property frame: " + ex.ErrorCode.ToString(CultureInfo.InvariantCulture));
            }

            if (useValue && (bldrType & _CTLBLDTYPE.CTLBLDTYPE_FEDITSOBJDIRECTLY) == 0){
               
               return pValue;//pValue.ToVariant();
            }
            return value;
        }

        /// <include file='doc\COM2PropertyBuilderUITypeEditor.uex' path='docs/doc[@for="Com2PropertyBuilderUITypeEditor.GetEditStyle"]/*' />
        /// <devdoc>
        ///      Retrieves the editing style of the Edit method.  If the method
        ///      is not supported, this will return None.
        /// </devdoc>
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
            return UITypeEditorEditStyle.Modal;
        }

    }

}
