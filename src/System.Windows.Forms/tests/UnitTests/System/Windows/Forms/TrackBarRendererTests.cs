// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using System.Drawing;
using System.Windows.Forms.VisualStyles;

namespace System.Windows.Forms.Tests;

public class TrackBarRendererTests : IDisposable
{
    private readonly Bitmap _bitmap;
    private readonly Graphics _graphics;

    public TrackBarRendererTests()
    {
        _bitmap = new(100, 100);
        _graphics = Graphics.FromImage(_bitmap);
    }

    public void Dispose()
    {
        _graphics.Dispose();
        _bitmap.Dispose();
    }

    [WinFormsFact]
    public void IsSupported_ReturnsExpected()
    {
        bool isSupported = TrackBarRenderer.IsSupported;
        isSupported.Should().Be(VisualStyleRenderer.IsSupported);
    }

    [WinFormsFact]
    public void DrawTrack_ValidParameters_DoesNotThrow()
    {
        void TestDrawTrack(Action<Graphics, Rectangle> drawTrack, Rectangle bounds)
        {
            Action a = () => drawTrack(_graphics, bounds);
            a.Should().NotThrow();
        }

        TestDrawTrack(TrackBarRenderer.DrawHorizontalTrack, new Rectangle(0, 0, 100, 10));
        TestDrawTrack(TrackBarRenderer.DrawVerticalTrack, new Rectangle(0, 0, 10, 100));
    }

    [WinFormsFact]
    public void DrawTrack_InvalidParameters_DoesNotThrow()
    {
        void TestDrawTrackWithInvalidBounds(Action<Graphics, Rectangle> drawTrack, Rectangle[] invalidBounds)
        {
            foreach (Rectangle bounds in invalidBounds)
            {
                Action a = () => drawTrack(_graphics, bounds);
                a.Should().NotThrow();
            }
        }

        var invalidBounds = new[]
        {
            new Rectangle(0, 0, 0, 10),
            new Rectangle(0, 0, 100, 0),
            new Rectangle(0, 0, -100, 10),
            new Rectangle(0, 0, 100, -10)
        };

        TestDrawTrackWithInvalidBounds(TrackBarRenderer.DrawHorizontalTrack, invalidBounds);
        TestDrawTrackWithInvalidBounds(TrackBarRenderer.DrawVerticalTrack, invalidBounds);
    }

    [WinFormsFact]
    public void DrawThumb_ValidParameters_DoesNotThrow()
    {
        void TestDrawThumb(Action<Graphics, Rectangle, TrackBarThumbState> drawThumb, Rectangle bounds, TrackBarThumbState state)
        {
            Action a = () => drawThumb(_graphics, bounds, state);
            a.Should().NotThrow();
        }

        Rectangle bounds = new(0, 0, 10, 10);
        TrackBarThumbState state = TrackBarThumbState.Normal;

        TestDrawThumb(TrackBarRenderer.DrawHorizontalThumb, bounds, state);
        TestDrawThumb(TrackBarRenderer.DrawVerticalThumb, bounds, state);
    }

    [WinFormsFact]
    public void GetPointingThumbSize_ValidParameters_ReturnsExpected()
    {
        void TestGetThumbSize(Func<Graphics, TrackBarThumbState, Size> getThumbSize)
        {
            Size size = getThumbSize(_graphics, TrackBarThumbState.Normal);
            size.Should().NotBe(Size.Empty);
        }

        TestGetThumbSize(TrackBarRenderer.GetLeftPointingThumbSize);
        TestGetThumbSize(TrackBarRenderer.GetRightPointingThumbSize);
        TestGetThumbSize(TrackBarRenderer.GetTopPointingThumbSize);
        TestGetThumbSize(TrackBarRenderer.GetBottomPointingThumbSize);
    }

    [WinFormsFact]
    public void DrawTicks_ValidParameters_DoesNotThrow()
    {
        void TestDrawTicks(Action<Graphics, Rectangle, int, EdgeStyle> drawTicks, Rectangle bounds, int tickCount, EdgeStyle edgeStyle)
        {
            Action a = () => drawTicks(_graphics, bounds, tickCount, edgeStyle);
            a.Should().NotThrow();
        }

        Rectangle horizontalBounds = new(0, 0, 100, 10);
        Rectangle verticalBounds = new(0, 0, 10, 100);
        int tickCount = 5;
        EdgeStyle edgeStyle = EdgeStyle.Raised;

        TestDrawTicks(TrackBarRenderer.DrawHorizontalTicks, horizontalBounds, tickCount, edgeStyle);
        TestDrawTicks(TrackBarRenderer.DrawVerticalTicks, verticalBounds, tickCount, edgeStyle);
    }

    [WinFormsFact]
    public void DrawTicks_InvalidParameters_DoesNotThrow()
    {
        void TestDrawTicksWithInvalidParameters(Action<Graphics, Rectangle, int, EdgeStyle> drawTicks, Rectangle bounds, int[] invalidTickCounts, Rectangle[] invalidBounds)
        {
            foreach (int count in invalidTickCounts)
            {
                Action a = () => drawTicks(_graphics, bounds, count, EdgeStyle.Raised);
                a.Should().NotThrow();
            }

            foreach (Rectangle invalidBound in invalidBounds)
            {
                Action a = () => drawTicks(_graphics, invalidBound, 5, EdgeStyle.Raised);
                a.Should().NotThrow();
            }
        }

        Rectangle Bounds = new(0, 0, 100, 10);
        int[] invalidTickCounts = [0, -1];
        Rectangle[] invalidBounds =
        [
            new(0, 0, 0, 10),
            new(0, 0, 100, 0)
        ];

        TestDrawTicksWithInvalidParameters(TrackBarRenderer.DrawHorizontalTicks, Bounds, invalidTickCounts, invalidBounds);
        TestDrawTicksWithInvalidParameters(TrackBarRenderer.DrawVerticalTicks, Bounds, invalidTickCounts, invalidBounds);
    }

    [WinFormsFact]
    public void DrawPointingThumb_ValidParameters_DoesNotThrow()
    {
        Rectangle bounds = new(0, 0, 10, 10);
        var drawThumbActions = new Action<Graphics, Rectangle, TrackBarThumbState>[]
        {
            TrackBarRenderer.DrawLeftPointingThumb,
            TrackBarRenderer.DrawRightPointingThumb,
            TrackBarRenderer.DrawTopPointingThumb,
            TrackBarRenderer.DrawBottomPointingThumb
        };

        foreach (Action<Graphics, Rectangle, TrackBarThumbState> drawThumb in drawThumbActions)
        {
            Action a = () => drawThumb(_graphics, bounds, TrackBarThumbState.Normal);
            a.Should().NotThrow();
        }
    }
}
