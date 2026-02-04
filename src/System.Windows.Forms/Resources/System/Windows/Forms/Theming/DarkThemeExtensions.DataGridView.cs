// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Theming;

public static class DarkThemeExtensions
{
    public static void ApplyDarkTheme(this DataGridView dataGridView)
    {
        ArgumentNullException.ThrowIfNull(dataGridView);

        var palette = DarkThemePalette.CreateDefault();

        // Disable the header's system theme so that our custom styles are not overridden by the system.
        dataGridView.EnableHeadersVisualStyles = false;

        // Table body
        dataGridView.BackgroundColor = palette.Surface;
        dataGridView.DefaultCellStyle.BackColor = palette.Surface;
        dataGridView.DefaultCellStyle.ForeColor = palette.OnSurface;

        // Column header
        dataGridView.ColumnHeadersDefaultCellStyle.BackColor = palette.HeaderBackgroundColor;
        dataGridView.ColumnHeadersDefaultCellStyle.ForeColor = palette.HeaderForegroundColor;

        // Light-colored dividing line
        dataGridView.GridColor = ControlPaint.Light(palette.Surface, 0.50f);

        // RowHeaders
        dataGridView.RowHeadersDefaultCellStyle.BackColor = palette.HeaderBackgroundColor;
        dataGridView.RowHeadersDefaultCellStyle.ForeColor = palette.HeaderForegroundColor;

        // Selected state
        dataGridView.RowHeadersDefaultCellStyle.SelectionBackColor = Color.Empty;
        dataGridView.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.Empty;

        dataGridView.DefaultCellStyle.SelectionBackColor = palette.SelectionBackgroundColor;
        dataGridView.DefaultCellStyle.SelectionForeColor = palette.SelectionForegroundColor;
    }
}
