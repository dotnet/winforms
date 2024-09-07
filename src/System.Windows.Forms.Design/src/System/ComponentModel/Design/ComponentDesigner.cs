// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.Configuration;
using System.Windows.Forms.Design;

namespace System.ComponentModel.Design;

/// <summary>
///  The default designer for all components.
/// </summary>
public partial class ComponentDesigner : ITreeDesigner, IDesignerFilter, IComponentInitializer
{
    private InheritanceAttribute? _inheritanceAttribute;
    private Dictionary<string, InheritedPropertyDescriptor>? _inheritedProps;
    private DesignerVerbCollection? _verbs;
    private DesignerActionListCollection? _actionLists;
    private ShadowPropertyCollection? _shadowProperties;
    private bool _settingsKeyExplicitlySet;
    private IComponent? _component;

    private protected const string SettingsKeyName = "SettingsKey";

    /// <summary>
    ///  Gets the design-time ActionLists supported by the component associated with the designer.
    /// </summary>
    public virtual DesignerActionListCollection ActionLists => _actionLists ??= [];

    /// <summary>
    ///  Retrieves a list of associated components. These are components that should be included in a cut or copy
    ///  operation on this component.
    /// </summary>
    public virtual ICollection AssociatedComponents => Array.Empty<IComponent>();

    private protected virtual void UpdateTextualDefaultProperty()
    {
        if (_component?.Site is { } site)
        {
            PropertyDescriptor? defaultProperty = TypeDescriptor.GetDefaultProperty(_component);
            if (defaultProperty is null)
            {
                return;
            }

            if (defaultProperty.TryGetValue(_component, out string? currentValue) && string.IsNullOrEmpty(currentValue))
            {
                defaultProperty.SetValue(_component, site.Name);
            }
        }
    }

    internal virtual bool CanBeAssociatedWith(IDesigner parentDesigner) => true;

    /// <summary>
    ///  Gets a value indicating whether or not this component is being inherited.
    /// </summary>
    [MemberNotNullWhen(true, nameof(InheritanceAttribute))]
    protected bool Inherited
    {
        get
        {
            InheritanceAttribute? inheritanceAttribute = InheritanceAttribute;
            return inheritanceAttribute is not null && !inheritanceAttribute.Equals(InheritanceAttribute.NotInherited);
        }
    }

    internal bool IsInheritedReadOnly
        => InheritanceAttribute!.InheritanceLevel == InheritanceLevel.InheritedReadOnly;

    /// <summary>
    ///  This property provides a generic mechanism for discovering parent relationships within designers,
    ///  and is used by ComponentDesigner's ITreeDesigner interface implementation. This property
    ///  should only return null when this designer is the root component. Otherwise, it should return
    ///  the parent component. The default implementation of this property returns the root component
    ///  for all components that are not the root component, and it returns null for the root component.
    /// </summary>
    protected virtual IComponent? ParentComponent
    {
        get
        {
            IComponent? root = GetService<IDesignerHost>()?.RootComponent;
            return root == Component ? null : root;
        }
    }

    /// <summary>
    ///  Gets the inheritance attribute for this component.
    /// </summary>
    protected virtual InheritanceAttribute? InheritanceAttribute
    {
        get
        {
            if (_inheritanceAttribute is null)
            {
                // Record if this component is being inherited or not.
                if (TryGetService(out IInheritanceService? inheritanceService))
                {
                    _inheritanceAttribute = inheritanceService.GetInheritanceAttribute(Component);
                }
                else
                {
                    _inheritanceAttribute = InheritanceAttribute.Default;
                }
            }

            return _inheritanceAttribute;
        }
    }

    /// <summary>
    ///  Gets a collection that houses shadow properties. Shadow properties. are properties that fall through to
    ///  the underlying component before they are set, but return their set values once they are set.
    /// </summary>
    [MemberNotNull(nameof(_shadowProperties))]
    protected ShadowPropertyCollection ShadowProperties => _shadowProperties ??= new ShadowPropertyCollection(this);

