// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace System.Windows.Forms
{
    /// <summary>
    ///   Represents a collection of <see cref="TaskDialogButton"/> objects.
    /// </summary>
    public class TaskDialogButtonCollection : Collection<TaskDialogButton>
    {
        // HashSet to detect duplicate items.
        private readonly HashSet<TaskDialogButton> _itemSet = new HashSet<TaskDialogButton>();

        /// <summary>
        ///   Initializes a new instance of the <see cref="TaskDialogButtonCollection"/> class.
        /// </summary>
        public TaskDialogButtonCollection()
        {
        }

        internal TaskDialogPage? BoundPage
        {
            get;
            set;
        }

        /// <summary>
        ///   Creates and adds a <see cref="TaskDialogButton"/> to the collection.
        /// </summary>
        /// <param name="text">The text of the custom button.</param>
        /// <param name="enabled">A value indicating whether the button can respond to user interaction.</param>
        /// <param name="allowCloseDialog">A value that indicates whether the task dialog should close
        ///   when this button is clicked.
        /// </param>
        /// <returns>The created <see cref="TaskDialogButton"/>.</returns>
        public TaskDialogButton Add(string? text, bool enabled = true, bool allowCloseDialog = true)
        {
            var button = new TaskDialogButton(text, enabled, allowCloseDialog);
            Add(button);

            return button;
        }

        /// <inheritdoc/>
        protected override void SetItem(int index, TaskDialogButton item)
        {
            // Disallow collection modification, so that we don't need to copy it
            // when binding the TaskDialogPage.
            BoundPage?.DenyIfBound();
            DenyIfHasOtherCollection(item);

            TaskDialogButton oldItem = this[index];
            if (oldItem != item)
            {
                // First, add the new item (which will throw if it is a duplicate entry),
                // then remove the old one.
                if (!_itemSet.Add(item))
                {
                    throw new ArgumentException(SR.TaskDialogControlAlreadyAddedToCollection);
                }

                _itemSet.Remove(oldItem);

                oldItem.Collection = null;
                item.Collection = this;
            }

            base.SetItem(index, item);
        }

        /// <inheritdoc/>
        protected override void InsertItem(int index, TaskDialogButton item)
        {
            // Disallow collection modification, so that we don't need to copy it
            // when binding the TaskDialogPage.
            BoundPage?.DenyIfBound();
            DenyIfHasOtherCollection(item);

            if (!_itemSet.Add(item))
            {
                throw new ArgumentException(SR.TaskDialogControlAlreadyAddedToCollection);
            }

            item.Collection = this;
            base.InsertItem(index, item);
        }

        /// <inheritdoc/>
        protected override void RemoveItem(int index)
        {
            // Disallow collection modification, so that we don't need to copy it
            // when binding the TaskDialogPage.
            BoundPage?.DenyIfBound();

            TaskDialogButton oldItem = this[index];
            oldItem.Collection = null;
            _itemSet.Remove(oldItem);
            base.RemoveItem(index);
        }

        /// <inheritdoc/>
        protected override void ClearItems()
        {
            // Disallow collection modification, so that we don't need to copy it
            // when binding the TaskDialogPage.
            BoundPage?.DenyIfBound();

            foreach (TaskDialogButton button in this)
            {
                button.Collection = null;
            }

            _itemSet.Clear();
            base.ClearItems();
        }

        private void DenyIfHasOtherCollection(TaskDialogButton item)
        {
            if (item.Collection != null && item.Collection != this)
                throw new InvalidOperationException(SR.TaskDialogControlIsPartOfOtherCollection);
        }
    }
}
