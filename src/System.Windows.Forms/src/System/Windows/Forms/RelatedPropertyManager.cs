// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.ComponentModel;
using System.Diagnostics;

namespace System.Windows.Forms
{
    internal class RelatedPropertyManager : PropertyManager
    {
        BindingManagerBase parentManager;
        string dataField;
        PropertyDescriptor fieldInfo;

        internal RelatedPropertyManager(BindingManagerBase parentManager, string dataField) : base(GetCurrentOrNull(parentManager), dataField)
        {
            Bind(parentManager, dataField);
        }

        private void Bind(BindingManagerBase parentManager, string dataField)
        {
            Debug.Assert(parentManager != null, "How could this be a null parentManager.");
            this.parentManager = parentManager;
            this.dataField = dataField;
            fieldInfo = parentManager.GetItemProperties().Find(dataField, true);
            if (fieldInfo == null)
            {
                throw new ArgumentException(string.Format(SR.RelatedListManagerChild, dataField));
            }
            // this.finalType = fieldInfo.PropertyType;
            parentManager.CurrentItemChanged += new EventHandler(ParentManager_CurrentItemChanged);
            Refresh();
        }

        internal override string GetListName()
        {
            string name = GetListName(new ArrayList());
            if (name.Length > 0)
            {
                return name;
            }
            return base.GetListName();
        }

        protected internal override string GetListName(ArrayList listAccessors)
        {
            listAccessors.Insert(0, fieldInfo);
            return parentManager.GetListName(listAccessors);
        }

        internal override PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[] listAccessors)
        {
            PropertyDescriptor[] accessors;

            if (listAccessors != null && listAccessors.Length > 0)
            {
                accessors = new PropertyDescriptor[listAccessors.Length + 1];
                listAccessors.CopyTo(accessors, 1);
            }
            else
            {
                accessors = new PropertyDescriptor[1];
            }

            // Set this accessor (add to the beginning)
            accessors[0] = fieldInfo;

            // Get props
            return parentManager.GetItemProperties(accessors);
        }

        private void ParentManager_CurrentItemChanged(object sender, EventArgs e)
        {
            Refresh();
        }

        private void Refresh()
        {
            EndCurrentEdit();
            SetDataSource(GetCurrentOrNull(parentManager));
            OnCurrentChanged(EventArgs.Empty);
        }

        internal override Type BindType
        {
            get
            {
                return fieldInfo.PropertyType;
            }
        }

        public override object Current
        {
            get
            {
                return (DataSource != null) ? fieldInfo.GetValue(DataSource) : null;
            }
        }

        static private object GetCurrentOrNull(BindingManagerBase parentManager)
        {
            bool anyCurrent = (parentManager.Position >= 0 && parentManager.Position < parentManager.Count);
            return anyCurrent ? parentManager.Current : null;
        }

    }
}