    /// <summary>
    ///  This method is called when an existing component is being re-initialized. This may occur after dragging
    ///  a component to another container, for example.
    ///
    ///  The defaultValues property contains a name/value dictionary of default values that should be applied to
    ///  properties. This dictionary may be null if no default values are specified.
    ///
    ///  You may use the defaultValues dictionary to apply recommended defaults to properties but you should not
    ///  modify component properties beyond what is stored in the dictionary, because this is an existing component
    ///  that may already have properties set on it.
    ///
    ///  The default implemenation of this method does nothing.
    /// </summary>
    public virtual void InitializeExistingComponent(IDictionary? defaultValues)
        => throw new NotImplementedException(SR.NotImplementedByDesign);

    /// <summary>
    ///  This method is called when a component is first initialized, typically after being first added
    ///  to a design surface. The defaultValues property contains a name/value dictionary of default
    ///  values that should be applied to properties. This dictionary may be null if no default values
    ///  are specified. You may perform any initialization of this component that you like, and you
    ///  may even ignore the defaultValues dictionary altogether if you wish.
    ///  The default implemenation of this method does nothing.
    /// </summary>
    public virtual void InitializeNewComponent(IDictionary? defaultValues)
    {
        // execute legacy code
        InitializeNonDefault();

        // Note: This was originally an obsoleted API called OnSetComponentDefaults(). The
        // default behavior of this API was to set the the default property to the component's
        // site name, if the property was a string and null or empty. We've removed the API
        // but preserved the same behavior, now controlled by SetTextualDefaultProperty.
        if (SetTextualDefaultProperty)
        {
            UpdateTextualDefaultProperty();
        }
    }

    void IDesignerFilter.PostFilterAttributes(IDictionary attributes) => PostFilterAttributes(attributes);

    void IDesignerFilter.PostFilterEvents(IDictionary events) => PostFilterEvents(events);

    void IDesignerFilter.PostFilterProperties(IDictionary properties) => PostFilterProperties(properties);

    void IDesignerFilter.PreFilterAttributes(IDictionary attributes) => PreFilterAttributes(attributes);

    void IDesignerFilter.PreFilterEvents(IDictionary events) => PreFilterEvents(events);

    void IDesignerFilter.PreFilterProperties(IDictionary properties) => PreFilterProperties(properties);

    /// <summary>
    ///  Gets or sets the component this designer is designing.
    /// </summary>
    public IComponent Component => _component ?? throw new InvalidOperationException("Designer is not initialized");

    /// <summary>
    /// Internal utility used primarily to dispose instances
    /// </summary>
    [MemberNotNullWhen(true, nameof(_component))]
    private protected bool HasComponent => _component is not null;

    /// <summary>
    ///  Gets the design-time verbs supported by the component associated with the designer.
    /// </summary>
    public virtual DesignerVerbCollection Verbs => _verbs ??= [];

    /// <summary>
    ///  Controls whether the default property of <see cref="Component"/> is automatically set
    ///  to <see cref="ISite.Name"/> on creation. The default is <see langword="true"/>.
    /// </summary>
    protected virtual bool SetTextualDefaultProperty => true;

    ICollection ITreeDesigner.Children
    {
        get
        {
            ICollection comps = AssociatedComponents;
            if (comps is not null && comps.Count > 0 && TryGetService(out IDesignerHost? host))
            {
                IDesigner[] designers = new IDesigner[comps.Count];
                int idx = 0;
                foreach (IComponent comp in comps.OfType<IComponent>())
                {
                    IDesigner? designer = host.GetDesigner(comp);
                    if (designer is not null)
                    {
                        designers[idx++] = designer;
                    }
                }

                // If there were missing designers, our array could have some missing bits. Because that's not
                // the norm, we don't optimize for that.
                Array.Resize(ref designers, idx);

                return designers;
            }

            return Array.Empty<object>();
        }
    }

