// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.



namespace System.Windows.Forms.Design {
    using System.Runtime.Remoting;
    using System.ComponentModel;

    using System.Diagnostics;

    using System;
    using System.Drawing;
    using System.Windows.Forms;
    using System.Windows.Forms.ComponentModel;
    using System.ComponentModel.Design;
    using Microsoft.Win32;
    using System.Runtime.InteropServices;
    using System.Runtime.Versioning;

    /// <devdoc>
    /// <para>Provides a base implementation for a <see cref='System.Windows.Forms.Design.ComponentEditorPage'/>.</para>
    /// </devdoc>
    [ComVisible(true),
     ClassInterface(ClassInterfaceType.AutoDispatch),
     System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1012:AbstractTypesShouldNotHaveConstructors") // Shipped in Everett
    ]
    public abstract class ComponentEditorPage : Panel {

        IComponentEditorPageSite pageSite;
        IComponent component;
        bool firstActivate;
        bool loadRequired;
        int loading;
        Icon icon;
        bool commitOnDeactivate;

        /// <devdoc>
        ///    <para>
        ///       Initializes a new instance of the <see cref='System.Windows.Forms.Design.ComponentEditorPage'/> class.
        ///    </para>
        /// </devdoc>
        public ComponentEditorPage() : base() {
            commitOnDeactivate = false;
            firstActivate = true;
            loadRequired = false;
            loading = 0;

            Visible = false;
        }


