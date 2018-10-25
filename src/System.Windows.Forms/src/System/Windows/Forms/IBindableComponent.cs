// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {

    using System;
    using System.ComponentModel;

    /// <include file='doc\IBindableComponent.uex' path='docs/doc[@for="IBindableComponent"]/*' />
    /// <devdoc>
    /// </devdoc>
    public interface IBindableComponent : IComponent {

        /// <include file='doc\IBindableComponent.uex' path='docs/doc[@for="IBindableComponent.DataBindings"]/*' />
        /// <devdoc>
        /// </devdoc>
        ControlBindingsCollection DataBindings { get; }

        /// <include file='doc\IBindableComponent.uex' path='docs/doc[@for="IBindableComponent.BindingContext"]/*' />
        /// <devdoc>
        /// </devdoc>
        BindingContext BindingContext { get; set; }

    }
}
