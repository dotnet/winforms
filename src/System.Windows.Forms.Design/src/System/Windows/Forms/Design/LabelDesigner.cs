// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms.Design.Behavior;

namespace System.Windows.Forms.Design;

/// <summary>
///  <para>
///  Provides a designer that can design components
///  that extend TextBoxBase.</para>
/// </summary>
internal class LabelDesigner : ControlDesigner
{
    public LabelDesigner()
    {
        AutoResizeHandles = true;
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
            ContentAlignment alignment = ContentAlignment.TopLeft;

            PropertyDescriptorCollection props = TypeDescriptor.GetProperties(Component);

            props.TryGetPropertyDescriptorValue(
                "TextAlign",
                Component,
                ref alignment);

            // a single text-baseline for the label (and linklabel) control
            int baseline = DesignerUtils.GetTextBaseline(Control, alignment);

            bool autoSize = false;
            if (props.TryGetPropertyDescriptorValue(
                "AutoSize",
                Component,
                ref autoSize)
                && !autoSize)
            {
                // Only adjust if AutoSize is false
                BorderStyle borderStyle = BorderStyle.None;
                props.TryGetPropertyDescriptorValue(
                    "BorderStyle",
                    Component,
                    ref borderStyle);

                baseline += LabelBaselineOffset(alignment, borderStyle);
            }

            snapLines.Add(new SnapLine(SnapLineType.Baseline, baseline, SnapLinePriority.Medium));

            // VSWhidbey# 414468
            Label? label = Control as Label;
            if (label is not null && label.BorderStyle == BorderStyle.None)
            {
                Type? type = Type.GetType("System.Windows.Forms.Label");
                if (type is not null)
                {
                    MethodInfo? info = type.GetMethod("GetLeadingTextPaddingFromTextFormatFlags", BindingFlags.Instance | BindingFlags.NonPublic);
                    if (info is not null)
                    {
                        int offset = (int)(info.Invoke(Component, parameters: null) ?? 0);
                        bool rtl = (label.RightToLeft == RightToLeft.Yes);

                        for (int i = 0; i < snapLines.Count; i++)
                        {
                            // remove previous padding snaplines
                            SnapLine snapLine = snapLines[i];
                            if (snapLine is not null && snapLine.SnapLineType == (rtl ? SnapLineType.Right : SnapLineType.Left))
                            {
                                snapLine.AdjustOffset(rtl ? -offset : offset);
                                break;
                            }
                        }
                    }
                    else
                    {
                        Debug.Fail("Who removed GetLeadingTextPaddingFromTextFormatFlags from Label?");
                    }
                }
            }

            return snapLines.Unwrap();
        }
    }

    private static int LabelBaselineOffset(ContentAlignment alignment, BorderStyle borderStyle)
    {
        if (((alignment & DesignerUtils.AnyMiddleAlignment) != 0) ||
             ((alignment & DesignerUtils.AnyTopAlignment) != 0))
        {
            if (borderStyle == BorderStyle.None)
            {
                return 0;
            }
            else if (borderStyle is BorderStyle.FixedSingle or BorderStyle.Fixed3D)
            {
                return 1;
            }
            else
            {
                Debug.Fail("Unknown BorderStyle");
                return 0;
            }
        }
        else
        {// bottom alignment
            if (borderStyle == BorderStyle.None)
            {
                return -1;
            }
            else if (borderStyle is BorderStyle.FixedSingle or BorderStyle.Fixed3D)
            {
                return 0;
            }
            else
            {
                Debug.Fail("Unknown BorderStyle");
                return 0;
            }
        }
    }

    /// <summary>
    ///  Retrieves a set of rules concerning the movement capabilities of a component.
    ///  This should be one or more flags from the SelectionRules class. If no designer
    ///  provides rules for a component, the component will not get any UI services.
    /// </summary>
    public override SelectionRules SelectionRules
    {
        get
        {
            SelectionRules rules = base.SelectionRules;
            bool autoSize = false;
            PropertyDescriptorCollection props = TypeDescriptor.GetProperties(Component);
            if (props.TryGetPropertyDescriptorValue(
                "AutoSize",
                Component,
                ref autoSize))
            {
                if (autoSize)
                {
                    rules &= ~SelectionRules.AllSizeable;
                }
            }

            return rules;
        }
    }
}
