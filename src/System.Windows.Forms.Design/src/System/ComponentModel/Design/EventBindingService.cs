// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.Reflection;
using System.Windows.Forms;

namespace System.ComponentModel.Design;

/// <summary>
///  This class provides a default implementation of the event
///  binding service.
/// </summary>
public abstract partial class EventBindingService : IEventBindingService
{
    private readonly IServiceProvider _provider;
    private IComponent? _showCodeComponent;
    private EventDescriptor? _showCodeEventDescriptor;
    private string? _showCodeMethodName;

    /// <summary>
    ///  You must provide a service provider to the binding
    ///  service.
    /// </summary>
    protected EventBindingService(IServiceProvider provider)
    {
        _provider = provider.OrThrowIfNull();
    }

    /// <summary>
    ///  Creates a unique method name. The name must be
    ///  compatible with the script language being used and
    ///  it must not conflict with any other name in the user's
    ///  code.
    /// </summary>
    protected abstract string CreateUniqueMethodName(IComponent component, EventDescriptor e);

    /// <summary>
    ///  This provides a notification that a particular method
    ///  is no longer being used by an event handler. Some implementations
    ///  may want to remove the event hander when no events are using
    ///  it. By overriding UseMethod and FreeMethod, an implementation
    ///  can know when a method is no longer needed.
    /// </summary>
    protected virtual void FreeMethod(IComponent component, EventDescriptor e, string methodName)
    { }

    /// <summary>
    ///  Returns a collection of strings. Each string is
    ///  the method name of a method whose signature is
    ///  compatible with the delegate contained in the
    ///  event descriptor. This should return an empty
    ///  collection if no names are compatible.
    /// </summary>
    protected abstract ICollection GetCompatibleMethods(EventDescriptor e);

    /// <summary>
    ///  Gets the requested service from our service provider.
    /// </summary>
    protected object? GetService(Type serviceType) => _provider.GetService(serviceType);

    /// <summary>
    ///  Shows the user code. This method does not show any
    ///  particular code; generally it shows the last code the
    ///  user typed. This returns true if it was possible to
    ///  show the code, or false if not.
    /// </summary>
    protected abstract bool ShowCode();

    /// <summary>
    ///  Shows the user code at the given line number. Line
    ///  numbers are one-based. This returns true if it was
    ///  possible to show the code, or false if not.
    /// </summary>
    protected abstract bool ShowCode(int lineNumber);

    /// <summary>
    ///  Shows the body of the user code with the given method
    ///  name. This returns true if it was possible to show
    ///  the code, or false if not.
    /// </summary>
    protected abstract bool ShowCode(IComponent component, EventDescriptor e, string methodName);

    /// <summary>
    ///  This provides a notification that a particular method
    ///  is being used by an event handler. Some implementations
    ///  may want to remove the event hander when no events are using
    ///  it. By overriding UseMethod and FreeMethod, an implementation
    ///  can know when a method is no longer needed.
    /// </summary>
    protected virtual void UseMethod(IComponent component, EventDescriptor e, string methodName)
    { }

    /// <summary>
    ///  This validates that the provided method name is valid for
    ///  the language / script being used. The default does nothing.
    ///  You may override this and throw an exception if the name
    ///  is invalid for your use.
    /// </summary>
    protected virtual void ValidateMethodName(string methodName)
    { }

    /// <summary>
    ///  This creates a name for an event handling method for the given component
    ///  and event. The name that is created is guaranteed to be unique in the user's source
    ///  code.
    /// </summary>
    string IEventBindingService.CreateUniqueMethodName(IComponent component, EventDescriptor e)
    {
        ArgumentNullException.ThrowIfNull(component);
        ArgumentNullException.ThrowIfNull(e);

        return CreateUniqueMethodName(component, e);
    }

    /// <summary>
    ///  Retrieves a collection of strings. Each string is the name of a method
    ///  in user code that has a signature that is compatible with the given event.
    /// </summary>
    ICollection IEventBindingService.GetCompatibleMethods(EventDescriptor e)
    {
        ArgumentNullException.ThrowIfNull(e);

        return GetCompatibleMethods(e);
    }