        /// <devdoc>
        ///    <para>
        ///       Hide the property
        ///    </para>
        /// </devdoc>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override bool AutoSize
        {
            get
            {
                return base.AutoSize;
            }
            set
            {
                base.AutoSize = value;
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler AutoSizeChanged
        {
            add
            {
                base.AutoSizeChanged += value;
            }
            remove
            {
                base.AutoSizeChanged -= value;
            }
        }

        /// <devdoc>
        ///    <para>Gets or sets the page site.</para>
        /// </devdoc>
        protected IComponentEditorPageSite PageSite {
            get { return pageSite; }
            set { pageSite = value; }
        }
        /// <devdoc>
        ///    <para>Gets or sets the component to edit.</para>
        /// </devdoc>
        protected IComponent Component {
            get { return component; }
            set { component = value; }
        }
        /// <devdoc>
        ///    <para>Indicates whether the page is being activated for the first time.</para>
        /// </devdoc>
        protected bool FirstActivate {
            get { return firstActivate; }
            set { firstActivate = value; }
        }
        /// <devdoc>
        ///    <para>Indicates whether a load is required previous to editing.</para>
        /// </devdoc>
        protected bool LoadRequired {
            get { return loadRequired; }
            set { loadRequired = value; }
        }
        /// <devdoc>
        ///    <para>Indicates if loading is taking place.</para>
        /// </devdoc>
        protected int Loading {
            get { return loading; }
            set { loading = value; }
        }

        /// <devdoc>
        ///    <para> Indicates whether an editor should apply its
        ///       changes before it is deactivated.</para>
        /// </devdoc>
        public bool CommitOnDeactivate {
            get {
                return commitOnDeactivate;
            }
            set {
                commitOnDeactivate = value;
            }
        }

        /// <devdoc>
        ///    <para>Gets or sets the creation parameters for this control.</para>
        /// </devdoc>
        protected override CreateParams CreateParams {
            get {
                CreateParams cp = base.CreateParams;
                cp.Style &= ~(NativeMethods.WS_BORDER | NativeMethods.WS_OVERLAPPED | NativeMethods.WS_DLGFRAME);
                return cp;
            }
        }

        /// <devdoc>
        ///    <para>Gets or sets the icon for this page.</para>
        /// </devdoc>
        public Icon Icon {
            
            
            get {
                if (icon == null) {
                    icon = new Icon(typeof(ComponentEditorPage), "ComponentEditorPage.ico");
                }
                return icon;
            }
            set {
                icon = value;
            }
        }

        /// <devdoc>
        ///    <para> 
        ///       Gets or sets the title of the page.</para>
        /// </devdoc>
        public virtual string Title {
            get {
                return base.Text;
            }
        }

        /// <devdoc>
        ///     Activates and displays the page.
        /// </devdoc>
        public virtual void Activate() {
            if (loadRequired) {
                EnterLoadingMode();
                LoadComponent();
                ExitLoadingMode();

                loadRequired = false;
            }
            Visible = true;
            firstActivate = false;
        }

        /// <devdoc>
        ///    <para>Applies changes to all the components being edited.</para>
        /// </devdoc>
        public virtual void ApplyChanges() {
            SaveComponent();
        }

        /// <devdoc>
        ///    <para>Deactivates and hides the page.</para>
        /// </devdoc>
        public virtual void Deactivate() {
            Visible = false;
        }

        /// <devdoc>
        ///    Increments the loading counter, which determines whether a page
        ///    is in loading mode.
        /// </devdoc>
        protected void EnterLoadingMode() {
            loading++;
        }

        /// <devdoc>
        ///    Decrements the loading counter, which determines whether a page
        ///    is in loading mode.
        /// </devdoc>
        protected void ExitLoadingMode() {
            Debug.Assert(loading > 0, "Unbalanced Enter/ExitLoadingMode calls");
            loading--;
        }

        /// <devdoc>
        ///    <para>Gets the control that represents the window for this page.</para>
        /// </devdoc>
        public virtual Control GetControl() {
            return this;
        }

        /// <devdoc>
        ///    <para>Gets the component that is to be edited.</para>
        /// </devdoc>
        protected IComponent GetSelectedComponent() {
            return component;
        }

        /// <devdoc>
        ///    <para>Processes messages that could be handled by the page.</para>
        /// </devdoc>
        public virtual bool IsPageMessage(ref Message msg) {
            return PreProcessMessage(ref msg);
        }

        /// <devdoc>
        ///    <para>Gets a value indicating whether the page is being activated for the first time.</para>
        /// </devdoc>
        protected bool IsFirstActivate() {
            return firstActivate;
        }

        /// <devdoc>
        ///    <para>Gets a value indicating whether the page is being loaded.</para>
        /// </devdoc>
        protected bool IsLoading() {
            return loading != 0;
        }

        /// <devdoc>
        ///    <para>Loads the component into the page UI.</para>
        /// </devdoc>
        protected abstract void LoadComponent();

        /// <devdoc>
        ///    <para> 
        ///       Called when the page along with its sibling
        ///       pages have applied their changes.</para>
        /// </devdoc>
        public virtual void OnApplyComplete() {
            ReloadComponent();
        }

        /// <devdoc>
        ///    <para>Called when the current component may have changed elsewhere
        ///       and needs to be reloded into the UI.</para>
        /// </devdoc>
        protected virtual void ReloadComponent() {
            if (Visible == false) {
                loadRequired = true;
            }
        }

        /// <devdoc>
        ///    <para>Saves the component from the page UI.</para>
        /// </devdoc>
        protected abstract void SaveComponent();

        /// <devdoc>
        ///    <para>Sets the page to be in dirty state.</para>
        /// </devdoc>
        protected virtual void SetDirty() {
            if (IsLoading() == false) {
                pageSite.SetDirty();
            }
        }

        /// <devdoc>
        ///    <para>Sets the component to be edited.</para>
        /// </devdoc>
        public virtual void SetComponent(IComponent component) {
            this.component = component;
            loadRequired = true;
        }

        /// <devdoc>
        ///     Sets the site for this page.
        /// </devdoc>
        public virtual void SetSite(IComponentEditorPageSite site) {
            this.pageSite = site;

            pageSite.GetControl().Controls.Add(this);
        }

        /// <devdoc>
        ///    <para> 
        ///       Provides help information to the help system.</para>
        /// </devdoc>
        public virtual void ShowHelp() {
        }

        /// <devdoc>
        ///    <para>Gets a value indicating whether the editor supports Help.</para>
        /// </devdoc>
        public virtual bool SupportsHelp() {
            return false;
        }
    }
}
