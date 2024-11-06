// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

namespace System.Windows.Forms.DataBinding.TestUtilities;

public class CommandViewModel : ViewModelBase
{
    private bool _testCommandExecutionAbility;

    public CommandViewModel()
    {
        _testCommand = new RelayCommand(TestCommandExecute, TestCommandCanExecute);
    }

    private RelayCommand? _testCommand;

    public RelayCommand? TestCommand
    {
        get => _testCommand;
        set => SetProperty(ref _testCommand, value);
    }

    public bool TestCommandExecutionAbility
    {
        get => _testCommandExecutionAbility;

        set
        {
            if (SetProperty(ref _testCommandExecutionAbility, value))
            {
                TestCommand?.RaiseCanExecuteChanged();
            }
        }
    }

    public object? CommandExecuteResult { get; private set; }

    private void TestCommandExecute(object? parameter)
    {
        CommandExecuteResult = parameter;
    }

    private bool TestCommandCanExecute(object? parameter)
    {
        return TestCommandExecutionAbility;
    }
}
