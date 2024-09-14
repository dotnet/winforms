// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing.Drawing2D;

namespace System.Drawing.Tests;

#pragma warning disable SYSLIB0016 // Type or member is obsolete
public partial class Graphics_GetContextTests : DrawingTest
{
    [Fact]
    public void GetContextInfo_DefaultGraphics()
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        object info = graphics.GetContextInfo();
        Assert.IsType<object[]>(info);
        object[] infoArray = (object[])info;
        Assert.Equal(2, infoArray.Length);
        Assert.IsType<Region>(infoArray[0]);
        Assert.IsType<Matrix>(infoArray[1]);
        using Region region = (Region)infoArray[0];
        using Matrix matrix = (Matrix)infoArray[1];
        Assert.True(region.IsInfinite(graphics));
        Assert.True(matrix.IsIdentity);
    }

    [Fact]
    public void GetContextInfo_Clipping()
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        using Region initialClip = new(new Rectangle(1, 2, 9, 10));
        graphics.Clip = initialClip;

        object[] info = (object[])graphics.GetContextInfo();
        using Region region = (Region)info[0];
        using Matrix matrix = (Matrix)info[1];
        Assert.Equal(initialClip.GetBounds(graphics), region.GetBounds(graphics));
        Assert.True(matrix.IsIdentity);
    }

    [Fact]
    public void GetContextInfo_Transform()
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        using Matrix initialTransform = new();
        initialTransform.Translate(1, 2);
        graphics.Transform = initialTransform;

        object[] info = (object[])graphics.GetContextInfo();
        using Region region = (Region)info[0];
        using Matrix matrix = (Matrix)info[1];
        Assert.True(region.IsInfinite(graphics));
        Assert.Equal(initialTransform, matrix);
    }

    [Fact]
    public void GetContextInfo_ClipAndTransform()
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        using Matrix initialTransform = new();
        using Region initialClip = new(new Rectangle(1, 2, 9, 10));
        graphics.Clip = initialClip;
        initialTransform.Translate(1, 2);
        graphics.Transform = initialTransform;

        object[] info = (object[])graphics.GetContextInfo();
        using Region region = (Region)info[0];
        using Matrix matrix = (Matrix)info[1];
        Assert.Equal(new RectangleF(0, 0, 9, 10), region.GetBounds(graphics));
        Assert.Equal(initialTransform, matrix);
    }

    [Fact]
    public void GetContextInfo_TransformAndClip()
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        using Matrix initialTransform = new();
        using Region initialClip = new(new Rectangle(1, 2, 9, 10));
        initialTransform.Translate(1, 2);
        graphics.Transform = initialTransform;
        graphics.Clip = initialClip;

        object[] info = (object[])graphics.GetContextInfo();
        using Region region = (Region)info[0];
        using Matrix matrix = (Matrix)info[1];
        Assert.Equal(new RectangleF(1, 2, 9, 10), region.GetBounds(graphics));
        Assert.Equal(initialTransform, matrix);
    }

    [Fact]
    public void GetContextInfo_ClipAndTransformSaveState()
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        using Matrix initialTransform = new();
        using Region initialClip = new(new Rectangle(1, 2, 9, 10));
        graphics.Clip = initialClip;
        initialTransform.Translate(1, 2);
        graphics.Transform = initialTransform;

        GraphicsState state = graphics.Save();
        object[] info = (object[])graphics.GetContextInfo();

        using Region region = (Region)info[0];
        using Matrix matrix = (Matrix)info[1];
        initialTransform.Translate(1, 2);
        Assert.Equal(new RectangleF(0, 0, 8, 8), region.GetBounds(graphics));
        Assert.Equal(initialTransform, matrix);
    }

    [Fact]
    public void GetContextInfo_ClipAndTransformSaveAndRestoreState()
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        using Matrix initialTransform = new();
        using Region initialClip = new(new Rectangle(1, 2, 9, 10));
        graphics.Clip = initialClip;
        initialTransform.Translate(1, 2);
        graphics.Transform = initialTransform;

        GraphicsState state = graphics.Save();
        object[] info = (object[])graphics.GetContextInfo();
        graphics.Restore(state);

        using Region region = (Region)info[0];
        using Matrix matrix = (Matrix)info[1];
        initialTransform.Translate(1, 2);
        Assert.Equal(new RectangleF(0, 0, 8, 8), region.GetBounds(graphics));
        Assert.Equal(initialTransform, matrix);
    }
}
#pragma warning restore SYSLIB0016

