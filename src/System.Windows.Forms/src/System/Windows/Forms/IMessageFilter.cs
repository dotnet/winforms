// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace System.Windows.Forms {
    using System.Runtime.Remoting;

    using System.Diagnostics;
    
    using System;
	using System.Security;
	using System.Security.Permissions;

    /// <include file='doc\IMessageFilter.uex' path='docs/doc[@for="IMessageFilter"]/*' />
    /// <devdoc>
    ///    <para> 
    ///       Defines a message filter interface.</para>
    /// </devdoc>
    public interface IMessageFilter {
    
        /// <include file='doc\IMessageFilter.uex' path='docs/doc[@for="IMessageFilter.PreFilterMessage"]/*' />
        /// <devdoc>
        ///    <para>Filters out a message before it is dispatched. </para>
        /// </devdoc>
        /// 







		[
		System.Security.Permissions.SecurityPermissionAttribute(System.Security.Permissions.SecurityAction.LinkDemand, Flags=System.Security.Permissions.SecurityPermissionFlag.UnmanagedCode)
		]
		bool PreFilterMessage(ref Message m);
    }
}
