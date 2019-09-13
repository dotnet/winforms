// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using static Interop;

namespace System.Windows.Forms
{
    [Flags]
    public enum GetChildAtPointSkip
    {
        None = (int)User32.CWP.ALL,
        Invisible = (int)User32.CWP.SKIPINVISIBLE,
        Disabled = (int)User32.CWP.SKIPDISABLED,
        Transparent = (int)User32.CWP.SKIPTRANSPARENT
    }
}
