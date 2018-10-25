// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms.Design {
using System.Diagnostics.CodeAnalysis;

       [AttributeUsage(AttributeTargets.Class)]
       public sealed class ToolStripItemDesignerAvailabilityAttribute : Attribute {
           private ToolStripItemDesignerAvailability  visibility;
            [
                SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")   // ToolStripDesignAvailabilityAttribute is
                                                                                                            // actually immutable.
            ]
           public static readonly ToolStripItemDesignerAvailabilityAttribute Default = new ToolStripItemDesignerAvailabilityAttribute();
                  
           // <devdoc> 
           // Specifies which ToolStrip types the Item can appear in - ToolStrip,MenuStrip,StatusStrip,ContextMenuStrip
           // Adding this attribute over a class lets you add to the list of custom items in the ToolStrip design time.
           // </devdoc>
           public ToolStripItemDesignerAvailabilityAttribute() {
                this.visibility = ToolStripItemDesignerAvailability.None;
           }
   
           public ToolStripItemDesignerAvailabilityAttribute(ToolStripItemDesignerAvailability visibility) {
              this.visibility = visibility;
           }
   
           public ToolStripItemDesignerAvailability ItemAdditionVisibility {
               get {
                   return visibility;
               }
           }
           public override bool Equals(object obj) {
               if (obj == this) {
                   return true;
               }
    
               ToolStripItemDesignerAvailabilityAttribute other = obj as ToolStripItemDesignerAvailabilityAttribute;
               return (other != null) && other.ItemAdditionVisibility == this.visibility;
           }
    
           public override int GetHashCode() {
               return visibility.GetHashCode();
           }
    
           public override bool IsDefaultAttribute() {
               return (this.Equals(Default));
           }
       }


}


