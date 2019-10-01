// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable

using System.Collections.Generic;
using System.Drawing;

namespace System.Windows.Forms
{
    /// <summary>
    /// Represents an icon that can be shown in the main area of a task dialog
    /// (by setting the <see cref="TaskDialogPage.Icon"/> property) or in the
    /// footer of a task dialog (by setting the <see cref="TaskDialogFooter.Icon"/>
    /// property).
    /// </summary>
    public class TaskDialogIcon
    {
#pragma warning disable IDE1006 // Naming Styles
        public static readonly TaskDialogIcon None = new TaskDialogIcon(TaskDialogStandardIcon.None);
        public static readonly TaskDialogIcon Information = new TaskDialogIcon(TaskDialogStandardIcon.Information);
        public static readonly TaskDialogIcon Warning = new TaskDialogIcon(TaskDialogStandardIcon.Warning);
        public static readonly TaskDialogIcon Error = new TaskDialogIcon(TaskDialogStandardIcon.Error);
        public static readonly TaskDialogIcon SecurityShield = new TaskDialogIcon(TaskDialogStandardIcon.SecurityShield);
        public static readonly TaskDialogIcon SecurityShieldBlueBar = new TaskDialogIcon(TaskDialogStandardIcon.SecurityShieldBlueBar);
        public static readonly TaskDialogIcon SecurityShieldGrayBar = new TaskDialogIcon(TaskDialogStandardIcon.SecurityShieldGrayBar);
        public static readonly TaskDialogIcon SecurityWarningYellowBar = new TaskDialogIcon(TaskDialogStandardIcon.SecurityWarningYellowBar);
        public static readonly TaskDialogIcon SecurityErrorRedBar = new TaskDialogIcon(TaskDialogStandardIcon.SecurityErrorRedBar);
        public static readonly TaskDialogIcon SecuritySuccessGreenBar = new TaskDialogIcon(TaskDialogStandardIcon.SecuritySuccessGreenBar);
#pragma warning restore IDE1006 // Naming Styles

        private readonly TaskDialogStandardIcon? _standardIcon;

        private readonly IntPtr? _iconHandle;
        

        public TaskDialogIcon(Icon? icon)
            : this(icon?.Handle ?? default)
        {
        }

        public TaskDialogIcon(IntPtr iconHandle)
        {
            _iconHandle = iconHandle;
        }

        private TaskDialogIcon(TaskDialogStandardIcon standardIcon)
        {
            _standardIcon = standardIcon;
        }

        /// <summary>
        /// The icon handle (<c>HICON</c>) that is represented by this
        /// <see cref="TaskDialogIcon"/> instance.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// This <see cref="TaskDialogIcon"/> instance was not created using a
        /// constructor that takes an icon or icon handle.
        /// </exception>
        public IntPtr IconHandle => _iconHandle ?? throw new InvalidOperationException();

        internal TaskDialogStandardIcon StandardIcon => _standardIcon ?? throw new InvalidOperationException();

        internal bool IsStandardIcon => _standardIcon != null;

        internal bool IsHandleIcon => _iconHandle != null;
    }
}
