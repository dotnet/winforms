// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class User32
    {
        public enum ItemControlType : uint
        {
            ODT_MENU = 1,
            ODT_LISTBOX = 2,
            ODT_COMBOBOX = 3,
            ODT_BUTTON = 4,
            ODT_STATIC = 5,
        }
    }
}
