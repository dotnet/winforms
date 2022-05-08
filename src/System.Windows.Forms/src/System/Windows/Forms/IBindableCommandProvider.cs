// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Windows.Input;

namespace System.Windows.Forms
{
    public delegate void BindableCommandEventHandler(object sender, BindableCommandEventArgs e);

    public interface IBindableCommandProvider
    {
        event EventHandler BindableCommandChanged;
        event BindableCommandEventHandler BindableCommandCanExecuteChanged;
        event BindableCommandEventHandler BindableCommandExecute;

        ICommand BindableCommand { get; set; }
        bool Enabled { get; set; }

        protected bool? PreviousEnabledStatus { get; set; }
        protected void HandleBindableCommandChanged(EventArgs e);
        protected void HandleBindableCommandCanExecuteChanged(object sender, BindableCommandEventArgs e);
        protected void HandleBindableCommandExecute(BindableCommandEventArgs e);

        protected static void BindableCommandSetter(
            IBindableCommandProvider commandComponent,
            ICommand newCommand,
            ref ICommand bindableCommandBackingField)
            => commandComponent.BindableCommandSetter(newCommand, ref bindableCommandBackingField);

        protected static void RequestCommandExecute(IBindableCommandProvider commandComponent)
        {
            BindableCommandEventArgs e = new();
            commandComponent.HandleBindableCommandExecute(e);

            if (!e.Cancel && (commandComponent.BindableCommand?.CanExecute(null) ?? false))
            {
                commandComponent.BindableCommand?.Execute(null);
            }
        }

        private void BindableCommandSetter(
            ICommand newCommand,
            ref ICommand bindableCommandBackingField)
        {
            if (!Equals(bindableCommandBackingField, newCommand))
            {
                if (bindableCommandBackingField is not null)
                {
                    bindableCommandBackingField.CanExecuteChanged -= CommandCanExecuteChanged;

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

                bindableCommandBackingField = newCommand;
                HandleBindableCommandChanged(EventArgs.Empty);

                if (bindableCommandBackingField is null)
                {
                    return;
                }

                bindableCommandBackingField.CanExecuteChanged += CommandCanExecuteChanged;
                PreviousEnabledStatus ??= Enabled;
                Enabled = bindableCommandBackingField.CanExecute(null);
            }
        }

        private void CommandCanExecuteChanged(object sender, EventArgs e)
        {
            BindableCommandEventArgs bindableCommandEventArgs = new();
            HandleBindableCommandCanExecuteChanged(sender, bindableCommandEventArgs);

            if (!bindableCommandEventArgs.Cancel)
            {
                Enabled = BindableCommand?.CanExecute(bindableCommandEventArgs.Parameter) ?? false;
            }
        }
    }

    internal class BindableCommandControl : Control, IBindableCommandProvider
    {
        public event EventHandler BindableCommandChanged;
        public event BindableCommandEventHandler BindableCommandCanExecuteChanged;
        public event BindableCommandEventHandler BindableCommandExecute;

        private ICommand _bindableCommand;

        public ICommand BindableCommand
        {
            get => _bindableCommand;
            set => IBindableCommandProvider.BindableCommandSetter(this, value, ref _bindableCommand);
        }

        bool? IBindableCommandProvider.PreviousEnabledStatus { get; set; }

        protected virtual void OnBindableCommandChanged(EventArgs e)
            => BindableCommandChanged?.Invoke(this, e);

        protected virtual void OnBindableCommandCanExecuteChanged(object sender, BindableCommandEventArgs e)
            => BindableCommandCanExecuteChanged?.Invoke(this, e);

        void IBindableCommandProvider.HandleBindableCommandChanged(EventArgs e)
            => OnBindableCommandChanged(e);

        void IBindableCommandProvider.HandleBindableCommandCanExecuteChanged(object sender, BindableCommandEventArgs e)
            => OnBindableCommandCanExecuteChanged(sender, e);

        void IBindableCommandProvider.HandleBindableCommandExecute(BindableCommandEventArgs e)
            => OnBindableCommandExecute(e);

        protected virtual void OnBindableCommandExecute(BindableCommandEventArgs e)
            => BindableCommandExecute?.Invoke(this, e);

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);
            IBindableCommandProvider.RequestCommandExecute(this);
        }
    }
}
