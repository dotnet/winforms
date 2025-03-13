// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing.Design;
using Microsoft.VisualStudio.Shell;

namespace System.Windows.Forms.ComponentModel.Com2Interop;

/// <summary>
///  Browsing handler for <see cref="IProvidePropertyBuilder"/>.
/// </summary>
internal sealed unsafe class Com2IProvidePropertyBuilderHandler : Com2ExtendedBrowsingHandler<IProvidePropertyBuilder>
{
    private bool GetBuilderGuidString(
        IProvidePropertyBuilder* target,
        int dispid,
        [NotNullWhen(true)] ref string? builderGuid,
        CTLBLDTYPE* builderType)
    {
        VARIANT_BOOL valid = VARIANT_BOOL.VARIANT_FALSE;
        using BSTR guid = default;
        if (!target->MapPropertyToBuilder(dispid, builderType, &guid, &valid).Succeeded)
        {
            return false;
        }

        if (valid == VARIANT_BOOL.VARIANT_TRUE && (*builderType & CTLBLDTYPE.CTLBLDTYPE_FINTERNALBUILDER) == 0)
        {
            Debug.Fail("Property Browser doesn't support standard builders -- NYI");
            return false;
        }

        builderGuid = guid.ToString() ?? Guid.Empty.ToString();
        return true;
    }

    public override void RegisterEvents(Com2PropertyDescriptor[]? properties)
    {
        if (properties is null)
        {
            return;
        }

        for (int i = 0; i < properties.Length; i++)
        {
            properties[i].QueryGetBaseAttributes += OnGetBaseAttributes;
            properties[i].QueryGetTypeConverterAndTypeEditor += OnGetTypeConverterAndTypeEditor;
        }
    }

    private void OnGetBaseAttributes(Com2PropertyDescriptor sender, GetAttributesEvent e)
    {
        using var propertyBuilder = TryGetComScope(sender.TargetObject, out HRESULT hr);
        if (hr.Failed)
        {
            return;
        }

        string? builderGuid = null;
        CTLBLDTYPE builderType = 0;
        bool builderValid = GetBuilderGuidString(propertyBuilder, sender.DISPID, ref builderGuid, &builderType);

        // We hide IDispatch props by default, we need to force showing them here.
        if (sender.CanShow && builderValid)
        {
            e.Add(BrowsableAttribute.Yes);
        }
    }

    private void OnGetTypeConverterAndTypeEditor(Com2PropertyDescriptor sender, GetTypeConverterAndTypeEditorEvent e)
    {
        using var propertyBuilder = TryGetComScope(sender.TargetObject, out HRESULT hr);
        if (hr.Failed)
        {
            return;
        }

        string? builderGuid = null;
        CTLBLDTYPE builderType = 0;
        if (GetBuilderGuidString(propertyBuilder, sender.DISPID, ref builderGuid, &builderType))
        {
            e.TypeEditor = new Com2PropertyBuilderUITypeEditor(sender, builderGuid, builderType, (UITypeEditor?)e.TypeEditor);
        }
    }
}
