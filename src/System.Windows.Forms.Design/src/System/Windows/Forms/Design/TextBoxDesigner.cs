// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;

namespace System.Windows.Forms.Design
{
    /// <devdoc>
    ///    <para>
    ///       Provides a designer for TextBox.</para>
    /// </devdoc>
    internal class TextBoxDesigner : TextBoxBaseDesigner
    {

        private char passwordChar;

        private DesignerActionListCollection _actionLists;
        public override DesignerActionListCollection ActionLists
        {
            get
            {
                if (_actionLists == null)
                {
                    _actionLists = new DesignerActionListCollection();
                    _actionLists.Add(new TextBoxActionList(this));
                }

                return _actionLists;
            }
        }

        /// <devdoc>
        ///      Allows a designer to filter the set of properties
        ///      the component it is designing will expose through the
        ///      TypeDescriptor object.  This method is called
        ///      immediately before its corresponding "Post" method.
        ///      If you are overriding this method you should call
        ///      the base implementation before you perform your own
        ///      filtering.
        /// </devdoc>
        protected override void PreFilterProperties(IDictionary properties)
        {
            base.PreFilterProperties(properties);

            PropertyDescriptor prop;

            string[] shadowProps = new string[]
            {
                "PasswordChar"
            };

            Attribute[] empty = new Attribute[0];

            for (int i = 0; i < shadowProps.Length; i++)
            {
                prop = (PropertyDescriptor)properties[shadowProps[i]];
                if (prop != null)
                {
                    properties[shadowProps[i]] = TypeDescriptor.CreateProperty(typeof(TextBoxDesigner), prop, empty);
                }
            }
        }

        /// <devdoc>
        ///     Shadows the PasswordChar.  UseSystemPasswordChar overrides PasswordChar so independent on the value
        ///     of PasswordChar it will return the systemp password char.  However, the value of PasswordChar is 
        ///     cached so if UseSystemPasswordChar is reset at design time the PasswordChar value can be restored.
        ///     So in the case both properties are set, we need to serialize the real PasswordChar value as well.
        /// </devdoc>
        private char PasswordChar
        {
            get
            {
                TextBox tb = this.Control as TextBox;
                Debug.Assert(tb != null, "Designed control is not a TextBox.");

                if (tb.UseSystemPasswordChar)
                {
                    return passwordChar;
                }
                else
                {
                    return tb.PasswordChar;
                }
            }
            set
            {
                TextBox tb = this.Control as TextBox;
                Debug.Assert(tb != null, "Designed control is not a TextBox.");

                passwordChar = value;
                tb.PasswordChar = value;
            }
        }
    }

}
