// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

            internal override object? GetPropertyValue(UiaCore.UIA propertyID)
            {
                switch (propertyID)
                {
                    case UiaCore.UIA.AccessKeyPropertyId:
                        return KeyboardShortcut;
                    default:
                        return base.GetPropertyValue(propertyID);
                }
            }

            /// <summary>
            ///  Indicates whether specified pattern is supported.
            /// </summary>
            /// <param name="patternId">The pattern ID.</param>
            /// <returns>True if specified </returns>
            internal override bool IsPatternSupported(UiaCore.UIA patternId)
            {
                if (patternId == UiaCore.UIA.InvokePatternId)
                {
                    return true;
                }

                return base.IsPatternSupported(patternId);
            }

            /// <summary>
            ///  Gets or sets the accessible name.
            /// </summary>
            public override string? Name
            {
                get
                {
                    if (_owningGridViewListBox != null)
                    {
                        return _owningItem.ToString();
                    }

                    return base.Name;
                }
            }

            /// <summary>
            ///  Gets the runtime ID.
            /// </summary>
            internal override int[] RuntimeId
            {
                get
                {
                    var runtimeId = new int[3];
                    runtimeId[0] = RuntimeIDFirstItem;
                    runtimeId[1] = (int)(long)_owningGridViewListBox.Handle;
                    runtimeId[2] = _owningItem.GetHashCode();

                    return runtimeId;
                }
            }
        }
    }
}
