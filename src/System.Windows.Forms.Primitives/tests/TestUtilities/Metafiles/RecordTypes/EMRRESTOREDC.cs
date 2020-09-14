// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable

using System.Runtime.InteropServices;

namespace System.Windows.Forms.Metafiles
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct EMRRESTOREDC
    {
        public EMR emr;
        public int iRelative;

        public override string ToString() => $"[{nameof(EMRRESTOREDC)}] Index: {iRelative}";
    }
}
