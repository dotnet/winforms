// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.



namespace System.Windows.Forms.Design {
    using System.Runtime.Remoting;
    using System.ComponentModel;

    using System.Diagnostics;

    using System;
    using System.Security.Permissions;
    using System.Drawing;
    using System.Windows.Forms;
    using System.Windows.Forms.ComponentModel;
    using System.ComponentModel.Design;
    using Microsoft.Win32;
    using System.Runtime.InteropServices;
    using System.Runtime.Versioning;

    /// <include file='doc\ComponentEditorPage.uex' path='docs/doc[@for="ComponentEditorPage"]/*' />
    /// <devdoc>
    /// <para>Provides a base implementation for a <see cref='System.Windows.Forms.Design.ComponentEditorPage'/>.</para>
    /// </devdoc>
    [ComVisible(true),
     ClassInterface(ClassInterfaceType.AutoDispatch),
     System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1012:AbstractTypesShouldNotHaveConstructors") // Shipped in Everett
    ]
    [System.Security.Permissions.PermissionSetAttribute(System.Security.Permissions.SecurityAction.InheritanceDemand, Name="FullTrust")]
    public abstract class ComponentEditorPage : Panel {

        IComponentEditorPageSite pageSite;
        IComponent component;
        bool firstActivate;
        bool loadRequired;
        int loading;
        Icon icon;
        bool commitOnDeactivate;

        /// <include file='doc\ComponentEditorPage.uex' path='docs/doc[@for="ComponentEditorPage.ComponentEditorPage"]/*' />
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


        /// <include file='doc\ComponentEditorPage.uex' path='docs/doc[@for="ComponentEditorPage.AutoSize"]/*' />
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

        /// <include file='doc\ComponentEditorPage.uex' path='docs/doc[@for="ComponentEditorPage.AutoSizeChanged"]/*' />
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

        /// <include file='doc\ComponentEditorPage.uex' path='docs/doc[@for="ComponentEditorPage.PageSite"]/*' />
        /// <devdoc>
        ///    <para>Gets or sets the page site.</para>
        /// </devdoc>
        protected IComponentEditorPageSite PageSite {
            get { return pageSite; }
            set { pageSite = value; }
        }
        /// <include file='doc\ComponentEditorPage.uex' path='docs/doc[@for="ComponentEditorPage.Component"]/*' />
        /// <devdoc>
        ///    <para>Gets or sets the component to edit.</para>
        /// </devdoc>
        protected IComponent Component {
            get { return component; }
            set { component = value; }
        }
        /// <include file='doc\ComponentEditorPage.uex' path='docs/doc[@for="ComponentEditorPage.FirstActivate"]/*' />
        /// <devdoc>
        ///    <para>Indicates whether the page is being activated for the first time.</para>
        /// </devdoc>
        protected bool FirstActivate {
            get { return firstActivate; }
            set { firstActivate = value; }
        }
        /// <include file='doc\ComponentEditorPage.uex' path='docs/doc[@for="ComponentEditorPage.LoadRequired"]/*' />
        /// <devdoc>
        ///    <para>Indicates whether a load is required previous to editing.</para>
        /// </devdoc>
        protected bool LoadRequired {
            get { return loadRequired; }
            set { loadRequired = value; }
        }
        /// <include file='doc\ComponentEditorPage.uex' path='docs/doc[@for="ComponentEditorPage.Loading"]/*' />
        /// <devdoc>
        ///    <para>Indicates if loading is taking place.</para>
        /// </devdoc>
        protected int Loading {
            get { return loading; }
            set { loading = value; }
        }

        /// <include file='doc\ComponentEditorPage.uex' path='docs/doc[@for="ComponentEditorPage.CommitOnDeactivate"]/*' />
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

