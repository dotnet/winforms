﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel.Design;
using System.ComponentModel;
using System.Collections;
using static Interop;

namespace System.Windows.Forms.Design
{
    /// <summary>
    ///  The RichTextBoxDesigner provides rich designtime behavior for the
    ///  RichTextBox control.
    /// </summary>
    internal class RichTextBoxDesigner : TextBoxBaseDesigner
    {
        private DesignerActionListCollection _actionLists;

        /// <summary>
        ///  Called when the designer is intialized.  This allows the designer to provide some
        ///  meaningful default values in the control.  The default implementation of this
        ///  sets the control's text to its name.
        /// </summary>
        public override void InitializeNewComponent(IDictionary defaultValues)
        {
            base.InitializeNewComponent(defaultValues);

            // Disable DragDrop at design time.
            // CONSIDER: Is this the correct function for doing this?
            Control control = Control;

            if (control != null && control.Handle != IntPtr.Zero)
            {
                Ole32.RevokeDragDrop(control.Handle);
                // DragAcceptFiles(control.Handle, false);
            }
        }

        public override DesignerActionListCollection ActionLists
        {
            get
            {
                if (_actionLists == null)
                {
                    _actionLists = new DesignerActionListCollection();
                    _actionLists.Add(new RichTextBoxActionList(this));
                }

                return _actionLists;
            }
        }

        /// <summary>
        ///  Allows a designer to filter the set of properties
        ///  the component it is designing will expose through the
        ///  TypeDescriptor object.  This method is called
        ///  immediately before its corresponding "Post" method.
        ///  If you are overriding this method you should call
        ///  the base implementation before you perform your own
        ///  filtering.
        /// </summary>
        protected override void PreFilterProperties(IDictionary properties)
        {
            base.PreFilterProperties(properties);

            PropertyDescriptor prop;

            // Handle shadowed properties
            //
            string[] shadowProps = new string[]
            {
                nameof(Text)
            };

            Attribute[] empty = Array.Empty<Attribute>();

            for (int i = 0; i < shadowProps.Length; i++)
            {
                prop = (PropertyDescriptor)properties[shadowProps[i]];
                if (prop != null)
                {
                    properties[shadowProps[i]] = TypeDescriptor.CreateProperty(typeof(RichTextBoxDesigner), prop, empty);
                }
            }
        }

        /// <summary>
        ///  Accessor for Text. We need to replace "\r\n" with "\n" in the designer before deciding whether
        ///  the old value and new value match.
        /// </summary>
        private string Text
        {
            get
            {
                return Control.Text;
            }
            set
            {
                string oldText = Control.Text;
                if (value != null)
                {
                    value = value.Replace("\r\n", "\n");
                }

                if (oldText != value)
                {
                    Control.Text = value;
                }
            }
        }
    }
}

