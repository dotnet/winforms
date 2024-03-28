// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

public partial class SendKeys
{
    /// <summary>
    ///  Helps us hold information about the various events we're going to journal.
    /// </summary>
    private readonly struct SKEvent
    {
        public readonly MessageId WM;
        public readonly uint ParamL;
        public readonly uint ParamH;
        public readonly HWND HWND;

        public SKEvent(MessageId wm, uint paramL, bool paramH, HWND hwnd)
        {
            WM = wm;
            ParamL = paramL;
            ParamH = paramH ? 1u : 0;
            HWND = hwnd;
        }

        public SKEvent(MessageId wm, uint paramL, uint paramH, HWND hwnd)
        {
            WM = wm;
            ParamL = paramL;
            ParamH = paramH;
            HWND = hwnd;
        }
    }
}
