// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;

namespace System.ComponentModel.Design
{
    /// <summary>
    ///     The menu command service allows designers to add and respond to
    ///     menu and toolbar items.  It is based on two interfaces.  Designers
    ///     request IMenuCommandService to add menu command handlers, while
    ///     the document or tool window forwards IOleCommandTarget requests
    ///     to this object.
    /// </summary>
    public class MenuCommandService : IMenuCommandService, IDisposable
    {
        private IServiceProvider _serviceProvider;
        private Dictionary<Guid, ArrayList> _commandGroups;
        private object _commandGroupsLock;
        private EventHandler _commandChangedHandler;
        private MenuCommandsChangedEventHandler _commandsChangedHandler;
        private ArrayList _globalVerbs;
        private ISelectionService _selectionService;

        internal static TraceSwitch MENUSERVICE = new TraceSwitch("MENUSERVICE", "MenuCommandService: Track menu command routing");

        // This is the set of verbs we offer through the Verbs property.
        // It consists of the global verbs + any verbs that the currently
        // selected designer wants to offer.  This collection changes with the
        // current selection.
        //
        private DesignerVerbCollection _currentVerbs;

        // this is the type that we last picked up verbs from
        // so we know when we need to refresh
        //
        private Type _verbSourceType;

        /// <summary>
        ///     Creates a new menu command service.
        /// </summary>
        public MenuCommandService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _commandGroupsLock = new Object();
            _commandGroups = new Dictionary<Guid, ArrayList>();
            _commandChangedHandler = new EventHandler(this.OnCommandChanged);
            TypeDescriptor.Refreshed += new RefreshEventHandler(this.OnTypeRefreshed);
        }

        /// <summary>
        ///     This event is thrown whenever a MenuCommand is removed
        ///     or added
        /// </summary>
        public event MenuCommandsChangedEventHandler MenuCommandsChanged
        {
            add
            {
                _commandsChangedHandler += value;
            }
            remove
            {
                _commandsChangedHandler -= value;
            }
        }

        /// <summary>
        ///      Retrieves a set of verbs that are global to all objects on the design
        ///      surface.  This set of verbs will be merged with individual component verbs.
        ///      In the case of a name conflict, the component verb will NativeMethods.
        /// </summary>
        public virtual DesignerVerbCollection Verbs
        {
            get
            {
                EnsureVerbs();
                return _currentVerbs;
            }
        }

