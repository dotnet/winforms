// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class DataGridViewColumnDesignTimeVisibleAttribute : Attribute
    {
        public DataGridViewColumnDesignTimeVisibleAttribute()
        {
        }

        public DataGridViewColumnDesignTimeVisibleAttribute(bool visible)
        {
            Visible = visible;
        }

        public bool Visible { get; }

        public static readonly DataGridViewColumnDesignTimeVisibleAttribute Yes = new DataGridViewColumnDesignTimeVisibleAttribute(true);

        public static readonly DataGridViewColumnDesignTimeVisibleAttribute No = new DataGridViewColumnDesignTimeVisibleAttribute(false);

        public static readonly DataGridViewColumnDesignTimeVisibleAttribute Default = Yes;

        public override bool Equals(object obj)
        {
            if (obj == this)
            {
                return true;
            }

            return obj is DataGridViewColumnDesignTimeVisibleAttribute other && other.Visible == Visible;
        }

        public override int GetHashCode() => HashCode.Combine(typeof(DataGridViewColumnDesignTimeVisibleAttribute), Visible);

        public override bool IsDefaultAttribute() => Visible == Default.Visible;
    }
}