        /// <include file='doc\ComponentEditorPage.uex' path='docs/doc[@for="ComponentEditorPage.CreateParams"]/*' />
        /// <devdoc>
        ///    <para>Gets or sets the creation parameters for this control.</para>
        /// </devdoc>
        protected override CreateParams CreateParams {
            [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
            get {
                CreateParams cp = base.CreateParams;
                cp.Style &= ~(NativeMethods.WS_BORDER | NativeMethods.WS_OVERLAPPED | NativeMethods.WS_DLGFRAME);
                return cp;
            }
        }

        /// <include file='doc\ComponentEditorPage.uex' path='docs/doc[@for="ComponentEditorPage.Icon"]/*' />
        /// <devdoc>
        ///    <para>Gets or sets the icon for this page.</para>
        /// </devdoc>
        public Icon Icon {
            [ResourceExposure(ResourceScope.Machine)]
            [ResourceConsumption(ResourceScope.Machine)]
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

        /// <include file='doc\ComponentEditorPage.uex' path='docs/doc[@for="ComponentEditorPage.Title"]/*' />
        /// <devdoc>
        ///    <para> 
        ///       Gets or sets the title of the page.</para>
        /// </devdoc>
        public virtual string Title {
            get {
                return base.Text;
            }
        }

        /// <include file='doc\ComponentEditorPage.uex' path='docs/doc[@for="ComponentEditorPage.Activate"]/*' />
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

        /// <include file='doc\ComponentEditorPage.uex' path='docs/doc[@for="ComponentEditorPage.ApplyChanges"]/*' />
        /// <devdoc>
        ///    <para>Applies changes to all the components being edited.</para>
        /// </devdoc>
        public virtual void ApplyChanges() {
            SaveComponent();
        }

        /// <include file='doc\ComponentEditorPage.uex' path='docs/doc[@for="ComponentEditorPage.Deactivate"]/*' />
        /// <devdoc>
        ///    <para>Deactivates and hides the page.</para>
        /// </devdoc>
        public virtual void Deactivate() {
            Visible = false;
        }

        /// <include file='doc\ComponentEditorPage.uex' path='docs/doc[@for="ComponentEditorPage.EnterLoadingMode"]/*' />
        /// <devdoc>
        ///    Increments the loading counter, which determines whether a page
        ///    is in loading mode.
        /// </devdoc>
        protected void EnterLoadingMode() {
            loading++;
        }

        /// <include file='doc\ComponentEditorPage.uex' path='docs/doc[@for="ComponentEditorPage.ExitLoadingMode"]/*' />
        /// <devdoc>
        ///    Decrements the loading counter, which determines whether a page
        ///    is in loading mode.
        /// </devdoc>
        protected void ExitLoadingMode() {
            Debug.Assert(loading > 0, "Unbalanced Enter/ExitLoadingMode calls");
            loading--;
        }

        /// <include file='doc\ComponentEditorPage.uex' path='docs/doc[@for="ComponentEditorPage.GetControl"]/*' />
        /// <devdoc>
        ///    <para>Gets the control that represents the window for this page.</para>
        /// </devdoc>
        public virtual Control GetControl() {
            return this;
        }

        /// <include file='doc\ComponentEditorPage.uex' path='docs/doc[@for="ComponentEditorPage.GetSelectedComponent"]/*' />
        /// <devdoc>
        ///    <para>Gets the component that is to be edited.</para>
        /// </devdoc>
        protected IComponent GetSelectedComponent() {
            return component;
        }

        /// <include file='doc\ComponentEditorPage.uex' path='docs/doc[@for="ComponentEditorPage.IsPageMessage"]/*' />
        /// <devdoc>
        ///    <para>Processes messages that could be handled by the page.</para>
        /// </devdoc>
        public virtual bool IsPageMessage(ref Message msg) {
            return PreProcessMessage(ref msg);
        }

        /// <include file='doc\ComponentEditorPage.uex' path='docs/doc[@for="ComponentEditorPage.IsFirstActivate"]/*' />
        /// <devdoc>
        ///    <para>Gets a value indicating whether the page is being activated for the first time.</para>
        /// </devdoc>
        protected bool IsFirstActivate() {
            return firstActivate;
        }

        /// <include file='doc\ComponentEditorPage.uex' path='docs/doc[@for="ComponentEditorPage.IsLoading"]/*' />
        /// <devdoc>
        ///    <para>Gets a value indicating whether the page is being loaded.</para>
        /// </devdoc>
        protected bool IsLoading() {
            return loading != 0;
        }

        /// <include file='doc\ComponentEditorPage.uex' path='docs/doc[@for="ComponentEditorPage.LoadComponent"]/*' />
        /// <devdoc>
        ///    <para>Loads the component into the page UI.</para>
        /// </devdoc>
        protected abstract void LoadComponent();

        /// <include file='doc\ComponentEditorPage.uex' path='docs/doc[@for="ComponentEditorPage.OnApplyComplete"]/*' />
        /// <devdoc>
        ///    <para> 
        ///       Called when the page along with its sibling
        ///       pages have applied their changes.</para>
        /// </devdoc>
        public virtual void OnApplyComplete() {
            ReloadComponent();
        }

        /// <include file='doc\ComponentEditorPage.uex' path='docs/doc[@for="ComponentEditorPage.ReloadComponent"]/*' />
        /// <devdoc>
        ///    <para>Called when the current component may have changed elsewhere
        ///       and needs to be reloded into the UI.</para>
        /// </devdoc>
        protected virtual void ReloadComponent() {
            if (Visible == false) {
                loadRequired = true;
            }
        }

        /// <include file='doc\ComponentEditorPage.uex' path='docs/doc[@for="ComponentEditorPage.SaveComponent"]/*' />
        /// <devdoc>
        ///    <para>Saves the component from the page UI.</para>
        /// </devdoc>
        protected abstract void SaveComponent();

        /// <include file='doc\ComponentEditorPage.uex' path='docs/doc[@for="ComponentEditorPage.SetDirty"]/*' />
        /// <devdoc>
        ///    <para>Sets the page to be in dirty state.</para>
        /// </devdoc>
        protected virtual void SetDirty() {
            if (IsLoading() == false) {
                pageSite.SetDirty();
            }
        }

        /// <include file='doc\ComponentEditorPage.uex' path='docs/doc[@for="ComponentEditorPage.SetComponent"]/*' />
        /// <devdoc>
        ///    <para>Sets the component to be edited.</para>
        /// </devdoc>
        public virtual void SetComponent(IComponent component) {
            this.component = component;
            loadRequired = true;
        }

        /// <include file='doc\ComponentEditorPage.uex' path='docs/doc[@for="ComponentEditorPage.SetSite"]/*' />
        /// <devdoc>
        ///     Sets the site for this page.
        /// </devdoc>
        public virtual void SetSite(IComponentEditorPageSite site) {
            this.pageSite = site;

            pageSite.GetControl().Controls.Add(this);
        }

        /// <include file='doc\ComponentEditorPage.uex' path='docs/doc[@for="ComponentEditorPage.ShowHelp"]/*' />
        /// <devdoc>
        ///    <para> 
        ///       Provides help information to the help system.</para>
        /// </devdoc>
        public virtual void ShowHelp() {
        }

        /// <include file='doc\ComponentEditorPage.uex' path='docs/doc[@for="ComponentEditorPage.SupportsHelp"]/*' />
        /// <devdoc>
        ///    <para>Gets a value indicating whether the editor supports Help.</para>
        /// </devdoc>
        public virtual bool SupportsHelp() {
            return false;
        }
    }
}
