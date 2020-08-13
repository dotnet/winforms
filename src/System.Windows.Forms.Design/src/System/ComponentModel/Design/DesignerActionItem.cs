// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Specialized;
using System.Text.RegularExpressions;

namespace System.ComponentModel.Design
{
    /// <summary>
    ///  A menu command that defines text and other metadata to describe a targeted task that can be performed.
    ///  Tasks typically walk the user through some multi-step process, such as configuring a data source for a component.
    ///  Designer tasks are shown in a custom piece of UI (Chrome).
    /// </summary>
    public abstract class DesignerActionItem
    {
        private IDictionary _properties;

        public DesignerActionItem(string displayName, string category, string description)
        {
            DisplayName = displayName is null ? null : Regex.Replace(displayName, @"\(\&.\)", "");
            Category = category;
            Description = description;
        }

        public bool AllowAssociate { get; set; }

        public virtual string Category { get; }

        public virtual string Description { get; }

        public virtual string DisplayName { get; }

        public IDictionary Properties => _properties ?? (_properties = new HybridDictionary());

        public bool ShowInSourceView { get; set; } = true;
    }
}
