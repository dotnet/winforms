// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;

internal static partial class Interop
{
    internal static partial class ComCtl32
    {
        public struct NMTREEVIEW
        {
            public User32.NMHDR nmhdr;
            public TVC action;
            public TVITEMW itemOld;
            public TVITEMW itemNew;
            public Point ptDrag;
        }
    }
}
