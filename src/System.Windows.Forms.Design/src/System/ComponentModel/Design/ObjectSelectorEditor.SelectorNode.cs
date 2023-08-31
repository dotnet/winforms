// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Windows.Forms;

namespace System.ComponentModel.Design;

public abstract partial class ObjectSelectorEditor
{
    public class SelectorNode : TreeNode
    {
        public object? value;

        /// <summary>
        ///  Sets label and value to given.
        /// </summary>
        public SelectorNode(string? label, object? value) : base(label)
        {
            this.value = value;
        }
    }
}
