﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms.Design.Behavior;

namespace System.Windows.Forms.Design
{
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
                ArrayList snapLines = base.SnapLines as ArrayList;
                ContentAlignment alignment = ContentAlignment.TopLeft;

                PropertyDescriptor prop;
                PropertyDescriptorCollection props = TypeDescriptor.GetProperties(Component);

                if ((prop = props["TextAlign"]) != null)
                {
                    alignment = (ContentAlignment)prop.GetValue(Component);
                }

                //a single text-baseline for the label (and linklabel) control
                int baseline = DesignerUtils.GetTextBaseline(Control, alignment);

                if ((prop = props["AutoSize"]) != null)
                {
                    if ((bool)prop.GetValue(Component) == false)
                    {
                        //Only adjust if AutoSize is false
                        BorderStyle borderStyle = BorderStyle.None;
                        if ((prop = props["BorderStyle"]) != null)
                        {
                            borderStyle = (BorderStyle)prop.GetValue(Component);
                        }

                        baseline += LabelBaselineOffset(alignment, borderStyle);
                    }
                }

                snapLines.Add(new SnapLine(SnapLineType.Baseline, baseline, SnapLinePriority.Medium));

                // VSWhidbey# 414468
                Label label = Control as Label;
                if (label != null && label.BorderStyle == BorderStyle.None)
                {
                    Type type = Type.GetType("System.Windows.Forms.Label");
                    if (type != null)
                    {
                        MethodInfo info = type.GetMethod("GetLeadingTextPaddingFromTextFormatFlags", BindingFlags.Instance | BindingFlags.NonPublic);
                        if (info != null)
                        {
                            int offset = (int)info.Invoke(Component, null);
                            bool rtl = (label.RightToLeft == RightToLeft.Yes);

                            for (int i = 0; i < snapLines.Count; i++)
                            {
                                // remove previous padding snaplines
                                SnapLine snapLine = snapLines[i] as SnapLine;
                                if (snapLine != null && snapLine.SnapLineType == (rtl ? SnapLineType.Right : SnapLineType.Left))
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

                return snapLines;
            }
        }

        private int LabelBaselineOffset(ContentAlignment alignment, BorderStyle borderStyle)
        {
            if (((alignment & DesignerUtils.AnyMiddleAlignment) != 0) ||
                 ((alignment & DesignerUtils.AnyTopAlignment) != 0))
            {
                if (borderStyle == BorderStyle.None)
                {
                    return 0;
                }
                else if ((borderStyle == BorderStyle.FixedSingle) || (borderStyle == BorderStyle.Fixed3D))
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
            {//bottom alignment
                if (borderStyle == BorderStyle.None)
                {
                    return -1;
                }
                else if ((borderStyle == BorderStyle.FixedSingle) || (borderStyle == BorderStyle.Fixed3D))
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
        ///  This should be one or more flags from the SelectionRules class.  If no designer
        ///  provides rules for a component, the component will not get any UI services.
        /// </summary>
        public override SelectionRules SelectionRules
        {
            get
            {
                SelectionRules rules = base.SelectionRules;
                object component = Component;

                PropertyDescriptor propAutoSize = TypeDescriptor.GetProperties(component)["AutoSize"];
                if (propAutoSize != null)
                {
                    bool autoSize = (bool)propAutoSize.GetValue(component);

                    if (autoSize)
                        rules &= ~SelectionRules.AllSizeable;
                }

                return rules;
            }
        }
    }
}
