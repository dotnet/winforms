// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;

namespace System.Windows.Forms.Design
{
    internal partial class DesignerExtenders
    {
        /// <devdoc>
        ///      This extender provider offers up read-only versions of "Name" property
        ///      for inherited components.
        /// </devdoc>
        private class NameInheritedExtenderProvider : NameExtenderProvider
        {

            /// <devdoc>
            ///      Creates a new DocumentInheritedExtenderProvider.
            /// </devdoc>
            internal NameInheritedExtenderProvider()
            {
            }

            /// <devdoc>
            ///     Determines if ths extender provider can extend the given object.  We extend
            ///     all objects, so we always return true.
            /// </devdoc>
            public override bool CanExtend(object o)
            {

                // We never extend the root
                //
                IComponent baseComp = GetBaseComponent(o);
                if (baseComp == o)
                {
                    return false;
                }

                // See if this object is inherited.  If so, then we are interested in it.
                //
                if (!TypeDescriptor.GetAttributes(o)[typeof(InheritanceAttribute)].Equals(InheritanceAttribute.NotInherited))
                {
                    return true;
                }

                return false;
            }

            [ReadOnly(true)]
            public override string GetName(IComponent comp)
            {
                return base.GetName(comp);
            }
        }
    }
}

