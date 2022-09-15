// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using Xunit;
using Xunit.Abstractions;
using static Interop;

namespace System.Windows.Forms.UITests
{
    public class ListViewGroup_ListViewGroupAccessibleObjectTests : ControlTestBase
    {
        public ListViewGroup_ListViewGroupAccessibleObjectTests(ITestOutputHelper testOutputHelper)
            : base(testOutputHelper)
        {
        }

        [WinFormsFact]
        public void ListViewGroupAccessibleObject_Bounds_ReturnsCorrectValue()
        {
            // Control.CheckForIllegalCrossThreadCalls is process-wide and may affect other unit tests that run in parallel. This test
            // method has been moved to UITests so that it can be executed serially, and without the risk of affecting other tests.
            Control.CheckForIllegalCrossThreadCalls = true;
            using Form form = new Form();

            using ListView list = new ListView();
            ListViewGroup listGroup = new ListViewGroup("Group1");
            ListViewItem listItem1 = new ListViewItem("Item1");
            ListViewItem listItem2 = new ListViewItem("Item2");
            list.Groups.Add(listGroup);
            listItem1.Group = listGroup;
            listItem2.Group = listGroup;
            list.Items.Add(listItem1);
            list.Items.Add(listItem2);
            list.CreateControl();
            form.Controls.Add(list);
            form.Show();

            AccessibleObject accessibleObject = list.AccessibilityObject;
            AccessibleObject group1AccObj = listGroup.AccessibilityObject;
            Assert.True(list.IsHandleCreated);

            RECT groupRect = new RECT();
            User32.SendMessageW(list, (User32.WM)ComCtl32.LVM.GETGROUPRECT, listGroup.ID, ref groupRect);

            int actualWidth = group1AccObj.Bounds.Width;
            int expectedWidth = groupRect.Width;
            Assert.Equal(expectedWidth, actualWidth);

            int actualHeight = group1AccObj.Bounds.Height;
            int expectedHeight = groupRect.Height;
            Assert.Equal(expectedHeight, actualHeight);

            Rectangle actualBounds = group1AccObj.Bounds;
            actualBounds.Location = new Point(0, 0);
            Rectangle expectedBounds = groupRect;
            Assert.Equal(expectedBounds, actualBounds);
        }
    }
}
