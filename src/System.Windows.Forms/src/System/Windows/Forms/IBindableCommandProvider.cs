// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Windows.Input;

namespace System.Windows.Forms
{
    public delegate void CommandEventHandler(object sender, CommandEventArgs e);

    public interface ICommandProvider
    {
        event EventHandler CommandChanged;
        event CommandEventHandler CommandCanExecuteChanged;
        event CommandEventHandler CommandExecute;

        ICommand Command { get; set; }
        bool Enabled { get; set; }

        protected bool? PreviousEnabledStatus { get; set; }
        protected void HandleCommandChanged(EventArgs e);
        protected void HandleCommandCanExecuteChanged(object sender, CommandEventArgs e);
        protected void HandleCommandExecute(CommandEventArgs e);

        protected static void CommandSetter(
            ICommandProvider commandComponent,
            ICommand newCommand,
            ref ICommand commandBackingField)
            => commandComponent.CommandSetter(newCommand, ref commandBackingField);

        protected static void RequestCommandExecute(ICommandProvider commandComponent)
        {
            CommandEventArgs e = new();
            commandComponent.HandleCommandExecute(e);

            if (!e.Cancel && (commandComponent.Command?.CanExecute(null) ?? false))
            {
                commandComponent.Command?.Execute(null);
            }
        }

        private void CommandSetter(
            ICommand newCommand,
            ref ICommand commandBackingField)
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
                HandleCommandChanged(EventArgs.Empty);

                if (commandBackingField is null)
                {
                    return;
                }

                commandBackingField.CanExecuteChanged += CommandCanExecuteChangedProc;
                PreviousEnabledStatus ??= Enabled;
                Enabled = commandBackingField.CanExecute(null);
            }
        }

        private void CommandCanExecuteChangedProc(object sender, EventArgs e)
        {
            CommandEventArgs commandEventArgs = new();
            HandleCommandCanExecuteChanged(sender, commandEventArgs);

            if (!commandEventArgs.Cancel)
            {
                Enabled = Command?.CanExecute(commandEventArgs.Parameter) ?? false;
            }
        }
    }

    internal class CommandControl : Control, ICommandProvider
    {
        public event EventHandler CommandChanged;
        public event CommandEventHandler CommandCanExecuteChanged;
        public event CommandEventHandler CommandExecute;

        private ICommand _command;

        public ICommand Command
        {
            get => _command;
            set => ICommandProvider.CommandSetter(this, value, ref _command);
        }

        bool? ICommandProvider.PreviousEnabledStatus { get; set; }

        protected virtual void OnCommandChanged(EventArgs e)
            => CommandChanged?.Invoke(this, e);

        protected virtual void OnCommandCanExecuteChanged(object sender, CommandEventArgs e)
            => CommandCanExecuteChanged?.Invoke(this, e);

        void ICommandProvider.HandleCommandChanged(EventArgs e)
            => OnCommandChanged(e);

        void ICommandProvider.HandleCommandCanExecuteChanged(object sender, CommandEventArgs e)
            => OnCommandCanExecuteChanged(sender, e);

        void ICommandProvider.HandleCommandExecute(CommandEventArgs e)
            => OnCommandExecute(e);

        protected virtual void OnCommandExecute(CommandEventArgs e)
            => CommandExecute?.Invoke(this, e);

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);
            ICommandProvider.RequestCommandExecute(this);
        }
    }
}
