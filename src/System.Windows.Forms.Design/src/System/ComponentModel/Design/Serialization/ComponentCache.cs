// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.CodeDom;

namespace System.ComponentModel.Design.Serialization;

/// <summary>
///  This class is used to cache serialized properties and events of components to speed-up Design to Code view switches
/// </summary>
internal sealed partial class ComponentCache : IDisposable
{
    private Dictionary<object, Entry>? _cache;
    private readonly IDesignerSerializationManager _serManager;

    internal ComponentCache(IDesignerSerializationManager manager)
    {
        _serManager = manager;
        if (manager.GetService(typeof(IComponentChangeService)) is IComponentChangeService cs)
        {
            cs.ComponentChanging += OnComponentChanging;
            cs.ComponentChanged += OnComponentChanged;
            cs.ComponentRemoving += OnComponentRemove;
            cs.ComponentRemoved += OnComponentRemove;
            cs.ComponentRename += OnComponentRename;
        }

        if (manager.TryGetService(out DesignerOptionService? options))
        {
            PropertyDescriptor? componentCacheProp = options.Options.Properties["UseOptimizedCodeGeneration"];
            object? optionValue = componentCacheProp?.GetValue(null);

            if (optionValue is bool boolValue)
            {
                Enabled = boolValue;
            }
        }
    }

    internal bool Enabled { get; } = true;

    /// <summary>
    ///  Access serialized Properties and events for the given component
    /// </summary>
    [DisallowNull]
    internal Entry? this[object component]
    {
        get
        {
            ArgumentNullException.ThrowIfNull(component);

            return Enabled && _cache is not null && _cache.TryGetValue(component, out Entry? result) && result.Valid
                ? result
                : null;
        }
        set
        {
            if (Enabled)
            {
                _cache ??= [];
            }

            // it's a 1:1 relationship so we can go back from entry to component
            // (if it's not setup yet.. which should not happen,
            // see ComponentCodeDomSerializer.cs::Serialize for more info)
            if (_cache is not null && component is IComponent)
            {
                value.Component ??= component;
                _cache[component] = value;
            }
        }
    }

    internal Entry? GetEntryAll(object component)
    {
        if (_cache is not null && _cache.TryGetValue(component, out Entry? result))
        {
            return result;
        }

        return null;
    }

    internal bool ContainsLocalName(string name)
    {
        if (_cache is null)
        {
            return false;
        }

        foreach (KeyValuePair<object, Entry> kvp in _cache)
        {
            List<string>? localNames = kvp.Value.LocalNames;
            if (localNames is not null && localNames.Contains(name))
            {
                return true;
            }
        }

        return false;
    }

    public void Dispose()
    {
        if (_serManager.TryGetService(out IComponentChangeService? cs))
        {
            cs.ComponentChanging -= OnComponentChanging;
            cs.ComponentChanged -= OnComponentChanged;
            cs.ComponentRemoving -= OnComponentRemove;
            cs.ComponentRemoved -= OnComponentRemove;
            cs.ComponentRename -= OnComponentRename;
        }
    }

    private void OnComponentRename(object? source, ComponentRenameEventArgs? args)
    {
        // we might have a symbolic rename that has side effects beyond our control,
        // so we don't have a choice but to clear the whole cache when a component gets renamed...
        _cache?.Clear();
    }

    private void OnComponentChanging(object? source, ComponentChangingEventArgs ce)
    {
        if (_cache is not null)
        {
            if (ce.Component is not null)
            {
                RemoveEntry(ce.Component);

                if (ce.Component is not IComponent)
                {
                    if (_serManager.TryGetService(out IReferenceService? rs))
                    {
                        IComponent? owningComp = rs.GetComponent(ce.Component);
                        if (owningComp is not null)
                        {
                            RemoveEntry(owningComp);
                        }
                        else
                        {
                            // Hmm. We were notified about an object change, but were unable to relate it back to a
                            // component we know about. In this situation, we have no option but to clear the whole
                            // cache, since we don't want serialization to miss something.
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

    private void OnComponentChanged(object? source, ComponentChangedEventArgs ce)
    {
        if (_cache is not null)
        {
            if (ce.Component is not null)
            {
                RemoveEntry(ce.Component);
                if (ce.Component is not IComponent)
                {
                    if (_serManager.TryGetService(out IReferenceService? rs))
                    {
                        IComponent? owningComp = rs.GetComponent(ce.Component);
                        if (owningComp is not null)
                        {
                            RemoveEntry(owningComp);
                        }
                        else
                        {
                            // Hmm. We were notified about an object change, but were unable to relate it back to a
                            // component we know about. In this situation, we have no option but to clear the whole
                            // cache, since we don't want serialization to miss something.
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

    private void OnComponentRemove(object? source, ComponentEventArgs ce)
    {
        if (_cache is not null)
        {
            if (ce.Component is not null and not IExtenderProvider)
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
        if (_cache is not null && _cache.TryGetValue(component, out Entry? entry))
        {
            if (entry.Tracking)
            {
                _cache.Clear();
                return;
            }

            _cache.Remove(component);
            // Clear its dependencies, if any
            if (entry.Dependencies is not null)
            {
                foreach (object? parent in entry.Dependencies)
                {
                    RemoveEntry(parent);
                }
            }
        }
    }

    // A single cache entry
    internal sealed class Entry
    {
        private List<ResourceEntry>? _resources;
        private List<ResourceEntry>? _metadata;

        internal Entry()
        {
            Valid = true;
        }

        public object? Component; // pointer back to the component that generated this entry
        public CodeStatementCollection Statements = [];

        public ICollection<ResourceEntry>? Metadata => _metadata;

        public ICollection<ResourceEntry>? Resources => _resources;

        public List<object>? Dependencies { get; private set; }

        internal List<string>? LocalNames { get; private set; }

        internal bool Valid { get; set; }

        internal bool Tracking { get; set; }

        internal void AddLocalName(string name)
        {
            LocalNames ??= [];

            LocalNames.Add(name);
        }

        public void AddDependency(object dep)
        {
            Dependencies ??= [];

            if (!Dependencies.Contains(dep))
            {
                Dependencies.Add(dep);
            }
        }

        public void AddMetadata(ResourceEntry re)
        {
            _metadata ??= [];

            _metadata.Add(re);
        }

        public void AddResource(ResourceEntry re)
        {
            _resources ??= [];

            _resources.Add(re);
        }
    }
}
