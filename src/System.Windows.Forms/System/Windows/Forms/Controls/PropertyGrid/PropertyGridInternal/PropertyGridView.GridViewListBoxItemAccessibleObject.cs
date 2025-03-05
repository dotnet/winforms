// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.UI.Accessibility;
using static System.Windows.Forms.ListBox;

namespace System.Windows.Forms.PropertyGridInternal;

internal partial class PropertyGridView
{
    private class GridViewListBoxItemAccessibleObject : ListBoxItemAccessibleObject
    {
        private readonly GridViewListBox _owningGridViewListBox;
        private readonly ItemArray.Entry _owningItem;

        public GridViewListBoxItemAccessibleObject(GridViewListBox owningGridViewListBox, ItemArray.Entry owningItem)
            : base(owningGridViewListBox, owningItem, (ListBoxAccessibleObject)owningGridViewListBox.AccessibilityObject)
        {
            _owningGridViewListBox = owningGridViewListBox;
            _owningItem = owningItem;
        }

        internal override IRawElementProviderFragmentRoot.Interface FragmentRoot => _owningGridViewListBox.AccessibilityObject;

        /// <inheritdoc />
        internal override bool IsPatternSupported(UIA_PATTERN_ID patternId)
            => patternId == UIA_PATTERN_ID.UIA_InvokePatternId || base.IsPatternSupported(patternId);

        /// <inheritdoc />
        public override string? Name
        {
            get
            {
                if (_owningGridViewListBox is not null)
                {
                    return _owningItem.ToString();
                }

                return base.Name;
            }
        }

        private protected override bool IsInternal => true;

        internal override int[] RuntimeId =>
        [
            RuntimeIDFirstItem,
            (int)_owningGridViewListBox.InternalHandle,
            _owningItem.GetHashCode()
        ];
    }
}
