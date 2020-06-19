﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms.Design
{
    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Windows.Forms.Design.Behavior;

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
                ArrayList snapLines = base.SnapLines as ArrayList;

                int baseline = DesignerUtils.GetTextBaseline(Control, System.Drawing.ContentAlignment.TopLeft);

                BorderStyle borderStyle = BorderStyle.Fixed3D;
                PropertyDescriptor prop = TypeDescriptor.GetProperties(Component)["BorderStyle"];
                if (prop != null)
                {
                    borderStyle = (BorderStyle)prop.GetValue(Component);
                }

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

                snapLines.Add(new SnapLine(SnapLineType.Baseline, baseline, SnapLinePriority.Medium));

                return snapLines;
            }
        }

        private string Text
        {
            get
            {
                return Control.Text;
            }
            set
            {
                Control.Text = value;

                // This fixes bug #48462. If the text box is not wide enough to display all of the text,
                // then we want to display the first portion at design-time. We can ensure this by
                // setting the selection to (0, 0).
                //
                ((TextBoxBase)Control).Select(0, 0);
            }
        }

        private bool ShouldSerializeText()
        {
            return TypeDescriptor.GetProperties(typeof(TextBoxBase))["Text"].ShouldSerializeValue(Component);
        }

        private void ResetText()
        {
            Control.Text = "";
        }

        /// <summary>
        /// We override this so we can clear the text field set by controldesigner.
        /// </summary>
        /// <param name="defaultValues">The default values.</param>
        public override void InitializeNewComponent(IDictionary defaultValues)
        {
            base.InitializeNewComponent(defaultValues);

            PropertyDescriptor textProp = TypeDescriptor.GetProperties(Component)["Text"];
            if (textProp != null && textProp.PropertyType == typeof(string) && !textProp.IsReadOnly && textProp.IsBrowsable)
            {
                textProp.SetValue(Component, "");
            }
        }

        protected override void PreFilterProperties(IDictionary properties)
        {
            base.PreFilterProperties(properties);

            PropertyDescriptor prop;

            // Handle shadowed properties
            //
            string[] shadowProps = new string[] {
                "Text",
            };

            Attribute[] empty = Array.Empty<Attribute>();

            for (int i = 0; i < shadowProps.Length; i++)
            {
                prop = (PropertyDescriptor)properties[shadowProps[i]];
                if (prop != null)
                {
                    properties[shadowProps[i]] = TypeDescriptor.CreateProperty(typeof(TextBoxBaseDesigner), prop, empty);
                }
            }
        }

        /// <summary>
        /// Retrieves a set of rules concerning the movement capabilities of a component.
        /// This should be one or more flags from the SelectionRules class.  If no designer
        /// provides rules for a component, the component will not get any UI services.
        /// </summary>
        public override SelectionRules SelectionRules
        {
            get
            {
                SelectionRules rules = base.SelectionRules;
                object component = Component;

                rules |= SelectionRules.AllSizeable;

                PropertyDescriptor prop = TypeDescriptor.GetProperties(component)["Multiline"];
                if (prop != null)
                {
                    Object value = prop.GetValue(component);
                    if (value is bool && (bool)value == false)
                    {
                        PropertyDescriptor propAuto = TypeDescriptor.GetProperties(component)["AutoSize"];
                        if (propAuto != null)
                        {
                            Object auto = propAuto.GetValue(component);
                            if (auto is bool && (bool)auto == true)
                            {
                                rules &= ~(SelectionRules.TopSizeable | SelectionRules.BottomSizeable);
                            }
                        }
                    }
                }

                return rules;
            }
        }
    }
}
