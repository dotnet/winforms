// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace System.Windows.Forms
{
    /// <summary>
    /// Represents a collection of <see cref="TaskDialogRadioButton"/> objects.
    /// </summary>
    public class TaskDialogRadioButtonCollection : Collection<TaskDialogRadioButton>
    {
        // HashSet to detect duplicate items.
        private readonly HashSet<TaskDialogRadioButton> _itemSet = new HashSet<TaskDialogRadioButton>();

        private TaskDialogPage _boundPage;

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskDialogRadioButtonCollection"/> class.
        /// </summary>
        public TaskDialogRadioButtonCollection()
        {
        }

        internal TaskDialogPage BoundPage
        {
            get => _boundPage;
            set => _boundPage = value;
        }

        /// <summary>
        /// Creates and adds a <see cref="TaskDialogRadioButton"/> to the collection.
        /// </summary>
        /// <param name="text">The text of the radio button.</param>
        /// <returns>The created <see cref="TaskDialogRadioButton"/>.</returns>
        public TaskDialogRadioButton Add(string text)
        {
            var button = new TaskDialogRadioButton()
            {
                Text = text
            };

            Add(button);
            return button;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        protected override void SetItem(int index, TaskDialogRadioButton item)
        {
            // Disallow collection modification, so that we don't need to copy it
            // when binding the TaskDialogPage.
            _boundPage?.DenyIfBound();
            DenyIfHasOtherCollection(item);

            TaskDialogRadioButton oldItem = this[index];
            if (oldItem != item)
            {
                // First, add the new item (which will throw if it is a duplicate entry),
                // then remove the old one.
                if (!_itemSet.Add(item))
                {
                    throw new ArgumentException();
                }

                _itemSet.Remove(oldItem);

                oldItem.Collection = null;
                item.Collection = this;
            }

            base.SetItem(index, item);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        protected override void InsertItem(int index, TaskDialogRadioButton item)
        {
            // Disallow collection modification, so that we don't need to copy it
            // when binding the TaskDialogPage.
            _boundPage?.DenyIfBound();
            DenyIfHasOtherCollection(item);

            if (!_itemSet.Add(item))
            {
                throw new ArgumentException();
            }

            item.Collection = this;
            base.InsertItem(index, item);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        protected override void RemoveItem(int index)
        {
            // Disallow collection modification, so that we don't need to copy it
            // when binding the TaskDialogPage.
            _boundPage?.DenyIfBound();

            TaskDialogRadioButton oldItem = this[index];
            oldItem.Collection = null;
            _itemSet.Remove(oldItem);
            base.RemoveItem(index);
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void ClearItems()
        {
            // Disallow collection modification, so that we don't need to copy it
            // when binding the TaskDialogPage.
            _boundPage?.DenyIfBound();

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
                throw new InvalidOperationException("This control is already part of a different collection.");
            }
        }
    }
}
