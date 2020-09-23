// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using System.Numerics;
using System.Windows.Forms.Metafiles;
using Xunit;
using static System.Windows.Forms.Metafiles.DataHelpers;
using static Interop;

namespace System.Windows.Forms.Tests
{
    public partial class ControlPaintTests
    {
        [WinFormsFact]
        public void ControlPaint_DrawBorder_Solid_Rendering()
        {
            using var emf = new EmfScope();
            DeviceContextState state = new DeviceContextState(emf);

            using Graphics graphics = Graphics.FromHdc((IntPtr)emf.HDC);

            Rectangle bounds = new Rectangle(10, 10, 10, 10);
            ControlPaint.DrawBorder(graphics, bounds, Color.Blue, ButtonBorderStyle.Solid);

            emf.Validate(
                state,
                Validate.Rectangle(
                    // We match the legacy GDI+ rendering, where the right and bottom are drawn inside the bounds
                    new Rectangle(10, 10, 9, 9),
                    State.Pen(1, Color.Blue, Gdi32.PS.SOLID)));
        }

        [WinFormsFact]
        public void ControlPaint_DrawBorder_Inset_Rendering()
        {
            using var emf = new EmfScope();
            DeviceContextState state = new DeviceContextState(emf);

            using Graphics graphics = Graphics.FromHdc((IntPtr)emf.HDC);

            Rectangle bounds = new Rectangle(10, 10, 10, 10);
            ControlPaint.DrawBorder(graphics, bounds, Color.Gray, ButtonBorderStyle.Inset);

            // For whatever reason GDI+ renders as polylines scaled 16x with a 1/16th world transform applied.
            // For test readability we'll transform the points from our coordinates to the logical coordinates.
            Matrix3x2 oneSixteenth = Matrix3x2.CreateScale(0.0625f);
            Matrix3x2 times16 = Matrix3x2.CreateScale(16.0f);

            // This is the default pen style GDI+ renders polylines with
            Gdi32.PS penStyle = Gdi32.PS.SOLID | Gdi32.PS.JOIN_ROUND | Gdi32.PS.COSMETIC | Gdi32.PS.ENDCAP_FLAT
                | Gdi32.PS.JOIN_MITER | Gdi32.PS.GEOMETRIC;

            emf.Validate(
                state,
                // Top
                Validate.Polyline16(
                    bounds: null,
                    PointArray(times16, 10, 10, 19, 10),
                    State.Pen(16, ControlPaint.DarkDark(Color.Gray), penStyle),
                    State.Transform(oneSixteenth)),
                // Left
                Validate.Polyline16(
                    bounds: null,
                    PointArray(times16, 10, 10, 10, 19),
                    State.Pen(16, ControlPaint.DarkDark(Color.Gray), penStyle),
                    State.Transform(oneSixteenth)),
                // Bottom
                Validate.Polyline16(
                    bounds: null,
                    PointArray(times16, 10, 19, 19, 19),
                    State.Pen(16, ControlPaint.LightLight(Color.Gray), penStyle),
                    State.Transform(oneSixteenth)),
                // Right
                Validate.Polyline16(
                    bounds: null,
                    PointArray(times16, 19, 10, 19, 19),
                    State.Pen(16, ControlPaint.LightLight(Color.Gray), penStyle),
                    State.Transform(oneSixteenth)),
                // Top inset
                Validate.Polyline16(
                    bounds: null,
                    PointArray(times16, 11, 11, 18, 11),
                    State.Pen(16, ControlPaint.Light(Color.Gray), penStyle),
                    State.Transform(oneSixteenth)),
                // Left inset
                Validate.Polyline16(
                    bounds: null,
                    PointArray(times16, 11, 11, 11, 18),
                    State.Pen(16, ControlPaint.Light(Color.Gray), penStyle),
                    State.Transform(oneSixteenth))
                );
        }

