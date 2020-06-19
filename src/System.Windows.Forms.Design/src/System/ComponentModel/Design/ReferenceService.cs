// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Diagnostics;
using System.Globalization;

namespace System.ComponentModel.Design
{
    /// <summary>
    ///  This service allows clients to work with all references on a form, not just the top-level sited components.
    /// </summary>
    internal sealed class ReferenceService : IReferenceService, IDisposable
    {
        private static readonly Attribute[] _attributes = new Attribute[] { DesignerSerializationVisibilityAttribute.Content };

        private IServiceProvider _provider; // service provider we use to get to other services
        private ArrayList _addedComponents; // list of newly added components
        private ArrayList _removedComponents; // list of newly removed components
        private ArrayList _references; // our current list of references
        private bool _populating;

        /// <summary>
        ///  Constructs the ReferenceService.
        /// </summary>
        internal ReferenceService(IServiceProvider provider)
        {
            _provider = provider;
        }

        /// <summary>
        ///  Creates an entry for a top-level component and it's children.
        /// </summary>
        private void CreateReferences(IComponent component)
        {
            CreateReferences(string.Empty, component, component);
        }

        /// <summary>
        ///  Recursively creates references for namespaced objects.
        /// </summary>
        private void CreateReferences(string trailingName, object reference, IComponent sitedComponent)
        {
            if (reference is null)
            {
                return;
            }

            _references.Add(new ReferenceHolder(trailingName, reference, sitedComponent));

            foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(reference, _attributes))
            {
                if (property.IsReadOnly)
                {
                    CreateReferences(string.Format(CultureInfo.CurrentCulture, "{0}.{1}", trailingName, property.Name), property.GetValue(reference), sitedComponent);
                }
            }
        }

        /// <summary>
        ///  Demand populates the _references variable.
        /// </summary>
        private void EnsureReferences()
        {
            // If the references are null, create them for the first time and connect up our events to listen to changes to the container. Otherwise, check to see if the added or removed lists contain anything for us to sync up.
            if (_references is null)
            {
                if (_provider is null)
                {
                    throw new ObjectDisposedException("IReferenceService");
                }

                IComponentChangeService cs = _provider.GetService(typeof(IComponentChangeService)) as IComponentChangeService;
                Debug.Assert(cs != null, "Reference service relies on IComponentChangeService");
                if (cs != null)
                {
                    cs.ComponentAdded += new ComponentEventHandler(OnComponentAdded);
                    cs.ComponentRemoved += new ComponentEventHandler(OnComponentRemoved);
                    cs.ComponentRename += new ComponentRenameEventHandler(OnComponentRename);
                }

                if (!(_provider.GetService(typeof(IContainer)) is IContainer container))
                {
                    Debug.Fail("Reference service cannot operate without IContainer");
                    throw new InvalidOperationException();
                }

                _references = new ArrayList(container.Components.Count);
                foreach (IComponent component in container.Components)
                {
                    CreateReferences(component);
                }
            }
            else if (!_populating)
            {
                _populating = true;
                try
                {
                    if (_addedComponents != null && _addedComponents.Count > 0)
                    {
                        // There is a possibility that this component already exists. If it does, just remove it first and then re-add it.
                        foreach (IComponent ic in _addedComponents)
                        {
                            RemoveReferences(ic);
                            CreateReferences(ic);
                        }
                        _addedComponents.Clear();
                    }

                    if (_removedComponents != null && _removedComponents.Count > 0)
                    {
                        foreach (IComponent ic in _removedComponents)
                        {
                            RemoveReferences(ic);
                        }
                        _removedComponents.Clear();
                    }
                }
                finally
                {
                    _populating = false;
                }
            }
        }

        /// <summary>
        ///  Listens for component additions to find all the references it contributes.
        /// </summary>
        private void OnComponentAdded(object sender, ComponentEventArgs cevent)
        {
            if (_addedComponents is null)
            {
                _addedComponents = new ArrayList();
            }

            IComponent compAdded = cevent.Component;
            if (!(compAdded.Site is INestedSite))
            {
                _addedComponents.Add(compAdded);
                if (_removedComponents != null)
                {
                    _removedComponents.Remove(compAdded);
                }
            }
        }

        /// <summary>
        ///  Listens for component removes to delete all the references it holds.
        /// </summary>
        private void OnComponentRemoved(object sender, ComponentEventArgs cevent)
        {
            if (_removedComponents is null)
            {
                _removedComponents = new ArrayList();
            }

            IComponent compRemoved = cevent.Component;
            if (!(compRemoved.Site is INestedSite))
            {
                _removedComponents.Add(compRemoved);
                if (_addedComponents != null)
                {
                    _addedComponents.Remove(compRemoved);
                }
            }
        }

        /// <summary>
        ///  Listens for component removes to delete all the references it holds.
        /// </summary>
        private void OnComponentRename(object sender, ComponentRenameEventArgs cevent)
        {
            foreach (ReferenceHolder reference in _references)
            {
                if (object.ReferenceEquals(reference.SitedComponent, cevent.Component))
                {
                    reference.ResetName();
                    return;
                }
            }
        }

        /// <summary>
        ///  Removes all the references that this component owns.
        /// </summary>
        private void RemoveReferences(IComponent component)
        {
            if (_references != null)
            {
                int size = _references.Count;
                for (int i = size - 1; i >= 0; i--)
                {
                    if (object.ReferenceEquals(((ReferenceHolder)_references[i]).SitedComponent, component))
                    {
                        _references.RemoveAt(i);
                    }
                }
            }
        }

