// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.Collections.Specialized;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace System.ComponentModel.Design;

/// <summary>
///  The selection service handles selection within a form.
///  There is one selection service for each <see cref="DesignerHost"/>.
///  A selection consists of an list of <see cref="IComponent"/>.
///  The first component in the list is designated the <see cref="PrimarySelection"/>.
/// </summary>
internal sealed class SelectionService : ISelectionService, IDisposable
{
    // These are the selection types we use for context help.
    private static readonly string[] s_selectionKeywords = ["None", "Single", "Multiple"];

    // State flags for the selection service
    private static readonly int s_stateTransaction = BitVector32.CreateMask(); // Designer is in a transaction
    private static readonly int s_stateTransactionChange = BitVector32.CreateMask(s_stateTransaction); // Component change occurred while in a transaction

    // ISelectionService events
    private static readonly object s_eventSelectionChanging = new();
    private static readonly object s_eventSelectionChanged = new();

    // Member variables
    private IServiceProvider _provider; // The service provider
    private BitVector32 _state; // state of the selection service
    private readonly EventHandlerList _events; // the events we raise
    private List<IComponent>? _selection; // list of selected objects
    private List<string>? _contextAttributes; // help context information we have pushed to the help service.
    private short _contextKeyword; // the offset into the selection keywords for the current selection.
    private StatusCommandUI _statusCommandUI; // UI for setting the StatusBar Information..

    /// <summary>
    ///  Creates a new selection manager object.
    ///  The selection manager manages all selection of all designers under the current form file.
    /// </summary>
    internal SelectionService(IServiceProvider provider) : base()
    {
        _provider = provider;
        _state = default;
        _events = new EventHandlerList();
        _statusCommandUI = new StatusCommandUI(provider);
    }

    /// <summary>
    ///  Adds the given selection to our selection list.
    /// </summary>
    internal void AddSelection(IComponent selection)
    {
        ArgumentNullException.ThrowIfNull(selection, nameof(selection));

        if (_selection is null)
        {
            _selection = [];

            // Now is the opportune time to hook up all of our events
            if (GetService(typeof(IComponentChangeService)) is IComponentChangeService cs)
            {
                cs.ComponentRemoved += OnComponentRemove;
            }

            if (GetService(typeof(IDesignerHost)) is IDesignerHost host)
            {
                host.TransactionOpened += OnTransactionOpened;
                host.TransactionClosed += OnTransactionClosed;
                if (host.InTransaction)
                {
                    OnTransactionOpened(host, EventArgs.Empty);
                }
            }
        }

        if (!_selection.Contains(selection))
        {
            _selection.Add(selection);
        }
    }

    /// <summary>
    ///  Called when our visibility or batch mode changes.
    ///  Flushes any pending notifications or updates if possible.
    /// </summary>
    private void FlushSelectionChanges()
    {
        if (!_state[s_stateTransaction] && _state[s_stateTransactionChange])
        {
            _state[s_stateTransactionChange] = false;
            OnSelectionChanged();
        }
    }

    /// <summary>
    ///  Helper function to retrieve services.
    /// </summary>
    private object? GetService(Type serviceType) => _provider?.GetService(serviceType);

    /// <summary>
    ///  called by the formcore when someone has removed a component.
    ///  This will remove any selection on the component without disturbing the rest of the selection
    /// </summary>
    private void OnComponentRemove(object? sender, ComponentEventArgs ce)
    {
        if (_selection is not null && ce.Component is not null && _selection.Contains(ce.Component))
        {
            RemoveSelection(ce.Component);
            OnSelectionChanged();
        }
    }

    /// <summary>
    ///  called anytime the selection has changed.
    ///  We update our UI for the selection, and then we fire a juicy change event.
    /// </summary>
    private void OnSelectionChanged()
    {
        if (_state[s_stateTransaction])
        {
            _state[s_stateTransactionChange] = true;
        }
        else
        {
            EventHandler? eh = _events[s_eventSelectionChanging] as EventHandler;
            eh?.Invoke(this, EventArgs.Empty);

            UpdateHelpKeyword(true);

            eh = _events[s_eventSelectionChanged] as EventHandler;
            if (eh is not null)
            {
                try
                {
                    eh(this, EventArgs.Empty);
                }
                catch
                {
                    // eat exceptions - required for compatibility with Everett.
                }
            }
        }
    }

    /// <summary>
    ///  Called by the designer host when it is entering or leaving a batch operation.
    ///  Here we queue up selection notification and we turn off our UI.
    /// </summary>
    private void OnTransactionClosed(object? sender, DesignerTransactionCloseEventArgs e)
    {
        if (e.LastTransaction)
        {
            _state[s_stateTransaction] = false;
            FlushSelectionChanges();
        }
    }

    /// <summary>
    ///  Called by the designer host when it is entering or leaving a batch operation.
    ///  Here we queue up selection notification and we turn off our UI.
    /// </summary>
    private void OnTransactionOpened(object? sender, EventArgs e) => _state[s_stateTransaction] = true;

    internal IComponent? PrimarySelection => _selection?.Count > 0 ? _selection[0] : null;

