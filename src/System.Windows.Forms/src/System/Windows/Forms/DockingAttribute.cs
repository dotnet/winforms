﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {

    /// <summary>
    ///    <para>
    ///       Specifies how a control should be docked by default when added through the designer.
    ///    </para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class DockingAttribute : Attribute {
        private DockingBehavior dockingBehavior;

        /// <summary>
        ///    <para>
        ///       Default constructor.
        ///    </para>
        /// </summary>
        public DockingAttribute() {
            this.dockingBehavior = DockingBehavior.Never;
        }
        
        /// <summary>
        ///    <para>
        ///       Constructor.
        ///    </para>
        /// </summary>
        public DockingAttribute(DockingBehavior dockingBehavior) {
            this.dockingBehavior = dockingBehavior;
        }

        /// <summary>
        /// <para>
        ///    Specifies the default value for the <see cref='System.ComponentModel.DockingAttribute'/>.
        ///    This <see langword='static '/>field is read-only.
        /// </para>
        /// </summary>
        public static readonly DockingAttribute Default = new DockingAttribute();

        /// <summary>
        ///    <para>
        ///       DockingBehavior property.
        ///    </para>
        /// </summary>
        public DockingBehavior DockingBehavior {
            get {
                return dockingBehavior;
            }
        }

        /// <summary>
        /// </summary>
        public override bool Equals(object obj) {
            if (obj == this) {
                return true;
            }

            DockingAttribute other = obj as DockingAttribute;

            return (other != null) && other.DockingBehavior == this.dockingBehavior;
        }

        public override int GetHashCode() {
            return dockingBehavior.GetHashCode();
        }

        /// <summary>
        /// </summary>
        public override bool IsDefaultAttribute() {
            return (this.Equals(Default));
        }
    }
}
