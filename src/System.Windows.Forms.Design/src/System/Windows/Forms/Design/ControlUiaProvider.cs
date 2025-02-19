// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using global::Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms.Design.System.Windows.Forms.Design;

internal class ControlUiaProvider : IRawElementProviderSimple,
                                    IValueProvider
{
    protected Control _owner;
    private static Dictionary<AccessibleRole, int> ControlTypeIds = new Dictionary<AccessibleRole, int>()
        {
            { AccessibleRole.ComboBox, ControlType.ComboBox.Id},
            { AccessibleRole.Link, ControlType.Hyperlink.Id},
            { AccessibleRole.List, ControlType.List.Id},
            { AccessibleRole.ListItem, ControlType.ListItem.Id},
            { AccessibleRole.MenuBar, ControlType.MenuBar.Id},
            { AccessibleRole.MenuItem, ControlType.MenuItem.Id},
            { AccessibleRole.Pane, ControlType.Pane.Id},
            { AccessibleRole.PushButton, ControlType.Button.Id},
            { AccessibleRole.RadioButton, ControlType.RadioButton.Id},
            { AccessibleRole.ProgressBar, ControlType.ProgressBar.Id},
            { AccessibleRole.ScrollBar, ControlType.ScrollBar.Id},
            { AccessibleRole.Separator, ControlType.Separator.Id},
            { AccessibleRole.Slider, ControlType.Slider.Id},
            { AccessibleRole.SpinButton, ControlType.Spinner.Id},
            { AccessibleRole.SplitButton, ControlType.SplitButton.Id},
            { AccessibleRole.StaticText, ControlType.Text.Id},
            { AccessibleRole.Table, ControlType.Table.Id},
            { AccessibleRole.Text, ControlType.Edit.Id},
            { AccessibleRole.TitleBar, ControlType.TitleBar.Id},
            { AccessibleRole.ToolBar, ControlType.ToolBar.Id},
            { AccessibleRole.ToolTip, ControlType.ToolTip.Id},
            { AccessibleRole.Window, ControlType.Window.Id},
            { AccessibleRole.Outline, ControlType.Tree.Id},
            { AccessibleRole.OutlineItem, ControlType.TreeItem.Id},
            { AccessibleRole.PageTab, ControlType.TabItem.Id},
            { AccessibleRole.PageTabList, ControlType.TabItem.Id}
        };

    public ControlUiaProvider(Control owner)
    {
        _owner = owner;
    }

    public virtual bool IsExpanded
    {
        get { return false; }
    }

    #region IRawElementProviderSimple
    public ProviderOptions ProviderOptions
    {
        get
        {
            return ProviderOptions.ProviderOptions_ServerSideProvider | ProviderOptions.ProviderOptions_UseComThreading;
        }
    }

    public IRawElementProviderSimple HostRawElementProvider
    {
        get
        {
            return AutomationInteropProvider.HostProviderFromHandle(_owner.Handle);
        }
    }

    public virtual object GetPatternProvider(int patternId)
    {
        if (patternId == ValuePatternIdentifiers.Pattern.Id)
        {
            return this as IValueProvider;
        }
        return null;
    }

    public virtual object GetPropertyValue(int propertyId)
    {
        if (propertyId == AutomationElementIdentifiers.NameProperty.Id)
        {
            return _owner.AccessibilityObject.Name;
        }
        else if (propertyId == AutomationElementIdentifiers.ControlTypeProperty.Id)
        {
            if (ControlTypeIds.TryGetValue(_owner.AccessibleRole, out int id))
            {
                return id;
            }

            return ControlType.Custom.Id;
        }
        else if (propertyId == AutomationElementIdentifiers.IsKeyboardFocusableProperty.Id)
        {
            return _owner.Enabled;
        }
        else if (propertyId == AutomationElementIdentifiers.HasKeyboardFocusProperty.Id)
        {
            return _owner.Focused;
        }
        return null;
    }
    #endregion

    #region IValueProvider
    public virtual bool IsReadOnly
    {
        get
        {
            return !_owner.Enabled;
        }
    }

    public virtual string Value
    {
        get
        {
            return _owner.Text;
        }
    }

    public virtual void SetValue(string newValue)
    {
        _owner.Text = newValue;
    }
    #endregion
}
