// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable disable

namespace System.Windows.Forms.Tests;

public class PrintPreviewDialogTests
{
    [WinFormsTheory]
    [InlineData(Keys.D1, 1, 1)]
    [InlineData(Keys.D2, 1, 2)]
    [InlineData(Keys.D3, 1, 3)]
    [InlineData(Keys.D4, 2, 2)]
    [InlineData(Keys.D5, 2, 3)]
    public void PrintPreviewDialog_Hotkey_Ctrl_Digit_AddsRowsAndColumns(Keys digitKey, int rows, int columns)
    {
        using TestPrintPreviewDialog testPrintPreviewDialog = new();
        testPrintPreviewDialog.TestProcessDialogKey(Keys.Control | digitKey);

        Assert.Equal(rows, testPrintPreviewDialog.PrintPreviewControl.Rows);
        Assert.Equal(columns, testPrintPreviewDialog.PrintPreviewControl.Columns);
    }

    [WinFormsTheory]
    [InlineData(Keys.Left)]
    [InlineData(Keys.Right)]
    [InlineData(Keys.Up)]
    [InlineData(Keys.Down)]
    public void PrintPreviewDialog_Hotkey_ArrowKeys_ReturnsFalse(Keys arrowKey)
    {
        using TestPrintPreviewDialog testPrintPreviewDialog = new();
        Assert.False(testPrintPreviewDialog.TestProcessDialogKey(arrowKey));
    }

    private class TestPrintPreviewDialog : PrintPreviewDialog
    {
        internal bool TestProcessDialogKey(Keys keyData)
        {
            return ProcessDialogKey(keyData);
        }
    }
}
