// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Collections;

namespace System.Windows.Forms
{
    /// <devdoc>
    /// Represents a collection of data bindings on a control.
    /// </devdoc>
    [DefaultEvent(nameof(CollectionChanged))]
    public class BindingsCollection : BaseCollection
    {
        private ArrayList _list;
        private CollectionChangeEventHandler _onCollectionChanging;
        private CollectionChangeEventHandler _onCollectionChanged;

        internal BindingsCollection()
        {
        }

        public override int Count => _list == null ? 0 : base.Count;

        /// <devdoc>
        /// Gets the bindings in the collection as an object.
        /// </devdoc>
        protected override ArrayList List => _list ?? (_list = new ArrayList());

        /// <devdoc>
        /// Gets the <see cref='System.Windows.Forms.Binding'/> at the specified index.
        /// </devdoc>
        public Binding this[int index] => (Binding)List[index];

        internal protected void Add(Binding binding)
        {
            var eventArgs = new CollectionChangeEventArgs(CollectionChangeAction.Add, binding);
            OnCollectionChanging(eventArgs);
            AddCore(binding);
            OnCollectionChanged(eventArgs);
        }

        /// <devdoc>
        /// Adds a <see cref='System.Windows.Forms.Binding'/> to the collection.
        /// </devdoc>
        protected virtual void AddCore(Binding dataBinding)
        {
            if (dataBinding == null)
            {
                throw new ArgumentNullException(nameof(dataBinding));
            }

            List.Add(dataBinding);
        }

        /// <devdoc>
        /// Occurs when the collection is about to change.
        /// </devdoc>
        [SRDescription(nameof(SR.collectionChangingEventDescr))]
        public event CollectionChangeEventHandler CollectionChanging
        {
            add
            {
                _onCollectionChanging += value;
            }
            remove
            {
                _onCollectionChanging -= value;
            }
        }

        /// <devdoc>
        /// Occurs when the collection is changed.
        /// </devdoc>
        [SRDescription(nameof(SR.collectionChangedEventDescr))]
        public event CollectionChangeEventHandler CollectionChanged
        {
            add
            {
                _onCollectionChanged += value;
            }
            remove
            {
                _onCollectionChanged -= value;
            }
        }

        internal protected void Clear()
        {
            var eventArgs = new CollectionChangeEventArgs(CollectionChangeAction.Refresh, null);
            OnCollectionChanging(eventArgs);
            ClearCore();
            OnCollectionChanged(eventArgs);
        }

        /// <devdoc>
        /// Clears the collection of any members.
        /// </devdoc>
        protected virtual void ClearCore() => List.Clear();

        /// <devdoc>
        /// Raises the <see cref='System.Windows.Forms.BindingsCollection.CollectionChanging'/> event.
        /// </devdoc>
        protected virtual void OnCollectionChanging(CollectionChangeEventArgs e)
        {
            _onCollectionChanging?.Invoke(this, e);
        }

        /// <devdoc>
        /// Raises the <see cref='System.Windows.Forms.BindingsCollection.CollectionChanged'/> event.
        /// </devdoc>
        protected virtual void OnCollectionChanged(CollectionChangeEventArgs ccevent)
        {
            _onCollectionChanged?.Invoke(this, ccevent);
        }

        internal protected void Remove(Binding binding)
        {
            var eventArgs = new CollectionChangeEventArgs(CollectionChangeAction.Remove, binding);
            OnCollectionChanging(eventArgs);
            RemoveCore(binding);
            OnCollectionChanged(eventArgs);
        }

        internal protected void RemoveAt(int index) => Remove(this[index]);

        /// <devdoc>
        /// Removes the specified <see cref='System.Windows.Forms.Binding'/> from the collection.
        /// </devdoc>
        protected virtual void RemoveCore(Binding dataBinding) => List.Remove(dataBinding);

        internal protected bool ShouldSerializeMyAll() => Count > 0;
    }
}
