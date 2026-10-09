// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Windows.Forms.UITests.Input;

namespace System.Windows.Forms.UITests;

/// <summary>
///  Tests input-count validation without sending keyboard or mouse input.
/// </summary>
public class InputSimulatorTests
{
    [Theory]
    [InlineData(1, 1u, 0)]
    [InlineData(2, 2u, 5)]
    [InlineData(4, 4u, 0)]
    public void VerifyInsertedCount_AllEventsInserted_DoesNotThrow(int requested, uint inserted, int error)
    {
        InputSimulator.VerifyInsertedCount(requested, inserted, error);
    }

    [Theory]
    [InlineData(1, 0u, 0)]
    [InlineData(2, 0u, 5)]
    [InlineData(4, 2u, 0)]
    [InlineData(4, 3u, 87)]
    public void VerifyInsertedCount_MissingEvents_Throws(int requested, uint inserted, int error)
    {
        InvalidOperationException exception = Assert.Throws<InvalidOperationException>(
            () => InputSimulator.VerifyInsertedCount(requested, inserted, error));

        Assert.Contains($"inserted {inserted} of {requested} requested input events", exception.Message);
        Assert.Contains($"Last Win32 error: {error}.", exception.Message);
        Assert.Equal(error == 0, exception.Message.Contains("No extended error information was reported."));
    }
}
