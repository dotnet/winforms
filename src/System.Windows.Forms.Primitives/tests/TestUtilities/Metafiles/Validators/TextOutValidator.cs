// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable

using System.Drawing;
using Xunit;
using static Interop;

namespace System.Windows.Forms.Metafiles
{
    internal sealed class TextOutValidator : IEmfValidator
    {
        private readonly Flags _validate;
        private readonly string _text;
        private readonly Color _textColor;
        private readonly Gdi32.MM _mapMode;
        private readonly Gdi32.BKMODE _backgroundMode;
        private readonly string? _fontFace;

        public TextOutValidator(
            string text,
            Color textColor,
            Gdi32.MM mapMode = default,
            Gdi32.BKMODE backgroundMode = default,
            string? fontFace = null,
            Flags validate = default)
        {
            _text = text;
            _textColor = textColor;
            _mapMode = mapMode;
            _backgroundMode = backgroundMode;
            _fontFace = fontFace;

            if (validate != default)
            {
                _validate = validate;
            }
            else
            {
                _validate = Flags.Text;

                if (!textColor.IsEmpty)
                {
                    _validate |= Flags.TextColor;
                }

                if (mapMode != default)
                {
                    _validate |= Flags.MapMode;
                }

                if (backgroundMode != default)
                {
                    _validate |= Flags.BackgroundMode;
                }

                if (!string.IsNullOrEmpty(fontFace))
                {
                    _validate |= Flags.FontFace;
                }
            }
        }

        public bool ShouldValidate(Gdi32.EMR recordType) => recordType == Gdi32.EMR.EXTTEXTOUTW;

        public unsafe void Validate(ref EmfRecord record, DeviceContextState state, out bool complete)
        {
            // We're only checking one TextOut record, so this call completes our work.
            complete = true;

            EMREXTTEXTOUTW* textOut = record.ExtTextOutWRecord;

            if (_validate.HasFlag(Flags.Text))
            {
                Assert.Equal(_text, textOut->emrtext.GetString().ToString());
            }

            if (_validate.HasFlag(Flags.MapMode))
            {
                Assert.Equal(_mapMode, state.MapMode);
            }

            if (_validate.HasFlag(Flags.BackgroundMode))
            {
                Assert.Equal(_backgroundMode, state.BackgroundMode);
            }

            if (_validate.HasFlag(Flags.TextColor))
            {
                Assert.Equal((COLORREF)_textColor, state.TextColor);
            }

            if (_validate.HasFlag(Flags.FontFace))
            {
                Assert.Equal(_fontFace, state.SelectedFont.FaceName.ToString());
            }
        }

        [Flags]
        public enum Flags : uint
        {
            Text            = 0b00000000_00000000_00000000_00000001,
            TextColor       = 0b00000000_00000000_00000000_00000010,
            MapMode         = 0b00000000_00000000_00000000_00000100,
            BackgroundMode  = 0b00000000_00000000_00000000_00001000,
            FontFace        = 0b00000000_00000000_00000000_00010000,
        }
    }
}
