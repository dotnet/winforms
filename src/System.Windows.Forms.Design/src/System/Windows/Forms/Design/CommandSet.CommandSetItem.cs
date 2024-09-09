// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel.Design;

namespace System.Windows.Forms.Design;

internal partial class CommandSet
{
    /// <internalonly/>
    /// <summary>
    ///  We extend MenuCommand for our command set items. A command set item
    ///  is a menu command with an added delegate that is used to determine the
    ///  flags for the menu item. We have different classes of delegates here.
    ///  For example, many menu items may be enabled when there is at least
    ///  one object selected, while others are only enabled if there is more than
    ///  one object or if there is a primary selection.
    /// </summary>
    protected partial class CommandSetItem : MenuCommand
    {
        private readonly EventHandler _statusHandler;
        private readonly IEventHandlerService _eventService;
        private readonly IUIService? _uiService;

        private readonly CommandSet? _commandSet;
        private static readonly Dictionary<EventHandler, StatusState> s_commandStatusHash = [];       // Dictionary of the command statuses we are tracking.
        private bool _updatingCommand; // flag we set when we're updating the command so we don't call back on the status handler.

        public CommandSetItem(CommandSet commandSet,
            EventHandler statusHandler,
            EventHandler invokeHandler,
            CommandID id,
            IUIService? uiService) : this(commandSet,
            statusHandler,
            invokeHandler,
            id,
            optimizeStatus: false,
            uiService)
        {
        }

        public CommandSetItem(CommandSet commandSet,
            EventHandler statusHandler,
            EventHandler invokeHandler,
            CommandID id) : this(commandSet,
            statusHandler,
            invokeHandler,
            id,
            optimizeStatus: false,
            uiService: null)
        {
        }

        public CommandSetItem(CommandSet commandSet,
            EventHandler statusHandler,
            EventHandler invokeHandler,
            CommandID id,
            bool optimizeStatus) : this(commandSet,
            statusHandler,
            invokeHandler,
            id,
            optimizeStatus,
            uiService: null)
        {
        }

        /// <summary>
        ///  Creates a new CommandSetItem.
        /// </summary>

        // Per SBurke...
        public CommandSetItem(CommandSet commandSet,
            EventHandler statusHandler,
            EventHandler invokeHandler,
            CommandID id,
            bool optimizeStatus,
            IUIService? uiService)
            : base(invokeHandler, id)
        {
            _uiService = uiService;
            _eventService = commandSet._eventService;
            _statusHandler = statusHandler;

            // when we optimize, it's because status is fully based on selection.
            // so what we do is only call the status handler once per selection change to prevent
            // doing the same work over and over again. we do this by hashing up the command statuses
            // and then filling in the results we get, so we can easily retrieve them when
            // the selection hasn't changed.
            //
            if (optimizeStatus && statusHandler is not null)
            {
                // we use this as our sentinel of when we're doing this.
                //
                _commandSet = commandSet;

                //
                // UNDONE:CommandSetItem is put in a static dictionary, and CommandSetItem
                // references CommandSet, CommandSet reference FormDesigner. If we don't
                // remove the CommandSetItem from the static dictionary, FormDesigner is
                // leaked. This demonstrates a bad design. We should not keep a static
                // dictionary for all the items, instead, we should keep a dictionary per
                // Designer. When designer is disposed, all command items got disposed
                // automatically. However, at this time, we would pick a simple way with
                // low risks to fix this.
                //
                // if this handler isn't already in there, add it.
                //
                if (!s_commandStatusHash.TryGetValue(statusHandler, out StatusState? state))
                {
                    state = new StatusState();
                    s_commandStatusHash.Add(statusHandler, state);
                }

                state._refCount++;
            }
        }

        /// <summary>
        /// Checks if the status for this command is valid, meaning we don't need to call the status handler.
        /// </summary>
        private bool CommandStatusValid
        {
            get
            {
                // check to see if this is a command we have hashed up and if it's version stamp
                // is the same as our current selection version.
                if (_commandSet is not null && s_commandStatusHash.TryGetValue(_statusHandler, out StatusState? state))
                {
                    if (state.SelectionVersion == _commandSet.SelectionVersion)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        /// <summary>
        /// Applies the cached status to this item.
        /// </summary>
        private void ApplyCachedStatus()
        {
            if (_commandSet is not null && s_commandStatusHash.TryGetValue(_statusHandler, out StatusState? state))
            {
                try
                {
                    // set our our updating flag so it doesn't call the status handler again.
                    //
                    _updatingCommand = true;

                    // and push the state into this command.
                    //
                    state.ApplyState(this);
                }
                finally
                {
                    _updatingCommand = false;
                }
            }
        }

        /// <summary>
        ///  This may be called to invoke the menu item.
        /// </summary>
        public override void Invoke()
        {
            // We allow outside parties to override the availability of particular menu commands.
            //
            try
            {
                if (_eventService is not null)
                {
                    IMenuStatusHandler? msh = (IMenuStatusHandler?)_eventService.GetHandler(typeof(IMenuStatusHandler));
                    if (msh is not null && msh.OverrideInvoke(this))
                    {
                        return;
                    }
                }

                base.Invoke();
            }
            catch (Exception e)
            {
                _uiService?.ShowError(e, string.Format(SR.CommandSetError, e.Message));

                if (e.IsCriticalException())
                {
                    throw;
                }
            }
        }

        ///<summary>
        /// Only pass this down to the base when we're not doing the cached update.
        ///</summary>
        protected override void OnCommandChanged(EventArgs e)
        {
            if (!_updatingCommand)
            {
                base.OnCommandChanged(e);
            }
        }

        ///<summary>
        /// Saves the status for this command to the statusstate that's stored in the dictionary
        /// based on our status handler delegate.
        ///</summary>
        private void SaveCommandStatus()
        {
            if (_commandSet is not null)
            {
                // see if we need to create one of these StatusState dudes.
                //
                if (!s_commandStatusHash.TryGetValue(_statusHandler, out StatusState? state))
                {
                    state = new StatusState();
                }

                // and save the enabled, visible, checked, and supported state.
                //
                state.SaveState(this, _commandSet.SelectionVersion);
            }
        }

        /// <summary>
        ///  Called when the status of this command should be re-queried.
        /// </summary>
        public void UpdateStatus()
        {
            // We allow outside parties to override the availability of particular menu commands.
            //
            if (_eventService is not null)
            {
                IMenuStatusHandler? msh = (IMenuStatusHandler?)_eventService.GetHandler(typeof(IMenuStatusHandler));
                if (msh is not null && msh.OverrideStatus(this))
                {
                    return;
                }
            }

            if (_statusHandler is not null)
            {
                // if we need to update our status,
                // call the status handler. otherwise,
                // get the cached status and push it into this
                // command.
                //
                if (!CommandStatusValid)
                {
                    try
                    {
                        _statusHandler(this, EventArgs.Empty);
                        SaveCommandStatus();
                    }
                    catch
                    {
                    }
                }
                else
                {
                    ApplyCachedStatus();
                }
            }
        }

        /// <summary>
        /// Remove this command item from the static dictionary to avoid leaking this object.
        /// </summary>
        public virtual void Dispose()
        {
            if (s_commandStatusHash.TryGetValue(_statusHandler, out StatusState? state))
            {
                state._refCount--;
                if (state._refCount == 0)
                {
                    s_commandStatusHash.Remove(_statusHandler);
                }
            }
        }
    }
}
