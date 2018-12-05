// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    using System;
    using System.Diagnostics;
    using System.ComponentModel;

    public class DataGridViewRowHeightInfoPushedEventArgs : HandledEventArgs
    {
        private int rowIndex;
        private int height;
        private int minimumHeight;

        internal DataGridViewRowHeightInfoPushedEventArgs(int rowIndex, int height, int minimumHeight) : base(false)
        {
            Debug.Assert(rowIndex >= -1);
            this.rowIndex = rowIndex;
            this.height = height;
            this.minimumHeight = minimumHeight;
        }

        public int Height
        {
            get
            {
                return this.height;
            }
        }

        public int MinimumHeight
        {
            get
            {
                return this.minimumHeight;
            }
        }

        public int RowIndex
        {
            get
            {
                return this.rowIndex;
            }
        }
    }
}
