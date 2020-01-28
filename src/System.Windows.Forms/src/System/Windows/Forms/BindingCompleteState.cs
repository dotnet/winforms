// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <summary>
    ///  Indicates the result of a completed binding operation.
    /// </summary>
    public enum BindingCompleteState
    {
        /// <summary>
        ///  Binding operation completed successfully.
        /// </summary>
        Success = 0,

        /// <summary>
        ///  Binding operation failed with a data error.
        /// </summary>
        DataError = 1,

        /// <summary>
        ///  Binding operation failed with an exception.
        /// </summary>
        Exception = 2,
    }
}
