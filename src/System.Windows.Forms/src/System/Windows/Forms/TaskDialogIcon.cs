// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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
    public abstract class TaskDialogIcon
    {
        private static readonly IReadOnlyDictionary<TaskDialogStandardIcon, TaskDialogStandardIconContainer> s_standardIcons =
            new Dictionary<TaskDialogStandardIcon, TaskDialogStandardIconContainer>() {
                { TaskDialogStandardIcon.None, new TaskDialogStandardIconContainer(TaskDialogStandardIcon.None) },
                { TaskDialogStandardIcon.Information, new TaskDialogStandardIconContainer(TaskDialogStandardIcon.Information) },
                { TaskDialogStandardIcon.Warning, new TaskDialogStandardIconContainer(TaskDialogStandardIcon.Warning) },
                { TaskDialogStandardIcon.Error, new TaskDialogStandardIconContainer(TaskDialogStandardIcon.Error) },
                { TaskDialogStandardIcon.SecurityShield, new TaskDialogStandardIconContainer(TaskDialogStandardIcon.SecurityShield) },
                { TaskDialogStandardIcon.SecurityShieldBlueBar, new TaskDialogStandardIconContainer(TaskDialogStandardIcon.SecurityShieldBlueBar) },
                { TaskDialogStandardIcon.SecurityShieldGrayBar, new TaskDialogStandardIconContainer(TaskDialogStandardIcon.SecurityShieldGrayBar) },
                { TaskDialogStandardIcon.SecurityWarningYellowBar, new TaskDialogStandardIconContainer(TaskDialogStandardIcon.SecurityWarningYellowBar) },
                { TaskDialogStandardIcon.SecurityErrorRedBar, new TaskDialogStandardIconContainer(TaskDialogStandardIcon.SecurityErrorRedBar) },
                { TaskDialogStandardIcon.SecuritySuccessGreenBar, new TaskDialogStandardIconContainer(TaskDialogStandardIcon.SecuritySuccessGreenBar) },
            };

        private protected TaskDialogIcon()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="icon"></param>
        public static implicit operator TaskDialogIcon(TaskDialogStandardIcon icon)
        {
            if (!s_standardIcons.TryGetValue(icon, out TaskDialogStandardIconContainer result))
            {
                throw new InvalidCastException(); // TODO: Is this the correct exception type?
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="icon"></param>
        public static implicit operator TaskDialogIcon(Icon icon)
        {
            return new TaskDialogIconHandle(icon);
        }

        /// <summary>
        /// Returns an <see cref="TaskDialogIcon"/> instance that reprents the specified
        /// <see cref="TaskDialogStandardIcon"/>.
        /// </summary>
        /// <param name="icon"></param>
        /// <returns></returns>
        public static TaskDialogIcon Get(TaskDialogStandardIcon icon)
        {
            if (!s_standardIcons.TryGetValue(icon, out TaskDialogStandardIconContainer result))
            {
                throw new ArgumentOutOfRangeException(nameof(icon));
            }

            return result;
        }
    }
}
