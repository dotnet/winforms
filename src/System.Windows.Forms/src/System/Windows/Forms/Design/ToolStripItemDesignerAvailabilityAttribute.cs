﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.CodeAnalysis;

namespace System.Windows.Forms.Design
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class ToolStripItemDesignerAvailabilityAttribute : Attribute
    {
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "This class is immutable")]
        public static readonly ToolStripItemDesignerAvailabilityAttribute Default = new ToolStripItemDesignerAvailabilityAttribute();

        /// <summary>
        /// Specifies which ToolStrip types the Item can appear in - ToolStrip,MenuStrip,StatusStrip,ContextMenuStrip
        /// Adding this attribute over a class lets you add to the list of custom items in the ToolStrip design time.
        /// </summary>
        public ToolStripItemDesignerAvailabilityAttribute()
        {
        }

        public ToolStripItemDesignerAvailabilityAttribute(ToolStripItemDesignerAvailability visibility)
        {
            ItemAdditionVisibility = visibility;
        }

        public ToolStripItemDesignerAvailability ItemAdditionVisibility { get; } = ToolStripItemDesignerAvailability.None;

        public override bool Equals(object obj)
        {
            if (obj == this)
            {
                return true;
            }

            return obj is ToolStripItemDesignerAvailabilityAttribute other && other.ItemAdditionVisibility == ItemAdditionVisibility;
        }

        public override int GetHashCode() => ItemAdditionVisibility.GetHashCode();

        public override bool IsDefaultAttribute() => Equals(Default);
    }
}
