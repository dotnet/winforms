// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using Xunit;

namespace System.Windows.Forms.Tests
{
    // NB: doesn't require thread affinity
    public class MaskInputRejectedEventArgsTests : IClassFixture<ThreadExceptionFixture>
    {
        [Theory]
        [InlineData(-2, MaskedTextResultHint.Unknown)]
        [InlineData(-1, MaskedTextResultHint.Success)]
        [InlineData(0, MaskedTextResultHint.PromptCharNotAllowed)]
        [InlineData(1, (MaskedTextResultHint)(MaskedTextResultHint.SignedDigitExpected - 1))]
        [InlineData(1, MaskedTextResultHint.Unknown)]
        public void Ctor_Int_Int_Bool(int position, MaskedTextResultHint rejectionHint)
        {
            var e = new MaskInputRejectedEventArgs(position, rejectionHint);
            Assert.Equal(position, e.Position);
            Assert.Equal(rejectionHint, e.RejectionHint);
        }
    }
}
