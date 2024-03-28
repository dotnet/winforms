// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.System.Variant;

namespace System.Windows.Forms;

public abstract partial class UpDownBase
{
    internal partial class UpDownEdit
    {
        internal sealed class UpDownEditAccessibleObject : TextBoxBaseAccessibleObject
        {
            private readonly UpDownBase _parent;

            public UpDownEditAccessibleObject(UpDownEdit owner, UpDownBase parent) : base(owner)
            {
                _parent = parent.OrThrowIfNull();
            }

            public override string? Name
            {
                get => _parent.AccessibilityObject.Name
                    ?? _parent switch
                    {
                        NumericUpDown or DomainUpDown => SR.EditDefaultAccessibleName,
                        _ => null
                    };
                set => _parent.AccessibilityObject.Name = value;
            }

            internal override bool CanGetNameInternal => false;

            internal override bool CanSetNameInternal => _parent.AccessibilityObject.CanSetNameInternal;

            internal override void SetNameInternal(BSTR value) => _parent.AccessibilityObject.SetNameInternal(value);

            public override string? KeyboardShortcut => _parent.AccessibilityObject.KeyboardShortcut;

            internal override bool CanGetKeyboardShortcutInternal => _parent.AccessibilityObject.CanGetKeyboardShortcutInternal;

            internal override BSTR GetKeyboardShortcutInternal(VARIANT childID) => _parent.AccessibilityObject.GetKeyboardShortcutInternal(childID);

            private protected override bool IsInternal => true;
        }
    }
}
