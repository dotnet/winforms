// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable

using System.Drawing;
using Xunit;
using static Interop;

namespace System.Windows.Forms.Metafiles
{
    internal sealed class LineToValidator : IEmfValidator
    {
        private readonly Flags _validate;
        private readonly Point _from;
        private readonly Point _to;
        private readonly int _penWidth;
        private readonly Color _penColor;
        private readonly Gdi32.PS _penStyle;
        private readonly Gdi32.R2 _rop2;
        private readonly Gdi32.BKMODE _backgroundMode;

        public LineToValidator(
            Point from,
            Point to,
            Color penColor,
            int penWidth = 1,
            Gdi32.PS penStyle = default,
            Gdi32.R2 rop2 = Gdi32.R2.COPYPEN,
            Gdi32.BKMODE backgroundMode = Gdi32.BKMODE.TRANSPARENT,
            Flags validate = default)
        {
            _from = from;
            _to = to;
            _penWidth = penWidth;
            _penColor = penColor;
            _penStyle = penStyle;
            _rop2 = rop2;
            _backgroundMode = backgroundMode;

            if (validate != default)
            {
                _validate = validate;
                return;
            }

            // Default values for all of these are valid expectations so we always turn them on.
            _validate = Flags.From | Flags.To | Flags.PenStyle;

            if (penWidth != 0)
            {
                _validate |= Flags.PenWidth;
            }

            if (!penColor.IsEmpty)
            {
                _validate |= Flags.PenColor;
            }

            if (backgroundMode != default)
            {
                _validate |= Flags.BackgroundMode;
            }

            if (_rop2 != default)
            {
                _validate |= Flags.RopMode;
            }
        }

        public bool ShouldValidate(Gdi32.EMR recordType) => recordType == Gdi32.EMR.LINETO;

        public unsafe void Validate(ref EmfRecord record, DeviceContextState state, out bool complete)
        {
            // We're only checking one TextOut record, so this call completes our work.
            complete = true;

            EMRPOINTRECORD* lineTo = record.LineToRecord;

            if (_validate.HasFlag(Flags.From))
            {
                Assert.Equal(_from, state.BrushOrigin);
            }

            if (_validate.HasFlag(Flags.To))
            {
                Assert.Equal(_to, lineTo->point);
            }

            if (_validate.HasFlag(Flags.PenColor))
            {
                Assert.Equal((COLORREF)_penColor, state.SelectedPen.lopnColor);
            }

            if (_validate.HasFlag(Flags.PenWidth))
            {
                Assert.Equal(_penWidth, state.SelectedPen.lopnWidth.X);
            }

            if (_validate.HasFlag(Flags.PenStyle))
            {
                Assert.Equal(_penStyle, state.SelectedPen.lopnStyle);
            }

            if (_validate.HasFlag(Flags.BackgroundMode))
            {
                Assert.Equal(_backgroundMode, state.BackgroundMode);
            }

            if (_validate.HasFlag(Flags.RopMode))
            {
                Assert.Equal(_rop2, state.Rop2Mode);
            }
        }

        [Flags]
        public enum Flags : uint
        {
            From            = 0b00000000_00000000_00000000_00000001,
            To              = 0b00000000_00000000_00000000_00000010,
            PenColor        = 0b00000000_00000000_00000000_00000100,
            PenWidth        = 0b00000000_00000000_00000000_00001000,
            PenStyle        = 0b00000000_00000000_00000000_00010000,
            RopMode         = 0b00000000_00000000_00000000_00100000,
            BackgroundMode  = 0b00000000_00000000_00000000_01000000,
        }
    }
}
