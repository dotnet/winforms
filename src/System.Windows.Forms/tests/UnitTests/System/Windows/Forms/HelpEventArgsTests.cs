// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Drawing;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class HelpEventArgsTests
    {
        public static IEnumerable<object[]> Ctor_Point_TestData()
        {
            yield return new object[] { Point.Empty };
            yield return new object[] { new Point(1, 2) };
            yield return new object[] { new Point(-1, -2) };
        }

        [Theory]
        [MemberData(nameof(Ctor_Point_TestData))]
        public void Ctor_Point(Point mousePos)
        {
            var e = new HelpEventArgs(mousePos);
            Assert.Equal(mousePos, e.MousePos);
            Assert.False(e.Handled);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Handled_Set_GetReturnsExpected(bool value)
        {
            var e = new HelpEventArgs(new Point(1, 2))
            {
                Handled = value
            };
            Assert.Equal(value, e.Handled);
        }
    }
}
