// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace System.Windows.Forms {

    using System.Diagnostics;
    using System;
    using System.ComponentModel;
    
    

    /// <include file='doc\PropertyValueChangedEventHandler.uex' path='docs/doc[@for="PropertyValueChangedEventHandler"]/*' />
    /// <devdoc>
    /// The event handler class that is invoked when a property
    /// in the grid is modified by the user.
    /// </devdoc>
    public delegate void PropertyValueChangedEventHandler(object s, PropertyValueChangedEventArgs e);
}
