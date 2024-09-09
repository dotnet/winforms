// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms.Primitives;

namespace System.Windows.Forms;

[Editor($"System.Windows.Forms.Design.TreeNodeCollectionEditor, {AssemblyRef.SystemDesign}", typeof(UITypeEditor))]
public class TreeNodeCollection : IList
{
    private readonly TreeNode _owner;

    ///  A caching mechanism for key accessor
    ///  We use an index here rather than control so that we don't have lifetime
    ///  issues by holding on to extra references.
    private int _lastAccessedIndex = -1;

    internal TreeNodeCollection(TreeNode owner)
    {
        _owner = owner;
    }

    // This index is used to optimize performance of AddRange
    // items are added from last to first after this index
    // (to work around TV_INSertItem comctl32 perf issue with consecutive adds in the end of the list)
    internal int FixedIndex { get; set; } = -1;

    public virtual TreeNode this[int index]
    {
        get
        {
            ArgumentOutOfRangeException.ThrowIfNegative(index);
            ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(index, _owner._childCount);

            return _owner._children[index];
        }
        set
        {
            ArgumentOutOfRangeException.ThrowIfNegative(index);
            ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(index, _owner._childCount);
            ArgumentNullException.ThrowIfNull(value);

            TreeView tv = _owner._treeView!;
            TreeNode actual = _owner._children[index];

            if (value._treeView is not null && value._treeView.Handle != tv.Handle)
            {
                throw new ArgumentException(string.Format(SR.TreeNodeBoundToAnotherTreeView), nameof(value));
            }

            if (tv._nodesByHandle.ContainsKey(value.Handle) && value._index != index)
            {
                throw new ArgumentException(string.Format(SR.OnlyOneControl, value.Text), nameof(value));
            }

            if (tv._nodesByHandle.ContainsKey(value.Handle)
                && value.Handle == actual.Handle
                && value._index == index)
            {
                return;
            }

            Remove(actual);
            Insert(index, value);
        }
    }

    object? IList.this[int index]
    {
        get => this[index];
        set
        {
            if (value is TreeNode treeNode)
            {
                this[index] = treeNode;
            }
            else
            {
                throw new ArgumentException(SR.TreeNodeCollectionBadTreeNode, nameof(value));
            }
        }
    }

    /// <summary>
    ///  Retrieves the child control with the specified key.
    /// </summary>
    public virtual TreeNode? this[string? key]
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

    // Make this property available to Intellisense. (Removed the EditorBrowsable attribute.)
    [Browsable(false)]
    public int Count => _owner._childCount;

    object ICollection.SyncRoot => this;

    bool ICollection.IsSynchronized => false;

    bool IList.IsFixedSize => false;

    public bool IsReadOnly => false;

    /// <summary>
    ///  Creates a new child node under this node. Child node is positioned after siblings.
    /// </summary>
    public virtual TreeNode Add(string? text)
    {
        TreeNode tn = new(text);
        Add(tn);
        return tn;
    }

    // <-- NEW ADD OVERLOADS IN WHIDBEY

    /// <summary>
    ///  Creates a new child node under this node. Child node is positioned after siblings.
    /// </summary>
    public virtual TreeNode Add(string? key, string? text)
    {
        TreeNode tn = new(text)
        {
            Name = key
        };
        Add(tn);
        return tn;
    }

    /// <summary>
    ///  Creates a new child node under this node. Child node is positioned after siblings.
    /// </summary>
    public virtual TreeNode Add(string? key, string? text, int imageIndex)
    {
        TreeNode tn = new(text)
        {
            Name = key,
            ImageIndex = imageIndex
        };
        Add(tn);
        return tn;
    }

    /// <summary>
    ///  Creates a new child node under this node. Child node is positioned after siblings.
    /// </summary>
    public virtual TreeNode Add(string? key, string? text, string? imageKey)
    {
        TreeNode tn = new(text)
        {
            Name = key,
            ImageKey = imageKey
        };
        Add(tn);
        return tn;
    }

