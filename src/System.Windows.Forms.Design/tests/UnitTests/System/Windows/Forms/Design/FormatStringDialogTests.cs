// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using System.ComponentModel;
using Moq;

namespace System.Windows.Forms.Design.Tests;

public class FormatStringDialogTests : IDisposable
{
    private readonly FormatStringDialog _formatStringDialog;

    private readonly Mock<ITypeDescriptorContext> _context = new(MockBehavior.Strict);

    public FormatStringDialogTests() => _formatStringDialog = new(_context.Object);

    public void Dispose() => _formatStringDialog.Dispose();

    [Fact]
    public void Ctor_WithContext_DoesNotThrow()
    {
        Action action = () =>
        {
            FormatStringDialog dialog = new(_context.Object);
            dialog.Dispose();
        };

        action.Should().NotThrow();
    }

    [Fact]
    public void FormatStringDialog_HasProperRTLConfiguration()
    {
        if (SR.RTL == "RTL_False")
        {
            _formatStringDialog.RightToLeft.Should().Be(RightToLeft.No);
            _formatStringDialog.RightToLeftLayout.Should().Be(false);
        }
        else
        {
            _formatStringDialog.RightToLeft.Should().Be(RightToLeft.Yes);
            _formatStringDialog.RightToLeftLayout.Should().Be(true);
        }
    }

    [Fact]
    public void SetsDataGridViewCellStyle_DoesNotThrow()
    {
        Action action = () => _formatStringDialog.DataGridViewCellStyle = new();

        action.Should().NotThrow();
    }

    [Fact]
    public void Dirty_ReturnsDefault() => _formatStringDialog.Dirty.Should().Be(false);

    [Fact]
    public void SetsListControl_DoesNotThrow()
    {
        Action action = () =>
        {
            using ListBox listControl = new();
            _formatStringDialog.ListControl = listControl;
        };
        action.Should().NotThrow();
    }

    [Fact]
    public void End_DoesNotThrow()
    {
        Action action = FormatStringDialog.End;

        action.Should().NotThrow();
    }

    [Fact]
    public void FormatControlFinishedLoading_AdjustsButtonPositionsCorrectly()
    {
        dynamic? okButtonField = _formatStringDialog.TestAccessor().Dynamic._okButton;
        dynamic? cancelButtonField = _formatStringDialog.TestAccessor().Dynamic._cancelButton;
        dynamic? formatControlField = _formatStringDialog.TestAccessor().Dynamic._formatControl1;

        int okButtonLeftOriginalState = okButtonField.Left;
        int cancelButtonLeftOriginalState = cancelButtonField.Left;

        static int GetRightSideOffset(Control ctl)
        {
            int result = ctl.Width;
            Control? control = ctl;
            while (control is not null)
            {
                result += control.Left;
                control = control.Parent;
            }

            return result;
        }

        int formatControlRightSideOffset = GetRightSideOffset(formatControlField);
        int cancelButtonRightSideOffset = GetRightSideOffset(cancelButtonField);

        _formatStringDialog.FormatControlFinishedLoading();

        ((object)okButtonField.Top).Should().Be(formatControlField.Bottom + 5);
        ((object)cancelButtonField.Top).Should().Be(formatControlField.Bottom + 5);
        ((object)okButtonField.Left).Should().Be(okButtonLeftOriginalState + formatControlRightSideOffset - cancelButtonRightSideOffset);
        ((object)cancelButtonField.Left).Should().Be(cancelButtonLeftOriginalState + formatControlRightSideOffset - cancelButtonRightSideOffset);
    }
}
