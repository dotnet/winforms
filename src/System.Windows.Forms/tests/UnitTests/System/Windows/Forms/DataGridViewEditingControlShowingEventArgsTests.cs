// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Xunit;

namespace System.Windows.Forms.Tests
{
    // NB: doesn't require thread affinity
    public class DataGridViewEditingControlShowingEventArgsTests : IClassFixture<ThreadExceptionFixture>
    {
        public static IEnumerable<object[]> Ctor_Control_DataGridViewCellStyle_TestData()
        {
            yield return new object[] { null, null };
            yield return new object[] { new Button(), new DataGridViewCellStyle() };
        }

        [WinFormsTheory]
        [MemberData(nameof(Ctor_Control_DataGridViewCellStyle_TestData))]
        public void Ctor_Control_DataGridViewCellStyle(Control control, DataGridViewCellStyle cellStyle)
        {
            var e = new DataGridViewEditingControlShowingEventArgs(control, cellStyle);
            Assert.Equal(control, e.Control);
            Assert.Equal(cellStyle, e.CellStyle);
        }

        [Fact]
        public void CellStyle_SetNonNull_GetReturnsExpected()
        {
            var value = new DataGridViewCellStyle();
            var e = new DataGridViewEditingControlShowingEventArgs(null, null) { CellStyle = value };
            Assert.Equal(value, e.CellStyle);
        }

        [Fact]
        public void CellStyle_SetNull_ThrowsArgumentNullException()
        {
            var e = new DataGridViewEditingControlShowingEventArgs(null, null);
            Assert.Throws<ArgumentNullException>("value", () => e.CellStyle = null);
        }
    }
}
