// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;

namespace System.Windows.Forms.Design;

internal partial class DataGridViewDesigner
{
    [ComplexBindingProperties("DataSource", "DataMember")]
    private class DataGridViewChooseDataSourceActionList : DesignerActionList
    {
        private readonly DataGridViewDesigner _owner;

        public DataGridViewChooseDataSourceActionList(DataGridViewDesigner owner) : base(owner.Component)
        {
            _owner = owner;
        }

        // The choose data source designer action item is missing
        public override DesignerActionItemCollection GetSortedActionItems()
        {
            DesignerActionItemCollection items = [];
            DesignerActionPropertyItem chooseDataSource = new(
                nameof(DataSource),
                SR.DataGridViewChooseDataSource)
            {
                RelatedComponent = _owner.Component
            };

            items.Add(chooseDataSource);
            return items;
        }

        [AttributeProvider(typeof(IListSource))]
        [Editor($"System.Windows.Forms.Design.DataSourceListEditor, {AssemblyRef.SystemDesign}", typeof(UITypeEditor))]
        public object? DataSource
        {
            // Use the shadow property which is defined on the designer.
            get => _owner.DataSource;
            set
            {
                // left to do: transaction stuff
                DataGridView dataGridView = _owner.Control;
                IDesignerHost? host = dataGridView.Site?.GetService<IDesignerHost>();
                PropertyDescriptor? dataSourceProp = TypeDescriptor.GetProperties(dataGridView)["DataSource"];
                IComponentChangeService? changeService = dataGridView.Site?.GetService<IComponentChangeService>();
                DesignerTransaction? transaction = host?.CreateTransaction(string.Format(SR.DataGridViewChooseDataSourceTransactionString, dataGridView.Name));
                try
                {
                    changeService?.OnComponentChanging(_owner.Component, dataSourceProp);
                    // Use the shadow property which is defined on the designer.
                    _owner.DataSource = value;
                    changeService?.OnComponentChanged(_owner.Component, dataSourceProp, null, null);
                    transaction?.Commit();
                    transaction = null;
                }
                finally
                {
                    transaction?.Cancel();
                }
            }
        }
    }
}
