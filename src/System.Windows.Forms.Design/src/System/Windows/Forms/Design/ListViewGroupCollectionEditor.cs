// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;

namespace System.Windows.Forms.Design
{
    /// <summary>
    ///  Provides an editor for a ListView groups collection.
    /// </summary>
    internal class ListViewGroupCollectionEditor : CollectionEditor
    {
        private object _editValue;

        public ListViewGroupCollectionEditor(Type type) : base(type)
        { }

        /// <summary>
        ///  Creates a ListViewGroup instance.
        /// </summary>
        protected override object CreateInstance(Type itemType)
        {
            ListViewGroup lvg = (ListViewGroup)base.CreateInstance(itemType);

            // Create an unique name for the list view group.
            lvg.Name = CreateListViewGroupName((ListViewGroupCollection)_editValue);

            return lvg;
        }

        private string CreateListViewGroupName(ListViewGroupCollection lvgCollection)
        {
            string lvgName = "ListViewGroup";
            string resultName;
            INameCreationService ncs = GetService(typeof(INameCreationService)) as INameCreationService;
            IContainer container = GetService(typeof(IContainer)) as IContainer;

            if (ncs != null && container != null)
            {
                lvgName = ncs.CreateName(container, typeof(ListViewGroup));
            }

            // strip the digits from the end.
            while (char.IsDigit(lvgName[lvgName.Length - 1]))
            {
                lvgName = lvgName.Substring(0, lvgName.Length - 1);
            }

            int i = 1;
            resultName = lvgName + i.ToString(System.Globalization.CultureInfo.CurrentCulture);

            while (lvgCollection[resultName] != null)
            {
                i++;
                resultName = lvgName + i.ToString(System.Globalization.CultureInfo.CurrentCulture);
            }

            return resultName;
        }

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            _editValue = value;
            object ret;

            // This will block while the ListViewGroupCollectionDialog is running.
            ret = base.EditValue(context, provider, value);

            // The user is done w/ the ListViewGroupCollectionDialog.
            // Don't need the edit value any longer
            _editValue = null;

            return ret;
        }
    }
}
