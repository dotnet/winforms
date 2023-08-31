// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable disable

using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;

namespace System.Windows.Forms.Design;

public partial class ControlDesigner
{
    [ListBindable(false)]
    [DesignerSerializer(typeof(DesignerControlCollectionCodeDomSerializer), typeof(CodeDomSerializer))]
    internal class DesignerControlCollection : Control.ControlCollection, IList
    {
        private readonly Control.ControlCollection _realCollection;

        public DesignerControlCollection(Control owner) : base(owner) => _realCollection = owner.Controls;

        public override int Count => _realCollection.Count;

        object ICollection.SyncRoot => this;

        bool ICollection.IsSynchronized => false;

        bool IList.IsFixedSize => false;

        public new bool IsReadOnly => _realCollection.IsReadOnly;

        int IList.Add(object control) => ((IList)_realCollection).Add(control);

        public override void Add(Control c) => _realCollection.Add(c);

        public override void AddRange(params Control[] controls) => _realCollection.AddRange(controls);

        bool IList.Contains(object control) => ((IList)_realCollection).Contains(control);

        public new void CopyTo(Array dest, int index) => _realCollection.CopyTo(dest, index);

        public override bool Equals(object other) => _realCollection.Equals(other);

        public new IEnumerator GetEnumerator() => _realCollection.GetEnumerator();

        public override int GetHashCode() => _realCollection.GetHashCode();

        int IList.IndexOf(object control) => ((IList)_realCollection).IndexOf(control);

        void IList.Insert(int index, object value) => ((IList)_realCollection).Insert(index, value);

        void IList.Remove(object control) => ((IList)_realCollection).Remove(control);

        void IList.RemoveAt(int index) => ((IList)_realCollection).RemoveAt(index);

        object IList.this[int index]
        {
            get => ((IList)_realCollection)[index];
            set => throw new NotSupportedException();
        }

        public override int GetChildIndex(Control child, bool throwException)
            => _realCollection.GetChildIndex(child, throwException);

        public override void SetChildIndex(Control child, int newIndex)
            => _realCollection.SetChildIndex(child, newIndex);

        public override void Clear()
        {
            for (int i = _realCollection.Count - 1; i >= 0; i--)
            {
                if (_realCollection[i] is not null &&
                    _realCollection[i].Site is not null &&
                    TypeDescriptor.GetAttributes(_realCollection[i]).Contains(InheritanceAttribute.NotInherited))
                {
                    _realCollection.RemoveAt(i);
                }
            }
        }
    }
}
