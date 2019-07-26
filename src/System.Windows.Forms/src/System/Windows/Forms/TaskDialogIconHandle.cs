// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable

using System.Drawing;

namespace System.Windows.Forms
{
    /// <summary>
    /// Represents a task dialog icon from an icon handle or a
    /// <see cref="Icon"/> object.
    /// </summary>
    public class TaskDialogIconHandle : TaskDialogIcon
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TaskDialogIconHandle"/> class
        /// from the specified icon handle (<c>HICON</c>).
        /// </summary>
        /// <param name="iconHandle"></param>
        public TaskDialogIconHandle(IntPtr iconHandle)
        {
            IconHandle = iconHandle;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskDialogIconHandle"/> class
        /// from the specified icon.
        /// </summary>
        /// <param name="icon"></param>
        public TaskDialogIconHandle(Icon? icon)
            : this(icon?.Handle ?? default)
        {
        }

        /// <summary>
        /// The icon handle (<c>HICON</c>) that is represented by this
        /// <see cref="TaskDialogIconHandle"/> instance.
        /// </summary>
        public IntPtr IconHandle
        {
            get;
        }
    }
}
