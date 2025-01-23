// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable

namespace System.Windows.Forms.DataBinding.TestUtilities;

/// <summary>
/// Provides the basic implementation of <see cref="INotifyPropertyChanged"/>
/// to simplify the creation of UI ViewModels/Controllers.
/// </summary>
public abstract class ViewModelBase : INotifyPropertyChanged
{
    /// <summary>
    /// Creates an instance of the BindableBase class.
    /// </summary>
    public ViewModelBase() : base()
    {
    }

    /// <summary>
    /// Event for property change notification.
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Assigns a new value to a property and raises <see cref="PropertyChanged"/> if the value has actually changed.
    /// </summary>
    /// <typeparam name="T">Type of the property.</typeparam>
    /// <param name="storage">Reference to the property backing field.</param>
    /// <param name="value">New value for the property.</param>
    /// <param name="propertyName">Name of the property used to notify listeners. This value
    /// is optional and can be provided automatically when invoked with support of
    /// <see cref="CallerMemberNameAttribute"/>.</param>
    protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string? propertyName = null)
    {
        if (Equals(storage, value))
        {
            return false;
        }

        storage = value;
        OnPropertyChanged(propertyName);
        return true;
    }

    /// <summary>
    /// Raises the <see cref="PropertyChanged"/> event.
    /// </summary>
    /// <param name="propertyName">Name of the property used to notify listeners. This value
    /// is optional and can be provided automatically when invoked from compilers that support
    /// <see cref="CallerMemberNameAttribute"/>.</param>
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
