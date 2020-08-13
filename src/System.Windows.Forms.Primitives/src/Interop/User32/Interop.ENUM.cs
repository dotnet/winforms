// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal static partial class Interop
{
    internal static partial class User32
    {
        public enum ENUM : uint
        {
            CURRENT_SETTINGS = unchecked((uint)(-1)),
            REGISTRY_SETTINGS = unchecked((uint)(-2)),
        }
    }
}
