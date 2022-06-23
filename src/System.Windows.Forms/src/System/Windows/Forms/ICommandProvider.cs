﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Input;

namespace System.Windows.Forms
{
    public interface ICommandProvider
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
        /// Occurs, when the command is about to be executed. 
        /// </summary>
        event CancelEventHandler? CommandExecuting;

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
        /// Gets or sets the previous value of the <see cref="Enabled"/> property, so
        /// that it can be restored to its original value on assignment of a new command.
        /// </summary>
        protected bool? PreviousEnabledStatus { get; set; }

        /// <summary>
        /// An implementation should raise the <see cref="CommandChanged"/> event 
        /// by calling component's or control's OnRaiseCommandChanged method.
        /// </summary>
        /// <param name="e">An empty <see cref="EventArgs"/> instance.</param>
        protected void RaiseCommandChanged(EventArgs e);

        /// <summary>
        /// An implementation should raise the <see cref="CommandCanExecuteChanged"/> event 
        /// by calling component's or control's OnCommandCanExecuteChanged method.
        /// </summary>
        /// <param name="sender">The sender of the event, passed from the data source.</param>
        /// <param name="e">An empty <see cref="EventArgs"/> instance.</param>
        protected void RaiseCommandCanExecuteChanged([AllowNull] object sender, EventArgs e);

        /// <summary>
        /// An implementation should raise the <see cref="CommandExecuting"/> event 
        /// by calling component's or control's OnCommandExecuting method.
        /// </summary>
        /// <param name="e"></param>
        protected void RaiseCommandExecuting(CancelEventArgs e);

        /// <summary>
        /// Method which should be called by a class implementing this 
        /// interface in the setter of the <see cref="Command"/> property.
        /// </summary>
        /// <param name="commandComponent">Instance of the class implementing this interface.</param>
        /// <param name="newCommand">The new value of the <see cref="Command"/>
        /// which should be assigned to the property.</param>
        /// <param name="commandBackingField">The backing field for
        /// the <see cref="Command"/> property.</param>
        protected static void CommandSetter(
            ICommandProvider commandComponent,
            ICommand? newCommand,
            ref ICommand? commandBackingField)
            => commandComponent.CommandSetter(newCommand, ref commandBackingField);

        /// <summary>
        /// Method which should be called by the class implementing this
        /// interface, when the assigned <see cref="Command"/> should be executed.
        /// </summary>
        /// <remarks>
        /// As an example, a <see cref="Button"/> should call this method inside the method which
        /// also raises the <see cref="Control.Click"/> Event of that Button, which
        /// would be the <see cref="Button.OnClick(EventArgs)"/> OnClick method.
        /// </remarks>
        /// <param name="commandComponent"></param>
        protected static void RequestCommandExecute(ICommandProvider commandComponent)
        {
            CancelEventArgs e = new();
            commandComponent.RaiseCommandExecuting(e);

            if (!e.Cancel && (commandComponent.Command?.CanExecute(commandComponent.CommandParameter) ?? false))
            {
                commandComponent.Command?.Execute(commandComponent.CommandParameter);
            }
        }

        private void CommandSetter(
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
                        if (PreviousEnabledStatus.HasValue)
                        {
                            Enabled = PreviousEnabledStatus.Value;
                            PreviousEnabledStatus = null;
                        }
                    }
                }

                commandBackingField = newCommand;
                RaiseCommandChanged(EventArgs.Empty);

                if (commandBackingField is null)
                {
                    return;
                }

                commandBackingField.CanExecuteChanged += CommandCanExecuteChangedProc;
                PreviousEnabledStatus ??= Enabled;
                Enabled = commandBackingField.CanExecute(CommandParameter);
            }
        }

        private void CommandCanExecuteChangedProc([AllowNull] object sender, EventArgs e)
        {
            CancelEventArgs cancelEventArgs = new();
            RaiseCommandCanExecuteChanged(sender, cancelEventArgs);

            if (!cancelEventArgs.Cancel)
            {
                Enabled = Command?.CanExecute(CommandParameter) ?? false;
            }
        }
    }
}
