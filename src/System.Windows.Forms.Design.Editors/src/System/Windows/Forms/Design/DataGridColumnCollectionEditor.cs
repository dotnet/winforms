// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel.Design;

namespace System.Windows.Forms.Design
{
    internal class DataGridColumnCollectionEditor : CollectionEditor
    {
        private static readonly Type s_dataGridTextBoxColumnType = typeof(DataGridTextBoxColumn);
        private static readonly Type s_dataGridBoolCoumnType = typeof(DataGridBoolColumn);

        public DataGridColumnCollectionEditor(Type type) : base(type) { }

        /// <summary>
        /// Retrieves the data types this collection can contain.  The default 
        /// implementation looks inside of the collection for the Item property
        /// and returns the returning datatype of the item.  Do not call this
        /// method directly.  Instead, use the ItemTypes property.  Use this
        /// method to override the default implementation.
        /// </summary>
        protected override Type[] CreateNewItemTypes()
            => new Type[]
            {
                s_dataGridTextBoxColumnType,
                s_dataGridBoolCoumnType,
            };
    }
}
