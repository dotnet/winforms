// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using System.Windows.Forms.TestUtilities;
using Xunit;
using static Interop;

namespace System.Windows.Forms.Tests
{
    using Point = System.Drawing.Point;

    public class ListViewInsertionMarkTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void ListViewInsertionMark_AppearsAfterItem_Get_ReturnsExpected()
        {
            using var control = new ListView();
            ListViewInsertionMark insertionMark = control.InsertionMark;
            Assert.False(insertionMark.AppearsAfterItem);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(typeof(CommonTestHelper), nameof(CommonTestHelper.GetBoolTheoryData))]
        public void ListViewInsertionMark_AppearsAfterItem_Set_GetReturnsExpected(bool value)
        {
            using var control = new ListView();
            ListViewInsertionMark insertionMark = control.InsertionMark;

            insertionMark.AppearsAfterItem = value;
            Assert.Equal(value, insertionMark.AppearsAfterItem);
            Assert.False(control.IsHandleCreated);

            // Set same.
            insertionMark.AppearsAfterItem = value;
            Assert.Equal(value, insertionMark.AppearsAfterItem);
            Assert.False(control.IsHandleCreated);

            // Set different.
            insertionMark.AppearsAfterItem = !value;
            Assert.Equal(!value, insertionMark.AppearsAfterItem);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(typeof(CommonTestHelper), nameof(CommonTestHelper.GetBoolTheoryData))]
        public void ListViewInsertionMark_AppearsAfterItem_SetWithHandle_GetReturnsExpected(bool value)
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

            insertionMark.AppearsAfterItem = value;
            Assert.Equal(value, insertionMark.AppearsAfterItem);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            insertionMark.AppearsAfterItem = value;
            Assert.Equal(value, insertionMark.AppearsAfterItem);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set different.
            insertionMark.AppearsAfterItem = !value;
            Assert.Equal(!value, insertionMark.AppearsAfterItem);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
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
                if (m.Msg == (int)ComCtl32.LVM.GETINSERTMARKRECT)
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
                if (MakeInvalid && m.Msg == (int)ComCtl32.LVM.GETINSERTMARKRECT)
                {
                    RECT* pRect = (RECT*)m.LParam;
                    *pRect = new RECT(1, 2, 3, 4);
                    m.Result = IntPtr.Zero;
                    return;
                }

                base.WndProc(ref m);
            }
        }

        [WinFormsFact]
        public void ListViewInsertionMark_Color_GetWithoutHandle_ReturnsExpected()
        {
            using var control = new ListView();
            ListViewInsertionMark insertionMark = control.InsertionMark;
            Assert.NotEqual(Color.Empty, insertionMark.Color);
            Assert.Equal(insertionMark.Color, insertionMark.Color);
            Assert.True(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void ListViewInsertionMark_Color_GetWithHandle_ReturnsExpected()
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

            Assert.NotEqual(Color.Empty, insertionMark.Color);
            Assert.Equal(insertionMark.Color, insertionMark.Color);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [CommonMemberData(typeof(CommonTestHelper), nameof(CommonTestHelper.GetColorTheoryData))]
        public void ListViewInsertionMark_Color_SetWithoutHandle_ReturnsExpected(Color value)
        {
            using var control = new ListView();
            ListViewInsertionMark insertionMark = control.InsertionMark;

            insertionMark.Color = value;
            Assert.Equal(value, insertionMark.Color);
            Assert.False(control.IsHandleCreated);

            // Set again.
            insertionMark.Color = value;
            Assert.Equal(value, insertionMark.Color);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(typeof(CommonTestHelper), nameof(CommonTestHelper.GetColorTheoryData))]
        public void ListViewInsertionMark_Color_SetWithHandle_ReturnsExpected(Color value)
        {
            using var control = new ListView();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            ListViewInsertionMark insertionMark = control.InsertionMark;
            insertionMark.Color = value;
            Assert.Equal(value, insertionMark.Color);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set again.
            insertionMark.Color = value;
            Assert.Equal(value, insertionMark.Color);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public void ListViewInsertionMark_Index_Get_ReturnsExpected()
        {
            using var control = new ListView();
            ListViewInsertionMark insertionMark = control.InsertionMark;
            Assert.Equal(0, insertionMark.Index);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(typeof(CommonTestHelper), nameof(CommonTestHelper.GetIntTheoryData))]
        public void ListViewInsertionMark_Index_SetWithoutHandle_GetReturnsExpected(int value)
        {
            using var control = new ListView();
            ListViewInsertionMark insertionMark = control.InsertionMark;

            insertionMark.Index = value;
            Assert.Equal(value, insertionMark.Index);
            Assert.False(control.IsHandleCreated);

            // Set again.
            insertionMark.Index = value;
            Assert.Equal(value, insertionMark.Index);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(typeof(CommonTestHelper), nameof(CommonTestHelper.GetIntTheoryData))]
        public void ListViewInsertionMark_Index_SetWithHandle_GetReturnsExpected(int value)
        {
            using var control = new ListView();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            ListViewInsertionMark insertionMark = control.InsertionMark;
            insertionMark.Index = value;
            Assert.Equal(value, insertionMark.Index);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set again.
            insertionMark.Index = value;
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
            Assert.Equal(value, insertionMark.Index);
        }

        [WinFormsFact]
        public void ListViewInsertionMark_NearestIndex_NoSuchPointWithoutHandle_ReturnsInvalid()
        {
            using var control = new ListView();
            ListViewInsertionMark insertionMark = control.InsertionMark;
            Assert.True(insertionMark.NearestIndex(new Point(-10, -11)) >= -1);
            Assert.True(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void ListViewInsertionMark_NearestIndex_NoSuchPointWithHandle_ReturnsInvalid()
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

            Assert.True(insertionMark.NearestIndex(new Point(-10, -11)) >= -1);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [InlineData(-2)]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        public void ListViewInsertionMark_NearestIndex_InvokeCustomInsertMarkHitTest_ReturnsExpected(int result)
        {
            using var control = new CustomInsertMarkHitTestListView
            {
                InsertMarkHitTestResult = result
            };
            ListViewInsertionMark insertionMark = control.InsertionMark;

            Assert.Equal(result, insertionMark.NearestIndex(new Point(1, 2)));
        }

        private class CustomInsertMarkHitTestListView : ListView
        {
            public int InsertMarkHitTestResult { get; set; }

            protected unsafe override void WndProc(ref Message m)
            {
                if (m.Msg == (int)ComCtl32.LVM.INSERTMARKHITTEST)
                {
                    Point* pPt = (Point*)m.WParam;
                    Assert.Equal(1, pPt->X);
                    Assert.Equal(2, pPt->Y);
                    ComCtl32.LVINSERTMARK* pInsertMark = (ComCtl32.LVINSERTMARK*)m.LParam;
                    pInsertMark->iItem = InsertMarkHitTestResult;
                    m.Result = (IntPtr)1;
                    return;
                }

                base.WndProc(ref m);
            }
        }
    }
}
