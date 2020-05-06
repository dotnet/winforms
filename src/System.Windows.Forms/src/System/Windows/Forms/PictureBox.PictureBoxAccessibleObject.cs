// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;
using static Interop;

namespace System.Windows.Forms
{
    public partial class PictureBox
    {
        internal class PictureBoxAccessibleObject : ControlAccessibleObject
        {
            internal PictureBoxAccessibleObject(PictureBox owner) : base(owner)
            {
            }

            internal override object? GetPropertyValue(UiaCore.UIA propertyID)
                => propertyID switch
                {
                    UiaCore.UIA.NamePropertyId
                        => Name,
                    UiaCore.UIA.ControlTypePropertyId
                        => UiaCore.UIA.PaneControlTypeId,
                    UiaCore.UIA.AutomationIdPropertyId
                        => Owner.Name,
                    UiaCore.UIA.IsKeyboardFocusablePropertyId
                        => true,
                    _ => base.GetPropertyValue(propertyID)
                };

            internal override bool IsPatternSupported(UiaCore.UIA patternId)
                => patternId switch
                {
                var p when
                    p == UiaCore.UIA.LegacyIAccessiblePatternId => true,
                _ => base.IsPatternSupported(patternId)
    };
        }
    }
}
