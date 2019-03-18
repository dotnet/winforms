// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Windows.Forms.Tests
{
    public class FormClosedEventArgsTests
    {
        [Theory]
        [InlineData(CloseReason.None)]
        [InlineData((CloseReason)(CloseReason.None - 1))]
        public void Ctor_CloseReason(CloseReason closeReason)
        {
            var e = new FormClosedEventArgs(closeReason);
            Assert.Equal(closeReason, e.CloseReason);
        }
    }
}
