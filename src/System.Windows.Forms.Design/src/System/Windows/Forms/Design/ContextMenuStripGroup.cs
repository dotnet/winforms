// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

namespace System.Windows.Forms.Design
{
    internal class ContextMenuStripGroup
    {
        private List<ToolStripItem> _items;
        private readonly string _name;

        public ContextMenuStripGroup(string name) => _name = name;

        public List<ToolStripItem> Items
        {
            get
            {
                _items ??= new List<ToolStripItem>();

                return _items;
            }
        }
    }
}
