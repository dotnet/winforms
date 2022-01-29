// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

internal partial class Interop
{
    internal static partial class Shell32
    {
        public enum DROPIMAGETYPE
        {
            DROPIMAGE_INVALID = -1,
            DROPIMAGE_NONE = 0,
            DROPIMAGE_COPY = 1,
            DROPIMAGE_MOVE = 2,
            DROPIMAGE_LINK = 4,
            DROPIMAGE_LABEL = 6,
            DROPIMAGE_WARNING = 7,
            DROPIMAGE_NOIMAGE = 8
        }
    }
}
