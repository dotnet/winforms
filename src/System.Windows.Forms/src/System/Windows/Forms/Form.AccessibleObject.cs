// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;

namespace System.Windows.Forms
{
    public partial class Form
    {
        /// <summary>
        ///  Form control accessible object with UI Automation provider functionality.
        ///  This inherits from the base ControlAccessibleObject
        ///  to have all base functionality.
        /// </summary>
        internal class FormAccessibleObject : ControlAccessibleObject
        {
            private readonly Form _owner;

            internal FormAccessibleObject(Form owner) : base(owner)
            {
                _owner = owner;
            }

            public override Rectangle Bounds
            {
                get
                {
                    // Get rectangle without a title bar
                    Rectangle rectangle = _owner.GetToolNativeScreenRectangle();

                    // Add a title bar height 
                    int titleHeight = rectangle.Y - _owner.Top - 1;
                    rectangle.Y -= titleHeight;
                    rectangle.Height += titleHeight;

                    return rectangle;
                }
            }

            internal override bool IsIAccessibleExSupported()
            {
                if (_owner != null)
                {
                    return true;
                }

                return base.IsIAccessibleExSupported();
            }

            internal override bool IsPatternSupported(int patternId)
            {
                if (patternId == NativeMethods.UIA_LegacyIAccessiblePatternId)
                {
                    return true;
                }

                return base.IsPatternSupported(patternId);
            }

            internal override void SetValue(string newValue)
            {
                Value = newValue;
            }
        }
    }
}
