// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal static partial class Interop
{
    internal static partial class ComCtl32
    {
        [Flags]
        public enum HDI : uint
        {
            /// <summary>
            /// The <c>cxy</c> member is valid and specifies the item's width.
            /// </summary>
            WIDTH = 0x0001,

            /// <summary>
            /// The same as <see cref="WIDTH"/>.
            /// </summary>
            HEIGHT = WIDTH,

            /// <summary>
            /// The <c>pszText</c> and <c>cchTextMax</c> members are valid.
            /// </summary>
            TEXT = 0x0002,

            /// <summary>
            /// The <c>fmt</c> member is valid.
            /// </summary>
            FORMAT = 0x0004,

            /// <summary>
            /// The <c>lParam</c> member is valid.
            /// </summary>
            LPARAM = 0x0008,

            /// <summary>
            /// The <c>hbm</c> member is valid.
            /// </summary>
            BITMAP = 0x0010,

            /// <summary>
            /// The <c>iImage</c> member is valid and specifies the image to be displayed with the item.
            /// </summary>
            IMAGE = 0x0020,

            /// <summary>
            /// While handling the message <c>HDM_GETITEM</c>, the header control may not
            /// have all the values needed to complete the request.
            /// In this case, the control must call the application back for the values
            /// via the <c>HDN_GETDISPINFO</c> notification. If <c>DI_SETITEM</c> has been passed
            /// in the <c>HDM_GETITEM</c> message, the control will cache any values
            /// returned from <c>HDN_GETDISPINFO</c> (otherwise the values remain unset.)
            /// </summary>
            DI_SETITEM = 0x0040,

            /// <summary>
            /// The <c>iOrder</c> member is valid and specifies the item's order value.
            /// </summary>
            ORDER = 0x0080,

            /// <summary>
            /// The <c>type</c> and <c>pvFilter</c> members are valid.
            /// This is used to filter out the values specified in the type member.
            /// </summary>
            FILTER = 0x0100,

            /// <summary>
            /// The <c>state</c> member is valid.
            /// </summary>
            STATE = 0x0200,
        }
    }
}
