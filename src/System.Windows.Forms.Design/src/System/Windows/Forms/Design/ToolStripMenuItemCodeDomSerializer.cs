// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.ComponentModel.Design.Serialization;

namespace System.Windows.Forms.Design
{
    /// <summary>
    ///  The Reason for having a CustomSerializer for ToolStripMenuItem is the existance of Dummy ToolStripMenuItem for ContextMenuStrips.
    ///  We add this Dummy ToolStripMenuItem on the "Non Site" ToolStrip to Host the DropDown which facilitates the entry of New MenuItems.
    ///  These items are then added to the ContextMenuStrip that we are designing. 
    ///  But we dont want the Dummy ToolStripMenuItem to Serialize and hence the need for this Custom Serializer.
    /// </summary>
    internal class ToolStripMenuItemCodeDomSerializer : CodeDomSerializer
    {
        /// <summary>
        /// We implement this for the abstract method on CodeDomSerializer.
        /// </summary>
        public override object Deserialize(IDesignerSerializationManager manager, object codeObject)
        {
            return GetBaseSerializer(manager).Deserialize(manager, codeObject);
        }

        /// <summary>
        /// This is a small helper method that returns the serializer for base Class
        /// </summary>
        private CodeDomSerializer GetBaseSerializer(IDesignerSerializationManager manager)
        {
            return (CodeDomSerializer)manager.GetSerializer(typeof(Component), typeof(CodeDomSerializer));
        }

        /// <summary>
        /// We implement this for the abstract method on CodeDomSerializer.  This method
        /// takes an object graph, and serializes the object into CodeDom elements.  
        /// </summary>
        public override object Serialize(IDesignerSerializationManager manager, object value)
        {
            ToolStripMenuItem item = value as ToolStripMenuItem;
            ToolStrip parent = item.GetCurrentParent() as ToolStrip;
            
            //Dont Serialize if we are Dummy Item ...
            if ((item != null) && !(item.IsOnDropDown) && (parent != null) && (parent.Site == null))
            {
                //dont serialize anything...
                return null;
            }
            else
            {
                CodeDomSerializer baseSerializer = (CodeDomSerializer)manager.GetSerializer(typeof(ImageList).BaseType, typeof(CodeDomSerializer));
                
                return baseSerializer.Serialize(manager, value);
            }
        }
    }
}
