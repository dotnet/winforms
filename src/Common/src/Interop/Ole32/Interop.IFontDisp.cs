// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class Ole32
    {
        [ComImport]
        [Guid("BEF6E003-A874-101A-8BBA-00AA00300CAB")]
        [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
        public interface IFontDisp
        {
            string Name { get; set; }

            long Size { get; set; }

            bool Bold { get; set; }

            bool Italic { get; set; }

            bool Underline { get; set; }

            bool Strikethrough { get; set; }

            short Weight { get; set; }

            short Charset { get; set; }
        }
    }
}
