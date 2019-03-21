// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.ComponentModel.Design
{
    public sealed class DesignerActionPropertyItem : DesignerActionItem
    {
        private string _memberName;
        private IComponent _relatedComponent;

        public DesignerActionPropertyItem(string memberName, string displayName, string category, string description) : base(displayName, category, description)
        {
            _memberName = memberName;
        }

        public DesignerActionPropertyItem(string memberName, string displayName) : this(memberName, displayName, null, null)
        {
        }

        public DesignerActionPropertyItem(string memberName, string displayName, string category) : this(memberName, displayName, category, null)
        {
        }

        public string MemberName
        {
            get => _memberName;
        }

        public IComponent RelatedComponent
        {
            get => _relatedComponent;
            set => _relatedComponent = value;
        }

    }
}
