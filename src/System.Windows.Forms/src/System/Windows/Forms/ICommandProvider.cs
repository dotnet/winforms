// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Windows.Input;

namespace System.Windows.Forms
{
    internal interface ICommandPropertyProvider
    {
        /// <summary>
        /// Occurs, when the Command property has changed.
        /// </summary>
        event EventHandler? CommandChanged;

        /// <summary>
        /// Occurs, when the execution context of the command was changed.
        /// </summary>
        event EventHandler? CommandCanExecuteChanged;

        /// <summary>
        /// Gets or sets the Command to invoke, when the implementing 
        /// Component or Control is triggered by the user.
        /// </summary>
        ICommand? Command { get; set; }

        /// <summary>
        /// Gets or sets the CommandParameter for the Component or Control.
        /// </summary>
        object? CommandParameter { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the control 
        /// can respond to user interaction.
        /// </summary>
        bool Enabled { get; set; }

        /// <summary>
        /// An implementation should raise the <see cref="CommandChanged"/> event 
        /// by calling component's or control's OnRaiseCommandChanged method.
        /// </summary>
        /// <param name="e">An empty <see cref="EventArgs"/> instance.</param>
        void RaiseCommandChanged(EventArgs e);

        /// <summary>
        /// An implementation should raise the <see cref="CommandCanExecuteChanged"/> event 
        /// by calling component's or control's OnCommandCanExecuteChanged method.
        /// </summary>
        /// <param name="e">An empty <see cref="EventArgs"/> instance.</param>
        void RaiseCommandCanExecuteChanged(EventArgs e);
    }

    internal class CommandProviderManager
    {
        private ICommandPropertyProvider _commandPropertyProvider;
        private bool? _previousEnabledStatus;

        public CommandProviderManager(ICommandPropertyProvider commandPropertyProvider)
        {
            _commandPropertyProvider = commandPropertyProvider;
            _previousEnabledStatus = commandPropertyProvider.Enabled;
        }

        public void CommandSetter(
            ICommand? newCommand,
            ref ICommand? commandBackingField)
        {
            if (!Equals(commandBackingField, newCommand))
            {
                if (commandBackingField is not null)
                {
                    commandBackingField.CanExecuteChanged -= CommandCanExecuteChangedProc;

                    // We need to restore Enabled, should we go from a defined Command to an undefined Command.
                    if (newCommand is null)
                    {
                        if (_previousEnabledStatus.HasValue)
                        {
                            _commandPropertyProvider.Enabled = _previousEnabledStatus.Value;
                            _previousEnabledStatus = null;
                        }
                    }
                }

                commandBackingField = newCommand;
                _commandPropertyProvider.RaiseCommandChanged(EventArgs.Empty);

                if (commandBackingField is null)
                {
                    return;
                }

                commandBackingField.CanExecuteChanged += CommandCanExecuteChangedProc;
                _previousEnabledStatus ??= _commandPropertyProvider.Enabled;

                _commandPropertyProvider.Enabled =
                    commandBackingField.CanExecute(_commandPropertyProvider.CommandParameter);
            }
        }

        private void CommandCanExecuteChangedProc(object? sender, EventArgs e)
            // TODO: Discuss: What do we pass here? ViewModel as sender, or View as sender?
            => _commandPropertyProvider.RaiseCommandCanExecuteChanged(e);

        /// <summary>
        /// Method which should be called by the class utelizing this
        /// Manager, when the assigned <see cref="Command"/> should be executed.
        /// </summary>
        internal void RequestCommandExecute()
        {
            _commandPropertyProvider.Command?.Execute(_commandPropertyProvider.CommandParameter);
        }
    }

    public abstract class BindableComponent : Component, IBindableComponent
    {
        internal static readonly object s_bindingContextChangedEvent = new object();

