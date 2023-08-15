// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.ComponentModel.Design.Serialization;
using System.ComponentModel.Design;
using System.ComponentModel;
using System.Windows.Forms.Design.Behavior;

namespace System.Windows.Forms.Design;
internal class DataGridViewColumnDesigner : ComponentDesigner
{
    private const int DATAGRIDVIEWCOLUMN_defaultWidth = 100;
    private bool userAddedColumn;
    private bool initializing;
    private BehaviorService? behaviorService;
    private ISelectionService? selectionService;
    private FilterCutCopyPasteDeleteBehavior? behavior;
    private bool behaviorPushed;
    private DataGridView? liveDataGridView;

    private string Name
    {
        get
        {
            DataGridViewColumn col = (DataGridViewColumn)this.Component;
            if (col.Site is not null)
            {
                return col.Site.Name;
            }
            else
            {
                return col.Name;
            }
        }

        set
        {
            value ??= string.Empty;

            DataGridViewColumn col = (DataGridViewColumn)this.Component;

            if (col is null)
            {
                return;
            }

            // Note: case sensitive because lookup inside DataGridViewColumnCollection is case sensitive.
            if (string.Compare(value, col.Name, false, System.Globalization.CultureInfo.InvariantCulture) == 0)
            {
                return;
            }

            DataGridView? dgv = col?.DataGridView;

            IDesignerHost? host = null;
            IContainer? container = null;
            INameCreationService? nameCreationService = null;

            if (dgv is not null && dgv.Site is not null)
            {
                host = dgv.Site.GetService(typeof(IDesignerHost)) as IDesignerHost;
                nameCreationService = dgv.Site.GetService(typeof(INameCreationService)) as INameCreationService;
            }

            if (host is not null)
            {
                container = host.Container;
            }

            // ValidName() checks any name conflicts on the DGV's column collection as well as any name conflicts
            // on the Container::Components collection.
            string errorString = string.Empty;
            if (dgv is not null &&
                !DataGridViewAddColumnDialog.ValidName(value,
                                                        dgv.Columns,
                                                        container,
                                                        nameCreationService,
                                                        this.liveDataGridView is not null ? this.liveDataGridView.Columns : null,
                                                        true /*allowDuplicateNameInLiveColumnCollection*/,
                                                        out errorString))
            {
                if (dgv is not null && dgv.Site is not null)
                {
                    IUIService? uiService = dgv.Site.GetService(typeof(IUIService)) as IUIService;
                    DataGridViewDesigner.ShowErrorDialog(uiService, errorString, this.liveDataGridView);
                }

                return;
            }

            // we are good.
            // Set the site name if the column is sited.
            // Then set the column name.
            if ((host is null || (host is not null && !host.Loading)) && this.Component.Site is not null)
            {
                Component.Site.Name = value;
            }

            col.Name = value;
        }
    }

    public DataGridView LiveDataGridView
    {
        set
        {
            this.liveDataGridView = value;
        }
    }

    /// <devdoc>
    ///     vsw 311922.
    ///     We want to add a design time only property which tracks if the user added this column or not.
    ///     Because this is a design time only property, it will be saved to *resx file.
    ///     Hence, its value will be saved from session to session.
    ///
    ///     The property is set in ONLY one place and is used in ONLY one place.
    ///
    ///     This property is set ONLY the user adds a column via the data grid view add column dialog.
    ///     If columns are added as a result of binding the data grid view to a data source or as a result
    ///     of meta data changes then they are not considered to be added by user.
    ///
    ///     This property is used ONLY by the data grid view designer when it has to decide whether or not to remove
    ///     a data bound property.
    /// </devdoc>
    private bool UserAddedColumn
    {
        get
        {
            return this.userAddedColumn;
        }
        set
        {
            this.userAddedColumn = value;
        }
    }

    private int Width
    {
        get
        {
            DataGridViewColumn col = (DataGridViewColumn)this.Component;
            return col.Width;
        }
        set
        {
            DataGridViewColumn col = (DataGridViewColumn)this.Component;
            value = Math.Max(col.MinimumWidth, value);
            col.Width = value;
        }
    }

