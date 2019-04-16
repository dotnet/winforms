﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using System.Drawing.Design;

namespace System.Windows.Forms.Design
{
    /// <devdoc>
	/// Associates Type with ToolStripMenuItem.
	/// </devdoc>
	internal class ItemTypeToolStripMenuItem : ToolStripMenuItem
    {
        private static readonly string s_systemWindowsFormsNamespace = typeof(ToolStripItem).Namespace;
        private static readonly ToolboxItem s_invalidToolboxItem = new ToolboxItem();
        private readonly Type _itemType;
        private bool _convertTo = false;
        private ToolboxItem _tbxItem = s_invalidToolboxItem;
        private Image _image = null;

        public ItemTypeToolStripMenuItem(Type t) => _itemType = t;

        public static string SystemWindowsFormsNamespace => s_systemWindowsFormsNamespace;

        public Type ItemType
        {
            get => _itemType;
        }

        public bool ConvertTo
        {
            get => _convertTo;
            set => _convertTo = value;
        }

        public override Image Image
        {
            get
            {
                if (_image == null)
                {
                    _image = ToolStripDesignerUtils.GetToolboxBitmap(ItemType);
                }
                return _image;
            }
            set
            {
            }
        }

        public override string Text
        {
            get => ToolStripDesignerUtils.GetToolboxDescription(ItemType);
            set
            {
            }
        }

        public ToolboxItem TbxItem { get => _tbxItem; set => _tbxItem = value; }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                TbxItem = null;
            }
            base.Dispose(disposing);
        }

    }
}
