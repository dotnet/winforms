// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;

namespace System.Windows.Forms.Tests;

// NB: doesn't require thread affinity
public class MaskInputRejectedEventArgsTests
{
    [Theory]
    [InlineData(-2, MaskedTextResultHint.Unknown)]
    [InlineData(-1, MaskedTextResultHint.Success)]
    [InlineData(0, MaskedTextResultHint.PromptCharNotAllowed)]
    [InlineData(1, (MaskedTextResultHint.SignedDigitExpected - 1))]
    [InlineData(1, MaskedTextResultHint.Unknown)]
    public void Ctor_Int_Int_Bool(int position, MaskedTextResultHint rejectionHint)
    {
        MaskInputRejectedEventArgs e = new(position, rejectionHint);
        Assert.Equal(position, e.Position);
        Assert.Equal(rejectionHint, e.RejectionHint);
    }
}
