// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal partial class Interop
{
    internal static partial class Ole32
    {
        [Flags]
        public enum DVASPECT : uint
        {
            CONTENT = 0x1,
            THUMBNAIL = 0x2,
            ICON = 0x4,
            DOCPRINT = 0x8,
            OPAQUE = 0x10,
            TRANSPARENT = 0x20,
        }
    }
}
