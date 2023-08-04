﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using static Interop;

namespace System.Windows.Forms.PropertyGridInternal;

internal partial class PropertyGridView
{
    /// <summary>
    ///  Represents the PropertyGridView ListBox accessibility object.
    /// </summary>
    private class GridViewListBoxAccessibleObject : ListBox.ListBoxAccessibleObject
    {
        /// <summary>
        ///  Constructs the new instance of GridViewListBoxAccessibleObject.
        /// </summary>
        /// <param name="owningGridViewListBox">The owning GridViewListBox.</param>
        public GridViewListBoxAccessibleObject(GridViewListBox owningGridViewListBox) : base(owningGridViewListBox)
        {
            if (owningGridViewListBox.OwningPropertyGridView is not PropertyGridView owningPropertyGridView)
            {
                throw new ArgumentException(null, nameof(owningGridViewListBox));
            }
        }

        internal override UiaCore.IRawElementProviderFragment? FragmentNavigate(UiaCore.NavigateDirection direction)
        {
            if (!this.TryGetOwnerAs(out GridViewListBox? owner)
                || !owner.OwningPropertyGridView.DropDownVisible
                || owner.OwningPropertyGridView.SelectedGridEntry is null
                || owner.OwningPropertyGridView.DropDownControlHolder.Component != owner
                // Created is set to false in WM_DESTROY, but the window Handle is released on NCDESTROY, which comes after DESTROY.
                // But between these calls, AccessibleObject can be recreated and might cause memory leaks.
                || !owner.OwningPropertyGridView.OwnerGrid.Created)
            {
                return null;
            }

            return direction switch
            {
                UiaCore.NavigateDirection.Parent => owner.OwningPropertyGridView.DropDownControlHolder.AccessibilityObject,
                _ => base.FragmentNavigate(direction)
            };
        }

        public override string? Name => base.Name ?? SR.PropertyGridEntryValuesListDefaultAccessibleName;

        internal override UiaCore.IRawElementProviderFragmentRoot FragmentRoot
            => this.TryGetOwnerAs(out GridViewListBox? owner)
                ? owner.OwningPropertyGridView.AccessibilityObject
                : UiaCore.StubFragmentRoot.Instance;
    }
}
