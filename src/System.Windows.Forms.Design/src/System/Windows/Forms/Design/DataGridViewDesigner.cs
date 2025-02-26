// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Collections;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;

namespace System.Windows.Forms.Design;

internal class DataGridViewDesigner : ControlDesigner
{
    protected DesignerVerbCollection? designerVerbs;

    public override DataGridView Control => (DataGridView)Component;

    // CHROME stuff
    //
    // DesignerActionLists
    private DesignerActionListCollection? _actionLists;

    // need this to trap meta data changes
    private CurrencyManager? _currencyManager;

    public DataGridViewDesigner()
    {
        AutoResizeHandles = true;
    }

    // need AssociatedComponents for Copy / Paste
    public override ICollection AssociatedComponents => Control.Columns;

    public DataGridViewAutoSizeColumnsMode AutoSizeColumnsMode
    {
        get => Control.AutoSizeColumnsMode;
        set
        {
            // DANIELHE: this is a workaround for a bug in ComponentCache: if a component changed then its list of AssociatedComponents
            // is not marked as changed to the serialization engine will not serialize them.
            DataGridView dataGridView = Control;
            IComponentChangeService? componentChangeService = dataGridView.Site?.GetService<IComponentChangeService>();
            PropertyDescriptor? prop = TypeDescriptor.GetProperties(typeof(DataGridViewColumn))["Width"];

            for (int i = 0; i < dataGridView.Columns.Count; i++)
            {
                componentChangeService?.OnComponentChanging(dataGridView.Columns[i], prop);
            }

            dataGridView.AutoSizeColumnsMode = value;

            for (int i = 0; i < dataGridView.Columns.Count; i++)
            {
                componentChangeService?.OnComponentChanged(dataGridView.Columns[i], prop, null, null);
            }
        }
    }

    public object? DataSource
    {
        get => Control.DataSource;
        set
        {
            if (Control is { AutoGenerateColumns: true, DataSource: null } dataGridView && value is not null)
            {
                // RefreshColumnCollection() method does the job of siting/unsiting DataGridViewColumns
                // and calls OnComponentChanged/ing at the right times.
                // So we need to create / remove columns from the DataGridView DataSource via RefreshColumnCollection() method.
                // Set AutoGenerateColumn to false before setting the DataSource, otherwise DataGridViewColumns will be created
                // via the runtime method.
                //
                // We set AutoGenerateColumns to FALSE only if the DataGridView will get a not-null DataSource.
                dataGridView.AutoGenerateColumns = false;
            }

            Control.DataSource = value;
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            // we still need the current Component
            // so execute this code before base.Dispose(...)

            // unhook our event handlers
            if (HasComponent)
            {
                DataGridView dataGridView = Control;
                dataGridView.DataSourceChanged -= dataGridViewChanged;
                dataGridView.DataMemberChanged -= dataGridViewChanged;
                dataGridView.BindingContextChanged -= dataGridViewChanged;
                dataGridView.ColumnRemoved -= dataGridView_ColumnRemoved;

                // unhook the ComponentRemoved event handler
                if (dataGridView.Site.TryGetService(out IComponentChangeService? ccs))
                {
                    ccs.ComponentRemoving -= DataGridViewDesigner_ComponentRemoving;
                }
            }

            // unhook MetaDataChanged handler from the currency manager
            if (_currencyManager is not null)
            {
                _currencyManager.MetaDataChanged -= dataGridViewMetaDataChanged;
            }

            _currencyManager = null;
        }

        base.Dispose(disposing);
    }

    public override void Initialize(IComponent component)
    {
        base.Initialize(component);

        if (component.Site.TryGetService(out IComponentChangeService? componentChangeService))
        {
            componentChangeService.ComponentRemoving += DataGridViewDesigner_ComponentRemoving;
        }

        DataGridView dataGridView = Control;

        // DataGridViewDesigner::Initialize runs after InitializeComponent was deserialized.
        // just in case the user tinkered w/ InitializeComponent set AutoGenerateColumns to
        // TRUE if there is no DataSource, otherwise set it to FALSE
        dataGridView.AutoGenerateColumns = dataGridView.DataSource is null;

        // hook up the DataSourceChanged event, DataMemberChanged event
        dataGridView.DataSourceChanged += dataGridViewChanged;
        dataGridView.DataMemberChanged += dataGridViewChanged;
        dataGridView.BindingContextChanged += dataGridViewChanged;

        // now add data bound columns
        dataGridViewChanged(dataGridView, EventArgs.Empty);

        // Attach to column removed for clean up
        dataGridView.ColumnRemoved += dataGridView_ColumnRemoved;
    }

