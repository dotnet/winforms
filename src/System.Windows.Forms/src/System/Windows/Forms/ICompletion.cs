// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if false
namespace System.Windows.Forms
{
    public interface ICompletion
    {
       /// <devdoc>
        /// This function will be called by the ThreadPool's worker threads when a
        /// packet is ready.
        /// </devdoc>
        void CompletionStatusChanged(bool status, int size, NativeMethods.OVERLAPPED overlapped);
    }
}
#endif
