// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using Moq;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class ListViewTests
    {
        [Fact]
        public void ListView_Constructor()
        {
            var lv = new ListView();

            Assert.NotNull(lv);
            Assert.Equal(0, lv.Location.X);
            Assert.Equal(0, lv.Location.Y);
            Assert.Equal(121, lv.Width);
            Assert.Equal(97, lv.Height);
            Assert.NotNull(lv.Items);
            Assert.NotNull(lv.Columns);
        }
    }
}
