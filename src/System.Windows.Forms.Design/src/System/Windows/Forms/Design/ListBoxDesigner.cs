// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel.Design;
using System.ComponentModel;
using System.Collections;
using static Interop;

namespace System.Windows.Forms.Design
{
    /// <include file='doc\ListBoxDesigner.uex' path='docs/doc[@for="ListBoxDesigner"]/*' />
    /// <devdoc>
    ///      This class handles all design time behavior for the list box class.
    ///      It adds a sample item to the list box at design time.
    /// </devdoc>
    internal class ListBoxDesigner : ControlDesigner
    {

        private DesignerActionListCollection _actionLists;

        public bool IntegralHeight
        {
            get
            {
                return (bool)ShadowProperties["IntegralHeight"];
            }
            set
            {
                ShadowProperties["IntegralHeight"] = value;

                ListBox listBox = (ListBox)this.Component;
                if ((listBox.Dock != DockStyle.Fill) &&
                    (listBox.Dock != DockStyle.Left) &&
                    (listBox.Dock != DockStyle.Right))
                {
                    listBox.IntegralHeight = value;
                }
            }
        }

        public DockStyle Dock
        {
            get
            {
                return ((ListBox)this.Component).Dock;
            }
            set
            {
                ListBox listBox = (ListBox)this.Component;
                if ((value == DockStyle.Fill) || (value == DockStyle.Left) || (value == DockStyle.Right))
                {
                    // VSO 159543
                    // Allow partial listbox item displays so that we don't try to resize the listbox after we dock.
                    listBox.IntegralHeight = false;
                    listBox.Dock = value;
                }
                else
                {
                    listBox.Dock = value;
                    // VSO 159543
                    // Restore the IntegralHeight after we dock. Order is necessary here. Setting IntegralHeight will 
                    // potentially resize the control height, but we don't want to base the height on the dock. 
                    // Instead, undock the control first, so the IntegralHeight is based on the restored size.
                    listBox.IntegralHeight = (bool)ShadowProperties["IntegralHeight"];
                }
            }
        }

        protected override void PreFilterProperties(IDictionary properties)
        {
            PropertyDescriptor integralHeightProp = (PropertyDescriptor)properties["IntegralHeight"];
            if (integralHeightProp != null)
            {
                properties["IntegralHeight"] = TypeDescriptor.CreateProperty(typeof(ListBoxDesigner), integralHeightProp, new Attribute[0]);
            }

            PropertyDescriptor dockProp = (PropertyDescriptor)properties["Dock"];
            if (dockProp != null)
            {
                properties["Dock"] = TypeDescriptor.CreateProperty(typeof(ListBoxDesigner), dockProp, new Attribute[0]);
            }

            base.PreFilterProperties(properties);
        }

        /// <include file='doc\ListBoxDesigner.uex' path='docs/doc[@for="ListBoxDesigner.Dispose"]/*' />
        /// <devdoc>
        ///      Destroys this designer.
        /// </devdoc>
        protected override void Dispose(bool disposing)
        {

            if (disposing)
            {
                // Now, hook the component rename event so we can update the text in the
                // list box.
                //
                IComponentChangeService cs = (IComponentChangeService)GetService(typeof(IComponentChangeService));
                if (cs != null)
                {
                    cs.ComponentRename -= new ComponentRenameEventHandler(this.OnComponentRename);
                    cs.ComponentChanged -= new ComponentChangedEventHandler(this.OnComponentChanged);
                }
            }
            base.Dispose(disposing);
        }

        /// <include file='doc\ListBoxDesigner.uex' path='docs/doc[@for="ListBoxDesigner.Initialize"]/*' />
        /// <devdoc>
        ///     Called by the host when we're first initialized.
        /// </devdoc>
        public override void Initialize(IComponent component)
        {
            base.Initialize(component);

            ListBox listBox = component as ListBox;
            if (null != listBox)
                this.IntegralHeight = listBox.IntegralHeight;

            AutoResizeHandles = true;

            // Now, hook the component rename event so we can update the text in the
            // list box.
            //
            IComponentChangeService cs = (IComponentChangeService)GetService(typeof(IComponentChangeService));
            if (cs != null)
            {
                cs.ComponentRename += new ComponentRenameEventHandler(this.OnComponentRename);
                cs.ComponentChanged += new ComponentChangedEventHandler(this.OnComponentChanged);
            }

        }

