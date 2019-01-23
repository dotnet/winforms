// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Globalization;

namespace System.Windows.Forms
{
    public class DataGridViewRowHeightInfoNeededEventArgs : EventArgs
    {
        private int _height;
        private int _minimumHeight;

        internal DataGridViewRowHeightInfoNeededEventArgs()
        {
            RowIndex = -1;
            _height = -1;
            _minimumHeight = -1;
        }

        public int RowIndex { get; private set; }

        public int Height
        {
            get => _height;
            set
            {
                if (value < _minimumHeight)
                {
                    value = _minimumHeight;
                }
                if (value > DataGridViewBand.maxBandThickness)
                {
                    throw new ArgumentOutOfRangeException(nameof(Height), string.Format(SR.InvalidHighBoundArgumentEx, "Height", (value).ToString(CultureInfo.CurrentCulture), (DataGridViewBand.maxBandThickness).ToString(CultureInfo.CurrentCulture)));
                }

                _height = value;
            }
        }

        public int MinimumHeight
        {
            get => _minimumHeight;
            set
            {
                if (value < DataGridViewBand.minBandThickness)
                {
                    throw new ArgumentOutOfRangeException(nameof(MinimumHeight), value, string.Format(SR.DataGridViewBand_MinimumHeightSmallerThanOne, (DataGridViewBand.minBandThickness).ToString(CultureInfo.CurrentCulture)));
                }

                if (_height < value)
                {
                    _height = value;
                }
                _minimumHeight = value;
            }
        }

        internal void SetProperties(int rowIndex, int height, int minimumHeight)
        {
            Debug.Assert(rowIndex >= -1);
            Debug.Assert(height > 0);
            Debug.Assert(minimumHeight > 0);
            Debug.Assert(height >= minimumHeight);
            RowIndex = rowIndex;
            _height = height;
            _minimumHeight = minimumHeight;
        }
    }
}
