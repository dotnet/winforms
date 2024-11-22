// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms.Design.Behavior;

namespace System.Windows.Forms.Design;

/// <summary>
///  <para>
///  Provides a designer that can design components
///  that extend ButtonBase.</para>
/// </summary>
internal class ButtonBaseDesigner : ControlDesigner
{
    public ButtonBaseDesigner()
    {
        AutoResizeHandles = true;
    }

    public override void InitializeNewComponent(IDictionary? defaultValues)
    {
        base.InitializeNewComponent(defaultValues);

        PropertyDescriptor? prop = TypeDescriptor.GetProperties(Component)["UseVisualStyleBackColor"];
        if (prop is not null && prop.PropertyType == typeof(bool) && !prop.IsReadOnly && prop.IsBrowsable)
        {
            // Dev10 Bug 685319: We should set the UseVisualStyleBackColor to trun only
            // when this property has not been set/changed by user
            if (!prop.ShouldSerializeValue(Component))
            {
                prop.SetValue(Component, true);
            }
        }
    }

    /// <summary>
    ///  Adds a baseline SnapLine to the list of SnapLines related
    ///  to this control.
    /// </summary>
    public override IList SnapLines
    {
        get
        {
            IList<SnapLine> snapLines = SnapLinesInternal;
            FlatStyle flatStyle = FlatStyle.Standard;
            ContentAlignment alignment = ContentAlignment.MiddleCenter;

            PropertyDescriptorCollection props = TypeDescriptor.GetProperties(Component);

            props.TryGetPropertyDescriptorValue(
                "TextAlign",
                Component,
                ref alignment);

            props.TryGetPropertyDescriptorValue(
                "FlatStyle",
                Component,
                ref flatStyle);

            int baseline = DesignerUtils.GetTextBaseline(Control, alignment);

            // based on the type of control and it's style, we need to add certain deltas to make
            // the snapline appear in the right place. Rather than adding a class for each control
            // we special case it here - for perf reasons.

            if (Control is CheckBox or RadioButton)
            {
                Appearance appearance = Appearance.Normal;

                props.TryGetPropertyDescriptorValue(
                    "Appearance",
                    Component,
                    ref appearance);

                if (appearance == Appearance.Normal)
                {
                    if (Control is CheckBox)
                    {
                        baseline += CheckboxBaselineOffset(alignment, flatStyle);
                    }
                    else
                    {
                        baseline += RadiobuttonBaselineOffset(alignment, flatStyle);
                    }
                }
                else
                {
                    baseline += DefaultBaselineOffset(alignment, flatStyle);
                }
            }
            else
            { // default case
                baseline += DefaultBaselineOffset(alignment, flatStyle);
            }

            snapLines.Add(new SnapLine(SnapLineType.Baseline, baseline, SnapLinePriority.Medium));

            return snapLines.Unwrap();
        }
    }

    private static int CheckboxBaselineOffset(ContentAlignment alignment, FlatStyle flatStyle)
    {
        if ((alignment & DesignerUtils.AnyMiddleAlignment) != 0)
        {
            if (flatStyle is FlatStyle.Standard or FlatStyle.System)
            {
                return -1;
            }
            else
            {
                return 0; // FlatStyle.Flat || FlatStyle.Popup || Unknown FlatStyle
            }
        }
        else if ((alignment & DesignerUtils.AnyTopAlignment) != 0)
        {
            if (flatStyle == FlatStyle.Standard)
            {
                return 1;
            }
            else if (flatStyle == FlatStyle.System)
            {
                return 0;
            }
            else if (flatStyle is FlatStyle.Flat or FlatStyle.Popup)
            {
                return 2;
            }
            else
            {
                Debug.Fail("Unknown FlatStyle");
                return 0; // Unknown FlatStyle
            }
        }
        else
        {
            // Bottom alignment
            if (flatStyle == FlatStyle.Standard)
            {
                return -3;
            }
            else if (flatStyle == FlatStyle.System)
            {
                return 0;
            }
            else if (flatStyle is FlatStyle.Flat or FlatStyle.Popup)
            {
                return -2;
            }
            else
            {
                Debug.Fail("Unknown FlatStyle");
                return 0; // Unknown FlatStyle
            }
        }
    }

    private static int RadiobuttonBaselineOffset(ContentAlignment alignment, FlatStyle flatStyle)
    {
        if ((alignment & DesignerUtils.AnyMiddleAlignment) != 0)
        {
            if (flatStyle == FlatStyle.System)
            {
                return -1;
            }
            else
            {
                return 0; // FlatStyle.Standard || FlatStyle.Flat || FlatStyle.Popup || Unknown FlatStyle
            }
        }
        else
        {// Top or bottom alignment
            if (flatStyle is FlatStyle.Standard or FlatStyle.Flat or FlatStyle.Popup)
            {
                return ((alignment & DesignerUtils.AnyTopAlignment) != 0) ? 2 : -2;
            }
            else if (flatStyle == FlatStyle.System)
            {
                return 0;
            }
            else
            {
                Debug.Fail("Unknown FlatStyle");
                return 0; // Unknown FlatStyle
            }
        }
    }

    private static int DefaultBaselineOffset(ContentAlignment alignment, FlatStyle flatStyle)
    {
        if ((alignment & DesignerUtils.AnyMiddleAlignment) != 0)
        {
            return 0;
        }
        else
        {
            // Top or bottom alignment
            if (flatStyle is FlatStyle.Standard or FlatStyle.Popup)
            {
                return ((alignment & DesignerUtils.AnyTopAlignment) != 0) ? 4 : -4;
            }
            else if (flatStyle == FlatStyle.System)
            {
                return ((alignment & DesignerUtils.AnyTopAlignment) != 0) ? 3 : -3;
            }
            else if (flatStyle == FlatStyle.Flat)
            {
                return ((alignment & DesignerUtils.AnyTopAlignment) != 0) ? 5 : -5;
            }
            else
            {
                Debug.Fail("Unknown FlatStyle");
                return 0; // Unknown FlatStyle
            }
        }
    }

    /*
            public override DesignerActionListCollection ActionLists {
                get {
                    if(_actionlists == null) {
                        _actionlists = new DesignerActionListCollection();
                        _actionlists.Add(new ButtonBaseActionList());
                    }
                    return _actionlists;
                }
            }
    */
}
