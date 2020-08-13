// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Xunit;

namespace System.Windows.Forms.Tests
{
    // NB: doesn't require thread affinity
    public class DataGridViewRowContextMenuStripNeededEventArgsTests
    {
        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        public void Ctor_Int(int rowIndex)
        {
            var e = new DataGridViewRowContextMenuStripNeededEventArgs(rowIndex);
            Assert.Equal(rowIndex, e.RowIndex);
            Assert.Null(e.ContextMenuStrip);
        }

        [Fact]
        public void Ctor_NegativeRowIndex_ThrowsArgumentOutOfRangeException()
        {
            Assert.Throws<ArgumentOutOfRangeException>("rowIndex", () => new DataGridViewRowContextMenuStripNeededEventArgs(-2));
        }

        public static IEnumerable<object[]> ContextMenuStrip_TestData()
        {
            yield return new object[] { null };
            yield return new object[] { new ContextMenuStrip() };
        }

        [WinFormsTheory]
        [MemberData(nameof(ContextMenuStrip_TestData))]
        public void ContextMenuStrip_Set_GetReturnsExpected(ContextMenuStrip value)
        {
            var e = new DataGridViewRowContextMenuStripNeededEventArgs(1)
            {
                ContextMenuStrip = value
            };
            Assert.Equal(value, e.ContextMenuStrip);
        }
    }
}
