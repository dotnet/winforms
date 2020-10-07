// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Diagnostics;

namespace System.Windows.Forms
{
    public partial class ComboBox
    {
        public class ChildAccessibleObject : AccessibleObject
        {
            private readonly ComboBox _owner;

            public ChildAccessibleObject(ComboBox owner, IntPtr handle)
            {
                Debug.Assert(owner?.IsHandleCreated is true, "ComboBox's handle hasn't been created");

                _owner = owner;

                if (owner?.IsHandleCreated is true)
                {
                    UseStdAccessibleObjects(handle);
                }
            }

            public override string Name
            {
                get
                {
                    return _owner.AccessibilityObject.Name;
                }
            }
        }
    }
}
