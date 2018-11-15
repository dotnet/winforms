﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms.Automation {
    
    /// <summary>
    /// Indicates the type of notification when raising the UIA Notification event.
    /// </summary>
    public enum AutomationNotificationKind {

        /// <summary>
        /// The current element container has had something added to it that should be presented to the user.
        /// </summary>
        ItemAdded = 0,

        /// <summary>
        /// The current element has had something removed from inside it that should be presented to the user.
        /// </summary>
        ItemRemoved = 1,

        /// <summary>
        /// The current element has a notification that an action was completed.
        /// </summary>
        ActionCompleted = 2,

        /// <summary>
        /// The current element has a notification that an action was abandoned.
        /// </summary>
        ActionAborted = 3,

        /// <summary>
        /// The current element has a notification not an add, remove, completed, or aborted action.
        /// </summary>
        Other = 4
    }
}