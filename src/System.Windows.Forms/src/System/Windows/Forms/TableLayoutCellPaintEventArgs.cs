// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {
    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Design;    
    using System.Globalization;
    using System.Windows.Forms.Layout;
    
    /// <include file='doc\TableLayoutSettings.uex' path='docs/doc[@for="PaintCellEventArgs"]/*' />
    /// <devdoc>
    /// This is the overrided PaintEventArgs for painting the cell of the table
    /// It contains additional information indicating the row/column of the cell
    /// as well as the bound of the cell
    /// </devdoc>
    public class TableLayoutCellPaintEventArgs : PaintEventArgs {
        private Rectangle bounds;
        private int row;
        private int column;

        /// <include file='doc\TableLayoutSettings.uex' path='docs/doc[@for="PaintCellEventArgs.PaintCellEventArgs"]/*' />
        public TableLayoutCellPaintEventArgs(Graphics g, Rectangle clipRectangle, Rectangle cellBounds, int column, int row) : base(g, clipRectangle) {
            this.bounds = cellBounds;
            this.row = row;
            this.column = column;
        }

        //the bounds of the cell
        /// <include file='doc\TableLayoutSettings.uex' path='docs/doc[@for="PaintCellEventArgs.Bounds"]/*' />
        public Rectangle CellBounds {
            get { return bounds; }
        }

        //the row index of the cell
        /// <include file='doc\TableLayoutSettings.uex' path='docs/doc[@for="PaintCellEventArgs.Row"]/*' />
        public int Row {
            get { return row; }
        }

        //the column index of the cell
        /// <include file='doc\TableLayoutSettings.uex' path='docs/doc[@for="PaintCellEventArgs.Column"]/*' />
        public int Column {
            get { return column; } 
        }
    }
    }

