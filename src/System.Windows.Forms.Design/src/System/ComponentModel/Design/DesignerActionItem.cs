// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;

namespace System.ComponentModel.Design
{
    /// <summary>
    ///     A menu command that defines text and other metadata to describe a targeted task that can be performed.
    ///      Tasks typically walk the user through some multi-step process, such as configuring a data source for a component.
    ///      Designer tasks are shown in a custom piece of UI (Chrome).
    /// </summary>
    public abstract class DesignerActionItem
    {
        public DesignerActionItem(string displayName, string category, string description)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        internal DesignerActionItem()
        {
        }

        public bool AllowAssociate
        {
            get => throw new NotImplementedException(SR.NotImplementedByDesign);
            set => throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        public virtual string Category => throw new NotImplementedException(SR.NotImplementedByDesign);

        public virtual string Description => throw new NotImplementedException(SR.NotImplementedByDesign);

        public virtual string DisplayName => throw new NotImplementedException(SR.NotImplementedByDesign);

        public IDictionary Properties => throw new NotImplementedException(SR.NotImplementedByDesign);

        /// <summary>
        ///     This property is used for determining availability of this command in the source view. Some designer actions
        ///     have no effect on the source code and are excluded from the list of available commands in chrome.
        /// </summary>
        public bool ShowInSourceView
        {
            get => throw new NotImplementedException(SR.NotImplementedByDesign);
            set => throw new NotImplementedException(SR.NotImplementedByDesign);
        }
    }
}
