// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using Xunit;
using static System.Windows.Forms.ListViewItem;
using static Interop;

namespace System.Windows.Forms.Tests
{
    public class ListViewItem_ListViewItemListAccessibleObjectTests
    {
        [WinFormsFact]
        public void ListViewItemListAccessibleObject_Ctor_OwnerListViewItemCannotBeNull()
        {
            Assert.Throws<ArgumentNullException>(() => new ListViewItemListAccessibleObject(null));
        }

        [WinFormsFact]
        public void ListViewItemListAccessibleObject_Ctor_OwnerListViewCannotBeNull()
        {
            Assert.Throws<InvalidOperationException>(() => new ListViewItemListAccessibleObject(new ListViewItem()));
        }

        [WinFormsFact]
        public void ListViewItemListAccessibleObject_Bounds_IsEmptyRectangle_IfOwningControlNotCreated()
        {
            using ListView control = new();
            control.View = View.List;
            control.Items.Add(new ListViewItem());

            Assert.Equal(Rectangle.Empty, control.Items[0].AccessibilityObject.Bounds);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void ListViewItemListAccessibleObject_FragmentNavigate_Parent()
        {
            using ListView control = new();
            control.View = View.List;
            control.Items.Add(new ListViewItem());
            AccessibleObject accessibleObject1 = control.Items[0].AccessibilityObject;

            Assert.Equal(control.AccessibilityObject, accessibleObject1.FragmentNavigate(UiaCore.NavigateDirection.Parent));
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void ListViewItemListAccessibleObject_FragmentNavigate_PreviousSibling()
        {
            using ListView control = new();
            control.View = View.List;
            control.Items.AddRange(new ListViewItem[] { new(), new(), new() });
            control.CreateControl();

            AccessibleObject accessibleObject1 = control.Items[0].AccessibilityObject;
            AccessibleObject accessibleObject2 = control.Items[1].AccessibilityObject;
            AccessibleObject accessibleObject3 = control.Items[2].AccessibilityObject;

            Assert.Null(accessibleObject1.FragmentNavigate(UiaCore.NavigateDirection.PreviousSibling));
            Assert.Equal(accessibleObject1, accessibleObject2.FragmentNavigate(UiaCore.NavigateDirection.PreviousSibling));
            Assert.Equal(accessibleObject2, accessibleObject3.FragmentNavigate(UiaCore.NavigateDirection.PreviousSibling));

            Assert.True(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void ListViewItemListAccessibleObject_FragmentNavigate_NextSibling()
        {
            using ListView control = new();
            control.View = View.List;
            control.Items.AddRange(new ListViewItem[] { new(), new(), new() });
            control.CreateControl();

            AccessibleObject accessibleObject1 = control.Items[0].AccessibilityObject;
            AccessibleObject accessibleObject2 = control.Items[1].AccessibilityObject;
            AccessibleObject accessibleObject3 = control.Items[2].AccessibilityObject;

            Assert.Equal(accessibleObject2, accessibleObject1.FragmentNavigate(UiaCore.NavigateDirection.NextSibling));
            Assert.Equal(accessibleObject3, accessibleObject2.FragmentNavigate(UiaCore.NavigateDirection.NextSibling));
            Assert.Null(accessibleObject3.FragmentNavigate(UiaCore.NavigateDirection.NextSibling));

            Assert.True(control.IsHandleCreated);
        }
    }
}
