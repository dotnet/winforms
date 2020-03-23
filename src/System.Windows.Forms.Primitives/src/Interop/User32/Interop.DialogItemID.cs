// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

internal static partial class Interop
{
    internal static partial class User32
    {
        // The values for this enum can be found in dlgs.h.
        // Because it has many values, we only have the ones that we are currently using.
        public enum DialogItemID : uint
        {
            stc4 = 0x0443,
            cmb4 = 0x0473,
        }
    }
}
