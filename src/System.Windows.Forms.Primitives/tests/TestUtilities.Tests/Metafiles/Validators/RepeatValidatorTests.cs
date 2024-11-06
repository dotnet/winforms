// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Moq;

namespace System.Windows.Forms.Metafiles.Tests;

public class RepeatValidatorTests
{
    [Fact]
    public unsafe void RepeatValidator_Validate_complete_should_be_expected()
    {
        Mock<IEmfValidator> emfValidator = new();
        RepeatValidator repeatValidator = new(emfValidator.Object, 2);

        EmfRecord emfRecord = default;

        repeatValidator.Validate(ref emfRecord, state: null!, out bool complete);
        Assert.False(complete);

        // call again - we'll be at zero now
        repeatValidator.Validate(ref emfRecord, state: null!, out complete);
        Assert.True(complete);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public unsafe void RepeatValidator_Validate_should_throw_IOE_if_count_not_positive(int count)
    {
        Mock<IEmfValidator> emfValidator = new();
        RepeatValidator repeatValidator = new(emfValidator.Object, count);

        EmfRecord emfRecord = default;

        Assert.Throws<InvalidOperationException>(() => repeatValidator.Validate(ref emfRecord, state: null!, out bool complete));
    }
}
