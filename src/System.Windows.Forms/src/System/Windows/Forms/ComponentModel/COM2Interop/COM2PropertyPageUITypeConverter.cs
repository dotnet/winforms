// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing.Design;
using System.Runtime.InteropServices;
using System.Windows.Forms.Design;
using Windows.Win32.System.Com;

namespace System.Windows.Forms.ComponentModel.Com2Interop;

internal sealed unsafe class Com2PropertyPageUITypeEditor : Com2ExtendedUITypeEditor, ICom2PropertyPageDisplayService
{
    private readonly Com2PropertyDescriptor _propertyDescriptor;
    private readonly Guid _guid;

    public Com2PropertyPageUITypeEditor(
        Com2PropertyDescriptor propertyDescriptor,
        Guid guid,
        UITypeEditor? baseEditor) : base(baseEditor)
    {
        _propertyDescriptor = propertyDescriptor;
        _guid = guid;
    }

    public override object? EditValue(ITypeDescriptorContext? context, IServiceProvider provider, object? value)
    {
        HWND parentHandle = PInvoke.GetFocus();

        try
        {
            if (!provider.TryGetService(out ICom2PropertyPageDisplayService? propertyPageService))
            {
                propertyPageService ??= this;
            }

            object? instance = context?.Instance;

            if (instance is not null && !instance.GetType().IsArray)
            {
                instance = _propertyDescriptor.TargetObject;
                if (instance is ICustomTypeDescriptor customTypeDescriptor)
                {
                    instance = customTypeDescriptor.GetPropertyOwner(_propertyDescriptor);
                }
            }

            Debug.Assert(instance is not null);
            propertyPageService.ShowPropertyPage(_propertyDescriptor.Name, instance, _propertyDescriptor.DISPID, _guid, parentHandle);
        }
        catch (Exception ex)
        {
            if (provider.TryGetService(out IUIService? uiService))
            {
                uiService.ShowError(ex, SR.ErrorTypeConverterFailed);
            }
        }

        return value;
    }

    public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext? context) => UITypeEditorEditStyle.Modal;

    public void ShowPropertyPage(string title, object component, int dispid, Guid pageGuid, nint parentHandle)
    {
        object[] objects = component.GetType().IsArray ? (object[])component : [component];
        nint[] addresses = new nint[objects.Length];

        try
        {
            for (int i = 0; i < addresses.Length; i++)
            {
                addresses[i] = (nint)ComHelpers.GetComPointer<IUnknown>(objects[i]);
            }

            fixed (void* pObjAddrs = addresses)
            {
                PInvoke.OleCreatePropertyFrame(
                    (HWND)parentHandle,
                    0,
                    0,
                    title,
                    (uint)addresses.Length,
                    (IUnknown**)pObjAddrs,
                    1,
                    pageGuid,
                    PInvokeCore.GetThreadLocale()).ThrowOnFailure();
            }
        }
        finally
        {
            for (int i = 0; i < addresses.Length; i++)
            {
                if (addresses[i] != 0)
                {
                    Marshal.Release(addresses[i]);
                }
            }
        }
    }
}
