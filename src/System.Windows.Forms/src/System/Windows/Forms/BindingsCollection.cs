// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Collections;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Represents a collection of data bindings on a control.
    /// </summary>
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

        /// <summary>
        ///  Gets the bindings in the collection as an object.
        /// </summary>
        protected override ArrayList List => _list ?? (_list = new ArrayList());

        /// <summary>
        ///  Gets the <see cref='Binding'/> at the specified index.
        /// </summary>
        public Binding this[int index] => (Binding)List[index];

        internal protected void Add(Binding binding)
        {
            var eventArgs = new CollectionChangeEventArgs(CollectionChangeAction.Add, binding);
            OnCollectionChanging(eventArgs);
            AddCore(binding);
            OnCollectionChanged(eventArgs);
        }

        /// <summary>
        ///  Adds a <see cref='Binding'/> to the collection.
        /// </summary>
        protected virtual void AddCore(Binding dataBinding)
        {
            if (dataBinding == null)
            {
                throw new ArgumentNullException(nameof(dataBinding));
            }

            List.Add(dataBinding);
        }

        /// <summary>
        ///  Occurs when the collection is about to change.
        /// </summary>
        [SRDescription(nameof(SR.collectionChangingEventDescr))]
        public event CollectionChangeEventHandler CollectionChanging
        {
            add => _onCollectionChanging += value;
            remove => _onCollectionChanging -= value;
        }

        /// <summary>
        ///  Occurs when the collection is changed.
        /// </summary>
        [SRDescription(nameof(SR.collectionChangedEventDescr))]
        public event CollectionChangeEventHandler CollectionChanged
        {
            add => _onCollectionChanged += value;
            remove => _onCollectionChanged -= value;
        }

        internal protected void Clear()
        {
            var eventArgs = new CollectionChangeEventArgs(CollectionChangeAction.Refresh, null);
            OnCollectionChanging(eventArgs);
            ClearCore();
            OnCollectionChanged(eventArgs);
        }

        /// <summary>
        ///  Clears the collection of any members.
        /// </summary>
        protected virtual void ClearCore() => List.Clear();

        /// <summary>
        ///  Raises the <see cref='CollectionChanging'/> event.
        /// </summary>
        protected virtual void OnCollectionChanging(CollectionChangeEventArgs e)
        {
            _onCollectionChanging?.Invoke(this, e);
        }

        /// <summary>
        ///  Raises the <see cref='CollectionChanged'/> event.
        /// </summary>
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

        /// <summary>
        ///  Removes the specified <see cref='Binding'/> from the collection.
        /// </summary>
        protected virtual void RemoveCore(Binding dataBinding) => List.Remove(dataBinding);

        internal protected bool ShouldSerializeMyAll() => Count > 0;
    }
}
