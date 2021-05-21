// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using static Interop;

namespace System.Windows.Forms
{
    public abstract partial class TextBoxBase
    {
        internal class TextBoxBaseAccessibleObject : ControlAccessibleObject
        {
            private readonly TextBoxBase _owningTextBoxBase;
            private readonly TextBoxBaseUiaTextProvider _textProvider;

            public TextBoxBaseAccessibleObject(TextBoxBase owner) : base(owner)
            {
                _owningTextBoxBase = owner;
                _textProvider = new TextBoxBaseUiaTextProvider(owner);

                UseTextProviders(_textProvider, _textProvider);
            }

            internal override bool IsIAccessibleExSupported() => true;

            internal override bool IsPatternSupported(UiaCore.UIA patternId) =>
                patternId switch
                {
                    UiaCore.UIA.TextPatternId => true,
                    UiaCore.UIA.TextPattern2Id => true,
                    _ => base.IsPatternSupported(patternId)
                };

            internal override object? GetPropertyValue(UiaCore.UIA propertyID) =>
                propertyID switch
                {
                    UiaCore.UIA.IsTextPatternAvailablePropertyId => IsPatternSupported(UiaCore.UIA.TextPatternId),
                    UiaCore.UIA.IsTextPattern2AvailablePropertyId => IsPatternSupported(UiaCore.UIA.TextPattern2Id),
                    UiaCore.UIA.IsValuePatternAvailablePropertyId => IsPatternSupported(UiaCore.UIA.ValuePatternId),
                    _ => base.GetPropertyValue(propertyID)
                };

            internal override bool IsReadOnly => _owningTextBoxBase.ReadOnly;
        }
    }
}
