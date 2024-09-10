// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;

namespace System.ComponentModel.Design;

/// <summary>
///  This service is requested by TypeDescriptor when asking for type information for a component.
///  Our implementation forwards this filter onto IDesignerFilter on the component's designer, should one exist.
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
    private static IDesigner? GetDesigner(IComponent component)
    {
        ISite? site = component.Site;
        if (site.TryGetService(out IDesignerHost? host))
        {
            return host.GetDesigner(component);
        }

        return null;
    }

    /// <summary>
    ///  Provides a way to filter the attributes from a component that are displayed to the user.
    /// </summary>
    bool ITypeDescriptorFilterService.FilterAttributes(IComponent component, IDictionary attributes)
    {
        ArgumentNullException.ThrowIfNull(component);
        ArgumentNullException.ThrowIfNull(attributes);

        IDesigner? designer = GetDesigner(component);

        if (designer is IDesignerFilter designerFilter)
        {
            designerFilter.PreFilterAttributes(attributes);
            designerFilter.PostFilterAttributes(attributes);
        }

        return designer is not null;
    }

    /// <summary>
    ///  Provides a way to filter the events from a component that are displayed to the user.
    /// </summary>
    bool ITypeDescriptorFilterService.FilterEvents(IComponent component, IDictionary events)
    {
        ArgumentNullException.ThrowIfNull(component);
        ArgumentNullException.ThrowIfNull(events);

        IDesigner? designer = GetDesigner(component);

        if (designer is IDesignerFilter designerFilter)
        {
            designerFilter.PreFilterEvents(events);
            designerFilter.PostFilterEvents(events);
        }

        return designer is not null;
    }

    /// <summary>
    ///  Provides a way to filter the properties from a component that are displayed to the user.
    /// </summary>
    bool ITypeDescriptorFilterService.FilterProperties(IComponent component, IDictionary properties)
    {
        ArgumentNullException.ThrowIfNull(component);
        ArgumentNullException.ThrowIfNull(properties);

        IDesigner? designer = GetDesigner(component);

        if (designer is IDesignerFilter designerFilter)
        {
            designerFilter.PreFilterProperties(properties);
            designerFilter.PostFilterProperties(properties);
        }

        return designer is not null;
    }
}
