// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    public partial class DomainUpDown
    {
        // This class is not used anyhow for building of DomainUpDown accessibility tree, but
        // we can't remove this class just like that because it's a public API.
        // See https://github.com/dotnet/winforms/issues/7344 for more details.
        public class DomainItemAccessibleObject : AccessibleObject
        {
            private string? _name;

            public DomainItemAccessibleObject(string? name, AccessibleObject parent)
            {
                _name = name;
            }

            public override string? Name
            {
                get
                {
                    return _name;
                }
                set
                {
                    _name = value;
                }
            }

            public override AccessibleObject? Parent => null;

            public override AccessibleRole Role
            {
                get
                {
                    return AccessibleRole.ListItem;
                }
            }

            public override AccessibleStates State
            {
                get
                {
                    return AccessibleStates.Selectable;
                }
            }

            public override string? Value
            {
                get
                {
                    return _name;
                }
            }

            internal override int[] RuntimeId => new int[] { RuntimeIDFirstItem, GetHashCode() };
        }
    }
}
