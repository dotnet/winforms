// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;

namespace System.Windows.Forms.Design
{
    /// <summary>
    ///  The ListControlStringCollectionEditor override StringCollectionEditor
    ///  to prevent the string collection from being edited if a DataSource
    ///  has been set on the control.
    /// </summary>
    internal class ListControlStringCollectionEditor : StringCollectionEditor
    {
        public ListControlStringCollectionEditor(Type type) : base(type)
        {
        }

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            // If we're trying to edit the items in an object that has a DataSource set, throw an exception
            ListControl control = context.Instance as ListControl;
            if (control?.DataSource != null)
            {
                throw new ArgumentException(SR.DataSourceLocksItems);
            }

            return base.EditValue(context, provider, value);
        }
    }
}
