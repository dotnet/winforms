// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.ComponentModel;
using System.Windows.Forms.Design.Behavior;

namespace System.Windows.Forms.Design;

/// <summary>
/// Provides a designer that can design components
/// that extend TextBoxBase.
/// </summary>
internal class TextBoxBaseDesigner : ControlDesigner
{
    public TextBoxBaseDesigner()
    {
        AutoResizeHandles = true;
    }

    /// <summary>
    /// Adds a baseline SnapLine to the list of SnapLines related
    /// to this control.
    /// </summary>
    public override IList SnapLines
    {
        get
        {
            int baseline = DesignerUtils.GetTextBaseline(Control, Drawing.ContentAlignment.TopLeft);
            BorderStyle borderStyle = BorderStyle.Fixed3D;
            PropertyDescriptorCollection props = TypeDescriptor.GetProperties(Component);
            props.TryGetPropertyDescriptorValue(
                "BorderStyle",
                Component,
                ref borderStyle);

            if (borderStyle == BorderStyle.None)
            {
                baseline += 0;
            }
            else if (borderStyle == BorderStyle.FixedSingle)
            {
                baseline += 2;
            }
            else if (borderStyle == BorderStyle.Fixed3D)
            {
                baseline += 3;
            }
            else
            {
                Debug.Fail("Unknown borderstyle");
                baseline += 0;
            }

            IList<SnapLine> snapLines = SnapLinesInternal;
            snapLines.Add(new SnapLine(SnapLineType.Baseline, baseline, SnapLinePriority.Medium));
            return snapLines.Unwrap();
        }
    }

    private string Text
    {
        get => Control.Text;
        set
        {
            Control.Text = value;

            // This fixes bug #48462. If the text box is not wide enough to display all of the text,
            // then we want to display the first portion at design-time. We can ensure this by
            // setting the selection to (0, 0).
            ((TextBoxBase)Control).Select(0, 0);
        }
    }

    private bool ShouldSerializeText()
    {
        return TypeDescriptor.GetProperties(typeof(TextBoxBase))["Text"]!.ShouldSerializeValue(Component);
    }

    private void ResetText()
    {
        Control.Text = string.Empty;
    }

    /// <summary>
    /// We override this so we can clear the text field set by controldesigner.
    /// </summary>
    /// <param name="defaultValues">The default values.</param>
    public override void InitializeNewComponent(IDictionary? defaultValues)
    {
        base.InitializeNewComponent(defaultValues);

        PropertyDescriptor? textProp = TypeDescriptor.GetProperties(Component)["Text"];
        if (textProp is not null && textProp.PropertyType == typeof(string) && !textProp.IsReadOnly && textProp.IsBrowsable)
        {
            textProp.SetValue(Component, string.Empty);
        }
    }

    protected override void PreFilterProperties(IDictionary properties)
    {
        base.PreFilterProperties(properties);

        // Handle shadowed properties
        string[] shadowProps =
        [
            "Text",
        ];

        for (int i = 0; i < shadowProps.Length; i++)
        {
            if (properties[shadowProps[i]] is PropertyDescriptor prop)
            {
                properties[shadowProps[i]] = TypeDescriptor.CreateProperty(typeof(TextBoxBaseDesigner), prop, []);
            }
        }
    }

    /// <summary>
    /// Retrieves a set of rules concerning the movement capabilities of a component.
    /// This should be one or more flags from the SelectionRules class. If no designer
    /// provides rules for a component, the component will not get any UI services.
    /// </summary>
    public override SelectionRules SelectionRules
    {
        get
        {
            SelectionRules rules = base.SelectionRules;
            object component = Component;

            rules |= SelectionRules.AllSizeable;

            PropertyDescriptor? prop = TypeDescriptor.GetProperties(component)["Multiline"];

            object? value = prop?.GetValue(component);
            if (value is bool valueAsBool && !valueAsBool)
            {
                PropertyDescriptor? propAuto = TypeDescriptor.GetProperties(component)["AutoSize"];
                object? auto = propAuto?.GetValue(component);
                if (auto is bool autoAsBool && autoAsBool)
                {
                    rules &= ~(SelectionRules.TopSizeable | SelectionRules.BottomSizeable);
                }
            }

            return rules;
        }
    }
}
