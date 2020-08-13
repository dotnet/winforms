// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Windows.Forms.Tests
{
    // NB: doesn't require thread affinity
    public class ToolStripDropDownClosedEventArgsTests : IClassFixture<ThreadExceptionFixture>
    {
        [Theory]
        [InlineData(ToolStripDropDownCloseReason.AppClicked)]
        [InlineData((ToolStripDropDownCloseReason)(ToolStripDropDownCloseReason.AppFocusChange - 1))]
        public void Ctor_CloseReason(ToolStripDropDownCloseReason closeReason)
        {
            var e = new ToolStripDropDownClosedEventArgs(closeReason);
            Assert.Equal(closeReason, e.CloseReason);
        }
    }
}
