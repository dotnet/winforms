// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;

internal static partial class Interop
{
    internal static partial class User32
    {
        public struct WINDOWPLACEMENT
        {
            public uint length;
            public WPF flags;
            public SW showCmd;
            public Point ptMinPosition;
            public Point ptMaxPosition;
            public RECT rcNormalPosition;
            public RECT rcDevice;
        }
    }
}
