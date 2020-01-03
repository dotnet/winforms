// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class User32
    {
        public enum ODT : uint
        {
            MENU = 1,
            LISTBOX = 2,
            COMBOBOX = 3,
            BUTTON = 4,
            STATIC = 5,
            HEADER = 100,
            TAB = 101,
            LISTVIEW = 102
        }
    }
}
