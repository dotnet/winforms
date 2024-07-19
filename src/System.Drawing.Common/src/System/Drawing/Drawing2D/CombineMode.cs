// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Drawing.Drawing2D;

public enum CombineMode
{
    Replace = GdiPlus.CombineMode.CombineModeReplace,
    Intersect = GdiPlus.CombineMode.CombineModeIntersect,
    Union = GdiPlus.CombineMode.CombineModeUnion,
    Xor = GdiPlus.CombineMode.CombineModeXor,
    Exclude = GdiPlus.CombineMode.CombineModeExclude,
    Complement = GdiPlus.CombineMode.CombineModeComplement
}
