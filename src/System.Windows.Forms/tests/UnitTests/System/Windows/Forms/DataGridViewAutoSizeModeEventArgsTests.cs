// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Windows.Forms.Tests
{
    // NB: doesn't require thread affinity
    public class DataGridViewAutoSizeModeEventArgsTests : IClassFixture<ThreadExceptionFixture>
    {
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Ctor_Bool(bool previousModeAutoSized)
        {
            var e = new DataGridViewAutoSizeModeEventArgs(previousModeAutoSized);
            Assert.Equal(previousModeAutoSized, e.PreviousModeAutoSized);
        }
    }
}
