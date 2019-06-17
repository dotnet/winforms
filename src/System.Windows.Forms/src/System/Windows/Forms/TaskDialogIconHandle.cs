// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;

namespace System.Windows.Forms
{
    /// <summary>
    /// 
    /// </summary>
    public class TaskDialogIconHandle : TaskDialogIcon
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="iconHandle"></param>
        public TaskDialogIconHandle(IntPtr iconHandle)
        {
            IconHandle = iconHandle;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="icon"></param>
        public TaskDialogIconHandle(Icon icon)
            : this(icon?.Handle ?? default)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public IntPtr IconHandle
        {
            get;
        }
    }
}
