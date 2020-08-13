// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    public partial class TabControl
    {
        private enum State
        {
            HotTrack = 0x00000001,
            Multiline = 0x00000002,
            ShowToolTips = 0x00000004,
            GetTabRectfromItemSize = 0x00000008,
            FromCreateHandles = 0x00000010,
            UISelection = 0x00000020,
            SelectFirstControl = 0x00000040,
            InsertingItem = 0x00000080,
        }
    }
}
