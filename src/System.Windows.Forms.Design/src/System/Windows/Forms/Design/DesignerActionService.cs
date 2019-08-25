// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Diagnostics;
using System.Windows.Forms.Design;

namespace System.ComponentModel.Design
{
    /// <summary>
    ///  The DesignerActionService manages DesignerActions. All DesignerActions are associated with an object. DesignerActions can be added or removed at any  given time. The DesignerActionService controls the expiration of DesignerActions by monitoring three basic events: selection change, component change, and timer expiration. Designer implementing this service will need to monitor the DesignerActionsChanged event on this class. This event will fire every time a change is made to any object's DesignerActions.
    /// </summary>
    public class DesignerActionService : IDisposable
    {
        private readonly Hashtable _designerActionLists; // this is how we store 'em.  Syntax: key = object, value = DesignerActionListCollection
        private DesignerActionListsChangedEventHandler _designerActionListsChanged;
        private readonly IServiceProvider _serviceProvider; // standard service provider
        private readonly ISelectionService _selSvc; // selection service
        private readonly Hashtable _componentToVerbsEventHookedUp; //table component true/false
        // Guard against ReEntrant Code. The Infragistics TabControlDesigner, Sets the Commands Status when the Verbs property is accesssed. This property is used in the OnVerbStatusChanged code here and hence causes recursion leading to Stack Overflow Exception.
        private bool _reEntrantCode = false;

        /// <summary>
        ///  Standard constructor. A Service Provider is necessary for monitoring selection and component changes.
        /// </summary>
        public DesignerActionService(IServiceProvider serviceProvider)
        {
            if (serviceProvider != null)
            {
                _serviceProvider = serviceProvider;
                if (serviceProvider.GetService(typeof(IDesignerHost)) is IDesignerHost host)
                {
                    host.AddService(typeof(DesignerActionService), this);
                }
                if (serviceProvider.GetService(typeof(IComponentChangeService)) is IComponentChangeService cs)
                {
                    cs.ComponentRemoved += new ComponentEventHandler(OnComponentRemoved);
                }
                _selSvc = serviceProvider.GetService(typeof(ISelectionService)) as ISelectionService;
            }

            _designerActionLists = new Hashtable();
            _componentToVerbsEventHookedUp = new Hashtable();
        }

        /// <summary>
        ///  This event is thrown whenever a DesignerActionList is removed or added for any object.
        /// </summary>
        public event DesignerActionListsChangedEventHandler DesignerActionListsChanged
        {
            add => _designerActionListsChanged += value;
            remove => _designerActionListsChanged -= value;
        }

        /// <summary>
        ///  Adds a new collection of DesignerActions to be monitored with the related comp object.
        /// </summary>
        public void Add(IComponent comp, DesignerActionListCollection designerActionListCollection)
        {
            if (comp == null)
            {
                throw new ArgumentNullException(nameof(comp));
            }
            if (designerActionListCollection == null)
            {
                throw new ArgumentNullException(nameof(designerActionListCollection));
            }

            DesignerActionListCollection dhlc = (DesignerActionListCollection)_designerActionLists[comp];
            if (dhlc != null)
            {
                dhlc.AddRange(designerActionListCollection);
            }
            else
            {
                _designerActionLists.Add(comp, designerActionListCollection);
            }

            //fire event
            OnDesignerActionListsChanged(new DesignerActionListsChangedEventArgs(comp, DesignerActionListsChangedType.ActionListsAdded, GetComponentActions(comp)));
        }

        /// <summary>
        ///  Adds a new DesignerActionList to be monitored with the related comp object
        /// </summary>
        public void Add(IComponent comp, DesignerActionList actionList)
        {
            Add(comp, new DesignerActionListCollection(new[] { actionList }));
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

            //this will represent the list of componets we just cleared
            ArrayList compsRemoved = new ArrayList(_designerActionLists.Count);
            foreach (DictionaryEntry entry in _designerActionLists)
            {
                compsRemoved.Add(entry.Key);
            }

            //actually clear our hashtable
            _designerActionLists.Clear();
            //fire our DesignerActionsChanged event for each comp we just removed
            foreach (Component comp in compsRemoved)
            {
                OnDesignerActionListsChanged(new DesignerActionListsChangedEventArgs(comp, DesignerActionListsChangedType.ActionListsRemoved, GetComponentActions(comp)));
            }

        }

