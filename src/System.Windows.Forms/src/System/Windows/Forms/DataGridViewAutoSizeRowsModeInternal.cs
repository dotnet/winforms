// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    [Flags]
    internal enum DataGridViewAutoSizeRowsModeInternal
    {
        None = 0x00,
        Header = 0x01,
        AllColumns = 0x02,
        AllRows = 0x04,
        DisplayedRows = 0x08
    }
}
