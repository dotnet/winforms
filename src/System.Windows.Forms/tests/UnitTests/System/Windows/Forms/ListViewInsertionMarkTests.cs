// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Drawing;
using WinForms.Common.Tests;
using Xunit;
using static Interop;
using static Interop.ComCtl32;

namespace System.Windows.Forms.Tests
{
    public class ListViewInsertionMarkTests : IClassFixture<ThreadExceptionFixture>
    {
        [Fact]
        public void ListViewInsertionMark_AppearsAfterItem_Get_ReturnsExpected()
        {
            var listView = new ListView();
            ListViewInsertionMark insertionMark = listView.InsertionMark;
            Assert.False(insertionMark.AppearsAfterItem);
        }

        [Fact]
        public void ListViewInsertionMark_AppearsAfterItem_SetWithHandle_ReturnsExpected()
        {
            var listView = new ListView();
            Assert.NotEqual(IntPtr.Zero, listView.Handle);
            ListViewInsertionMark insertionMark = listView.InsertionMark;

            // Set true.
            insertionMark.AppearsAfterItem = true;
            Assert.True(insertionMark.AppearsAfterItem);

            // Set again to test caching behaviour.
            insertionMark.AppearsAfterItem = true;
            Assert.True(insertionMark.AppearsAfterItem);

            // Set false.
            insertionMark.AppearsAfterItem = false;
            Assert.False(insertionMark.AppearsAfterItem);

            // Set with color.
            insertionMark.Color = Color.Blue;
            insertionMark.AppearsAfterItem = true;
            Assert.True(insertionMark.AppearsAfterItem);
        }

        [Fact]
        public void ListViewInsertionMark_AppearsAfterItem_SetWithoutHandle_ReturnsExpected()
        {
            var listView = new ListView();
            ListViewInsertionMark insertionMark = listView.InsertionMark;

            // Set true.
            insertionMark.AppearsAfterItem = true;
            Assert.True(insertionMark.AppearsAfterItem);

            // Set again to test caching behaviour.
            insertionMark.AppearsAfterItem = true;
            Assert.True(insertionMark.AppearsAfterItem);

            // Set false.
            insertionMark.AppearsAfterItem = false;
            Assert.False(insertionMark.AppearsAfterItem);
        }

