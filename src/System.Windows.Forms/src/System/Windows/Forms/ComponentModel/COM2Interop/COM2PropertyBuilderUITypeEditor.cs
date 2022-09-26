// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Diagnostics;
using System.Drawing.Design;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Windows.Forms.Design;
using static Interop;

namespace System.Windows.Forms.ComponentModel.Com2Interop
{
    internal class Com2PropertyBuilderUITypeEditor : Com2ExtendedUITypeEditor
    {
        private readonly Com2PropertyDescriptor _propDesc;
        private readonly string _guidString;
        private readonly VSSDK.CTLBLDTYPE _bldrType;

        public Com2PropertyBuilderUITypeEditor(Com2PropertyDescriptor pd, string guidString, VSSDK.CTLBLDTYPE type, UITypeEditor baseEditor) : base(baseEditor)
        {
            _propDesc = pd;
            _guidString = guidString;
            _bldrType = type;
        }

        /// <summary>
        ///  Takes the value returned from valueAccess.getValue() and modifies or replaces
        ///  the value, passing the result into valueAccess.setValue().  This is where
        ///  an editor can launch a modal dialog or create a drop down editor to allow
        ///  the user to modify the value.  Host assistance in presenting UI to the user
        ///  can be found through the valueAccess.getService function.
        /// </summary>
        public unsafe override object? EditValue(ITypeDescriptorContext? context, IServiceProvider provider, object? value)
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

            BOOL useValue = false;
            object? pValue = value;

            try
            {
                object? obj = _propDesc.TargetObject;
                if (obj is ICustomTypeDescriptor customTypeDescriptor)
                {
                    obj = customTypeDescriptor.GetPropertyOwner(_propDesc);
                }

                Debug.Assert(obj is VSSDK.IProvidePropertyBuilder, "object is not IProvidePropertyBuilder");
                VSSDK.IProvidePropertyBuilder propBuilder = (VSSDK.IProvidePropertyBuilder)obj;

                if (!propBuilder.ExecuteBuilder(
                    _propDesc.DISPID,
                    _guidString,
                    null,
                    parentHandle,
                    ref pValue,
                    &useValue).Succeeded)
                {
                    useValue = false;
                }
            }
            catch (ExternalException ex)
            {
                Debug.Fail("Failed to show property frame: " + ex.ErrorCode.ToString(CultureInfo.InvariantCulture));
            }

            if (useValue && (_bldrType & VSSDK.CTLBLDTYPE.FEDITSOBJIDRECTLY) == 0)
            {
                return pValue;
            }

            return value;
        }

        /// <summary>
        ///  Retrieves the editing style of the Edit method.  If the method
        ///  is not supported, this will return None.
        /// </summary>
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext? context)
        {
            return UITypeEditorEditStyle.Modal;
        }
    }
}
