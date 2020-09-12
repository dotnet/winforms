// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable

using System.Drawing;
using Xunit;
using static Interop;

namespace System.Windows.Forms.Metafiles
{
    internal sealed class TextOutValidator : Validator
    {
        private readonly string? _text;
        private readonly string? _fontFace;
        private readonly Rectangle? _bounds;

        public TextOutValidator(
            string? text,
            Rectangle? bounds = default,
            string? fontFace = default,
            params IStateValidator[] stateValidators) : base(stateValidators)
        {
            _text = text;
            _fontFace = fontFace;
            _bounds = bounds;
        }

        public override bool ShouldValidate(Gdi32.EMR recordType) => recordType == Gdi32.EMR.EXTTEXTOUTW;

        public override unsafe void Validate(ref EmfRecord record, DeviceContextState state, out bool complete)
        {
            base.Validate(ref record, state, out _);

            // We're only checking one TextOut record, so this call completes our work.
            complete = true;

            EMREXTTEXTOUTW* textOut = record.ExtTextOutWRecord;

            if (_text != null)
            {
                Assert.Equal(_text, textOut->emrtext.GetText().ToString());
            }

            if (_bounds.HasValue)
            {
                Assert.Equal(_bounds.Value, (Rectangle)textOut->rclBounds);
            }

            if (_fontFace != null)
            {
                Assert.Equal(_fontFace, state.SelectedFont.FaceName.ToString());
            }
        }
    }
}
