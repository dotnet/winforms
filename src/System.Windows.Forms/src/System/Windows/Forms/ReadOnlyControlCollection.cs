// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

namespace System.Windows.Forms
{
    /// <summary>
    ///  This is a ControlCollection which can be made readonly. In readonly mode, this
    ///  ControlCollection throws NotSupportedExceptions for any operation that attempts
    ///  to modify the collection.
    /// </summary>
    internal class ReadOnlyControlCollection : Control.ControlCollection
    {
        private readonly bool _isReadOnly;

        public ReadOnlyControlCollection(Control owner, bool isReadOnly) : base(owner)
        {
            _isReadOnly = isReadOnly;
        }

        public override void Add(Control value)
        {
            if (IsReadOnly)
            {
                throw new NotSupportedException(SR.ReadonlyControlsCollection);
            }

            AddInternal(value);
        }

        internal virtual void AddInternal(Control value) => base.Add(value);

        public override void Clear()
        {
            if (IsReadOnly)
            {
                throw new NotSupportedException(SR.ReadonlyControlsCollection);
            }

            base.Clear();
        }

        internal virtual void RemoveInternal(Control value) => base.Remove(value);

        public override void RemoveByKey(string key)
        {
            if (IsReadOnly)
            {
                throw new NotSupportedException(SR.ReadonlyControlsCollection);
            }

            base.RemoveByKey(key);
        }

        public override bool IsReadOnly => _isReadOnly;
    }
}