    IDesigner? ITreeDesigner.Parent
    {
        get
        {
            IComponent? parent = ParentComponent;
            if (parent is not null && TryGetService(out IDesignerHost? host))
            {
                return host.GetDesigner(parent);
            }

            return null;
        }
    }

    /// <summary>
    ///  Disposes of the resources (other than memory) used by the <see cref="ComponentDesigner"/>.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    ///  Creates a method signature in the source code file for the default event on the component and navigates
    ///  the user's cursor to that location in preparation to assign the default action.
    /// </summary>
    public virtual void DoDefaultAction()
    {
        // If the event binding service is not available, there is nothing much we can do, so just return.
        if (!TryGetService(out IEventBindingService? ebs)
            || !TryGetService(out ISelectionService? selectionService))
        {
            return;
        }

        ICollection components = selectionService.GetSelectedComponents();
        EventDescriptor? thisDefaultEvent = null;
        string? thisHandler = null;
        IDesignerHost? host = GetService<IDesignerHost>();
        DesignerTransaction? t = null;

        if (components is null)
        {
            return;
        }

        try
        {
            foreach (IComponent comp in components.OfType<IComponent>())
            {
                EventDescriptor? defaultEvent = TypeDescriptor.GetDefaultEvent(comp);
                bool eventChanged = false;

                if (defaultEvent is null)
                {
                    continue;
                }

                PropertyDescriptor? defaultPropEvent = ebs.GetEventProperty(defaultEvent);

                // If we couldn't find a property for this event, or of the property is read only, then abort.
                if (defaultPropEvent is null || defaultPropEvent.IsReadOnly)
                {
                    continue;
                }

                try
                {
                    if (host is not null && t is null)
                    {
                        t = host.CreateTransaction(string.Format(SR.ComponentDesignerAddEvent, defaultEvent.Name));
                    }
                }
                catch (CheckoutException cxe) when (cxe == CheckoutException.Canceled)
                {
                    return;
                }

                // handler will be null if there is no explicit event hookup in the parsed init method
                object? result = defaultPropEvent.GetValue(comp);
                if (result is not string handler)
                {
                    // Skip invalid properties.
                    if (result is not null)
                    {
                        continue;
                    }

                    eventChanged = true;
                    handler = ebs.CreateUniqueMethodName(comp, defaultEvent);
                }
                else
                {
                    // ensure the handler is still there
                    eventChanged = true;
                    ICollection methods = ebs.GetCompatibleMethods(defaultEvent);
                    if (methods is not null)
                    {
                        foreach (string compatibleMethod in methods.OfType<string>())
                        {
                            if (handler == compatibleMethod)
                            {
                                eventChanged = false;
                                break;
                            }
                        }
                    }
                }

                // Save the new value... BEFORE navigating to it!
                if (eventChanged)
                {
                    defaultPropEvent.SetValue(comp, handler);
                }

                if (Component == comp)
                {
                    thisDefaultEvent = defaultEvent;
                    thisHandler = handler;
                }
            }
        }
        catch (InvalidOperationException)
        {
            t?.Cancel();
            t = null;
        }
        finally
        {
            t?.Commit();
        }

        // Now show the event code.
        if (thisHandler is not null && thisDefaultEvent is not null)
        {
            ebs.ShowCode(Component, thisDefaultEvent);
        }
    }

    internal bool IsRootDesigner
    {
        get
        {
            Debug.Assert(
                _component is not null,
                "this.component needs to be set before this method is valid.");

            return TryGetService(out IDesignerHost? host) && _component == host.RootComponent;
        }
    }

