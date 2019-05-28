// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using WinForms.Common.Tests;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class ListViewInsertionMarkTests
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

        [Fact]
        public void ListViewInsertionMark_Color_Get_ReturnsEqual()
        {
            var listView = new ListView();
            ListViewInsertionMark insertionMark = listView.InsertionMark;
            Assert.Equal(insertionMark.Bounds, insertionMark.Bounds);
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
