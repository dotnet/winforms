// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

internal partial class Interop
{
    internal static partial class Hhctl
    {
        public unsafe struct HH_ALINKW
        {
            public int cbStruct;
            public BOOL fReserved;
            public char* pszKeywords;
            public char* pszUrl;
            public char* pszMsgText;
            public char* pszMsgTitle;
            public char* pszWindow;
            public BOOL fIndexOnFail;
        }
    }
}