        /// <summary>
        ///  Returns true if the DesignerActionService is currently managing the comp object.
        /// </summary>
        public bool Contains(IComponent comp)
        {
            if (comp == null)
            {
                throw new ArgumentNullException(nameof(comp));
            }
            return _designerActionLists.Contains(comp);
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
            if (disposing && _serviceProvider != null)
            {
                if (_serviceProvider.GetService(typeof(IDesignerHost)) is IDesignerHost host)
                {
                    host.RemoveService(typeof(DesignerActionService));
                }

                if (_serviceProvider.GetService(typeof(IComponentChangeService)) is IComponentChangeService cs)
                {
                    cs.ComponentRemoved -= new ComponentEventHandler(OnComponentRemoved);
                }
            }
        }

        public DesignerActionListCollection GetComponentActions(IComponent component)
        {
            return GetComponentActions(component, ComponentActionsType.All);
        }

        public virtual DesignerActionListCollection GetComponentActions(IComponent component, ComponentActionsType type)
        {
            if (component == null)
            {
                throw new ArgumentNullException(nameof(component));
            }
            DesignerActionListCollection result = new DesignerActionListCollection();
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
            if (component == null)
            {
                throw new ArgumentNullException(nameof(component));
            }

            if (actionLists == null)
            {
                throw new ArgumentNullException(nameof(actionLists));
            }

            if (component.Site is IServiceContainer sc)
            {
                if (sc.GetService(typeof(DesignerCommandSet)) is DesignerCommandSet dcs)
                {
                    DesignerActionListCollection pullCollection = dcs.ActionLists;
                    if (pullCollection != null)
                    {
                        actionLists.AddRange(pullCollection);
                    }

                    // if we don't find any, add the verbs for this component there...
                    if (actionLists.Count == 0)
                    {
                        DesignerVerbCollection verbs = dcs.Verbs;
                        if (verbs != null && verbs.Count != 0)
                        {
                            ArrayList verbsArray = new ArrayList();
                            bool hookupEvents = _componentToVerbsEventHookedUp[component] == null;
                            if (hookupEvents)
                            {
                                _componentToVerbsEventHookedUp[component] = true;
                            }
                            foreach (DesignerVerb verb in verbs)
                            {
                                if (verb == null)
                                {
                                    continue;
                                }

                                if (hookupEvents)
                                {
                                    verb.CommandChanged += new EventHandler(OnVerbStatusChanged);
                                }
                                if (verb.Enabled && verb.Visible)
                                {
                                    verbsArray.Add(verb);
                                }
                            }
                            if (verbsArray.Count != 0)
                            {
                                DesignerActionVerbList davl = new DesignerActionVerbList((DesignerVerb[])verbsArray.ToArray(typeof(DesignerVerb)));
                                actionLists.Add(davl);
                            }
                        }
                    }

                    // remove all the ones that are empty... ie GetSortedActionList returns nothing. we might waste some time doing this twice but don't have much of a choice here... the panel is not yet displayed and we want to know if a non empty panel is present...
                    // NOTE: We do this AFTER the verb check that way to disable auto verb upgrading you can just return an empty actionlist collection
                    if (pullCollection != null)
                    {
                        foreach (DesignerActionList actionList in pullCollection)
                        {
                            DesignerActionItemCollection collection = actionList?.GetSortedActionItems();
                            if (collection == null || collection.Count == 0)
                            {
                                actionLists.Remove(actionList);
                            }
                        }
                    }
                }
            }
        }

