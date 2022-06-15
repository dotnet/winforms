// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable

using System.Drawing;
using System.Runtime.InteropServices;
using static Interop;

namespace System.Windows.Forms.Metafiles
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct EMRTEXT
    {
        public Point ptlReference;
        public uint nChars;
        public uint offString;          // Offset to the string
        public uint fOptions;
        public RECT rcl;
        public uint offDx;              // Offset to the inter-character spacing array.

        public unsafe ReadOnlySpan<char> GetText()
        {
            int offset = (int)offString - sizeof(EMREXTTEXTOUTW) + sizeof(EMRTEXT);
            fixed (Point* p = &ptlReference)
            {
                byte* b = (byte*)(void*)p;
                b += offset;
                return new ReadOnlySpan<char>((void*)b, (int)nChars);
            }
        }
    }
}
