// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {

    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System;


    /// <internalonly />
    [SuppressMessage("Microsoft.Design", "CA1027:MarkEnumsWithFlags")]
    [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly")]
    public enum PreProcessControlState {

        /// <devdoc>
        ///    <para>Indicates the message has been processed, and no further processing is necessary</para>
        /// </devdoc>
        MessageProcessed =  0x00,

        /// <devdoc>
        ///    <para>Indicates the control wants the message and processing should continue</para>
        /// </devdoc>
        MessageNeeded    =  0x01,

        /// <devdoc>
        ///    <para>Indicates the control doesn't care about the message</para>
        /// </devdoc>
        MessageNotNeeded  =  0x02
    }
}

