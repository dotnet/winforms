﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
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
                            NumericUpDown _ => SR.EditDefaultAccessibleName,
                            DomainUpDown _ => SR.EditDefaultAccessibleName,
                            _ => null
                        };
                    set => _parent.AccessibilityObject.Name = value;
                }

                public override string? KeyboardShortcut => _parent.AccessibilityObject.KeyboardShortcut;
            }
        }
    }
}
