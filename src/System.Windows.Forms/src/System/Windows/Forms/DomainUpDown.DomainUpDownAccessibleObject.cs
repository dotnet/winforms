// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    public partial class DomainUpDown
    {
        // This class can be removed in the future when it is marked as obsolete.
        // This class is not needed to be used as an accessible object of DomainUpDown,
        // UpDownBaseAccessibleObject does it well.
        // We can't remove this class just like that because it's a public API.
        public class DomainUpDownAccessibleObject : ControlAccessibleObject
        {
            private readonly UpDownBaseAccessibleObject _upDownBaseAccessibleObject;

            public DomainUpDownAccessibleObject(DomainUpDown owner) : base(owner)
            {
                _upDownBaseAccessibleObject = new(owner);
            }

            public override AccessibleRole Role => _upDownBaseAccessibleObject.Role;

            public override AccessibleObject? GetChild(int index) => _upDownBaseAccessibleObject.GetChild(index);

            public override int GetChildCount() => _upDownBaseAccessibleObject.GetChildCount();

            internal override int[] RuntimeId => _upDownBaseAccessibleObject.RuntimeId;
        }
    }
}