    public override void Initialize(IComponent component)
    {
        initializing = true;
        base.Initialize(component);

        if (component.Site is not null)
        {
            // Acquire a reference to ISelectionService.
            this.selectionService =
                GetService(typeof(ISelectionService))
                as ISelectionService;
            Debug.Assert(selectionService is not null);

            // Acquire a reference to BehaviorService.
            this.behaviorService =
                GetService(typeof(BehaviorService))
                as BehaviorService;

            if (behaviorService is not null && selectionService is not null)
            {
                behavior = new FilterCutCopyPasteDeleteBehavior(true, behaviorService);

                UpdateBehavior();
                this.selectionService.SelectionChanged += new EventHandler(selectionService_SelectionChanged);
            }
        }

        initializing = false;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            PopBehavior();
            if (selectionService is not null)
            {
                this.selectionService.SelectionChanged -= new EventHandler(selectionService_SelectionChanged);
            }

            selectionService = null;
            behaviorService = null;
        }
    }

    private void PushBehavior()
    {
        if (!behaviorPushed)
        {
            Debug.Assert(behavior is not null);
            try
            {
                this.behaviorService?.PushBehavior(behavior);
            }
            finally
            {
                behaviorPushed = true;
            }
        }
    }

    private void PopBehavior()
    {
        if (behaviorPushed)
        {
            Debug.Assert(behavior is not null);
            try
            {
                this.behaviorService?.PopBehavior(behavior);
            }
            finally
            {
                behaviorPushed = false;
            }
        }
    }

    private void UpdateBehavior()
    {
        if (selectionService is not null)
        {
            if (selectionService.PrimarySelection is not null
                    && this.Component.Equals(selectionService.PrimarySelection))
            {
                PushBehavior();
            }
            else
            {
                //ensure we are popped off
                PopBehavior();
            }
        }
    }

    private void selectionService_SelectionChanged(object? sender, EventArgs e)
    {
        UpdateBehavior();
    }

    protected override void PreFilterProperties(IDictionary properties)
    {
        base.PreFilterProperties(properties);

        PropertyDescriptor? prop = properties["Width"] as PropertyDescriptor;
        if (prop is not null)
        {
            properties["Width"] = TypeDescriptor.CreateProperty(typeof(DataGridViewColumnDesigner), prop, new Attribute[0]);
        }

        prop = properties["Name"] as PropertyDescriptor;
        if (prop is not null)
        {
            if (this.Component.Site is null)
            {
                // When the Site is not null the Extender provider will add the (Name) property.
                // However, when columns are on the data grid view column collection editor, the site is null.
                // In that case we have to make the (Name) property browsable true.
                properties["Name"] = TypeDescriptor.CreateProperty(typeof(DataGridViewColumnDesigner),
                                     prop,
                                     BrowsableAttribute.Yes,
                                     CategoryAttribute.Design,
                                     new DescriptionAttribute(SR.DesignerPropName),
                                     new ParenthesizePropertyNameAttribute(true));
            }
            else
            {
                // When the column is sited use DataGridViewColumnDesigner::Name property
                // so it can use the Name validation logic specific to DataGridViewColumnCollection.
                properties["Name"] = TypeDescriptor.CreateProperty(typeof(DataGridViewColumnDesigner),
                                     prop,
                                     new ParenthesizePropertyNameAttribute(true));
            }
        }

        // Add the UserAddedColumn design time only property.
        properties["UserAddedColumn"] = TypeDescriptor.CreateProperty(typeof(DataGridViewColumnDesigner), "UserAddedColumn", typeof(bool),
                                        new DefaultValueAttribute(false),
                                        BrowsableAttribute.No,
                                        DesignOnlyAttribute.Yes);
    }

    private bool ShouldSerializeWidth()
    {
        DataGridViewColumn col = (DataGridViewColumn)this.Component;
        return col.InheritedAutoSizeMode != DataGridViewAutoSizeColumnMode.Fill && col.Width != DATAGRIDVIEWCOLUMN_defaultWidth;
    }

    private bool ShouldSerializeName()
    {
        IDesignerHost host = (IDesignerHost)GetService(typeof(IDesignerHost));
        if (host is null)
        {
            // The column is hosted in the column collection dialog.
            // Return false : let the user type whatever he feels like.
            return false;
        }

        return initializing ? (Component != host.RootComponent)  //for non root components, respect the name that the base Control serialized unless changed
            : ShadowProperties.ShouldSerializeValue("Name", null);
    }

    public class FilterCutCopyPasteDeleteBehavior : System.Windows.Forms.Design.Behavior.Behavior
    {
        public FilterCutCopyPasteDeleteBehavior(bool callParentBehavior, BehaviorService behaviorService) : base(callParentBehavior, behaviorService) { }

        public override MenuCommand? FindCommand(CommandID commandId)
        {
            MenuCommand command;
            if ((commandId.ID == StandardCommands.Copy.ID) && (commandId.Guid == StandardCommands.Copy.Guid))
            {
                command = new MenuCommand(new EventHandler(this.handler), StandardCommands.Copy);
                command.Enabled = false;

                return command;
            }

            if ((commandId.ID == StandardCommands.Paste.ID) && (commandId.Guid == StandardCommands.Paste.Guid))
            {
                command = new MenuCommand(new EventHandler(this.handler), StandardCommands.Paste);
                command.Enabled = false;

                return command;
            }

            if ((commandId.ID == StandardCommands.Delete.ID) && (commandId.Guid == StandardCommands.Delete.Guid))
            {
                command = new MenuCommand(new EventHandler(this.handler), StandardCommands.Delete);
                command.Enabled = false;

                return command;
            }

            if ((commandId.ID == StandardCommands.Cut.ID) && (commandId.Guid == StandardCommands.Cut.Guid))
            {
                command = new MenuCommand(new EventHandler(this.handler), StandardCommands.Cut);
                command.Enabled = false;

                return command;
            }

            return base.FindCommand(commandId);
        }

        private void handler(object? sender, EventArgs e) { }
    }
}