    /// <summary>
    ///  Creates a new child node under this node. Child node is positioned after siblings.
    /// </summary>
    public virtual TreeNode Add(string? key, string? text, int imageIndex, int selectedImageIndex)
    {
        TreeNode tn = new(text, imageIndex, selectedImageIndex)
        {
            Name = key
        };
        Add(tn);
        return tn;
    }

    /// <summary>
    ///  Creates a new child node under this node. Child node is positioned after siblings.
    /// </summary>
    public virtual TreeNode Add(string? key, string? text, string? imageKey, string? selectedImageKey)
    {
        TreeNode tn = new(text)
        {
            Name = key,
            ImageKey = imageKey,
            SelectedImageKey = selectedImageKey
        };
        Add(tn);
        return tn;
    }

    // END - NEW ADD OVERLOADS IN WHIDBEY -->

    public virtual void AddRange(params TreeNode[] nodes)
    {
        ArgumentNullException.ThrowIfNull(nodes);

        if (nodes.Length == 0)
        {
            return;
        }

        TreeView? tv = _owner.TreeView;
        if (tv is not null && nodes.Length > TreeNode.MAX_TREENODES_OPS)
        {
            tv.BeginUpdate();
        }

        if (!LocalAppContextSwitches.TreeNodeCollectionAddRangeRespectsSortOrder || tv is null || !tv.Sorted)
        {
            _owner.Nodes.FixedIndex = _owner._childCount;
        }

        _owner.EnsureCapacity(nodes.Length);
        for (int i = nodes.Length - 1; i >= 0; i--)
        {
            AddInternal(nodes[i], delta: i);
        }

        _owner.Nodes.FixedIndex = -1;
        if (tv is not null && nodes.Length > TreeNode.MAX_TREENODES_OPS)
        {
            tv.EndUpdate();
        }
    }

    public TreeNode[] Find(string key, bool searchAllChildren)
    {
        key.ThrowIfNullOrEmptyWithMessage(SR.FindKeyMayNotBeEmptyOrNull);

        List<TreeNode> foundNodes = FindInternal(key, searchAllChildren, this, []);

        return [.. foundNodes];
    }

    private static List<TreeNode> FindInternal(
        string key,
        bool searchAllChildren,
        TreeNodeCollection treeNodeCollectionToLookIn,
        List<TreeNode> foundTreeNodes)
    {
        // Perform breadth first search - as it's likely people will want tree nodes belonging
        // to the same parent close to each other.
        for (int i = 0; i < treeNodeCollectionToLookIn.Count; i++)
        {
            if (treeNodeCollectionToLookIn[i] is null)
            {
                continue;
            }

            if (WindowsFormsUtils.SafeCompareStrings(treeNodeCollectionToLookIn[i].Name, key, ignoreCase: true))
            {
                foundTreeNodes.Add(treeNodeCollectionToLookIn[i]);
            }
        }

        // Optional recursive search for controls in child collections.
        if (searchAllChildren)
        {
            for (int i = 0; i < treeNodeCollectionToLookIn.Count; i++)
            {
                if (treeNodeCollectionToLookIn[i] is null)
                {
                    continue;
                }

                if ((treeNodeCollectionToLookIn[i].Nodes is not null) && treeNodeCollectionToLookIn[i].Nodes.Count > 0)
                {
                    // If it has a valid child collection, append those results to our collection.
                    foundTreeNodes = FindInternal(key, searchAllChildren, treeNodeCollectionToLookIn[i].Nodes, foundTreeNodes);
                }
            }
        }

        return foundTreeNodes;
    }

    /// <summary>
    ///  Adds a new child node to this node. Child node is positioned after siblings.
    /// </summary>
    public virtual int Add(TreeNode node) => AddInternal(node, delta: 0);

