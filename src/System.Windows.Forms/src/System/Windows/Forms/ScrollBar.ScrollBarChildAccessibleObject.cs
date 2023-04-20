// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using static Interop;

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

        public override string? DefaultAction
            => ParentInternal.SystemIAccessible.TryGetDefaultAction(GetChildId());

        public override string? Description
            => ParentInternal.SystemIAccessible.TryGetDescription(GetChildId());

        public override string? Name
            => ParentInternal.SystemIAccessible.TryGetName(GetChildId());

        public override AccessibleRole Role
            => ParentInternal.SystemIAccessible.TryGetRole(GetChildId());

        public override AccessibleStates State
            => ParentInternal.SystemIAccessible.TryGetState(GetChildId());

        internal override UiaCore.IRawElementProviderFragmentRoot FragmentRoot => ParentInternal;

        internal virtual bool IsDisplayed => OwningScrollBar.Visible;

        internal ScrollBarAccessibleObject ParentInternal => (ScrollBarAccessibleObject)OwningScrollBar.AccessibilityObject;

        internal override int[] RuntimeId
            => new int[]
            {
                RuntimeIDFirstItem,
                PARAM.ToInt(OwningScrollBar.InternalHandle),
                GetChildId()
            };

        internal override UiaCore.IRawElementProviderFragment? FragmentNavigate(UiaCore.NavigateDirection direction)
            => direction switch
            {
                UiaCore.NavigateDirection.Parent => ParentInternal,
                _ => base.FragmentNavigate(direction)
            };

        internal override object? GetPropertyValue(UiaCore.UIA propertyID)
            => propertyID switch
            {
                UiaCore.UIA.ControlTypePropertyId => UiaCore.UIA.ButtonControlTypeId,
                UiaCore.UIA.HasKeyboardFocusPropertyId => false,
                UiaCore.UIA.IsEnabledPropertyId => OwningScrollBar.Enabled,
                UiaCore.UIA.IsKeyboardFocusablePropertyId => false,
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

        internal override bool IsPatternSupported(UiaCore.UIA patternId)
            => patternId switch
            {
                UiaCore.UIA.LegacyIAccessiblePatternId => true,
                _ => base.IsPatternSupported(patternId)
            };
    }
}
