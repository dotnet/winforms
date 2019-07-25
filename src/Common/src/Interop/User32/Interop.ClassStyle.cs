// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

internal static partial class Interop
{
    internal static partial class User32
    {
        public enum ClassStyle : uint
        {
            CS_DBLCLKS = 0x0008,
            CS_DROPSHADOW = 0x00020000,
            CS_SAVEBITS = 0x0800
        }
    }
}
