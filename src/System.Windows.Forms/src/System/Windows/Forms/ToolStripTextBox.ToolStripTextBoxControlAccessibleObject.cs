// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using static System.Windows.Forms.TextBoxBase;
using static Interop;

namespace System.Windows.Forms
{
    public partial class ToolStripTextBox
    {
        private class ToolStripTextBoxControlAccessibleObject : ToolStripHostedControlAccessibleObject
        {
            private readonly TextBoxBase _owningTextBoxBase;
            private readonly TextBoxBaseUiaTextProvider _textProvider;

            public ToolStripTextBoxControlAccessibleObject(TextBox toolStripHostedControl, ToolStripControlHost? toolStripControlHost) : base(toolStripHostedControl, toolStripControlHost)
            {
                _owningTextBoxBase = toolStripHostedControl;
                _textProvider = new TextBoxBaseUiaTextProvider(toolStripHostedControl);
                UseTextProviders(_textProvider, _textProvider);
            }

            internal override object? GetPropertyValue(UiaCore.UIA propertyID) =>
                propertyID switch
                {
                    UiaCore.UIA.ControlTypePropertyId => UiaCore.UIA.EditControlTypeId,
                    UiaCore.UIA.NamePropertyId => Name,
                    UiaCore.UIA.IsTextPatternAvailablePropertyId => IsPatternSupported(UiaCore.UIA.TextPatternId),
                    UiaCore.UIA.IsTextPattern2AvailablePropertyId => IsPatternSupported(UiaCore.UIA.TextPattern2Id),
                    UiaCore.UIA.IsValuePatternAvailablePropertyId => IsPatternSupported(UiaCore.UIA.ValuePatternId),
                    _ => base.GetPropertyValue(propertyID)
                };

            internal override bool IsPatternSupported(UiaCore.UIA patternId) =>
                patternId switch
                {
                    UiaCore.UIA.ValuePatternId => true,
                    UiaCore.UIA.TextPatternId => true,
                    UiaCore.UIA.TextPattern2Id => true,
                    _ => base.IsPatternSupported(patternId)
                };

            internal override bool IsReadOnly => _owningTextBoxBase.ReadOnly;
        }
    }
}
