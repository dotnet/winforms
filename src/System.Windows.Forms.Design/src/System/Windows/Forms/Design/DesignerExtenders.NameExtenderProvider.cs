// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics.CodeAnalysis;

namespace System.Windows.Forms.Design
{
    internal partial class DesignerExtenders
    {
        /// <devdoc>
        ///     This is the base extender provider for all winform document
        ///     designers.  It provides the "Name" property.
        /// </devdoc>
        [
        ProvideProperty("Name", typeof(IComponent))
        ]
        private class NameExtenderProvider : IExtenderProvider
        {

            private IComponent baseComponent;

            /// <devdoc>
            ///      Creates a new DocumentExtenderProvider.
            /// </devdoc>
            internal NameExtenderProvider()
            {
            }

            protected IComponent GetBaseComponent(object o)
            {
                if (baseComponent == null)
                {
                    ISite site = ((IComponent)o).Site;
                    if (site != null)
                    {
                        IDesignerHost host = (IDesignerHost)site.GetService(typeof(IDesignerHost));
                        if (host != null)
                        {
                            baseComponent = host.RootComponent;
                        }
                    }
                }
                return baseComponent;
            }

            /// <devdoc>
            ///     Determines if ths extender provider can extend the given object.  We extend
            ///     all objects, so we always return true.
            /// </devdoc>
            public virtual bool CanExtend(object o)
            {

                // We always extend the root
                //
                IComponent baseComp = GetBaseComponent(o);
                if (baseComp == o)
                {
                    return true;
                }

                // See if this object is inherited.  If so, then we don't want to
                // extend.
                //
                if (!TypeDescriptor.GetAttributes(o)[typeof(InheritanceAttribute)].Equals(InheritanceAttribute.NotInherited))
                {
                    return false;
                }

                return true;
            }

            /// <devdoc>
            ///     This is an extender property that we offer to all components
            ///     on the form.  It implements the "Name" property.
            /// </devdoc>
            [
            DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
            ParenthesizePropertyName(true),
            MergableProperty(false),
            SRDescriptionAttribute(nameof(SR.DesignerPropName)),
            Category("Design")
            ]
            [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
            public virtual string GetName(IComponent comp)
            {
                ISite site = comp.Site;
                if (site != null)
                {
                    return site.Name;
                }
                return null;
            }

            /// <devdoc>
            ///     This is an extender property that we offer to all components
            ///     on the form.  It implements the "Name" property.
            /// </devdoc>
            [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
            public void SetName(IComponent comp, string newName)
            {
                ISite site = comp.Site;
                if (site != null)
                {
                    site.Name = newName;
                }
            }
        }
    }
}

