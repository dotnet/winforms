// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.ComponentModel.Design;

/// <summary>
///  This service allows clients to work with all references on a form, not just the top-level sited components.
/// </summary>
internal sealed class ReferenceService : IReferenceService, IDisposable
{
    private static readonly Attribute[] s_attributes = [DesignerSerializationVisibilityAttribute.Content];

    private IServiceProvider _provider; // service provider we use to get to other services
    private List<IComponent>? _addedComponents; // list of newly added components
    private List<IComponent>? _removedComponents; // list of newly removed components
    private List<ReferenceHolder>? _references; // our current list of references
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
    private static void CreateReferences(IComponent component, List<ReferenceHolder> references)
    {
        CreateReferences(string.Empty, component, component, references);
    }

    /// <summary>
    ///  Recursively creates references for namespaced objects.
    /// </summary>
    private static void CreateReferences(string trailingName, object? reference, IComponent sitedComponent, List<ReferenceHolder> references)
    {
        if (reference is null)
        {
            return;
        }

        references.Add(new ReferenceHolder(trailingName, reference, sitedComponent));

        foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(reference, s_attributes))
        {
            if (property.IsReadOnly)
            {
                CreateReferences($"{trailingName}.{property.Name}", property.GetValue(reference), sitedComponent, references);
            }
        }
    }

    /// <summary>
    ///  Demand populates the _references variable.
    /// </summary>
    [MemberNotNull(nameof(_references))]
    private void EnsureReferences()
    {
        // If the references are null, create them for the first time and
        // connect up our events to listen to changes to the container.
        // Otherwise, check to see if the added or removed lists contain anything for us to sync up.
        if (_references is null)
        {
            ObjectDisposedException.ThrowIf(_provider is null, typeof(IReferenceService));

            IComponentChangeService? cs = _provider.GetService<IComponentChangeService>();
            Debug.Assert(cs is not null, "Reference service relies on IComponentChangeService");
            if (cs is not null)
            {
                cs.ComponentAdded += OnComponentAdded;
                cs.ComponentRemoved += OnComponentRemoved;
                cs.ComponentRename += OnComponentRename;
            }

            if (_provider.GetService(typeof(IContainer)) is not IContainer container)
            {
                Debug.Fail("Reference service cannot operate without IContainer");
                throw new InvalidOperationException();
            }

            _references = new(container.Components.Count);
            foreach (IComponent component in container.Components)
            {
                CreateReferences(component, _references);
            }
        }
        else if (!_populating)
        {
            _populating = true;
            try
            {
                if (_addedComponents is not null && _addedComponents.Count > 0)
                {
                    // There is a possibility that this component already exists. If it does, just remove it first and then re-add it.
                    foreach (IComponent ic in _addedComponents)
                    {
                        RemoveReferences(ic);
                        CreateReferences(ic, _references);
                    }

                    _addedComponents.Clear();
                }

                if (_removedComponents is not null && _removedComponents.Count > 0)
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
    [MemberNotNull(nameof(_addedComponents))]
    private void OnComponentAdded(object? sender, ComponentEventArgs cevent)
    {
        _addedComponents ??= [];
        IComponent compAdded = cevent.Component!;
        if (compAdded.Site is not INestedSite)
        {
            _addedComponents.Add(compAdded);
            _removedComponents?.Remove(compAdded);
        }
    }

    /// <summary>
    ///  Listens for component removes to delete all the references it holds.
    /// </summary>
    [MemberNotNull(nameof(_removedComponents))]
    private void OnComponentRemoved(object? sender, ComponentEventArgs cevent)
    {
        _removedComponents ??= [];
        IComponent compRemoved = cevent.Component!;
        if (compRemoved.Site is not INestedSite)
        {
            _removedComponents.Add(compRemoved);
            _addedComponents?.Remove(compRemoved);
        }
    }

    /// <summary>
    ///  Listens for component removes to delete all the references it holds.
    /// </summary>
    private void OnComponentRename(object? sender, ComponentRenameEventArgs cevent)
    {
        foreach (ReferenceHolder reference in _references!)
        {
            if (ReferenceEquals(reference.SitedComponent, cevent.Component))
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
        if (_references is not null)
        {
            int size = _references.Count;
            for (int i = size - 1; i >= 0; i--)
            {
                if (ReferenceEquals(_references[i].SitedComponent, component))
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
        if (_references is not null)
        {
            if (_provider.TryGetService(out IComponentChangeService? cs))
            {
                cs.ComponentAdded -= OnComponentAdded;
                cs.ComponentRemoved -= OnComponentRemoved;
                cs.ComponentRename -= OnComponentRename;
            }

            _references = null;
            _provider = null!;
        }
    }

    /// <summary>
    ///  Finds the sited component for a given reference, returning null if not found.
    /// </summary>
    IComponent? IReferenceService.GetComponent(object reference)
    {
        ArgumentNullException.ThrowIfNull(reference);

        EnsureReferences();
        foreach (ReferenceHolder holder in _references)
        {
            if (ReferenceEquals(holder.Reference, reference))
            {
                return holder.SitedComponent;
            }
        }

        return null;
    }

    /// <summary>
    ///  Finds name for a given reference, returning null if not found.
    /// </summary>
    string? IReferenceService.GetName(object reference)
    {
        ArgumentNullException.ThrowIfNull(reference);

        EnsureReferences();
        foreach (ReferenceHolder holder in _references)
        {
            if (ReferenceEquals(holder.Reference, reference))
            {
                return holder.Name;
            }
        }

        return null;
    }

    /// <summary>
    ///  Finds a reference with the given name, returning null if not found.
    /// </summary>
    object? IReferenceService.GetReference(string name)
    {
        ArgumentNullException.ThrowIfNull(name);

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
            references[i] = _references[i].Reference;
        }

        return references;
    }

    /// <summary>
    ///  Returns all references available in this designer that are assignable to the given type.
    /// </summary>
    object[] IReferenceService.GetReferences(Type baseType)
    {
        ArgumentNullException.ThrowIfNull(baseType);

        EnsureReferences();
        List<object> results = new(_references.Count);
        foreach (ReferenceHolder holder in _references)
        {
            object reference = holder.Reference;
            if (baseType.IsInstanceOfType(reference))
            {
                results.Add(reference);
            }
        }

        return [.. results];
    }

    /// <summary>
    ///  The class that holds the information about a reference.
    /// </summary>
    private sealed class ReferenceHolder
    {
        private readonly string _trailingName;
        private string? _fullName;

        /// <summary>
        ///  Creates a new reference holder.
        /// </summary>
        internal ReferenceHolder(string trailingName, object reference, IComponent sitedComponent)
        {
            _trailingName = trailingName;
            Reference = reference;
            SitedComponent = sitedComponent;

            Debug.Assert(trailingName is not null, "Expected a trailing name");
            Debug.Assert(reference is not null, "Expected a reference");
            Debug.Assert(sitedComponent is not null, "Expected a sited component");
            Debug.Assert(sitedComponent.Site is not null, $"Sited component is not really sited: {sitedComponent}");
            Debug.Assert(TypeDescriptor.GetComponentName(sitedComponent) is not null, $"Sited component has no name: {sitedComponent}");
        }

        /// <summary>
        ///  Resets the name of this reference holder. It will be re-acquired on demand
        /// </summary>
        internal void ResetName()
        {
            _fullName = null;
        }

        /// <summary>
        ///  The name of the reference we are holding.
        /// </summary>
        [MemberNotNull(nameof(_fullName))]
        internal string Name
        {
            get
            {
                if (_fullName is null)
                {
                    if (SitedComponent is not null)
                    {
                        string? siteName = TypeDescriptor.GetComponentName(SitedComponent);
                        if (siteName is not null)
                        {
                            _fullName = $"{siteName}{_trailingName}";
                        }

                        Debug.Assert(SitedComponent.Site is not null, $"Sited component is not really sited: {SitedComponent}");
                        Debug.Assert(siteName is not null, $"Sited component has no name: {SitedComponent}");
                    }

                    _fullName ??= string.Empty;
                }

                return _fullName;
            }
        }

        /// <summary>
        ///  The reference we are holding.
        /// </summary>
        internal object Reference { get; }

        /// <summary>
        ///  The sited component associated with this reference.
        /// </summary>
        internal IComponent SitedComponent { get; }
    }
}
