// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal static partial class Interop
{
    internal static partial class ComCtl32
    {
        [Flags]
        public enum LVCF : uint
        {
            /// <summary>
            /// The <c>fmt</c> member is valid.
            /// </summary>
            FMT = 0x0001,

            /// <summary>
            /// The <c>cx</c> member is valid.
            /// </summary>
            WIDTH = 0x0002,

            /// <summary>
            /// The <c>pszText</c> member is valid.
            /// </summary>
            TEXT = 0x0004,

            /// <summary>
            /// The <c>iSubItem</c> member is valid.
            /// </summary>
            SUBITEM = 0x0008,

            /// <summary>
            /// The <c>iImage</c> member is valid.
            /// </summary>
            IMAGE = 0x0010,

            /// <summary>
            /// The <c>iOrder </c> member is valid.
            /// </summary>
            ORDER = 0x0020,

            /// <summary>
            /// The <c>cxMin</c> member is valid.
            /// </summary>
            MINWIDTH = 0x0040,

            /// <summary>
            /// The <c>cxDefault</c> member is valid.
            /// </summary>
            DEFAULTWIDTH = 0x0080,

            /// <summary>
            /// The <c>cxIdeal</c> member is valid.
            /// </summary>
            IDEALWIDTH = 0x0100,
        }
    }
}