        /// <summary>
        ///  Cleanup and detach from our events.
        /// </summary>
        void IDisposable.Dispose()
        {
            if (_references != null && _provider != null)
            {
                if (_provider.GetService(typeof(IComponentChangeService)) is IComponentChangeService cs)
                {
                    cs.ComponentAdded -= new ComponentEventHandler(OnComponentAdded);
                    cs.ComponentRemoved -= new ComponentEventHandler(OnComponentRemoved);
                    cs.ComponentRename -= new ComponentRenameEventHandler(OnComponentRename);
                }
                _references = null;
                _provider = null;
            }
        }

        /// <summary>
        ///  Finds the sited component for a given reference, returning null if not found.
        /// </summary>
        IComponent IReferenceService.GetComponent(object reference)
        {
            if (reference is null)
            {
                throw new ArgumentNullException(nameof(reference));
            }

            EnsureReferences();
            foreach (ReferenceHolder holder in _references)
            {
                if (object.ReferenceEquals(holder.Reference, reference))
                {
                    return holder.SitedComponent;
                }
            }
            return null;
        }

        /// <summary>
        ///  Finds name for a given reference, returning null if not found.
        /// </summary>
        string IReferenceService.GetName(object reference)
        {
            if (reference is null)
            {
                throw new ArgumentNullException(nameof(reference));
            }

            EnsureReferences();
            foreach (ReferenceHolder holder in _references)
            {
                if (object.ReferenceEquals(holder.Reference, reference))
                {
                    return holder.Name;
                }
            }

            return null;
        }

        /// <summary>
        ///  Finds a reference with the given name, returning null if not found.
        /// </summary>
        object IReferenceService.GetReference(string name)
        {
            if (name is null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            EnsureReferences();
            foreach (ReferenceHolder holder in _references)
            {
                if (string.Equals(holder.Name, name, StringComparison.OrdinalIgnoreCase))
                {
                    return holder.Reference;
                }
            }
            return null;
        }

        /// <summary>
        ///  Returns all references available in this designer.
        /// </summary>
        object[] IReferenceService.GetReferences()
        {
            EnsureReferences();
            object[] references = new object[_references.Count];

            for (int i = 0; i < references.Length; i++)
            {
                references[i] = ((ReferenceHolder)_references[i]).Reference;
            }
            return references;
        }

        /// <summary>
        ///  Returns all references available in this designer that are assignable to the given type.
        /// </summary>
        object[] IReferenceService.GetReferences(Type baseType)
        {
            if (baseType is null)
            {
                throw new ArgumentNullException(nameof(baseType));
            }

            EnsureReferences();
            ArrayList results = new ArrayList(_references.Count);

            foreach (ReferenceHolder holder in _references)
            {
                object reference = holder.Reference;
                if (baseType.IsAssignableFrom(reference.GetType()))
                {
                    results.Add(reference);
                }
            }

            object[] references = new object[results.Count];
            results.CopyTo(references, 0);
            return references;
        }

        /// <summary>
        ///  The class that holds the information about a reference.
        /// </summary>
        private sealed class ReferenceHolder
        {
            private readonly string _trailingName;
            private readonly object _reference;
            private readonly IComponent _sitedComponent;
            private string _fullName;

            /// <summary>
            ///  Creates a new reference holder.
            /// </summary>
            internal ReferenceHolder(string trailingName, object reference, IComponent sitedComponent)
            {
                _trailingName = trailingName;
                _reference = reference;
                _sitedComponent = sitedComponent;

                Debug.Assert(trailingName != null, "Expected a trailing name");
                Debug.Assert(reference != null, "Expected a reference");
#if DEBUG
                Debug.Assert(sitedComponent != null, "Expected a sited component");
                if (sitedComponent != null)
                {
                    Debug.Assert(sitedComponent.Site != null, "Sited component is not really sited: " + sitedComponent.ToString());
                }

                if (sitedComponent != null)
                {
                    Debug.Assert(TypeDescriptor.GetComponentName(sitedComponent) != null, "Sited component has no name: " + sitedComponent.ToString());
                }
#endif // DEBUG
            }

            /// <summary>
            ///  Resets the name of this reference holder.  It will be re-acquired on demand
            /// </summary>
            internal void ResetName()
            {
                _fullName = null;
            }

            /// <summary>
            ///  The name of the reference we are holding.
            /// </summary>
            internal string Name
            {
                get
                {
                    if (_fullName is null)
                    {
                        if (_sitedComponent != null)
                        {
                            string siteName = TypeDescriptor.GetComponentName(_sitedComponent);
                            if (siteName != null)
                            {
                                _fullName = string.Format(CultureInfo.CurrentCulture, "{0}{1}", siteName, _trailingName);
                            }
                        }

                        if (_fullName is null)
                        {
                            _fullName = string.Empty;
#if DEBUG
                            if (_sitedComponent != null)
                            {
                                Debug.Assert(_sitedComponent.Site != null, "Sited component is not really sited: " + _sitedComponent.ToString());
                            }

                            if (_sitedComponent != null)
                            {
                                Debug.Assert(TypeDescriptor.GetComponentName(_sitedComponent) != null, "Sited component has no name: " + _sitedComponent.ToString());
                            }
#endif // DEBUG
                        }
                    }
                    return _fullName;
                }
            }

            /// <summary>
            ///  The reference we are holding.
            /// </summary>
            internal object Reference
            {
                get => _reference;
            }

            /// <summary>
            ///  The sited component associated with this reference.
            /// </summary>
            internal IComponent SitedComponent
            {
                get => _sitedComponent;
            }
        }
    }
}
