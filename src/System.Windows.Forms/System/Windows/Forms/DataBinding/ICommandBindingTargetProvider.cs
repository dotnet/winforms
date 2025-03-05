// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Windows.Input;

namespace System.Windows.Forms;

internal interface ICommandBindingTargetProvider
{
    /// <summary>
    ///  Occurs when the <see cref="Command"/> property has changed.
    /// </summary>
    event EventHandler? CommandChanged;

    /// <summary>
    ///  Occurs when the execution context of the <see cref="Command"/> was changed.
    /// </summary>
    event EventHandler? CommandCanExecuteChanged;

    /// <summary>
    ///  Gets or sets the Command to invoke, when the implementing Component or Control is triggered by the user.
    /// </summary>
    ICommand? Command { get; set; }

    /// <summary>
    ///  Gets or sets the CommandParameter for the Component or Control.
    /// </summary>
    object? CommandParameter { get; set; }

    /// <summary>
    ///  Gets or sets a value indicating whether the control can respond to user interaction.
    /// </summary>
    bool Enabled { get; set; }

    /// <summary>
    ///  Gets or sets the previous value of the <see cref="Enabled"/> property, so that it can be restored to
    ///  its original value on assignment of a new command.
    /// </summary>
    protected bool? PreviousEnabledStatus { get; set; }

    /// <summary>
    ///  An implementation should raise the <see cref="CommandChanged"/> event  by calling component's or
    ///  control's OnRaiseCommandChanged method.
    /// </summary>
    /// <param name="e">An empty <see cref="EventArgs"/> instance.</param>
    protected void RaiseCommandChanged(EventArgs e);

    /// <summary>
    ///  An implementation should raise the <see cref="CommandCanExecuteChanged"/> event
    ///  by calling component's or control's OnCommandCanExecuteChanged method.
    /// </summary>
    /// <param name="e">An empty <see cref="EventArgs"/> instance.</param>
    protected void RaiseCommandCanExecuteChanged(EventArgs e);

    /// <summary>
    ///  Method which should be called by a class implementing this interface in the setter of the
    /// <see cref="Command"/> property.
    /// </summary>
    /// <param name="commandComponent">Instance of the class implementing this interface.</param>
    /// <param name="newCommand">The new value of the <see cref="Command"/>
    ///  which should be assigned to the property.</param>
    /// <param name="commandBackingField">
    ///  The backing field for the <see cref="Command"/> property.
    /// </param>
    protected static void CommandSetter(
        ICommandBindingTargetProvider commandComponent,
        ICommand? newCommand,
        ref ICommand? commandBackingField)
        => commandComponent.CommandSetter(newCommand, ref commandBackingField);

    /// <summary>
    ///  Method which should be called by the class implementing this interface, when the assigned
    /// <see cref="Command"/> should be executed.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   As an example, a <see cref="Button"/> should call this method inside the method which
    ///   also raises the <see cref="Control.Click"/> Event of that Button, which
    ///   would be the <see cref="Button.OnClick(EventArgs)"/> OnClick method.
    ///   See <see cref="ButtonBase"/> for an example implementation.
    ///  </para>
    /// </remarks>
    /// <param name="commandComponent"></param>
    protected static void RequestCommandExecute(ICommandBindingTargetProvider commandComponent)
    {
        commandComponent.Command?.Execute(commandComponent.CommandParameter);
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
        RaiseCommandCanExecuteChanged(e);
        Enabled = Command?.CanExecute(CommandParameter) ?? false;
    }
}
