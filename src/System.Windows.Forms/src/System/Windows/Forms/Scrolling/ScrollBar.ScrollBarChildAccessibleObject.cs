// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using Windows.Win32.System.Variant;
using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms;

public partial class ScrollBar
{
    internal class ScrollBarChildAccessibleObject : AccessibleObject
    {
        internal ScrollBar OwningScrollBar { get; private set; }

        public ScrollBarChildAccessibleObject(ScrollBar owningScrollBar)
        {
            OwningScrollBar = owningScrollBar;
        }

        public override Rectangle Bounds
        {
            get
            {
                if (!OwningScrollBar.IsHandleCreated || !IsDisplayed)
                {
                    return Rectangle.Empty;
                }

                // The "GetChildId" method returns to the id of the ScrollBar element,
                // which allows to use the native "accLocation" method to get the "Bounds" property

                return ParentInternal.SystemIAccessible.TryGetLocation(GetChildId());
            }
        }

        public override string? DefaultAction => GetDefaultActionInternal().ToNullableStringAndFree();

        private protected override bool IsInternal => true;

        internal override BSTR GetDefaultActionInternal() =>
            ParentInternal.SystemIAccessible.TryGetDefaultAction(GetChildId());

        public override string? Description => GetDescriptionInternal().ToNullableStringAndFree();

        internal override unsafe BSTR GetDescriptionInternal() =>
            ParentInternal.SystemIAccessible.TryGetDescription(GetChildId());

        public override string? Name => GetNameInternal().ToNullableStringAndFree();

        internal override BSTR GetNameInternal() => ParentInternal.SystemIAccessible.TryGetName(GetChildId());

        public override AccessibleRole Role
            => ParentInternal.SystemIAccessible.TryGetRole(GetChildId());

        public override AccessibleStates State
            => ParentInternal.SystemIAccessible.TryGetState(GetChildId());

        internal override IRawElementProviderFragmentRoot.Interface FragmentRoot => ParentInternal;

        internal virtual bool IsDisplayed => OwningScrollBar.Visible;

        internal ScrollBarAccessibleObject ParentInternal => (ScrollBarAccessibleObject)OwningScrollBar.AccessibilityObject;

        internal override int[] RuntimeId =>
        [
            RuntimeIDFirstItem,
            (int)OwningScrollBar.InternalHandle,
            GetChildId()
        ];

        internal override IRawElementProviderFragment.Interface? FragmentNavigate(NavigateDirection direction)
            => direction switch
            {
                NavigateDirection.NavigateDirection_Parent => ParentInternal,
                _ => base.FragmentNavigate(direction)
            };

        internal override VARIANT GetPropertyValue(UIA_PROPERTY_ID propertyID)
            => propertyID switch
            {
                UIA_PROPERTY_ID.UIA_ControlTypePropertyId => (VARIANT)(int)UIA_CONTROLTYPE_ID.UIA_ButtonControlTypeId,
                UIA_PROPERTY_ID.UIA_HasKeyboardFocusPropertyId => VARIANT.False,
                UIA_PROPERTY_ID.UIA_IsEnabledPropertyId => (VARIANT)OwningScrollBar.Enabled,
                UIA_PROPERTY_ID.UIA_IsKeyboardFocusablePropertyId => VARIANT.False,
                _ => base.GetPropertyValue(propertyID)
            };

        internal override void Invoke()
        {
            if (OwningScrollBar.IsHandleCreated && IsDisplayed)
            {
                // The "GetChildId" method returns to the id of the ScrollBar element,
                // which allows to use the native "accDoDefaultAction" method when the "Invoke" method is called
                ParentInternal.SystemIAccessible.TryDoDefaultAction(GetChildId());
            }
        }

        internal override bool IsPatternSupported(UIA_PATTERN_ID patternId)
            => patternId switch
            {
                UIA_PATTERN_ID.UIA_LegacyIAccessiblePatternId => true,
                _ => base.IsPatternSupported(patternId)
            };
    }
}
