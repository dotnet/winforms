// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Drawing.Imaging.Tests;

#if NET8_0_OR_GREATER
public class CachedBitmapTests
{
    [Fact]
    public void Ctor_Throws_ArgumentNullException()
    {
        using Bitmap bitmap = new(10, 10);
        using var graphics = Graphics.FromImage(bitmap);

        Assert.Throws<ArgumentNullException>(() => new CachedBitmap(bitmap, null));
        Assert.Throws<ArgumentNullException>(() => new CachedBitmap(null, graphics));
    }
}
#endif
