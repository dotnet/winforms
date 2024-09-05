// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;

namespace System.Windows.Forms.Design;

internal partial class DesignerExtenders
{
    /// <summary>
    ///  This extender provider offers up read-only versions of "Name" property
    ///  for inherited components.
    /// </summary>
    private class NameInheritedExtenderProvider : NameExtenderProvider
    {
        /// <summary>
        ///  Creates a new DocumentInheritedExtenderProvider.
        /// </summary>
        internal NameInheritedExtenderProvider()
        {
        }

        /// <summary>
        ///  Determines if ths extender provider can extend the given object. We extend
        ///  all objects, so we always return true.
        /// </summary>
        public override bool CanExtend(object o)
        {
            // We never extend the root
            //
            IComponent? baseComp = GetBaseComponent(o);
            if (baseComp == o)
            {
                return false;
            }

            // See if this object is inherited. If so, then we are interested in it.
            //
            if (!TypeDescriptor.GetAttributes(o)[typeof(InheritanceAttribute)]?.Equals(InheritanceAttribute.NotInherited) ?? false)
            {
                return true;
            }

            return false;
        }

        [ReadOnly(true)]
        public override string? GetName(IComponent comp)
        {
            return base.GetName(comp);
        }
    }
}
