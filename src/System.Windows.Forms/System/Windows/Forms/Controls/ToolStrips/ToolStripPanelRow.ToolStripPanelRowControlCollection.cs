// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.ComponentModel;
using System.Windows.Forms.Layout;

namespace System.Windows.Forms;

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
    internal partial class ToolStripPanelRowControlCollection : ArrangedElementCollection, IList, IEnumerable
    {
        private readonly ToolStripPanelRow _owner;
        private ArrangedElementCollection? _cellCollection;

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

            if (value is not ISupportToolStripPanel control)
            {
                throw new NotSupportedException(string.Format(SR.TypedControlCollectionShouldBeOfType, nameof(ToolStrip)));
            }

            int index = ((IList)InnerList).Add(control.ToolStripPanelCell);

            OnAdd(control, index);
            return index;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void AddRange(params Control[] value)
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

        public override IEnumerator GetEnumerator() { return new ToolStripPanelCellToControlEnumerator(InnerList.GetEnumerator()); }

        private Control GetControl(int index)
        {
            if (index < Count && index >= 0)
            {
                var cell = (ToolStripPanelCell)(InnerList[index]);
                return cell.Control;
            }

            throw new IndexOutOfRangeException();
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

        bool IList.IsFixedSize { get { return ((IList)InnerList).IsFixedSize; } }

        bool IList.Contains(object? value) { return InnerList.Contains(value); }

        bool IList.IsReadOnly { get { return ((IList)InnerList).IsReadOnly; } }

        void IList.RemoveAt(int index) { RemoveAt(index); }

        void IList.Remove(object? value) { Remove((Control)value!); }

        int IList.Add(object? value) { return Add((Control)value!); }

        int IList.IndexOf(object? value) { return IndexOf((Control)value!); }

        void IList.Insert(int index, object? value) { Insert(index, (Control)value!); }

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

            if (value is not ISupportToolStripPanel control)
            {
                throw new NotSupportedException(string.Format(SR.TypedControlCollectionShouldBeOfType, nameof(ToolStrip)));
            }

            InnerList.Insert(index, control.ToolStripPanelCell!);
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
                using LayoutTransaction t = new(ToolStripPanel, control, PropertyNames.Parent);
                _owner.ToolStripPanel.Controls.Remove(control);
                _owner.OnControlRemoved(control, index);
            }
        }

        private void OnAdd(ISupportToolStripPanel controlToBeDragged, int index)
        {
            if (_owner is not null)
            {
                LayoutTransaction? layoutTransaction = null;
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
                InnerList.RemoveAt(index);
                OnAfterRemove(control, index);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void CopyTo(Control[] array, int index)
        {
            ArgumentNullException.ThrowIfNull(array);

            ArgumentOutOfRangeException.ThrowIfNegative(index);

            if (index >= array.Length || InnerList.Count > array.Length - index)
            {
                throw new ArgumentException(SR.ToolStripPanelRowControlCollectionIncorrectIndexLength);
            }

            for (int i = 0; i < InnerList.Count; i++)
            {
                array[index++] = GetControl(i);
            }
        }
    }
}
