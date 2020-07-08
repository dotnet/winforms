// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Interop;

namespace System.Windows.Forms
{
    public partial class ScrollBar
    {
        internal class ScrollBarAccessibleObject : ControlAccessibleObject
        {
            private ScrollBar _owningScrollBar;

            internal ScrollBarAccessibleObject(ScrollBar owner) : base(owner)
            {
                _owningScrollBar = owner;
            }

            internal override bool IsIAccessibleExSupported() => true;

            internal override bool IsPatternSupported(UiaCore.UIA patternId)
            {
                if (patternId == UiaCore.UIA.ValuePatternId)
                {
                    return true;
                }

                return base.IsPatternSupported(patternId);
            }

            internal override object? GetPropertyValue(UiaCore.UIA propertyID)
                => propertyID switch
                {
                    UiaCore.UIA.NamePropertyId => Name,
                    UiaCore.UIA.AutomationIdPropertyId => _owningScrollBar.Name,
                    UiaCore.UIA.ControlTypePropertyId => UiaCore.UIA.ScrollBarControlTypeId,
                    UiaCore.UIA.IsKeyboardFocusablePropertyId => true,
                    UiaCore.UIA.IsValuePatternAvailablePropertyId => IsPatternSupported(UiaCore.UIA.ValuePatternId),
                    _ => base.GetPropertyValue(propertyID)
                };
        }
    }
}