    public override void InitializeNewComponent(IDictionary? defaultValues)
    {
        base.InitializeNewComponent(defaultValues);
        Control.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
    }

    /// <devdoc>
    ///  <para>
    ///   Gets the inheritance attribute for the data grid view.
    ///  </para>
    /// </devdoc>
    protected override InheritanceAttribute? InheritanceAttribute
    {
        get
        {
            if ((base.InheritanceAttribute == InheritanceAttribute.Inherited)
                || (base.InheritanceAttribute == InheritanceAttribute.InheritedReadOnly))
            {
                return InheritanceAttribute.InheritedReadOnly;
            }

            return base.InheritanceAttribute;
        }
    }

    public override DesignerVerbCollection Verbs
    {
        get
        {
            if (designerVerbs is null)
            {
                designerVerbs = new DesignerVerbCollection();
                designerVerbs.Add(new DesignerVerb(SR.DataGridViewEditColumnsVerb, OnEditColumns));
                designerVerbs.Add(new DesignerVerb(SR.DataGridViewAddColumnVerb, OnAddColumn));
            }

            return designerVerbs;
        }
    }

    public override DesignerActionListCollection ActionLists
    {
        get
        {
            if (_actionLists is null)
            {
                BuildActionLists();
            }

            return _actionLists;
        }
    }

    [MemberNotNull(nameof(_actionLists))]
    private void BuildActionLists()
    {
        _actionLists = new DesignerActionListCollection();

        // ChooseDataSource action list
        _actionLists.Add(new DataGridViewChooseDataSourceActionList(this)
        {
            // if one actionList has AutoShow == true then the chrome panel will popup when the user DnD the DataGridView onto the form
            // It would make sense to promote AutoShow to DesignerActionListCollection.
            // But we don't own the DesignerActionListCollection so we just set AutoShow on the first ActionList
            //
            AutoShow = true
        });

        // column collection editing
        _actionLists.Add(new DataGridViewColumnEditingActionList(this));

        // AllowUserToAddRows / ReadOnly / AllowUserToDeleteRows / AllowUserToOrderColumns
        _actionLists.Add(new DataGridViewPropertiesActionList(this));
    }

    private void dataGridViewChanged(object? sender, EventArgs e)
    {
        DataGridView dataGridView = Control;

        CurrencyManager? newCM = null;
        if (dataGridView.DataSource is not null)
        {
            newCM = (CurrencyManager?)dataGridView.BindingContext?[dataGridView.DataSource, dataGridView.DataMember];
        }

        if (newCM != _currencyManager)
        {
            // unwire cm
            if (_currencyManager is not null)
            {
                _currencyManager.MetaDataChanged -= dataGridViewMetaDataChanged;
            }

            _currencyManager = newCM;

            // wire cm
            if (_currencyManager is not null)
            {
                _currencyManager.MetaDataChanged += dataGridViewMetaDataChanged;
            }
        }

        if (dataGridView.BindingContext is null)
        {
            MakeSureColumnsAreSited(dataGridView);
            return;
        }

        // the user changed the DataSource from null to something else
        // Because AutoGenerateColumns is already set to true, all the bound columns are created
        // Just set AutoGenerateColumns to false and return
        if (dataGridView.AutoGenerateColumns && dataGridView.DataSource is not null)
        {
            Debug.Assert(string.IsNullOrEmpty(dataGridView.DataMember), "in the designer we can't set DataSource and DataMember at the same time. Did you forget to set AutoGenerateColumns to false at a previous stage?");
            dataGridView.AutoGenerateColumns = false;
            MakeSureColumnsAreSited(dataGridView);
            return;
        }

        if (dataGridView.DataSource is null)
        {
            if (dataGridView.AutoGenerateColumns)
            {
                // the BindingContext changed;
                // nothing to do
                MakeSureColumnsAreSited(dataGridView);
                return;
            }
            else
            {
                // DataSource changed from something to null
                // set AutoGenerateColumns to true and remove the columns marked as DataBound
                dataGridView.AutoGenerateColumns = true;
            }
        }
        else
        {
            // we have a data source.
            // set AutoGenerateColumns to false and party on the new DS/DM
            dataGridView.AutoGenerateColumns = false;
        }

        RefreshColumnCollection();
    }

