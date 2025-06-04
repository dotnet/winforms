// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Windows.Forms.Design;

namespace System.ComponentModel.Design;

/// <summary>
///  The DesignerActionService manages DesignerActions. All DesignerActions are associated with an object.
///  DesignerActions can be added or removed at any given time. The DesignerActionService controls the expiration
///  of DesignerActions by monitoring three basic events: selection change, component change, and timer expiration.
///  Designer implementing this service will need to monitor the DesignerActionsChanged event on this class.
///  This event will fire every time a change is made to any object's DesignerActions.
/// </summary>
public class DesignerActionService : IDisposable
{
    private readonly Dictionary<IComponent, DesignerActionListCollection> _designerActionLists; // this is how we store 'em. Syntax: key = object, value = DesignerActionListCollection
    private DesignerActionListsChangedEventHandler? _designerActionListsChanged;
    private readonly IServiceProvider? _serviceProvider; // standard service provider
    private readonly ISelectionService? _selectionService; // selection service
    // HashSet of components which have events hooked up.
    private readonly HashSet<IComponent> _componentToVerbsEventHookedUp;
    // Guard against ReEntrant Code. The Infragistics TabControlDesigner, Sets the Commands Status when the
    // Verbs property is accessed. This property is used in the OnVerbStatusChanged code here and hence causes
    // recursion leading to Stack Overflow Exception.
    private bool _reEntrantCode;

    /// <summary>
    ///  Standard constructor. A Service Provider is necessary for monitoring selection and component changes.
    /// </summary>
    public DesignerActionService(IServiceProvider? serviceProvider)
    {
        if (serviceProvider is not null)
        {
            _serviceProvider = serviceProvider;
            IDesignerHost? host = serviceProvider.GetService<IDesignerHost>();
            host?.AddService(typeof(DesignerActionService), this);

            if (serviceProvider.TryGetService(out IComponentChangeService? componentChangeService))
            {
                componentChangeService.ComponentRemoved += OnComponentRemoved;
            }

            _selectionService = serviceProvider.GetService<ISelectionService>();
        }

        _designerActionLists = [];
        _componentToVerbsEventHookedUp = [];
    }

    /// <summary>
    ///  This event is thrown whenever a DesignerActionList is removed or added for any object.
    /// </summary>
    public event DesignerActionListsChangedEventHandler? DesignerActionListsChanged
    {
        add => _designerActionListsChanged += value;
        remove => _designerActionListsChanged -= value;
    }

    /// <summary>
    ///  Adds a new collection of DesignerActions to be monitored with the related comp object.
    /// </summary>
    public void Add(IComponent comp, DesignerActionListCollection designerActionListCollection)
    {
        ArgumentNullException.ThrowIfNull(comp);
        ArgumentNullException.ThrowIfNull(designerActionListCollection);

        if (_designerActionLists.TryGetValue(comp, out DesignerActionListCollection? collection))
        {
            collection.AddRange(designerActionListCollection);
        }
        else
        {
            _designerActionLists.Add(comp, designerActionListCollection);
        }

        // fire event
        OnDesignerActionListsChanged(new DesignerActionListsChangedEventArgs(comp, DesignerActionListsChangedType.ActionListsAdded, GetComponentActions(comp)));
    }

    /// <summary>
    ///  Adds a new DesignerActionList to be monitored with the related comp object
    /// </summary>
    public void Add(IComponent comp, DesignerActionList actionList)
    {
        Add(comp, new DesignerActionListCollection([actionList]));
    }

    /// <summary>
    ///  Clears all objects and DesignerActions from the DesignerActionService.
    /// </summary>
    public void Clear()
    {
        if (_designerActionLists.Count == 0)
        {
            return;
        }

        // Get list of components
        IComponent[] compsRemoved = [.. _designerActionLists.Keys];

        // Actually clear our dictionary.
        _designerActionLists.Clear();

        // Fire our DesignerActionsChanged event for each comp we just removed.
        foreach (IComponent comp in compsRemoved)
        {
            OnDesignerActionListsChanged(new(comp, DesignerActionListsChangedType.ActionListsRemoved, GetComponentActions(comp)));
        }
    }

