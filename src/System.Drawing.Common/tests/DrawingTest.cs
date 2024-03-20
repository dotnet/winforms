// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing.Imaging;

namespace System.Drawing.Tests;

public abstract class DrawingTest
{
    private static readonly Security.Cryptography.SHA256 s_md5 = Security.Cryptography.SHA256.Create();

    protected static unsafe void ValidateBitmapContent(Bitmap bitmap, params byte[] expectedHash)
    {
        BitmapData data = bitmap.LockBits(new Rectangle(default, bitmap.Size), ImageLockMode.ReadWrite, bitmap.PixelFormat);
        try
        {
            byte[] hash = new byte[expectedHash.Length];
            if (!s_md5.TryComputeHash(
                new ReadOnlySpan<byte>((void*)data.Scan0, data.Stride * data.Height),
                hash,
                out _))
            {
                Assert.Fail("Could not compute hash.");
            }

            Assert.Equal(expectedHash, hash);
        }
        finally
        {
            bitmap.UnlockBits(data);
        }
    }
}
