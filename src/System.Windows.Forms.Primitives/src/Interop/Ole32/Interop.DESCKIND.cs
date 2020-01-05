// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

internal static partial class Interop
{
    internal static partial class Ole32
    {
        public enum DESCKIND : int
        {
            NONE = 0,
            FUNCDESC = 1,
            VARDESC = 2,
            TYPECOMP = 3,
            IMPLICITAPPOBJ = 4,
            MAX = 5
        }
    }
}
