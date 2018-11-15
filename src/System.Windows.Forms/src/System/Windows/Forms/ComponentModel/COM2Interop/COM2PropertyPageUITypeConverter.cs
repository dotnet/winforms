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

    /// <include file='doc\COM2PropertyPageUITypeConverter.uex' path='docs/doc[@for="Com2PropertyPageUITypeEditor"]/*' />
    /// <devdoc>
    /// </devdoc>
    internal class Com2PropertyPageUITypeEditor : Com2ExtendedUITypeEditor, ICom2PropertyPageDisplayService {

        private Com2PropertyDescriptor propDesc;
        private Guid guid;

        public Com2PropertyPageUITypeEditor(Com2PropertyDescriptor pd, Guid guid, UITypeEditor baseEditor) : base(baseEditor){
            propDesc = pd;
            this.guid = guid;
        }

        /// <include file='doc\COM2PropertyPageUITypeConverter.uex' path='docs/doc[@for="Com2PropertyPageUITypeEditor.EditValue"]/*' />
        /// <devdoc>
        ///     Takes the value returned from valueAccess.getValue() and modifies or replaces
        ///     the value, passing the result into valueAccess.setValue().  This is where
        ///     an editor can launch a modal dialog or create a drop down editor to allow
        ///     the user to modify the value.  Host assistance in presenting UI to the user
        ///     can be found through the valueAccess.getService function.
        /// </devdoc>
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {

            IntPtr hWndParent = UnsafeNativeMethods.GetFocus(); // Windows.GetForegroundWindow

            try {


                ICom2PropertyPageDisplayService propPageSvc = (ICom2PropertyPageDisplayService)provider.GetService(typeof(ICom2PropertyPageDisplayService));

                if (propPageSvc == null) {
                    propPageSvc = this;
                }

                object instance = context.Instance;

                if (!instance.GetType().IsArray) {
                    instance = propDesc.TargetObject;
                    if (instance is ICustomTypeDescriptor) {
                        instance = ((ICustomTypeDescriptor)instance).GetPropertyOwner(propDesc);
                    }
                }

                propPageSvc.ShowPropertyPage(propDesc.Name, instance, propDesc.DISPID, this.guid, hWndParent);

            }
            catch (Exception ex1) {
                if (provider != null) {
                      IUIService uiSvc = (IUIService)provider.GetService(typeof(IUIService));
                      if (uiSvc != null){
                        uiSvc.ShowError(ex1, SR.ErrorTypeConverterFailed);
                      }
                }
            }
            return value;
        }

        /// <include file='doc\COM2PropertyPageUITypeConverter.uex' path='docs/doc[@for="Com2PropertyPageUITypeEditor.GetEditStyle"]/*' />
        /// <devdoc>
        ///      Retrieves the editing style of the Edit method.  If the method
        ///      is not supported, this will return None.
        /// </devdoc>
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
            return UITypeEditorEditStyle.Modal;
        }

        public unsafe void ShowPropertyPage(string title, object component, int dispid, Guid pageGuid, IntPtr parentHandle){
            Guid[] guids = new Guid[]{pageGuid};
            IntPtr guidsAddr = Marshal.UnsafeAddrOfPinnedArrayElement(guids, 0);

            object[] objs = component.GetType().IsArray ? (object[])component : new object[]{component};

            int nObjs =  objs.Length;
            IntPtr[] objAddrs = new IntPtr[nObjs];

            try {
                for (int i=0; i < nObjs; i++) {
                    objAddrs[i] = Marshal.GetIUnknownForObject(objs[i]);
                }

                fixed (IntPtr* pAddrs = objAddrs) {
                    SafeNativeMethods.OleCreatePropertyFrame(new HandleRef(null, parentHandle),
                                                             0, 0,
                                                             title,
                                                             nObjs,
                                                             new HandleRef(null, (IntPtr)(long)pAddrs),
                                                             1,
                                                             new HandleRef(null, guidsAddr),
                                                             SafeNativeMethods.GetThreadLCID(),
                                                             0, IntPtr.Zero );
                }
            } finally {
                for (int i=0; i < nObjs; i++) {
                    if (objAddrs[i] != IntPtr.Zero) {
                        Marshal.Release(objAddrs[i]);
                    }
                }
            }
        }
    }

}