    private void DataGridViewDesigner_ComponentRemoving(object? sender, ComponentEventArgs e)
    {
        DataGridView dataGridView = Control;

        if (e.Component is not null && e.Component == dataGridView.DataSource)
        {
            //
            // The current data source was removed from the designer
            // Set DataSource to null.
            // This will:
            // 1. clear DataGridView::DataSource.
            // 2. clear DataGridView::DataMember.
            // 3. delete the unbound data grid view columns from the DataGridView column collection.
            // 4. set AutoGenerateColumns to TRUE.
            //

            IComponentChangeService? componentChangeService = GetService<IComponentChangeService>();
            string previousDataMember = dataGridView.DataMember;

            PropertyDescriptorCollection props = TypeDescriptor.GetProperties(dataGridView);
            PropertyDescriptor? propertyDescriptor = props?["DataMember"];

            if (propertyDescriptor is not null)
            {
                componentChangeService?.OnComponentChanging(dataGridView, propertyDescriptor);
            }

            dataGridView.DataSource = null;

            if (propertyDescriptor is not null)
            {
                componentChangeService?.OnComponentChanged(dataGridView, propertyDescriptor, previousDataMember, string.Empty);
            }
        }
    }

    private void dataGridView_ColumnRemoved(object? sender, DataGridViewColumnEventArgs e)
    {
        // Clear out the DisplayIndex for columns removed at design time.
        // If we don't do this, the columns will be reordered after build when there are both databound
        // columns and non databound columns. See VSWhidbey 572859. Ideally, we would do this on the Clear
        // of the column collection but there is no event there and we are restricted from adding any
        // Note: this does not break Undo/Redo because Undo/Redo uses serialization the same way
        // the build does and DisplayIndex is not serialized
        if (e.Column is not null && !e.Column.IsDataBound)
            e.Column.DisplayIndex = -1;
    }

    private static void MakeSureColumnsAreSited(DataGridView dataGridView)
    {
        // Other designers ( VS Data ) may manipulate the column collection but not add the columns to the
        // current container.
        // So we have to make sure that the data grid view columns are sited on the current container.
        IContainer? currentContainer = dataGridView.Site?.Container;
        for (int i = 0; i < dataGridView.Columns.Count; i++)
        {
            DataGridViewColumn col = dataGridView.Columns[i];
            IContainer? container = col.Site?.Container;
            if (currentContainer != container)
            {
                container?.Remove(col);

                currentContainer?.Add(col);
            }
        }
    }

    protected override void PreFilterProperties(IDictionary properties)
    {
        base.PreFilterProperties(properties);

        // Handle shadowed properties
        //
        string[] shadowProps =
        [
            "AutoSizeColumnsMode",
            "DataSource"
        ];

        Attribute[] empty = [];

        for (int i = 0; i < shadowProps.Length; i++)
        {
            if (properties[shadowProps[i]] is PropertyDescriptor prop)
            {
                properties[shadowProps[i]] = TypeDescriptor.CreateProperty(typeof(DataGridViewDesigner), prop, empty);
            }
        }
    }

