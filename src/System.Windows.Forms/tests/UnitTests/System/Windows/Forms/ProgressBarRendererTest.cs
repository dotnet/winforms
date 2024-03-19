// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Windows.Forms.VisualStyles;

namespace System.Windows.Forms.Tests;

public class ProgressBarRendererTest
{
    [WinFormsFact]
    public void IsSupported_ShouldReturnExpectedValue()
    {
        bool expectedValue = VisualStyleRenderer.IsSupported;
        bool actualValue = ProgressBarRenderer.IsSupported;

        actualValue.Should().Be(expectedValue);
    }

    [WinFormsFact]
    public void DrawHorizontalBar_ShouldNotThrowException_WhenCalledWithValidParameters()
    {
        TestProgressBarRenderer(ProgressBarRenderer.DrawHorizontalBar);
    }

    [WinFormsFact]
    public void DrawVerticalBar_ShouldNotThrowException_WhenCalledWithValidParameters()
    {
        TestProgressBarRenderer(ProgressBarRenderer.DrawVerticalBar);
    }

    [WinFormsFact]
    public void DrawHorizontalChunks_ShouldNotThrowException_WhenCalledWithValidParameters()
    {
        TestProgressBarRenderer(ProgressBarRenderer.DrawHorizontalChunks);
    }

    [WinFormsFact]
    public void DrawVerticalChunks_ShouldNotThrowException_WhenCalledWithValidParameters()
    {
        TestProgressBarRenderer(ProgressBarRenderer.DrawVerticalChunks);
    }

    [WinFormsFact]
    public void ChunkThickness_ShouldReturnExpectedValue()
    {
        // Test with VisualStyleElement.ProgressBar.Chunk.Normal
        TestChunkProperty(() => ProgressBarRenderer.ChunkThickness, VisualStyleElement.ProgressBar.Chunk.Normal, 6);

        // Test with VisualStyleElement.ProgressBar.ChunkVertical.Normal
        TestChunkProperty(() => ProgressBarRenderer.ChunkThickness, VisualStyleElement.ProgressBar.ChunkVertical.Normal, 6);
    }

    [WinFormsFact]
    public void ChunkSpaceThickness_ShouldReturnExpectedValue_BasedOnVisualStyleElement()
    {
        // Test with VisualStyleElement.ProgressBar.Chunk.Normal
        TestChunkProperty(() => ProgressBarRenderer.ChunkSpaceThickness, VisualStyleElement.ProgressBar.Chunk.Normal, 0);

        // Test with VisualStyleElement.ProgressBar.ChunkVertical.Normal
        TestChunkProperty(() => ProgressBarRenderer.ChunkSpaceThickness, VisualStyleElement.ProgressBar.ChunkVertical.Normal, 0);
    }

    private void TestProgressBarRenderer(Action<Graphics, Rectangle> drawAction)
    {
        using Bitmap bitmap1 = new(100, 100);
        using Bitmap bitmap2 = new(100, 100);
        using Graphics graphics1 = Graphics.FromImage(bitmap1);
        Rectangle rectangle1 = new(0, 0, 50, 50);

        Action act = () => drawAction(graphics1, rectangle1);

        act.Should().NotThrow();
        bitmap1.Should().BeEquivalentTo(bitmap2);
    }

    private void TestChunkProperty(Func<int> propertyGetter, VisualStyleElement styleElement, int expectedValue)
    {
        bool isElementDefined = VisualStyleRenderer.IsElementDefined(styleElement);
        int actualValue = propertyGetter();

        isElementDefined.Should().BeTrue();
        actualValue.Should().Be(expectedValue);
    }
}