        [WinFormsFact]
        public void ControlPaint_DrawBorder_Inset_ControlColor_Rendering()
        {
            using var emf = new EmfScope();
            DeviceContextState state = new DeviceContextState(emf);

            using Graphics graphics = Graphics.FromHdc((IntPtr)emf.HDC);

            Rectangle bounds = new Rectangle(10, 10, 10, 10);
            ControlPaint.DrawBorder(graphics, bounds, SystemColors.Control, ButtonBorderStyle.Inset);

            // For whatever reason GDI+ renders as polylines scaled 16x with a 1/16th world transform applied.
            // For test readability we'll transform the points from our coordinates to the logical coordinates.
            Matrix3x2 oneSixteenth = Matrix3x2.CreateScale(0.0625f);
            Matrix3x2 times16 = Matrix3x2.CreateScale(16.0f);

            // This is the default pen style GDI+ renders polylines with
            Gdi32.PS penStyle = Gdi32.PS.SOLID | Gdi32.PS.JOIN_ROUND | Gdi32.PS.COSMETIC | Gdi32.PS.ENDCAP_FLAT
                | Gdi32.PS.JOIN_MITER | Gdi32.PS.GEOMETRIC;

            emf.Validate(
                state,
                // Top
                Validate.Polyline16(
                    bounds: null,
                    PointArray(times16, 10, 10, 19, 10),
                    State.Pen(16, ControlPaint.DarkDark(SystemColors.Control), penStyle),
                    State.Transform(oneSixteenth)),
                // Left
                Validate.Polyline16(
                    bounds: null,
                    PointArray(times16, 10, 10, 10, 19),
                    State.Pen(16, ControlPaint.DarkDark(SystemColors.Control), penStyle),
                    State.Transform(oneSixteenth)),
                // Bottom
                Validate.Polyline16(
                    bounds: null,
                    PointArray(times16, 10, 19, 19, 19),
                    State.Pen(16, ControlPaint.LightLight(SystemColors.Control), penStyle),
                    State.Transform(oneSixteenth)),
                // Right
                Validate.Polyline16(
                    bounds: null,
                    PointArray(times16, 19, 10, 19, 19),
                    State.Pen(16, ControlPaint.LightLight(SystemColors.Control), penStyle),
                    State.Transform(oneSixteenth)),
                // Top inset
                Validate.Polyline16(
                    bounds: null,
                    PointArray(times16, 11, 11, 18, 11),
                    State.Pen(16, ControlPaint.Light(SystemColors.Control), penStyle),
                    State.Transform(oneSixteenth)),
                // Left inset
                Validate.Polyline16(
                    bounds: null,
                    PointArray(times16, 11, 11, 11, 18),
                    State.Pen(16, ControlPaint.Light(SystemColors.Control), penStyle),
                    State.Transform(oneSixteenth)),

                // The bottom/right insets are only drawn if the original color was SystemColors.Control.

                // Bottom inset
                Validate.Polyline16(
                    bounds: null,
                    PointArray(times16, 11, 18, 18, 18),
                    State.Pen(16, SystemColors.ControlLight, penStyle),
                    State.Transform(oneSixteenth)),
                // Right inset
                Validate.Polyline16(
                    bounds: null,
                    PointArray(times16, 18, 11, 18, 18),
                    State.Pen(16, SystemColors.ControlLight, penStyle),
                    State.Transform(oneSixteenth))
                );
        }

