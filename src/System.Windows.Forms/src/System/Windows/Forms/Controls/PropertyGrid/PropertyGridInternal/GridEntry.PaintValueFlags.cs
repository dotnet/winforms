// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.PropertyGridInternal;

internal abstract partial class GridEntry
{
    [Flags]
    internal enum PaintValueFlags
    {
        None                    = 0x0,
        DrawSelected            = 0x1,
        CheckShouldSerialize    = 0x4,
        PaintInPlace            = 0x8
    }
}
