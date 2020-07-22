// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

namespace System.Windows.Forms
{
    public partial class DataGridView
    {
        private struct MouseClickInfo
        {
            public MouseButtons Button;
            public long TimeStamp;
            public int X;
            public int Y;
            public int Col;
            public int Row;
        }
    }
}
