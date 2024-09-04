﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.System.Variant;
using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms;

public partial class PrintControllerWithStatusDialog
{
    private partial class StatusDialog
    {
        public class StatusDialogAccessibility : ControlAccessibleObject
        {
            public StatusDialogAccessibility(StatusDialog owner) : base(owner)
            {
            }

            internal override VARIANT GetPropertyValue(UIA_PROPERTY_ID propertyID) =>
                propertyID switch
                {
                    UIA_PROPERTY_ID.UIA_IsDialogPropertyId => (VARIANT)true,
                    _ => base.GetPropertyValue(propertyID)
                };

            public override string? Name
            {
                get
                {
                    if (this.TryGetOwnerAs(out StatusDialog? owner) && owner.AccessibleName is { } name)
                    {
                        return name;
                    }

                    return owner?.Text;
                }
                set
                {
                    if (this.TryGetOwnerAs(out StatusDialog? owner))
                    {
                        owner.AccessibleName = value;
                    }
                }
            }
        }
    }
}
