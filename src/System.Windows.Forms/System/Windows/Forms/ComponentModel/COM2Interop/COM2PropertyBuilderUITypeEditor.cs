// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms.Design;
using Microsoft.VisualStudio.Shell;
using Windows.Win32.System.Variant;

namespace System.Windows.Forms.ComponentModel.Com2Interop;

internal class Com2PropertyBuilderUITypeEditor : Com2ExtendedUITypeEditor
{
    private readonly Com2PropertyDescriptor _propDesc;
    private readonly string _guidString;
    private readonly CTLBLDTYPE _bldrType;

    public Com2PropertyBuilderUITypeEditor(
        Com2PropertyDescriptor pd,
        string guidString,
        CTLBLDTYPE type,
        UITypeEditor? baseEditor) : base(baseEditor)
    {
        _propDesc = pd;
        _guidString = guidString;
        _bldrType = type;
    }

    public override unsafe object? EditValue(ITypeDescriptorContext? context, IServiceProvider provider, object? value)
    {
        HWND parentHandle = PInvoke.GetFocus();

        IUIService? uiSvc = (IUIService?)provider.GetService(typeof(IUIService));
        if (uiSvc is not null)
        {
            IWin32Window parent = uiSvc.GetDialogOwnerWindow();
            if (parent is not null)
            {
                parentHandle = (HWND)parent.Handle;
            }
        }

        object? target = _propDesc.TargetObject;
        if (target is ICustomTypeDescriptor customTypeDescriptor)
        {
            target = customTypeDescriptor.GetPropertyOwner(_propDesc);
        }

        using var propertyBuilder = ComHelpers.TryGetComScope<IProvidePropertyBuilder>(target, out HRESULT hr);
        Debug.Assert(hr.Succeeded, $"Failed to get IProvidePropertyBuilder: {hr}");

        VARIANT_BOOL useValue = VARIANT_BOOL.VARIANT_FALSE;

        // This is always an out value.
        using VARIANT variantValue = default;

        using BSTR guidString = new(_guidString);
        hr = propertyBuilder.Value->ExecuteBuilder(
            _propDesc.DISPID,
            &guidString,
            null,
            parentHandle,
            &variantValue,
            &useValue);

        if (hr.Failed)
        {
            useValue = VARIANT_BOOL.VARIANT_FALSE;
            Debug.Fail($"Failed to show property frame: {hr}");
        }

        return useValue == VARIANT_BOOL.VARIANT_TRUE && !_bldrType.HasFlag(CTLBLDTYPE.CTLBLDTYPE_FEDITSOBJIDRECTLY)
            ? variantValue.ToObject()
            : value;
    }

    public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext? context) => UITypeEditorEditStyle.Modal;
}
