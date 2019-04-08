// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class ListViewTests
    {
        [Fact]
        public void ListView_Ctor_Default()
        {
            var listView = new ListView();
            Assert.Equal(Point.Empty, listView.Location);
            Assert.Equal(121, listView.Width);
            Assert.Equal(97, listView.Height);
            Assert.Empty(listView.Items);
            Assert.Empty(listView.Columns);
        }
    }
}
