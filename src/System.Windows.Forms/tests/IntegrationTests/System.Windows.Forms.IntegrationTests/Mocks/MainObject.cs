// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;

namespace System.Windows.Forms.IntegrationTests.Mocks;

public class MainObject : INotifyPropertyChanged
{
    private string _text;
    private PropertyChangedEventHandler _propertyChanged;

    public string Text
    {
        get => _text;
        set
        {
            if (_text != value)
            {
                _text = value;
                if (_propertyChanged is not null)
                {
                    _propertyChanged(this, new PropertyChangedEventArgs(nameof(Text)));
                }
            }
        }
    }

    public event PropertyChangedEventHandler PropertyChanged
    {
        add => _propertyChanged += value;
        remove => _propertyChanged -= value;
    }

    public bool IsPropertyChangedAssigned { get { return _propertyChanged is not null; } }
}