    /// <summary>
    ///  If the previous schema and the current schema have common columns then ProcessSimilarSchema does the
    ///  work of removing columns which are not common and returns TRUE.
    ///  Otherwise ProcessSimilarSchema returns FALSE.
    /// </summary>
    private bool ProcessSimilarSchema(DataGridView dataGridView)
    {
        Debug.Assert(dataGridView.DataSource is null || _currencyManager is not null, "if we have a data source we should also have a currency manager by now");
        PropertyDescriptorCollection? backEndProps = null;

        if (_currencyManager is not null)
        {
            try
            {
                backEndProps = _currencyManager.GetItemProperties();
            }
            catch (ArgumentException ex)
            {
                throw new InvalidOperationException(SR.DataGridViewDataSourceNoLongerValid, ex);
            }
        }

        IContainer? currentContainer = dataGridView.Site?.Container;

        bool similarSchema = false;

        for (int i = 0; i < dataGridView.Columns.Count; i++)
        {
            DataGridViewColumn dataGridViewColumn = dataGridView.Columns[i];

            if (string.IsNullOrEmpty(dataGridViewColumn.DataPropertyName))
            {
                continue;
            }

            PropertyDescriptor? pd = TypeDescriptor.GetProperties(dataGridViewColumn)["UserAddedColumn"];
            object? UserAddedColumn = pd?.GetValue(dataGridViewColumn);
            if (UserAddedColumn is not null && (bool)UserAddedColumn)
            {
                continue;
            }

            PropertyDescriptor? dataFieldProperty = backEndProps?[dataGridViewColumn.DataPropertyName];
            // remove the column if the DataGridViewColumn::DataPropertyName does not map to any column anymore
            // or if the DataGridViewColumn::DataPropertyName maps to a sub list
            bool removeColumn = false;

            if (dataFieldProperty is null)
            {
                // DataPropertyName does not map to a column anymore
                removeColumn = true;
            }
            else if (typeof(IList).IsAssignableFrom(dataFieldProperty.PropertyType))
            {
                // DataPropertyName may map to an Image column.
                // Check for that using the Image TypeConverter
                TypeConverter imageTypeConverter = TypeDescriptor.GetConverter(typeof(Image));
                if (!imageTypeConverter.CanConvertFrom(dataFieldProperty.PropertyType))
                {
                    // DataPropertyName points to a sub list.
                    removeColumn = true;
                }
            }

            similarSchema = !removeColumn;
            if (similarSchema)
            {
                break;
            }
        }

        if (similarSchema)
        {
            ISite? site = dataGridView.Site;
            IComponentChangeService? changeService = site?.GetService<IComponentChangeService>();
            PropertyDescriptor? columnsProp = TypeDescriptor.GetProperties(dataGridView)["Columns"];
            try
            {
                changeService?.OnComponentChanging(dataGridView, columnsProp);
            }
            catch (InvalidOperationException)
            {
                return similarSchema;
            }

            for (int i = 0; i < dataGridView.Columns.Count;)
            {
                DataGridViewColumn dataGridViewColumn = dataGridView.Columns[i];

                if (string.IsNullOrEmpty(dataGridViewColumn.DataPropertyName))
                {
                    i++;
                    continue;
                }

                PropertyDescriptor? pd = TypeDescriptor.GetProperties(dataGridViewColumn)["UserAddedColumn"];
                object? UserAddedColumn = pd?.GetValue(dataGridViewColumn);
                if (UserAddedColumn is not null && (bool)UserAddedColumn)
                {
                    i++;
                    continue;
                }

                PropertyDescriptor? dataFieldProperty = backEndProps?[dataGridViewColumn.DataPropertyName];
                // remove the column if the DataGridViewColumn::DataPropertyName does not map to any column anymore
                // or if the DataGridViewColumn::DataPropertyName maps to a sub list
                bool removeColumn = false;

                if (dataFieldProperty is null)
                {
                    // DataPropertyName does not map to a column anymore
                    removeColumn = true;
                }
                else if (typeof(IList).IsAssignableFrom(dataFieldProperty.PropertyType))
                {
                    // DataPropertyName may map to an Image column.
                    // Check for that using the Image TypeConverter
                    TypeConverter imageTypeConverter = TypeDescriptor.GetConverter(typeof(Image));
                    if (!imageTypeConverter.CanConvertFrom(dataFieldProperty.PropertyType))
                    {
                        // DataPropertyName points to a sub list.
                        removeColumn = true;
                    }
                }

                if (removeColumn)
                {
                    dataGridView.Columns.Remove(dataGridViewColumn);
                    currentContainer?.Remove(dataGridViewColumn);
                }
                else
                {
                    i++;
                }
            }

            changeService?.OnComponentChanged(dataGridView, columnsProp, oldValue: null, newValue: null);
        }

        return similarSchema;
    }

