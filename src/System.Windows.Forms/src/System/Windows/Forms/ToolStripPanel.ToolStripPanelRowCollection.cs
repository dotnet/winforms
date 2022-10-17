﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Collections;
using System.ComponentModel;
#if DEBUG
using System.Diagnostics;
#endif
using System.Windows.Forms.Layout;

namespace System.Windows.Forms
{
    public partial class ToolStripPanel
    {
        [ListBindable(false)]
        public class ToolStripPanelRowCollection : ArrangedElementCollection, IList
        {
            private readonly ToolStripPanel _owner;

            public ToolStripPanelRowCollection(ToolStripPanel owner)
            {
                _owner = owner;
            }

            public ToolStripPanelRowCollection(ToolStripPanel owner, ToolStripPanelRow[] value)
            {
                _owner = owner;
                AddRange(value);
            }

            /// <summary>
            ///
            /// </summary>
            public new virtual ToolStripPanelRow this[int index]
            {
                get
                {
                    return (ToolStripPanelRow)(InnerList[index]);
                }
            }

            public int Add(ToolStripPanelRow value)
            {
                ArgumentNullException.ThrowIfNull(value);

                int retVal = InnerList.Add(value);
                OnAdd(value, retVal);
                return retVal;
            }

            public void AddRange(ToolStripPanelRow[] value)
            {
                ArgumentNullException.ThrowIfNull(value);

                ToolStripPanel currentOwner = _owner;
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

            public void AddRange(ToolStripPanelRowCollection value)
            {
                ArgumentNullException.ThrowIfNull(value);

                ToolStripPanel currentOwner = _owner;
                currentOwner?.SuspendLayout();

                try
                {
                    int currentCount = value.Count;
                    for (int i = 0; i < currentCount; i++)
                    {
                        Add(value[i]);
                    }
                }
                finally
                {
                    currentOwner?.ResumeLayout();
                }
            }

            public bool Contains(ToolStripPanelRow value)
            {
                return InnerList.Contains(value);
            }

            public virtual void Clear()
            {
                _owner?.SuspendLayout();

                try
                {
                    while (Count != 0)
                    {
                        RemoveAt(Count - 1);
                    }
                }
                finally
                {
                    _owner?.ResumeLayout();
                }
            }

            void IList.Clear() { Clear(); }
            bool IList.IsFixedSize { get { return InnerList.IsFixedSize; } }
            bool IList.Contains(object value) { return InnerList.Contains(value); }
            bool IList.IsReadOnly { get { return InnerList.IsReadOnly; } }
            void IList.RemoveAt(int index) { RemoveAt(index); }
            void IList.Remove(object value) { Remove(value as ToolStripPanelRow); }
            int IList.Add(object value) { return Add(value as ToolStripPanelRow); }
            int IList.IndexOf(object value) { return IndexOf(value as ToolStripPanelRow); }
            void IList.Insert(int index, object value) { Insert(index, value as ToolStripPanelRow); }

            object IList.this[int index]
            {
                get { return InnerList[index]; }
                set { throw new NotSupportedException(SR.ToolStripCollectionMustInsertAndRemove); /* InnerList[index] = value; */ }
            }

            public int IndexOf(ToolStripPanelRow value)
            {
                return InnerList.IndexOf(value);
            }

            public void Insert(int index, ToolStripPanelRow value)
            {
                ArgumentNullException.ThrowIfNull(value);

                InnerList.Insert(index, value);
                OnAdd(value, index);
            }

            private void OnAdd(ToolStripPanelRow value, int index)
            {
                if (_owner is not null)
                {
                    LayoutTransaction.DoLayout(_owner, value, PropertyNames.Parent);
                }
            }

            /// <summary>
            ///  Do proper cleanup of ownership, etc.
            /// </summary>
            private static void OnAfterRemove(ToolStripPanelRow row)
            {
#if DEBUG
                if (s_toolStripPanelMissingRowDebug.TraceVerbose)
                {
                    if (row is not null)
                    {
                        Debug.Write("Removing row: ");
                        row.Debug_PrintRowID();
                        Debug.WriteLine(new StackTrace().ToString());
                    }
                }
#endif

            }

            public void Remove(ToolStripPanelRow value)
            {
                InnerList.Remove(value);
                OnAfterRemove(value);
            }

            public void RemoveAt(int index)
            {
                ToolStripPanelRow item = null;
                if (index < Count && index >= 0)
                {
                    item = (ToolStripPanelRow)(InnerList[index]);
                }

                InnerList.RemoveAt(index);
                OnAfterRemove(item);
            }

            public void CopyTo(ToolStripPanelRow[] array, int index)
            {
                InnerList.CopyTo(array, index);
            }
        }
    }
}
