// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;

namespace System.Windows.Forms.IntegrationTests.Mocks
{
    public class MainObject : INotifyPropertyChanged
    {
        private string text;
        private PropertyChangedEventHandler _propertyChanged;

        public string Text
        {
            get { return text; }
            set
            {
                if (text != value)
                {
                    text = value;
                    if (_propertyChanged != null)
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

        public bool IsPropertyChangedAssigned { get { return _propertyChanged != null; } }
    }
}