        private ControlBindingsCollection? _dataBindings;
        private BindingContext? _bindingContext;

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [SRCategory(nameof(SR.CatData))]
        [SRDescription(nameof(SR.ToolStripItemBindingContextDescr))]
        public BindingContext BindingContext
        {
            get => _bindingContext ??= new BindingContext();
            set
            {
                if (!Equals(_bindingContext, value))
                {
                    _bindingContext = value;
                    OnBindingContextChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Occurs when the binding context has changed
        /// </summary>
        [SRCategory(nameof(SR.CatData))]
        [SRDescription(nameof(SR.ToolStripItemOnBindingContextChangedDescr))]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public event EventHandler BindingContextChanged
        {
            add => Events.AddHandler(s_bindingContextChangedEvent, value);
            remove => Events.RemoveHandler(s_bindingContextChangedEvent, value);
        }

        /// <summary>
        ///  Raises the <see cref="BindableComponent.BindingContextChanged"/> event.
        ///  Inheriting classes should override this method to handle this event.
        ///  Call base.OnBindingContextChanged to send this event to any registered event listeners.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnBindingContextChanged(EventArgs e)
        {
            if (_bindingContext is not null)
            {
                for (int i = 0; i < DataBindings.Count; i++)
                {
                    BindingContext.UpdateBinding(BindingContext, DataBindings[i]);
                }
            }

            RaiseEvent(s_bindingContextChangedEvent, e);
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [RefreshProperties(RefreshProperties.All)]
        [ParenthesizePropertyName(true)]
        public ControlBindingsCollection DataBindings
            => _dataBindings ??= new ControlBindingsCollection(this);

        protected void RaiseEvent(object key, EventArgs e)
            => ((EventHandler?)Events[key])?.Invoke(this, e);
    }

    public abstract class CommandComponent : BindableComponent, ICommandPropertyProvider
    {
        // Holds the logic for controlling a command's execution context.
        private CommandProviderManager _commandProviderManager;

        // Backing fields for the infrastructure to make ToolStripItem bindable and introduce (bindable) ICommand.
        private System.Windows.Input.ICommand? _command;
        private object? _commandParameter;

        internal static readonly object s_commandChangedEvent = new object();
        internal static readonly object s_commandParameterChangedEvent = new object();
        internal static readonly object s_commandCanExecuteChangedEvent = new object();

        public CommandComponent() : base()
        {
            _commandProviderManager = new CommandProviderManager(this);
        }

        public abstract bool Enabled { get; set; }

        [Bindable(true)]
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [SRCategory(nameof(SR.CatData))]
        public System.Windows.Input.ICommand? Command
        {
            get => _command;
            set => _commandProviderManager.CommandSetter(value, ref _command);
        }

        /// <summary>
        /// Occurs when the Command.CanExecute status has changed.
        /// </summary>
        [SRCategory(nameof(SR.CatData))]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public event EventHandler? CommandCanExecuteChanged
        {
            add => Events.AddHandler(s_commandCanExecuteChangedEvent, value);
            remove => Events.RemoveHandler(s_commandCanExecuteChangedEvent, value);
        }

        /// <summary>
        /// Occurs when the Command has changed.
        /// </summary>
        [SRCategory(nameof(SR.CatData))]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public event EventHandler? CommandChanged
        {
            add => Events.AddHandler(s_commandChangedEvent, value);
            remove => Events.RemoveHandler(s_commandChangedEvent, value);
        }

        [Bindable(true)]
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [SRCategory(nameof(SR.CatData))]
        public object? CommandParameter
        {
            get => _commandParameter;

            set
            {
                if (!Equals(_commandParameter, value))
                {
                    _commandParameter = value;
                    OnCommandParameterChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Occurs when the CommandParameter has changed.
        /// </summary>
        [SRCategory(nameof(SR.CatData))]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public event EventHandler CommandParameterChanged
        {
            add => Events.AddHandler(s_commandParameterChangedEvent, value);
            remove => Events.RemoveHandler(s_commandParameterChangedEvent, value);
        }

        /// <summary>
        ///  Raises the <see cref="CommandComponent.CommandChanged"/> event.
        ///  Inheriting classes should override this method to handle this event.
        ///  Call base.CommandChanged to send this event to any registered event listeners.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnCommandChanged(EventArgs e)
            => RaiseEvent(s_commandChangedEvent, e);

        /// <summary>
        ///  Raises the <see cref="CommandComponent.CommandCanExecuteChanged"/> event.
        ///  Inheriting classes should override this method to handle this event.
        ///  Call base.CommandCanExecuteChanged to send this event to any registered event listeners.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnCommandCanExecuteChanged(EventArgs e)
            // TODO: Is passing 'this' correct here?
            => ((EventHandler?)Events[s_commandCanExecuteChangedEvent])?.Invoke(this, e);

        /// <summary>
        ///  Raises the <see cref="CommandComponent.CommandParameterChanged"/> event.
        ///  Inheriting classes should override this method to handle this event.
        ///  Call base.CommandChanged to send this event to any registered event listeners.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnCommandParameterChanged(EventArgs e) => RaiseEvent(s_commandParameterChangedEvent, e);

        /// <summary>
        ///  Called by the event of a Control deriving from this class to execute the command.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnRequestCommandExecute(EventArgs e)
            => _commandProviderManager.RequestCommandExecute();

        // Called by the CommandProviderManager's internal DIM-based logic.
        void ICommandPropertyProvider.RaiseCommandChanged(EventArgs e)
            => OnCommandChanged(e);

        // Called by the CommandProviderManager's internal DIM-based logic.
        void ICommandPropertyProvider.RaiseCommandCanExecuteChanged(EventArgs e)
            => OnCommandCanExecuteChanged(e);
    }

    public abstract class CommandControl : Control, ICommandPropertyProvider
    {
        // Holds the logic for controlling a command's execution context.
        private CommandProviderManager _commandProviderManager;

        // Backing fields for the infrastructure to make ToolStripItem bindable and introduce (bindable) ICommand.
        private System.Windows.Input.ICommand? _command;
        private object? _commandParameter;

        internal static readonly object s_commandChangedEvent = new object();
        internal static readonly object s_commandParameterChangedEvent = new object();
        internal static readonly object s_commandCanExecuteChangedEvent = new object();

        public CommandControl() : base()
        {
            _commandProviderManager = new CommandProviderManager(this);
        }

        [Bindable(true)]
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [SRCategory(nameof(SR.CatData))]
        public System.Windows.Input.ICommand? Command
        {
            get => _command;
            set => _commandProviderManager.CommandSetter(value, ref _command);
        }

        /// <summary>
        /// Occurs when the Command.CanExecute status has changed.
        /// </summary>
        [SRCategory(nameof(SR.CatData))]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public event EventHandler? CommandCanExecuteChanged
        {
            add => Events.AddHandler(s_commandCanExecuteChangedEvent, value);
            remove => Events.RemoveHandler(s_commandCanExecuteChangedEvent, value);
        }

        /// <summary>
        /// Occurs when the Command has changed.
        /// </summary>
        [SRCategory(nameof(SR.CatData))]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public event EventHandler? CommandChanged
        {
            add => Events.AddHandler(s_commandChangedEvent, value);
            remove => Events.RemoveHandler(s_commandChangedEvent, value);
        }

        [Bindable(true)]
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [SRCategory(nameof(SR.CatData))]
        public object? CommandParameter
        {
            get => _commandParameter;

            set
            {
                if (!Equals(_commandParameter, value))
                {
                    _commandParameter = value;
                    OnCommandParameterChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Occurs when the CommandParameter has changed.
        /// </summary>
        [SRCategory(nameof(SR.CatData))]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public event EventHandler CommandParameterChanged
        {
            add => Events.AddHandler(s_commandParameterChangedEvent, value);
            remove => Events.RemoveHandler(s_commandParameterChangedEvent, value);
        }

        /// <summary>
        ///  Raises the <see cref="CommandComponent.CommandChanged"/> event.
        ///  Inheriting classes should override this method to handle this event.
        ///  Call base.CommandChanged to send this event to any registered event listeners.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnCommandChanged(EventArgs e)
            => RaiseEvent(s_commandChangedEvent, e);

        /// <summary>
        ///  Raises the <see cref="CommandComponent.CommandCanExecuteChanged"/> event.
        ///  Inheriting classes should override this method to handle this event.
        ///  Call base.CommandCanExecuteChanged to send this event to any registered event listeners.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnCommandCanExecuteChanged(EventArgs e)
            // TODO: Is passing 'this' correct here?
            => ((EventHandler?)Events[s_commandCanExecuteChangedEvent])?.Invoke(this, e);

        /// <summary>
        ///  Raises the <see cref="CommandComponent.CommandParameterChanged"/> event.
        ///  Inheriting classes should override this method to handle this event.
        ///  Call base.CommandChanged to send this event to any registered event listeners.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnCommandParameterChanged(EventArgs e) => RaiseEvent(s_commandParameterChangedEvent, e);

        /// <summary>
        ///  Called by the event of a Control deriving from this class to execute the command.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnRequestCommandExecute(EventArgs e)
            => _commandProviderManager.RequestCommandExecute();

        // Called by the CommandProviderManager's internal DIM-based logic.
        void ICommandPropertyProvider.RaiseCommandChanged(EventArgs e)
            => OnCommandChanged(e);

        // Called by the CommandProviderManager's internal DIM-based logic.
        void ICommandPropertyProvider.RaiseCommandCanExecuteChanged(EventArgs e)
            => OnCommandCanExecuteChanged(e);

        private void RaiseEvent(object key, EventArgs e)
            => ((EventHandler?)Events[key])?.Invoke(this, e);
    }
}
