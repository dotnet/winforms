﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Diagnostics;
using System.Drawing.Design;
using System.Runtime.InteropServices;
using Windows.Win32.System.Com;
using static Interop;

namespace System.Windows.Forms.ComponentModel.Com2Interop;

internal partial class Com2AboutBoxPropertyDescriptor
{
    public class AboutBoxUITypeEditor : UITypeEditor
    {
        public override unsafe object? EditValue(ITypeDescriptorContext? context, IServiceProvider provider, object? value)
        {
            ArgumentNullException.ThrowIfNull(context);
            object? component = context.Instance;

            if (component is not null
                && Marshal.IsComObject(component)
                && component is Oleaut32.IDispatch pDisp)
            {
                EXCEPINFO pExcepInfo = default;
                DISPPARAMS dispParams = default;
                Guid g = Guid.Empty;
                HRESULT hr = pDisp.Invoke(
                    PInvoke.DISPID_ABOUTBOX,
                    &g,
                    PInvoke.GetThreadLocale(),
                    DISPATCH_FLAGS.DISPATCH_METHOD,
                    &dispParams,
                    null,
                    &pExcepInfo,
                    null);
                Debug.Assert(hr.Succeeded, "Failed to launch about box.");
            }

            return value;
        }

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext? context) => UITypeEditorEditStyle.Modal;
    }
}
