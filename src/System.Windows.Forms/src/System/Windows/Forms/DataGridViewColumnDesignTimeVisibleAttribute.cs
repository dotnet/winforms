// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    using System.Diagnostics.CodeAnalysis;

    [AttributeUsage(AttributeTargets.Class)]
    public sealed class DataGridViewColumnDesignTimeVisibleAttribute : Attribute
    {
        private bool visible;

        public DataGridViewColumnDesignTimeVisibleAttribute(bool visible)
        {
            this.visible = visible;
        }

        public DataGridViewColumnDesignTimeVisibleAttribute()
        {
        }

        public bool Visible
        {
            get
            {
                return visible;
            }
        }

        [
            SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")   // DataGridViewColumnDesignTimeVisibleAttribute 
                                                                                                        // actually immutable.
        ]
        public static readonly DataGridViewColumnDesignTimeVisibleAttribute Yes = new DataGridViewColumnDesignTimeVisibleAttribute(true);

        [
            SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")   // DataGridViewColumnDesignTimeVisibleAttribute 
                                                                                                        // actually immutable.
        ]
        public static readonly DataGridViewColumnDesignTimeVisibleAttribute No = new DataGridViewColumnDesignTimeVisibleAttribute(false);

        [
            SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")   // DataGridViewColumnDesignTimeVisibleAttribute 
                                                                                                        // actually immutable.
        ]
        public static readonly DataGridViewColumnDesignTimeVisibleAttribute Default = Yes;

        public override bool Equals(object obj)
        {
            if (obj == this)
            {
                return true;
            }

            return obj is DataGridViewColumnDesignTimeVisibleAttribute other && other.Visible == visible;
        }

        public override int GetHashCode()
        {
            return typeof(DataGridViewColumnDesignTimeVisibleAttribute).GetHashCode() ^ (visible ? -1 : 0);
        }

        public override bool IsDefaultAttribute()
        {
            return (Visible == Default.Visible);
        }
    }
}
