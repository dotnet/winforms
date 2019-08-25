// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.CodeDom;
using System.Collections.Generic;

namespace System.ComponentModel.Design.Serialization
{
    /// <summary>
    ///  This class is used to cache serialized properties and events of components to speed-up Design to Code view switches
    /// </summary>
    internal class ComponentCache : IDisposable
    {
        private Dictionary<object, Entry> _cache;
        private readonly IDesignerSerializationManager _serManager;
        private readonly bool _enabled = true;

        internal ComponentCache(IDesignerSerializationManager manager)
        {
            _serManager = manager;
            if (manager.GetService(typeof(IComponentChangeService)) is IComponentChangeService cs)
            {
                cs.ComponentChanging += new ComponentChangingEventHandler(OnComponentChanging);
                cs.ComponentChanged += new ComponentChangedEventHandler(OnComponentChanged);
                cs.ComponentRemoving += new ComponentEventHandler(OnComponentRemove);
                cs.ComponentRemoved += new ComponentEventHandler(OnComponentRemove);
                cs.ComponentRename += new ComponentRenameEventHandler(OnComponentRename);
            }

            object optionValue = null;
            if (manager.GetService(typeof(DesignerOptionService)) is DesignerOptionService options)
            {
                PropertyDescriptor componentCacheProp = options.Options.Properties["UseOptimizedCodeGeneration"];
                if (componentCacheProp != null)
                {
                    optionValue = componentCacheProp.GetValue(null);
                }

                if (optionValue != null && optionValue is bool)
                {
                    _enabled = (bool)optionValue;
                }
            }
        }

        internal bool Enabled
        {
            get => _enabled;
        }

        /// <summary>
        ///  Access serialized Properties and events for the given component
        /// </summary>
        internal Entry this[object component]
        {
            get
            {
                if (component == null)
                {
                    throw new ArgumentNullException(nameof(component));
                }

                if (_cache != null && _cache.TryGetValue(component, out Entry result))
                {
                    if (result != null && result.Valid && Enabled)
                    {
                        return result;
                    }
                }
                return null;
            }
            set
            {
                if (_cache == null && Enabled)
                {
                    _cache = new Dictionary<object, Entry>();
                }
                // it's a 1:1 relationship so we can go back from entry to  component (if it's not setup yet.. which should not happen, see ComponentCodeDomSerializer.cs::Serialize for more info)
                if (_cache != null && component is IComponent)
                {
                    if (value != null && value.Component == null)
                    {
                        value.Component = component;
                    }
                    _cache[component] = value;
                }
            }
        }

        internal Entry GetEntryAll(object component)
        {
            if (_cache != null && _cache.TryGetValue(component, out Entry result))
            {
                return result;
            }
            return null;
        }

        internal bool ContainsLocalName(string name)
        {
            if (_cache == null)
            {
                return false;
            }

            foreach (KeyValuePair<object, Entry> kvp in _cache)
            {
                List<string> localNames = kvp.Value.LocalNames;
                if (localNames != null && localNames.Contains(name))
                {
                    return true;
                }
            }
            return false;
        }

        public void Dispose()
        {
            if (_serManager != null)
            {
                IComponentChangeService cs = (IComponentChangeService)_serManager.GetService(typeof(IComponentChangeService));
                if (cs != null)
                {
                    cs.ComponentChanging -= new ComponentChangingEventHandler(OnComponentChanging);
                    cs.ComponentChanged -= new ComponentChangedEventHandler(OnComponentChanged);
                    cs.ComponentRemoving -= new ComponentEventHandler(OnComponentRemove);
                    cs.ComponentRemoved -= new ComponentEventHandler(OnComponentRemove);
                    cs.ComponentRename -= new ComponentRenameEventHandler(OnComponentRename);
                }
            }
        }

        private void OnComponentRename(object source, ComponentRenameEventArgs args)
        {
            // we might have a symbolic rename that has side effects beyond our control, so we don't have a choice but to clear the whole cache when a component gets renamed...
            if (_cache != null)
            {
                _cache.Clear();
            }
        }

