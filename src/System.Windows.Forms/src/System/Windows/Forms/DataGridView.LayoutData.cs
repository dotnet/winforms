// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using System.Text;

namespace System.Windows.Forms
{
    public partial class DataGridView
    {
        internal class LayoutData
        {
            internal bool dirty = true;

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
                StringBuilder sb = new StringBuilder(100);
                sb.Append(base.ToString());
                sb.Append(" { \n");
                sb.Append("ClientRectangle = ");
                sb.Append(ClientRectangle.ToString());
                sb.Append('\n');
                sb.Append("Inside = ");
                sb.Append(Inside.ToString());
                sb.Append('\n');
                sb.Append("TopLeftHeader = ");
                sb.Append(TopLeftHeader.ToString());
                sb.Append('\n');
                sb.Append("ColumnHeaders = ");
                sb.Append(ColumnHeaders.ToString());
                sb.Append('\n');
                sb.Append("RowHeaders = ");
                sb.Append(RowHeaders.ToString());
                sb.Append('\n');
                sb.Append("Data = ");
                sb.Append(Data.ToString());
                sb.Append('\n');
                sb.Append("ResizeBoxRect = ");
                sb.Append(ResizeBoxRect.ToString());
                sb.Append('\n');
                sb.Append("ColumnHeadersVisible = ");
                sb.Append(ColumnHeadersVisible.ToString());
                sb.Append('\n');
                sb.Append("RowHeadersVisible = ");
                sb.Append(RowHeadersVisible.ToString());
                sb.Append(" }");
                return sb.ToString();
            }
        }
    }
}
