// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

namespace System.Windows.Forms
{
    /// <summary>
    ///  Specifies how a control should be docked by default when added through the designer.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class DockingAttribute : Attribute
    {
        public DockingAttribute()
        {
            DockingBehavior = DockingBehavior.Never;
        }

        public DockingAttribute(DockingBehavior dockingBehavior)
        {
            DockingBehavior = dockingBehavior;
        }

        public static readonly DockingAttribute Default = new DockingAttribute();

        public DockingBehavior DockingBehavior { get; }

        public override bool Equals(object obj)
        {
            if (obj == this)
            {
                return true;
            }

            return (obj is DockingAttribute other && other.DockingBehavior == DockingBehavior);
        }

        public override int GetHashCode() => DockingBehavior.GetHashCode();

        public override bool IsDefaultAttribute() => Equals(Default);
    }
}
