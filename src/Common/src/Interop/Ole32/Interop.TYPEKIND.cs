// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

internal static partial class Interop
{
    internal static partial class Ole32
    {
        public enum TYPEKIND : int
        {
            ENUM = 0,
            RECORD = 1,
            MODULE = 2,
            INTERFACE = 3,
            DISPATCH = 4,
            COCLASS = 5,
            ALIAS = 6,
            UNION = 7,
            MAX = 8
        }
    }
}