    private int AddInternal(TreeNode node, int delta)
    {
        ArgumentNullException.ThrowIfNull(node);

        if (node.HTREEITEMInternal != IntPtr.Zero)
        {
            throw new ArgumentException(string.Format(SR.OnlyOneControl, node.Text), nameof(node));
        }

        // Check for ParentingCycle
        _owner.CheckParentingCycle(node);

        // If the TreeView is sorted, index is ignored
        TreeView? tv = _owner.TreeView;

        if (tv is not null)
        {
            foreach (TreeNode treeNode in node.GetSelfAndChildNodes())
            {
                KeyboardToolTipStateMachine.Instance.Hook(treeNode, tv.KeyboardToolTip);
            }
        }

        if (tv is not null && tv.Sorted)
        {
            return _owner.AddSorted(tv, node);
        }

        node._parent = _owner;
        int fixedIndex = _owner.Nodes.FixedIndex;
        if (fixedIndex != -1)
        {
            node._index = fixedIndex + delta;
        }
        else
        {
            // if fixedIndex != -1 capacity was ensured by AddRange
            Debug.Assert(delta == 0, "delta should be 0");
            _owner.EnsureCapacity(1);
            node._index = _owner._childCount;
        }

        _owner._children[node._index] = node;
        _owner._childCount++;
        node.Realize(false);

        if (tv is not null && node == tv._selectedNode)
        {
            tv.SelectedNode = node; // communicate this to the handle
        }

        if (tv is not null && tv.TreeViewNodeSorter is not null)
        {
            tv.Sort();
        }

        return node._index;
    }

    int IList.Add(object? node)
    {
        ArgumentNullException.ThrowIfNull(node);

        if (node is TreeNode treeNode)
        {
            return Add(treeNode);
        }
        else
        {
            return Add(node.ToString())._index;
        }
    }

    public bool Contains(TreeNode node) => IndexOf(node) != -1;

    /// <summary>
    ///  Returns true if the collection contains an item with the specified key, false otherwise.
    /// </summary>
    public virtual bool ContainsKey(string? key) => IsValidIndex(IndexOfKey(key));

    bool IList.Contains(object? node) => node is TreeNode treeNode && Contains(treeNode);

    public int IndexOf(TreeNode node)
    {
        for (int index = 0; index < Count; ++index)
        {
            if (this[index] == node)
            {
                return index;
            }
        }

        return -1;
    }

    int IList.IndexOf(object? node) =>
        node is TreeNode treeNode
            ? IndexOf(treeNode)
            : -1;

    /// <summary>
    ///  The zero-based index of the first occurrence of value within the entire CollectionBase, if found; otherwise, -1.
    /// </summary>
    public virtual int IndexOfKey(string? key)
    {
        // Step 0 - Arg validation
        if (string.IsNullOrEmpty(key))
        {
            return -1; // we don't support empty or null keys.
        }

        // step 1 - check the last cached item
        if (IsValidIndex(_lastAccessedIndex))
        {
            if (WindowsFormsUtils.SafeCompareStrings(this[_lastAccessedIndex].Name, key, ignoreCase: true))
            {
                return _lastAccessedIndex;
            }
        }

        // step 2 - search for the item
        for (int i = 0; i < Count; i++)
        {
            if (WindowsFormsUtils.SafeCompareStrings(this[i].Name, key, ignoreCase: true))
            {
                _lastAccessedIndex = i;
                return i;
            }
        }

        // step 3 - we didn't find it. Invalidate the last accessed index and return -1.
        _lastAccessedIndex = -1;
        return -1;
    }

    /// <summary>
    ///  Inserts a new child node on this node. Child node is positioned as specified by index.
    /// </summary>
    public virtual void Insert(int index, TreeNode node)
    {
        if (node.HTREEITEMInternal != IntPtr.Zero)
        {
            throw new ArgumentException(string.Format(SR.OnlyOneControl, node.Text), nameof(node));
        }

        // Check for ParentingCycle
        _owner.CheckParentingCycle(node);

        // If the TreeView is sorted, index is ignored
        TreeView? tv = _owner.TreeView;

        if (tv is not null)
        {
            foreach (TreeNode treeNode in node.GetSelfAndChildNodes())
            {
                KeyboardToolTipStateMachine.Instance.Hook(treeNode, tv.KeyboardToolTip);
            }
        }

        if (tv is not null && tv.Sorted)
        {
            _owner.AddSorted(tv, node);
            return;
        }

        if (index < 0)
        {
            index = 0;
        }

        if (index > _owner._childCount)
        {
            index = _owner._childCount;
        }

        _owner.InsertNodeAt(index, node);
    }

