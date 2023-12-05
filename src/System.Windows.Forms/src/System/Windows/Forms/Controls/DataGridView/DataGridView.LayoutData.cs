// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace System.Windows.Forms;

public partial class DataGridView
{
    internal class LayoutData
    {
        internal bool _dirty = true;

        // used for resizing.
        public Rectangle ClientRectangle = Rectangle.Empty;

        // region inside the dataGridView's borders.
        public Rectangle Inside = Rectangle.Empty;

        // region occupied by row headers
        public Rectangle RowHeaders = Rectangle.Empty;

        // region occupied by column headers
        public Rectangle ColumnHeaders = Rectangle.Empty;

        // top left header cell
        public Rectangle TopLeftHeader = Rectangle.Empty;

        // region for the cells
        public Rectangle Data = Rectangle.Empty;

        // square connecting the two scrollbars
        public Rectangle ResizeBoxRect = Rectangle.Empty;

        public bool ColumnHeadersVisible;
        public bool RowHeadersVisible;

        public LayoutData()
        {
        }

        public LayoutData(LayoutData src)
        {
            ClientRectangle = src.ClientRectangle;
            TopLeftHeader = src.TopLeftHeader;
            ColumnHeaders = src.ColumnHeaders;
            RowHeaders = src.RowHeaders;
            Inside = src.Inside;
            Data = src.Data;
            ResizeBoxRect = src.ResizeBoxRect;
            ColumnHeadersVisible = src.ColumnHeadersVisible;
            RowHeadersVisible = src.RowHeadersVisible;
        }

        public override string ToString()
        {
            return $$"""
                {{base.ToString()}} {
                ClientRectangle = {{ClientRectangle}}
                Inside = {{Inside}}
                TopLeftHeader = {{TopLeftHeader}}
                ColumnHeaders = {{ColumnHeaders}}
                RowHeaders = {{RowHeaders}}
                Data = {{Data}}
                ResizeBoxRect = {{ResizeBoxRect}}
                ColumnHeadersVisible = {{ColumnHeadersVisible}}
                RowHeadersVisible = {{RowHeadersVisible}} }
                """;
        }
    }
}
