// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.UI.Input.KeyboardAndMouse;

namespace System;

internal static class VirtualKeyUtilities
{
    public static bool IsExtendedKey(VIRTUAL_KEY vk)
    {
        // From the SDK:
        // The extended-key flag indicates whether the keystroke message originated from one of
        // the additional keys on the enhanced keyboard. The extended keys consist of the ALT and
        // CTRL keys on the right-hand side of the keyboard; the INS, DEL, HOME, END, PAGE UP,
        // PAGE DOWN, and arrow keys in the clusters to the left of the numeric keypad; the NUM LOCK
        // key; the BREAK (CTRL+PAUSE) key; the PRINT SCRN key; and the divide (/) and ENTER keys in
        // the numeric keypad. The extended-key flag is set if the key is an extended key.
        //
        // - docs appear to be incorrect. Use of Spy++ indicates that break is not an extended key.
        // Also, menu key and windows keys also appear to be extended.
        return vk is VIRTUAL_KEY.VK_RMENU or
            VIRTUAL_KEY.VK_RCONTROL or
            VIRTUAL_KEY.VK_NUMLOCK or
            VIRTUAL_KEY.VK_INSERT or
            VIRTUAL_KEY.VK_DELETE or
            VIRTUAL_KEY.VK_HOME or
            VIRTUAL_KEY.VK_END or
            VIRTUAL_KEY.VK_PRIOR or
            VIRTUAL_KEY.VK_NEXT or
            VIRTUAL_KEY.VK_UP or
            VIRTUAL_KEY.VK_DOWN or
            VIRTUAL_KEY.VK_LEFT or
            VIRTUAL_KEY.VK_RIGHT or
            VIRTUAL_KEY.VK_APPS or
            VIRTUAL_KEY.VK_RWIN or
            VIRTUAL_KEY.VK_LWIN;
        // Note that there are no distinct values for the following keys:
        // numpad divide
        // numpad enter
    }
}
