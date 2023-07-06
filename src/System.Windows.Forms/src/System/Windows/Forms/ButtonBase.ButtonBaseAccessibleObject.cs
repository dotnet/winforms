// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms;

public partial class ButtonBase
{
    public class ButtonBaseAccessibleObject : ControlAccessibleObject
    {
        public ButtonBaseAccessibleObject(Control owner) : base((owner is ButtonBase owningButtonBase)
            ? owner
            : throw new ArgumentException(string.Format(SR.ConstructorArgumentInvalidValueType, nameof(owner), typeof(ButtonBase))))
        {
        }

        public override AccessibleStates State
            => this.TryGetOwnerAs(out ButtonBase? owner) && owner.IsHandleCreated && owner.OwnerDraw && owner.MouseIsDown
                ? base.State | AccessibleStates.Pressed
                : base.State;

        public override void DoDefaultAction()
        {
            if (this.TryGetOwnerAs(out ButtonBase? owner) && owner.IsHandleCreated)
            {
                owner.OnClick(EventArgs.Empty);
            }
        }

        internal static string? GetKeyboardShortcut(Control control, bool useMnemonic, Label? previousLabel)
        {
            char mnemonic = (char)0;

            if (!useMnemonic && previousLabel is not null && previousLabel.UseMnemonic)
            {
                string text = previousLabel.Text;
                if (!string.IsNullOrEmpty(text))
                {
                    mnemonic = WindowsFormsUtils.GetMnemonic(text, false);
                }
            }
            else if (useMnemonic)
            {
                string text = control.Text;
                if (!string.IsNullOrEmpty(text))
                {
                    mnemonic = WindowsFormsUtils.GetMnemonic(text, false);
                }
            }

            return (mnemonic == (char)0) ? null : $"Alt+{mnemonic}";
        }

        public override string? KeyboardShortcut => this.TryGetOwnerAs(out ButtonBase? owner)
            ? GetKeyboardShortcut(owner, owner.UseMnemonic, PreviousLabel)
            : null;

        public override string? Name
        {
            get
            {
                if (!this.TryGetOwnerAs(out ButtonBase? owner))
                {
                    return null;
                }

                if (owner.AccessibleName is { } name)
                {
                    return name;
                }

                return owner.UseMnemonic ? WindowsFormsUtils.TextWithoutMnemonics(TextLabel) : TextLabel;
            }
        }
    }
}
