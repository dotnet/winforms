// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace System.Windows.Forms {
    using System.Runtime.Serialization.Formatters;
    using System.Runtime.Remoting;
    using System.Runtime.InteropServices;

    using Microsoft.Win32;
    using System.Collections;
    using System.ComponentModel;
    using System.ComponentModel.Design.Serialization;
    using System.Drawing;
    using System.Diagnostics;
    using System.Globalization;
    using System.Reflection;

    /// <include file='doc\TreeNodeConverter.uex' path='docs/doc[@for="TreeNodeConverter"]/*' />
    /// <devdoc>
    ///      TreeNodeConverter is a class that can be used to convert
    ///      TreeNode objects from one data type to another.  Access this
    ///      class through the TypeDescriptor.
    /// </devdoc>
    public class TreeNodeConverter : TypeConverter {
    
        /// <include file='doc\TreeNodeConverter.uex' path='docs/doc[@for="TreeNodeConverter.CanConvertTo"]/*' />
        /// <devdoc>
        ///    <para>Gets a value indicating whether this converter can
        ///       convert an object to the given destination type using the context.</para>
        /// </devdoc>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
            if (destinationType == typeof(InstanceDescriptor)) {
                return true;
            }
            return base.CanConvertTo(context, destinationType);
        }
        
        /// <include file='doc\TreeNodeConverter.uex' path='docs/doc[@for="TreeNodeConverter.ConvertTo"]/*' />
        /// <devdoc>
        ///      Converts the given object to another type.  The most common types to convert
        ///      are to and from a string object.  The default implementation will make a call
        ///      to ToString on the object if the object is valid and if the destination
        ///      type is string.  If this cannot convert to the desitnation type, this will
        ///      throw a NotSupportedException.
        /// </devdoc>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
            if (destinationType == null) {
                throw new ArgumentNullException(nameof(destinationType));
            }

            if (destinationType == typeof(InstanceDescriptor) && value is TreeNode) {
                TreeNode node = (TreeNode)value;
                MemberInfo info = null;
                object[] args = null;
                
                if (node.ImageIndex == -1 || node.SelectedImageIndex == -1) {
                    if (node.Nodes.Count == 0) {
                        info = typeof(TreeNode).GetConstructor(new Type[] {typeof(string)});
                        args = new object[] {node.Text};
                    }
                    else {
                        info = typeof(TreeNode).GetConstructor(new Type[] {typeof(string), typeof(TreeNode[])});
                        
                        TreeNode[] nodesArray = new TreeNode[node.Nodes.Count];
                        node.Nodes.CopyTo(nodesArray, 0);
                        
                        args = new object[] {node.Text, nodesArray};
                    }
                }
                else {
                    if (node.Nodes.Count == 0) {
                        info = typeof(TreeNode).GetConstructor(new Type[] {
                            typeof(string),
                            typeof(int),
                            typeof(int)});
                        args = new object[] {
                            node.Text,
                            node.ImageIndex,
                            node.SelectedImageIndex};
                    }
                    else {
                        info = typeof(TreeNode).GetConstructor(new Type[] {
                            typeof(string),
                            typeof(int),
                            typeof(int),
                            typeof(TreeNode[])});
                            
                        TreeNode[] nodesArray = new TreeNode[node.Nodes.Count];
                        node.Nodes.CopyTo(nodesArray, 0);    
                            
                        args = new object[] {
                            node.Text,
                            node.ImageIndex,
                            node.SelectedImageIndex,
                            nodesArray};
                    }
                }
                
                if (info != null) {
                    return new InstanceDescriptor(info, args, false);
                }
            }
            
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}

