// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows.Forms.Design;

namespace System.ComponentModel.Design
{
    /// <summary>
    ///  The default designer for all components.
    /// </summary>
    public class ComponentDesigner : ITreeDesigner, IDesignerFilter, IComponentInitializer
    {
        IComponent _component;
        InheritanceAttribute _inheritanceAttribute;
        Hashtable _inheritedProps;
        DesignerVerbCollection _verbs;
        DesignerActionListCollection _actionLists;
        ShadowPropertyCollection _shadowProperties;
        bool _settingsKeyExplicitlySet;

        private protected const string SettingsKeyName = "SettingsKey";

        /// <summary>
        ///  Gets the design-time actionlists supported by the component associated with the designer.
        /// </summary>
        public virtual DesignerActionListCollection ActionLists
        {
            get
            {
                if (_actionLists is null)
                {
                    _actionLists = new DesignerActionListCollection();
                }
                return _actionLists;
            }
        }

        /// <summary>
        ///  Retrieves a list of associated components. These are components that should be incluced in a cut or copy operation on this component.
        /// </summary>
        public virtual ICollection AssociatedComponents
        {
            get => Array.Empty<IComponent>();
        }

        internal virtual bool CanBeAssociatedWith(IDesigner parentDesigner) => true;

        /// <summary>
        ///  Gets or sets a value indicating whether or not this component is being inherited.
        /// </summary>
        protected bool Inherited
        {
            get
            {
                InheritanceAttribute inheritanceAttribute = InheritanceAttribute;
                return inheritanceAttribute != null && !inheritanceAttribute.Equals(InheritanceAttribute.NotInherited);
            }
        }

        /// <summary>
        ///  This property provides a generic mechanism for discovering parent relationships within designers,
        ///  and is used by ComponentDesigner's ITreeDesigner interface implementation.  This property
        ///  should only return null when this designer is the root component.  Otherwise, it should return
        ///  the parent component.  The default implementation of this property returns the root component
        ///  for all components that are not the root component, and it returns null for the root component.
        /// </summary>
        protected virtual IComponent ParentComponent
        {
            get
            {
                IDesignerHost host = GetService(typeof(IDesignerHost)) as IDesignerHost;
                IComponent root = host?.RootComponent;
                if (root == Component)
                {
                    return null;
                }
                else
                {
                    return root;
                }
            }
        }

