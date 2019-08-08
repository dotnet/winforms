// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms.Design
{
    /// <summary>
    ///  Specifies numeric IDs for different types of adornments on a component.
    /// </summary>
    internal enum AdornmentType
    {
        /// <summary>
        ///  Specifies the type as grab handle adornments.
        /// </summary>
        GrabHandle = 1,
        /// <summary>
        ///  Specifies the type as container selector adornments.
        /// </summary>
        ContainerSelector = 2,
        /// <summary>
        ///  Specifies the type as the maximum size of any adornment.
        /// </summary>
        Maximum = 3,
    }
}
