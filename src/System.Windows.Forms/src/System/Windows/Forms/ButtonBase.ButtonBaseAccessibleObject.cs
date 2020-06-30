// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    public partial class ButtonBase
    {
        public class ButtonBaseAccessibleObject : ControlAccessibleObject
        {
            private readonly ButtonBase _owningButtonBase;

            public ButtonBaseAccessibleObject(Control owner)
                : base((owner is ButtonBase owningButtonBase) ? owner : throw new ArgumentException(string.Format(SR.ConstructorArgumentInvalidValueType, nameof(Owner), typeof(ButtonBase))))
            {
                _owningButtonBase = owningButtonBase;
            }

            public override AccessibleStates State
                => _owningButtonBase.IsHandleCreated && _owningButtonBase.OwnerDraw && _owningButtonBase.MouseIsDown
                    ? base.State | AccessibleStates.Pressed
                    : base.State;

            public override void DoDefaultAction()
            {
                if (_owningButtonBase.IsHandleCreated)
                {
                    _owningButtonBase.OnClick(EventArgs.Empty);
                }
            }
        }
    }
}
