// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Diagnostics;

namespace System.Windows.Forms
{
    public class DataGridViewRowHeightInfoPushedEventArgs : HandledEventArgs
    {
        internal DataGridViewRowHeightInfoPushedEventArgs(int rowIndex, int height, int minimumHeight) : base(false)
        {
            Debug.Assert(rowIndex >= -1);
            RowIndex = rowIndex;
            Height = height;
            MinimumHeight = minimumHeight;
        }

        public int RowIndex { get; }

        public int Height { get; }

        public int MinimumHeight { get; }
    }
}
