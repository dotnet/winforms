// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.ComponentModel.Design;

namespace System.Windows.Forms.Design
{
    internal class ColumnHeaderCollectionEditor : CollectionEditor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref='System.Windows.Forms.Design.ImageCollectionEditor'/> class.
        /// </summary>
        public ColumnHeaderCollectionEditor(Type type) : base(type)
        {
        }

        /// <summary>
        /// Gets the help topic to display for the dialog help button or pressing F1. Override to display a different help topic.
        /// </summary>
        protected override string HelpTopic
        {
            get => "net.ComponentModel.ColumnHeaderCollectionEditor";
        }

        /// <summary>
        /// Sets the specified collection to have the specified array of items.
        /// </summary>
        protected override object SetItems(object editValue, object[] value)
        {
            if (editValue is ListView.ColumnHeaderCollection list)
            {
                list.Clear();
                ColumnHeader[] colHeaders = new ColumnHeader[value.Length];
                Array.Copy(value, 0, colHeaders, 0, value.Length);
                list.AddRange(colHeaders);
            }
            return editValue;
        }

        /// <summary>
        /// Removes the item from listview column header collection
        /// </summary>
        internal override void OnItemRemoving(object item)
        {
            if (!(Context.Instance is ListView listview))
            {
                return;
            }

            if (item is ColumnHeader column)
            {
                IComponentChangeService cs = GetService(typeof(IComponentChangeService)) as IComponentChangeService;
                PropertyDescriptor itemsProp = null;
                if (cs != null)
                {
                    itemsProp = TypeDescriptor.GetProperties(Context.Instance)["Columns"];
                    cs.OnComponentChanging(Context.Instance, itemsProp);
                }
                listview.Columns.Remove(column);

                if (cs != null && itemsProp != null)
                {
                    cs.OnComponentChanged(Context.Instance, itemsProp, null, null);
                }
            }
        }
    }
}
