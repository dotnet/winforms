// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

public partial class ToolTip
{
    private partial class TipInfo
    {
        [Flags]
        public enum Type
        {
            None = 0x0000,
            Auto = 0x0001,
            Absolute = 0x0002,
            SemiAbsolute = 0x0004
        }
    }
}
