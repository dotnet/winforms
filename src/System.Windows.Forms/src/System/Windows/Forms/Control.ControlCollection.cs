// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Forms.Layout;

namespace System.Windows.Forms
{
    public partial class Control
    {
        /// <summary>
        ///  Collection of controls...
        /// </summary>
        [ListBindable(false)]
        public class ControlCollection : ArrangedElementCollection, IList, ICloneable
        {
            ///  A caching mechanism for key accessor
            ///  We use an index here rather than control so that we don't have lifetime
            ///  issues by holding on to extra references.
            ///  Note this is not Thread Safe - but WinForms has to be run in a STA anyways.
            private int _lastAccessedIndex = -1;

            public ControlCollection(Control owner)
            {
                Owner = owner ?? throw new ArgumentNullException(nameof(owner));
            }

            /// <summary>
            ///  Returns true if the collection contains an item with the specified key, false otherwise.
            /// </summary>
            public virtual bool ContainsKey(string key)
            {
                return IsValidIndex(IndexOfKey(key));
            }

            /// <summary>
            ///  Adds a child control to this control. The control becomes the last control in
            ///  the child control list. If the control is already a child of another control it
            ///  is first removed from that control.
            /// </summary>
            public virtual void Add(Control value)
            {
                if (value is null)
                {
                    return;
                }

                if (value.GetTopLevel())
                {
                    throw new ArgumentException(SR.TopLevelControlAdd);
                }

                // Verify that the control being added is on the same thread as
                // us...or our parent chain.
                if (Owner.CreateThreadId != value.CreateThreadId)
                {
                    throw new ArgumentException(SR.AddDifferentThreads);
                }

                CheckParentingCycle(Owner, value);

                if (value._parent == Owner)
                {
                    value.SendToBack();
                    return;
                }

                // Remove the new control from its old parent (if any)
                if (value._parent != null)
                {
                    value._parent.Controls.Remove(value);
                }

                // Add the control
                InnerList.Add(value);

                if (value._tabIndex == -1)
                {
                    // Find the next highest tab index
                    int nextTabIndex = 0;
                    for (int c = 0; c < (Count - 1); c++)
                    {
                        int t = this[c].TabIndex;
                        if (nextTabIndex <= t)
                        {
                            nextTabIndex = t + 1;
                        }
                    }
                    value._tabIndex = nextTabIndex;
                }

                // if we don't suspend layout, AssignParent will indirectly trigger a layout event
                // before we're ready (AssignParent will fire a PropertyChangedEvent("Visible"), which calls PerformLayout)
#if DEBUG
                int dbgLayoutCheck = Owner.LayoutSuspendCount;
#endif
                Owner.SuspendLayout();

                try
                {
                    Control oldParent = value._parent;
                    try
                    {
                        // AssignParent calls into user code - this could throw, which
                        // would make us short-circuit the rest of the reparenting logic.
                        // you could end up with a control half reparented.
                        value.AssignParent(Owner);
                    }
                    finally
                    {
                        if (oldParent != value._parent && (Owner._state & States.Created) != 0)
                        {
                            value.SetParentHandle(Owner.InternalHandle);
                            if (value.Visible)
                            {
                                value.CreateControl();
                            }
                        }
                    }

                    value.InitLayout();
                }
                finally
                {
                    Owner.ResumeLayout(false);
#if DEBUG
                    Owner.AssertLayoutSuspendCount(dbgLayoutCheck);
#endif
                }

                // Not putting in the finally block, as it would eat the original
                // exception thrown from AssignParent if the following throws an exception.
                LayoutTransaction.DoLayout(Owner, value, PropertyNames.Parent);
                Owner.OnControlAdded(new ControlEventArgs(value));
            }

            int IList.Add(object control)
            {
                if (control is Control)
                {
                    Add((Control)control);
                    return IndexOf((Control)control);
                }
                else
                {
                    throw new ArgumentException(SR.ControlBadControl, nameof(control));
                }
            }

