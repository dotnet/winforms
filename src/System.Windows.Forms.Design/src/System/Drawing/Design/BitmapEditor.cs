// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Drawing.Design;

/// <summary>
///  Provides an editor that can perform default file searching for bitmap (.bmp)
///  files.
/// </summary>
[CLSCompliant(false)]
public class BitmapEditor : ImageEditor
{
    protected static List<string> BitmapExtensions = ["bmp", "gif", "jpg", "jpeg", "png", "ico"];

    protected override string GetFileDialogDescription() => SR.bitmapFileDescription;

    protected override string[] GetExtensions() => [.. BitmapExtensions];

    protected override Image LoadFromStream(Stream stream) => new Bitmap(stream);
}
