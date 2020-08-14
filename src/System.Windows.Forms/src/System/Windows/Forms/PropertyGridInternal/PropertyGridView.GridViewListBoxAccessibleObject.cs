// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Interop;

namespace System.Windows.Forms.PropertyGridInternal
{
    internal partial class PropertyGridView
    {
        /// <summary>
        ///  Represents the PropertyGridView ListBox accessibility object.
        /// </summary>
        private class GridViewListBoxAccessibleObject : ListBox.ListBoxAccessibleObject
        {
            private readonly PropertyGridView _owningPropertyGridView;

            /// <summary>
            ///  Constructs the new instance of GridViewListBoxAccessibleObject.
            /// </summary>
            /// <param name="owningGridViewListBox">The owning GridViewListBox.</param>
            public GridViewListBoxAccessibleObject(GridViewListBox owningGridViewListBox) : base(owningGridViewListBox)
            {
                if (owningGridViewListBox.OwningPropertyGridView is null)
                {
                    throw new ArgumentNullException(nameof(owningGridViewListBox.OwningPropertyGridView));
                }

                _owningPropertyGridView = owningGridViewListBox.OwningPropertyGridView;
            }

            /// <summary>
            ///  Request to return the element in the specified direction.
            /// </summary>
            /// <param name="direction">Indicates the direction in which to navigate.</param>
            /// <returns>Returns the element in the specified direction.</returns>
            internal override UiaCore.IRawElementProviderFragment? FragmentNavigate(UiaCore.NavigateDirection direction)
            {
                if (direction == UiaCore.NavigateDirection.Parent && _owningPropertyGridView.SelectedGridEntry != null)
                {
                    return _owningPropertyGridView.SelectedGridEntry.AccessibilityObject;
                }

                if (direction == UiaCore.NavigateDirection.NextSibling)
                {
                    return _owningPropertyGridView.Edit.AccessibilityObject;
                }

                return base.FragmentNavigate(direction);
            }

            public override string? Name
            {
                get => base.Name ?? SR.PropertyGridEntryValuesListDefaultAccessibleName;
            }

            /// <summary>
            ///  Return the element that is the root node of this fragment of UI.
            /// </summary>
            internal override UiaCore.IRawElementProviderFragmentRoot FragmentRoot
                => _owningPropertyGridView.AccessibilityObject;
        }
    }
}
