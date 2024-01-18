// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing.Design;
using Windows.Win32.System.Com;

namespace System.Windows.Forms.ComponentModel.Com2Interop;

internal partial class Com2AboutBoxPropertyDescriptor
{
    public unsafe class AboutBoxUITypeEditor : UITypeEditor
    {
        public override object? EditValue(ITypeDescriptorContext? context, IServiceProvider provider, object? value)
        {
            ArgumentNullException.ThrowIfNull(context);
            object? component = context.Instance;

            using var dispatch = ComHelpers.TryGetComScope<IDispatch>(context.Instance, out HRESULT hr);

            if (hr.Succeeded)
            {
                EXCEPINFO pExcepInfo = default;
                DISPPARAMS dispParams = default;
                hr = dispatch.Value->Invoke(
                    PInvokeCore.DISPID_ABOUTBOX,
                    IID.NULL(),
                    PInvokeCore.GetThreadLocale(),
                    DISPATCH_FLAGS.DISPATCH_METHOD,
                    &dispParams,
                    pVarResult: null,
                    &pExcepInfo,
                    puArgErr: null);
                Debug.Assert(hr.Succeeded, $"Failed to launch about box. {hr}");
            }

            return value;
        }

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext? context) => UITypeEditorEditStyle.Modal;
    }
}
