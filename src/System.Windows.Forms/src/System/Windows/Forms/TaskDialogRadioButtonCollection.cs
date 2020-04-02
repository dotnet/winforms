// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace System.Windows.Forms
{
    /// <summary>
    ///   Represents a collection of <see cref="TaskDialogRadioButton"/> objects.
    /// </summary>
    public class TaskDialogRadioButtonCollection : Collection<TaskDialogRadioButton>
    {
        // HashSet to detect duplicate items.
        private readonly HashSet<TaskDialogRadioButton> _itemSet = new HashSet<TaskDialogRadioButton>();

        /// <summary>
        ///   Initializes a new instance of the <see cref="TaskDialogRadioButtonCollection"/> class.
        /// </summary>
        public TaskDialogRadioButtonCollection()
        {
        }

        internal TaskDialogPage? BoundPage
        {
            get;
            set;
        }

        /// <summary>
        ///   Creates and adds a <see cref="TaskDialogRadioButton"/> to the collection.
        /// </summary>
        /// <param name="text">The text of the radio button.</param>
        /// <returns>The created <see cref="TaskDialogRadioButton"/>.</returns>
        public TaskDialogRadioButton Add(string? text)
        {
            var button = new TaskDialogRadioButton()
            {
                Text = text
            };

            Add(button);
            return button;
        }

        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="item"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///   <paramref name="item"/> has already been added to the collection.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        ///   <paramref name="item"/> is already a part of a different collection.
        /// </exception>
        protected override void SetItem(int index, TaskDialogRadioButton item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            // Disallow collection modification, so that we don't need to copy it
            // when binding the TaskDialogPage.
            BoundPage?.DenyIfBound();
            DenyIfHasOtherCollection(item);

            TaskDialogRadioButton oldItem = this[index];
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
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="item"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///   <paramref name="item"/> has already been added to the collection.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        ///   <paramref name="item"/> is already a part of a different collection.
        /// </exception>
        protected override void InsertItem(int index, TaskDialogRadioButton item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

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

            TaskDialogRadioButton oldItem = this[index];
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

            foreach (TaskDialogRadioButton button in this)
            {
                button.Collection = null;
            }

            _itemSet.Clear();
            base.ClearItems();
        }

        private void DenyIfHasOtherCollection(TaskDialogRadioButton item)
        {
            if (item.Collection != null && item.Collection != this)
            {
                throw new InvalidOperationException(SR.TaskDialogControlIsPartOfOtherCollection);
            }
        }
    }
}