        private void OnComponentChanging(object source, ComponentChangingEventArgs ce)
        {
            if (_cache != null)
            {
                if (ce.Component != null)
                {
                    RemoveEntry(ce.Component);

                    if (!(ce.Component is IComponent) && _serManager != null)
                    {
                        if (_serManager.GetService(typeof(IReferenceService)) is IReferenceService rs)
                        {
                            IComponent owningComp = rs.GetComponent(ce.Component);
                            if (owningComp != null)
                            {
                                RemoveEntry(owningComp);
                            }
                            else
                            {
                                // Hmm. We were notified about an object change, but were unable to relate it back to a component we know about. In this situation, we have no option but to clear the whole cache, since we don't want serialization to miss something.
                                _cache.Clear();
                            }
                        }
                    }
                }
                else
                {
                    _cache.Clear();
                }
            }
        }

        private void OnComponentChanged(object source, ComponentChangedEventArgs ce)
        {
            if (_cache != null)
            {
                if (ce.Component != null)
                {
                    RemoveEntry(ce.Component);
                    if (!(ce.Component is IComponent) && _serManager != null)
                    {
                        if (_serManager.GetService(typeof(IReferenceService)) is IReferenceService rs)
                        {
                            IComponent owningComp = rs.GetComponent(ce.Component);
                            if (owningComp != null)
                            {
                                RemoveEntry(owningComp);
                            }
                            else
                            {
                                // Hmm. We were notified about an object change, but were unable to relate it back to a component we know about. In this situation, we have no option but to clear the whole cache, since we don't want serialization to miss something.
                                _cache.Clear();
                            }
                        }
                    }
                }
                else
                {
                    _cache.Clear();
                }
            }
        }

        private void OnComponentRemove(object source, ComponentEventArgs ce)
        {
            if (_cache != null)
            {
                if (ce.Component != null && !(ce.Component is IExtenderProvider))
                {
                    RemoveEntry(ce.Component);
                }
                else
                {
                    _cache.Clear();
                }
            }
        }

        /// <summary>
        ///  Helper to remove an entry from the cache.
        /// </summary>
        internal void RemoveEntry(object component)
        {
            if (_cache != null && _cache.TryGetValue(component, out Entry entry))
            {
                if (entry.Tracking)
                {
                    _cache.Clear();
                    return;
                }

                _cache.Remove(component);
                // Clear its dependencies, if any
                if (entry.Dependencies != null)
                {
                    foreach (object parent in entry.Dependencies)
                    {
                        RemoveEntry(parent);
                    }
                }

            }
        }

        internal struct ResourceEntry
        {
            public bool ForceInvariant;
            public bool EnsureInvariant;
            public bool ShouldSerializeValue;
            public string Name;
            public object Value;
            public PropertyDescriptor PropertyDescriptor;
            public ExpressionContext ExpressionContext;
        }

        // A single cache entry
        internal sealed class Entry
        {
            private readonly ComponentCache _cache;
            private List<object> _dependencies;
            private List<string> _localNames;
            private List<ResourceEntry> _resources;
            private List<ResourceEntry> _metadata;
            private bool _valid;
            private bool _tracking;

            internal Entry(ComponentCache cache)
            {
                _cache = cache;
                _valid = true;
            }

            public object Component; // pointer back to the component that generated this entry
            public CodeStatementCollection Statements;

            public ICollection<ResourceEntry> Metadata
            {
                get => _metadata;
            }

            public ICollection<ResourceEntry> Resources
            {
                get => _resources;
            }

            public List<object> Dependencies
            {
                get => _dependencies;
            }

            internal List<string> LocalNames
            {
                get => _localNames;
            }

            internal bool Valid
            {
                get => _valid;
                set => _valid = value;
            }

            internal bool Tracking
            {
                get => _tracking;
                set => _tracking = value;
            }

            internal void AddLocalName(string name)
            {
                if (_localNames == null)
                {
                    _localNames = new List<string>();
                }

                _localNames.Add(name);
            }

            public void AddDependency(object dep)
            {
                if (_dependencies == null)
                {
                    _dependencies = new List<object>();
                }

                if (!_dependencies.Contains(dep))
                {
                    _dependencies.Add(dep);
                }
            }

            public void AddMetadata(ResourceEntry re)
            {
                if (_metadata == null)
                {
                    _metadata = new List<ResourceEntry>();
                }
                _metadata.Add(re);
            }

            public void AddResource(ResourceEntry re)
            {
                if (_resources == null)
                {
                    _resources = new List<ResourceEntry>();
                }
                _resources.Add(re);
            }
        }
    }
}
