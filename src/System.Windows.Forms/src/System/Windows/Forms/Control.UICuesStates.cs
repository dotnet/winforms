// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    public partial class Control
    {
        [Flags]
        private enum UICuesStates
        {
            FocusMask = 0x000F,
            FocusHidden = 0x0001,
            FocusShow = 0x0002,
            KeyboardMask = 0x00F0,
            KeyboardHidden = 0x0010,
            KeyboardShow = 0x0020
        }
    }
}
