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
                    // If we don't set a default role for the accessible object
                    // it will be retrieved from Windows.
                    // And we don't have a 100% guarantee it will be correct, hence set it ourselves.
                    UiaCore.UIA.ControlTypePropertyId => _owningTextBoxBase.AccessibleRole == AccessibleRole.Default
                                                         ? UiaCore.UIA.EditControlTypeId
                                                         : base.GetPropertyValue(propertyID),
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