        public override void InitializeNewComponent(IDictionary defaultValues)
        {
            base.InitializeNewComponent(defaultValues);

            // in Whidbey, formattingEnabled is true
            ((ListBox)this.Component).FormattingEnabled = true;

            // VSWhidbey 497239 - Setting FormattingEnabled clears the text we set in
            // OnCreateHandle so let's set it here again. We need to keep setting the text in
            // OnCreateHandle, otherwise we introduce VSWhidbey 498162.
            PropertyDescriptor nameProp = TypeDescriptor.GetProperties(Component)["Name"];
            if (nameProp != null)
            {
                UpdateControlName(nameProp.GetValue(Component).ToString());
            }

        }

        /// <include file='doc\ListBoxDesigner.uex' path='docs/doc[@for="ListBoxDesigner.OnComponentRename"]/*' />
        /// <devdoc>
        ///      Raised when a component's name changes.  Here we update the contents of the list box
        ///      if we are displaying the component's name in it.
        /// </devdoc>
        private void OnComponentRename(object sender, ComponentRenameEventArgs e)
        {
            if (e.Component == Component)
            {
                UpdateControlName(e.NewName);
            }
        }

        /// <include file='doc\ListBoxDesigner.uex' path='docs/doc[@for="ListBoxDesigner.OnComponentChanged"]/*' />
        /// <devdoc>
        ///      Raised when ComponentChanges. We listen to this to check if the "Items" propertychanged.
        ///      and if so .. then update the Text within the ListBox.
        /// </devdoc>
        private void OnComponentChanged(object sender, ComponentChangedEventArgs e)
        {
            if (e.Component == Component && e.Member != null && e.Member.Name == "Items")
            {
                PropertyDescriptor nameProp = TypeDescriptor.GetProperties(Component)["Name"];
                if (nameProp != null)
                {
                    UpdateControlName(nameProp.GetValue(Component).ToString());
                }
            }
        }

        /// <include file='doc\ListBoxDesigner.uex' path='docs/doc[@for="ListBoxDesigner.OnCreateHandle"]/*' />
        /// <devdoc>
        ///      This is called immediately after the control handle has been created.
        /// </devdoc>
        protected override void OnCreateHandle()
        {
            base.OnCreateHandle();
            PropertyDescriptor nameProp = TypeDescriptor.GetProperties(Component)["Name"];
            if (nameProp != null)
            {
                UpdateControlName(nameProp.GetValue(Component).ToString());
            }
        }

        /// <include file='doc\ListBoxDesigner.uex' path='docs/doc[@for="ListBoxDesigner.UpdateControlName"]/*' />
        /// <devdoc>
        ///      Updates the name being displayed on this control.  This will do nothing if
        ///      the control has items in it.
        /// </devdoc>
        private void UpdateControlName(string name)
        {
            ListBox lb = (ListBox)Control;
            if (lb.IsHandleCreated && lb.Items.Count == 0)
            {
                User32.SendMessageW(lb, (User32.WM)User32.LB.RESETCONTENT);
                User32.SendMessageW(lb, (User32.WM)User32.LB.ADDSTRING, IntPtr.Zero, name);
            }
        }


        public override DesignerActionListCollection ActionLists
        {
            get
            {
                if (_actionLists == null)
                {
                    _actionLists = new DesignerActionListCollection();
                    if (this.Component is CheckedListBox)
                    {
                        _actionLists.Add(new ListControlUnboundActionList(this));
                    }
                    else
                    {
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
                }
                return _actionLists;
            }
        }
    }
}