            [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
            public virtual void AddRange(Control[] controls)
            {
                if (controls is null)
                {
                    throw new ArgumentNullException(nameof(controls));
                }
                if (controls.Length > 0)
                {
#if DEBUG
                    int dbgLayoutCheck = Owner.LayoutSuspendCount;
#endif
                    Owner.SuspendLayout();
                    try
                    {
                        for (int i = 0; i < controls.Length; ++i)
                        {
                            Add(controls[i]);
                        }
                    }
                    finally
                    {
                        Owner.ResumeLayout(true);
                    }
#if DEBUG
                    Owner.AssertLayoutSuspendCount(dbgLayoutCheck);
#endif
                }
            }

            object ICloneable.Clone()
            {
                // Use CreateControlInstance so we get the same type of ControlCollection, but whack the
                // owner so adding controls to this new collection does not affect the control we cloned from.
                ControlCollection ccOther = Owner.CreateControlsInstance();

                // We add using InnerList to prevent unnecessary parent cycle checks, etc.
                ccOther.InnerList.AddRange(this);
                return ccOther;
            }

            public bool Contains(Control control)
            {
                return InnerList.Contains(control);
            }

            /// <summary>
            ///  Searches for Controls by their Name property, builds up an array
            ///  of all the controls that match.
            /// </summary>
            public Control[] Find(string key, bool searchAllChildren)
            {
                if (string.IsNullOrEmpty(key))
                {
                    throw new ArgumentNullException(nameof(key), SR.FindKeyMayNotBeEmptyOrNull);
                }

                ArrayList foundControls = FindInternal(key, searchAllChildren, this, new ArrayList());

                // Make this a stongly typed collection.
                Control[] stronglyTypedFoundControls = new Control[foundControls.Count];
                foundControls.CopyTo(stronglyTypedFoundControls, 0);

                return stronglyTypedFoundControls;
            }

            /// <summary>
            ///  Searches for Controls by their Name property, builds up an array list
            ///  of all the controls that match.
            /// </summary>
            private ArrayList FindInternal(string key, bool searchAllChildren, ControlCollection controlsToLookIn, ArrayList foundControls)
            {
                if ((controlsToLookIn is null) || (foundControls is null))
                {
                    return null;
                }

                try
                {
                    // Perform breadth first search - as it's likely people will want controls belonging
                    // to the same parent close to each other.
                    for (int i = 0; i < controlsToLookIn.Count; i++)
                    {
                        if (controlsToLookIn[i] is null)
                        {
                            continue;
                        }

                        if (WindowsFormsUtils.SafeCompareStrings(controlsToLookIn[i].Name, key, /* ignoreCase = */ true))
                        {
                            foundControls.Add(controlsToLookIn[i]);
                        }
                    }

                    // Optional recurive search for controls in child collections.

                    if (searchAllChildren)
                    {
                        for (int i = 0; i < controlsToLookIn.Count; i++)
                        {
                            if (controlsToLookIn[i] is null)
                            {
                                continue;
                            }
                            if ((controlsToLookIn[i].Controls != null) && controlsToLookIn[i].Controls.Count > 0)
                            {
                                // if it has a valid child collecion, append those results to our collection
                                foundControls = FindInternal(key, searchAllChildren, controlsToLookIn[i].Controls, foundControls);
                            }
                        }
                    }
                }
                catch (Exception e) when (!ClientUtils.IsCriticalException(e))
                {
                }
                return foundControls;
            }

            public override IEnumerator GetEnumerator()
            {
                return new ControlCollectionEnumerator(this);
            }

            public int IndexOf(Control control)
            {
                return InnerList.IndexOf(control);
            }

            /// <summary>
            ///  The zero-based index of the first occurrence of value within the entire CollectionBase, if found; otherwise, -1.
            /// </summary>
            public virtual int IndexOfKey(string key)
            {
                // Step 0 - Arg validation
                if (string.IsNullOrEmpty(key))
                {
                    return -1; // we dont support empty or null keys.
                }

                // step 1 - check the last cached item
                if (IsValidIndex(_lastAccessedIndex))
                {
                    if (WindowsFormsUtils.SafeCompareStrings(this[_lastAccessedIndex].Name, key, /* ignoreCase = */ true))
                    {
                        return _lastAccessedIndex;
                    }
                }

                // step 2 - search for the item
                for (int i = 0; i < Count; i++)
                {
                    if (WindowsFormsUtils.SafeCompareStrings(this[i].Name, key, /* ignoreCase = */ true))
                    {
                        _lastAccessedIndex = i;
                        return i;
                    }
                }

                // step 3 - we didn't find it.  Invalidate the last accessed index and return -1.
                _lastAccessedIndex = -1;
                return -1;
            }

            /// <summary>
            ///  Determines if the index is valid for the collection.
            /// </summary>
            private bool IsValidIndex(int index)
            {
                return ((index >= 0) && (index < Count));
            }

            /// <summary>
            ///  Who owns this control collection.
            /// </summary>
            public Control Owner { get; }

            /// <summary>
            ///  Removes control from this control. Inheriting controls should call
            ///  base.remove to ensure that the control is removed.
            /// </summary>
            public virtual void Remove(Control value)
            {
                // Sanity check parameter
                if (value is null)
                {
                    return;     // Don't do anything
                }

                if (value.ParentInternal == Owner)
                {
                    value.SetParentHandle(IntPtr.Zero);

                    // Remove the control from the internal control array
                    InnerList.Remove(value);
                    value.AssignParent(null);
                    LayoutTransaction.DoLayout(Owner, value, PropertyNames.Parent);
                    Owner.OnControlRemoved(new ControlEventArgs(value));

                    // ContainerControl needs to see it needs to find a new ActiveControl.
                    if (Owner.GetContainerControl() is ContainerControl cc)
                    {
                        cc.AfterControlRemoved(value, Owner);
                    }
                }
            }

            void IList.Remove(object control)
            {
                if (control is Control)
                {
                    Remove((Control)control);
                }
            }

            public void RemoveAt(int index)
            {
                Remove(this[index]);
            }

            /// <summary>
            ///  Removes the child control with the specified key.
            /// </summary>
            public virtual void RemoveByKey(string key)
            {
                int index = IndexOfKey(key);
                if (IsValidIndex(index))
                {
                    RemoveAt(index);
                }
            }

            /// <summary>
            ///  Retrieves the child control with the specified index.
            /// </summary>
            public new virtual Control this[int index]
            {
                get
                {
                    //do some bounds checking here...
                    if (index < 0 || index >= Count)
                    {
                        throw new ArgumentOutOfRangeException(
                            nameof(index),
                            string.Format(SR.IndexOutOfRange, index.ToString(CultureInfo.CurrentCulture)));
                    }

                    Control control = (Control)InnerList[index];
                    Debug.Assert(control != null, "Why are we returning null controls from a valid index?");
                    return control;
                }
            }

            /// <summary>
            ///  Retrieves the child control with the specified key.
            /// </summary>
            public virtual Control this[string key]
            {
                get
                {
                    // We do not support null and empty string as valid keys.
                    if (string.IsNullOrEmpty(key))
                    {
                        return null;
                    }

                    // Search for the key in our collection
                    int index = IndexOfKey(key);
                    if (IsValidIndex(index))
                    {
                        return this[index];
                    }
                    else
                    {
                        return null;
                    }
                }
            }

            public virtual void Clear()
            {
#if DEBUG
                int layoutSuspendCount = Owner.LayoutSuspendCount;
#endif
                Owner.SuspendLayout();
                // clear all preferred size caches in the tree -
                // inherited fonts could go away, etc.
                CommonProperties.xClearAllPreferredSizeCaches(Owner);

                try
                {
                    while (Count != 0)
                    {
                        RemoveAt(Count - 1);
                    }
                }
                finally
                {
                    Owner.ResumeLayout();
#if DEBUG
                    Debug.Assert(Owner.LayoutSuspendCount == layoutSuspendCount, "Suspend/Resume layout mismatch!");
#endif
                }
            }

            /// <summary>
            ///  Retrieves the index of the specified
            ///  child control in this array.  An ArgumentException
            ///  is thrown if child is not parented to this
            ///  Control.
            /// </summary>
            public int GetChildIndex(Control child)
            {
                return GetChildIndex(child, true);
            }

            /// <summary>
            ///  Retrieves the index of the specified
            ///  child control in this array.  An ArgumentException
            ///  is thrown if child is not parented to this
            ///  Control.
            /// </summary>
            public virtual int GetChildIndex(Control child, bool throwException)
            {
                int index = IndexOf(child);
                if (index == -1 && throwException)
                {
                    throw new ArgumentException(SR.ControlNotChild);
                }
                return index;
            }

            /// <summary>
            ///  This is internal virtual method so that "Readonly Collections" can override this and throw as they should not allow changing
            ///  the child control indices.
            /// </summary>
            internal virtual void SetChildIndexInternal(Control child, int newIndex)
            {
                // Sanity check parameters
                if (child is null)
                {
                    throw new ArgumentNullException(nameof(child));
                }

                int currentIndex = GetChildIndex(child);

                if (currentIndex == newIndex)
                {
                    return;
                }

                if (newIndex >= Count || newIndex == -1)
                {
                    newIndex = Count - 1;
                }

                MoveElement(child, currentIndex, newIndex);
                child.UpdateZOrder();

                LayoutTransaction.DoLayout(Owner, child, PropertyNames.ChildIndex);
            }

            /// <summary>
            ///  Sets the index of the specified
            ///  child control in this array.  An ArgumentException
            ///  is thrown if child is not parented to this
            ///  Control.
            /// </summary>
            public virtual void SetChildIndex(Control child, int newIndex)
            {
                SetChildIndexInternal(child, newIndex);
            }

            // This is the same as WinformsUtils.ArraySubsetEnumerator
            // however since we're no longer an array, we've gotta employ a
            // special version of this.
            private class ControlCollectionEnumerator : IEnumerator
            {
                private readonly ControlCollection controls;
                private int current;
                private readonly int originalCount;

                public ControlCollectionEnumerator(ControlCollection controls)
                {
                    this.controls = controls;
                    originalCount = controls.Count;
                    current = -1;
                }

                public bool MoveNext()
                {
                    // We have to use Controls.Count here because someone could have deleted
                    // an item from the array.
                    //
                    // this can happen if someone does:
                    //     foreach (Control c in Controls) { c.Dispose(); }
                    //
                    // We also dont want to iterate past the original size of the collection
                    //
                    // this can happen if someone does
                    //     foreach (Control c in Controls) { c.Controls.Add(new Label()); }

                    if (current < controls.Count - 1 && current < originalCount - 1)
                    {
                        current++;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }

                public void Reset()
                {
                    current = -1;
                }

                public object Current
                {
                    get
                    {
                        if (current == -1)
                        {
                            return null;
                        }
                        else
                        {
                            return controls[current];
                        }
                    }
                }
            }
        }
    }
}
