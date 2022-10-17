﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms.Layout;

namespace System.Windows.Forms
{
    public partial class ToolStripPanelRow
    {
        /// <summary>
        ///  ToolStripPanelRowControlCollection
        ///
        ///  this class represents the collection of controls on a particular row.
        ///  when you add and remove controls from this collection - you also add and remove
        ///  controls to and from the ToolStripPanel.Control's collection (which happens
        ///  to be externally readonly.)
        ///
        ///  This class is used to represent the IArrangedElement.Children for the ToolStripPanelRow -
        ///  which means that this collection represents the IArrangedElements to layout for
        ///  a particular ToolStripPanelRow.
        ///
        ///  We need to keep copies of the controls in both the ToolStripPanelRowControlCollection and
        ///  the ToolStripPanel.Control collection  as the ToolStripPanel.Control collection
        ///  is responsible for parenting and unparenting the controls (ToolStripPanelRows do NOT derive from
        ///  Control and thus are NOT hwnd backed).
        /// </summary>
        internal class ToolStripPanelRowControlCollection : ArrangedElementCollection, IList, IEnumerable
        {
            private readonly ToolStripPanelRow _owner;
            private ArrangedElementCollection _cellCollection;

            public ToolStripPanelRowControlCollection(ToolStripPanelRow owner)
            {
                _owner = owner;
            }

            public ToolStripPanelRowControlCollection(ToolStripPanelRow owner, Control[] value)
            {
                _owner = owner;
                AddRange(value);
            }

            public new virtual Control this[int index]
            {
                get
                {
                    return GetControl(index);
                }
            }

            public ArrangedElementCollection Cells
            {
                get
                {
                    _cellCollection ??= new ArrangedElementCollection(InnerList);

                    return _cellCollection;
                }
            }

            public ToolStripPanel ToolStripPanel
            {
                get
                {
                    return _owner.ToolStripPanel;
                }
            }

            [EditorBrowsable(EditorBrowsableState.Never)]
            public int Add(Control value)
            {
                ArgumentNullException.ThrowIfNull(value);

                if (!(value is ISupportToolStripPanel control))
                {
                    throw new NotSupportedException(string.Format(SR.TypedControlCollectionShouldBeOfType, nameof(ToolStrip)));
                }

                int index = InnerList.Add(control.ToolStripPanelCell);

                OnAdd(control, index);
                return index;
            }

            [EditorBrowsable(EditorBrowsableState.Never)]
            public void AddRange(Control[] value)
            {
                ArgumentNullException.ThrowIfNull(value);

                ToolStripPanel currentOwner = ToolStripPanel;

                currentOwner?.SuspendLayout();

                try
                {
                    for (int i = 0; i < value.Length; i++)
                    {
                        Add(value[i]);
                    }
                }
                finally
                {
                    currentOwner?.ResumeLayout();
                }
            }

            public bool Contains(Control value)
            {
                for (int i = 0; i < Count; i++)
                {
                    if (GetControl(i) == value)
                    {
                        return true;
                    }
                }

                return false;
            }

            public virtual void Clear()
            {
                if (_owner is not null)
                {
                    ToolStripPanel.SuspendLayout();
                }

                try
                {
                    while (Count != 0)
                    {
                        RemoveAt(Count - 1);
                    }
                }
                finally
                {
                    if (_owner is not null)
                    {
                        ToolStripPanel.ResumeLayout();
                    }
                }
            }

            public override IEnumerator GetEnumerator() { return new ToolStripPanelCellToControlEnumerator(InnerList); }

            private Control GetControl(int index)
            {
                Control control = null;
                ToolStripPanelCell cell = null;

                if (index < Count && index >= 0)
                {
                    cell = (ToolStripPanelCell)(InnerList[index]);
                    control = cell?.Control;
                }

                return control;
            }

            private int IndexOfControl(Control c)
            {
                for (int i = 0; i < Count; i++)
                {
                    ToolStripPanelCell cell = (ToolStripPanelCell)(InnerList[i]);
                    if (cell.Control == c)
                    {
                        return i;
                    }
                }

                return -1;
            }

            void IList.Clear() { Clear(); }

            bool IList.IsFixedSize { get { return InnerList.IsFixedSize; } }

            bool IList.Contains(object value) { return InnerList.Contains(value); }

            bool IList.IsReadOnly { get { return InnerList.IsReadOnly; } }

            void IList.RemoveAt(int index) { RemoveAt(index); }

            void IList.Remove(object value) { Remove(value as Control); }

            int IList.Add(object value) { return Add(value as Control); }