        [WinFormsFact]
        public void ControlPaint_DrawBorder_OutSet_Rendering()
        {
            using var emf = new EmfScope();
            DeviceContextState state = new DeviceContextState(emf);

            using Graphics graphics = Graphics.FromHdc((IntPtr)emf.HDC);

            Rectangle bounds = new Rectangle(10, 10, 10, 10);
            ControlPaint.DrawBorder(graphics, bounds, Color.PeachPuff, ButtonBorderStyle.Outset);

            string dump = emf.RecordsToStringWithState(state);

            // For whatever reason GDI+ renders as polylines scaled 16x with a 1/16th world transform applied.
            // For test readability we'll transform the points from our coordinates to the logical coordinates.
            Matrix3x2 oneSixteenth = Matrix3x2.CreateScale(0.0625f);
            Matrix3x2 times16 = Matrix3x2.CreateScale(16.0f);

            // This is the default pen style GDI+ renders polylines with
            Gdi32.PS penStyle = Gdi32.PS.SOLID | Gdi32.PS.JOIN_ROUND | Gdi32.PS.COSMETIC | Gdi32.PS.ENDCAP_FLAT
                | Gdi32.PS.JOIN_MITER | Gdi32.PS.GEOMETRIC;

            emf.Validate(
                state,
                // Top
                Validate.Polyline16(
                    bounds: null,
                    PointArray(times16, 10, 10, 19, 10),
                    State.Pen(16, ControlPaint.LightLight(Color.PeachPuff), penStyle),
                    State.Transform(oneSixteenth)),
                // Left
                Validate.Polyline16(
                    bounds: null,
                    PointArray(times16, 10, 10, 10, 19),
                    State.Pen(16, ControlPaint.LightLight(Color.PeachPuff), penStyle),
                    State.Transform(oneSixteenth)),
                // Bottom
                Validate.Polyline16(
                    bounds: null,
                    PointArray(times16, 10, 19, 19, 19),
                    State.Pen(16, ControlPaint.DarkDark(Color.PeachPuff), penStyle),
                    State.Transform(oneSixteenth)),
                // Right
                Validate.Polyline16(
                    bounds: null,
                    PointArray(times16, 19, 10, 19, 19),
                    State.Pen(16, ControlPaint.DarkDark(Color.PeachPuff), penStyle),
                    State.Transform(oneSixteenth)),
                // Top inset
                Validate.Polyline16(
                    bounds: null,
                    PointArray(times16, 11, 11, 18, 11),
                    State.Pen(16, Color.PeachPuff, penStyle),
                    State.Transform(oneSixteenth)),
                // Left inset
                Validate.Polyline16(
                    bounds: null,
                    PointArray(times16, 11, 11, 11, 18),
                    State.Pen(16, Color.PeachPuff, penStyle),
                    State.Transform(oneSixteenth)),
                // Bottom inset
                Validate.Polyline16(
                    bounds: null,
                    PointArray(times16, 11, 18, 18, 18),
                    State.Pen(16, ControlPaint.Dark(Color.PeachPuff), penStyle),
                    State.Transform(oneSixteenth)),
                // Right inset
                Validate.Polyline16(
                    bounds: null,
                    PointArray(times16, 18, 11, 18, 18),
                    State.Pen(16, ControlPaint.Dark(Color.PeachPuff), penStyle),
                    State.Transform(oneSixteenth))
                );
        }

        [WinFormsFact]
        public void ControlPaint_DrawBorder_OutSet_ControlColor_Rendering()
        {
            using var emf = new EmfScope();
            DeviceContextState state = new DeviceContextState(emf);

            using Graphics graphics = Graphics.FromHdc((IntPtr)emf.HDC);

            Rectangle bounds = new Rectangle(10, 10, 10, 10);
            ControlPaint.DrawBorder(graphics, bounds, SystemColors.Control, ButtonBorderStyle.Outset);

            string dump = emf.RecordsToStringWithState(state);

            // For whatever reason GDI+ renders as polylines scaled 16x with a 1/16th world transform applied.
            // For test readability we'll transform the points from our coordinates to the logical coordinates.
            Matrix3x2 oneSixteenth = Matrix3x2.CreateScale(0.0625f);
            Matrix3x2 times16 = Matrix3x2.CreateScale(16.0f);

            // This is the default pen style GDI+ renders polylines with
            Gdi32.PS penStyle = Gdi32.PS.SOLID | Gdi32.PS.JOIN_ROUND | Gdi32.PS.COSMETIC | Gdi32.PS.ENDCAP_FLAT
                | Gdi32.PS.JOIN_MITER | Gdi32.PS.GEOMETRIC;

            emf.Validate(
                state,
                // Top
                Validate.Polyline16(
                    bounds: null,
                    PointArray(times16, 10, 10, 19, 10),
                    State.Pen(16, SystemColors.ControlLightLight, penStyle),
                    State.Transform(oneSixteenth)),
                // Left
                Validate.Polyline16(
                    bounds: null,
                    PointArray(times16, 10, 10, 10, 19),
                    State.Pen(16, SystemColors.ControlLightLight, penStyle),
                    State.Transform(oneSixteenth)),
                // Bottom
                Validate.Polyline16(
                    bounds: null,
                    PointArray(times16, 10, 19, 19, 19),
                    State.Pen(16, SystemColors.ControlDarkDark, penStyle),
                    State.Transform(oneSixteenth)),
                // Right
                Validate.Polyline16(
                    bounds: null,
                    PointArray(times16, 19, 10, 19, 19),
                    State.Pen(16, SystemColors.ControlDarkDark, penStyle),
                    State.Transform(oneSixteenth)),
                // Top inset
                Validate.Polyline16(
                    bounds: null,
                    PointArray(times16, 11, 11, 18, 11),
                    State.Pen(16, SystemColors.Control, penStyle),
                    State.Transform(oneSixteenth)),
                // Left inset
                Validate.Polyline16(
                    bounds: null,
                    PointArray(times16, 11, 11, 11, 18),
                    State.Pen(16, SystemColors.Control, penStyle),
                    State.Transform(oneSixteenth)),
                // Bottom inset
                Validate.Polyline16(
                    bounds: null,
                    PointArray(times16, 11, 18, 18, 18),
                    State.Pen(16, SystemColors.ControlDark, penStyle),
                    State.Transform(oneSixteenth)),
                // Right inset
                Validate.Polyline16(
                    bounds: null,
                    PointArray(times16, 18, 11, 18, 18),
                    State.Pen(16, SystemColors.ControlDark, penStyle),
                    State.Transform(oneSixteenth))
                );
        }