        /// <summary>
        ///  Gets or sets the inheritance attribute for this component.
        /// </summary>
        protected virtual InheritanceAttribute InheritanceAttribute
        {
            get
            {
                if (_inheritanceAttribute is null)
                {
                    // Record if this component is being inherited or not.
                    if (GetService(typeof(IInheritanceService)) is IInheritanceService inher)
                    {
                        _inheritanceAttribute = inher.GetInheritanceAttribute(Component);
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
        ///  Gets a collection that houses shadow properties.  Shadow properties. are properties that fall through to the underlying component before they are set, but return their set values once they are set.
        /// </summary>
        protected ShadowPropertyCollection ShadowProperties
        {
            get
            {
                if (_shadowProperties is null)
                {
                    _shadowProperties = new ShadowPropertyCollection(this);
                }
                return _shadowProperties;
            }
        }

        /// <summary>
        ///  This method is called when an existing component is being re-initialized.  This may occur after dragging a component to another container, for example.
        ///  The defaultValues property contains a name/value dictionary of default values that should be applied to properties. This dictionary may be null if no default values are specified.
        ///  You may use the defaultValues dictionary to apply recommended defaults to proeprties but you should not modify component properties beyond what is stored in the dictionary, because this is an existing component that may already have properties set on it.
        ///  The default implemenation of this method does nothing.
        /// </summary>
        public virtual void InitializeExistingComponent(IDictionary defaultValues)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///  This method is called when a component is first initialized, typically after being first added
        ///  to a design surface.  The defaultValues property contains a name/value dictionary of default
        ///  values that should be applied to properties.  This dictionary may be null if no default values
        ///  are specified.  You may perform any initialization of this component that you like, and you
        ///  may even ignore the defaultValues dictionary altogether if you wish.
        ///  The default implemenation of this method does nothing.
        /// </summary>
        public virtual void InitializeNewComponent(IDictionary defaultValues)
        {
            // execute legacy code
            InitializeNonDefault();
        }

        void IDesignerFilter.PostFilterAttributes(IDictionary attributes)
        {
            PostFilterAttributes(attributes);
        }

        void IDesignerFilter.PostFilterEvents(IDictionary events)
        {
            PostFilterEvents(events);
        }

        void IDesignerFilter.PostFilterProperties(IDictionary properties)
        {
            PostFilterProperties(properties);
        }

        void IDesignerFilter.PreFilterAttributes(IDictionary attributes)
        {
            PreFilterAttributes(attributes);
        }

        void IDesignerFilter.PreFilterEvents(IDictionary events)
        {
            PreFilterEvents(events);
        }

        void IDesignerFilter.PreFilterProperties(IDictionary properties)
        {
            PreFilterProperties(properties);
        }

        /// <summary>
        ///  Gets or sets the component this designer is designing.
        /// </summary>
        public IComponent Component
        {
            get
            {
                return _component;
            }
        }

        /// <summary>
        ///  Gets the design-time verbs supported by the component associated with the designer.
        /// </summary>
        public virtual DesignerVerbCollection Verbs
        {
            get
            {
                if (_verbs is null)
                {
                    _verbs = new DesignerVerbCollection();
                }
                return _verbs;
            }
        }

        ICollection ITreeDesigner.Children
        {
            get
            {
                ICollection comps = AssociatedComponents;
                if (comps != null && comps.Count > 0 && GetService(typeof(IDesignerHost)) is IDesignerHost host)
                {
                    IDesigner[] designers = new IDesigner[comps.Count];
                    int idx = 0;
                    foreach (IComponent comp in comps.OfType<IComponent>())
                    {
                        designers[idx] = host.GetDesigner(comp);
                        if (designers[idx] != null)
                        {
                            idx++;
                        }
                    }

                    // If there were missing designers, our array could have some missing bits. Because that's not the norm, we don't optimize for that.
                    if (idx < designers.Length)
                    {
                        IDesigner[] newDesigners = new IDesigner[idx];
                        Array.Copy(designers, 0, newDesigners, 0, idx);
                        designers = newDesigners;
                    }

                    return designers;
                }
                return Array.Empty<object>();
            }
        }

        IDesigner ITreeDesigner.Parent
        {
            get
            {
                IComponent parent = ParentComponent;
                if (parent != null)
                {
                    if (GetService(typeof(IDesignerHost)) is IDesignerHost host)
                    {
                        return host.GetDesigner(parent);
                    }
                }
                return null;
            }
        }

        /// <summary>
        ///  Disposes of the resources (other than memory) used by the <see cref='System.ComponentModel.Design.ComponentDesigner' />.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///  Creates a method signature in the source code file for the default event on the component and navigates the user's cursor to that location in preparation to assign the default action.
        /// </summary>
        public virtual void DoDefaultAction()
        {
            // If the event binding service is not available, there is nothing much we can do, so just return.
            if (!(GetService(typeof(IEventBindingService)) is IEventBindingService eps))
            {
                return;
            }
            if (!(GetService(typeof(ISelectionService)) is ISelectionService selectionService))
            {
                return;
            }

            ICollection components = selectionService.GetSelectedComponents();
            EventDescriptor thisDefaultEvent = null;
            string thisHandler = null;
            IDesignerHost host = GetService(typeof(IDesignerHost)) as IDesignerHost;
            DesignerTransaction t = null;

            if (components is null)
            {
                return;
            }
            try
            {
                foreach (IComponent comp in components.OfType<IComponent>())
                {
                    EventDescriptor defaultEvent = TypeDescriptor.GetDefaultEvent(comp);
                    PropertyDescriptor defaultPropEvent = null;
                    bool eventChanged = false;

                    if (defaultEvent != null)
                    {
                        defaultPropEvent = eps.GetEventProperty(defaultEvent);
                    }

                    // If we couldn't find a property for this event, or of the property is read only, then abort.
                    if (defaultPropEvent is null || defaultPropEvent.IsReadOnly)
                    {
                        continue;
                    }

                    try
                    {
                        if (host != null && t is null)
                        {
                            t = host.CreateTransaction(string.Format(SR.ComponentDesignerAddEvent, defaultEvent.Name));
                        }
                    }
                    catch (CheckoutException cxe) when (cxe == CheckoutException.Canceled)
                    {
                        return;
                    }

                    // handler will be null if there is no explicit event hookup in the parsed init method
                    object result = defaultPropEvent.GetValue(comp);
                    string handler = result as string;
                    if (handler is null)
                    {
                        // Skip invalid properties.
                        if (result != null)
                        {
                            continue;
                        }

                        eventChanged = true;
                        handler = eps.CreateUniqueMethodName((IComponent)comp, defaultEvent);
                    }
                    else
                    {
                        // ensure the handler is still there
                        eventChanged = true;
                        ICollection methods = eps.GetCompatibleMethods(defaultEvent);
                        if (methods != null)
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
                    //s_codemarkers.CodeMarker(CodeMarkerEvent.perfFXBindEventDesignToCode);
                    if (eventChanged)
                    {
                        defaultPropEvent.SetValue(comp, handler);
                    }

                    if (_component == comp)
                    {
                        thisDefaultEvent = defaultEvent;
                        thisHandler = handler;
                    }
                }
            }
            catch (InvalidOperationException)
            {
                if (t != null)
                {
                    t.Cancel();
                    t = null;
                }
            }
            finally
            {
                if (t != null)
                {
                    t.Commit();
                }
            }

            // Now show the event code.
            if (thisHandler != null && thisDefaultEvent != null)
            {
                eps.ShowCode(_component, thisDefaultEvent);
            }
        }

        internal bool IsRootDesigner
        {
            get
            {
                Debug.Assert(_component != null,
                    "this.component needs to be set before this method is valid.");

                bool isRoot = false;
                IDesignerHost host = (IDesignerHost)GetService(typeof(IDesignerHost));
                if (host != null && _component == host.RootComponent)
                {
                    isRoot = true;
                }
                return isRoot;
            }
        }

        /// <summary>
        ///  Initializes a new instance of the <see cref='System.ComponentModel.Design.ComponentDesigner' /> class using the specified component.
        /// </summary>
        public virtual void Initialize(IComponent component)
        {
            _component = component;

            // For inherited components, save off the current values so we can compute a delta.  We also do this for the root component, but,
            // as it is ALWAYS inherited, the computation of default values favors the presence of a default value attribute over the current code value.
            bool isRoot = false;
            if (GetService(typeof(IDesignerHost)) is IDesignerHost host && component == host.RootComponent)
            {
                isRoot = true;
            }

            if (component?.Site is IServiceContainer sc && GetService(typeof(DesignerCommandSet)) is null)
            {
                sc.AddService(typeof(DesignerCommandSet), new CDDesignerCommandSet(this));
            }

            if (GetService(typeof(IComponentChangeService)) is IComponentChangeService cs)
            {
                cs.ComponentRename += new ComponentRenameEventHandler(OnComponentRename);
            }

            InheritanceAttribute inheritanceAttribute = InheritanceAttribute;
            if (isRoot || InheritanceAttribute is null || !inheritanceAttribute.Equals(InheritanceAttribute.NotInherited))
            {
                InitializeInheritedProperties();
            }
        }

        /// <summary>
        ///  DesignerCommandSet to be used as a site specific service.
        /// </summary>
        private class CDDesignerCommandSet : DesignerCommandSet
        {
            private readonly ComponentDesigner _componentDesigner;

            public CDDesignerCommandSet(ComponentDesigner componentDesigner)
            {
                _componentDesigner = componentDesigner;
            }

            public override ICollection GetCommands(string name)
            {
                if (name == VerbsCommand)
                {
                    return _componentDesigner.Verbs;
                }
                else if (name == ActionListsCommand)
                {
                    return _componentDesigner.ActionLists;
                }
                else
                {
                    return base.GetCommands(name);
                }
            }
        }

        private void InitializeInheritedProperties()
        {
            Hashtable props = new Hashtable();
            InheritanceAttribute inheritanceAttribute = InheritanceAttribute;
            bool readOnlyInherit = inheritanceAttribute != null && inheritanceAttribute.Equals(InheritanceAttribute.InheritedReadOnly);

            if (!readOnlyInherit)
            {
                PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(Component);

                // Now loop through all the properties.  For each one, try to match a pre-created property.
                // If that fails, then create a new property.
                PropertyDescriptor[] values = new PropertyDescriptor[properties.Count];
                properties.CopyTo(values, 0);

                for (int i = 0; i < values.Length; i++)
                {
                    PropertyDescriptor prop = values[i];

                    // Skip some properties
                    if (object.Equals(prop.Attributes[typeof(DesignOnlyAttribute)], DesignOnlyAttribute.Yes))
                    {
                        continue;
                    }

                    if (prop.SerializationVisibility == DesignerSerializationVisibility.Hidden && !prop.IsBrowsable)
                    {
                        continue;
                    }

                    PropertyDescriptor inheritedProp = (PropertyDescriptor)props[prop.Name];

                    if (inheritedProp is null)
                    {
                        // This ia a publicly inherited component.  We replace all component properties with inherited versions that reset the default property values to those that are currently on the component.
                        props[prop.Name] = new InheritedPropertyDescriptor(prop, _component);
                    }
                }
            }

            _inheritedProps = props;
            TypeDescriptor.Refresh(Component); // force TypeDescriptor to re-query us.
        }

        /// <summary>
        ///  Invokes the get inheritance attribute of the specified ComponentDesigner.
        /// </summary>
        protected InheritanceAttribute InvokeGetInheritanceAttribute(ComponentDesigner toInvoke)
        {
            return toInvoke?.InheritanceAttribute;
        }

        /// <summary>
        ///  Disposes of the resources (other than memory) used by the <see cref='System.ComponentModel.Design.ComponentDesigner' />.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (GetService(typeof(IComponentChangeService)) is IComponentChangeService cs)
                {
                    cs.ComponentRename -= new ComponentRenameEventHandler(OnComponentRename);
                }

                _component = null;
                _inheritedProps = null;
            }
        }

        /// <summary>
        ///  Raised when a component's name changes.  Here we update the SettingsKey property if necessary.
        /// </summary>
        private void OnComponentRename(object sender, ComponentRenameEventArgs e)
        {
            if (Component is IPersistComponentSettings)
            {
                IDesignerHost host = GetService(typeof(IDesignerHost)) as IDesignerHost;
                IComponent rootComponent = host?.RootComponent;

                // SettingsKey is formed based on the name of the component and the root component. If either of
                // these change, we reset settings key (if it hasn't been explicitly set) so it can be recomputed.
                if (!_settingsKeyExplicitlySet && (e.Component == Component || e.Component == rootComponent))
                {
                    ResetSettingsKey();
                }
            }
        }

        /// <summary>
        ///  shadowing the SettingsKey so we can default it to be RootComponent.Name + "." + Control.Name
        /// </summary>
        private string SettingsKey
        {
            get
            {
                if (string.IsNullOrEmpty((string)ShadowProperties[SettingsKeyName]))
                {
                    IComponent rootComponent = GetService(typeof(IDesignerHost)) is IDesignerHost host ? host.RootComponent : null;

                    if (Component is IPersistComponentSettings persistableComponent && rootComponent != null)
                    {
                        if (string.IsNullOrEmpty(persistableComponent.SettingsKey))
                        {
                            if (rootComponent != null && rootComponent != persistableComponent)
                            {
                                ShadowProperties[SettingsKeyName] = string.Format(CultureInfo.CurrentCulture, "{0}.{1}", rootComponent.Site.Name, Component.Site.Name);
                            }
                            else
                            {
                                ShadowProperties[SettingsKeyName] = Component.Site.Name;
                            }
                        }
                        persistableComponent.SettingsKey = ShadowProperties[SettingsKeyName] as string;
                        return persistableComponent.SettingsKey;
                    }
                }
                return ShadowProperties[SettingsKeyName] as string;
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
        ///  Called when the designer has been associated with a control that is not in it's default state, such as one that has been pasted or drag-dropped onto the designer.
        ///  This is an opportunity to fixup any shadowed properties in a different way than for default components.
        ///  This is called after the other initialize functions.
        /// </summary>
        [Obsolete("This method has been deprecated. Use InitializeExistingComponent instead.  http://go.microsoft.com/fwlink/?linkid=14202")]
        public virtual void InitializeNonDefault()
        {
        }

        /// <summary>
        ///  Provides a way for a designer to get services from the hosting environment.
        /// </summary>
        protected virtual object GetService(Type serviceType)
        {
            if (_component != null)
            {
                ISite site = _component.Site;
                if (site != null)
                {
                    return site.GetService(serviceType);
                }
            }
            return null;
        }

        /// <summary>
        ///  Raises the SetComponentDefault event.
        /// </summary>
        [Obsolete("This method has been deprecated. Use InitializeNewComponent instead.  http://go.microsoft.com/fwlink/?linkid=14202")]
        public virtual void OnSetComponentDefaults()
        {
            ISite site = Component?.Site;
            if (site != null)
            {
                IComponent component = Component;
                PropertyDescriptor pd = TypeDescriptor.GetDefaultProperty(component);
                if (pd != null && pd.PropertyType.Equals(typeof(string)))
                {
                    string current = (string)pd.GetValue(component);
                    if (current is null || current.Length == 0)
                    {
                        pd.SetValue(component, site.Name);
                    }
                }
            }
        }

        /// <summary>
        ///  Called when the context menu should be displayed
        /// </summary>
        internal virtual void ShowContextMenu(int x, int y)
        {
            IMenuCommandService mcs = (IMenuCommandService)GetService(typeof(IMenuCommandService));
            if (mcs != null)
            {
                mcs.ShowContextMenu(MenuCommands.SelectionMenu, x, y);
            }
        }

        /// <summary>
        ///  Allows a designer to filter the set of member attributes the component it is designing will expose through the TypeDescriptor object.
        /// </summary>
        protected virtual void PostFilterAttributes(IDictionary attributes)
        {
            // If this component is being inherited, mark it as such in the class attributes.
            // Also, set our member variable to ensure that what you get by querying through the TypeDescriptor and through InheritanceAttribute directly is the same.
            if (attributes is null)
            {
                return;
            }

            if (attributes.Contains(typeof(InheritanceAttribute)))
            {
                _inheritanceAttribute = attributes[typeof(InheritanceAttribute)] as InheritanceAttribute;
                return;
            }

            InheritanceAttribute inheritanceAttribute = InheritanceAttribute;
            if (inheritanceAttribute != null && !inheritanceAttribute.Equals(InheritanceAttribute.NotInherited))
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

            InheritanceAttribute inheritanceAttribute = InheritanceAttribute;
            if (inheritanceAttribute is null || !inheritanceAttribute.Equals(InheritanceAttribute.InheritedReadOnly))
            {
                return;
            }

            EventDescriptor[] values = new EventDescriptor[events.Values.Count];
            events.Values.CopyTo(values, 0);

            for (int i = 0; i < values.Length; i++)
            {
                EventDescriptor evt = values[i];
                if (evt != null)
                {
                    events[evt.Name] = TypeDescriptor.CreateEvent(evt.ComponentType, evt, ReadOnlyAttribute.Yes);
                }
            }
        }

        /// <summary>
        ///  Allows a designer to filter the set of properties the component it is designing will expose through the TypeDescriptor object.
        /// </summary>
        protected virtual void PostFilterProperties(IDictionary properties)
        {
            if (_inheritedProps is null)
            {
                return;
            }

            bool readOnlyInherit = InheritanceAttribute.Equals(InheritanceAttribute.InheritedReadOnly);
            if (readOnlyInherit)
            {
                // Now loop through all the properties.  For each one, try to match a pre-created property.
                // If that fails, then create a new property.
                PropertyDescriptor[] values = new PropertyDescriptor[properties.Values.Count];
                properties.Values.CopyTo(values, 0);

                for (int i = 0; i < values.Length; i++)
                {
                    PropertyDescriptor prop = values[i];
                    // This is a private component. Therefore, the user should not be allowed to modify any properties. We replace all properties with read-only versions.
                    properties[prop.Name] = TypeDescriptor.CreateProperty(prop.ComponentType, prop, ReadOnlyAttribute.Yes);
                }

                return;
            }

            // otherwise apply our inherited properties to the actual property list.
            foreach (DictionaryEntry de in _inheritedProps)
            {
                if (de.Value is InheritedPropertyDescriptor inheritedPropDesc)
                {
                    // replace the property descriptor it was created with with the new one in case we're shadowing
                    PropertyDescriptor newInnerProp = (PropertyDescriptor)properties[de.Key];
                    if (newInnerProp != null)
                    {
                        inheritedPropDesc.PropertyDescriptor = newInnerProp;
                        properties[de.Key] = inheritedPropDesc;
                    }
                }
            }
        }

        /// <summary>
        ///  Allows a designer to filter the set of member attributes the component it is designing will expose through the TypeDescriptor object.
        /// </summary>
        protected virtual void PreFilterAttributes(IDictionary attributes)
        {
        }

        /// <summary>
        ///  Allows a designer to filter the set of events the component it is designing will expose through the TypeDescriptor object.
        /// </summary>
        protected virtual void PreFilterEvents(IDictionary events)
        {
        }

        /// <summary>
        ///  Allows a designer to filter the set of properties the component it is designing will expose through the TypeDescriptor object.
        /// </summary>
        protected virtual void PreFilterProperties(IDictionary properties)
        {
            if (Component is IPersistComponentSettings && properties != null)
            {
                if (properties[SettingsKeyName] is PropertyDescriptor prop)
                {
                    properties[SettingsKeyName] = TypeDescriptor.CreateProperty(typeof(ComponentDesigner), prop, Array.Empty<Attribute>());
                }
            }
        }

        /// <summary>
        ///  Notifies the <see cref='System.ComponentModel.Design.IComponentChangeService' /> that this component has been changed.
        ///  You only need to call this when you are affecting component properties directly and not through the MemberDescriptor's accessors.
        /// </summary>
        protected void RaiseComponentChanged(MemberDescriptor member, object oldValue, object newValue)
        {
            if (GetService(typeof(IComponentChangeService)) is IComponentChangeService changeSvc)
            {
                changeSvc.OnComponentChanged(Component, member, oldValue, newValue);
            }
        }

        /// <summary>
        ///  Notifies the <see cref='System.ComponentModel.Design.IComponentChangeService' /> that this component is about to be changed.
        ///  You only need to call this when you are affecting component properties directly and not through the MemberDescriptor's accessors.
        /// </summary>
        protected void RaiseComponentChanging(MemberDescriptor member)
        {
            if (GetService(typeof(IComponentChangeService)) is IComponentChangeService changeSvc)
            {
                changeSvc.OnComponentChanging(Component, member);
            }
        }

        /// <summary>
        ///  Collection that holds shadow properties.
        /// </summary>
        protected sealed class ShadowPropertyCollection
        {
            private readonly ComponentDesigner _designer;
            private Hashtable _properties;
            private Hashtable _descriptors;
            internal ShadowPropertyCollection(ComponentDesigner designer)
            {
                _designer = designer;
            }

            /// <summary>
            ///  Accesses the given property name.  This will throw an exception if the property does not exsit on the base component.
            /// </summary>
            public object this[string propertyName]
            {
                get
                {
                    if (propertyName is null)
                    {
                        throw new ArgumentNullException(nameof(propertyName));
                    }

                    // First, check to see if the name is in the given properties table
                    if (_properties != null && _properties.ContainsKey(propertyName))
                    {
                        return _properties[propertyName];
                    }

                    // Next, check to see if the name is in the descriptors table.  If it isn't, we will search the underlying component and add it.
                    PropertyDescriptor property = GetShadowedPropertyDescriptor(propertyName);

                    return property.GetValue(_designer.Component);
                }
                set
                {
                    if (_properties is null)
                    {
                        _properties = new Hashtable();
                    }
                    _properties[propertyName] = value;
                }
            }

            /// <summary>
            ///  Returns true if this shadow properties object contains the given property name.
            /// </summary>
            public bool Contains(string propertyName)
            {
                return _properties != null && _properties.ContainsKey(propertyName);
            }

            /// <summary>
            ///  Returns the underlying property descriptor for this property on the component
            /// </summary>
            private PropertyDescriptor GetShadowedPropertyDescriptor(string propertyName)
            {
                if (_descriptors is null)
                {
                    _descriptors = new Hashtable();
                }

                PropertyDescriptor property = (PropertyDescriptor)_descriptors[propertyName];
                if (property is null)
                {
                    property = TypeDescriptor.GetProperties(_designer.Component.GetType())[propertyName];
                    //_descriptors[propertyName] = property ?? throw new ArgumentException(SR.GetResourceString(SR.DesignerPropNotFound, propertyName));
                }
                return property;
            }

            /// <summary>
            ///  Returns true if the given property name should be serialized, or false  if not.
            ///  This is useful in implementing your own ShouldSerialize* methods on shadowed properties.
            /// </summary>
            internal bool ShouldSerializeValue(string propertyName, object defaultValue)
            {
                if (propertyName is null)
                {
                    throw new ArgumentNullException(nameof(propertyName));
                }

                if (Contains(propertyName))
                {
                    return !object.Equals(this[propertyName], defaultValue);
                }
                else
                {
                    return GetShadowedPropertyDescriptor(propertyName).ShouldSerializeValue(_designer.Component);
                }
            }
        }
    }
}