    /// <summary>
    ///  Removes the given selection from the selection list.
    /// </summary>
    internal void RemoveSelection(IComponent selection) => _selection?.Remove(selection);

    private void ApplicationIdle(object? source, EventArgs args)
    {
        UpdateHelpKeyword(false);
        Application.Idle -= ApplicationIdle;
    }

    /// <summary>
    ///  Pushes the help context into the help service for the current set of selected objects.
    /// </summary>
    private void UpdateHelpKeyword(bool tryLater)
    {
        if (GetService(typeof(IHelpService)) is not IHelpService helpService)
        {
            if (tryLater)
            {
                // We don't have a HelpService yet, hook up to the ApplicationIdle event.
                // VS is always returning a UserContext, so instantiating the HelpService
                // beforehand and doing class PushContext on it to try to
                // stack up help context in the HelpService to be flushed when we get the
                // documentation event may not work, so we need to wait for a HelpService instead.
                Application.Idle += ApplicationIdle;
            }

            return;
        }

        // If there is an old set of context attributes, remove them.
        if (_contextAttributes is not null)
        {
            foreach (string helpContext in _contextAttributes)
            {
                helpService.RemoveContextAttribute("Keyword", helpContext);
            }

            _contextAttributes = null;
        }

        // Clear the selection keyword
        helpService.RemoveContextAttribute("Selection", s_selectionKeywords[_contextKeyword]);
        // Get a list of unique class names.
        Debug.Assert(_selection is not null, "Should be impossible to update the help context before configuring the selection hash");
        bool baseComponentSelected = false;
        if (_selection.Count == 0)
        {
            baseComponentSelected = true;
        }
        else if (_selection.Count == 1)
        {
            if (GetService(typeof(IDesignerHost)) is IDesignerHost host && _selection.Contains(host.RootComponent))
            {
                baseComponentSelected = true;
            }
        }

        _contextAttributes = [];

        for (int i = 0; i < _selection.Count; i++)
        {
            object component = _selection[i];
            string? helpContext = TypeDescriptor.GetClassName(component);
            HelpKeywordAttribute? contextAttribute = (HelpKeywordAttribute?)TypeDescriptor.GetAttributes(component)[typeof(HelpKeywordAttribute)];
            if (contextAttribute is not null && !contextAttribute.IsDefaultAttribute())
            {
                helpContext = contextAttribute.HelpKeyword;
            }

            if (helpContext is not null)
            {
                _contextAttributes.Add(helpContext);
            }
        }

        // And push them into the help context as keywords.
        HelpKeywordType selectionType = baseComponentSelected ? HelpKeywordType.GeneralKeyword : HelpKeywordType.F1Keyword;
        foreach (string helpContext in _contextAttributes)
        {
            helpService.AddContextAttribute("Keyword", helpContext, selectionType);
        }

        // Now add the appropriate selection keyword.
        // Note that we do not count the base component as being selected if it is the only thing selected.
        int count = _selection.Count;
        if (count == 1 && baseComponentSelected)
        {
            count--;
        }

        _contextKeyword = (short)Math.Min(count, s_selectionKeywords.Length - 1);
        helpService.AddContextAttribute("Selection", s_selectionKeywords[_contextKeyword], HelpKeywordType.FilterKeyword);
    }

    /// <summary>
    ///  Disposes the entire selection service.
    /// </summary>
    void IDisposable.Dispose()
    {
        if (_selection is not null)
        {
            if (GetService(typeof(IDesignerHost)) is IDesignerHost host)
            {
                host.TransactionOpened -= OnTransactionOpened;
                host.TransactionClosed -= OnTransactionClosed;
                if (host.InTransaction)
                {
                    OnTransactionClosed(host, new(commit: true, lastTransaction: true));
                }
            }

            if (GetService(typeof(IComponentChangeService)) is IComponentChangeService cs)
            {
                cs.ComponentRemoved -= OnComponentRemove;
            }

            _selection.Clear();
        }

        _statusCommandUI = null!;
        _provider = null!;
    }

    /// <summary>
    ///  Retrieves the object that is currently the primary selection.
    ///  The primary selection has a slightly different UI look and is used as
    ///  a "key" when an operation is to be done on multiple components.
    /// </summary>
    object? ISelectionService.PrimarySelection => PrimarySelection;

    /// <summary>
    ///  Retrieves the count of selected objects.
    /// </summary>
    int ISelectionService.SelectionCount => _selection?.Count ?? 0;

    /// <summary>
    ///  Adds a <see cref="ISelectionService.SelectionChanged"/> event handler to the selection service.
    /// </summary>
    event EventHandler ISelectionService.SelectionChanged
    {
        add => _events.AddHandler(s_eventSelectionChanged, value);
        remove => _events.RemoveHandler(s_eventSelectionChanged, value);
    }

    /// <summary>
    ///  Occurs whenever the user changes the current list of selected components in the designer.
    ///  This event is raised before the actual selection changes.
    /// </summary>
    event EventHandler ISelectionService.SelectionChanging
    {
        add => _events.AddHandler(s_eventSelectionChanging, value);
        remove => _events.RemoveHandler(s_eventSelectionChanging, value);
    }

