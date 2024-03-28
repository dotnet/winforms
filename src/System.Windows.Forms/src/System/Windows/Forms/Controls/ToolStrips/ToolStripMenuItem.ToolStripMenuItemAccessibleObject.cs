// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.System.Variant;
using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms;

public partial class ToolStripMenuItem
{
    /// <summary>
    ///  An implementation of AccessibleChild for use with ToolStripItems
    /// </summary>
    internal sealed class ToolStripMenuItemAccessibleObject : ToolStripDropDownItemAccessibleObject
    {
        private readonly ToolStripMenuItem _owningToolStripMenuItem;

        public ToolStripMenuItemAccessibleObject(ToolStripMenuItem ownerItem) : base(ownerItem)
        {
            _owningToolStripMenuItem = ownerItem;
        }

        public override AccessibleStates State
        {
            get
            {
                if (_owningToolStripMenuItem.Enabled)
                {
                    AccessibleStates state = base.State;

                    if ((state & AccessibleStates.Pressed) == AccessibleStates.Pressed)
                    {
                        // for some reason menu items are never "pressed".
                        state &= ~AccessibleStates.Pressed;
                    }

                    if (_owningToolStripMenuItem.Checked)
                    {
                        state |= AccessibleStates.Checked;
                    }

                    return state;
                }

                return base.State;
            }
        }

        internal override VARIANT GetPropertyValue(UIA_PROPERTY_ID propertyID) =>
            propertyID switch
            {
                UIA_PROPERTY_ID.UIA_AcceleratorKeyPropertyId => _owningToolStripMenuItem.GetShortcutText() is { } shortcutText ? (VARIANT)shortcutText : VARIANT.Empty,
                UIA_PROPERTY_ID.UIA_PositionInSetPropertyId => GetPositionInSet() is { } position ? (VARIANT)position : VARIANT.Empty,
                UIA_PROPERTY_ID.UIA_SizeOfSetPropertyId => GetSizeOfSet() is { } size ? (VARIANT)size : VARIANT.Empty,
                _ => base.GetPropertyValue(propertyID)
            };

        /// <summary>
        ///  Gets <see cref="UIA_PROPERTY_ID.UIA_PositionInSetPropertyId"/> property value.
        /// </summary>
        private int? GetPositionInSet()
        {
            ToolStripItemCollection? displayedItems = _owningToolStripMenuItem.ParentInternal?.DisplayedItems;

            if (displayedItems is null)
            {
                return null;
            }

            int index = displayedItems.IndexOf(_owningToolStripMenuItem);

            if (index < 0)
            {
                return null;
            }

            foreach (ToolStripItem item in displayedItems)
            {
                if (item == _owningToolStripMenuItem)
                {
                    break;
                }

                if (item is ToolStripSeparator)
                {
                    index--;
                }
            }

            return index + 1; // UIA_PositionInSet is 1-based
        }

        /// <summary>
        ///  Gets <see cref="UIA_PROPERTY_ID.UIA_SizeOfSetPropertyId"/> property value.
        /// </summary>
        private int? GetSizeOfSet()
        {
            ToolStripItemCollection? displayedItems = _owningToolStripMenuItem.ParentInternal?.DisplayedItems;

            if (displayedItems is null)
            {
                return null;
            }

            int sizeOfSet = displayedItems.Count;

            foreach (ToolStripItem item in displayedItems)
            {
                if (item is ToolStripSeparator)
                {
                    sizeOfSet--;
                }
            }

            return sizeOfSet;
        }

        public override string DefaultAction =>
            Owner.AccessibleDefaultActionDescription
                ?? (_owningToolStripMenuItem.CheckOnClick ? SR.AccessibleActionCheck : base.DefaultAction);

        private protected override bool IsInternal => true;

        internal override bool IsPatternSupported(UIA_PATTERN_ID patternId) =>
            patternId switch
            {
                UIA_PATTERN_ID.UIA_TogglePatternId => _owningToolStripMenuItem.CheckOnClick || _owningToolStripMenuItem.Checked,
                _ => base.IsPatternSupported(patternId)
            };

        #region Toggle Pattern

        internal override void Toggle()
        {
            if (_owningToolStripMenuItem.CheckOnClick)
            {
                _owningToolStripMenuItem.Checked = !_owningToolStripMenuItem.Checked;
            }
        }

        internal override ToggleState ToggleState =>
            _owningToolStripMenuItem.CheckState switch
            {
                CheckState.Checked => ToggleState.ToggleState_On,
                CheckState.Unchecked => ToggleState.ToggleState_Off,
                _ => ToggleState.ToggleState_Indeterminate
            };

        #endregion
    }
}
