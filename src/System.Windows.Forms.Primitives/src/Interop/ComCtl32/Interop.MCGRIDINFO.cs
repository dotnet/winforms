﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class ComCtl32
    {
        /// <summary>
        /// MonthCalendar grid info structure.
        /// Copied form CommCtrl.h
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public unsafe struct MCGRIDINFO
        {
            public uint cbSize;
            public MCGRIDINFO_PART dwPart;
            public MCGRIDINFO_FLAGS dwFlags;
            public int iCalendar;
            public int iRow;
            public int iCol;
            public BOOL bSelected;
            public SYSTEMTIME stStart;
            public SYSTEMTIME stEnd;
            public RECT rc;
            public char* pszName;
            public UIntPtr cchName;
        }
    }
}
