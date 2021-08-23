﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel.Design;
using System.ComponentModel;
using System.Collections;
using System.Windows.Forms.Design.Behavior;

namespace System.Windows.Forms.Design
{
    /// <summary>
    ///  <para>
    ///  Provides a designer that can design components
    ///  that extend ComboBox.</para>
    /// </summary>
    internal class ComboBoxDesigner : ControlDesigner
    {
        private EventHandler propChanged; // Delegate used to dirty the selectionUIItem when needed.

        /// <summary>
        ///  Adds a baseline SnapLine to the list of SnapLines related
        ///  to this control.
        /// </summary>
        public override IList SnapLines
        {
            get
            {
                ArrayList snapLines = base.SnapLines as ArrayList;

                //a single text-baseline for the label (and linklabel) control
                int baseline = DesignerUtils.GetTextBaseline(Control, Drawing.ContentAlignment.TopLeft);
                baseline += 3;
                snapLines.Add(new SnapLine(SnapLineType.Baseline, baseline, SnapLinePriority.Medium));

                return snapLines;
            }
        }

        /// <summary>
        ///  Disposes of this object.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Hook up the property change notification so that we can dirty the SelectionUIItem when needed.
                if (propChanged != null)
                {
                    ((ComboBox)Control).StyleChanged -= propChanged;
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

            AutoResizeHandles = true;

            // Hook up the property change notification so that we can dirty the SelectionUIItem when needed.
            propChanged = new EventHandler(OnControlPropertyChanged);
            ((ComboBox)Control).StyleChanged += propChanged;
        }

        /// <summary>
        ///  We override this so we can clear the text field set by controldesigner.
        /// </summary>
        public override void InitializeNewComponent(IDictionary defaultValues)
        {
            base.InitializeNewComponent(defaultValues);

            // in Whidbey, formattingEnabled is TRUE
            ((ComboBox)Component).FormattingEnabled = true;

            PropertyDescriptor textProp = TypeDescriptor.GetProperties(Component)["Text"];
            if (textProp != null && textProp.PropertyType == typeof(string) && !textProp.IsReadOnly && textProp.IsBrowsable)
            {
                textProp.SetValue(Component, "");
            }
        }

        /// <summary>
        ///  For controls, we sync their property changed event so our component can track their location.
        /// </summary>
        private void OnControlPropertyChanged(object sender, EventArgs e)
        {
            if (BehaviorService != null)
            {
                BehaviorService.SyncSelection();
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

                PropertyDescriptor propStyle = TypeDescriptor.GetProperties(component)["DropDownStyle"];
                if (propStyle != null)
                {
                    ComboBoxStyle style = (ComboBoxStyle)propStyle.GetValue(component);

                    // Height is not user-changable for these styles
                    if (style == ComboBoxStyle.DropDown || style == ComboBoxStyle.DropDownList)
                        rules &= ~(SelectionRules.TopSizeable | SelectionRules.BottomSizeable);
                }

                return rules;
            }
        }

        private DesignerActionListCollection _actionLists;

        public override DesignerActionListCollection ActionLists
        {
            get
            {
                if (_actionLists == null)
                {
                    _actionLists = new DesignerActionListCollection();

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

                return _actionLists;
            }
        }
    }
}

