// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.ComponentModel;
using System.Drawing.Design;
using System.Runtime.InteropServices;
using System.Windows.Forms.Design;
using static Interop;

namespace System.Windows.Forms.ComponentModel.Com2Interop
{
    internal class Com2PropertyPageUITypeEditor : Com2ExtendedUITypeEditor, ICom2PropertyPageDisplayService
    {
        private readonly Com2PropertyDescriptor propDesc;
        private Guid guid;

        public Com2PropertyPageUITypeEditor(Com2PropertyDescriptor pd, Guid guid, UITypeEditor baseEditor) : base(baseEditor)
        {
            propDesc = pd;
            this.guid = guid;
        }

        /// <summary>
        ///  Takes the value returned from valueAccess.getValue() and modifies or replaces
        ///  the value, passing the result into valueAccess.setValue().  This is where
        ///  an editor can launch a modal dialog or create a drop down editor to allow
        ///  the user to modify the value.  Host assistance in presenting UI to the user
        ///  can be found through the valueAccess.getService function.
        /// </summary>
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            IntPtr hWndParent = User32.GetFocus(); // Windows.GetForegroundWindow

            try
            {
                ICom2PropertyPageDisplayService propPageSvc = (ICom2PropertyPageDisplayService)provider.GetService(typeof(ICom2PropertyPageDisplayService));

                if (propPageSvc is null)
                {
                    propPageSvc = this;
                }

                object instance = context.Instance;

                if (!instance.GetType().IsArray)
                {
                    instance = propDesc.TargetObject;
                    if (instance is ICustomTypeDescriptor)
                    {
                        instance = ((ICustomTypeDescriptor)instance).GetPropertyOwner(propDesc);
                    }
                }

                propPageSvc.ShowPropertyPage(propDesc.Name, instance, (int)propDesc.DISPID, guid, hWndParent);
            }
            catch (Exception ex1)
            {
                if (provider != null)
                {
                    IUIService uiSvc = (IUIService)provider.GetService(typeof(IUIService));
                    if (uiSvc != null)
                    {
                        uiSvc.ShowError(ex1, SR.ErrorTypeConverterFailed);
                    }
                }
            }
            return value;
        }

        /// <summary>
        ///  Retrieves the editing style of the Edit method.  If the method
        ///  is not supported, this will return None.
        /// </summary>
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }

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
                        Kernel32.GetThreadLocale(),
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
