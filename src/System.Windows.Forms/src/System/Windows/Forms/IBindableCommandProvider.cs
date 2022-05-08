// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.ComponentModel;
using System.Windows.Input;

namespace System.Windows.Forms
{
    internal interface IBindableCommandProvider
    {
        event EventHandler BindableCommandChanged;
        event EventHandler BindableCommandCanExecuteChanged;
        event CancelEventHandler BindableCommandExecute;

        ICommand BindableCommand { get; set; }
        protected bool EnabledStatus {get; set;}
        protected bool? PreviousEnabledStatus { get; set; }
        protected void HandleBindableCommandChanged(EventArgs e);
        protected void HandleBindableCommandCanExecuteChanged(EventArgs e);
        protected void HandleCommandExecute(CancelEventArgs e);

        protected static void BindableCommandSetter(
            IBindableCommandProvider commandComponent,
            ICommand newCommand,
            ICommand bindableCommandBackingField)
            => commandComponent.BindableCommandSetter(newCommand, bindableCommandBackingField);

        protected static void RequestCommandExecute(IBindableCommandProvider commandComponent)
        {
        }

        private void BindableCommandSetter(
            ICommand newCommand,
            ICommand bindableCommandBackingField)
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
                            EnabledStatus = PreviousEnabledStatus.Value;
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
                PreviousEnabledStatus ??= EnabledStatus;
                EnabledStatus = bindableCommandBackingField.CanExecute(null);
            }
        }

        private void CommandCanExecuteChanged(object? sender, EventArgs e)
        {
            EnabledStatus = BindableCommand?.CanExecute(null) ?? false;
            HandleBindableCommandCanExecuteChanged(EventArgs.Empty);
        }
    }

    internal class BindableCommandControl : Control, IBindableCommandProvider
    {
        public event EventHandler BindableCommandChanged;
        public event EventHandler BindableCommandCanExecuteChanged;
        public event CancelEventHandler BindableCommandExecute;

        private ICommand _bindableCommand;

        public ICommand BindableCommand
        {
            get => _bindableCommand;
            set => IBindableCommandProvider.BindableCommandSetter(this, value, _bindableCommand);
        }

        bool IBindableCommandProvider.EnabledStatus
        {
            get => Enabled;
            set => Enabled = value;
        }

        bool? IBindableCommandProvider.PreviousEnabledStatus { get; set; }

        void IBindableCommandProvider.HandleBindableCommandChanged(EventArgs e)
            => OnBindableCommandChanged(e);

        protected virtual void OnBindableCommandChanged(EventArgs e)
            => BindableCommandChanged?.Invoke(this, e);

        void IBindableCommandProvider.HandleBindableCommandCanExecuteChanged(EventArgs e)
            => OnBindableCommandCanExecuteChanged(e);

        protected virtual void OnBindableCommandCanExecuteChanged(EventArgs e)
            => BindableCommandCanExecuteChanged?.Invoke(this, e);

        void IBindableCommandProvider.HandleCommandExecute(CancelEventArgs e)
            => OnHandleCommandExecute(e);

        protected virtual void OnHandleCommandExecute(CancelEventArgs e)
            => BindableCommandExecute?.Invoke(this, e);

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);
            IBindableCommandProvider.RequestCommandExecute(this);
        }
    }
}
