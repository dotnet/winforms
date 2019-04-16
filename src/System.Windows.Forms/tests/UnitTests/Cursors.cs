// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Windows.Forms.Tests
{
    public class CursorsTests
    {
        /// <summary>
        /// Data for the tests
        /// </summary>
        public static TheoryData<Cursor> AllCursorsData =>
            TestHelper.GetCursorTheoryData();

        [Theory]
        [MemberData(nameof(AllCursorsData))]
        public void Cursors_NotNull(Cursor cursor)
        {
            Assert.NotNull(cursor);
        }

        [Theory]
        [MemberData(nameof(AllCursorsData))]
        public void Cursors_NotNullHandle(Cursor cursor)
        {
            Assert.NotEqual(IntPtr.Zero, cursor.Handle);
        }
    }
}
