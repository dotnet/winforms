// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {

    using System;
    using System.ComponentModel;

    /// <devdoc>
    /// </devdoc>
    public interface IBindableComponent : IComponent {

        /// <devdoc>
        /// </devdoc>
        ControlBindingsCollection DataBindings { get; }

        /// <devdoc>
        /// </devdoc>
        BindingContext BindingContext { get; set; }

    }
}
