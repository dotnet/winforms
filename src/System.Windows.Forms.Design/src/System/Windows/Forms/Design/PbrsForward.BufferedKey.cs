// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Design;

internal partial class PbrsForward
{
    private readonly struct BufferedKey
    {
        public readonly Message KeyDown;
        public readonly Message KeyUp;
        public readonly Message KeyChar;

        public BufferedKey(Message keyDown, Message keyChar, Message keyUp)
        {
            KeyChar = keyChar;
            KeyDown = keyDown;
            KeyUp = keyUp;
        }
    }
}
