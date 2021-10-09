// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using static Interop;

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
            DeviceDpiNew = PARAM.SignedLOWORD(m.WParamInternal);
            Debug.Assert(PARAM.SignedHIWORD(m.WParamInternal) == DeviceDpiNew, "Non-square pixels!");
            RECT suggestedRect = *(RECT*)m.LParamInternal;
            SuggestedRectangle = Rectangle.FromLTRB(suggestedRect.left, suggestedRect.top, suggestedRect.right, suggestedRect.bottom);
        }

        public int DeviceDpiOld { get; }

        public int DeviceDpiNew { get; }

        public Rectangle SuggestedRectangle { get; }

        public override string ToString() => $"was: {DeviceDpiOld}, now: {DeviceDpiNew}";
    }
}
