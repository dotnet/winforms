// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel.Design;

namespace System.Windows.Forms.Design
{
    internal partial class StyleCollectionEditor : CollectionEditor
    {
        private readonly bool _isRowCollection;
        protected string? _helpTopic;

        /// <summary>
        ///  This collection editor is used to add value to the <see cref="TableLayoutPanel" />'s
        ///  'Style' collections: RowStyle and ColumnStyle.
        /// </summary>
        /// <param name="type">A specified collection type.</param>
        public StyleCollectionEditor(Type type) : base(type)
        {
            _isRowCollection = type.IsAssignableFrom(typeof(TableLayoutRowStyleCollection));
        }

        /// <summary>
        ///  Overridden to create our editor form instead of the standard collection editor form.
        /// </summary>
        /// <returns>An instance of a StyleEditorForm</returns>
        protected override CollectionForm CreateCollectionForm()
        {
            return new StyleEditorForm(this, _isRowCollection);
        }

        /// <summary>
        ///  Gets the help topic to display for the dialog help button or pressing F1.
        ///  Override to display a different help topic.
        /// </summary>
        protected override string HelpTopic => _helpTopic ?? base.HelpTopic;
    }
}
