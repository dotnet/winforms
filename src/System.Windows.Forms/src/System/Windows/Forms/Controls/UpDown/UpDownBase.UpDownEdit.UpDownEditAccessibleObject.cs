// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

public abstract partial class UpDownBase
{
    internal partial class UpDownEdit
    {
        internal class UpDownEditAccessibleObject : TextBoxBaseAccessibleObject
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

            public override string? KeyboardShortcut => _parent.AccessibilityObject.KeyboardShortcut;
        }
    }
}
