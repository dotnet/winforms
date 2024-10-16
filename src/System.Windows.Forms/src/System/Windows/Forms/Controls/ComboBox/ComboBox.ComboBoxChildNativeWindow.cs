// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms;

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

        protected override unsafe void WndProc(ref Message m)
        {
            switch ((uint)m.MsgInternal)
            {
                case PInvokeCore.WM_GETOBJECT:
                    WmGetObject(ref m);
                    return;
                case PInvokeCore.WM_MOUSEMOVE:
                    if (_childWindowType == ChildWindowType.DropDownList)
                    {
                        // Need to track the selection change via mouse over to
                        // raise focus changed event for the items. Monitoring
                        // item change in setters does not guarantee that focus
                        // is properly announced.
                        object? before = _owner.SelectedItem;
                        DefWndProc(ref m);
                        object? after = _owner.SelectedItem;

                        // Call the focus event for the new selected item accessible object provided by ComboBoxAccessibleObject.
                        // If the owning ComboBox has a custom accessible object,
                        // it should override the logic and implement setting an item focus by itself.
                        if (before != after
                            && _owner.IsAccessibilityObjectCreated
                            && _owner.AccessibilityObject is ComboBoxAccessibleObject comboBoxAccessibleObject)
                        {
                            comboBoxAccessibleObject.SetComboBoxItemFocus();
                        }
                    }
                    else
                    {
                        _owner.ChildWndProc(ref m);
                    }

                    break;
                case PInvokeCore.WM_DESTROY:
                    AccessibleObject? accessibilityObject = GetChildAccessibleObjectIfCreated();

                    if (accessibilityObject is not null)
                    {
                        if (!HWND.IsNull)
                        {
                            PInvoke.UiaReturnRawElementProvider(HWND, wParam: 0, lParam: 0, (IRawElementProviderSimple*)null);
                        }

                        PInvoke.UiaDisconnectProvider(accessibilityObject);

                        ClearChildAccessibleObject();
                    }

                    if (_childWindowType == ChildWindowType.DropDownList)
                    {
                        DefWndProc(ref m);
                    }
                    else
                    {
                        _owner.ChildWndProc(ref m);
                    }

                    break;
                default:
                    if (_childWindowType == ChildWindowType.DropDownList)
                    {
                        // Drop Down window should behave by its own.
                        DefWndProc(ref m);
                    }
                    else
                    {
                        _owner.ChildWndProc(ref m);
                    }

                    break;
            }
        }

        private ChildAccessibleObject GetChildAccessibleObject()
            => _childWindowType switch
            {
                ChildWindowType.Edit => _owner.ChildEditAccessibleObject,
                ChildWindowType.ListBox or ChildWindowType.DropDownList => _owner.ChildListAccessibleObject,
                _ => throw new ArgumentOutOfRangeException(nameof(_childWindowType))
            };

        private ChildAccessibleObject? GetChildAccessibleObjectIfCreated()
            => _childWindowType switch
            {
                ChildWindowType.Edit => _owner._childEditAccessibleObject,
                ChildWindowType.ListBox or ChildWindowType.DropDownList => _owner._childListAccessibleObject,
                _ => throw new ArgumentOutOfRangeException(nameof(_childWindowType))
            };

        private void ClearChildAccessibleObject()
        {
            if (_childWindowType == ChildWindowType.Edit)
            {
                _owner.ClearChildEditAccessibleObject();
            }
            else if (_childWindowType is ChildWindowType.ListBox or ChildWindowType.DropDownList)
            {
                _owner.ClearChildListAccessibleObject();
            }
        }

        private unsafe void WmGetObject(ref Message m)
        {
            if (m.LParamInternal != PInvoke.UiaRootObjectId && (int)m.LParamInternal != (int)OBJECT_IDENTIFIER.OBJID_CLIENT)
            {
                // Do default message processing.
                DefWndProc(ref m);
                return;
            }

            AccessibleObject accessibilityObject = GetChildAccessibleObject();

            if (m.LParamInternal == PInvoke.UiaRootObjectId)
            {
                // If the requested object identifier is UiaRootObjectId,
                // we should return an UI Automation provider using the UiaReturnRawElementProvider function.
                m.ResultInternal = PInvoke.UiaReturnRawElementProvider(
                    this,
                    m.WParamInternal,
                    m.LParamInternal,
                    accessibilityObject);

                return;
            }

            try
            {
                m.ResultInternal = accessibilityObject.GetLRESULT(m.WParamInternal);
            }
            catch (Exception e)
            {
                throw new InvalidOperationException(SR.RichControlLresult, e);
            }
        }
    }
}