    /// <summary>
    ///  For properties that are representing events, this will return the event
    ///  that the property represents.
    /// </summary>
    EventDescriptor? IEventBindingService.GetEvent(PropertyDescriptor property)
    {
        return (property as EventPropertyDescriptor)?.Event;
    }

    /// <summary>
    ///  Returns true if the given event has a generic argument or return value in its raise method.
    /// </summary>
    private static bool HasGenericArgument([NotNullWhen(true)] EventDescriptor? ed)
    {
        if (ed is null || ed.ComponentType is null)
        {
            return false;
        }

        EventInfo? evInfo = ed.ComponentType.GetEvent(ed.Name);

        if (evInfo is null || !evInfo.EventHandlerType!.IsGenericType)
        {
            return false;
        }

        Type[] args = evInfo.EventHandlerType.GetGenericArguments();

        if (args is not null && args.Length > 0)
        {
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i].IsGenericType)
                {
                    return true;
                }
            }
        }

        return false;
    }

    /// <summary>
    ///  Converts a set of events to a set of properties.
    /// </summary>
    PropertyDescriptorCollection IEventBindingService.GetEventProperties(EventDescriptorCollection events)
    {
        ArgumentNullException.ThrowIfNull(events);

        List<PropertyDescriptor> props = new(events.Count);

        // We cache the property descriptors here for speed. Create those for
        // events that we don't have yet.
        for (int i = 0; i < events.Count; i++)
        {
            if (HasGenericArgument(events[i]))
            {
                continue;
            }

            PropertyDescriptor prop = new EventPropertyDescriptor(events[i]!, this);
            props.Add(prop);
        }

        return new PropertyDescriptorCollection([.. props]);
    }

    /// <summary>
    ///  Converts a single event to a property.
    /// </summary>
    PropertyDescriptor IEventBindingService.GetEventProperty(EventDescriptor e)
    {
        ArgumentNullException.ThrowIfNull(e);

        PropertyDescriptor prop = new EventPropertyDescriptor(e, this);

        return prop;
    }

    /// <summary>
    ///  Displays the user code for this designer. This will return true if the user
    ///  code could be displayed, or false otherwise.
    /// </summary>
    bool IEventBindingService.ShowCode()
    {
        return ShowCode();
    }

    /// <summary>
    ///  Displays the user code for the designer. This will return true if the user
    ///  code could be displayed, or false otherwise.
    /// </summary>
    bool IEventBindingService.ShowCode(int lineNumber)
    {
        return ShowCode(lineNumber);
    }

    /// <summary>
    ///  Displays the user code for the given event. This will return true if the user
    ///  code could be displayed, or false otherwise.
    /// </summary>
    bool IEventBindingService.ShowCode(IComponent component, EventDescriptor e)
    {
        ArgumentNullException.ThrowIfNull(component);
        ArgumentNullException.ThrowIfNull(e);

        PropertyDescriptor prop = ((IEventBindingService)this).GetEventProperty(e);
        string? methodName = (string?)prop.GetValue(component);

        if (methodName is null)
        {
            return false;   // the event is not bound to a method.
        }

        Debug.Assert(_showCodeComponent is null && _showCodeEventDescriptor is null && _showCodeMethodName is null, "show code already pending");
        _showCodeComponent = component;
        _showCodeEventDescriptor = e;
        _showCodeMethodName = methodName;
        Application.Idle += ShowCodeIdle;

        return true;
    }

    /// <summary>
    ///  Displays the user code for the given event. This will return true if the user
    ///  code could be displayed, or false otherwise.
    /// </summary>
    private void ShowCodeIdle(object? sender, EventArgs e)
    {
        Application.Idle -= ShowCodeIdle;

        try
        {
            ShowCode(_showCodeComponent!, _showCodeEventDescriptor!, _showCodeMethodName!);
        }
        finally
        {
            _showCodeComponent = null;
            _showCodeEventDescriptor = null;
            _showCodeMethodName = null;
        }
    }
}
