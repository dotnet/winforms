// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using static Interop;
using static Interop.UiaCore;

namespace System.Windows.Forms
{
    public abstract partial class TextBoxBase
    {
        internal class TextBoxBaseAccessibleObject : ControlAccessibleObject
        {
            private readonly TextBoxBaseUiaTextProvider _textProvider;

            public TextBoxBaseAccessibleObject(TextBoxBase owner) : base(owner)
            {
                _textProvider = new TextBoxBaseUiaTextProvider(owner);
            }

            internal override bool IsIAccessibleExSupported() => true;

            internal override bool IsPatternSupported(UIA patternId)
                => patternId switch
                {
                    UIA.TextPatternId => true,
                    UIA.TextPattern2Id => true,
                    _ => base.IsPatternSupported(patternId)
                };

            internal override bool IsReadOnly
                => Owner is TextBoxBase textBoxBase && textBoxBase.ReadOnly;

            internal override ITextRangeProvider DocumentRangeInternal
                => _textProvider.DocumentRange;

            internal override ITextRangeProvider[]? GetTextSelection()
                => _textProvider.GetSelection();

            internal override ITextRangeProvider[]? GetTextVisibleRanges()
                => _textProvider.GetVisibleRanges();

            internal override ITextRangeProvider? GetTextRangeFromChild(IRawElementProviderSimple childElement)
                => _textProvider.RangeFromChild(childElement);

            internal override ITextRangeProvider? GetTextRangeFromPoint(Point screenLocation)
                => _textProvider.RangeFromPoint(screenLocation);

            internal override SupportedTextSelection SupportedTextSelectionInternal
                => _textProvider.SupportedTextSelection;

            internal override ITextRangeProvider? GetTextCaretRange(out BOOL isActive)
                => _textProvider.GetCaretRange(out isActive);

            internal override ITextRangeProvider GetRangeFromAnnotation(IRawElementProviderSimple annotationElement)
                => _textProvider.RangeFromAnnotation(annotationElement);
        }
    }
}
