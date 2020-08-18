// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal partial class Interop
{
    internal static partial class Richedit
    {
        [Flags]
        public enum GT : uint
        {
            DEFAULT         = 0,
            USECRLF         = 1,
            SELECTION       = 2,
            RAWTEXT         = 4,
            NOHIDDENTEXT    = 8
        }
    }
}
