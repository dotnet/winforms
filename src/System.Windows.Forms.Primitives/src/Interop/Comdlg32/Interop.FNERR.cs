// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

internal partial class Interop
{
    internal partial class Comdlg32
    {
        public enum FNERR : uint
        {
            SUBCLASSFAILURE = 0x3001,
            INVALIDFILENAME = 0x3002,
            BUFFERTOOSMALL = 0x3003
        }
    }
}