            int IList.IndexOf(object value) { return IndexOf(value as Control); }

            void IList.Insert(int index, object value) { Insert(index, value as Control); }

            public int IndexOf(Control value)
            {
                for (int i = 0; i < Count; i++)
                {
                    if (GetControl(i) == value)
                    {
                        return i;
                    }
                }

                return -1;
            }

            [EditorBrowsable(EditorBrowsableState.Never)]
            public void Insert(int index, Control value)
            {
                ArgumentNullException.ThrowIfNull(value);

                if (!(value is ISupportToolStripPanel control))
                {
                    throw new NotSupportedException(string.Format(SR.TypedControlCollectionShouldBeOfType, nameof(ToolStrip)));
                }

                InnerList.Insert(index, control.ToolStripPanelCell);
                OnAdd(control, index);
            }

            /// <summary>
            ///  Do proper cleanup of ownership, etc.
            /// </summary>
            private void OnAfterRemove(Control control, int index)
            {
                if (_owner is not null)
                {
                    // unfortunately we don't know the index of the control in the ToolStripPanel's
                    // control collection, as all rows share this collection.
                    // To unparent this control we need to use Remove instead  of RemoveAt.
                    using (LayoutTransaction t = new LayoutTransaction(ToolStripPanel, control, PropertyNames.Parent))
                    {
                        _owner.ToolStripPanel.Controls.Remove(control);
                        _owner.OnControlRemoved(control, index);
                    }
                }
            }

            private void OnAdd(ISupportToolStripPanel controlToBeDragged, int index)
            {
                if (_owner is not null)
                {
                    LayoutTransaction layoutTransaction = null;
                    if (ToolStripPanel is not null && ToolStripPanel.ParentInternal is not null)
                    {
                        layoutTransaction = new LayoutTransaction(ToolStripPanel, ToolStripPanel.ParentInternal, PropertyNames.Parent);
                    }

                    try
                    {
                        if (controlToBeDragged is not null)
                        {
                            controlToBeDragged.ToolStripPanelRow = _owner;

                            if (controlToBeDragged is Control control)
                            {
                                control.ParentInternal = _owner.ToolStripPanel;
                                _owner.OnControlAdded(control, index);
                            }
                        }
                    }
                    finally
                    {
                        layoutTransaction?.Dispose();
                    }
                }
            }

            [EditorBrowsable(EditorBrowsableState.Never)]
            public void Remove(Control value)
            {
                int index = IndexOfControl(value);
                RemoveAt(index);
            }

            [EditorBrowsable(EditorBrowsableState.Never)]
            public void RemoveAt(int index)
            {
                if (index >= 0 && index < Count)
                {
                    Control control = GetControl(index);
                    ToolStripPanelCell cell = InnerList[index] as ToolStripPanelCell;
                    InnerList.RemoveAt(index);
                    OnAfterRemove(control, index);
                }
            }

            [EditorBrowsable(EditorBrowsableState.Never)]
            public void CopyTo(Control[] array, int index)
            {
                ArgumentNullException.ThrowIfNull(array);

                if (index < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(index));
                }

                if (index >= array.Length || InnerList.Count > array.Length - index)
                {
                    throw new ArgumentException(SR.ToolStripPanelRowControlCollectionIncorrectIndexLength);
                }

                for (int i = 0; i < InnerList.Count; i++)
                {
                    array[index++] = GetControl(i);
                }
            }

            ///  We want to pretend like we're only holding controls... so everywhere we've returned controls.
            ///  but the problem is if you do a foreach, you'll get the cells not the controls.  So we've got
            ///  to sort of write a wrapper class around the ArrayList enumerator.
            private class ToolStripPanelCellToControlEnumerator : IEnumerator, ICloneable
            {
                private readonly IEnumerator _arrayListEnumerator;

                internal ToolStripPanelCellToControlEnumerator(ArrayList list)
                {
                    _arrayListEnumerator = ((IEnumerable)list).GetEnumerator();
                }

                public virtual object Current
                {
                    get
                    {
                        ToolStripPanelCell cell = _arrayListEnumerator.Current as ToolStripPanelCell;
                        Debug.Assert(cell is not null, "Expected ToolStripPanel cells only!!!" + _arrayListEnumerator.Current.GetType().ToString());
                        return cell?.Control;
                    }
                }

                public object Clone()
                {
                    return MemberwiseClone();
                }

                public virtual bool MoveNext()
                {
                    return _arrayListEnumerator.MoveNext();
                }

                public virtual void Reset()
                {
                    _arrayListEnumerator.Reset();
                }
            }
        }
    }
}
