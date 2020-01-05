// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

internal static partial class Interop
{
    internal static partial class Ole32
    {
        public enum OLEIVERB : int
        {
            PRIMARY = 0,
            SHOW = -1,
            OPEN = -2,
            HIDE = -3,
            UIACTIVATE = -4,
            INPLACEACTIVATE = -5,
            DISCARDUNDOSTATE = -6,
            PROPERTIES = -7,
        }
    }
}
