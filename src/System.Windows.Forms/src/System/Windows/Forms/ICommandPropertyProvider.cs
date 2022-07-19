// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Input;

namespace System.Windows.Forms
{
    internal interface ICommandPropertyProvider
    {
        public class CommandProviderManager
        {
            private ICommandPropertyProvider _commandPropertyProvider;
            private bool? _previousEnabledStatus;

            public CommandProviderManager(ICommandPropertyProvider commandPropertyProvider)
            {
                _commandPropertyProvider = commandPropertyProvider;
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
}
