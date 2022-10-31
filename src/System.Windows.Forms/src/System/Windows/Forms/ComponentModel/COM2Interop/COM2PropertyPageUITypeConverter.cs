// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Drawing.Design;
using System.Runtime.InteropServices;
using System.Windows.Forms.Design;
using static Interop;

namespace System.Windows.Forms.ComponentModel.Com2Interop
{
    internal class Com2PropertyPageUITypeEditor : Com2ExtendedUITypeEditor, ICom2PropertyPageDisplayService
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
            HWND hWndParent = PInvoke.GetFocus();

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

                propertyPageService.ShowPropertyPage(_propertyDescriptor.Name, instance, (int)_propertyDescriptor.DISPID, _guid, hWndParent);
            }
            catch (Exception ex)
            {
                if (provider.TryGetService(out IUIService? uiService))
                {
                    uiService?.ShowError(ex, SR.ErrorTypeConverterFailed);
                }
            }

            return value;
        }

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext? context) => UITypeEditorEditStyle.Modal;

        public unsafe void ShowPropertyPage(string title, object component, int dispid, Guid pageGuid, IntPtr parentHandle)
        {
            object[] objs = component.GetType().IsArray ? (object[])component : new object[] { component };
            IntPtr[] objAddrs = new IntPtr[objs.Length];

            try
            {
                for (int i = 0; i < objAddrs.Length; i++)
                {
                    objAddrs[i] = Marshal.GetIUnknownForObject(objs[i]);
                }

                fixed (IntPtr* pObjAddrs = objAddrs)
                {
                    Oleaut32.OleCreatePropertyFrame(
                        parentHandle,
                        0,
                        0,
                        title,
                        (uint)objAddrs.Length,
                        pObjAddrs,
                        1,
                        &pageGuid,
                        PInvoke.GetThreadLocale(),
                        0,
                        IntPtr.Zero);
                }
            }
            finally
            {
                for (int i = 0; i < objAddrs.Length; i++)
                {
                    if (objAddrs[i] != IntPtr.Zero)
                    {
                        Marshal.Release(objAddrs[i]);
                    }
                }
            }
        }
    }
}
