// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.ComponentModel.Design;
using System.Runtime.InteropServices;

namespace System.Windows.Forms.Design;

/// <summary>
///  Provides a tab on the property browser to display events for selection and linking.
/// </summary>
public class EventsTab : PropertyTab
{
    private readonly IServiceProvider? _serviceProvider;
    private IDesignerHost? _currentHost;
    private bool _hookedDesignerChanged;

    /// <summary>
    ///  Initializes a new instance of the <see cref="EventsTab"/> class.
    /// </summary>
    public EventsTab(IServiceProvider? sp) => _serviceProvider = sp;

    public override string TabName => SR.PBRSToolTipEvents;

    public override string HelpKeyword => "Events";

    public override bool CanExtend(object? extendee) => extendee is null || !Marshal.IsComObject(extendee);

    private void OnActiveDesignerChanged(object? sender, ActiveDesignerEventArgs e)
        => _currentHost = e.NewDesigner;

    public override PropertyDescriptor? GetDefaultProperty(object obj)
    {
        if (GetEventBindingService(obj, context: null) is not IEventBindingService eventPropertyService)
        {
            return null;
        }

        // Find the default event. Note that we rely on GetEventProperties caching the property to event match,
        // so we can == on the default event. We assert that this always works.
        return TypeDescriptor.GetDefaultEvent(obj) is EventDescriptor defaultEvent
            ? eventPropertyService.GetEventProperty(defaultEvent)
            : null;
    }

    /// <summary>
    ///  Returns the most relevant <see cref="IEventBindingService"/>.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   This looks first at the current <see cref="IDesignerHost"/>, then the <paramref name="object"/>'s
    ///   <see cref="ISite"/>, then finally the passed in <paramref name="context"/>.
    ///  </para>
    /// </remarks>
    private IEventBindingService? GetEventBindingService(object @object, ITypeDescriptorContext? context)
    {
        if (!_hookedDesignerChanged)
        {
            if (_serviceProvider.TryGetService(out IDesignerEventService? designerEventService))
            {
                designerEventService.ActiveDesignerChanged += OnActiveDesignerChanged;
            }

            _hookedDesignerChanged = true;
        }

        if (_currentHost.TryGetService(out IEventBindingService? hostEventBindingService))
        {
            return hostEventBindingService;
        }

        if (@object is IComponent component)
        {
            if (component.Site.TryGetService(out IEventBindingService? siteEventBindingService))
            {
                return siteEventBindingService;
            }
        }

        if (context.TryGetService(out IEventBindingService? contextEventBindingService))
        {
            return contextEventBindingService;
        }

        return null;
    }

    public override PropertyDescriptorCollection GetProperties(object component, Attribute[]? attributes)
        => GetProperties(context: null, component, attributes);

    /// <inheritdoc/>
    /// <devdoc>
    ///  The <see cref="EventsTab"/> uses <see cref="IEventBindingService.GetEventProperties(EventDescriptorCollection)"/>
    ///  to get <paramref name="component"/> property descriptors from <see cref="TypeDescriptor.GetEvents(object)"/>.
    /// </devdoc>
    public override PropertyDescriptorCollection GetProperties(
        ITypeDescriptorContext? context,
        object component,
        Attribute[]? attributes)
    {
        if (GetEventBindingService(component, context) is not IEventBindingService eventBindingService)
        {
            return new(null);
        }

        // By passing `noCustomTypeDesc: false` we allow the component itself to participate in getting events
        // (if `component` implements `ICustomTypeDescriptor`).
        var componentEventProperties = eventBindingService.GetEventProperties(
           TypeDescriptor.GetEvents(component, attributes, noCustomTypeDesc: false));

        if (component is null)
        {
            return componentEventProperties;
        }

        // Add DesignerSerializationVisibilityAttribute.Content to the specified attributes.
        Attribute[] specifiedAttributesPlusContent;

        if (attributes is null)
        {
            specifiedAttributesPlusContent = [DesignerSerializationVisibilityAttribute.Content];
        }
        else
        {
            specifiedAttributesPlusContent = new Attribute[attributes.Length + 1];
            Array.Copy(attributes, 0, specifiedAttributesPlusContent, 0, attributes.Length);
            specifiedAttributesPlusContent[attributes.Length] = DesignerSerializationVisibilityAttribute.Content;
        }

        var matchingComponentProperties = TypeDescriptor.GetProperties(component, specifiedAttributesPlusContent);
        if (matchingComponentProperties.Count == 0)
        {
            return componentEventProperties;
        }

        // Check the matching component properties to see if they themselves have any matching events.

        List<PropertyDescriptor>? matchingPropertyEvents = null;
        foreach (PropertyDescriptor property in matchingComponentProperties)
        {
            if (!property.Converter.GetPropertiesSupported())
            {
                continue;
            }

            object? value = property.GetValue(component);

            // Annotations on TypeDescriptor.GetEvents aren't correct, it doesn't care about null.
            EventDescriptorCollection propertyEvents = TypeDescriptor.GetEvents(value!, attributes!);
            if (propertyEvents.Count > 0)
            {
                matchingPropertyEvents ??= [];

                // Make the matching property non-mergable.
                matchingPropertyEvents.Add(TypeDescriptor.CreateProperty(property.ComponentType, property, MergablePropertyAttribute.No));
            }
        }

        if (matchingPropertyEvents is not null)
        {
            // Found matching events on the component properties, merge them with the events directly on the component.
            var mergedEvents = new PropertyDescriptor[componentEventProperties.Count + matchingPropertyEvents.Count];
            componentEventProperties.CopyTo(mergedEvents, 0);
            matchingPropertyEvents.CopyTo(mergedEvents, componentEventProperties.Count);
            componentEventProperties = new(mergedEvents);
        }

        return componentEventProperties;
    }
}
