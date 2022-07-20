// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Runtime.Versioning;

namespace System.Windows.Forms
{
    public abstract class CommandComponent : BindableComponent, ICommandPropertyProvider
    {
        // Holds the logic for controlling a command's execution context.
        private readonly ICommandPropertyProvider.CommandProviderManager _commandProviderManager;

        // Backing fields for the infrastructure to make ToolStripItem bindable and introduce (bindable) ICommand.
        private System.Windows.Input.ICommand? _command;
        private object? _commandParameter;

        internal static readonly object s_commandChangedEvent = new();
        internal static readonly object s_commandParameterChangedEvent = new();
        internal static readonly object s_commandCanExecuteChangedEvent = new();

        [RequiresPreviewFeatures]
        public CommandComponent() : base()
        {
            _commandProviderManager = new(this);
        }

        [RequiresPreviewFeatures]
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
        [RequiresPreviewFeatures]
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
        [RequiresPreviewFeatures]
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
            [RequiresPreviewFeatures]
            get => _commandParameter;

            [RequiresPreviewFeatures]
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
        [RequiresPreviewFeatures]
        [SRCategory(nameof(SR.CatData))]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public event EventHandler? CommandParameterChanged
        {
            add => Events.AddHandler(s_commandParameterChangedEvent, value);
            remove => Events.RemoveHandler(s_commandParameterChangedEvent, value);
        }

        public abstract bool Enabled { get; set; }

        /// <summary>
        ///  Raises the <see cref="CommandComponent.CommandChanged"/> event.
        ///  Inheriting classes should override this method to handle this event.
        ///  Call base.CommandChanged to send this event to any registered event listeners.
        /// </summary>
        [RequiresPreviewFeatures]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnCommandChanged(EventArgs e)
            => RaiseEvent(s_commandChangedEvent, e);

        /// <summary>
        ///  Raises the <see cref="CommandComponent.CommandCanExecuteChanged"/> event.
        ///  Inheriting classes should override this method to handle this event.
        ///  Call base.CommandCanExecuteChanged to send this event to any registered event listeners.
        /// </summary>
        [RequiresPreviewFeatures]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnCommandCanExecuteChanged(EventArgs e)
            // TODO: Is passing 'this' correct here?
            => ((EventHandler?)Events[s_commandCanExecuteChangedEvent])?.Invoke(this, e);

        /// <summary>
        ///  Raises the <see cref="CommandComponent.CommandParameterChanged"/> event.
        ///  Inheriting classes should override this method to handle this event.
        ///  Call base.CommandChanged to send this event to any registered event listeners.
        /// </summary>
        [RequiresPreviewFeatures]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnCommandParameterChanged(EventArgs e) => RaiseEvent(s_commandParameterChangedEvent, e);

        /// <summary>
        ///  Called by the event of a Control deriving from this class to execute the command.
        /// </summary>
        /// <param name="e"></param>
        [RequiresPreviewFeatures]
        protected virtual void OnRequestCommandExecute(EventArgs e)
            => _commandProviderManager.RequestCommandExecute();

        // Called by the CommandProviderManager's internal DIM-based logic.
        [RequiresPreviewFeatures]
        void ICommandPropertyProvider.RaiseCommandChanged(EventArgs e)
            => OnCommandChanged(e);

        // Called by the CommandProviderManager's internal DIM-based logic.
        [RequiresPreviewFeatures]
        void ICommandPropertyProvider.RaiseCommandCanExecuteChanged(EventArgs e)
            => OnCommandCanExecuteChanged(e);
    }
}
