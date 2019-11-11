// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace System.Windows.Forms.Design.Editors
{
    internal class ColumnHeaderCollectionEditor : CollectionEditor
    {
        /// <devdoc>
        /// Initializes a new instance of the <see cref='System.Windows.Forms.Design.ImageCollectionEditor'/> class.
        /// </devdoc>

        //Called through reflection
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public ColumnHeaderCollectionEditor(Type type) : base(type)
        {
        }


        /// <devdoc>
        ///    Gets the help topic to display for the dialog help button or pressing F1. Override to display a different help topic.
        /// </devdoc>
        protected override string HelpTopic
        {
            get
            {
                return "net.ComponentModel.ColumnHeaderCollectionEditor";
            }
        }

        /// <devdoc>
        ///       Sets the specified collection to have the specified array of items.
        /// </devdoc>
        protected override object SetItems(object editValue, object[] value)
        {

            if (editValue != null)
            {
                // We look to see if the value implements IList, and if it does,  we set through that.
                Debug.Assert(editValue is System.Collections.IList, "editValue is not an IList");
                ListView.ColumnHeaderCollection list = editValue as ListView.ColumnHeaderCollection;
                if (editValue != null)
                {
                    list.Clear();
                    ColumnHeader[] colHeaders = new ColumnHeader[value.Length];
                    Array.Copy(value, 0, colHeaders, 0, value.Length);
                    list.AddRange(colHeaders);
                }
            }

            return editValue;
        }

        /// <devdoc>
        ///    <para>
        ///       Removes the item from listview column header collection
        ///    </para>
        /// </devdoc>
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
