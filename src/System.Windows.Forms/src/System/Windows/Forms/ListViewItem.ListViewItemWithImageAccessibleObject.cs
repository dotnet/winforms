// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using static Interop;

namespace System.Windows.Forms
{
    public partial class ListViewItem
    {
        internal abstract class ListViewItemWithImageAccessibleObject : ListViewItemBaseAccessibleObject
        {
            private const int ImageAccessibleObjectIndex = 0;

            private ListViewItemImageAccessibleObject? _imageAccessibleObject;

            public ListViewItemWithImageAccessibleObject(ListViewItem owningItem) : base(owningItem)
            {
            }

            internal override int FirstSubItemIndex => HasImage ? 1 : 0;

            private ListViewItemImageAccessibleObject ImageAccessibleObject => _imageAccessibleObject ??= new(_owningItem);

            internal override UiaCore.IRawElementProviderFragment? FragmentNavigate(UiaCore.NavigateDirection direction)
            {
                switch (direction)
                {
                    case UiaCore.NavigateDirection.FirstChild:
                    case UiaCore.NavigateDirection.LastChild:
                        return GetChild(ImageAccessibleObjectIndex);
                }

                return base.FragmentNavigate(direction);
            }

            internal AccessibleObject GetAccessibleObject(Point point)
            {
                return HasImage && ImageAccessibleObject.GetImageRectangle().Contains(point)
                    ? ImageAccessibleObject
                    : this;
            }

            public override AccessibleObject? GetChild(int index)
            {
                if (_owningListView.View != View)
                {
                    throw new InvalidOperationException(string.Format(SR.ListViewItemAccessibilityObjectInvalidViewException, View.ToString()));
                }

                return index == ImageAccessibleObjectIndex && HasImage
                    ? ImageAccessibleObject
                    : (AccessibleObject?)null;
            }

            public override int GetChildCount()
            {
                if (_owningListView.View != View)
                {
                    throw new InvalidOperationException(string.Format(SR.ListViewItemAccessibilityObjectInvalidViewException, View.ToString()));
                }

                return !_owningListView.IsHandleCreated || !HasImage
                    ? InvalidIndex
                    : 1;
            }

            internal override int GetChildIndex(AccessibleObject? child)
                => child is ListViewItemImageAccessibleObject
                    ? ImageAccessibleObjectIndex
                    : InvalidIndex;

            internal override Rectangle GetSubItemBounds(int accessibleChildIndex)
                => _owningListView.IsHandleCreated && accessibleChildIndex == ImageAccessibleObjectIndex && HasImage
                    ? ImageAccessibleObject.Bounds
                    : Rectangle.Empty;

            /// <devdoc>
            ///  Caller should ensure that the current OS is Windows 8 or greater.
            /// </devdoc>
            internal override void ReleaseChildUiaProviders()
            {
                base.ReleaseChildUiaProviders();

                UiaCore.UiaDisconnectProvider(_imageAccessibleObject);
            }
        }
    }
}
