// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;

namespace System.Windows.Forms
{
    public sealed class LayoutEventArgs : EventArgs
    {
        public LayoutEventArgs(IComponent affectedComponent, string affectedProperty)
        {
            AffectedComponent = affectedComponent;
            AffectedProperty = affectedProperty;
        }

        public LayoutEventArgs(Control affectedControl, string affectedProperty) : this((IComponent)affectedControl, affectedProperty)
        {
        }

        public IComponent AffectedComponent { get; }

        public Control AffectedControl => AffectedComponent as Control;

        public string AffectedProperty { get; }
    }
}
