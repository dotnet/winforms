// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;

namespace System.ComponentModel.Design
{
    /// <summary>
    ///  This service is requested by TypeDescriptor when asking for type information for a component.  Our implementation forwards this filter onto IDesignerFilter on the component's designer, should one exist.
    /// </summary>
    internal sealed class TypeDescriptorFilterService : ITypeDescriptorFilterService
    {
        /// <summary>
        ///  Internal ctor to prevent semitrust from creating us.
        /// </summary>
        internal TypeDescriptorFilterService()
        {
        }

        /// <summary>
        ///  Helper method to return the designer for a given component.
        /// </summary>
        private IDesigner GetDesigner(IComponent component)
        {
            ISite site = component.Site;
            if (site != null)
            {
                if (site.GetService(typeof(IDesignerHost)) is IDesignerHost host)
                {
                    return host.GetDesigner(component);
                }
            }
            return null;
        }

        /// <summary>
        ///  Provides a way to filter the attributes from a component that are displayed to the user.
        /// </summary>
        bool ITypeDescriptorFilterService.FilterAttributes(IComponent component, IDictionary attributes)
        {
            if (component == null)
            {
                throw new ArgumentNullException(nameof(component));
            }
            if (attributes == null)
            {
                throw new ArgumentNullException(nameof(attributes));
            }

            IDesigner designer = GetDesigner(component);

            if (designer is IDesignerFilter)
            {
                ((IDesignerFilter)designer).PreFilterAttributes(attributes);
                ((IDesignerFilter)designer).PostFilterAttributes(attributes);
            }
            return designer != null;
        }

        /// <summary>
        ///  Provides a way to filter the events from a component that are displayed to the user.
        /// </summary>
        bool ITypeDescriptorFilterService.FilterEvents(IComponent component, IDictionary events)
        {
            if (component == null)
            {
                throw new ArgumentNullException(nameof(component));
            }
            if (events == null)
            {
                throw new ArgumentNullException(nameof(events));
            }

            IDesigner designer = GetDesigner(component);

            if (designer is IDesignerFilter)
            {
                ((IDesignerFilter)designer).PreFilterEvents(events);
                ((IDesignerFilter)designer).PostFilterEvents(events);
            }
            return designer != null;
        }

        /// <summary>
        ///  Provides a way to filter the properties from a component that are displayed to the user.
        /// </summary>
        bool ITypeDescriptorFilterService.FilterProperties(IComponent component, IDictionary properties)
        {
            if (component == null)
            {
                throw new ArgumentNullException(nameof(component));
            }
            if (properties == null)
            {
                throw new ArgumentNullException(nameof(properties));
            }

            IDesigner designer = GetDesigner(component);

            if (designer is IDesignerFilter)
            {
                ((IDesignerFilter)designer).PreFilterProperties(properties);
                ((IDesignerFilter)designer).PostFilterProperties(properties);
            }
            return designer != null;
        }
    }
}
