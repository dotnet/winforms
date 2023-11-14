// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel.Design;

namespace System.Windows.Forms.Design
{
    /// <summary>
    ///  This collection editor is used to add value to the TableLayoutPanel's
    ///  'Style' collections: RowStyle and ColumnStyle.  Here, we override the
    ///  CreateInstance method and set the width or height of the new style
    ///  to a default minimum size.
    /// </summary>
    internal partial class StyleCollectionEditor(Type type) : CollectionEditor(type)
    {
        private readonly bool isRowCollection = type.IsAssignableFrom(typeof(TableLayoutRowStyleCollection));
        protected string? _helpTopic;

        /// <summary>
        ///  Overridden to create our editor form instead of the standard collection editor form.
        /// </summary>
        /// <returns>An instance of a StyleEditorForm</returns>
        protected override CollectionForm CreateCollectionForm()
        {
            return new StyleEditorForm(this, isRowCollection);
        }

        /// <summary>
        ///  Gets the help topic to display for the dialog help button or pressing F1.
        ///  Override to display a different help topic.
        /// </summary>
        protected override string HelpTopic => _helpTopic ?? base.HelpTopic;
    }
}
