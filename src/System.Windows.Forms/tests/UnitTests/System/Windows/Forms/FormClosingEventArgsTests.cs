// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Windows.Forms.Tests
{
    // NB: doesn't require thread affinity
    public class FormClosingEventArgsTests : IClassFixture<ThreadExceptionFixture>
    {
        [Theory]
        [InlineData(CloseReason.None, true)]
        [InlineData((CloseReason)(CloseReason.None - 1), false)]
        public void Ctor_CloseReason_Bool(CloseReason closeReason, bool cancel)
        {
            var e = new FormClosingEventArgs(closeReason, cancel);
            Assert.Equal(closeReason, e.CloseReason);
            Assert.Equal(cancel, e.Cancel);
        }
    }
}
