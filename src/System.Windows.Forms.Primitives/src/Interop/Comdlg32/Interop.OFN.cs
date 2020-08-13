// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal partial class Interop
{
    internal partial class Comdlg32
    {
        [Flags]
        public enum OFN
        {
            READONLY = 0x00000001,
            OVERWRITEPROMPT = 0x00000002,
            HIDEREADONLY = 0x00000004,
            NOCHANGEDIR = 0x00000008,
            SHOWHELP = 0x00000010,
            ENABLEHOOK = 0x00000020,
            ENABLETEMPLATE = 0x00000040,
            ENABLETEMPLATEHANDLE = 0x00000080,
            NOVALIDATE = 0x00000100,
            ALLOWMULTISELECT = 0x00000200,
            EXTENSIONDIFFERENT = 0x00000400,
            PATHMUSTEXIST = 0x00000800,
            FILEMUSTEXIST = 0x00001000,
            CREATEPROMPT = 0x00002000,
            SHAREAWARE = 0x00004000,
            NOREADONLYRETURN = 0x00008000,
            NOTESTFILECREATE = 0x00010000,
            NONETWORKBUTTON = 0x00020000,
            NOLONGNAMES = 0x00040000,
            EXPLORER = 0x00080000,
            NODEREFERENCELINKS = 0x00100000,
            LONGNAMES = 0x00200000,
            ENABLEINCLUDENOTIFY = 0x00400000,
            ENABLESIZING = 0x00800000,
            DONTADDTORECENT = 0x02000000,
            FORCESHOWHIDDEN = 0x10000000
        }
    }
}
