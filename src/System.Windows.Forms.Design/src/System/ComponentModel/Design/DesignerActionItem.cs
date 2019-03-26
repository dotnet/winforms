// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Specialized;
using System.Text.RegularExpressions;

namespace System.ComponentModel.Design
{
    /// <summary>
    /// A menu command that defines text and other metadata to describe a targeted task that can be performed.
    /// Tasks typically walk the user through some multi-step process, such as configuring a data source for a component.
    /// Designer tasks are shown in a custom piece of UI (Chrome).
    /// </summary>
    public abstract class DesignerActionItem
    {
        private bool _allowAssociate;
        private readonly string _displayName;
        private readonly string _description;
        private readonly string _category;
        private IDictionary _properties;
        private bool _showInSourceView = true;

        public DesignerActionItem(string displayName, string category, string description)
        {
            _category = category;
            _description = description;
            _displayName = displayName == null ? null : Regex.Replace(displayName, @"\(\&.\)", "");
        }

        internal DesignerActionItem()
        {
        }

        public bool AllowAssociate
        {
            get => _allowAssociate;
            set => _allowAssociate = value;
        }

        public virtual string Category
        {
            get => _category;
        }

        public virtual string Description
        {
            get => _description;
        }

        public virtual string DisplayName
        {
            get => _displayName;
        }

        public IDictionary Properties
        {
            get
            {
                if (_properties == null)
                {
                    _properties = new HybridDictionary();
                }
                return _properties;
            }
        }

        public bool ShowInSourceView
        {
            get => _showInSourceView;
            set => _showInSourceView = value;
        }
    }
}
