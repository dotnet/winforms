// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing.Design;
using Microsoft.VisualStudio.Shell;
using static Interop;

namespace System.Windows.Forms.ComponentModel.Com2Interop;

internal class Com2IProvidePropertyBuilderHandler : Com2ExtendedBrowsingHandler
{
    public override Type Interface => typeof(IProvidePropertyBuilder.Interface);

    private unsafe bool GetBuilderGuidString(
        IProvidePropertyBuilder.Interface target,
        int dispid,
        [NotNullWhen(true)] ref string? strGuidBldr,
        CTLBLDTYPE* bldrType)
    {
        VARIANT_BOOL valid = VARIANT_BOOL.VARIANT_FALSE;
        using BSTR pGuidBldr = default;
        if (!target.MapPropertyToBuilder(dispid, bldrType, &pGuidBldr, &valid).Succeeded)
        {
            return false;
        }

        if (valid == VARIANT_BOOL.VARIANT_TRUE && (*bldrType & CTLBLDTYPE.CTLBLDTYPE_FINTERNALBUILDER) == 0)
        {
            Debug.Fail("Property Browser doesn't support standard builders -- NYI");
            return false;
        }

        strGuidBldr = pGuidBldr.ToString() ?? Guid.Empty.ToString();
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
            propDesc[i].QueryGetBaseAttributes += OnGetBaseAttributes;
            propDesc[i].QueryGetTypeConverterAndTypeEditor += OnGetTypeConverterAndTypeEditor;
        }
    }

    /// <summary>
    ///  Here is where we handle IVsPerPropertyBrowsing.GetLocalizedPropertyInfo and IVsPerPropertyBrowsing. We
    ///  hide properties such as IPerPropertyBrowsing, IProvidePropertyBuilder, etc.
    /// </summary>
    private unsafe void OnGetBaseAttributes(Com2PropertyDescriptor sender, GetAttributesEvent attrEvent)
    {
        if (sender.TargetObject is not IProvidePropertyBuilder.Interface target)
        {
            return;
        }

        string? guidString = null;
        CTLBLDTYPE bldrType = 0;
        bool builderValid = GetBuilderGuidString(target, sender.DISPID, ref guidString, &bldrType);

        // We hide IDispatch props by default, we we need to force showing them here
        if (sender.CanShow && builderValid && typeof(Oleaut32.IDispatch).IsAssignableFrom(sender.PropertyType))
        {
            attrEvent.Add(BrowsableAttribute.Yes);
        }
    }

    private unsafe void OnGetTypeConverterAndTypeEditor(Com2PropertyDescriptor sender, GetTypeConverterAndTypeEditorEvent gveevent)
    {
        if (sender.TargetObject is IProvidePropertyBuilder.Interface propertyBuilder)
        {
            string? guidString = null;
            CTLBLDTYPE pctlBldType = 0;
            if (GetBuilderGuidString(propertyBuilder, sender.DISPID, ref guidString, &pctlBldType))
            {
                gveevent.TypeEditor = new Com2PropertyBuilderUITypeEditor(sender, guidString, pctlBldType, (UITypeEditor?)gveevent.TypeEditor);
            }
        }
    }
}
