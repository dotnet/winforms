// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    using System;
    using System.Diagnostics;
    using System.Globalization;

    public class DataGridViewRowHeightInfoNeededEventArgs : EventArgs
    {
        private int rowIndex;
        private int height;
        private int minimumHeight;

        internal DataGridViewRowHeightInfoNeededEventArgs()
        {
            this.rowIndex = -1;
            this.height = -1;
            this.minimumHeight = -1;
        }

        public int Height
        {
            get
            {
                return this.height;
            }
            set
            {
                if (value < this.minimumHeight)
                {
                    value = this.minimumHeight;
                }
                if (value > DataGridViewBand.maxBandThickness)
                {
                    throw new ArgumentOutOfRangeException(nameof(Height), string.Format(SR.InvalidHighBoundArgumentEx, "Height", (value).ToString(CultureInfo.CurrentCulture), (DataGridViewBand.maxBandThickness).ToString(CultureInfo.CurrentCulture)));
                }
                this.height = value;
            }
        }

        public int MinimumHeight
        {
            get
            {
                return this.minimumHeight;
            }
            set
            {
                if (value < DataGridViewBand.minBandThickness)
                {
                    throw new ArgumentOutOfRangeException(nameof(MinimumHeight), value, string.Format(SR.DataGridViewBand_MinimumHeightSmallerThanOne, (DataGridViewBand.minBandThickness).ToString(CultureInfo.CurrentCulture)));
                }
                if (this.height < value)
                {
                    this.height = value;
                }
                this.minimumHeight = value;
            }
        }

        public int RowIndex
        {
            get
            {
                return this.rowIndex;
            }
        }

        internal void SetProperties(int rowIndex, int height, int minimumHeight)
        {
            Debug.Assert(rowIndex >= -1);
            Debug.Assert(height > 0);
            Debug.Assert(minimumHeight > 0);
            Debug.Assert(height >= minimumHeight);
            this.rowIndex = rowIndex;
            this.height = height;
            this.minimumHeight = minimumHeight;
        }
    }
}
