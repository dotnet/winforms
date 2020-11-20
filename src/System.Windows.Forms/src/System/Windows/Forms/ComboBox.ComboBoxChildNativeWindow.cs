// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Runtime.InteropServices;
using static Interop;
using static Interop.User32;

namespace System.Windows.Forms
{
    public partial class ComboBox
    {
        private class ComboBoxChildNativeWindow : NativeWindow
        {
            private readonly ComboBox _owner;
            private InternalAccessibleObject _accessibilityObject;
            private readonly ChildWindowType _childWindowType;

            public ComboBoxChildNativeWindow(ComboBox comboBox, ChildWindowType childWindowType)
            {
                _owner = comboBox;
                _childWindowType = childWindowType;
            }

            protected override void WndProc(ref Message m)
            {
                switch ((User32.WM)m.Msg)
                {
                    case WM.GETOBJECT:
                        WmGetObject(ref m);
                        return;
                    case WM.MOUSEMOVE:
                        if (_childWindowType == ChildWindowType.DropDownList)
                        {
                            // Need to track the selection change via mouse over to
                            // raise focus changed event for the items. Monitoring
                            // item change in setters does not guarantee that focus
                            // is properly announced.
                            object before = _owner.SelectedItem;
                            DefWndProc(ref m);
                            object after = _owner.SelectedItem;
                            if (before != after)
                            {
                                (_owner.AccessibilityObject as ComboBoxAccessibleObject).SetComboBoxItemFocus();
                            }
                        }
                        else
                        {
                            _owner.ChildWndProc(ref m);
                        }
                        break;
                    default:
                        if (_childWindowType == ChildWindowType.DropDownList)
                        {
                            DefWndProc(ref m); // Drop Down window should behave by its own.
                        }
                        else
                        {
                            _owner.ChildWndProc(ref m);
                        }
                        break;
                }
            }

            private ChildAccessibleObject GetChildAccessibleObject(ChildWindowType childWindowType)
            {
                if (childWindowType == ChildWindowType.Edit)
                {
                    return _owner.ChildEditAccessibleObject;
                }
                else if (childWindowType == ChildWindowType.ListBox || childWindowType == ChildWindowType.DropDownList)
                {
                    return _owner.ChildListAccessibleObject;
                }

                return new ChildAccessibleObject(_owner, Handle);
            }

            private void WmGetObject(ref Message m)
            {
                if (m.LParam == (IntPtr)NativeMethods.UiaRootObjectId)
                {
                    AccessibleObject uiaProvider = GetChildAccessibleObject(_childWindowType);

                    // If the requested object identifier is UiaRootObjectId,
                    // we should return an UI Automation provider using the UiaReturnRawElementProvider function.
                    InternalAccessibleObject internalAccessibleObject = new InternalAccessibleObject(uiaProvider);
                    m.Result = UiaCore.UiaReturnRawElementProvider(
                        new HandleRef(this, Handle),
                        m.WParam,
                        m.LParam,
                        internalAccessibleObject);

                    return;
                }

                // See "How to Handle WM_GETOBJECT" in MSDN
                //
                if (unchecked((int)(long)m.LParam) == OBJID.CLIENT)
                {
                    // Get the IAccessible GUID
                    //
                    Guid IID_IAccessible = new Guid(NativeMethods.uuid_IAccessible);

                    // Get an Lresult for the accessibility Object for this control
                    //
                    IntPtr punkAcc;
                    try
                    {
                        AccessibleObject wfAccessibleObject = null;
                        UiaCore.IAccessibleInternal iacc = null;

                        if (_accessibilityObject is null)
                        {
                            wfAccessibleObject = GetChildAccessibleObject(_childWindowType);
                            _accessibilityObject = new InternalAccessibleObject(wfAccessibleObject);
                        }
                        iacc = (UiaCore.IAccessibleInternal)_accessibilityObject;

                        // Obtain the Lresult
                        //
                        punkAcc = Marshal.GetIUnknownForObject(iacc);

                        try
                        {
                            m.Result = Oleacc.LresultFromObject(ref IID_IAccessible, m.WParam, new HandleRef(this, punkAcc));
                        }
                        finally
                        {
                            Marshal.Release(punkAcc);
                        }
                    }
                    catch (Exception e)
                    {
                        throw new InvalidOperationException(SR.RichControlLresult, e);
                    }
                }
                else
                {  // m.lparam != OBJID_CLIENT, so do default message processing
                    DefWndProc(ref m);
                }
            }
        }
    }
}
