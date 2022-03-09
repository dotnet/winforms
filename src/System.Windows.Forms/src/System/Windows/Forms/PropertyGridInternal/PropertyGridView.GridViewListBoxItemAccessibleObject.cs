// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using static System.Windows.Forms.ListBox;
using static Interop;

namespace System.Windows.Forms.PropertyGridInternal
{
    internal partial class PropertyGridView
    {
        private class GridViewListBoxItemAccessibleObject : ListBox.ListBoxItemAccessibleObject
        {
            private readonly GridViewListBox _owningGridViewListBox;
            private readonly ItemArray.Entry _owningItem;

            public GridViewListBoxItemAccessibleObject(GridViewListBox owningGridViewListBox, ItemArray.Entry owningItem)
                : base(owningGridViewListBox, owningItem, (ListBoxAccessibleObject)owningGridViewListBox.AccessibilityObject)
            {
                _owningGridViewListBox = owningGridViewListBox;
                _owningItem = owningItem;
            }

            internal override UiaCore.IRawElementProviderFragmentRoot FragmentRoot
                => _owningGridViewListBox.AccessibilityObject;

            /// <inheritdoc />
            internal override bool IsPatternSupported(UiaCore.UIA patternId)
                => patternId == UiaCore.UIA.InvokePatternId || base.IsPatternSupported(patternId);

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

            /// <inheritdoc />
            internal override int[] RuntimeId
                => new int[]
                {
                    RuntimeIDFirstItem,
                    PARAM.ToInt(_owningGridViewListBox.InternalHandle),
                    _owningItem.GetHashCode()
                };
        }
    }
}
