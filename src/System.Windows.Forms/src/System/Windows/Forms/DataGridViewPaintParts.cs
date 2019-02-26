// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    [Flags]
    public enum DataGridViewPaintParts
    {
        None = 0x00,
        All = 0x7F,
        Background = 0x01,
        Border = 0x02,
        ContentBackground = 0x04,
        ContentForeground = 0x08,
        ErrorIcon = 0x10,
        Focus = 0x20,
        SelectionBackground = 0x40
    }
}
