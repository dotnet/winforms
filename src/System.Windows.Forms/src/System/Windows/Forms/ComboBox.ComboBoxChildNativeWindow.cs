// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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

                            // Call the focus event for the new selected item accessible object provided by ComboBoxAccessibleObject.
                            // If the owning ComboBox has a custom accessible object,
                            // it should override the logic and implement setting an item focus by itself.
                            if (before != after && _owner.IsAccessibilityObjectCreated && _owner.AccessibilityObject is ComboBoxAccessibleObject comboBoxAccessibleObject)
                            {
                                comboBoxAccessibleObject.SetComboBoxItemFocus();
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
                => childWindowType switch
                {
                    ChildWindowType.Edit => _owner.ChildEditAccessibleObject,
                    ChildWindowType.ListBox or ChildWindowType.DropDownList => _owner.ChildListAccessibleObject,
                    _ => throw new ArgumentOutOfRangeException(nameof(childWindowType))
                };

            private void WmGetObject(ref Message m)
            {
                if (m.LParamInternal == NativeMethods.UiaRootObjectId)
                {
                    // If the requested object identifier is UiaRootObjectId,
                    // we should return an UI Automation provider using the UiaReturnRawElementProvider function.
                    m.ResultInternal = UiaCore.UiaReturnRawElementProvider(
                        this,
                        m.WParamInternal,
                        m.LParamInternal,
                        GetChildAccessibleObject(_childWindowType));

                    return;
                }

                // See "How to Handle WM_GETOBJECT" in MSDN
                if ((int)m.LParamInternal == OBJID.CLIENT)
                {
                    // Get an Lresult for the accessibility Object for this control
                    try
                    {
                        // Obtain the Lresult
                        IntPtr pUnknown = Marshal.GetIUnknownForObject(GetChildAccessibleObject(_childWindowType));

                        try
                        {
                            m.ResultInternal = Oleacc.LresultFromObject(in IID.IAccessible, m.WParamInternal, new HandleRef(this, pUnknown));
                        }
                        finally
                        {
                            Marshal.Release(pUnknown);
                        }
                    }
                    catch (Exception e)
                    {
                        throw new InvalidOperationException(SR.RichControlLresult, e);
                    }
                }
                else
                {
                    // m.lparam != OBJID_CLIENT, so do default message processing
                    DefWndProc(ref m);
                }
            }
        }
    }
}
