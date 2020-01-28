// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    [Flags]
    internal enum DataGridViewAutoSizeColumnCriteriaInternal
    {
        NotSet = 0x00,
        None = 0x01,
        Header = 0x02,
        AllRows = 0x04,
        DisplayedRows = 0x08,
        Fill = 0x10
    }
}
