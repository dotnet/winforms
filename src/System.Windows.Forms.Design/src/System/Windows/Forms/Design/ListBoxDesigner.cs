// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable disable

using System.ComponentModel.Design;
using System.ComponentModel;
using System.Collections;

namespace System.Windows.Forms.Design;

/// <summary>
///  This class handles all design time behavior for the list box class.
///  It adds a sample item to the list box at design time.
/// </summary>
internal class ListBoxDesigner : ControlDesigner
{
    private DesignerActionListCollection _actionLists;

    public bool IntegralHeight
    {
        get
        {
            return (bool)ShadowProperties[nameof(IntegralHeight)];
        }
        set
        {
            ShadowProperties[nameof(IntegralHeight)] = value;

            ListBox listBox = (ListBox)Component;
            if (listBox.Dock is not DockStyle.Fill
                and not DockStyle.Left
                and not DockStyle.Right)
            {
                listBox.IntegralHeight = value;
            }
        }
    }

    public DockStyle Dock
    {
        get
        {
            return ((ListBox)Component).Dock;
        }
        set
        {
            ListBox listBox = (ListBox)Component;
            if (value is DockStyle.Fill or DockStyle.Left or DockStyle.Right)
            {
                // VSO 159543
                // Allow partial listbox item displays so that we don't try to resize the listbox after we dock.
                listBox.IntegralHeight = false;
                listBox.Dock = value;
            }
            else
            {
                listBox.Dock = value;
                // VSO 159543
                // Restore the IntegralHeight after we dock. Order is necessary here. Setting IntegralHeight will
                // potentially resize the control height, but we don't want to base the height on the dock.
                // Instead, undock the control first, so the IntegralHeight is based on the restored size.
                listBox.IntegralHeight = (bool)ShadowProperties[nameof(IntegralHeight)];
            }
        }
    }

    protected override void PreFilterProperties(IDictionary properties)
    {
        PropertyDescriptor integralHeightProp = (PropertyDescriptor)properties["IntegralHeight"];
        if (integralHeightProp is not null)
        {
            properties["IntegralHeight"] = TypeDescriptor.CreateProperty(typeof(ListBoxDesigner), integralHeightProp, []);
        }

        PropertyDescriptor dockProp = (PropertyDescriptor)properties["Dock"];
        if (dockProp is not null)
        {
            properties["Dock"] = TypeDescriptor.CreateProperty(typeof(ListBoxDesigner), dockProp, []);
        }

        base.PreFilterProperties(properties);
    }

    /// <summary>
    ///  Destroys this designer.
    /// </summary>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            // Now, hook the component rename event so we can update the text in the
            // list box.
            //
            IComponentChangeService cs = (IComponentChangeService)GetService(typeof(IComponentChangeService));
            if (cs is not null)
            {
                cs.ComponentRename -= new ComponentRenameEventHandler(OnComponentRename);
                cs.ComponentChanged -= new ComponentChangedEventHandler(OnComponentChanged);
            }
        }

        base.Dispose(disposing);
    }

    /// <summary>
    ///  Called by the host when we're first initialized.
    /// </summary>
    public override void Initialize(IComponent component)
    {
        base.Initialize(component);

        ListBox listBox = component as ListBox;
        if (listBox is not null)
            IntegralHeight = listBox.IntegralHeight;

        AutoResizeHandles = true;

        // Now, hook the component rename event so we can update the text in the
        // list box.
        //
        IComponentChangeService cs = (IComponentChangeService)GetService(typeof(IComponentChangeService));
        if (cs is not null)
        {
            cs.ComponentRename += new ComponentRenameEventHandler(OnComponentRename);
            cs.ComponentChanged += new ComponentChangedEventHandler(OnComponentChanged);
        }
    }

    public override void InitializeNewComponent(IDictionary defaultValues)
    {
        base.InitializeNewComponent(defaultValues);

        // in Whidbey, formattingEnabled is true
        ((ListBox)Component).FormattingEnabled = true;

        // VSWhidbey 497239 - Setting FormattingEnabled clears the text we set in
        // OnCreateHandle so let's set it here again. We need to keep setting the text in
        // OnCreateHandle, otherwise we introduce VSWhidbey 498162.
        PropertyDescriptor nameProp = TypeDescriptor.GetProperties(Component)["Name"];
        if (nameProp is not null)
        {
            UpdateControlName(nameProp.GetValue(Component).ToString());
        }
    }

    /// <summary>
    ///  Raised when a component's name changes.  Here we update the contents of the list box
    ///  if we are displaying the component's name in it.
    /// </summary>
    private void OnComponentRename(object sender, ComponentRenameEventArgs e)
    {
        if (e.Component == Component)
        {
            UpdateControlName(e.NewName);
        }
    }

    /// <summary>
    ///  Raised when ComponentChanges. We listen to this to check if the "Items" propertychanged.
    ///  and if so .. then update the Text within the ListBox.
    /// </summary>
    private void OnComponentChanged(object sender, ComponentChangedEventArgs e)
    {
        if (e.Component == Component && e.Member is not null && e.Member.Name == "Items")
        {
            PropertyDescriptor nameProp = TypeDescriptor.GetProperties(Component)["Name"];
            if (nameProp is not null)
            {
                UpdateControlName(nameProp.GetValue(Component).ToString());
            }
        }
    }

    /// <summary>
    ///  This is called immediately after the control handle has been created.
    /// </summary>
    protected override void OnCreateHandle()
    {
        base.OnCreateHandle();
        PropertyDescriptor nameProp = TypeDescriptor.GetProperties(Component)["Name"];
        if (nameProp is not null)
        {
            UpdateControlName(nameProp.GetValue(Component).ToString());
        }
    }

    /// <summary>
    ///  Updates the name being displayed on this control.  This will do nothing if
    ///  the control has items in it.
    /// </summary>
    private void UpdateControlName(string name)
    {
        ListBox lb = (ListBox)Control;
        if (lb.IsHandleCreated && lb.Items.Count == 0)
        {
            PInvoke.SendMessage(lb, PInvoke.LB_RESETCONTENT);
            PInvoke.SendMessage(lb, PInvoke.LB_ADDSTRING, 0, name);
        }
    }

    public override DesignerActionListCollection ActionLists
    {
        get
        {
            if (_actionLists is null)
            {
                _actionLists = new DesignerActionListCollection();
                if (Component is CheckedListBox)
                {
                    _actionLists.Add(new ListControlUnboundActionList(this));
                }
                else
                {
                    // TODO: investigate necessity and possibility of porting databinding infra
#if DESIGNER_DATABINDING
                    // Requires:
                    // - System.Windows.Forms.Design.DataMemberFieldEditor
                    // - System.Windows.Forms.Design.DesignBindingConverter
                    // - System.Windows.Forms.Design.DesignBindingEditor
                    //
                    _actionLists.Add(new ListControlBoundActionList(this));
#else
                    _actionLists.Add(new ListControlUnboundActionList(this));
#endif
                }
            }

            return _actionLists;
        }
    }
}
