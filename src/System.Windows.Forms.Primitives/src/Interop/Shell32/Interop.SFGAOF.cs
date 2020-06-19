// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal partial class Interop
{
    internal static partial class Shell32
    {
        [Flags]
        public enum SFGAOF : uint
        {
            CANCOPY = 0x00000001,
            CANMOVE = 0x00000002,
            CANLINK = 0x00000004,
            STORAGE = 0x00000008,
            CANRENAME = 0x00000010,
            CANDELETE = 0x00000020,
            HASPROPSHEET = 0x00000040,
            DROPTARGET = 0x00000100,
            CAPABILITYMASK = 0x00000177,
            SYSTEM = 0x00001000,
            ENCRYPTED = 0x00002000,
            ISSLOW = 0x00004000,
            GHOSTED = 0x00008000,
            LINK = 0x00010000,
            SHARE = 0x00020000,
            READONLY = 0x00040000,
            HIDDEN = 0x00080000,
            DISPLAYATTRMASK = 0x000FC000,
            FILESYSANCESTOR = 0x10000000,
            FOLDER = 0x20000000,
            FILESYSTEM = 0x40000000,
            HASSUBFOLDER = 0x80000000,
            CONTENTSMASK = 0x80000000,
            VALIDATE = 0x01000000,
            REMOVABLE = 0x02000000,
            COMPRESSED = 0x04000000,
            BROWSABLE = 0x08000000,
            NONENUMERATED = 0x00100000,
            NEWCONTENT = 0x00200000,
            CANMONIKER = 0x00400000,
            HASSTORAGE = 0x00400000,
            STREAM = 0x00400000,
            STORAGEANCESTOR = 0x00800000,
            STORAGECAPMASK = 0x70C50008,
            PKEYSFGAOMASK = 0x81044000
        }
    }
}