    /// <summary>
    ///  Initializes a new instance of the <see cref="ComponentDesigner"/> class using the specified component.
    /// </summary>
    public virtual void Initialize(IComponent component)
    {
        ArgumentNullException.ThrowIfNull(component);
        _component = component;

        // For inherited components, save off the current values so we can compute a delta. We also do this for
        // the root component, but, as it is ALWAYS inherited, the computation of default values favors the
        // presence of a default value attribute over the current code value.
        bool isRoot = TryGetService(out IDesignerHost? host) && component == host.RootComponent;

        if (component.Site is IServiceContainer sc && GetService(typeof(DesignerCommandSet)) is null)
        {
            sc.AddService<DesignerCommandSet>(new CDDesignerCommandSet(this));
        }

        if (TryGetService(out IComponentChangeService? cs))
        {
            cs.ComponentRename += OnComponentRename;
        }

        InheritanceAttribute? inheritanceAttribute = InheritanceAttribute;
        if (isRoot || inheritanceAttribute is null || !inheritanceAttribute.Equals(InheritanceAttribute.NotInherited))
        {
            InitializeInheritedProperties();
        }
    }

    private void InitializeInheritedProperties()
    {
        Dictionary<string, InheritedPropertyDescriptor> props = [];
        InheritanceAttribute? inheritanceAttribute = InheritanceAttribute;
        bool readOnlyInherit = inheritanceAttribute is not null && inheritanceAttribute.Equals(InheritanceAttribute.InheritedReadOnly);

        if (!readOnlyInherit)
        {
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(Component);

            // Now loop through all the properties. For each one, try to match a pre-created property.
            // If that fails, then create a new property.
            PropertyDescriptor[] values = new PropertyDescriptor[properties.Count];
            properties.CopyTo(values, 0);

            foreach (PropertyDescriptor prop in values)
            {
                // Skip some properties
                if (Equals(prop.Attributes[typeof(DesignOnlyAttribute)], DesignOnlyAttribute.Yes))
                {
                    continue;
                }

                if (prop is { SerializationVisibility: DesignerSerializationVisibility.Hidden, IsBrowsable: false })
                {
                    continue;
                }

                if (!props.ContainsKey(prop.Name))
                {
                    // This ia a publicly inherited component. We replace all component properties with inherited
                    // versions that reset the default property values to those that are currently on the component.
                    props.Add(prop.Name, new(prop, Component));
                }
            }
        }

        _inheritedProps = props;
        TypeDescriptor.Refresh(Component); // force TypeDescriptor to re-query us.
    }

    /// <summary>
    ///  Invokes the get inheritance attribute of the specified ComponentDesigner.
    /// </summary>
    protected InheritanceAttribute? InvokeGetInheritanceAttribute(ComponentDesigner? toInvoke)
        => toInvoke?.InheritanceAttribute;

    /// <summary>
    ///  Disposes of the resources (other than memory) used by the <see cref="ComponentDesigner"/>.
    /// </summary>
    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            if (TryGetService(out IComponentChangeService? cs))
            {
                cs.ComponentRename -= OnComponentRename;
            }