    private void RefreshColumnCollection()
    {
        DataGridView dataGridView = Control;

        if (dataGridView.DataSource is ISupportInitializeNotification { IsInitialized: false })
        {
            // The DataSource is not initialized yet.
            // When the dataSource gets initialized it will send a MetaDataChanged event and at
            // that point we will refresh the column collection.
            return;
        }

        ISite? site = dataGridView.Site;
        IDesignerHost? host = site?.GetService<IDesignerHost>();

        // if after changing the data source / data member the dataGridView has columns which are still databound
        // then keep them and don't change the column collection.
        // we do this because bound columns represent the dataSource schema
        // ALSO, we get Undo/Redo for free w/ this.
        //
        // For example: our data grid view is bound to a dataManager which in turns is bound to a web service
        // which returns different dataSets w/ the same schema.
        // If the user wants to see only the first 2 columns and makes some changes to the first two columns ( width, cell style, etc )
        // then we don't want to throw these columns away when the dataManager receives another dataSet from the web service.
        // and we don't want to add another columns

        if (ProcessSimilarSchema(dataGridView))
        {
            return;
        }

        // Here is the sequence of actions to guarantee a good Undo/Redo:
        //
        // 1. OnComponentChanging DataGridView.Columns
        // 2. Remove the columns which should be removed
        // 3. OnComponentChanged DataGridView.Columns
        // 4. IContainer.Remove(dataGridView.Columns)
        // 5. IContainer.Add(new dataGridView.Columns)
        // 6. OnComponentChanging DataGridView.Columns
        // 7. DataGridView.Columns.Add( new DataGridViewColumns)
        // 8. OnComponentChanged DataGridView.Columns

        Debug.Assert(dataGridView.DataSource is null || _currencyManager is not null, "if we have a data source we should also have a currency manager by now");
        PropertyDescriptorCollection? backEndProps = null;

        if (_currencyManager is not null)
        {
            try
            {
                backEndProps = _currencyManager.GetItemProperties();
            }
            catch (ArgumentException ex)
            {
                throw new InvalidOperationException(SR.DataGridViewDataSourceNoLongerValid, ex);
            }
        }

        IContainer? currentContainer = site?.Container;
        IComponentChangeService? changeService = site?.GetService<IComponentChangeService>();
        PropertyDescriptor? columnsProp = TypeDescriptor.GetProperties(dataGridView)["Columns"];

        // 1. OnComponentChanging DataGridView.Columns
        changeService?.OnComponentChanging(dataGridView, columnsProp);

        DataGridViewColumn[] removeColumns = new DataGridViewColumn[dataGridView.Columns.Count];
        int removeColumnsCount = 0;
        for (int i = 0; i < dataGridView.Columns.Count; i++)
        {
            DataGridViewColumn col = dataGridView.Columns[i];
            if (!string.IsNullOrEmpty(col.DataPropertyName))
            {
                PropertyDescriptor? propertyDescriptor = TypeDescriptor.GetProperties(col)["UserAddedColumn"];
                object? UserAddedColumn = propertyDescriptor?.GetValue(col);
                if (propertyDescriptor is null || UserAddedColumn is null || !(bool)UserAddedColumn)
                {
                    removeColumns[removeColumnsCount] = col;
                    removeColumnsCount++;
                }
            }
        }

        // 2. Remove the columns which should be removed
        for (int i = 0; i < removeColumnsCount; i++)
        {
            dataGridView.Columns.Remove(removeColumns[i]);
        }

        // 3. OnComponentChanged DataGridView.Columns
        changeService?.OnComponentChanged(dataGridView, columnsProp, oldValue: null, newValue: null);

        // 4. IContainer.Remove(dataGridView.Columns)
        if (currentContainer is not null)
        {
            for (int i = 0; i < removeColumnsCount; i++)
            {
                currentContainer.Remove(removeColumns[i]);
            }
        }

        List<DataGridViewColumn>? columnsToBeAdded = null;
        if (dataGridView.DataSource is not null)
        {
            // backEndProps is not null here because _currencyManager cannot be null if dataGridView.DataSource is not null
            columnsToBeAdded = new List<DataGridViewColumn>(backEndProps!.Count);
            for (int i = 0; i < backEndProps.Count; i++)
            {
                TypeConverter imageTypeConverter = TypeDescriptor.GetConverter(typeof(Image));

                Type propType = backEndProps[i].PropertyType;

                Type columnType;

                if (typeof(IList).IsAssignableFrom(propType))
                {
                    if (imageTypeConverter.CanConvertFrom(propType))
                    {
                        columnType = typeof(DataGridViewImageColumn);
                    }
                    else
                    {
                        continue;
                    }
                }
                else if (propType == typeof(bool) || propType == typeof(CheckState))
                {
                    columnType = typeof(DataGridViewCheckBoxColumn);
                }
                else if (typeof(Image).IsAssignableFrom(propType) || imageTypeConverter.CanConvertFrom(propType))
                {
                    columnType = typeof(DataGridViewImageColumn);
                }
                else
                {
                    columnType = typeof(DataGridViewTextBoxColumn);
                }

                string nameFromText = ToolStripDesigner.NameFromText(backEndProps[i].Name, columnType, site);

                //
                // host.CreateComponent adds the column to its list of components
                //
                // 5. IContainer.Add(newDataGridView.Columns
                //
                DataGridViewColumn? dataGridViewColumn = TypeDescriptor.CreateInstance(host, columnType, null, null) as DataGridViewColumn;

                if (dataGridViewColumn is not null)
                {
                    dataGridViewColumn.DataPropertyName = backEndProps[i].Name;
                    dataGridViewColumn.HeaderText = !string.IsNullOrEmpty(backEndProps[i].DisplayName) ? backEndProps[i].DisplayName : backEndProps[i].Name;
                    dataGridViewColumn.Name = backEndProps[i].Name;
                    dataGridViewColumn.ValueType = backEndProps[i].PropertyType;
                    dataGridViewColumn.ReadOnly = backEndProps[i].IsReadOnly;

                    host?.Container.Add(dataGridViewColumn, nameFromText);

                    columnsToBeAdded.Add(dataGridViewColumn);
                }
            }
        }

        // 6. OnComponentChanging DataGridView.Columns
        changeService?.OnComponentChanging(dataGridView, columnsProp);

        // 7. DataGridView.Columns.Add( new DataGridViewColumns)
        if (columnsToBeAdded is not null)
        {
            for (int i = 0; i < columnsToBeAdded.Count; i++)
            {
                // wipe out the display index
                columnsToBeAdded[i].DisplayIndex = -1;
                dataGridView.Columns.Add(columnsToBeAdded[i]);
            }
        }

        // 8. OnComponentChanged DataGridView.Columns
        changeService?.OnComponentChanged(dataGridView, columnsProp, null, null);
    }

