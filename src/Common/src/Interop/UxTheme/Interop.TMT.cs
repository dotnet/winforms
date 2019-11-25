// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal static partial class Interop
{
    public static partial class UxTheme
    {
        [Flags]
        public enum TMT : uint
        {
            // This enum has many values, so we only add the ones that we are currently using.
            FLATMENUS = 1001,
            MINCOLORDEPTH = 1301
        }
    }
}
