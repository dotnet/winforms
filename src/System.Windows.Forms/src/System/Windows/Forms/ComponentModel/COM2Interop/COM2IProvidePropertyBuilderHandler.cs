// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Diagnostics;
using System.Drawing.Design;
using static Interop;

namespace System.Windows.Forms.ComponentModel.Com2Interop
{
    internal class Com2IProvidePropertyBuilderHandler : Com2ExtendedBrowsingHandler
    {
        public override Type Interface
        {
            get
            {
                return typeof(NativeMethods.IProvidePropertyBuilder);
            }
        }

        private unsafe bool GetBuilderGuidString(NativeMethods.IProvidePropertyBuilder target, Ole32.DispatchID dispid, ref string strGuidBldr, int[] bldrType)
        {
            BOOL valid = BOOL.FALSE;
            var pGuidBldr = new string[1];
            if (!target.MapPropertyToBuilder(dispid, bldrType, pGuidBldr, &valid).Succeeded())
            {
                return false;
            }

            if (valid != BOOL.FALSE && (bldrType[0] & _CTLBLDTYPE.CTLBLDTYPE_FINTERNALBUILDER) == 0)
            {
                Debug.Fail("Property Browser doesn't support standard builders -- NYI");
                return false;
            }

            strGuidBldr = pGuidBldr[0] ?? Guid.Empty.ToString();
            return true;
        }

        public override void SetupPropertyHandlers(Com2PropertyDescriptor[] propDesc)
        {
            if (propDesc == null)
            {
                return;
            }
            for (int i = 0; i < propDesc.Length; i++)
            {
                propDesc[i].QueryGetBaseAttributes += new GetAttributesEventHandler(OnGetBaseAttributes);

                propDesc[i].QueryGetTypeConverterAndTypeEditor += new GetTypeConverterAndTypeEditorEventHandler(OnGetTypeConverterAndTypeEditor);

            }
        }

        /// <summary>
        ///  Here is where we handle IVsPerPropertyBrowsing.GetLocalizedPropertyInfo and IVsPerPropertyBrowsing.   HideProperty
        ///  such as IPerPropertyBrowsing, IProvidePropertyBuilder, etc.
        /// </summary>
        private void OnGetBaseAttributes(Com2PropertyDescriptor sender, GetAttributesEvent attrEvent)
        {
            if (sender.TargetObject is NativeMethods.IProvidePropertyBuilder target)
            {
                string s = null;
                bool builderValid = GetBuilderGuidString(target, sender.DISPID, ref s, new int[1]);
                // we hide IDispatch props by default, we we need to force showing them here
                if (sender.CanShow && builderValid)
                {
                    if (typeof(UnsafeNativeMethods.IDispatch).IsAssignableFrom(sender.PropertyType))
                    {
                        attrEvent.Add(BrowsableAttribute.Yes);
                    }
                }
            }
        }

        private void OnGetTypeConverterAndTypeEditor(Com2PropertyDescriptor sender, GetTypeConverterAndTypeEditorEvent gveevent)
        {
            object target = sender.TargetObject;

            if (target is NativeMethods.IProvidePropertyBuilder propBuilder)
            {
                int[] pctlBldType = new int[1];
                string guidString = null;

                if (GetBuilderGuidString(propBuilder, sender.DISPID, ref guidString, pctlBldType))
                {
                    gveevent.TypeEditor = new Com2PropertyBuilderUITypeEditor(sender, guidString, pctlBldType[0], (UITypeEditor)gveevent.TypeEditor);
                }
            }
        }
    }
}
