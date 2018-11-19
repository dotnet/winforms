// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Windows.Forms.Tests
{
    using System.Drawing;

    public class ProgressBarTests
    {
        [Fact]
        public void ProgressBar_Constructor()
        {
            var pb = new ProgressBar();

            Assert.NotNull(pb);
            Assert.Equal(SystemColors.Highlight, pb.ForeColor);
        }
    }
}
