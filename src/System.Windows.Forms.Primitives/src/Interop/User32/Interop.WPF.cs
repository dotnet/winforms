// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal static partial class Interop
{
    internal static partial class User32
    {
        /// <summary>
        ///  Window placement flags for <see cref="WINDOWPLACEMENT"/>.
        /// </summary>
        [Flags]
        public enum WPF : uint
        {
            SETMINPOSITION = 0x0001,
            RESTORETOMAXIMIZED = 0x0002,
            ASYNCWINDOWPLACEMENT = 0x0004,
        }
    }
}
