// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms.Design
{
    /// <summary>
    ///  Provides data for the <see cref='System.Windows.Forms.Design.ISelectionUIService.ContainerSelectorActive'/> event.
    /// </summary>
    internal class ContainerSelectorActiveEventArgs : EventArgs
    {
        private readonly object _component;
        private readonly ContainerSelectorActiveEventArgsType _eventType;

        /// <summary>
        ///  Initializes a new instance of the 'ContainerSelectorActiveEventArgs' class.
        /// </summary>
        public ContainerSelectorActiveEventArgs(object component) : this(component, ContainerSelectorActiveEventArgsType.Mouse)
        {
        }

        /// <summary>
        ///  Initializes a new instance of the 'ContainerSelectorActiveEventArgs' class.
        /// </summary>
        public ContainerSelectorActiveEventArgs(object component, ContainerSelectorActiveEventArgsType eventType)
        {
            _component = component;
            _eventType = eventType;
        }
    }
}
