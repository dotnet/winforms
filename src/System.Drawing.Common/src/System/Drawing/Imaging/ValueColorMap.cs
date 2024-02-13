// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Drawing.Imaging;
#if NET9_0_OR_GREATER
/// <summary>
///  Defines a map for converting colors.
/// </summary>
/// <param name="OldColor">Specifies the existing <see cref='Color'/> to be converted.</param>
/// <param name="NewColor">Specifies the new <see cref='Color'/> to which to convert.</param>
public readonly record struct ValueColorMap(Color OldColor, Color NewColor);
#endif
