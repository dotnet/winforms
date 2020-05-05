// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Drawing;
using Xunit;

namespace System.Windows.Forms.Tests
{
    // NB: doesn't require thread affinity
    public class ContentsResizedEventArgsTests : IClassFixture<ThreadExceptionFixture>
    {
        public static IEnumerable<object[]> Ctor_Rectangle_TestData()
        {
            yield return new object[] { Rectangle.Empty };
            yield return new object[] { new Rectangle(1, 2, 3, 4) };
            yield return new object[] { new Rectangle(-1, -2, -3, -4) };
        }

        [Theory]
        [MemberData(nameof(Ctor_Rectangle_TestData))]
        public void Ctor_Rectangle(Rectangle newRectangle)
        {
            var e = new ContentsResizedEventArgs(newRectangle);
            Assert.Equal(newRectangle, e.NewRectangle);
        }
    }
}
