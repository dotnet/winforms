// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

internal static partial class Interop
{
    internal static partial class Ole32
    {
        /// <summary>
        ///  Picture type used with <see cref="IPicture"/> and <see cref="PICTDESC"/>.
        ///  <see href="https://docs.microsoft.com/windows/win32/com/pictype-constants"/>
        /// </summary>
        /// <remarks>
        ///  This enum is explicitly set as int to try and avoid sign-extension
        ///  errors when converting between type sizes (as it's usage is different
        ///  between IPicture and PICTDESC). We use the larger type here to allow
        ///  the PICTDESC struct to directly use this and remain blittable.
        /// </remarks>
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
