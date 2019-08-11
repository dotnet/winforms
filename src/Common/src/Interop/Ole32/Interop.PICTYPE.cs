// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

internal static partial class Interop
{
    internal static partial class Ole32
    {
        /// <summary>
        ///  Picture type used with <see cref="IPicture"/> and
        ///  <see href="https://docs.microsoft.com/en-us/windows/win32/com/pictype-constants"/>
        /// </summary>
        public enum PICTYPE : int
        {
            UNINITIALIZED = -1,
            NONE          = 0,
            BITMAP        = 1,
            METAFILE      = 2,
            ICON          = 3,
            ENHMETAFILE   = 4
        }
    }
}
