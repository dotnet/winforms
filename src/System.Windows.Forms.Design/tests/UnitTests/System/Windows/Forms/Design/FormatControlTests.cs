// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

namespace System.Windows.Forms.Design.Tests;
public class FormatControlTests : IDisposable
{
    private readonly FormatControl _formatControl = new();
    public void Dispose() => _formatControl.Dispose();

    [Fact]
    public void DefaultProperties_ShouldReturnExpected()
    {
        _formatControl.Should().NotBeNull();
        _formatControl.Should().BeOfType<FormatControl>();
        _formatControl.Should().BeAssignableTo<Control>();
        _formatControl.Dirty.Should().BeFalse();
        _formatControl.FormatType.Should().BeEmpty();
        _formatControl.NullValue.Should().BeNull();
    }

    [Fact]
    public void SetsNullValueTextBoxEnabled_ShouldNotThrow()
    {
        _formatControl.Invoking(f => f.NullValueTextBoxEnabled = false).Should().NotThrow();
    }

    public static IEnumerable<object[]> TestData()
    {
        yield return new object[] { null!, SR.BindingFormattingDialogFormatTypeNoFormatting };
        yield return new object[] { "N1", SR.BindingFormattingDialogFormatTypeNumeric };
        yield return new object[] { "d", SR.BindingFormattingDialogFormatTypeDateTime };
        yield return new object[] { "E1", SR.BindingFormattingDialogFormatTypeScientific };
        yield return new object[] { "CustomString", SR.BindingFormattingDialogFormatTypeCustom };
    }

    [Theory]
    [MemberData(nameof(TestData))]
    public void FormatTypeStringFromFormatString_ShouldReturnCorrectFormatType(string formatString, string formatTypeString) => FormatControl.FormatTypeStringFromFormatString(formatString).Should().Be(formatTypeString);

    [Fact]
    public void ResetFormattingInfo_ShouldNotThrow() => _formatControl.Invoking(f => f.ResetFormattingInfo()).Should().NotThrow();
}
