// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace System.ComponentModel.Design
{
    /// <summary>
    ///  The selection service handles selection within a form.  There is one selection service for each designer host. A selection consists of an array of objects.  One objects is designated the "primary" selection.
    /// </summary>
    internal sealed class SelectionService : ISelectionService, IDisposable
    {
        // These are the selection types we use for context help.
        private static readonly string[] s_selectionKeywords = new string[] { "None", "Single", "Multiple" };

        // State flags for the selection service
        private static readonly int s_stateTransaction = BitVector32.CreateMask(); // Designer is in a transaction
        private static readonly int s_stateTransactionChange = BitVector32.CreateMask(s_stateTransaction); // Component change occurred while in a transaction

        // ISelectionService events
        private static readonly object s_eventSelectionChanging = new object();
        private static readonly object s_eventSelectionChanged = new object();

        // Member variables
        private IServiceProvider _provider; // The service provider
        private BitVector32 _state; // state of the selection service
        private readonly EventHandlerList _events; // the events we raise
        private ArrayList _selection; // list of selected objects
        private string[] _contextAttributes; // help context information we have pushed to the help service.
        private short _contextKeyword; // the offset into the selection keywords for the current selection.
        private StatusCommandUI _statusCommandUI; // UI for setting the StatusBar Information..

        /// <summary>
        ///  Creates a new selection manager object.  The selection manager manages all selection of all designers under the current form file.
        /// </summary>
        internal SelectionService(IServiceProvider provider) : base()
        {
            _provider = provider;
            _state = new BitVector32();
            _events = new EventHandlerList();
            _statusCommandUI = new StatusCommandUI(provider);
        }

        /// <summary>
        ///  Adds the given selection to our selection list.
        /// </summary>
        internal void AddSelection(object sel)
        {
            if (_selection == null)
            {
                _selection = new ArrayList();
                // Now is the opportune time to hook up all of our events
                if (GetService(typeof(IComponentChangeService)) is IComponentChangeService cs)
                {
                    cs.ComponentRemoved += new ComponentEventHandler(OnComponentRemove);
                }

                if (GetService(typeof(IDesignerHost)) is IDesignerHost host)
                {
                    host.TransactionOpened += new EventHandler(OnTransactionOpened);
                    host.TransactionClosed += new DesignerTransactionCloseEventHandler(OnTransactionClosed);
                    if (host.InTransaction)
                    {
                        OnTransactionOpened(host, EventArgs.Empty);
                    }
                }
            }
            if (!_selection.Contains(sel))
            {
                _selection.Add(sel);
            }
        }

        /// <summary>
        ///  Called when our visiblity or batch mode changes.  Flushes any pending notifications or updates if possible.
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
        private object GetService(Type serviceType)
        {
            if (_provider != null)
            {
                return _provider.GetService(serviceType);
            }
            return null;
        }

        /// <summary>
        ///  called by the formcore when someone has removed a component.  This will remove any selection on the component without disturbing the rest of the selection
        /// </summary>
        private void OnComponentRemove(object sender, ComponentEventArgs ce)
        {
            if (_selection != null && _selection.Contains(ce.Component))
            {
                RemoveSelection(ce.Component);
                OnSelectionChanged();
            }
        }

        /// <summary>
        ///  called anytime the selection has changed.  We update our UI for the selection, and then we fire a juicy change event.
        /// </summary>
        private void OnSelectionChanged()
        {
            if (_state[s_stateTransaction])
            {
                _state[s_stateTransactionChange] = true;
            }
            else
            {
                EventHandler eh = _events[s_eventSelectionChanging] as EventHandler;
                eh?.Invoke(this, EventArgs.Empty);

                UpdateHelpKeyword(true);

                eh = _events[s_eventSelectionChanged] as EventHandler;
                if (eh != null)
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
        ///  Called by the designer host when it is entering or leaving a batch operation.  Here we queue up selection notification and we turn off our UI.
        /// </summary>
        private void OnTransactionClosed(object sender, DesignerTransactionCloseEventArgs e)
        {
            if (e.LastTransaction)
            {
                _state[s_stateTransaction] = false;
                FlushSelectionChanges();
            }
        }

        /// <summary>
        ///  Called by the designer host when it is entering or leaving a batch operation.  Here we queue up selection notification and we turn off our UI.
        /// </summary>
        private void OnTransactionOpened(object sender, EventArgs e)
        {
            _state[s_stateTransaction] = true;
        }

        internal object PrimarySelection
        {
            get => (_selection != null && _selection.Count > 0) ? _selection[0] : null;
        }

        /// <summary>
        ///  Removes the given selection from the selection list.
        /// </summary>
        internal void RemoveSelection(object sel)
        {
            if (_selection != null)
            {
                _selection.Remove(sel);
            }
        }

        private void ApplicationIdle(object source, EventArgs args)
        {
            UpdateHelpKeyword(false);
            Windows.Forms.Application.Idle -= new EventHandler(ApplicationIdle);
        }

        /// <summary>
        ///  Pushes the help context into the help service for the current set of selected objects.
        /// </summary>
        private void UpdateHelpKeyword(bool tryLater)
        {
            if (!(GetService(typeof(IHelpService)) is IHelpService helpService))
            {
                if (tryLater)
                {
                    // we don't have an help service YET, we need to wait for it...
                    // hook up to the application.idle event
                    // yes this is UGLY but we don't have a choice, vs is always returning a UserContext, so even if we manage to instanciate the HelpService beforehand and class pushcontext on it (trying to stack up help context in the helpservice to be flushed when we get the documentactivation event we just don't know if that's going to work or not... so we just wait...) :(((
                    Windows.Forms.Application.Idle += new EventHandler(ApplicationIdle);
                }
                return;
            }

            // If there is an old set of context attributes, remove them.
            if (_contextAttributes != null)
            {
                foreach (string s in _contextAttributes)
                {
                    helpService.RemoveContextAttribute("Keyword", s);
                }
                _contextAttributes = null;
            }

            // Clear the selection keyword
            helpService.RemoveContextAttribute("Selection", s_selectionKeywords[_contextKeyword]);
            // Get a list of unique class names.
            Debug.Assert(_selection != null, "Should be impossible to update the help context before configuring the selection hash");
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
            _contextAttributes = new string[_selection.Count];

            for (int i = 0; i < _selection.Count; i++)
            {
                object s = _selection[i];
                string helpContext = TypeDescriptor.GetClassName(s);
                HelpKeywordAttribute contextAttr = (HelpKeywordAttribute)TypeDescriptor.GetAttributes(s)[typeof(HelpKeywordAttribute)];
                if (contextAttr != null && !contextAttr.IsDefaultAttribute())
                {
                    helpContext = contextAttr.HelpKeyword;
                }
                _contextAttributes[i] = helpContext;
            }

            // And push them into the help context as keywords.
            HelpKeywordType selectionType = baseComponentSelected ? HelpKeywordType.GeneralKeyword : HelpKeywordType.F1Keyword;
            foreach (string helpContext in _contextAttributes)
            {
                helpService.AddContextAttribute("Keyword", helpContext, selectionType);
            }

            // Now add the appropriate selection keyword.  Note that we do not count the base component as being selected if it is the only thing selected.
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
            if (_selection != null)
            {
                if (GetService(typeof(IDesignerHost)) is IDesignerHost host)
                {
                    host.TransactionOpened -= new EventHandler(OnTransactionOpened);
                    host.TransactionClosed -= new DesignerTransactionCloseEventHandler(OnTransactionClosed);
                    if (host.InTransaction)
                    {
                        OnTransactionClosed(host, new DesignerTransactionCloseEventArgs(true, true));
                    }
                }

                if (GetService(typeof(IComponentChangeService)) is IComponentChangeService cs)
                {
                    cs.ComponentRemoved -= new ComponentEventHandler(OnComponentRemove);
                }
                _selection.Clear();
            }
            _statusCommandUI = null;
            _provider = null;
        }

        /// <summary>
        ///  Retrieves the object that is currently the primary selection.  The primary selection has a slightly different UI look and is used as a "key" when an operation is to be done on multiple components.
        /// </summary>
        object ISelectionService.PrimarySelection
        {
            get
            {
                if (_selection != null && _selection.Count > 0)
                {
                    return _selection[0];
                }

                return null;
            }
        }

        /// <summary>
        ///  Retrieves the count of selected objects.
        /// </summary>
        int ISelectionService.SelectionCount
        {
            get
            {
                if (_selection != null)
                {
                    return _selection.Count;
                }
                return 0;
            }
        }

        /// <summary>
        ///  Adds a <see cref='System.ComponentModel.Design.ISelectionService.SelectionChanged'/> event handler to the selection service.
        /// </summary>
        event EventHandler ISelectionService.SelectionChanged
        {
            add => _events.AddHandler(s_eventSelectionChanged, value);
            remove => _events.RemoveHandler(s_eventSelectionChanged, value);
        }

        /// <summary>
        ///  Occurs whenever the user changes the current list of selected components in the designer.  This event is raised before the actual selection changes.
        /// </summary>
        event EventHandler ISelectionService.SelectionChanging
        {
            add => _events.AddHandler(s_eventSelectionChanging, value);
            remove => _events.RemoveHandler(s_eventSelectionChanging, value);
        }

        /// <summary>
        ///  Determines if the component is currently selected.  This is faster than getting the entire list of selelected components.
        /// </summary>
        bool ISelectionService.GetComponentSelected(object component)
        {
            if (component == null)
            {
                throw new ArgumentNullException(nameof(component));
            }
            return (_selection != null && _selection.Contains(component));
        }

        /// <summary>
        ///  Retrieves an array of components that are currently part of the user's selection.
        /// </summary>
        ICollection ISelectionService.GetSelectedComponents()
        {
            if (_selection != null)
            {
                // Must clone here.  Otherwise the values collection is a live collection and will change when the  selection changes.  GetSelectedComponents should be a snapshot.
                object[] selectedValues = new object[_selection.Count];
                _selection.CopyTo(selectedValues, 0);
                return selectedValues;
            }
            return Array.Empty<object>();
        }

        /// <summary>
        ///  Changes the user's current set of selected components to the components in the given array.  If the array is null or doesn't contain any components, this will select the top level component in the designer.
        /// </summary>
        void ISelectionService.SetSelectedComponents(ICollection components)
        {
            ((ISelectionService)this).SetSelectedComponents(components, SelectionTypes.Auto);
        }

        /// <summary>
        ///  Changes the user's current set of selected components to the components in the given array.  If the array is null or doesn't contain any components, this will select the top level component in the designer.
        /// </summary>
        void ISelectionService.SetSelectedComponents(ICollection components, SelectionTypes selectionType)
        {
            bool fToggle = (selectionType & SelectionTypes.Toggle) == SelectionTypes.Toggle;
            bool fPrimary = (selectionType & SelectionTypes.Primary) == SelectionTypes.Primary;
            bool fAdd = (selectionType & SelectionTypes.Add) == SelectionTypes.Add;
            bool fRemove = (selectionType & SelectionTypes.Remove) == SelectionTypes.Remove;
            bool fReplace = (selectionType & SelectionTypes.Replace) == SelectionTypes.Replace;
            bool fAuto = !(fToggle | fAdd | fRemove | fReplace);

            // We always want to allow NULL arrays coming in.
            if (components == null)
            {
                components = Array.Empty<object>();
            }
            // If toggle, replace, remove or add are not specifically specified, infer them from  the state of the modifer keys.  This creates the "Auto" selection type for us by default.
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
            object requestedPrimary = null;
            int primaryIndex;

            if (fPrimary && 1 == components.Count)
            {
                foreach (object o in components)
                {
                    requestedPrimary = o;
                    if (o == null)
                    {
                        throw new ArgumentNullException(nameof(components));
                    }
                    break;
                }
            }

            if (requestedPrimary != null && _selection != null && (primaryIndex = _selection.IndexOf(requestedPrimary)) != -1)
            {
                if (primaryIndex != 0)
                {
                    object tmp = _selection[0];
                    _selection[0] = _selection[primaryIndex];
                    _selection[primaryIndex] = tmp;
                    fChanged = true;
                }
            }
            else
            {
                // If we are replacing the selection, only remove the ones that are not in our new list. We also handle the special case here of having a singular component selected that's already selected.  In this case we just move it to the primary selection.
                if (!fToggle && !fAdd && !fRemove)
                {
                    if (_selection != null)
                    {
                        object[] selections = new object[_selection.Count];
                        _selection.CopyTo(selections, 0);
                        // Yucky and N^2, but even with several hundred components this should be fairly fast
                        foreach (object item in selections)
                        {
                            bool remove = true;
                            foreach (object comp in components)
                            {
                                if (comp == null)
                                {
                                    throw new ArgumentNullException(nameof(components));
                                }

                                if (object.ReferenceEquals(comp, item))
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
                foreach (object comp in components)
                {
                    if (comp == null)
                    {
                        throw new ArgumentNullException(nameof(components));
                    }

                    if (_selection != null && _selection.Contains(comp))
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
                //Set the SelectionInformation
                if (_selection.Count > 0)
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
}
