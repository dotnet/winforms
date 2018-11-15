// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {
using System.Diagnostics.CodeAnalysis;

    /// <include file='doc\DataGridViewColumnDesignTimeVisibleAttribute.uex' path='docs/doc[@for="DataGridViewColumnDesignTimeVisibleAttribute"]/*' />
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class DataGridViewColumnDesignTimeVisibleAttribute: Attribute {
        private bool visible;

        /// <include file='doc\DataGridViewColumnDesignTimeVisibleAttribute.uex' path='docs/doc[@for="DataGridViewColumnDesignTimeVisibleAttribute.DataGridViewColumnDesignTimeVisibleAttribute"]/*' />
        public DataGridViewColumnDesignTimeVisibleAttribute (bool visible) {
            this.visible = visible;
        }
        
        /// <include file='doc\DataGridViewColumnDesignTimeVisibleAttribute.uex' path='docs/doc[@for="DataGridViewColumnDesignTimeVisibleAttribute.DataGridViewColumnDesignTimeVisibleAttribute1"]/*' />
        public DataGridViewColumnDesignTimeVisibleAttribute () {
        }

        /// <include file='doc\DataGridViewColumnDesignTimeVisibleAttribute.uex' path='docs/doc[@for="DataGridViewColumnDesignTimeVisibleAttribute.Visible"]/*' />
        public bool Visible {
            get {
                return visible;
            }
        }

        /// <include file='doc\DataGridViewColumnDesignTimeVisibleAttribute.uex' path='docs/doc[@for="DataGridViewColumnDesignTimeVisibleAttribute.Yes"]/*' />
        [
            SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")   // DataGridViewColumnDesignTimeVisibleAttribute 
                                                                                                        // actually immutable.
        ]
        public static readonly DataGridViewColumnDesignTimeVisibleAttribute Yes = new DataGridViewColumnDesignTimeVisibleAttribute(true);

        /// <include file='doc\DataGridViewColumnDesignTimeVisibleAttribute.uex' path='docs/doc[@for="DataGridViewColumnDesignTimeVisibleAttribute.No"]/*' />
        [
            SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")   // DataGridViewColumnDesignTimeVisibleAttribute 
                                                                                                        // actually immutable.
        ]
        public static readonly DataGridViewColumnDesignTimeVisibleAttribute No = new DataGridViewColumnDesignTimeVisibleAttribute(false);

        /// <include file='doc\DataGridViewColumnDesignTimeVisibleAttribute.uex' path='docs/doc[@for="DataGridViewColumnDesignTimeVisibleAttribute.Default"]/*' />
        [
            SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")   // DataGridViewColumnDesignTimeVisibleAttribute 
                                                                                                        // actually immutable.
        ]
        public static readonly DataGridViewColumnDesignTimeVisibleAttribute Default = Yes;
        
        /// <include file='doc\DataGridViewColumnDesignTimeVisibleAttribute.uex' path='docs/doc[@for="DataGridViewColumnDesignTimeVisibleAttribute.Equals"]/*' />
        public override bool Equals(object obj) {
            if (obj == this) {
                return true;
            }

            DataGridViewColumnDesignTimeVisibleAttribute other = obj as DataGridViewColumnDesignTimeVisibleAttribute;
            return other != null && other.Visible == visible;
        }

        /// <include file='doc\DataGridViewColumnDesignTimeVisibleAttribute.uex' path='docs/doc[@for="DataGridViewColumnDesignTimeVisibleAttribute.GetHashCode"]/*' />
        public override int GetHashCode() {
            return typeof(DataGridViewColumnDesignTimeVisibleAttribute).GetHashCode() ^ (visible ? -1 : 0);
        }
        
        /// <include file='doc\DataGridViewColumnDesignTimeVisibleAttribute.uex' path='docs/doc[@for="DataGridViewColumnDesignTimeVisibleAttribute.IsDefaultAttribute"]/*' />
        public override bool IsDefaultAttribute() {
            return (this.Visible == Default.Visible);
        }
    }
}
