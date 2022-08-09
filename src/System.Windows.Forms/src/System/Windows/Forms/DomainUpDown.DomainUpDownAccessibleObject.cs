// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;

namespace System.Windows.Forms
{
    public partial class DomainUpDown
    {
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete(
            Obsoletions.DomainUpDownAccessibleObjectMessage,
            error: false,
            DiagnosticId = Obsoletions.DomainUpDownAccessibleObjectDiagnosticId,
            UrlFormat = Obsoletions.SharedUrlFormat)]
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
