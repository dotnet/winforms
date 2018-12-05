// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {

    /// <devdoc>
    ///    <para>
    ///       Specifies how a control should be docked by default when added through the designer.
    ///    </para>
    /// </devdoc>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class DockingAttribute : Attribute {
        private DockingBehavior dockingBehavior;

        /// <devdoc>
        ///    <para>
        ///       Default constructor.
        ///    </para>
        /// </devdoc>
        public DockingAttribute() {
            this.dockingBehavior = DockingBehavior.Never;
        }
        
        /// <devdoc>
        ///    <para>
        ///       Constructor.
        ///    </para>
        /// </devdoc>
        public DockingAttribute(DockingBehavior dockingBehavior) {
            this.dockingBehavior = dockingBehavior;
        }

        /// <devdoc>
        /// <para>
        ///    Specifies the default value for the <see cref='System.ComponentModel.DockingAttribute'/>.
        ///    This <see langword='static '/>field is read-only.
        /// </para>
        /// </devdoc>
        public static readonly DockingAttribute Default = new DockingAttribute();

        /// <devdoc>
        ///    <para>
        ///       DockingBehavior property.
        ///    </para>
        /// </devdoc>
        public DockingBehavior DockingBehavior {
            get {
                return dockingBehavior;
            }
        }

        /// <internalonly/>
        /// <devdoc>
        /// </devdoc>
        public override bool Equals(object obj) {
            if (obj == this) {
                return true;
            }

            DockingAttribute other = obj as DockingAttribute;

            return (other != null) && other.DockingBehavior == this.dockingBehavior;
        }

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public override int GetHashCode() {
            return dockingBehavior.GetHashCode();
        }

        /// <internalonly/>
        /// <devdoc>
        /// </devdoc>
        public override bool IsDefaultAttribute() {
            return (this.Equals(Default));
        }
    }
}
