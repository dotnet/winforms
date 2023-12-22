// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

public partial class ComboBox
{
    public class ChildAccessibleObject : AccessibleObject
    {
        private readonly ComboBox _owner;

        public ChildAccessibleObject(ComboBox owner, IntPtr handle)
        {
            _owner = owner.OrThrowIfNull();

            Debug.Assert(owner.IsHandleCreated, "ComboBox's handle hasn't been created");

            if (owner.IsHandleCreated)
            {
                UseStdAccessibleObjects(handle);
            }
        }

        public override string? Name => _owner.AccessibilityObject.Name;

        internal override BSTR GetNameInternal() => _owner.AccessibilityObject.GetNameInternal();

        internal override bool CanGetNameInternal => _owner.AccessibilityObject.CanGetNameInternal;
    }
}
