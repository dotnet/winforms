// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.CodeAnalysis;

namespace System.Windows.Forms
{
    [SuppressMessage("Microsoft.Design", "CA1027:MarkEnumsWithFlags")]
    [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly")]
    public enum PreProcessControlState
    {
        /// <summary>
        /// Indicates the message has been processed, and no further processing
        /// is necessary
        /// </devdoc>
        MessageProcessed = 0x00,

        /// <summary>
        /// Indicates the control wants the message and processing should continue
        /// </devdoc>
        MessageNeeded = 0x01,

        /// <summary>
        /// Indicates the control doesn't care about the message
        /// </devdoc>
        MessageNotNeeded = 0x02
    }
}
