// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms.ComponentModel.Com2Interop {
    using System.ComponentModel;
    using System.Diagnostics;
    using System;
    using System.Collections;
    using Microsoft.Win32;
    using System.Drawing.Design;

    [System.Security.SuppressUnmanagedCodeSecurityAttribute()]
    internal class Com2IProvidePropertyBuilderHandler : Com2ExtendedBrowsingHandler {

        public override Type Interface {
            get {
                return typeof(NativeMethods.IProvidePropertyBuilder);
            }
        }

        private bool GetBuilderGuidString(NativeMethods.IProvidePropertyBuilder target, int dispid, ref string strGuidBldr, int[] bldrType) {
            bool valid = false;
            string[] pGuidBldr = new string[1];
            if (NativeMethods.Failed(target.MapPropertyToBuilder(dispid, bldrType, pGuidBldr, ref valid))) {
                valid = false;
            }
                        
            if (valid && (bldrType[0] & _CTLBLDTYPE.CTLBLDTYPE_FINTERNALBUILDER) == 0) {
                valid = false;
                Debug.Fail("Property Browser doesn't support standard builders -- NYI");
            }

            if (!valid) {
                return false;
            }

            if (pGuidBldr[0] == null) {
                strGuidBldr = Guid.Empty.ToString();
            }
            else {
                strGuidBldr = pGuidBldr[0];
            }
            return true;
        }

        public override void SetupPropertyHandlers(Com2PropertyDescriptor[] propDesc) {
            if (propDesc == null) {
                return;
            }
            for (int i = 0; i < propDesc.Length; i++) {
                propDesc[i].QueryGetBaseAttributes += new GetAttributesEventHandler(this.OnGetBaseAttributes);
                
                propDesc[i].QueryGetTypeConverterAndTypeEditor += new GetTypeConverterAndTypeEditorEventHandler(this.OnGetTypeConverterAndTypeEditor);
                
            }
        }


        /// <include file='doc\COM2IProvidePropertyBuilderHandler.uex' path='docs/doc[@for="Com2IProvidePropertyBuilderHandler.OnGetAttributes"]/*' />
        /// <devdoc>
        /// Here is where we handle IVsPerPropertyBrowsing.GetLocalizedPropertyInfo and IVsPerPropertyBrowsing.   HideProperty
        /// such as IPerPropertyBrowsing, IProvidePropertyBuilder, etc.
        /// </devdoc>
        private void OnGetBaseAttributes(Com2PropertyDescriptor sender, GetAttributesEvent attrEvent) {
            NativeMethods.IProvidePropertyBuilder target = sender.TargetObject as NativeMethods.IProvidePropertyBuilder;

            if (target != null ) {
                string s = null;
                bool builderValid = GetBuilderGuidString(target, sender.DISPID, ref s, new int[1]);
                // we hide IDispatch props by default, we we need to force showing them here
                if (sender.CanShow && builderValid) {
                    if (typeof(UnsafeNativeMethods.IDispatch).IsAssignableFrom(sender.PropertyType)) {
                        attrEvent.Add(BrowsableAttribute.Yes);
                    }
                }
            }
        }
        
        private void OnGetTypeConverterAndTypeEditor(Com2PropertyDescriptor sender, GetTypeConverterAndTypeEditorEvent gveevent) {
            object target = sender.TargetObject;

            if (target is NativeMethods.IProvidePropertyBuilder) {
                NativeMethods.IProvidePropertyBuilder propBuilder = (NativeMethods.IProvidePropertyBuilder)target;
                int[] pctlBldType = new int[1];
                string guidString = null;

                if (GetBuilderGuidString(propBuilder, sender.DISPID, ref guidString, pctlBldType)) {
                    gveevent.TypeEditor = new Com2PropertyBuilderUITypeEditor(sender, guidString, pctlBldType[0], (UITypeEditor)gveevent.TypeEditor);
                }
            }
        }
    }
}
