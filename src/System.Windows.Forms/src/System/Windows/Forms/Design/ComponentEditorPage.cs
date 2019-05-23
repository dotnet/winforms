// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.



namespace System.Windows.Forms.Design
{
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

    /// <summary>
    /// <para>Provides a base implementation for a <see cref='System.Windows.Forms.Design.ComponentEditorPage'/>.</para>
    /// </summary>
    [ComVisible(true),
     ClassInterface(ClassInterfaceType.AutoDispatch),
     System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1012:AbstractTypesShouldNotHaveConstructors") // Shipped in Everett
    ]
    public abstract class ComponentEditorPage : Panel
    {

        IComponentEditorPageSite pageSite;
        IComponent component;
        bool firstActivate;
        bool loadRequired;
        int loading;
        Icon icon;
        bool commitOnDeactivate;

        /// <summary>
        ///    <para>
        ///       Initializes a new instance of the <see cref='System.Windows.Forms.Design.ComponentEditorPage'/> class.
        ///    </para>
        /// </summary>
        public ComponentEditorPage() : base()
        {
            commitOnDeactivate = false;
            firstActivate = true;
            loadRequired = false;
            loading = 0;

            Visible = false;
        }


        /// <summary>
        ///    <para>
        ///       Hide the property
        ///    </para>
        /// </summary>
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
            add => base.AutoSizeChanged += value;
            remove => base.AutoSizeChanged -= value;
        }

        /// <summary>
        ///    <para>Gets or sets the page site.</para>
        /// </summary>
        protected IComponentEditorPageSite PageSite
        {
            get { return pageSite; }
            set { pageSite = value; }
        }
        /// <summary>
        ///    <para>Gets or sets the component to edit.</para>
        /// </summary>
        protected IComponent Component
        {
            get { return component; }
            set { component = value; }
        }
        /// <summary>
        ///    <para>Indicates whether the page is being activated for the first time.</para>
        /// </summary>
        protected bool FirstActivate
        {
            get { return firstActivate; }
            set { firstActivate = value; }
        }
        /// <summary>
        ///    <para>Indicates whether a load is required previous to editing.</para>
        /// </summary>
        protected bool LoadRequired
        {
            get { return loadRequired; }
            set { loadRequired = value; }
        }
        /// <summary>
        ///    <para>Indicates if loading is taking place.</para>
        /// </summary>
        protected int Loading
        {
            get { return loading; }
            set { loading = value; }
        }

        /// <summary>
        ///    <para> Indicates whether an editor should apply its
        ///       changes before it is deactivated.</para>
        /// </summary>
        public bool CommitOnDeactivate
        {
            get
            {
                return commitOnDeactivate;
            }
            set
            {
                commitOnDeactivate = value;
            }
        }

        /// <summary>
        ///    <para>Gets or sets the creation parameters for this control.</para>
        /// </summary>
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.Style &= ~(NativeMethods.WS_BORDER | NativeMethods.WS_OVERLAPPED | NativeMethods.WS_DLGFRAME);
                return cp;
            }
        }

        /// <summary>
        ///    <para>Gets or sets the icon for this page.</para>
        /// </summary>
        public Icon Icon
        {


            get
            {
                if (icon == null)
                {
                    icon = new Icon(typeof(ComponentEditorPage), "ComponentEditorPage");
                }
                return icon;
            }
            set
            {
                icon = value;
            }
        }

        /// <summary>
        ///    <para> 
        ///       Gets or sets the title of the page.</para>
        /// </summary>
        public virtual string Title
        {
            get
            {
                return base.Text;
            }
        }

        /// <summary>
        ///     Activates and displays the page.
        /// </summary>
        public virtual void Activate()
        {
            if (loadRequired)
            {
                EnterLoadingMode();
                LoadComponent();
                ExitLoadingMode();

                loadRequired = false;
            }
            Visible = true;
            firstActivate = false;
        }

        /// <summary>
        ///    <para>Applies changes to all the components being edited.</para>
        /// </summary>
        public virtual void ApplyChanges()
        {
            SaveComponent();
        }

        /// <summary>
        ///    <para>Deactivates and hides the page.</para>
        /// </summary>
        public virtual void Deactivate()
        {
            Visible = false;
        }

        /// <summary>
        ///    Increments the loading counter, which determines whether a page
        ///    is in loading mode.
        /// </summary>
        protected void EnterLoadingMode()
        {
            loading++;
        }

        /// <summary>
        ///    Decrements the loading counter, which determines whether a page
        ///    is in loading mode.
        /// </summary>
        protected void ExitLoadingMode()
        {
            Debug.Assert(loading > 0, "Unbalanced Enter/ExitLoadingMode calls");
            loading--;
        }

        /// <summary>
        ///    <para>Gets the control that represents the window for this page.</para>
        /// </summary>
        public virtual Control GetControl()
        {
            return this;
        }

        /// <summary>
        ///    <para>Gets the component that is to be edited.</para>
        /// </summary>
        protected IComponent GetSelectedComponent()
        {
            return component;
        }

        /// <summary>
        ///    <para>Processes messages that could be handled by the page.</para>
        /// </summary>
        public virtual bool IsPageMessage(ref Message msg)
        {
            return PreProcessMessage(ref msg);
        }

        /// <summary>
        ///    <para>Gets a value indicating whether the page is being activated for the first time.</para>
        /// </summary>
        protected bool IsFirstActivate()
        {
            return firstActivate;
        }

        /// <summary>
        ///    <para>Gets a value indicating whether the page is being loaded.</para>
        /// </summary>
        protected bool IsLoading()
        {
            return loading != 0;
        }

        /// <summary>
        ///    <para>Loads the component into the page UI.</para>
        /// </summary>
        protected abstract void LoadComponent();

        /// <summary>
        ///    <para> 
        ///       Called when the page along with its sibling
        ///       pages have applied their changes.</para>
        /// </summary>
        public virtual void OnApplyComplete()
        {
            ReloadComponent();
        }

        /// <summary>
        ///    <para>Called when the current component may have changed elsewhere
        ///       and needs to be reloded into the UI.</para>
        /// </summary>
        protected virtual void ReloadComponent()
        {
            if (Visible == false)
            {
                loadRequired = true;
            }
        }

        /// <summary>
        ///    <para>Saves the component from the page UI.</para>
        /// </summary>
        protected abstract void SaveComponent();

        /// <summary>
        ///    <para>Sets the page to be in dirty state.</para>
        /// </summary>
        protected virtual void SetDirty()
        {
            if (IsLoading() == false)
            {
                pageSite.SetDirty();
            }
        }

        /// <summary>
        ///    <para>Sets the component to be edited.</para>
        /// </summary>
        public virtual void SetComponent(IComponent component)
        {
            this.component = component;
            loadRequired = true;
        }

        /// <summary>
        ///     Sets the site for this page.
        /// </summary>
        public virtual void SetSite(IComponentEditorPageSite site)
        {
            this.pageSite = site;

            pageSite.GetControl().Controls.Add(this);
        }

        /// <summary>
        ///    <para> 
        ///       Provides help information to the help system.</para>
        /// </summary>
        public virtual void ShowHelp()
        {
        }

        /// <summary>
        ///    <para>Gets a value indicating whether the editor supports Help.</para>
        /// </summary>
        public virtual bool SupportsHelp()
        {
            return false;
        }
    }
}
