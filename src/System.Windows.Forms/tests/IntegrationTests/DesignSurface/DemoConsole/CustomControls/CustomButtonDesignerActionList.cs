// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;

namespace TestConsole;

public class CustomButtonDesignerActionList : DesignerActionList
{
    private readonly IDesignerHost _host;
    private readonly CustomButton _control;
    private DesignerActionItemCollection _actonListItems;

    public CustomButtonDesignerActionList(IComponent component)
        : base(component)
    {
        _control = component as CustomButton;
        _host = GetService(typeof(IDesignerHost)) as IDesignerHost;
    }

    public override DesignerActionItemCollection GetSortedActionItems()
    {
        _actonListItems = new DesignerActionItemCollection();
        _actonListItems.Add(new DesignerActionHeaderItem("Change color"));
        _actonListItems.Add(new DesignerActionVerbItem(new DesignerVerb(GetActionName(), OnColorActionClick)));

        return _actonListItems;
    }

    public string Name
    {
        get
        {
            string name = string.Empty;
            if (_control is not null)
            {
                CustomButton control = _control;
                name = control.Name;
            }

            return name;
        }
        set
        {
            SetValue(nameof(Name), value);
        }
    }

    private string GetActionName()
    {
        PropertyDescriptor dockProp = TypeDescriptor.GetProperties(Component)[nameof(CustomButton.BackColor)];
        if (dockProp is not null)
        {
            Color backColor = (Color)dockProp.GetValue(Component);
            if (backColor != Color.Yellow)
            {
                return "Make Yellow";
            }
            else
            {
                return "Turn Blue";
            }
        }

        return null;
    }

    private void OnColorActionClick(object sender, EventArgs e)
    {
        if (_host is null || sender is not DesignerVerb designerVerb)
        {
            return;
        }

        using DesignerTransaction t = _host.CreateTransaction(designerVerb.Text);

        Color backColor = _control.BackColor;
        _control.BackColor = backColor != Color.Yellow ? Color.Yellow : Color.Blue;

        t.Commit();
    }

    protected void SetValue(string propertyName, object value)
    {
        GetProperty(propertyName).SetValue(_control, value);
    }

    protected PropertyDescriptor GetProperty(string propertyName)
    {
        PropertyDescriptor pd = TypeDescriptor.GetProperties(_control)[propertyName];
        if (pd is null)
        {
            throw new ArgumentException($"Property {propertyName} not found in {nameof(CustomButton)}");
        }
        else
        {
            return pd;
        }
    }
}
