// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal partial class Interop
{
    internal static partial class Shell32
    {
        [Flags]
        public enum FOS : uint
        {
            OVERWRITEPROMPT = 0x2,
            STRICTFILETYPES = 0x4,
            NOCHANGEDIR = 0x8,
            PICKFOLDERS = 0x20,
            FORCEFILESYSTEM = 0x40,     // Ensure that items returned are filesystem items.
            ALLNONSTORAGEITEMS = 0x80,  // Allow choosing items that have no storage.
            NOVALIDATE = 0x100,
            ALLOWMULTISELECT = 0x200,
            PATHMUSTEXIST = 0x800,
            FILEMUSTEXIST = 0x1000,
            CREATEPROMPT = 0x2000,
            SHAREAWARE = 0x4000,
            NOREADONLYRETURN = 0x8000,
            NOTESTFILECREATE = 0x10000,
            HIDEMRUPLACES = 0x20000,
            HIDEPINNEDPLACES = 0x40000,
            NODEREFERENCELINKS = 0x100000,
            OKBUTTONNEEDSINTERACTION = 0x200000,
            DONTADDTORECENT = 0x2000000,
            FORCESHOWHIDDEN = 0x10000000,
            DEFAULTNOMINIMODE = 0x20000000,
            FORCEPREVIEWPANEON = 0x40000000,
            SUPPORTSTREAMABLEITEMS = 0x80000000
        }
    }
}
