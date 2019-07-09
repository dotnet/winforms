// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using TaskDialogFlags = Interop.TaskDialog.TASKDIALOG_FLAGS;

namespace System.Windows.Forms
{
    /// <summary>
    /// Represents a control of a task dialog.
    /// </summary>
    public abstract class TaskDialogControl
    {
        // Disallow inheritance by specifying a private protected constructor.
        private protected TaskDialogControl()
        {
        }

        /// <summary>
        /// Gets or sets the object that contains data about the control.
        /// </summary>
        /// <value>
        /// An <see cref="object"/> that contains data about the control.
        /// The default is <c>null</c>.
        /// </value>
        public object Tag
        {
            get;
            set;
        }

        internal TaskDialogPage BoundPage
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a value that indicates if the current state of this control
        /// allows it to be created in a task dialog when binding it.
        /// </summary>
        internal virtual bool IsCreatable
        {
            get => true;
        }

        /// <summary>
        /// Gets or sets a value that indicates if this control has been created
        /// in a bound task dialog.
        /// </summary>
        internal bool IsCreated
        {
            get;
            private set;
        }

        internal TaskDialogFlags Bind(TaskDialogPage page)
        {
            BoundPage = page ?? throw new ArgumentNullException(nameof(page));

            // Use the current value of IsCreatable to determine if the control is
            // created. This is important because IsCreatable can change while the
            // control is displayed (e.g. if it depends on the Text property).
            IsCreated = IsCreatable;

            return IsCreated ? BindCore() : default;
        }

        internal void Unbind()
        {
            if (IsCreated)
            {
                UnbindCore();
            }

            IsCreated = false;
            BoundPage = null;
        }

        /// <summary>
        /// Applies initialization after the task dialog is displayed or navigated.
        /// </summary>
        internal void ApplyInitialization()
        {
            // Only apply the initialization if the control is actually created.
            if (IsCreated)
            {
                ApplyInitializationCore();
            }
        }

        /// <summary>
        /// When overridden in a subclass, runs additional binding logic and returns
        /// flags to be specified before the task dialog is displayed or navigated.
        /// </summary>
        /// <remarks>
        /// This method will only be called if <see cref="IsCreatable"/> returns <see langword="true"/>.
        /// </remarks>
        /// <returns></returns>
        private protected virtual TaskDialogFlags BindCore()
        {
            return default;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// This method will only be called if <see cref="BindCore"/> was called.
        /// </remarks>
        private protected virtual void UnbindCore()
        {
        }

        /// <summary>
        /// When overridden in a subclass, applies initialization after the task dialog
        /// is displayed or navigated.
        /// </summary>
        /// <remarks>
        /// This method will only be called if <see cref="IsCreatable"/> returns <see langword="true"/>.
        /// </remarks>
        private protected virtual void ApplyInitializationCore()
        {
        }

        private protected void DenyIfBound()
        {
            BoundPage?.DenyIfBound();
        }

        private protected void DenyIfWaitingForInitialization()
        {
            BoundPage?.DenyIfWaitingForInitialization();
        }

        private protected void DenyIfNotBoundOrWaitingForInitialization()
        {
            DenyIfWaitingForInitialization();

            if (BoundPage == null)
            {
                throw new InvalidOperationException(SR.TaskDialogControlNotBound);
            }
        }

        private protected void DenyIfBoundAndNotCreated()
        {
            if (BoundPage != null && !IsCreated)
            {
                throw new InvalidOperationException(SR.TaskDialogControlNotCreated);
            }
        }
    }
}
