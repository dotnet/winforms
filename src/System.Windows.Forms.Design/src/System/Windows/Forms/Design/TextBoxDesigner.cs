// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;

namespace System.Windows.Forms.Design;

/// <summary>
///  Provides a designer for TextBox.
/// </summary>
internal class TextBoxDesigner : TextBoxBaseDesigner
{
    private char _passwordChar;

    private DesignerActionListCollection? _actionLists;

    public override DesignerActionListCollection ActionLists
    {
        get
        {
            if (_actionLists is null)
            {
                _actionLists = new DesignerActionListCollection();
                _actionLists.Add(new TextBoxActionList(this));
            }

            return _actionLists;
        }
    }

    /// <summary>
    ///  Allows a designer to filter the set of properties
    ///  the component it is designing will expose through the
    ///  TypeDescriptor object. This method is called
    ///  immediately before its corresponding "Post" method.
    ///  If you are overriding this method you should call
    ///  the base implementation before you perform your own
    ///  filtering.
    /// </summary>
    protected override void PreFilterProperties(IDictionary properties)
    {
        base.PreFilterProperties(properties);

        PropertyDescriptor? prop;

        string[] shadowProps =
        [
            "PasswordChar"
        ];

        Attribute[] empty = [];

        for (int i = 0; i < shadowProps.Length; i++)
        {
            prop = (PropertyDescriptor?)properties[shadowProps[i]];
            if (prop is not null)
            {
                properties[shadowProps[i]] = TypeDescriptor.CreateProperty(typeof(TextBoxDesigner), prop, empty);
            }
        }
    }

    /// <summary>
    ///  Shadows the PasswordChar. UseSystemPasswordChar overrides PasswordChar so independent on the value
    ///  of PasswordChar it will return the system password char. However, the value of PasswordChar is
    ///  cached so if UseSystemPasswordChar is reset at design time the PasswordChar value can be restored.
    ///  So in the case both properties are set, we need to serialize the real PasswordChar value as well.
    /// </summary>
    private char PasswordChar
    {
        get
        {
            TextBox tb = (Control as TextBox)!;
            Debug.Assert(tb is not null, "Designed control is not a TextBox.");

            if (tb.UseSystemPasswordChar)
            {
                return _passwordChar;
            }
            else
            {
                return tb.PasswordChar;
            }
        }
        set
        {
            TextBox tb = (Control as TextBox)!;
            Debug.Assert(tb is not null, "Designed control is not a TextBox.");

            _passwordChar = value;
            tb.PasswordChar = value;
        }
    }
}
