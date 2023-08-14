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
    /// <include file='doc\TableDesigner.uex' path='docs/doc[@for="DataGridViewDesigner.designerVerbs"]/*' />
    /// <devdoc>
    ///    <para>Gets the design-time verbs suppoted by the component associated with the 
    ///       designer.</para>
    /// </devdoc>
    protected DesignerVerbCollection designerVerbs;

    // CHROME stuff
    //
    // DesignerActionLists
    DesignerActionListCollection actionLists = null;

    // need this to trap meta data changes
    CurrencyManager cm = null;

    // cache this type cause we will use it a lot
    private static Type typeofIList = typeof(IList);
    private static Type typeofDataGridViewImageColumn = typeof(DataGridViewImageColumn);
    private static Type typeofDataGridViewTextBoxColumn = typeof(DataGridViewTextBoxColumn);
    private static Type typeofDataGridViewCheckBoxColumn = typeof(DataGridViewCheckBoxColumn);

    /// <include file='doc\TableDesigner.uex' path='docs/doc[@for="DataGridViewDesigner.DataGridViewDesigner"]/*' />
    /// <devdoc>
    /// <para>Initializes a new instance of the <see cref='System.Windows.Forms.Design.DataGridViewDesigner'/> class.</para>
    /// </devdoc>
    public DataGridViewDesigner()
    {
        AutoResizeHandles = true;
    }

    // need AssociatedComponents for Copy / Paste
    public override ICollection AssociatedComponents
    {
        get
        {
            DataGridView dataGridView = this.Component as DataGridView;
            if (dataGridView != null)
            {
                return dataGridView.Columns;
            }

            return base.AssociatedComponents;
        }
    }

    public DataGridViewAutoSizeColumnsMode AutoSizeColumnsMode
    {
        get
        {
            DataGridView dataGridView = this.Component as DataGridView;
            Debug.Assert(dataGridView != null, "Unexpected null dataGridView in DataGridViewDesigner:get_AutoSizeColumnsMode");
            return dataGridView.AutoSizeColumnsMode;
        }
        set
        {
            // DANIELHE: this is a workaround for a bug in ComponentCache: if a component changed then its list of AssociatedComponents
            // is not marked as changed to the serialization engine will not serialize them.
            DataGridView dataGridView = this.Component as DataGridView;
            Debug.Assert(dataGridView != null, "Unexpected null dataGridView in DataGridViewDesigner:set_AutoSizeColumnsMode");
            IComponentChangeService ccs = this.Component.Site.GetService(typeof(IComponentChangeService)) as IComponentChangeService;
            PropertyDescriptor prop = TypeDescriptor.GetProperties(typeof(DataGridViewColumn))["Width"];

            for (int i = 0; i < dataGridView.Columns.Count; i++)
            {
                ccs.OnComponentChanging(dataGridView.Columns[i], prop);
            }

            dataGridView.AutoSizeColumnsMode = value;

            for (int i = 0; i < dataGridView.Columns.Count; i++)
            {
                ccs.OnComponentChanged(dataGridView.Columns[i], prop, null, null);
            }
        }
    }

    public object DataSource
    {
        get
        {
            return ((DataGridView)this.Component).DataSource;
        }
        set
        {
            DataGridView dgv = this.Component as DataGridView;
            if (dgv.AutoGenerateColumns && dgv.DataSource == null && value != null)
            {
                //
                // RefreshColumnCollection() method does the job of siting/unsiting DataGridViewColumns 
                // and calls OnComponentChanged/ing at the right times.
                // So we need to create / remove columns from the DataGridView DataSource via RefreshColumnCollection() method.
                // Set AutoGenerateColumn to false before setting the DataSource, otherwise DataGridViewColumns will be created
                // via the runtime method.
                //
                // We set AutoGenerateColumns to FALSE only if the DataGridView will get a not-null DataSource.
                // 
                dgv.AutoGenerateColumns = false;
            }

            ((DataGridView)this.Component).DataSource = value;
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            // we still need the current Component
            // so execute this code before base.Dispose(...)
            Debug.Assert(this.Component != null, "how did this get set to null?");
            DataGridView dataGridView = this.Component as DataGridView;

            // unhook our event handlers
            dataGridView.DataSourceChanged -= new EventHandler(dataGridViewChanged);
            dataGridView.DataMemberChanged -= new EventHandler(dataGridViewChanged);
            dataGridView.BindingContextChanged -= new EventHandler(dataGridViewChanged);
            dataGridView.ColumnRemoved -= new DataGridViewColumnEventHandler(dataGridView_ColumnRemoved);

            // unhook MetaDataChanged handler from the currency manager
            if (this.cm != null)
            {
                cm.MetaDataChanged -= new EventHandler(this.dataGridViewMetaDataChanged);
            }

            this.cm = null;

            // unhook the ComponentRemoved event handler
            if (this.Component.Site != null)
            {
                IComponentChangeService ccs = this.Component.Site.GetService(typeof(IComponentChangeService)) as IComponentChangeService;
                if (ccs != null)
                {
                    ccs.ComponentRemoving -= new ComponentEventHandler(DataGridViewDesigner_ComponentRemoving);
                }
            }
        }

        base.Dispose(disposing);
    }

    public override void Initialize(IComponent component)
    {
        base.Initialize(component);

        if (component.Site != null)
        {
            IComponentChangeService ccs = component.Site.GetService(typeof(IComponentChangeService)) as IComponentChangeService;
            if (ccs != null)
            {
                ccs.ComponentRemoving += new ComponentEventHandler(DataGridViewDesigner_ComponentRemoving);
            }
        }

        DataGridView dataGridView = (DataGridView)component;

        // DataGridViewDesigner::Initialize runs after InitializeComponent was deserialized.
        // just in case the user tinkered w/ InitializeComponent set AutoGenerateColumns to TRUE if there is no DataSource, otherwise set it to FALSE
        dataGridView.AutoGenerateColumns = (dataGridView.DataSource == null);

        // hook up the DataSourceChanged event, DataMemberChanged event
        dataGridView.DataSourceChanged += new EventHandler(dataGridViewChanged);
        dataGridView.DataMemberChanged += new EventHandler(dataGridViewChanged);
        dataGridView.BindingContextChanged += new EventHandler(dataGridViewChanged);

        // now add data bound columns
        this.dataGridViewChanged(this.Component, EventArgs.Empty);

        // Attach to column removed for clean up
        dataGridView.ColumnRemoved += new DataGridViewColumnEventHandler(dataGridView_ColumnRemoved);
    }

    public override void InitializeNewComponent(IDictionary defaultValues)
    {
        base.InitializeNewComponent(defaultValues);
        ((DataGridView)this.Component).ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
    }


    /// <devdoc>
    ///     <para>
    ///         Gets the inheritance attribute for the data grid view.
    ///     </para>
    /// </devdoc>
    protected override InheritanceAttribute InheritanceAttribute
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

    /// <include file='doc\TableDesigner.uex' path='docs/doc[@for="DataGridViewDesigner.Verbs"]/*' />
    /// <devdoc>
    ///    <para>Gets the design-time verbs supported by the component associated with the 
    ///       designer.</para>
    /// </devdoc>
    public override DesignerVerbCollection Verbs
    {
        get
        {
            if (designerVerbs == null)
            {
                designerVerbs = new DesignerVerbCollection();
                designerVerbs.Add(new DesignerVerb((SR.DataGridViewEditColumnsVerb), new EventHandler(this.OnEditColumns)));
                designerVerbs.Add(new DesignerVerb((SR.DataGridViewAddColumnVerb), new EventHandler(this.OnAddColumn)));
            }

            return designerVerbs;
        }
    }

    /// <include file='doc\TableDesigner.uex' path='docs/doc[@for="DataGridViewDesigner.ActionLists"]/*' />
    /// <devdoc>
    ///    <para>Gets the design-time action lists supported by the component associated with the 
    ///       designer.</para>
    /// </devdoc>
    public override DesignerActionListCollection ActionLists
    {
        get
        {
            if (actionLists == null)
            {
                BuildActionLists();
            }

            return actionLists;
        }
    }

    private void BuildActionLists()
    {
        actionLists = new DesignerActionListCollection();

        // ChooseDataSource action list
        actionLists.Add(new DataGridViewChooseDataSourceActionList(this));

        // column collection editing
        actionLists.Add(new DataGridViewColumnEditingActionList(this));

        // AllowUserToAddRows / ReadOnly / AllowUserToDeleteRows / AllowUserToOrderColumns
        actionLists.Add(new DataGridViewPropertiesActionList(this));

        // if one actionList has AutoShow == true then the chrome panel will popup when the user DnD the DataGridView onto the form
        // It would make sense to promote AutoShow to DesignerActionListCollection.
        // But we don't own the DesignerActionListCollection so we just set AutoShow on the first ActionList
        //
        actionLists[0].AutoShow = true;
    }

    private void dataGridViewChanged(object sender, EventArgs e)
    {
        DataGridView dataGridView = (DataGridView)this.Component;

        CurrencyManager newCM = null;
        if (dataGridView.DataSource != null && dataGridView.BindingContext != null)
        {
            newCM = (CurrencyManager)dataGridView.BindingContext[dataGridView.DataSource, dataGridView.DataMember];
        }

        if (newCM != this.cm)
        {
            // unwire cm
            if (this.cm != null)
            {
                this.cm.MetaDataChanged -= new EventHandler(this.dataGridViewMetaDataChanged);
            }

            cm = newCM;

            // wire cm
            if (this.cm != null)
            {
                this.cm.MetaDataChanged += new EventHandler(this.dataGridViewMetaDataChanged);
            }
        }

        if (dataGridView.BindingContext == null)
        {
            MakeSureColumnsAreSited(dataGridView);
            return;
        }

        // the user changed the DataSource from null to something else
        // Because AutoGenerateColumns is already set to true, all the bound columns are created
        // Just set AutoGenerateColumns to false and return
        if (dataGridView.AutoGenerateColumns && dataGridView.DataSource != null)
        {
            Debug.Assert(String.IsNullOrEmpty(dataGridView.DataMember), "in the designer we can't set DataSource and DataMember at the same time. Did you forget to set AutoGenerateColumns to false at a previous stage?");
            dataGridView.AutoGenerateColumns = false;
            MakeSureColumnsAreSited(dataGridView);
            return;
        }

        if (dataGridView.DataSource == null)
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

    private void DataGridViewDesigner_ComponentRemoving(object sender, ComponentEventArgs e)
    {
        DataGridView dataGridView = this.Component as DataGridView;

        if (e.Component != null && e.Component == dataGridView.DataSource)
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

            IComponentChangeService ccs = (IComponentChangeService)GetService(typeof(IComponentChangeService));
            string previousDataMember = dataGridView.DataMember;

            PropertyDescriptorCollection props = TypeDescriptor.GetProperties(dataGridView);
            PropertyDescriptor dmPD = props != null ? props["DataMember"] : null;

            if (ccs != null)
            {
                if (dmPD != null)
                {
                    ccs.OnComponentChanging(dataGridView, dmPD);
                }
            }

            dataGridView.DataSource = null;

            if (ccs != null)
            {
                if (dmPD != null)
                {
                    ccs.OnComponentChanged(dataGridView, dmPD, previousDataMember, "");
                }
            }
        }
    }

    private void dataGridView_ColumnRemoved(object sender, DataGridViewColumnEventArgs e)
    {
        // Clear out the DisplayIndex for columns removed at design time.
        // If we don't do this, the columns will be reordered after build when there are both databound
        // columns and non databound columns.  See VSWhidbey 572859. Ideally, we would do this on the Clear
        // of the column collection but there is no event there and we are restricted from adding any
        // Note: this does not break Undo/Redo because Undo/Redo uses serialization the same way
        // the build does and DisplayIndex is not serialized
        if (e.Column != null && e.Column.IsDataBound == false)
            e.Column.DisplayIndex = -1;
    }

    private static void MakeSureColumnsAreSited(DataGridView dataGridView)
    {
        // Other designers ( VS Data ) may manipulate the column collection but not add the columns to the
        // current container.
        // So we have to make sure that the data grid view columns are sited on the current container.
        IContainer currentContainer = dataGridView.Site != null ? dataGridView.Site.Container : null;
        for (int i = 0; i < dataGridView.Columns.Count; i++)
        {
            DataGridViewColumn col = dataGridView.Columns[i];
            IContainer cont = col.Site != null ? col.Site.Container : null;
            if (currentContainer != cont)
            {
                if (cont != null)
                {
                    cont.Remove(col);
                }

                if (currentContainer != null)
                {
                    currentContainer.Add(col);
                }
            }
        }
    }

    protected override void PreFilterProperties(IDictionary properties)
    {
        base.PreFilterProperties(properties);

        PropertyDescriptor prop;

        // Handle shadowed properties
        //
        string[] shadowProps = new string[]
        {
            "AutoSizeColumnsMode",
            "DataSource"
        };

        Attribute[] empty = new Attribute[0];

        for (int i = 0; i < shadowProps.Length; i++)
        {
            prop = (PropertyDescriptor)properties[shadowProps[i]];
            if (prop != null)
            {
                properties[shadowProps[i]] = TypeDescriptor.CreateProperty(typeof(DataGridViewDesigner), prop, empty);
            }
        }
    }

    //
    // If the previous schema and the current schema have common columns then ProcessSimilarSchema does the
    // work of removing columns which are not common and returns TRUE.
    // Otherwise ProcessSimilarSchema returns FALSE.
    //
    [SuppressMessage("Microsoft.Performance", "CA1808:AvoidCallsThatBoxValueTypes")]
    private bool ProcessSimilarSchema(DataGridView dataGridView)
    {
        Debug.Assert(dataGridView.DataSource == null || this.cm != null, "if we have a data source we should also have a currency manager by now");
        PropertyDescriptorCollection backEndProps = null;

        if (cm != null)
        {
            try
            {
                backEndProps = cm.GetItemProperties();
            }
            catch (ArgumentException ex)
            {
                throw new InvalidOperationException((SR.DataGridViewDataSourceNoLongerValid), ex);
            }

        }

        IContainer currentContainer = dataGridView.Site != null ? dataGridView.Site.Container : null;

        bool similarSchema = false;

        for (int i = 0; i < dataGridView.Columns.Count; i++)
        {

            DataGridViewColumn dataGridViewColumn = dataGridView.Columns[i];

            if (String.IsNullOrEmpty(dataGridViewColumn.DataPropertyName))
            {
                continue;
            }

            PropertyDescriptor pd = TypeDescriptor.GetProperties(dataGridViewColumn)["UserAddedColumn"];
            if (pd != null && (bool)pd.GetValue(dataGridViewColumn))
            {
                continue;
            }

            PropertyDescriptor dataFieldProperty = (backEndProps != null ? backEndProps[dataGridViewColumn.DataPropertyName] : null);
            // remove the column if the DataGridViewColumn::DataPropertyName does not map to any column anymore
            // or if the DataGridViewColumn::DataPropertyName maps to a sub list
            bool removeColumn = false;

            if (dataFieldProperty == null)
            {
                // DataPropertyName does not map to a column anymore
                removeColumn = true;
            }
            else if (typeofIList.IsAssignableFrom(dataFieldProperty.PropertyType))
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
            IComponentChangeService changeService = this.Component.Site.GetService(typeof(IComponentChangeService)) as IComponentChangeService;
            PropertyDescriptor columnsProp = TypeDescriptor.GetProperties(this.Component)["Columns"];
            try
            {
                changeService.OnComponentChanging(this.Component, columnsProp);
            }
            catch (InvalidOperationException)
            {
                return similarSchema;
            }

            for (int i = 0; i < dataGridView.Columns.Count;)
            {
                DataGridViewColumn dataGridViewColumn = dataGridView.Columns[i];

                if (String.IsNullOrEmpty(dataGridViewColumn.DataPropertyName))
                {
                    i++;
                    continue;
                }

                PropertyDescriptor pd = TypeDescriptor.GetProperties(dataGridViewColumn)["UserAddedColumn"];
                if (pd != null && (bool)pd.GetValue(dataGridViewColumn))
                {
                    i++;
                    continue;
                }

                PropertyDescriptor dataFieldProperty = (backEndProps != null ? backEndProps[dataGridViewColumn.DataPropertyName] : null);
                // remove the column if the DataGridViewColumn::DataPropertyName does not map to any column anymore
                // or if the DataGridViewColumn::DataPropertyName maps to a sub list
                bool removeColumn = false;

                if (dataFieldProperty == null)
                {
                    // DataPropertyName does not map to a column anymore
                    removeColumn = true;
                }
                else if (typeofIList.IsAssignableFrom(dataFieldProperty.PropertyType))
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
                    if (currentContainer != null)
                    {
                        currentContainer.Remove(dataGridViewColumn);
                    }
                }
                else
                {
                    i++;
                }
            }

            changeService.OnComponentChanged(this.Component, columnsProp, null, null);
        }

        return similarSchema;
    }

    [SuppressMessage("Microsoft.Performance", "CA1808:AvoidCallsThatBoxValueTypes")]
    private void RefreshColumnCollection()
    {
        DataGridView dataGridView = (DataGridView)this.Component;

        ISupportInitializeNotification ds = dataGridView.DataSource as ISupportInitializeNotification;
        if (ds != null && !ds.IsInitialized)
        {
            // The DataSource is not initialized yet.
            // When the dataSource gets initialized it will send a MetaDataChanged event and at
            // that point we will refresh the column collection.
            return;
        }

        IComponentChangeService changeService = null;
        PropertyDescriptor columnsProp = null;
        IDesignerHost host = this.Component.Site.GetService(typeof(IDesignerHost)) as IDesignerHost;

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
        //

        Debug.Assert(dataGridView.DataSource == null || this.cm != null, "if we have a data source we should also have a currency manager by now");
        PropertyDescriptorCollection backEndProps = null;

        if (cm != null)
        {
            try
            {
                backEndProps = cm.GetItemProperties();
            }
            catch (ArgumentException ex)
            {
                throw new InvalidOperationException((SR.DataGridViewDataSourceNoLongerValid), ex);
            }

        }

        IContainer currentContainer = dataGridView.Site != null ? dataGridView.Site.Container : null;
        changeService = this.Component.Site.GetService(typeof(IComponentChangeService)) as IComponentChangeService;
        columnsProp = TypeDescriptor.GetProperties(this.Component)["Columns"];

        // 1. OnComponentChanging DataGridView.Columns
        changeService.OnComponentChanging(this.Component, columnsProp);

        DataGridViewColumn[] removeColumns = new DataGridViewColumn[dataGridView.Columns.Count];
        int removeColumnsCount = 0;
        for (int i = 0; i < dataGridView.Columns.Count; i++)
        {
            DataGridViewColumn col = dataGridView.Columns[i];
            if (!String.IsNullOrEmpty(col.DataPropertyName))
            {
                PropertyDescriptor pd = TypeDescriptor.GetProperties(col)["UserAddedColumn"];
                if (pd == null || !((bool)pd.GetValue(col)))
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
        changeService.OnComponentChanged(this.Component, columnsProp, null, null);

        // 4. IContainer.Remove(dataGridView.Columns)
        if (currentContainer != null)
        {
            for (int i = 0; i < removeColumnsCount; i++)
            {
                currentContainer.Remove(removeColumns[i]);
            }
        }

        DataGridViewColumn[] columnsToBeAdded = null;
        int columnsToBeAddedCount = 0;
        if (dataGridView.DataSource != null)
        {
            columnsToBeAdded = new DataGridViewColumn[backEndProps.Count];
            columnsToBeAddedCount = 0;
            for (int i = 0; i < backEndProps.Count; i++)
            {
                DataGridViewColumn dataGridViewColumn = null;

                TypeConverter imageTypeConverter = TypeDescriptor.GetConverter(typeof(System.Drawing.Image));

                Type propType = backEndProps[i].PropertyType;

                Type columnType;

                if (typeof(IList).IsAssignableFrom(propType))
                {
                    if (imageTypeConverter.CanConvertFrom(propType))
                    {
                        columnType = typeofDataGridViewImageColumn;
                    }
                    else
                    {
                        continue;
                    }
                }
                else if (propType == typeof(bool) || propType == typeof(CheckState))
                {
                    columnType = typeofDataGridViewCheckBoxColumn;
                }
                else if (typeof(System.Drawing.Image).IsAssignableFrom(propType) || imageTypeConverter.CanConvertFrom(propType))
                {
                    columnType = typeofDataGridViewImageColumn;
                }
                else
                {
                    columnType = typeofDataGridViewTextBoxColumn;
                }

                string nameFromText = ToolStripDesigner.NameFromText(backEndProps[i].Name, columnType, this.Component.Site);

                //
                // host.CreateComponent adds the column to its list of components
                //
                // 5. IContainer.Add(newDataGridView.Columns
                //
                dataGridViewColumn = TypeDescriptor.CreateInstance(host, columnType, null, null) as DataGridViewColumn;

                dataGridViewColumn.DataPropertyName = backEndProps[i].Name;
                dataGridViewColumn.HeaderText = !String.IsNullOrEmpty(backEndProps[i].DisplayName) ? backEndProps[i].DisplayName : backEndProps[i].Name;
                dataGridViewColumn.Name = backEndProps[i].Name;
                dataGridViewColumn.ValueType = backEndProps[i].PropertyType;
                dataGridViewColumn.ReadOnly = backEndProps[i].IsReadOnly;

                host.Container.Add(dataGridViewColumn, nameFromText);

                columnsToBeAdded[columnsToBeAddedCount] = dataGridViewColumn;
                columnsToBeAddedCount++;
            }
        }

        // 6. OnComponentChanging DataGridView.Columns
        changeService.OnComponentChanging(this.Component, columnsProp);

        // 7. DataGridView.Columns.Add( new DataGridViewColumns)
        for (int i = 0; i < columnsToBeAddedCount; i++)
        {
            // wipe out the display index
            columnsToBeAdded[i].DisplayIndex = -1;
            dataGridView.Columns.Add(columnsToBeAdded[i]);
        }

        // 8. OnComponentChanged DataGridView.Columns
        changeService.OnComponentChanged(this.Component, columnsProp, null, null);
    }

    private bool ShouldSerializeAutoSizeColumnsMode()
    {
        DataGridView dataGridView = this.Component as DataGridView;
        if (dataGridView != null)
        {
            return dataGridView.AutoSizeColumnsMode != DataGridViewAutoSizeColumnsMode.None;
        }
        else
        {
            return false;
        }
    }

    private bool ShouldSerializeDataSource()
    {
        return ((DataGridView)this.Component).DataSource != null;
    }

    // Ideally there would be a public method like this somewhere.
    internal static void ShowErrorDialog(IUIService uiService, Exception ex, Control dataGridView)
    {
        if (uiService != null)
        {
            uiService.ShowError(ex);
        }
        else
        {
            string message = ex.Message;
            if (message == null || message.Length == 0)
            {
                message = ex.ToString();
            }

            RTLAwareMessageBox.Show(dataGridView, message, null, MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                    MessageBoxDefaultButton.Button1, 0);
        }
    }

    internal static void ShowErrorDialog(IUIService uiService, string errorString, Control dataGridView)
    {
        if (uiService != null)
        {
            uiService.ShowError(errorString);
        }
        else
        {
            RTLAwareMessageBox.Show(dataGridView, errorString, null, MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                    MessageBoxDefaultButton.Button1, 0);
        }
    }

    private void dataGridViewMetaDataChanged(object sender, EventArgs e)
    {
        RefreshColumnCollection();
    }

    public void OnEditColumns(object sender, EventArgs e)
    {
        IDesignerHost host = Component.Site.GetService(typeof(IDesignerHost)) as IDesignerHost;

        // child modal dialog -launching in System Aware mode
        DataGridViewColumnCollectionDialog dialog = DpiHelper.CreateInstanceInSystemAwareContext(() => new DataGridViewColumnCollectionDialog(((DataGridView)this.Component).Site));
        dialog.SetLiveDataGridView((DataGridView)this.Component);
        DesignerTransaction transaction = host.CreateTransaction((SR.DataGridViewEditColumnsTransactionString));
        DialogResult result = DialogResult.Cancel;

        try
        {
            result = this.ShowDialog(dialog);
        }
        finally
        {
            if (result == DialogResult.OK)
            {
                transaction.Commit();
            }
            else
            {
                transaction.Cancel();
            }
        }
    }

    public void OnAddColumn(object sender, EventArgs e)
    {
        IDesignerHost host = Component.Site.GetService(typeof(IDesignerHost)) as IDesignerHost;
        DesignerTransaction transaction = host.CreateTransaction((SR.DataGridViewAddColumnTransactionString));
        DialogResult result = DialogResult.Cancel;

        // child modal dialog -launching in System Aware mode
        DataGridViewAddColumnDialog dialog = DpiHelper.CreateInstanceInSystemAwareContext(() => new DataGridViewAddColumnDialog(((DataGridView)this.Component).Columns, (DataGridView)this.Component));
        dialog.Start(((DataGridView)this.Component).Columns.Count, true /*persistChangesToDesigner*/);

        try
        {
            result = this.ShowDialog(dialog);
        }
        finally
        {
            if (result == DialogResult.OK)
            {
                transaction.Commit();
            }
            else
            {
                transaction.Cancel();
            }
        }
    }

    private DialogResult ShowDialog(Form dialog)
    {
        IUIService uiSvc = this.Component.Site.GetService(typeof(IUIService)) as IUIService;
        if (uiSvc != null)
        {
            return uiSvc.ShowDialog(dialog);
        }
        else
        {
            return dialog.ShowDialog(this.Component as IWin32Window);
        }
    }

    [ComplexBindingProperties("DataSource", "DataMember")]
    private class DataGridViewChooseDataSourceActionList : DesignerActionList
    {
        DataGridViewDesigner owner;

        public DataGridViewChooseDataSourceActionList(DataGridViewDesigner owner) : base(owner.Component)
        {
            this.owner = owner;
        }

        public override DesignerActionItemCollection GetSortedActionItems()
        {
            DesignerActionItemCollection items = new DesignerActionItemCollection();
            DesignerActionPropertyItem chooseDataSource = new DesignerActionPropertyItem("DataSource",                          // property name
                                                                                         (SR.DataGridViewChooseDataSource));// displayName
            chooseDataSource.RelatedComponent = this.owner.Component;
            items.Add(chooseDataSource);
            return items;
        }

        [AttributeProvider(typeof(IListSource))]
        [Editor($"System.Windows.Forms.Design.DataSourceListEditor, {AssemblyRef.SystemDesign}", typeof(UITypeEditor))]
        public object DataSource
        {
            [
                SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode") // DAP calls this method thru Reflection.
            ]
            get
            {
                // Use the shadow property which is defined on the designer.
                return this.owner.DataSource;
            }

            [
                SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode") // DAP calls this method thru Reflection.
            ]
            set
            {
                // left to do: transaction stuff
                DataGridView dataGridView = (DataGridView)this.owner.Component;
                IDesignerHost host = this.owner.Component.Site.GetService(typeof(IDesignerHost)) as IDesignerHost;
                PropertyDescriptor dataSourceProp = TypeDescriptor.GetProperties(dataGridView)["DataSource"];
                IComponentChangeService changeService = this.owner.Component.Site.GetService(typeof(IComponentChangeService)) as IComponentChangeService;
                DesignerTransaction transaction = host.CreateTransaction(string.Format(SR.DataGridViewChooseDataSourceTransactionString, dataGridView.Name));
                try
                {
                    changeService.OnComponentChanging(this.owner.Component, dataSourceProp);
                    // Use the shadow property which is defined on the designer.
                    this.owner.DataSource = value;
                    changeService.OnComponentChanged(this.owner.Component, dataSourceProp, null, null);
                    transaction.Commit();
                    transaction = null;
                }
                finally
                {
                    if (transaction != null)
                    {
                        transaction.Cancel();
                    }
                }
            }
        }
    }

    private class DataGridViewColumnEditingActionList : DesignerActionList
    {
        DataGridViewDesigner owner;
        public DataGridViewColumnEditingActionList(DataGridViewDesigner owner) : base(owner.Component)
        {
            this.owner = owner;
        }

        public override DesignerActionItemCollection GetSortedActionItems()
        {
            DesignerActionItemCollection items = new DesignerActionItemCollection();
            items.Add(new DesignerActionMethodItem(this,
                                                                                "EditColumns",                                  // method name
                                                                                (SR.DataGridViewEditColumnsVerb),   // display name
                                                                                true));                                          // promoteToDesignerVerb
            items.Add(new DesignerActionMethodItem(this,
                                                                               "AddColumn",                                     // method name
                                                                               (SR.DataGridViewAddColumnVerb),      // display name
                                                                               true));                                           // promoteToDesignerVerb

            return items;
        }

        [
            SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode") // DAP calls this method thru Reflection.
        ]
        public void EditColumns()
        {
            this.owner.OnEditColumns(this, EventArgs.Empty);
        }

        [
            SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode") // DAP calls this method thru Reflection.
        ]
        public void AddColumn()
        {
            this.owner.OnAddColumn(this, EventArgs.Empty);
        }
    }

    private class DataGridViewPropertiesActionList : DesignerActionList
    {
        DataGridViewDesigner owner;

        public DataGridViewPropertiesActionList(DataGridViewDesigner owner) : base(owner.Component)
        {
            this.owner = owner;
        }

        public override DesignerActionItemCollection GetSortedActionItems()
        {
            DesignerActionItemCollection items = new DesignerActionItemCollection();
            items.Add(new DesignerActionPropertyItem("AllowUserToAddRows",
                                                                                     (SR.DataGridViewEnableAdding)));
            items.Add(new DesignerActionPropertyItem("ReadOnly",
                                                        (SR.DataGridViewEnableEditing)));
            items.Add(new DesignerActionPropertyItem("AllowUserToDeleteRows",
                                                        (SR.DataGridViewEnableDeleting)));
            items.Add(new DesignerActionPropertyItem("AllowUserToOrderColumns",
                                                        (SR.DataGridViewEnableColumnReordering)));
            return items;
        }

        public bool AllowUserToAddRows
        {
            [
                SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode") // DAP calls this method thru Reflection.
            ]
            get
            {
                return ((DataGridView)this.owner.Component).AllowUserToAddRows;
            }

            [
                SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode") // DAP calls this method thru Reflection.
            ]
            set
            {
                if (value != this.AllowUserToAddRows)
                {
                    IDesignerHost host = this.owner.Component.Site.GetService(typeof(IDesignerHost)) as IDesignerHost;

                    DesignerTransaction transaction;

                    if (value)
                    {
                        transaction = host.CreateTransaction((SR.DataGridViewEnableAddingTransactionString));
                    }
                    else
                    {
                        transaction = host.CreateTransaction((SR.DataGridViewDisableAddingTransactionString));
                    }

                    try
                    {
                        IComponentChangeService changeService = this.owner.Component.Site.GetService(typeof(IComponentChangeService)) as IComponentChangeService;
                        PropertyDescriptor prop = TypeDescriptor.GetProperties(this.owner.Component)["AllowUserToAddRows"];
                        changeService.OnComponentChanging(this.owner.Component, prop);

                        ((DataGridView)this.owner.Component).AllowUserToAddRows = value;

                        changeService.OnComponentChanged(this.owner.Component, prop, null, null);
                        transaction.Commit();
                        transaction = null;
                    }
                    finally
                    {
                        if (transaction != null)
                        {
                            transaction.Cancel();
                        }
                    }

                }
            }
        }

        public bool AllowUserToDeleteRows
        {
            [
                SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode") // DAP calls this method thru Reflection.
            ]
            get
            {
                return ((DataGridView)this.owner.Component).AllowUserToDeleteRows;
            }

            [
                SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode") // DAP calls this method thru Reflection.
            ]
            set
            {
                if (value != this.AllowUserToDeleteRows)
                {
                    IDesignerHost host = this.owner.Component.Site.GetService(typeof(IDesignerHost)) as IDesignerHost;

                    DesignerTransaction transaction;

                    if (value)
                    {
                        transaction = host.CreateTransaction((SR.DataGridViewEnableDeletingTransactionString));
                    }
                    else
                    {
                        transaction = host.CreateTransaction((SR.DataGridViewDisableDeletingTransactionString));
                    }

                    try
                    {
                        IComponentChangeService changeService = this.owner.Component.Site.GetService(typeof(IComponentChangeService)) as IComponentChangeService;
                        PropertyDescriptor prop = TypeDescriptor.GetProperties(this.owner.Component)["AllowUserToDeleteRows"];
                        changeService.OnComponentChanging(this.owner.Component, prop);

                        ((DataGridView)this.owner.Component).AllowUserToDeleteRows = value;

                        changeService.OnComponentChanged(this.owner.Component, prop, null, null);
                        transaction.Commit();
                        transaction = null;
                    }
                    finally
                    {
                        if (transaction != null)
                        {
                            transaction.Cancel();
                        }
                    }

                }
            }
        }

        public bool AllowUserToOrderColumns
        {
            [
                SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode") // DAP calls this method thru Reflection.
            ]
            get
            {
                return ((DataGridView)this.owner.Component).AllowUserToOrderColumns;
            }

            [
                SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode") // DAP calls this method thru Reflection.
            ]
            set
            {
                if (value != this.AllowUserToOrderColumns)
                {
                    IDesignerHost host = this.owner.Component.Site.GetService(typeof(IDesignerHost)) as IDesignerHost;

                    DesignerTransaction transaction;

                    if (value)
                    {
                        transaction = host.CreateTransaction((SR.DataGridViewEnableColumnReorderingTransactionString));
                    }
                    else
                    {
                        transaction = host.CreateTransaction((SR.DataGridViewDisableColumnReorderingTransactionString));
                    }

                    try
                    {
                        IComponentChangeService changeService = this.owner.Component.Site.GetService(typeof(IComponentChangeService)) as IComponentChangeService;
                        PropertyDescriptor prop = TypeDescriptor.GetProperties(this.owner.Component)["AllowUserToReorderColumns"];
                        changeService.OnComponentChanging(this.owner.Component, prop);

                        ((DataGridView)this.owner.Component).AllowUserToOrderColumns = value;

                        changeService.OnComponentChanged(this.owner.Component, prop, null, null);
                        transaction.Commit();
                        transaction = null;
                    }
                    finally
                    {
                        if (transaction != null)
                        {
                            transaction.Cancel();
                        }
                    }

                }
            }
        }

        public bool ReadOnly
        {
            [
                SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode") // DAP calls this method thru Reflection.
            ]
            get
            {
                return !((DataGridView)this.owner.Component).ReadOnly;
            }

            [
                SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode") // DAP calls this method thru Reflection.
            ]
            set
            {
                if (value != this.ReadOnly)
                {
                    IDesignerHost host = this.owner.Component.Site.GetService(typeof(IDesignerHost)) as IDesignerHost;

                    DesignerTransaction transaction;

                    if (value)
                    {
                        transaction = host.CreateTransaction((SR.DataGridViewEnableEditingTransactionString));
                    }
                    else
                    {
                        transaction = host.CreateTransaction((SR.DataGridViewDisableEditingTransactionString));
                    }

                    try
                    {
                        IComponentChangeService changeService = this.owner.Component.Site.GetService(typeof(IComponentChangeService)) as IComponentChangeService;
                        PropertyDescriptor prop = TypeDescriptor.GetProperties(this.owner.Component)["ReadOnly"];
                        changeService.OnComponentChanging(this.owner.Component, prop);

                        ((DataGridView)this.owner.Component).ReadOnly = !value;

                        changeService.OnComponentChanged(this.owner.Component, prop, null, null);
                        transaction.Commit();
                        transaction = null;
                    }
                    finally
                    {
                        if (transaction != null)
                        {
                            transaction.Cancel();
                        }
                    }

                }
            }
        }
    }
}