            _component = null;
            _inheritedProps = null;
        }
    }

    /// <summary>
    ///  Raised when a component's name changes. Here we update the SettingsKey property if necessary.
    /// </summary>
    private void OnComponentRename(object? sender, ComponentRenameEventArgs e)
    {
        if (Component is IPersistComponentSettings)
        {
            IComponent? rootComponent = GetService<IDesignerHost>()?.RootComponent;

            // SettingsKey is formed based on the name of the component and the root component. If either of
            // these change, we reset settings key (if it hasn't been explicitly set) so it can be recomputed.
            if (!_settingsKeyExplicitlySet && (e.Component == Component || e.Component == rootComponent))
            {
                ResetSettingsKey();
            }
        }
    }

    /// <summary>
    ///  Shadowing the SettingsKey so we can default it to be RootComponent.Name + "." + Control.Name
    /// </summary>
    private string? SettingsKey
    {
        get
        {
            string? settingsKeyName = (string?)ShadowProperties[SettingsKeyName];
            if (!string.IsNullOrEmpty(settingsKeyName))
            {
                return settingsKeyName;
            }

            IComponent? rootComponent = GetService<IDesignerHost>()?.RootComponent;

            if (Component is IPersistComponentSettings persistableComponent && rootComponent is not null)
            {
                if (string.IsNullOrEmpty(persistableComponent.SettingsKey))
                {
                    Debug.Assert(Component.Site is not null);
                    if (rootComponent != persistableComponent)
                    {
                        settingsKeyName = $"{rootComponent.Site!.Name}.{Component.Site.Name}";
                    }
                    else
                    {
                        settingsKeyName = Component.Site.Name;
                    }

                    ShadowProperties[SettingsKeyName] = settingsKeyName;
                }

                persistableComponent.SettingsKey = settingsKeyName;
            }

            return settingsKeyName;
        }
        set
        {
            ShadowProperties[SettingsKeyName] = value;
            _settingsKeyExplicitlySet = true;
            if (Component is IPersistComponentSettings persistableComponent)
            {
                persistableComponent.SettingsKey = value;
            }
        }
    }

    private void ResetSettingsKey()
    {
        if (Component is IPersistComponentSettings)
        {
            SettingsKey = null;
            _settingsKeyExplicitlySet = false;
        }
    }

    /// <summary>
    ///  Called when the designer has been associated with a control that is not in it's default state, such as
    ///  one that has been pasted or drag-dropped onto the designer.
    ///
    ///  This is an opportunity to fixup any shadowed properties in a different way than for default components.
    ///
    ///  This is called after the other initialize functions.
    /// </summary>
    [Obsolete("This method has been deprecated. Use InitializeExistingComponent instead.  https://go.microsoft.com/fwlink/?linkid=14202")]
    public virtual void InitializeNonDefault()
    {
    }

    /// <summary>
    ///  Provides a way for a designer to get services from the hosting environment.
    /// </summary>
    protected virtual object? GetService(Type serviceType) => _component?.Site?.GetService(serviceType);

    internal T? GetService<T>() where T : class
        => GetService(typeof(T)) as T;

    internal T GetRequiredService<T>() where T : class
    {
        ISite componentSite = Component.Site ?? throw new InvalidOperationException("Component should have a site");
        return componentSite.GetRequiredService<T>();
    }

    internal bool TryGetService<T>([NotNullWhen(true)] out T? service) where T : class
    {
        service = GetService<T>();
        return service is not null;
    }

    /// <summary>
    ///  Raises the SetComponentDefault event.
    /// </summary>
    [Obsolete("This method has been deprecated. Use InitializeNewComponent instead.  https://go.microsoft.com/fwlink/?linkid=14202")]
    public virtual void OnSetComponentDefaults()
    {
        UpdateTextualDefaultProperty();
    }

    /// <summary>
    ///  Called when the context menu should be displayed
    /// </summary>
    internal virtual void ShowContextMenu(int x, int y)
        => GetService<IMenuCommandService>()?.ShowContextMenu(MenuCommands.SelectionMenu, x, y);

    /// <summary>
    ///  Allows a designer to filter the set of member attributes the component it is designing will expose
    ///  through the TypeDescriptor object.
    /// </summary>
    protected virtual void PostFilterAttributes(IDictionary attributes)
    {
        // If this component is being inherited, mark it as such in the class attributes.
        // Also, set our member variable to ensure that what you get by querying through the TypeDescriptor and
        // through InheritanceAttribute directly is the same.
        if (attributes is null)
        {
            return;
        }

        if (attributes.Contains(typeof(InheritanceAttribute)))
        {
            _inheritanceAttribute = attributes[typeof(InheritanceAttribute)] as InheritanceAttribute;
            return;
        }

        InheritanceAttribute? inheritanceAttribute = InheritanceAttribute;
        if (inheritanceAttribute is not null && !inheritanceAttribute.Equals(InheritanceAttribute.NotInherited))
        {
            attributes[typeof(InheritanceAttribute)] = InheritanceAttribute;
        }
    }

    /// <summary>
    ///  Allows a designer to filter the set of events the component it is designing will expose through the TypeDescriptor object.
    /// </summary>
    protected virtual void PostFilterEvents(IDictionary events)
    {
        // If this component is being privately inherited, we need to filter the events to make them read-only.
        if (events is null)
        {
            return;
        }

        InheritanceAttribute? inheritanceAttribute = InheritanceAttribute;
        if (inheritanceAttribute is null || !inheritanceAttribute.Equals(InheritanceAttribute.InheritedReadOnly))
        {
            return;
        }

        EventDescriptor?[] values = new EventDescriptor[events.Values.Count];
        events.Values.CopyTo(values, 0);

        foreach (EventDescriptor? evt in values)
        {
            if (evt is not null)
            {
                events[evt.Name] = TypeDescriptor.CreateEvent(evt.ComponentType, evt, ReadOnlyAttribute.Yes);
            }
        }
    }

    /// <summary>
    ///  Allows a designer to filter the set of properties the component it is
    ///  designing will expose through the TypeDescriptor object.
    /// </summary>
    protected virtual void PostFilterProperties(IDictionary properties)
    {
        if (_inheritedProps is null)
        {
            return;
        }

        bool readOnlyInherit = InheritanceAttribute.InheritedReadOnly.Equals(InheritanceAttribute);
        if (readOnlyInherit)
        {
            // Now loop through all the properties. For each one, try to match a pre-created property.
            // If that fails, then create a new property.
            PropertyDescriptor[] values = new PropertyDescriptor[properties.Values.Count];
            properties.Values.CopyTo(values, 0);

            foreach (PropertyDescriptor prop in values)
            {
                // This is a private component. Therefore, the user should not be allowed to modify any properties.
                // We replace all properties with read-only versions.
                properties[prop.Name] = TypeDescriptor.CreateProperty(prop.ComponentType, prop, ReadOnlyAttribute.Yes);
            }

            return;
        }

        // otherwise apply our inherited properties to the actual property list.
        foreach (KeyValuePair<string, InheritedPropertyDescriptor> de in _inheritedProps)
        {
            // replace the property descriptor it was created with with the new one in case we're shadowing
            PropertyDescriptor? newInnerProp = (PropertyDescriptor?)properties[de.Key];
            if (newInnerProp is not null)
            {
                InheritedPropertyDescriptor inheritedPropDesc = de.Value;
                inheritedPropDesc.PropertyDescriptor = newInnerProp;
                properties[de.Key] = inheritedPropDesc;
            }
        }
    }

    /// <summary>
    ///  Allows a designer to filter the set of member attributes the component it is designing will expose
    ///  through the TypeDescriptor object.
    /// </summary>
    protected virtual void PreFilterAttributes(IDictionary attributes)
    {
    }

    /// <summary>
    ///  Allows a designer to filter the set of events the component it is designing will expose through the
    ///  TypeDescriptor object.
    /// </summary>
    protected virtual void PreFilterEvents(IDictionary events)
    {
    }

    /// <summary>
    ///  Allows a designer to filter the set of properties the component it is designing will expose through
    ///  the TypeDescriptor object.
    /// </summary>
    protected virtual void PreFilterProperties(IDictionary properties)
    {
        if (Component is IPersistComponentSettings
            && properties?[SettingsKeyName] is PropertyDescriptor prop)
        {
            properties[SettingsKeyName] = TypeDescriptor.CreateProperty(
                typeof(ComponentDesigner),
                prop,
                []);
        }
    }

    /// <summary>
    ///  Notifies the <see cref="IComponentChangeService"/> that this component has been changed.
    ///  You only need to call this when you are affecting component properties directly and not through
    ///  the MemberDescriptor's accessors.
    /// </summary>
    protected void RaiseComponentChanged(MemberDescriptor? member, object? oldValue, object? newValue)
        => GetService<IComponentChangeService>()?.OnComponentChanged(Component, member, oldValue, newValue);

    /// <summary>
    ///  Notifies the <see cref="IComponentChangeService"/> that this component is
    ///  about to be changed. You only need to call this when you are affecting component properties directly and
    ///  not through the MemberDescriptor's accessors.
    /// </summary>
    protected void RaiseComponentChanging(MemberDescriptor? member)
        => GetService<IComponentChangeService>()?.OnComponentChanging(Component, member);
}
