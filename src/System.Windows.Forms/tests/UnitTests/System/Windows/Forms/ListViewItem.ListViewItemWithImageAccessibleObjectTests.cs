// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using static System.Windows.Forms.ListViewItem;
using static Interop;

namespace System.Windows.Forms.Tests
{
    public class ListViewItem_ListViewItemWithImageAccessibleObjectTests
    {
        [WinFormsTheory]
        [MemberData(nameof(GetViewTheoryData))]
        public void ListViewItemListAccessibleObject_FragmentNavigate_Children_ReturnsNull_WithoutImage(View view)
        {
            using ListView control = new();
            control.View = view;
            control.Items.AddRange(new ListViewItem[] { new() });

            AccessibleObject listViewItemAccessibleObject = control.Items[0].AccessibilityObject;

            Assert.Null(listViewItemAccessibleObject.FragmentNavigate(UiaCore.NavigateDirection.FirstChild));
            Assert.Null(listViewItemAccessibleObject.FragmentNavigate(UiaCore.NavigateDirection.LastChild));
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(GetViewTheoryData))]
        public void ListViewItemListAccessibleObject_FragmentNavigate_Children_IsExpected_WithImage(View view)
        {
            using ImageList imageCollection = new();
            imageCollection.Images.Add(Form.DefaultIcon);

            ListViewItem listViewItem = new("Test", 0);
            using ListView control = new()
            {
                View = view,
                SmallImageList = imageCollection
            };
            control.Items.Add(listViewItem);

            AccessibleObject listViewItemAccessibleObject = control.Items[0].AccessibilityObject;
            var firstChild = listViewItemAccessibleObject.FragmentNavigate(UiaCore.NavigateDirection.FirstChild);
            var lastChild = listViewItemAccessibleObject.FragmentNavigate(UiaCore.NavigateDirection.LastChild);

            Assert.IsType<ListViewItemImageAccessibleObject>(firstChild);
            Assert.IsType<ListViewItemImageAccessibleObject>(lastChild);
            Assert.Same(firstChild, lastChild);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(GetViewTheoryData))]
        public void ListViewItemListAccessibleObject_GetChild_ReturnsNull_WithoutImage(View view)
        {
            using ListView control = new();
            control.View = view;
            control.Items.AddRange(new ListViewItem[] { new() });

            AccessibleObject listViewItemAccessibleObject = control.Items[0].AccessibilityObject;

            Assert.Null(listViewItemAccessibleObject.GetChild(0));
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(GetViewTheoryData))]
        public void ListViewItemListAccessibleObject_GetChild_IsExpected_WithImage(View view)
        {
            using ImageList imageCollection = new();
            imageCollection.Images.Add(Form.DefaultIcon);

            ListViewItem listViewItem = new("Test", 0);
            using ListView control = new()
            {
                View = view,
                SmallImageList = imageCollection
            };
            control.Items.Add(listViewItem);

            AccessibleObject listViewItemAccessibleObject = control.Items[0].AccessibilityObject;

            Assert.IsType<ListViewItemImageAccessibleObject>(listViewItemAccessibleObject.GetChild(0));
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(GetViewTheoryData))]
        public void ListViewItemListAccessibleObject_GetChildCount_WithoutImage(View view)
        {
            using ListView control = new();
            control.View = view;
            control.Items.AddRange(new ListViewItem[] { new() });
            control.CreateControl();

            AccessibleObject listViewItemAccessibleObject = control.Items[0].AccessibilityObject;

            Assert.Equal(AccessibleObject.InvalidIndex, listViewItemAccessibleObject.GetChildCount());
            Assert.True(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(GetViewTheoryData))]
        public void ListViewItemListAccessibleObject_GetChildCount_WithImage(View view)
        {
            using ImageList imageCollection = new();
            imageCollection.Images.Add(Form.DefaultIcon);

            ListViewItem listViewItem = new("Test", 0);
            using ListView control = new()
            {
                View = view,
                SmallImageList = imageCollection
            };
            control.Items.Add(listViewItem);
            control.CreateControl();

            AccessibleObject listViewItemAccessibleObject = control.Items[0].AccessibilityObject;

            Assert.Equal(1, listViewItemAccessibleObject.GetChildCount());
            Assert.True(control.IsHandleCreated);
        }

        public static TheoryData<View> GetViewTheoryData()
        {
            return new TheoryData<View>
            {
                View.LargeIcon,
                View.SmallIcon
            };
        }
    }
}
