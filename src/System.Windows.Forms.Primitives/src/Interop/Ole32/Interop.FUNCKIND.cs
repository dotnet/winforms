// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

internal static partial class Interop
{
    internal static partial class Ole32
    {
        public enum FUNCKIND : int
        {
            VIRTUAL = 0,
            PUREVIRTUAL = 1,
            NONVIRTUAL = 2,
            STATIC = 3,
            DISPATCH = 4
        }
    }
}
