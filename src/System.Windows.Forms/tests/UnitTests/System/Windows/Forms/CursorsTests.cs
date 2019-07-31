// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Drawing;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class CursorsTests
    {
        public static IEnumerable<object[]> Cursors_TestData()
        {
            // Identity function to avoid constant casting
            Func<Cursor> I(Func<Cursor> factory) => factory;

            yield return new object[] { I(() => Cursors.AppStarting) };
            yield return new object[] { I(() => Cursors.Arrow) };
            yield return new object[] { I(() => Cursors.Cross) };
            yield return new object[] { I(() => Cursors.Default) };
            yield return new object[] { I(() => Cursors.Hand) };
            yield return new object[] { I(() => Cursors.Help) };
            yield return new object[] { I(() => Cursors.HSplit) };
            yield return new object[] { I(() => Cursors.IBeam) };
            yield return new object[] { I(() => Cursors.No) };
            yield return new object[] { I(() => Cursors.NoMove2D) };
            yield return new object[] { I(() => Cursors.NoMoveHoriz) };
            yield return new object[] { I(() => Cursors.NoMoveVert) };
            yield return new object[] { I(() => Cursors.PanEast) };
            yield return new object[] { I(() => Cursors.PanNE) };
            yield return new object[] { I(() => Cursors.PanNorth) };
            yield return new object[] { I(() => Cursors.PanNW) };
            yield return new object[] { I(() => Cursors.PanSE) };
            yield return new object[] { I(() => Cursors.PanSouth) };
            yield return new object[] { I(() => Cursors.PanSW) };
            yield return new object[] { I(() => Cursors.PanWest) };
            yield return new object[] { I(() => Cursors.SizeAll) };
            yield return new object[] { I(() => Cursors.SizeNESW) };
            yield return new object[] { I(() => Cursors.SizeNS) };
            yield return new object[] { I(() => Cursors.SizeNWSE) };
            yield return new object[] { I(() => Cursors.SizeWE) };
            yield return new object[] { I(() => Cursors.UpArrow) };
            yield return new object[] { I(() => Cursors.VSplit) };
            yield return new object[] { I(() => Cursors.WaitCursor) };
        }

        [Theory]
        [MemberData(nameof(Cursors_TestData))]
        public void Cursors_KnownCursor_Get_ReturnsExpected(Func<Cursor> getCursor)
        {
            Cursor cursor = getCursor();
            Assert.NotEqual(IntPtr.Zero, cursor.Handle);
            Point hotSpot = cursor.HotSpot;
            Assert.True(hotSpot.X >= 0 && hotSpot.X <= cursor.Size.Width);
            Assert.True(hotSpot.Y >= 0 && hotSpot.Y <= cursor.Size.Height);
            Assert.True(cursor.Size == new Size(32, 32) || cursor.Size == new Size(64, 64));
            Assert.Null(cursor.Tag);
            Assert.Same(cursor, getCursor());
        }
    }
}