        [WinFormsFact]
        public void ListViewInsertionMark_Bounds_GetWithoutHandle_ReturnsEqual()
        {
            using var control = new ListView();
            ListViewInsertionMark insertionMark = control.InsertionMark;
            Assert.Equal(Rectangle.Empty, insertionMark.Bounds);
            Assert.Equal(insertionMark.Bounds, insertionMark.Bounds);
            Assert.True(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void ListViewInsertionMark_Bounds_GetWithHandle_ReturnsEqual()
        {
            using var control = new ListView();
            ListViewInsertionMark insertionMark = control.InsertionMark;
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            Assert.Equal(Rectangle.Empty, insertionMark.Bounds);
            Assert.Equal(insertionMark.Bounds, insertionMark.Bounds);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        public static IEnumerable<object[]> Bounds_GetCustomInsertMarkRect_TestData()
        {
            yield return new object[] { new RECT(), Rectangle.Empty };
            yield return new object[] { new RECT(1, 2, 3, 4), new Rectangle(1, 2, 2, 2) };
        }

        [WinFormsTheory]
        [MemberData(nameof(Bounds_GetCustomInsertMarkRect_TestData))]
        public void ListViewInsertionMark_Bounds_GetCustomGetInsertMarkRect_ReturnsExpected(object getInsertMarkRectResult, Rectangle expected)
        {
            using var control = new CustomGetInsertMarkRectListView
            {
                GetInsertMarkRectResult = (RECT)getInsertMarkRectResult
            };
            ListViewInsertionMark insertionMark = control.InsertionMark;
            Assert.NotEqual(IntPtr.Zero, control.Handle);

            Assert.Equal(expected, insertionMark.Bounds);
        }

        private class CustomGetInsertMarkRectListView : ListView
        {
            public RECT GetInsertMarkRectResult { get; set; }

            protected unsafe override void WndProc(ref Message m)
            {
                if (m.Msg == (int)LVM.GETINSERTMARKRECT)
                {
                    RECT* pRect = (RECT*)m.LParam;
                    *pRect = GetInsertMarkRectResult;
                    m.Result = (IntPtr)1;
                    return;
                }

                base.WndProc(ref m);
            }
        }

        [WinFormsFact]
        public void ListViewInsertionMark_Bounds_GetInvalidGetInsertMarkRect_ReturnsExpected()
        {
            using var control = new InvalidGetInsertMarkRectListView();
            ListViewInsertionMark insertionMark = control.InsertionMark;
            Assert.NotEqual(IntPtr.Zero, control.Handle);

            control.MakeInvalid = true;
            Assert.Equal(new Rectangle(1, 2, 2, 2), insertionMark.Bounds);
        }

        private class InvalidGetInsertMarkRectListView : ListView
        {
            public bool MakeInvalid { get; set; }

            protected unsafe override void WndProc(ref Message m)
            {
                if (MakeInvalid && m.Msg == (int)LVM.GETINSERTMARKRECT)
                {
                    RECT* pRect = (RECT*)m.LParam;
                    *pRect = new RECT(1, 2, 3, 4);
                    m.Result = IntPtr.Zero;
                    return;
                }

                base.WndProc(ref m);
            }
        }

        [Fact]
        public void ListViewInsertionMark_Color_Get_ReturnsExpected()
        {
            var listView = new ListView();
            ListViewInsertionMark insertionMark = listView.InsertionMark;
            Assert.NotEqual(Color.Empty, insertionMark.Color);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetColorTheoryData))]
        public void Color_SetWithHandle_ReturnsExpected(Color value)
        {
            var listView = new ListView();
            Assert.NotEqual(IntPtr.Zero, listView.Handle);

            ListViewInsertionMark insertionMark = listView.InsertionMark;
            insertionMark.Color = value;
            Assert.Equal(value, insertionMark.Color);

            // Set again to test caching behaviour.
            insertionMark.Color = value;
            Assert.Equal(value, insertionMark.Color);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetColorTheoryData))]
        public void ListViewInsertionMark_Color_SetWithoutHandle_ReturnsExpected(Color value)
        {
            var listView = new ListView();
            ListViewInsertionMark insertionMark = listView.InsertionMark;
            insertionMark.Color = value;
            Assert.Equal(value, insertionMark.Color);

            // Set again to test caching behaviour.
            insertionMark.Color = value;
            Assert.Equal(value, insertionMark.Color);
        }

        [Fact]
        public void ListViewInsertionMark_Index_Get_ReturnsExpected()
        {
            var listView = new ListView();
            ListViewInsertionMark insertionMark = listView.InsertionMark;
            Assert.Equal(0, insertionMark.Index);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetIntTheoryData))]
        public void ListViewInsertionMark_Index_SetWithHandle_GetReturnsExpected(int value)
        {
            var listView = new ListView();
            Assert.NotEqual(IntPtr.Zero, listView.Handle);

            ListViewInsertionMark insertionMark = listView.InsertionMark;
            insertionMark.Index = value;
            Assert.Equal(value, insertionMark.Index);

            // Set again to test caching behaviour.
            insertionMark.Index = value;
            Assert.Equal(value, insertionMark.Index);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetIntTheoryData))]
        public void ListViewInsertionMark_Index_SetWithoutHandle_GetReturnsExpected(int value)
        {
            var listView = new ListView();
            ListViewInsertionMark insertionMark = listView.InsertionMark;
            insertionMark.Index = value;
            Assert.Equal(value, insertionMark.Index);

            // Set again to test caching behaviour.
            insertionMark.Index = value;
            Assert.Equal(value, insertionMark.Index);
        }

        [Fact]
        public void ListViewInsertionMark_NearestIndex_NoSuchPoint_ReturnsInvalid()
        {
            var listView = new ListView();
            ListViewInsertionMark insertionMark = listView.InsertionMark;
            Assert.True(insertionMark.NearestIndex(new Point(-10, -11)) >= -1);
        }
    }
}
