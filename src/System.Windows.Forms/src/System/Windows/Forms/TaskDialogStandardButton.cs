// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class TaskDialogStandardButton : TaskDialogButton
    {
        private TaskDialogResult _result;

        private bool _visible = true;

        /// <summary>
        /// 
        /// </summary>
        public TaskDialogStandardButton()
            // Use 'OK' by default instead of 'None' (which would not be a valid
            // standard button).
            : this(TaskDialogResult.OK)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        public TaskDialogStandardButton(TaskDialogResult result)
        {
            if (!IsValidStandardButtonResult(result))
            {
                throw new ArgumentOutOfRangeException(nameof(result));
            }

            _result = result;
        }

        /// <summary>
        /// Gets or sets the <see cref="TaskDialogResult"/> which is represented by
        /// this <see cref="TaskDialogStandardButton"/>.
        /// </summary>
        public TaskDialogResult Result
        {
            get => _result;

            set
            {
                if (!IsValidStandardButtonResult(value))
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }

                DenyIfBound();

                // If we are part of a StandardButtonCollection, we must now notify it
                // that we changed our result.
                Collection?.HandleKeyChange(this, value);

                // If this was successful or we are not part of a collection,
                // we can now set the result.
                _result = value;
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates if this
        /// <see cref="TaskDialogStandardButton"/> should be shown when displaying
        /// the Task Dialog.
        /// </summary>
        /// <remarks>
        /// Setting this to <c>false</c> allows you to still receive the
        /// <see cref="TaskDialogButton.Click"/> event (e.g. for the
        /// <see cref="TaskDialogResult.Cancel"/> button when
        /// <see cref="TaskDialogPage.AllowCancel"/> is set), or to call the
        /// <see cref="TaskDialogButton.PerformClick"/> method even if the button
        /// is not shown.
        /// </remarks>
        public bool Visible
        {
            get => _visible;

            set
            {
                DenyIfBound();

                _visible = value;
            }
        }

        internal override bool IsCreatable
        {
            get => base.IsCreatable && _visible;
        }

        internal override int ButtonID
        {
            get => (int)_result;
        }

        internal new TaskDialogStandardButtonCollection Collection
        {
            get => (TaskDialogStandardButtonCollection)base.Collection;
            set => base.Collection = value;
        }

        private static TaskDialogButtons GetButtonFlagForResult(TaskDialogResult result)
        {
            switch (result)
            {
                case TaskDialogResult.OK:
                    return TaskDialogButtons.OK;
                case TaskDialogResult.Cancel:
                    return TaskDialogButtons.Cancel;
                case TaskDialogResult.Abort:
                    return TaskDialogButtons.Abort;
                case TaskDialogResult.Retry:
                    return TaskDialogButtons.Retry;
                case TaskDialogResult.Ignore:
                    return TaskDialogButtons.Ignore;
                case TaskDialogResult.Yes:
                    return TaskDialogButtons.Yes;
                case TaskDialogResult.No:
                    return TaskDialogButtons.No;
                case TaskDialogResult.Close:
                    return TaskDialogButtons.Close;
                case TaskDialogResult.Help:
                    return TaskDialogButtons.Help;
                case TaskDialogResult.TryAgain:
                    return TaskDialogButtons.TryAgain;
                case TaskDialogResult.Continue:
                    return TaskDialogButtons.Continue;
                default:
                    return default;
            }
        }

        private static bool IsValidStandardButtonResult(TaskDialogResult result)
        {
            return GetButtonFlagForResult(result) != default;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return _result.ToString();
        }

        internal TaskDialogButtons GetButtonFlag()
        {
            return GetButtonFlagForResult(_result);
        }
    }
}
