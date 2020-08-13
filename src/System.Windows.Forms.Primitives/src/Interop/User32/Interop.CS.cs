// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal static partial class Interop
{
    internal static partial class User32
    {
        /// <summary>
        ///  Class styles for <see cref="WNDCLASS"/>
        /// </summary>
        [Flags]
        public enum CS : uint
        {
            VREDRAW = 0x0001,
            HREDRAW = 0x0002,
            DBLCLKS = 0x0008,
            DROPSHADOW = 0x00020000,
            SAVEBITS = 0x0800
        }
    }
}
