// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;

namespace System.Windows.Forms;

public partial class DomainUpDown
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete(
        Obsoletions.DomainItemAccessibleObjectMessage,
        error: false,
        DiagnosticId = Obsoletions.DomainItemAccessibleObjectDiagnosticId,
        UrlFormat = Obsoletions.SharedUrlFormat)]
    public class DomainItemAccessibleObject : AccessibleObject
    {
        private string? _name;

        public DomainItemAccessibleObject(string? name, AccessibleObject parent)
        {
            _name = name;
        }

        public override string? Name
        {
            get => _name;
            set => _name = value;
        }

        internal override bool CanGetNameInternal => false;

        internal override bool CanSetNameInternal => false;

        public override AccessibleObject? Parent => null;

        public override AccessibleRole Role => AccessibleRole.ListItem;

        public override AccessibleStates State => AccessibleStates.Selectable;

        public override string? Value => _name;

        internal override bool CanGetValueInternal => false;

        internal override int[] RuntimeId => [RuntimeIDFirstItem, GetHashCode()];
    }
}