        private void OnVerbStatusChanged(object sender, EventArgs args)
        {
            if (!_reEntrantCode)
            {
                try
                {
                    _reEntrantCode = true;
                    if (_selSvc?.PrimarySelection is IComponent comp)
                    {
                        if (comp.Site is IServiceContainer sc)
                        {
                            DesignerCommandSet dcs = (DesignerCommandSet)sc.GetService(typeof(DesignerCommandSet));
                            foreach (DesignerVerb verb in dcs.Verbs)
                            {
                                if (verb == sender)
                                {
                                    DesignerActionUIService dapUISvc = (DesignerActionUIService)sc.GetService(typeof(DesignerActionUIService));
                                    if (dapUISvc != null)
                                    {
                                        dapUISvc.Refresh(comp); // we need to refresh, a verb on the current panel has changed its state
                                    }
                                }
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
            if (component == null)
            {
                throw new ArgumentNullException(nameof(component));
            }

            if (actionLists == null)
            {
                throw new ArgumentNullException(nameof(actionLists));
            }

            DesignerActionListCollection pushCollection = (DesignerActionListCollection)_designerActionLists[component];
            if (pushCollection != null)
            {
                actionLists.AddRange(pushCollection);
                // remove all the ones that are empty... ie GetSortedActionList returns nothing. we might waste some time doing this twice but don't have much of a choice here... the panel is not yet displayed and we want to know if a non empty panel is present...
                foreach (DesignerActionList actionList in pushCollection)
                {
                    DesignerActionItemCollection collection = actionList?.GetSortedActionItems();
                    if (collection == null || collection.Count == 0)
                    {
                        actionLists.Remove(actionList);
                    }
                }
            }
        }

        /// <summary>
        ///  We hook the OnComponentRemoved event so we can clean up  all associated actions.
        /// </summary>
        private void OnComponentRemoved(object source, ComponentEventArgs ce)
        {
            Remove(ce.Component);
        }

        /// <summary>
        ///  This fires our DesignerActionsChanged event.
        /// </summary>
        private void OnDesignerActionListsChanged(DesignerActionListsChangedEventArgs e)
        {
            _designerActionListsChanged?.Invoke(this, e);
        }

        /// <summary>
        ///  This will remove all DesignerActions associated with the 'comp' object.  All alarms will be unhooked and the DesignerActionsChagned event will be fired.
        /// </summary>
        public void Remove(IComponent comp)
        {
            if (comp == null)
            {
                throw new ArgumentNullException(nameof(comp));
            }

            if (!_designerActionLists.Contains(comp))
            {
                return;
            }

            _designerActionLists.Remove(comp);
            OnDesignerActionListsChanged(new DesignerActionListsChangedEventArgs(comp, DesignerActionListsChangedType.ActionListsRemoved, GetComponentActions(comp)));
        }

        /// <summary>
        ///  This will remove the specified Designeraction from the DesignerActionService.  All alarms will be unhooked and the DesignerActionsChagned event will be fired.
        /// </summary>
        public void Remove(DesignerActionList actionList)
        {
            if (actionList == null)
            {
                throw new ArgumentNullException(nameof(actionList));
            }

            //find the associated component
            foreach (IComponent comp in _designerActionLists.Keys)
            {
                if (((DesignerActionListCollection)_designerActionLists[comp]).Contains(actionList))
                {
                    Remove(comp, actionList);
                    break;
                }
            }
        }

        /// <summary>
        ///  This will remove the all instances of the DesignerAction from  the 'comp' object. If an alarm was set, it will be unhooked. This will also fire the DesignerActionChanged event.
        /// </summary>
        public void Remove(IComponent comp, DesignerActionList actionList)
        {
            if (comp == null)
            {
                throw new ArgumentNullException(nameof(comp));
            }
            if (actionList == null)
            {
                throw new ArgumentNullException(nameof(actionList));
            }
            if (!_designerActionLists.Contains(comp))
            {
                return;
            }

            DesignerActionListCollection actionLists = (DesignerActionListCollection)_designerActionLists[comp];
            if (!actionLists.Contains(actionList))
            {
                return;
            }

            if (actionLists.Count == 1)
            {
                //this is the last action for this object, remove the entire thing
                Remove(comp);
            }
            else
            {
                //remove each instance of this action
                ArrayList actionListsToRemove = new ArrayList(1);
                foreach (DesignerActionList t in actionLists)
                {
                    if (actionList.Equals(t))
                    {
                        //found one to remove
                        actionListsToRemove.Add(t);
                    }
                }

                foreach (DesignerActionList t in actionListsToRemove)
                {
                    actionLists.Remove(t);
                }
                OnDesignerActionListsChanged(new DesignerActionListsChangedEventArgs(comp, DesignerActionListsChangedType.ActionListsRemoved, GetComponentActions(comp)));
            }
        }

        internal event DesignerActionUIStateChangeEventHandler DesignerActionUIStateChange
        {
            add
            {
                DesignerActionUIService dapUISvc = (DesignerActionUIService)_serviceProvider.GetService(typeof(DesignerActionUIService));
                if (dapUISvc != null)
                {
                    dapUISvc.DesignerActionUIStateChange += value;
                }
            }
            remove
            {
                DesignerActionUIService dapUISvc = (DesignerActionUIService)_serviceProvider.GetService(typeof(DesignerActionUIService));
                if (dapUISvc != null)
                {
                    dapUISvc.DesignerActionUIStateChange -= value;
                }
            }
        }
    }
}
