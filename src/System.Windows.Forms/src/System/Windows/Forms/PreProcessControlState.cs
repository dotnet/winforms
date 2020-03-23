// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    public enum PreProcessControlState
    {
        /// <summary>
        ///  Indicates the message has been processed, and no further processing
        ///  is necessary
        /// </summary>
        MessageProcessed = 0x00,

        /// <summary>
        ///  Indicates the control wants the message and processing should continue
        /// </summary>
        MessageNeeded = 0x01,

        /// <summary>
        ///  Indicates the control doesn't care about the message
        /// </summary>
        MessageNotNeeded = 0x02
    }
}
