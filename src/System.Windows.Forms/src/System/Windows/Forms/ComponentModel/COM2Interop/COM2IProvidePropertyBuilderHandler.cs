﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing.Design;
using static Interop;

namespace System.Windows.Forms.ComponentModel.Com2Interop
{
    internal class Com2IProvidePropertyBuilderHandler : Com2ExtendedBrowsingHandler
    {
        public override Type Interface => typeof(VSSDK.IProvidePropertyBuilder);

        private unsafe bool GetBuilderGuidString(
            VSSDK.IProvidePropertyBuilder target,
            Ole32.DispatchID dispid,
            [NotNullWhen(true)] ref string? strGuidBldr,
            VSSDK.CTLBLDTYPE* bldrType)
        {
            BOOL valid = false;
            var pGuidBldr = new string[1];
            if (!target.MapPropertyToBuilder(dispid, bldrType, pGuidBldr, &valid).Succeeded)
            {
                return false;
            }

            if (valid && (*bldrType & VSSDK.CTLBLDTYPE.FINTERNALBUILDER) == 0)
            {
                Debug.Fail("Property Browser doesn't support standard builders -- NYI");
                return false;
            }

            strGuidBldr = pGuidBldr[0] ?? Guid.Empty.ToString();
            return true;
        }

        public override void SetupPropertyHandlers(Com2PropertyDescriptor[]? propDesc)
        {
            if (propDesc is null)
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
        ///  Here is where we handle IVsPerPropertyBrowsing.GetLocalizedPropertyInfo and IVsPerPropertyBrowsing. We
        ///  hide properties such as IPerPropertyBrowsing, IProvidePropertyBuilder, etc.
        /// </summary>
        private unsafe void OnGetBaseAttributes(Com2PropertyDescriptor sender, GetAttributesEvent attrEvent)
        {
            if (sender.TargetObject is not VSSDK.IProvidePropertyBuilder target)
            {
                return;
            }

            string? guidString = null;
            VSSDK.CTLBLDTYPE bldrType = 0;
            bool builderValid = GetBuilderGuidString(target, sender.DISPID, ref guidString, &bldrType);

            // We hide IDispatch props by default, we we need to force showing them here
            if (sender.CanShow && builderValid && typeof(Oleaut32.IDispatch).IsAssignableFrom(sender.PropertyType))
            {
                attrEvent.Add(BrowsableAttribute.Yes);
            }
        }

        private unsafe void OnGetTypeConverterAndTypeEditor(Com2PropertyDescriptor sender, GetTypeConverterAndTypeEditorEvent gveevent)
        {
            if (sender.TargetObject is VSSDK.IProvidePropertyBuilder propertyBuilder)
            {
                string? guidString = null;
                VSSDK.CTLBLDTYPE pctlBldType = 0;
                if (GetBuilderGuidString(propertyBuilder, sender.DISPID, ref guidString, &pctlBldType))
                {
                    gveevent.TypeEditor = new Com2PropertyBuilderUITypeEditor(sender, guidString, pctlBldType, (UITypeEditor?)gveevent.TypeEditor);
                }
            }
        }
    }
}
