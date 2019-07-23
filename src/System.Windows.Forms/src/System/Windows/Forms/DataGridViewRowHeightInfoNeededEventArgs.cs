// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;

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
                if (value > DataGridViewBand.MaxBandThickness)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), string.Format(SR.InvalidHighBoundArgumentEx, nameof(Height), value, DataGridViewBand.MaxBandThickness));
                }

                _height = value;
            }
        }

        public int MinimumHeight
        {
            get => _minimumHeight;
            set
            {
                if (value < DataGridViewBand.MinBandThickness)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value, string.Format(SR.DataGridViewBand_MinimumHeightSmallerThanOne, DataGridViewBand.MinBandThickness));
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
