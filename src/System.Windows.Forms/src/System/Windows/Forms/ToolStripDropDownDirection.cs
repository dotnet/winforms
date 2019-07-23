// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    public enum ToolStripDropDownDirection
    {
        // Dir L=0
        AboveLeft = 0x0000, // 0 0 0   Above =00
        AboveRight = 0x0001, // 0 0 1   Below =01
        BelowLeft = 0x0002, // 0 1 0   Side  =10
        BelowRight = 0x0003, // 0 1 1   Last bit indicates left or right.
        Left = 0x0004, // 1 0 0
        Right = 0x0005, // 1 0 1
        Default = 0x0007  // 1 1 1
    }
}
