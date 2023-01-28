// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Provides information about a DpiChanged event.
    /// </summary>
    public sealed class DpiChangedEventArgs : CancelEventArgs
    {
        /// <summary>
        ///  Parameter units are pixels(dots) per inch.
        /// </summary>
        internal unsafe DpiChangedEventArgs(int old, Message m)
        {
            DeviceDpiOld = old;
            DeviceDpiNew = (short)m.WParamInternal.LOWORD;
            Debug.Assert((short)m.WParamInternal.HIWORD == DeviceDpiNew, "Non-square pixels!");
            RECT suggestedRect = *(RECT*)(nint)m.LParamInternal;
            SuggestedRectangle = Rectangle.FromLTRB(suggestedRect.left, suggestedRect.top, suggestedRect.right, suggestedRect.bottom);
        }

        public int DeviceDpiOld { get; }

        public int DeviceDpiNew { get; }

        public Rectangle SuggestedRectangle { get; }

        public override string ToString() => $"was: {DeviceDpiOld}, now: {DeviceDpiNew}";
    }
}