    /// <summary>
    ///  Returns true if the DesignerActionService is currently managing the comp object.
    /// </summary>
    public bool Contains(IComponent comp)
    {
        ArgumentNullException.ThrowIfNull(comp);
        return _designerActionLists.ContainsKey(comp);
    }

    /// <summary>
    ///  Disposes all resources and unhooks all events.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing && _serviceProvider is not null)
        {
            IDesignerHost? host = _serviceProvider.GetService<IDesignerHost>();
            host?.RemoveService<DesignerActionService>();

            if (_serviceProvider.TryGetService(out IComponentChangeService? componentChangeService))
            {
                componentChangeService.ComponentRemoved -= OnComponentRemoved;
            }
        }
    }

    public DesignerActionListCollection GetComponentActions(IComponent component)
    {
        return GetComponentActions(component, ComponentActionsType.All);
    }

    public virtual DesignerActionListCollection GetComponentActions(IComponent component, ComponentActionsType type)
    {
        ArgumentNullException.ThrowIfNull(component);

        DesignerActionListCollection result = [];
        switch (type)
        {
            case ComponentActionsType.All:
                GetComponentDesignerActions(component, result);
                GetComponentServiceActions(component, result);
                break;
            case ComponentActionsType.Component:
                GetComponentDesignerActions(component, result);
                break;
            case ComponentActionsType.Service:
                GetComponentServiceActions(component, result);
                break;
        }

        return result;
    }

    protected virtual void GetComponentDesignerActions(IComponent component, DesignerActionListCollection actionLists)
    {
        ArgumentNullException.ThrowIfNull(component);
        ArgumentNullException.ThrowIfNull(actionLists);

        IServiceContainer? serviceContainer = component.Site as IServiceContainer;
        if (serviceContainer.TryGetService(out DesignerCommandSet? designerCommandSet))
        {
            DesignerActionListCollection? pullCollection = designerCommandSet.ActionLists;
            if (pullCollection is not null)
            {
                actionLists.AddRange(pullCollection);
            }

            // if we don't find any, add the verbs for this component there...
            if (actionLists.Count == 0)
            {
                DesignerVerbCollection? verbs = designerCommandSet.Verbs;
                if (verbs is not null && verbs.Count != 0)
                {
                    List<DesignerVerb> verbsArray = [];
                    bool hookupEvents = _componentToVerbsEventHookedUp.Add(component);

                    foreach (DesignerVerb verb in verbs)
                    {
                        if (verb is null)
                        {
                            continue;
                        }

                        if (hookupEvents)
                        {
                            verb.CommandChanged += OnVerbStatusChanged;
                        }

                        if (verb is { Enabled: true, Visible: true })
                        {
                            verbsArray.Add(verb);
                        }
                    }

                    if (verbsArray.Count != 0)
                    {
                        actionLists.Add(new DesignerActionVerbList([.. verbsArray]));
                    }
                }
            }

            // remove all the ones that are empty... ie GetSortedActionList returns nothing. we might waste some
            // time doing this twice but don't have much of a choice here... the panel is not yet displayed and
            // we want to know if a non empty panel is present...
            // NOTE: We do this AFTER the verb check that way to disable auto verb upgrading you can just return an
            // empty actionList collection
            if (pullCollection is not null)
            {
                foreach (DesignerActionList actionList in pullCollection)
                {
                    DesignerActionItemCollection? collection = actionList?.GetSortedActionItems();
                    if (collection is null || collection.Count == 0)
                    {
                        actionLists.Remove(actionList);
                    }
                }
            }
        }
    }

    private void OnVerbStatusChanged(object? sender, EventArgs args)
    {
        if (!_reEntrantCode)
        {
            try
            {
                _reEntrantCode = true;
                if (_selectionService?.PrimarySelection is IComponent { Site: IServiceContainer container } comp)
                {
                    DesignerCommandSet commandSet = container.GetRequiredService<DesignerCommandSet>();
                    foreach (DesignerVerb verb in commandSet.Verbs!)
                    {
                        if (verb == sender)
                        {
                            DesignerActionUIService? designerActionUIService = container.GetService<DesignerActionUIService>();
                            designerActionUIService?.Refresh(comp); // we need to refresh, a verb on the current panel has changed its state
                        }
                    }
                }
            }
            finally
            {
                _reEntrantCode = false;
            }
        }
    }

    protected virtual void GetComponentServiceActions(IComponent component, DesignerActionListCollection actionLists)
    {
        ArgumentNullException.ThrowIfNull(component);
        ArgumentNullException.ThrowIfNull(actionLists);

        if (_designerActionLists.TryGetValue(component, out DesignerActionListCollection? pushCollection))
        {
            actionLists.AddRange(pushCollection);
            // remove all the ones that are empty... ie GetSortedActionList returns nothing. we might waste some time
            // doing this twice but don't have much of a choice here... the panel is not yet displayed and we want
            // to know if a non empty panel is present...
            foreach (DesignerActionList actionList in pushCollection)
            {
                DesignerActionItemCollection? collection = actionList?.GetSortedActionItems();
                if (collection is null || collection.Count == 0)
                {
                    actionLists.Remove(actionList);
                }
            }
        }
    }

    /// <summary>
    ///  We hook the OnComponentRemoved event so we can clean up all associated actions.
    /// </summary>
    private void OnComponentRemoved(object? source, ComponentEventArgs componentEventArgs)
    {
        Remove(componentEventArgs.Component!);
    }

    /// <summary>
    ///  This fires our DesignerActionsChanged event.
    /// </summary>
    private void OnDesignerActionListsChanged(DesignerActionListsChangedEventArgs e)
    {
        _designerActionListsChanged?.Invoke(this, e);
    }

    /// <summary>
    ///  This will remove all DesignerActions associated with the 'comp' object.
    ///  All alarms will be unhooked and the DesignerActionsChanged event will be fired.
    /// </summary>
    public void Remove(IComponent comp)
    {
        ArgumentNullException.ThrowIfNull(comp);

        if (_designerActionLists.Remove(comp))
        {
            OnDesignerActionListsChanged(new DesignerActionListsChangedEventArgs(comp, DesignerActionListsChangedType.ActionListsRemoved, GetComponentActions(comp)));
        }
    }

    /// <summary>
    ///  This will remove the specified DesignerAction from the DesignerActionService.
    ///  All alarms will be unhooked and the DesignerActionsChanged event will be fired.
    /// </summary>
    public void Remove(DesignerActionList actionList)
    {
        ArgumentNullException.ThrowIfNull(actionList);

        // find the associated component
        foreach (IComponent comp in _designerActionLists.Keys)
        {
            if (_designerActionLists.TryGetValue(comp, out DesignerActionListCollection? dacl) && dacl.Contains(actionList))
            {
                Remove(comp, actionList);
                break;
            }
        }
    }

    /// <summary>
    ///  This will remove the all instances of the DesignerAction from the 'comp' object.
    ///  If an alarm was set, it will be unhooked. This will also fire the DesignerActionChanged event.
    /// </summary>
    public void Remove(IComponent comp, DesignerActionList actionList)
    {
        ArgumentNullException.ThrowIfNull(comp);
        ArgumentNullException.ThrowIfNull(actionList);

        if (!_designerActionLists.TryGetValue(comp, out DesignerActionListCollection? actionLists) || !actionLists.Contains(actionList))
        {
            return;
        }

        if (actionLists.Count == 1)
        {
            // this is the last action for this object, remove the entire thing
            Remove(comp);
        }
        else
        {
            // remove each instance of this action
            for (int i = actionLists.Count - 1; i >= 0; i--)
            {
                if (actionList.Equals(actionLists[i]))
                {
                    // found one to remove
                    actionLists.RemoveAt(i);
                }
            }

            OnDesignerActionListsChanged(new DesignerActionListsChangedEventArgs(comp, DesignerActionListsChangedType.ActionListsRemoved, GetComponentActions(comp)));
        }
    }

    internal event DesignerActionUIStateChangeEventHandler? DesignerActionUIStateChange
    {
        add
        {
            if (_serviceProvider.TryGetService(out DesignerActionUIService? designerActionUIService))
            {
                designerActionUIService.DesignerActionUIStateChange += value;
            }
        }
        remove
        {
            if (_serviceProvider.TryGetService(out DesignerActionUIService? designerActionUIService))
            {
                designerActionUIService.DesignerActionUIStateChange -= value;
            }
        }
    }
}