    private bool ShouldSerializeAutoSizeColumnsMode() => Control.AutoSizeColumnsMode != DataGridViewAutoSizeColumnsMode.None;

    private bool ShouldSerializeDataSource() => Control.DataSource is not null;

    // Ideally there would be a public method like this somewhere.
    internal static void ShowErrorDialog(IUIService? uiService, Exception ex, Control? dataGridView)
    {
        if (uiService is not null)
        {
            uiService.ShowError(ex);
        }
        else
        {
            string message = ex.Message;
            if (string.IsNullOrEmpty(message))
            {
                message = ex.ToString();
            }

            RTLAwareMessageBox.Show(dataGridView, message, null, MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                    MessageBoxDefaultButton.Button1, 0);
        }
    }

    internal static void ShowErrorDialog(IUIService? uiService, string errorString, Control? dataGridView)
    {
        if (uiService is not null)
        {
            uiService.ShowError(errorString);
        }
        else
        {
            RTLAwareMessageBox.Show(dataGridView, errorString, null, MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                    MessageBoxDefaultButton.Button1, 0);
        }
    }

    private void dataGridViewMetaDataChanged(object? sender, EventArgs e)
    {
        RefreshColumnCollection();
    }

    public void OnEditColumns(object? sender, EventArgs e)
    {
        IDesignerHost? host = Component.Site?.GetService<IDesignerHost>();

        // child modal dialog -launching in System Aware mode
        DataGridViewColumnCollectionDialog dialog = ScaleHelper.InvokeInSystemAwareContext(
            () => new DataGridViewColumnCollectionDialog());
        dialog.SetLiveDataGridView(Control);
        DesignerTransaction? transaction = host?.CreateTransaction(SR.DataGridViewEditColumnsTransactionString);
        DialogResult result = DialogResult.Cancel;

        try
        {
            result = ShowDialog(dialog);
        }
        finally
        {
            if (result == DialogResult.OK)
            {
                transaction?.Commit();
            }
            else
            {
                transaction?.Cancel();
            }
        }
    }

