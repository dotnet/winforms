// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal static partial class Interop
{
    internal static partial class ComCtl32
    {
        [Flags]
        public enum LVIF : uint
        {
            /// <summary>
            /// The <c>pszText</c> member is valid or must be set.
            /// </summary>
            TEXT = 0x00000001,

            /// <summary>
            /// The <c>iImage</c> member is valid or must be set.
            /// </summary>
            IMAGE = 0x00000002,

            /// <summary>
            /// The <c>lParam</c> member is valid or must be set.
            /// </summary>
            PARAM = 0x00000004,

            /// <summary>
            /// The <c>state</c> member is valid or must be set.
            /// </summary>
            STATE = 0x00000008,

            /// <summary>
            /// The <c>iIndent</c> member is valid or must be set.
            /// </summary>
            INDENT = 0x00000010,

            /// <summary>
            /// The control will not generate <see cref="LVN.GETDISPINFOW"/>
            /// to retrieve text information if it receives an <see cref="LVM.GETITEMW"/> message.
            /// Instead, the <c>pszText</c> member will contain <c>LPSTR_TEXTCALLBACK</c>.
            /// </summary>
            NORECOMPUTE = 0x00000800,

            /// <summary>
            /// The <c>iGroupId</c> member is valid or must be set.
            /// If this flag is not set when an <see cref="LVM.INSERTITEMW"/> message is sent,
            /// the value of <c>iGroupId</c> is assumed to be <c>I_GROUPIDCALLBACK</c>.
            /// </summary>
            GROUPID = 0x00000100,

            /// <summary>
            /// The <c>cColumns</c> member is valid or must be set.
            /// </summary>
            COLUMNS = 0x00000200,

            /// <summary>
            /// The operating system should store the requested list item information
            /// and not ask for it again. This flag is used only with the
            /// <see cref="LVN.GETDISPINFOW"/> notification code.
            /// </summary>
            DI_SETITEM = 0x00001000,

            /// <summary>
            /// The <c>piColFmt</c> member is valid or must be set.
            /// If this flag is used, the <c>cColumns</c> member is valid or must be set.
            /// </summary>
            COLFMT = 0x00010000,
        }
    }
}