    void IList.Insert(int index, object? node)
    {
        if (node is TreeNode treeNode)
        {
            Insert(index, treeNode);
        }
        else
        {
            throw new ArgumentException(SR.TreeNodeCollectionBadTreeNode, nameof(node));
        }
    }

    // <-- NEW INSERT OVERLOADS IN WHIDBEY

    /// <summary>
    ///  Inserts a new child node on this node. Child node is positioned as specified by index.
    /// </summary>
    public virtual TreeNode Insert(int index, string? text)
    {
        TreeNode tn = new(text);
        Insert(index, tn);
        return tn;
    }

    /// <summary>
    ///  Inserts a new child node on this node. Child node is positioned as specified by index.
    /// </summary>
    public virtual TreeNode Insert(int index, string? key, string? text)
    {
        TreeNode tn = new(text)
        {
            Name = key
        };
        Insert(index, tn);
        return tn;
    }

    /// <summary>
    ///  Inserts a new child node on this node. Child node is positioned as specified by index.
    /// </summary>
    public virtual TreeNode Insert(int index, string? key, string? text, int imageIndex)
    {
        TreeNode tn = new(text)
        {
            Name = key,
            ImageIndex = imageIndex
        };
        Insert(index, tn);
        return tn;
    }

    /// <summary>
    ///  Inserts a new child node on this node. Child node is positioned as specified by index.
    /// </summary>
    public virtual TreeNode Insert(int index, string? key, string? text, string? imageKey)
    {
        TreeNode tn = new(text)
        {
            Name = key,
            ImageKey = imageKey
        };
        Insert(index, tn);
        return tn;
    }

    /// <summary>
    ///  Inserts a new child node on this node. Child node is positioned as specified by index.
    /// </summary>
    public virtual TreeNode Insert(int index, string? key, string? text, int imageIndex, int selectedImageIndex)
    {
        TreeNode tn = new(text, imageIndex, selectedImageIndex)
        {
            Name = key
        };
        Insert(index, tn);
        return tn;
    }

    /// <summary>
    ///  Inserts a new child node on this node. Child node is positioned as specified by index.
    /// </summary>
    public virtual TreeNode Insert(int index, string? key, string? text, string? imageKey, string? selectedImageKey)
    {
        TreeNode tn = new(text)
        {
            Name = key,
            ImageKey = imageKey,
            SelectedImageKey = selectedImageKey
        };
        Insert(index, tn);
        return tn;
    }

    // END - NEW INSERT OVERLOADS IN WHIDBEY -->

    /// <summary>
    ///  Determines if the index is valid for the collection.
    /// </summary>
    private bool IsValidIndex(int index) => (index >= 0) && (index < Count);

    /// <summary>
    ///  Remove all nodes from the tree view.
    /// </summary>
    public virtual void Clear()
    {
        _owner.Clear();
    }

    public void CopyTo(Array dest, int index)
    {
        if (_owner._childCount > 0)
        {
            Array.Copy(_owner._children, 0, dest, index, _owner._childCount);
        }
    }

    public void Remove(TreeNode node)
    {
        node.Remove();
    }

    void IList.Remove(object? node)
    {
        if (node is TreeNode treeNode)
        {
            Remove(treeNode);
        }
    }

    public virtual void RemoveAt(int index)
    {
        this[index].Remove();
    }

    /// <summary>
    ///  Removes the child control with the specified key.
    /// </summary>
    public virtual void RemoveByKey(string? key)
    {
        int index = IndexOfKey(key);
        if (IsValidIndex(index))
        {
            RemoveAt(index);
        }
    }

    public IEnumerator GetEnumerator()
    {
        if (_owner._children is not null)
        {
            return new ArraySubsetEnumerator(_owner._children, _owner._childCount);
        }
        else
        {
            return Array.Empty<TreeNode>().GetEnumerator();
        }
    }
}