    public void OnAddColumn(object? sender, EventArgs e)
    {
        IDesignerHost? host = Component.Site?.GetService<IDesignerHost>();
        DesignerTransaction? transaction = host?.CreateTransaction(SR.DataGridViewAddColumnTransactionString);
        DialogResult result = DialogResult.Cancel;

        // child modal dialog -launching in System Aware mode
        DataGridViewAddColumnDialog dialog = ScaleHelper.InvokeInSystemAwareContext(
            () => new DataGridViewAddColumnDialog(Control.Columns, Control));
        dialog.Start(Control.Columns.Count, persistChangesToDesigner: true);

        try
        {
            result = ShowDialog(dialog);
        }
        finally
        {
            if (result == DialogResult.OK)
            {
                transaction?.Commit();
            }
            else
            {
                transaction?.Cancel();
            }
        }
    }

    private DialogResult ShowDialog(Form dialog)
    {
        if (Component.Site.TryGetService(out IUIService? service))
        {
            return service.ShowDialog(dialog);
        }
        else
        {
            return dialog.ShowDialog(Control);
        }
    }

    [ComplexBindingProperties("DataSource", "DataMember")]
    private class DataGridViewChooseDataSourceActionList : DesignerActionList
    {
        private readonly DataGridViewDesigner _owner;

        public DataGridViewChooseDataSourceActionList(DataGridViewDesigner owner) : base(owner.Component)
        {
            _owner = owner;
        }

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

    private class DataGridViewColumnEditingActionList : DesignerActionList
    {
        private readonly DataGridViewDesigner _owner;
        public DataGridViewColumnEditingActionList(DataGridViewDesigner owner) : base(owner.Component)
        {
            _owner = owner;
        }

        public override DesignerActionItemCollection GetSortedActionItems()
        {
            DesignerActionItemCollection items =
            [
                new DesignerActionMethodItem(this,
                    memberName: nameof(EditColumns),
                    displayName: SR.DataGridViewEditColumnsVerb,
                    includeAsDesignerVerb:true),
                new DesignerActionMethodItem(this,
                    memberName: nameof(AddColumn),
                    displayName: SR.DataGridViewAddColumnVerb,
                    includeAsDesignerVerb: true),
            ];

            return items;
        }

        public void EditColumns() => _owner.OnEditColumns(this, EventArgs.Empty);

        public void AddColumn() => _owner.OnAddColumn(this, EventArgs.Empty);
    }

    private class DataGridViewPropertiesActionList : DesignerActionList
    {
        private readonly DataGridViewDesigner _owner;

        public DataGridViewPropertiesActionList(DataGridViewDesigner owner) : base(owner.Component)
        {
            _owner = owner;
        }

        public override DesignerActionItemCollection GetSortedActionItems() =>
        [
            new DesignerActionPropertyItem(nameof(AllowUserToAddRows), SR.DataGridViewEnableAdding),
            new DesignerActionPropertyItem(nameof(ReadOnly), SR.DataGridViewEnableEditing),
            new DesignerActionPropertyItem(nameof(AllowUserToDeleteRows), SR.DataGridViewEnableDeleting),
            new DesignerActionPropertyItem(nameof(AllowUserToOrderColumns), SR.DataGridViewEnableColumnReordering)
        ];

