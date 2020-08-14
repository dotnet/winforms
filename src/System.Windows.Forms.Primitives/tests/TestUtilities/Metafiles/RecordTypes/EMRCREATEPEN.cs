// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable

using System.Runtime.InteropServices;
using static Interop;

namespace System.Windows.Forms.Metafiles
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct EMRCREATEPEN
    {
        public EMR emr;
        public uint ihPen;
        public Gdi32.LOGPEN lopn;

        public override string ToString()
            => $@"[{nameof(EMRCREATEPEN)}] Index: {ihPen} Style: {lopn.lopnStyle} Width: {lopn.lopnWidth} Color: {
                lopn.lopnColor.ToSystemColorString()}";
    }
}
