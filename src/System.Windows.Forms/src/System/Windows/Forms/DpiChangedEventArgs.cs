// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing;

namespace System.Windows.Forms;

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
        SuggestedRectangle = *(RECT*)(nint)m.LParamInternal;
    }

    public int DeviceDpiOld { get; }

    public int DeviceDpiNew { get; }

    public Rectangle SuggestedRectangle { get; }

    public override string ToString() => $"was: {DeviceDpiOld}, now: {DeviceDpiNew}";
}
