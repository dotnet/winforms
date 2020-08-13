// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

internal partial class Interop
{
    internal static partial class Hhctl
    {
        public unsafe struct HH_FTS_QUERYW
        {
            public const int DEFAULT_PROXIMITY = -1;

            public int cbStruct;
            public BOOL fUniCodeStrings;
            public char* pszSearchQuery;
            public int iProximity;
            public BOOL fStemmedSearch;
            public BOOL fTitleOnly;
            public BOOL fExecute;
            public char* pszWindow;
        }
    }
}