        /// <summary>
        ///     Adds a menu command to the document.  The menu command must already exist
        ///     on a menu; this merely adds a handler for it.
        /// </summary>
        public virtual void AddCommand(MenuCommand command)
        {
            if (command is null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            // If the command already exists, it is an error to add
            // a duplicate.
            //
            if (((IMenuCommandService)this).FindCommand(command.CommandID) != null)
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, SR.MenuCommandService_DuplicateCommand, command.CommandID.ToString()));
            }

            ArrayList commandsList;
            lock (_commandGroupsLock)
            {
                if (!_commandGroups.TryGetValue(command.CommandID.Guid, out commandsList))
                {
                    commandsList = new ArrayList();
                    commandsList.Add(command);
                    _commandGroups.Add(command.CommandID.Guid, commandsList);
                }
                else
                {
                    commandsList.Add(command);
                }
            }

            command.CommandChanged += _commandChangedHandler;
            Debug.WriteLineIf(MENUSERVICE.TraceVerbose, "Command added: " + command.ToString());

            // raise event
            OnCommandsChanged(new MenuCommandsChangedEventArgs(MenuCommandsChangedType.CommandAdded, command));
        }

        /// <summary>
        ///      Adds a verb to the set of global verbs.  Individual components should
        ///      use the Verbs property of their designer, rather than call this method.
        ///      This method is intended for objects that want to offer a verb that is
        ///      available regardless of what components are selected.
        /// </summary>
        public virtual void AddVerb(DesignerVerb verb)
        {
            if (verb is null)
            {
                throw new ArgumentNullException(nameof(verb));
            }

            if (_globalVerbs is null)
            {
                _globalVerbs = new ArrayList();
            }

            _globalVerbs.Add(verb);
            OnCommandsChanged(new MenuCommandsChangedEventArgs(MenuCommandsChangedType.CommandAdded, verb));
            EnsureVerbs();
            if (!((IMenuCommandService)this).Verbs.Contains(verb))
            {
                ((IMenuCommandService)this).Verbs.Add(verb);
            }
        }

        /// <summary>
        ///     Disposes of this service.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        ///     Disposes of this service.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_selectionService != null)
                {
                    _selectionService.SelectionChanging -= new EventHandler(this.OnSelectionChanging);
                    _selectionService = null;
                }

                if (_serviceProvider != null)
                {
                    _serviceProvider = null;
                    TypeDescriptor.Refreshed -= new RefreshEventHandler(this.OnTypeRefreshed);
                }
                lock (_commandGroupsLock)
                {
                    foreach (KeyValuePair<Guid, ArrayList> group in _commandGroups)
                    {
                        ArrayList commands = group.Value;
                        foreach (MenuCommand command in commands)
                        {
                            command.CommandChanged -= _commandChangedHandler;
                        }
                        commands.Clear();
                    }
                }
            }
        }

        /// <summary>
        ///      Ensures that the verb list has been created.
        /// </summary>
        protected void EnsureVerbs()
        {
            // We apply global verbs only if the base component is the
            // currently selected object.
            //
            bool useGlobalVerbs = false;

            if (_currentVerbs is null && _serviceProvider != null)
            {
                Hashtable buildVerbs = null;
                ArrayList verbsOrder;

                if (_selectionService is null)
                {
                    _selectionService = GetService(typeof(ISelectionService)) as ISelectionService;

                    if (_selectionService != null)
                    {
                        _selectionService.SelectionChanging += new EventHandler(this.OnSelectionChanging);
                    }
                }

                int verbCount = 0;
                DesignerVerbCollection localVerbs = null;
                DesignerVerbCollection designerActionVerbs = new DesignerVerbCollection(); // we instanciate this one here...
                IDesignerHost designerHost = GetService(typeof(IDesignerHost)) as IDesignerHost;

                if (_selectionService != null && designerHost != null && _selectionService.SelectionCount == 1)
                {
                    object selectedComponent = _selectionService.PrimarySelection;
                    if (selectedComponent is IComponent &&
                        !TypeDescriptor.GetAttributes(selectedComponent).Contains(InheritanceAttribute.InheritedReadOnly))
                    {
                        useGlobalVerbs = (selectedComponent == designerHost.RootComponent);

                        // LOCAL VERBS
                        IDesigner designer = designerHost.GetDesigner((IComponent)selectedComponent);
                        if (designer != null)
                        {
                            localVerbs = designer.Verbs;
                            if (localVerbs != null)
                            {
                                verbCount += localVerbs.Count;
                                _verbSourceType = selectedComponent.GetType();
                            }
                            else
                            {
                                _verbSourceType = null;
                            }
                        }

                        // DesignerAction Verbs
                        DesignerActionService daSvc = GetService(typeof(DesignerActionService)) as DesignerActionService;
                        if (daSvc != null)
                        {
                            DesignerActionListCollection actionLists = daSvc.GetComponentActions(selectedComponent as IComponent);
                            if (actionLists != null)
                            {
                                foreach (DesignerActionList list in actionLists)
                                {
                                    DesignerActionItemCollection dai = list.GetSortedActionItems();
                                    if (dai != null)
                                    {
                                        for (int i = 0; i < dai.Count; i++)
                                        {
                                            DesignerActionMethodItem dami = dai[i] as DesignerActionMethodItem;
                                            if (dami != null && dami.IncludeAsDesignerVerb)
                                            {
                                                EventHandler handler = new EventHandler(dami.Invoke);
                                                DesignerVerb verb = new DesignerVerb(dami.DisplayName, handler);
                                                designerActionVerbs.Add(verb);
                                                verbCount++;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                // GLOBAL VERBS
                if (useGlobalVerbs && _globalVerbs is null)
                {
                    useGlobalVerbs = false;
                }

                if (useGlobalVerbs)
                {
                    verbCount += _globalVerbs.Count;
                }

                // merge all
                buildVerbs = new Hashtable(verbCount, StringComparer.OrdinalIgnoreCase);
                verbsOrder = new ArrayList(verbCount);

                // PRIORITY ORDER FROM HIGH TO LOW: LOCAL VERBS - DESIGNERACTION VERBS - GLOBAL VERBS
                if (useGlobalVerbs)
                {
                    for (int i = 0; i < _globalVerbs.Count; i++)
                    {
                        string key = ((DesignerVerb)_globalVerbs[i]).Text;
                        buildVerbs[key] = verbsOrder.Add(_globalVerbs[i]);
                    }
                }
                if (designerActionVerbs.Count > 0)
                {
                    for (int i = 0; i < designerActionVerbs.Count; i++)
                    {
                        string key = designerActionVerbs[i].Text;
                        buildVerbs[key] = verbsOrder.Add(designerActionVerbs[i]);
                    }
                }
                if (localVerbs != null && localVerbs.Count > 0)
                {
                    for (int i = 0; i < localVerbs.Count; i++)
                    {
                        string key = localVerbs[i].Text;
                        buildVerbs[key] = verbsOrder.Add(localVerbs[i]);
                    }
                }

                // look for duplicate, prepare the result table
                DesignerVerb[] result = new DesignerVerb[buildVerbs.Count];
                int j = 0;
                for (int i = 0; i < verbsOrder.Count; i++)
                {
                    DesignerVerb value = (DesignerVerb)verbsOrder[i];
                    string key = value.Text;
                    if ((int)buildVerbs[key] == i)
                    { // there's not been a duplicate for this entry
                        result[j] = value;
                        j++;
                    }
                }

                _currentVerbs = new DesignerVerbCollection(result);
            }
        }

        /// <summary>
        ///     Searches for the given command ID and returns the MenuCommand
        ///     associated with it.
        /// </summary>
        public MenuCommand FindCommand(CommandID commandID)
        {
            return FindCommand(commandID.Guid, commandID.ID);
        }

        /// <summary>
        ///     Locates the requested command. This will throw an appropriate
        ///     ComFailException if the command couldn't be found.
        /// </summary>
        protected MenuCommand FindCommand(Guid guid, int id)
        {
            Debug.WriteLineIf(MENUSERVICE.TraceVerbose, "MCS Searching for command: " + guid.ToString() + " : " + id.ToString(CultureInfo.CurrentCulture));

            // Search in the list of commands only if the command group is known
            ArrayList commands;
            lock (_commandGroupsLock)
            {
                _commandGroups.TryGetValue(guid, out commands);
            }
            if (commands != null)
            {
                Debug.WriteLineIf(MENUSERVICE.TraceVerbose, "\t...MCS Found group");
                foreach (MenuCommand command in commands)
                {
                    if (command.CommandID.ID == id)
                    {
                        Debug.WriteLineIf(MENUSERVICE.TraceVerbose, "\t... MCS Found Command");
                        return command;
                    }
                }
            }

            // Next, search the verb list as well.
            //
            EnsureVerbs();
            if (_currentVerbs != null)
            {
                int currentID = StandardCommands.VerbFirst.ID;
                foreach (DesignerVerb verb in _currentVerbs)
                {
                    CommandID cid = verb.CommandID;

                    if (cid.ID == id)
                    {
                        Debug.WriteLineIf(MENUSERVICE.TraceVerbose, "\t...MCS Found verb");

                        if (cid.Guid.Equals(guid))
                        {
                            Debug.WriteLineIf(MENUSERVICE.TraceVerbose, "\t...MCS Found group");
                            return verb;
                        }
                    }

                    // We assign virtual sequential IDs to verbs we get from the component. This allows users
                    // to not worry about assigning these IDs themselves.
                    //
                    if (currentID == id)
                    {
                        Debug.WriteLineIf(MENUSERVICE.TraceVerbose, "\t...MCS Found verb");

                        if (cid.Guid.Equals(guid))
                        {
                            Debug.WriteLineIf(MENUSERVICE.TraceVerbose, "\t...MCS Found group");
                            return verb;
                        }
                    }

                    if (cid.Equals(StandardCommands.VerbFirst))
                        currentID++;
                }
            }

            return null;
        }

        /// <summary>
        ///     Get the command list for a given GUID
        /// </summary>
        protected ICollection GetCommandList(Guid guid)
        {
            ArrayList commands = null;
            lock (_commandGroupsLock)
            {
                _commandGroups.TryGetValue(guid, out commands);
            }
            return commands;
        }

        /// <summary>
        /// </summary>
        protected object GetService(Type serviceType)
        {
            if (serviceType is null)
            {
                throw new ArgumentNullException(nameof(serviceType));
            }
            if (_serviceProvider != null)
            {
                return _serviceProvider.GetService(serviceType);
            }
            return null;
        }

        /// <summary>
        ///     Invokes a command on the local form or in the global environment.
        ///     The local form is first searched for the given command ID.  If it is
        ///     found, it is invoked.  Otherwise the the command ID is passed to the
        ///     global environment command handler, if one is available.
        /// </summary>
        public virtual bool GlobalInvoke(CommandID commandID)
        {
            // try to find it locally
            MenuCommand cmd = ((IMenuCommandService)this).FindCommand(commandID);
            if (cmd != null)
            {
                cmd.Invoke();
                return true;
            }
            return false;
        }

        /// <summary>
        ///     Invokes a command on the local form or in the global environment.
        ///     The local form is first searched for the given command ID.  If it is
        ///     found, it is invoked.  Otherwise the the command ID is passed to the
        ///     global environment command handler, if one is available.
        /// </summary>
        public virtual bool GlobalInvoke(CommandID commandId, object arg)
        {
            // try to find it locally
            MenuCommand cmd = ((IMenuCommandService)this).FindCommand(commandId);
            if (cmd != null)
            {
                cmd.Invoke(arg);
                return true;
            }
            return false;
        }

        /// <summary>
        ///     This is called by a menu command when it's status has changed.
        /// </summary>
        private void OnCommandChanged(object sender, EventArgs e)
        {
            Debug.WriteLineIf(MENUSERVICE.TraceVerbose, "Command dirty: " + ((sender != null) ? sender.ToString() : "(null sender)"));
            OnCommandsChanged(new MenuCommandsChangedEventArgs(MenuCommandsChangedType.CommandChanged, (MenuCommand)sender));
        }

        /// <summary>
        ///
        /// </summary>
        protected virtual void OnCommandsChanged(MenuCommandsChangedEventArgs e)
        {
            if (_commandsChangedHandler != null)
            {
                _commandsChangedHandler(this, e);
            }
        }

        /// <summary>
        ///     Called by TypeDescriptor when a type changes.  If this type is currently holding
        ///     our verb, invalidate the list.
        /// </summary>
        private void OnTypeRefreshed(RefreshEventArgs e)
        {
            if (_verbSourceType != null && _verbSourceType.IsAssignableFrom(e.TypeChanged))
            {
                _currentVerbs = null;
            }
        }

        /// <summary>
        ///      This is called by the selection service when the selection has changed.  Here
        ///      we invalidate our verb list.
        /// </summary>
        private void OnSelectionChanging(object sender, EventArgs e)
        {
            if (_currentVerbs != null)
            {
                _currentVerbs = null;
                OnCommandsChanged(new MenuCommandsChangedEventArgs(MenuCommandsChangedType.CommandChanged, null));
            }
        }

        /// <summary>
        ///     Removes the given menu command from the document.
        /// </summary>
        public virtual void RemoveCommand(MenuCommand command)
        {
            if (command is null)
            {
                throw new ArgumentNullException(nameof(command));
            }
            ArrayList commands;
            lock (_commandGroupsLock)
            {
                if (_commandGroups.TryGetValue(command.CommandID.Guid, out commands))
                {
                    int index = commands.IndexOf(command);
                    if (-1 != index)
                    {
                        commands.RemoveAt(index);
                        // If there are no more commands in this command group, remove the group
                        if (commands.Count == 0)
                        {
                            _commandGroups.Remove(command.CommandID.Guid);
                        }

                        command.CommandChanged -= _commandChangedHandler;

                        Debug.WriteLineIf(MENUSERVICE.TraceVerbose, "Command removed: " + command.ToString());

                        // raise event
                        OnCommandsChanged(new MenuCommandsChangedEventArgs(MenuCommandsChangedType.CommandRemoved, command));
                    }
                    return;
                }
            }
            Debug.WriteLineIf(MENUSERVICE.TraceVerbose, "Unable to remove command: " + command.ToString());
        }

        /// <summary>
        ///     Removes the given verb from the document.
        /// </summary>
        public virtual void RemoveVerb(DesignerVerb verb)
        {
            if (verb is null)
            {
                throw new ArgumentNullException(nameof(verb));
            }

            if (_globalVerbs != null)
            {
                int index = _globalVerbs.IndexOf(verb);
                if (index != -1)
                {
                    _globalVerbs.RemoveAt(index);
                    EnsureVerbs();
                    if (((IMenuCommandService)this).Verbs.Contains(verb))
                    {
                        ((IMenuCommandService)this).Verbs.Remove(verb);
                    }
                    OnCommandsChanged(new MenuCommandsChangedEventArgs(MenuCommandsChangedType.CommandRemoved, verb));
                }
            }
        }

        /// <summary>
        ///     Shows the context menu with the given command ID at the given
        ///     location.
        /// </summary>
        public virtual void ShowContextMenu(CommandID menuID, int x, int y)
        {
        }
    }
}