        public bool AllowUserToAddRows
        {
            get => _owner.Control.AllowUserToAddRows;
            set
            {
                if (value == AllowUserToAddRows)
                {
                    return;
                }

                DataGridView dataGridView = _owner.Control;
                IDesignerHost? host = dataGridView.Site?.GetService<IDesignerHost>();

                DesignerTransaction? transaction;

                if (value)
                {
                    transaction = host?.CreateTransaction(SR.DataGridViewEnableAddingTransactionString);
                }
                else
                {
                    transaction = host?.CreateTransaction(SR.DataGridViewDisableAddingTransactionString);
                }

                try
                {
                    IComponentChangeService? changeService = dataGridView.Site?.GetService<IComponentChangeService>();
                    PropertyDescriptor? prop = TypeDescriptor.GetProperties(dataGridView)["AllowUserToAddRows"];
                    changeService?.OnComponentChanging(dataGridView, prop);

                    dataGridView.AllowUserToAddRows = value;

                    changeService?.OnComponentChanged(dataGridView, prop, null, null);
                    transaction?.Commit();
                    transaction = null;
                }
                finally
                {
                    transaction?.Cancel();
                }
            }
        }

        public bool AllowUserToDeleteRows
        {
            get => _owner.Control.AllowUserToDeleteRows;
            set
            {
                if (value == AllowUserToDeleteRows)
                {
                    return;
                }

                DataGridView dataGridView = _owner.Control;
                IDesignerHost? host = dataGridView.Site?.GetService<IDesignerHost>();

                DesignerTransaction? transaction;

                if (value)
                {
                    transaction = host?.CreateTransaction(SR.DataGridViewEnableDeletingTransactionString);
                }
                else
                {
                    transaction = host?.CreateTransaction(SR.DataGridViewDisableDeletingTransactionString);
                }

                try
                {
                    IComponentChangeService? changeService = dataGridView.Site?.GetService<IComponentChangeService>();
                    PropertyDescriptor? prop = TypeDescriptor.GetProperties(dataGridView)["AllowUserToDeleteRows"];
                    changeService?.OnComponentChanging(dataGridView, prop);

                    dataGridView.AllowUserToDeleteRows = value;

                    changeService?.OnComponentChanged(dataGridView, prop, null, null);
                    transaction?.Commit();
                    transaction = null;
                }
                finally
                {
                    transaction?.Cancel();
                }
            }
        }

        public bool AllowUserToOrderColumns
        {
            get => _owner.Control.AllowUserToOrderColumns;
            set
            {
                if (value == AllowUserToOrderColumns)
                {
                    return;
                }

                DataGridView dataGridView = _owner.Control;
                IDesignerHost? host = dataGridView.Site?.GetService<IDesignerHost>();

                DesignerTransaction? transaction;

                if (value)
                {
                    transaction = host?.CreateTransaction(SR.DataGridViewEnableColumnReorderingTransactionString);
                }
                else
                {
                    transaction = host?.CreateTransaction(SR.DataGridViewDisableColumnReorderingTransactionString);
                }

                try
                {
                    IComponentChangeService? changeService = dataGridView.Site?.GetService<IComponentChangeService>();
                    PropertyDescriptor? prop = TypeDescriptor.GetProperties(dataGridView)["AllowUserToReorderColumns"];
                    changeService?.OnComponentChanging(dataGridView, prop);

                    dataGridView.AllowUserToOrderColumns = value;

                    changeService?.OnComponentChanged(dataGridView, prop, null, null);
                    transaction?.Commit();
                    transaction = null;
                }
                finally
                {
                    transaction?.Cancel();
                }
            }
        }

        public bool ReadOnly
        {
            get => !_owner.Control.ReadOnly;
            set
            {
                if (value == ReadOnly)
                {
                    return;
                }

                DataGridView dataGridView = _owner.Control;
                IDesignerHost? host = dataGridView.Site?.GetService<IDesignerHost>();

                DesignerTransaction? transaction;

                if (value)
                {
                    transaction = host?.CreateTransaction(SR.DataGridViewEnableEditingTransactionString);
                }
                else
                {
                    transaction = host?.CreateTransaction(SR.DataGridViewDisableEditingTransactionString);
                }

                try
                {
                    IComponentChangeService? changeService = dataGridView.Site?.GetService<IComponentChangeService>();
                    PropertyDescriptor? prop = TypeDescriptor.GetProperties(dataGridView)["ReadOnly"];
                    changeService?.OnComponentChanging(dataGridView, prop);

                    dataGridView.ReadOnly = !value;

                    changeService?.OnComponentChanged(dataGridView, prop, null, null);
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
