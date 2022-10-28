// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Com = Windows.Win32.System.Com;
internal static partial class Interop
{
    internal static partial class Ole32
    {
        /// <summary>
        ///  Stat flags for <see cref="Com.IStream.Interface.Stat(Com.STATSTG*, Com.STATFLAG)"/>.
        /// <see href="https://docs.microsoft.com/en-us/windows/desktop/api/wtypes/ne-wtypes-tagstatflag"/>
        /// </summary>
        public enum STATFLAG : uint
        {
            /// <summary>
            ///  Stat includes the name.
            /// </summary>
            DEFAULT = 0,

            /// <summary>
            ///  Stat doesn't include the name.
            /// </summary>
            NONAME = 1
        }
    }
}