        [WinFormsFact]
        public void ControlPaint_DrawBorder_Dotted_Rendering()
        {
            using var emf = new EmfScope();
            DeviceContextState state = new DeviceContextState(emf);

            using Graphics graphics = Graphics.FromHdc((IntPtr)emf.HDC);

            Rectangle bounds = new Rectangle(10, 10, 10, 10);
            ControlPaint.DrawBorder(graphics, bounds, Color.Green, ButtonBorderStyle.Dotted);

            // For whatever reason GDI+ renders as polygons scaled 16x with a 1/16th world transform applied.
            Matrix3x2 oneSixteenth = Matrix3x2.CreateScale(0.0625f);

            // This is the default pen style GDI+ renders dotted lines with
            Gdi32.PS penStyle = Gdi32.PS.SOLID | Gdi32.PS.JOIN_ROUND | Gdi32.PS.COSMETIC | Gdi32.PS.ENDCAP_FLAT
                | Gdi32.PS.JOIN_BEVEL | Gdi32.PS.GEOMETRIC;

            emf.Validate(
            state,
                Validate.PolyPolygon16(
                    new Rectangle(8, 8, 13, 13),
                    polyCount: 18,
                    State.Transform(oneSixteenth),
                    State.Pen(16, Color.Green, penStyle),
                    State.Brush(Color.Green, Gdi32.BS.SOLID)));
        }

        [WinFormsFact]
        public void ControlPaint_DrawBorder_Dashed_Rendering()
        {
            using var emf = new EmfScope();
            DeviceContextState state = new DeviceContextState(emf);

            using Graphics graphics = Graphics.FromHdc((IntPtr)emf.HDC);

            Rectangle bounds = new Rectangle(10, 10, 10, 10);
            ControlPaint.DrawBorder(graphics, bounds, Color.Pink, ButtonBorderStyle.Dashed);

            // For whatever reason GDI+ renders as polygons scaled 16x with a 1/16th world transform applied.
            Matrix3x2 oneSixteenth = Matrix3x2.CreateScale(0.0625f);

            // This is the default pen style GDI+ renders dotted lines with
            Gdi32.PS penStyle = Gdi32.PS.SOLID | Gdi32.PS.JOIN_ROUND | Gdi32.PS.COSMETIC | Gdi32.PS.ENDCAP_FLAT
                | Gdi32.PS.JOIN_BEVEL | Gdi32.PS.GEOMETRIC;

            emf.Validate(
            state,
                Validate.PolyPolygon16(
                    new Rectangle(8, 8, 13, 13),
                    polyCount: 9,
                    State.Transform(oneSixteenth),
                    State.Pen(16, Color.Pink, penStyle),
                    State.Brush(Color.Pink, Gdi32.BS.SOLID)));
        }
    }
}