    /// <summary>
    ///  Determines if the component is currently selected.
    ///  This is faster than getting the entire list of selected components.
    /// </summary>
    bool ISelectionService.GetComponentSelected(object component)
    {
        ArgumentNullException.ThrowIfNull(component, nameof(component));
        return _selection is not null && _selection.Contains(component);
    }

    /// <summary>
    ///  Retrieves an array of components that are currently part of the user's selection.
    /// </summary>
    ICollection ISelectionService.GetSelectedComponents()
    {
        // Must clone here. Otherwise the values collection is a live collection and will change when the
        // selection changes. GetSelectedComponents should be a snapshot.
        return _selection?.ToArray() ?? [];
    }

    /// <summary>
    ///  Changes the user's current set of selected components to the components in the given array.
    ///  If the array is null or doesn't contain any components, this will select the top level component in the designer.
    /// </summary>
    void ISelectionService.SetSelectedComponents(ICollection? components)
        => ((ISelectionService)this).SetSelectedComponents(components, SelectionTypes.Auto);

    /// <summary>
    ///  Changes the user's current set of selected components to the components in the given array.
    ///  If the array is null or doesn't contain any components, this will select the top level component in the designer.
    /// </summary>
    void ISelectionService.SetSelectedComponents(ICollection? components, SelectionTypes selectionType)
    {
        bool fToggle = (selectionType & SelectionTypes.Toggle) == SelectionTypes.Toggle;
        bool fPrimary = (selectionType & SelectionTypes.Primary) == SelectionTypes.Primary;
        bool fAdd = (selectionType & SelectionTypes.Add) == SelectionTypes.Add;
        bool fRemove = (selectionType & SelectionTypes.Remove) == SelectionTypes.Remove;
        bool fReplace = (selectionType & SelectionTypes.Replace) == SelectionTypes.Replace;
        bool fAuto = !(fToggle | fAdd | fRemove | fReplace);

        // We always want to allow NULL arrays coming in.
        components ??= Array.Empty<IComponent>();

        // If toggle, replace, remove or add are not specifically specified, infer them from the state of the modifier keys.
        // This creates the "Auto" selection type for us by default.
        if (fAuto)
        {
            fToggle = (Control.ModifierKeys & (Keys.Control | Keys.Shift)) > 0;
            fAdd |= Control.ModifierKeys == Keys.Shift;
            // If we are in auto mode, and if we are toggling or adding new controls, then cancel out the primary flag.
            if (fToggle || fAdd)
            {
                fPrimary = false;
            }
        }

        // This flag is true if we changed selection and should therefore raise a selection change event.
        bool fChanged = false;
        // Handle the click case
        IComponent? requestedPrimary = null;
        int primaryIndex;

        if (fPrimary && components.Count == 1)
        {
            foreach (IComponent component in components)
            {
                requestedPrimary = component;
                ArgumentNullException.ThrowIfNull(component, nameof(components));

                break;
            }
        }

        if (requestedPrimary is not null && _selection is not null && (primaryIndex = _selection.IndexOf(requestedPrimary)) != -1)
        {
            if (primaryIndex != 0)
            {
                (_selection[primaryIndex], _selection[0]) = (_selection[0], _selection[primaryIndex]);
                fChanged = true;
            }
        }
        else
        {
            // If we are replacing the selection, only remove the ones that are not in our new list.
            // We also handle the special case here of having a singular component selected that's already selected.
            // In this case we just move it to the primary selection.
            if (!fToggle && !fAdd && !fRemove)
            {
                if (_selection is not null)
                {
                    IComponent[] selections = new IComponent[_selection.Count];
                    _selection.CopyTo(selections, 0);
                    // Yucky and N^2, but even with several hundred components this should be fairly fast
                    foreach (IComponent item in selections)
                    {
                        bool remove = true;
                        foreach (IComponent comp in components)
                        {
                            ArgumentNullException.ThrowIfNull(comp, nameof(components));

                            if (ReferenceEquals(comp, item))
                            {
                                remove = false;
                                break;
                            }
                        }

                        if (remove)
                        {
                            RemoveSelection(item);
                            fChanged = true;
                        }
                    }
                }
            }

            // Now select / toggle the components.
            foreach (IComponent comp in components)
            {
                ArgumentNullException.ThrowIfNull(comp, nameof(components));

                if (_selection is not null && _selection.Contains(comp))
                {
                    if (fToggle || fRemove)
                    {
                        RemoveSelection(comp);
                        fChanged = true;
                    }
                }
                else if (!fRemove)
                {
                    AddSelection(comp);
                    fChanged = true;
                }
            }
        }

        // Notify that our selection has changed
        if (fChanged)
        {
            // Set the SelectionInformation
            if (_selection?.Count > 0)
            {
                _statusCommandUI.SetStatusInformation(_selection[0] as Component);
            }
            else
            {
                _statusCommandUI.SetStatusInformation(Rectangle.Empty);
            }

            OnSelectionChanged();
        }
    }
}
