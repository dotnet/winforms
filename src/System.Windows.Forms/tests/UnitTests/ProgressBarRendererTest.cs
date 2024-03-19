// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

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
        using Bitmap bitmap1 = new(100, 100);
        using Bitmap bitmap2 = new(100, 100);
        using Graphics graphics1 = Graphics.FromImage(bitmap1);
        Rectangle rectangle1 = new(0, 0, 50, 50);

        Action act = () => ProgressBarRenderer.DrawHorizontalBar(graphics1, rectangle1);

        act.Should().NotThrow();
        bitmap1.Should().BeEquivalentTo(bitmap2);
    }

    [WinFormsFact]
    public void DrawVerticalBar_ShouldNotThrowException_WhenCalledWithValidParameters()
    {
        using Bitmap bitmap1 = new(100, 100);
        using Bitmap bitmap2 = new(100, 100);
        using Graphics graphics1 = Graphics.FromImage(bitmap1);
        Rectangle rectangle1 = new(0, 0, 50, 50);

        Action act = () => ProgressBarRenderer.DrawVerticalBar(graphics1, rectangle1);

        act.Should().NotThrow();
        bitmap1.Should().BeEquivalentTo(bitmap2);
    }

    [WinFormsFact]
    public void DrawHorizontalChunks_ShouldNotThrowException_WhenCalledWithValidParameters()
    {
        using Bitmap bitmap1 = new(100, 100);
        using Bitmap bitmap2 = new(100, 100);
        using Graphics graphics1 = Graphics.FromImage(bitmap1);
        Rectangle rectangle1 = new(0, 0, 50, 50);

        Action act = () => ProgressBarRenderer.DrawHorizontalChunks(graphics1, rectangle1);

        act.Should().NotThrow();
        bitmap1.Should().BeEquivalentTo(bitmap2);
    }

    [WinFormsFact]
    public void DrawVerticalChunks_ShouldNotThrowException_WhenCalledWithValidParameters()
    {
        using Bitmap bitmap1 = new(100, 100);
        using Bitmap bitmap2 = new(100, 100);
        using Graphics graphics1 = Graphics.FromImage(bitmap1);
        Rectangle rectangle1 = new(0, 0, 50, 50);

        Action act = () => ProgressBarRenderer.DrawVerticalChunks(graphics1, rectangle1);

        act.Should().NotThrow();
        bitmap1.Should().BeEquivalentTo(bitmap2);
    }

    [WinFormsFact]
    public void ChunkThickness_ShouldReturnExpectedValue()
    {
        // Test with VisualStyleElement.ProgressBar.Chunk.Normal
        bool isElementDefined1 = VisualStyleRenderer.IsElementDefined(VisualStyleElement.ProgressBar.Chunk.Normal);
        int actualValue1 = ProgressBarRenderer.ChunkThickness;
        int expectedValue1= 6;

        isElementDefined1.Should().BeTrue();     
        actualValue1.Should().Be(expectedValue1);

        // Test with VisualStyleElement.ProgressBar.ChunkVertical.Normal
        bool isElementDefined2 = VisualStyleRenderer.IsElementDefined(VisualStyleElement.ProgressBar.Chunk.Normal);
        int actualValue2 = ProgressBarRenderer.ChunkThickness;
        int expectedValue2 = 6;

        isElementDefined2.Should().BeTrue();
        actualValue2.Should().Be(expectedValue2);
    }

    [WinFormsFact]
    public void ChunkSpaceThickness_ShouldReturnExpectedValue_BasedOnVisualStyleElement()
    {
        // Test with VisualStyleElement.ProgressBar.Chunk.Normal
        bool isElementDefined1 = VisualStyleRenderer.IsElementDefined(VisualStyleElement.ProgressBar.Chunk.Normal);
        int actualValue1 = ProgressBarRenderer.ChunkSpaceThickness;
        int expectedValue1 = 0;

        isElementDefined1.Should().BeTrue();       
        actualValue1.Should().Be(expectedValue1);

        // Test with VisualStyleElement.ProgressBar.ChunkVertical.Normal
        bool isElementDefined2 = VisualStyleRenderer.IsElementDefined(VisualStyleElement.ProgressBar.ChunkVertical.Normal);
        int actualValue2 = ProgressBarRenderer.ChunkSpaceThickness;
        int expectedValue2 = 0;

        isElementDefined2.Should().BeTrue();       
        actualValue2.Should().Be(expectedValue2);
    }
}
