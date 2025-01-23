// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

namespace System.Windows.Forms.DataBinding.TestUtilities;

/// <summary>
///  Basic implementation of a command in a ViewModel/UI-Controller which can be bound to a property of type
///  <see cref="Input.ICommand"/>.
/// </summary>
public class RelayCommand : Input.ICommand
{
    public event EventHandler? CanExecuteChanged;

    private readonly Action<object?> _execute;
    private readonly Func<object?, bool>? _canExecute;

    /// <summary>
    ///  Creates an instance of this class and passes the command's execution delegate.
    /// </summary>
    /// <param name="execute">Action delegate which gets invoked with the <see cref="Execute(object?)"/> method.</param>
    public RelayCommand(Action<object?> execute) : this(execute, null)
    {
    }

    /// <summary>
    ///  Creates an instance of this class and passes the command's execution delegate and the command's
    ///  predicate for determining whether the command can be executed.
    /// </summary>
    /// <param name="execute">Action delegate which gets invoked with the <see cref="Execute(object?)"/> method.</param>
    /// <param name="canExecute">Predicate for determining with <see cref="CanExecute(object?)"/>
    /// whether the command can be executed.</param>
    /// <exception cref="ArgumentNullException"></exception>
    public RelayCommand(Action<object?> execute, Func<object?, bool>? canExecute)
    {
        _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        _canExecute = canExecute;
    }

    /// <summary>
    ///  Called to indicate that the execution context of the command has changed.
    /// </summary>
    public void RaiseCanExecuteChanged()
        => CanExecuteChanged?.Invoke(this, EventArgs.Empty);

    /// <summary>
    ///  Determines whether the command can be executed.
    /// </summary>
    public bool CanExecute(object? parameter)
        => _canExecute is null || _canExecute(parameter);

    /// <summary>
    ///  Executes the command.
    /// </summary>
    public void Execute(object? parameter)
        => _execute(parameter);
}
