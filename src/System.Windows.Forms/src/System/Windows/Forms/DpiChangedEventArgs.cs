// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;

namespace System.Windows.Forms
{
    /// <devdoc>
    /// Provides information about a DpiChanged event.
    /// </devdoc>
    public sealed class DpiChangedEventArgs : CancelEventArgs
    {
        /// <devdov>
        /// Parameter units are pixels(dots) per inch.
        /// </devdoc>
        internal DpiChangedEventArgs(int old, Message m)
        {
            DeviceDpiOld = old;
            DeviceDpiNew = NativeMethods.Util.SignedLOWORD(m.WParam);
            Debug.Assert(NativeMethods.Util.SignedHIWORD(m.WParam) == DeviceDpiNew, "Non-square pixels!");
            ref readonly NativeMethods.RECT suggestedRect = ref m.GetLParamRef<NativeMethods.RECT>();
            SuggestedRectangle = Rectangle.FromLTRB(suggestedRect.left, suggestedRect.top, suggestedRect.right, suggestedRect.bottom);
        }

        public int DeviceDpiOld { get; }

        public int DeviceDpiNew { get; }

        public Rectangle SuggestedRectangle { get; }

        public override string ToString() => $"was: {DeviceDpiOld}, now: {DeviceDpiNew}";
    }
}
