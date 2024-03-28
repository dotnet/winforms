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
    private bool _userAddedColumn;
    private bool _initializing;
    private BehaviorService? _behaviorService;
    private ISelectionService? _selectionService;
    private FilterCutCopyPasteDeleteBehavior? _behavior;
    private bool _behaviorPushed;
    private DataGridView? _liveDataGridView;

    private string? Name
    {
        get
        {
            DataGridViewColumn col = (DataGridViewColumn)Component;
            return col.Site is not null ? col.Site.Name : col.Name;
        }

        set
        {
            value ??= string.Empty;

            DataGridViewColumn? col = (DataGridViewColumn)Component;

            if (col is null)
            {
                return;
            }

            // Note: case sensitive because lookup inside DataGridViewColumnCollection is case sensitive.
            if (string.Compare(value, col.Name, false, Globalization.CultureInfo.InvariantCulture) == 0)
            {
                return;
            }

            DataGridView? dataGridView = col.DataGridView;

            IDesignerHost? host = null;
            INameCreationService? nameCreationService = null;

            if (dataGridView is not null && dataGridView.Site is not null)
            {
                host = dataGridView.Site.GetService<IDesignerHost>();
                nameCreationService = dataGridView.Site.GetService<INameCreationService>();
            }

            IContainer? container = host?.Container;

            // ValidName() checks any name conflicts on the DGV's column collection as well as any name conflicts
            // on the Container::Components collection.
            if (dataGridView is not null &&
                !DataGridViewAddColumnDialog.ValidName(value,
                    dataGridView.Columns,
                    container,
                    nameCreationService,
                    _liveDataGridView?.Columns,
                    true,
                    out string errorString))
            {
                if (dataGridView is not null && dataGridView.Site is not null)
                {
                    IUIService? uiService = dataGridView.Site.GetService<IUIService>();
                    DataGridViewDesigner.ShowErrorDialog(uiService, errorString, _liveDataGridView);
                }

                return;
            }

            // we are good.
            // Set the site name if the column is sited.
            // Then set the column name.
            if ((host is null || (host is not null && !host.Loading)) && Component.Site is not null)
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
            _liveDataGridView = value;
        }
    }

    /// <devdoc>
    ///  vsw 311922.
    ///  We want to add a design time only property which tracks if the user added this column or not.
    ///  Because this is a design time only property, it will be saved to *resx file.
    ///  Hence, its value will be saved from session to session.
    ///
    ///  The property is set in ONLY one place and is used in ONLY one place.
    ///
    ///  This property is set ONLY the user adds a column via the data grid view add column dialog.
    ///  If columns are added as a result of binding the data grid view to a data source or as a result
    ///  of meta data changes then they are not considered to be added by user.
    ///
    ///  This property is used ONLY by the data grid view designer when it has to decide whether or not to remove
    ///  a data bound property.
    /// </devdoc>
    private bool UserAddedColumn
    {
        get => _userAddedColumn;
        set => _userAddedColumn = value;
    }

    private int Width
    {
        get
        {
            DataGridViewColumn col = (DataGridViewColumn)Component;
            return col.Width;
        }
        set
        {
            DataGridViewColumn col = (DataGridViewColumn)Component;
            value = Math.Max(col.MinimumWidth, value);
            col.Width = value;
        }
    }

    public override void Initialize(IComponent component)
    {
        _initializing = true;
        base.Initialize(component);

        if (component.Site is not null)
        {
            // Acquire a reference to ISelectionService.
            _selectionService = GetService<ISelectionService>();
            Debug.Assert(_selectionService is not null);

            // Acquire a reference to BehaviorService.
            _behaviorService = GetService<BehaviorService>();

            if (_behaviorService is not null && _selectionService is not null)
            {
                _behavior = new FilterCutCopyPasteDeleteBehavior(true, _behaviorService);

                UpdateBehavior();
                _selectionService.SelectionChanged += selectionService_SelectionChanged;
            }
        }

        _initializing = false;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            PopBehavior();
            if (_selectionService is not null)
            {
                _selectionService.SelectionChanged -= selectionService_SelectionChanged;
            }

            _selectionService = null;
            _behaviorService = null;
        }
    }

    private void PushBehavior()
    {
        if (!_behaviorPushed)
        {
            Debug.Assert(_behavior is not null);
            try
            {
                _behaviorService?.PushBehavior(_behavior);
            }
            finally
            {
                _behaviorPushed = true;
            }
        }
    }

    private void PopBehavior()
    {
        if (_behaviorPushed)
        {
            Debug.Assert(_behavior is not null);
            try
            {
                _behaviorService?.PopBehavior(_behavior);
            }
            finally
            {
                _behaviorPushed = false;
            }
        }
    }

    private void UpdateBehavior()
    {
        if (_selectionService is not null)
        {
            if (_selectionService.PrimarySelection is not null
                    && Component.Equals(_selectionService.PrimarySelection))
            {
                PushBehavior();
            }
            else
            {
                // ensure we are popped off
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
            properties["Width"] = TypeDescriptor.CreateProperty(typeof(DataGridViewColumnDesigner), prop, []);
        }

        prop = properties["Name"] as PropertyDescriptor;
        if (prop is not null)
        {
            if (Component.Site is null)
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
        DataGridViewColumn col = (DataGridViewColumn)Component;
        return col.InheritedAutoSizeMode != DataGridViewAutoSizeColumnMode.Fill && col.Width != DATAGRIDVIEWCOLUMN_defaultWidth;
    }

    private bool ShouldSerializeName()
    {
        if (!TryGetService(out IDesignerHost? host))
        {
            // The column is hosted in the column collection dialog.
            // Return false : let the user type whatever he feels like.
            return false;
        }

        return _initializing ? (Component != host.RootComponent)  // for non root components, respect the name that the base Control serialized unless changed
            : ShadowProperties.ShouldSerializeValue(nameof(Name), null);
    }

    public class FilterCutCopyPasteDeleteBehavior : Behavior.Behavior
    {
        public FilterCutCopyPasteDeleteBehavior(bool callParentBehavior, BehaviorService behaviorService) : base(callParentBehavior, behaviorService) { }

        public override MenuCommand? FindCommand(CommandID commandId)
        {
            MenuCommand command;
            if ((commandId.ID == StandardCommands.Copy.ID) && (commandId.Guid == StandardCommands.Copy.Guid))
            {
                command = new MenuCommand(handler, StandardCommands.Copy)
                {
                    Enabled = false
                };

                return command;
            }

            if ((commandId.ID == StandardCommands.Paste.ID) && (commandId.Guid == StandardCommands.Paste.Guid))
            {
                command = new MenuCommand(handler, StandardCommands.Paste)
                {
                    Enabled = false
                };

                return command;
            }

            if ((commandId.ID == StandardCommands.Delete.ID) && (commandId.Guid == StandardCommands.Delete.Guid))
            {
                command = new MenuCommand(handler, StandardCommands.Delete)
                {
                    Enabled = false
                };

                return command;
            }

            if ((commandId.ID == StandardCommands.Cut.ID) && (commandId.Guid == StandardCommands.Cut.Guid))
            {
                command = new MenuCommand(handler, StandardCommands.Cut)
                {
                    Enabled = false
                };

                return command;
            }

            return base.FindCommand(commandId);
        }

        private void handler(object? sender, EventArgs e) { }
    }
}
