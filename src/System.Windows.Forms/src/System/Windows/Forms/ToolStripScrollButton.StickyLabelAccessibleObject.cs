﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.UI.Accessibility;
using static Interop;

namespace System.Windows.Forms;

internal partial class ToolStripScrollButton
{
    internal class StickyLabelAccessibleObject : Label.LabelAccessibleObject
    {
        private StickyLabel _owner;

        public StickyLabelAccessibleObject(StickyLabel owner) : base(owner)
        {
            _owner = owner;
        }

        internal override UiaCore.IRawElementProviderFragment? FragmentNavigate(UiaCore.NavigateDirection direction)
        {
            if (_owner.OwnerScrollButton?.Parent is not ToolStripDropDownMenu toolStripDropDownMenu)
            {
                return null;
            }

            return direction switch
            {
                UiaCore.NavigateDirection.Parent => toolStripDropDownMenu.AccessibilityObject,
                UiaCore.NavigateDirection.NextSibling
                    => _owner.UpDirection && toolStripDropDownMenu.Items.Count > 0
                        ? toolStripDropDownMenu.Items[0].AccessibilityObject
                        : null,
                UiaCore.NavigateDirection.PreviousSibling
                    => !_owner.UpDirection && toolStripDropDownMenu.Items.Count > 0
                        ? toolStripDropDownMenu.Items[^1].AccessibilityObject
                        : null,
                _ => null
            };
        }

        internal override UiaCore.IRawElementProviderFragmentRoot? FragmentRoot
            => _owner.OwnerScrollButton?.Owner?.AccessibilityObject;

        public override string? Name => _owner.UpDirection
            ? SR.ToolStripScrollButtonUpAccessibleName
            : SR.ToolStripScrollButtonDownAccessibleName;

        public override string? DefaultAction => SR.AccessibleActionPress;

        internal override object? GetPropertyValue(UIA_PROPERTY_ID propertyID) => propertyID switch
        {
            UIA_PROPERTY_ID.UIA_ControlTypePropertyId => UIA_CONTROLTYPE_ID.UIA_ButtonControlTypeId,
            _ => base.GetPropertyValue(propertyID)
        };
    }
}
