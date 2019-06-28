// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Windows.Forms.Tests
{
    public class LinkLabelTests
    {
        [Fact]
        public void LinkLabel_Constructor()
        {
            var label = new LinkLabel();

            Assert.NotNull(label);
            Assert.True(label.LinkArea.IsEmpty);
            Assert.Equal(0, label.LinkArea.Start);
            Assert.Equal(0, label.LinkArea.Length);
        }
    }
}
