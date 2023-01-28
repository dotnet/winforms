// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Diagnostics;
using System.Drawing.Design;
using System.Runtime.InteropServices;
using System.Windows.Forms.Design;
using Microsoft.VisualStudio.Shell;
using Windows.Win32.System.Com;

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

        VARIANT_BOOL useValue = VARIANT_BOOL.VARIANT_FALSE;
        VARIANT variantValue = default;

        try
        {
            object? obj = _propDesc.TargetObject;
            if (obj is ICustomTypeDescriptor customTypeDescriptor)
            {
                obj = customTypeDescriptor.GetPropertyOwner(_propDesc);
            }

            Debug.Assert(obj is IProvidePropertyBuilder.Interface, "object is not IProvidePropertyBuilder");
            IProvidePropertyBuilder.Interface propBuilder = (IProvidePropertyBuilder.Interface)obj;

            // Is it actually necessary to pass the value in?
            Marshal.GetNativeVariantForObject(value, (nint)(void*)&variantValue);

            using BSTR guidString = new(_guidString);
            if (!propBuilder.ExecuteBuilder(
                _propDesc.DISPID,
                &guidString,
                null,
                parentHandle,
                &variantValue,
                &useValue).Succeeded)
            {
                useValue = VARIANT_BOOL.VARIANT_FALSE;
            }
        }
        catch (ExternalException ex)
        {
            Debug.Fail($"Failed to show property frame: {ex.ErrorCode}");
        }

        return useValue == VARIANT_BOOL.VARIANT_TRUE && (_bldrType & CTLBLDTYPE.CTLBLDTYPE_FEDITSOBJIDRECTLY) == 0
            ? variantValue.ToObject()
            : value;
    }

    public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext? context) => UITypeEditorEditStyle.Modal;
}
