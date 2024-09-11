// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel.Design.Serialization;

namespace System.ComponentModel.Design;

internal sealed partial class DesignerHost
{
    /// <summary>
    ///  Site is the site we use at design time when we host components.
    /// </summary>
    internal class Site : ISite, IServiceContainer, IDictionaryService
    {
        private readonly IComponent _component;
        private Dictionary<object, object>? _dictionary;
        private readonly DesignerHost _host;
        private string? _name;
        private bool _disposed;
        private SiteNestedContainer? _nestedContainer;
        private readonly Container _container;

        internal Site(IComponent component, DesignerHost host, string? name, Container container)
        {
            _component = component;
            _host = host;
            _name = name;
            _container = container;
        }

        /// <summary>
        ///  Used by the IServiceContainer implementation to return a container-specific service container.
        /// </summary>
        private IServiceContainer SiteServiceContainer
        {
            get
            {
                SiteNestedContainer? nc = (((IServiceProvider)this).GetService(typeof(INestedContainer)) as SiteNestedContainer);
                Debug.Assert(nc is not null, "We failed to resolve a nested container.");
                IServiceContainer? sc = (nc.GetServiceInternal(typeof(IServiceContainer)) as IServiceContainer);
                Debug.Assert(sc is not null, "We failed to resolve a service container from the nested container.");
                return sc;
            }
        }

        /// <summary>
        ///  Retrieves the key corresponding to the given value.
        /// </summary>
        object? IDictionaryService.GetKey(object? value)
        {
            if (value is not null && _dictionary is not null)
            {
                foreach (KeyValuePair<object, object> de in _dictionary)
                {
                    object o = de.Value;
                    if (value.Equals(o))
                    {
                        return de.Key;
                    }
                }
            }

            return null;
        }

        /// <summary>
        ///  Retrieves the value corresponding to the given key.
        /// </summary>
        object? IDictionaryService.GetValue(object key)
        {
            if (_dictionary is null || !_dictionary.TryGetValue(key, out object? value))
            {
                return null;
            }

            return value;
        }

        /// <summary>
        ///  Stores the given key-value pair in an object's site. This key-value pair is stored on a per-object basis,
        ///  and is a handy place to save additional information about a component.
        /// </summary>
        void IDictionaryService.SetValue(object key, object? value)
        {
            _dictionary ??= [];

            if (value is null)
            {
                _dictionary.Remove(key);
            }
            else
            {
                _dictionary[key] = value;
            }
        }

        /// <summary>
        ///  Adds the given service to the service container.
        /// </summary>
        void IServiceContainer.AddService(Type serviceType, object serviceInstance)
        {
            SiteServiceContainer.AddService(serviceType, serviceInstance);
        }

        /// <summary>
        ///  Adds the given service to the service container.
        /// </summary>
        void IServiceContainer.AddService(Type serviceType, object serviceInstance, bool promote)
        {
            SiteServiceContainer.AddService(serviceType, serviceInstance, promote);
        }

        /// <summary>
        ///  Adds the given service to the service container.
        /// </summary>
        void IServiceContainer.AddService(Type serviceType, ServiceCreatorCallback callback)
        {
            SiteServiceContainer.AddService(serviceType, callback);
        }

        /// <summary>
        ///  Adds the given service to the service container.
        /// </summary>
        void IServiceContainer.AddService(Type serviceType, ServiceCreatorCallback callback, bool promote)
        {
            SiteServiceContainer.AddService(serviceType, callback, promote);
        }

        /// <summary>
        ///  Removes the given service type from the service container.
        /// </summary>
        void IServiceContainer.RemoveService(Type serviceType)
        {
            SiteServiceContainer.RemoveService(serviceType);
        }

        /// <summary>
        ///  Removes the given service type from the service container.
        /// </summary>
        void IServiceContainer.RemoveService(Type serviceType, bool promote)
        {
            SiteServiceContainer.RemoveService(serviceType, promote);
        }

        /// <summary>
        ///  Returns the requested service.
        /// </summary>
        object? IServiceProvider.GetService(Type service)
        {
            ArgumentNullException.ThrowIfNull(service);

            // We always resolve IDictionaryService to ourselves.
            if (service == typeof(IDictionaryService))
            {
                return this;
            }

            // NestedContainer is demand created
            if (service == typeof(INestedContainer))
            {
                if (_nestedContainer is null)
                {
                    _nestedContainer = new SiteNestedContainer(_component, null, _host);

                    // Initialize IServiceContainer in the nested container as soon as INestedContainer is created,
                    // otherwise site has no access to the DesignerHost's services.
                    _ = _nestedContainer.GetServiceInternal(typeof(IServiceContainer));
                }

                return _nestedContainer;
            }

            // SiteNestedContainer does offer IServiceContainer and IContainer as services, but we always want a default
            // site query for these services to delegate to the host. Because it is more common to add services to the
            // host than it is to add them to the site itself, and also because we need this for backward compatibility.
            if (service != typeof(IServiceContainer) && service != typeof(IContainer) && _nestedContainer is not null)
            {
                return _nestedContainer.GetServiceInternal(service);
            }

            return _host.GetService(service);
        }

        /// <summary>
        ///  The component sited by this component site.
        /// </summary>
        IComponent ISite.Component => _component;

        /// <summary>
        ///  The container in which the component is sited.
        /// </summary>
        IContainer ISite.Container => _container;

        /// <summary>
        ///  Indicates whether the component is in design mode.
        /// </summary>
        bool ISite.DesignMode => true;

        /// <summary>
        ///  Indicates whether this Site has been disposed.
        /// </summary>
        internal bool Disposed
        {
            get => _disposed;
            set
            {
                _disposed = value;

                // We need to do the cleanup when the site is set as disposed by its user
                if (_disposed)
                {
                    _dictionary = null;
                }
            }
        }

        /// <summary>
        ///  The name of the component.
        /// </summary>
        string? ISite.Name
        {
            get => _name;
            set
            {
                value ??= string.Empty;

                if (_name == value)
                {
                    return;
                }

                bool validateName = true;
                if (value.Length > 0)
                {
                    IComponent? namedComponent = _container.Components[value];
                    validateName = _component != namedComponent;

                    // Sllow renames that are just case changes of the current name.
                    if (namedComponent is not null && validateName)
                    {
                        throw new InvalidOperationException(string.Format(SR.DesignerHostDuplicateName, value))
                        {
                            HelpLink = SR.DesignerHostDuplicateName
                        };
                    }
                }

                if (validateName && this.TryGetService(out INameCreationService? nameService))
                {
                    nameService.ValidateName(value);
                }

                // It is OK to change the name to this value. Announce the change and do it.
                string? oldName = _name;
                _name = value;
                _host.OnComponentRename(_component, oldName, _name);
            }
        }
    }
}
