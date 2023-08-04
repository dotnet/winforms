﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using static Interop;

namespace System.Windows.Forms;

public partial class ToolStripMenuItem
{
    /// <summary>
    ///  An implementation of AccessibleChild for use with ToolStripItems
    /// </summary>
    internal class ToolStripMenuItemAccessibleObject : ToolStripDropDownItemAccessibleObject
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

        internal override object? GetPropertyValue(UiaCore.UIA propertyID) =>
            propertyID switch
            {
                UiaCore.UIA.AcceleratorKeyPropertyId => _owningToolStripMenuItem.GetShortcutText(),
                UiaCore.UIA.PositionInSetPropertyId => GetPositionInSet(),
                UiaCore.UIA.SizeOfSetPropertyId => GetSizeOfSet(),
                _ => base.GetPropertyValue(propertyID)
            };

        /// <summary>
        ///  Gets <see cref="UiaCore.UIA.PositionInSetPropertyId"/> property value.
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
        ///  Gets <see cref="UiaCore.UIA.SizeOfSetPropertyId"/> property value.
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

        public override string DefaultAction
        {
            get
            {
                return Owner.AccessibleDefaultActionDescription
                    ?? (_owningToolStripMenuItem.CheckOnClick ? SR.AccessibleActionCheck : base.DefaultAction);
            }
        }

        internal override bool IsPatternSupported(UiaCore.UIA patternId) =>
            patternId switch
            {
                UiaCore.UIA.TogglePatternId => _owningToolStripMenuItem.CheckOnClick || _owningToolStripMenuItem.Checked,
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

        internal override UiaCore.ToggleState ToggleState =>
            _owningToolStripMenuItem.CheckState switch
            {
                CheckState.Checked => UiaCore.ToggleState.On,
                CheckState.Unchecked => UiaCore.ToggleState.Off,
                _ => UiaCore.ToggleState.Indeterminate
            };

        #endregion
    }
}
