// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.System.Variant;
using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms;

public partial class ErrorProvider
{
    internal partial class ErrorWindow
    {
        private class ErrorWindowAccessibleObject : AccessibleObject
        {
            private readonly ErrorWindow _owner;

            public ErrorWindowAccessibleObject(ErrorWindow owner)
            {
                _owner = owner;
            }

            internal override IRawElementProviderFragment.Interface? ElementProviderFromPoint(double x, double y)
            {
                AccessibleObject? element = HitTest((int)x, (int)y);

                if (element is not null)
                {
                    return element;
                }

                return base.ElementProviderFromPoint(x, y);
            }

            internal override IRawElementProviderFragment.Interface? FragmentNavigate(NavigateDirection direction)
                => direction switch
                {
                    NavigateDirection.NavigateDirection_FirstChild => GetChild(0),
                    NavigateDirection.NavigateDirection_LastChild => GetChild(GetChildCount() - 1),
                    _ => base.FragmentNavigate(direction),
                };

            internal override IRawElementProviderFragmentRoot.Interface FragmentRoot => this;

            public override AccessibleObject? GetChild(int index)
            {
                if (index >= 0 && index <= GetChildCount() - 1)
                {
                    return _owner.ControlItems[index].AccessibilityObject;
                }

                return base.GetChild(index);
            }

            private protected override bool IsInternal => true;

            public override int GetChildCount() => _owner.ControlItems.Count;

            internal override VARIANT GetPropertyValue(UIA_PROPERTY_ID propertyID) =>
                propertyID switch
                {
                    UIA_PROPERTY_ID.UIA_ControlTypePropertyId => (VARIANT)(int)UIA_CONTROLTYPE_ID.UIA_GroupControlTypeId,
                    UIA_PROPERTY_ID.UIA_NativeWindowHandlePropertyId => UIAHelper.WindowHandleToVariant(_owner.Handle),
                    _ => base.GetPropertyValue(propertyID)
                };

            public override AccessibleObject? HitTest(int x, int y)
            {
                foreach (ControlItem control in _owner.ControlItems)
                {
                    if (control.AccessibilityObject.Bounds.Contains(x, y))
                    {
                        return control.AccessibilityObject;
                    }
                }

                return null;
            }

            internal override unsafe IRawElementProviderSimple* HostRawElementProvider
            {
                get
                {
                    PInvoke.UiaHostProviderFromHwnd(new HandleRef<HWND>(this, _owner.HWND), out IRawElementProviderSimple* provider);
                    return provider;
                }
            }

            internal override bool IsIAccessibleExSupported() => true;

            internal override bool IsPatternSupported(UIA_PATTERN_ID patternId)
            {
                if (patternId == UIA_PATTERN_ID.UIA_LegacyIAccessiblePatternId)
                {
                    return true;
                }

                return base.IsPatternSupported(patternId);
            }

            internal override bool IsReadOnly => true;

            [AllowNull]
            public override string Name
            {
                get
                {
                    string? name = base.Name;
                    return string.IsNullOrEmpty(name) ? SR.ErrorProviderDefaultAccessibleName : name;
                }
            }

            internal override bool CanGetNameInternal => false;

            public override AccessibleRole Role => AccessibleRole.Grouping;

            internal override int[] RuntimeId =>
            [
                RuntimeIDFirstItem,
                (int)_owner.Handle,
                _owner.GetHashCode()
            ];

            public override AccessibleStates State => AccessibleStates.ReadOnly;
        }
    }
}
