// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using System.Runtime.InteropServices;

namespace System.Windows.Forms.Metafiles;

[StructLayout(LayoutKind.Sequential)]
internal struct EMRCREATEPEN
{
    public EMR emr;
    public uint ihPen;
    public LOGPEN lopn;

    public override string ToString()
        => $@"[{nameof(EMRCREATEPEN)}] Index: {ihPen} Style: {lopn.lopnStyle} Width: {lopn.lopnWidth} Color: {lopn.lopnColor.ToSystemColorString()}";
}
