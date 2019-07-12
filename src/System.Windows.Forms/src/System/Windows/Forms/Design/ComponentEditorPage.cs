// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;

namespace System.Windows.Forms.Design
{
    /// <summary>
    /// Provides a base implementation for a <see cref='ComponentEditorPage'/>.
    /// </summary>
    [ComVisible(true),
        ClassInterface(ClassInterfaceType.AutoDispatch)]
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
        ///  Initializes a new instance of the <see cref='ComponentEditorPage'/> class.
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
        ///  Hide the property
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
        ///  Gets or sets the page site.
        /// </summary>
        protected IComponentEditorPageSite PageSite
        {
            get { return pageSite; }
            set { pageSite = value; }
        }
        /// <summary>
        ///  Gets or sets the component to edit.
        /// </summary>
        protected IComponent Component
        {
            get { return component; }
            set { component = value; }
        }
        /// <summary>
        ///  Indicates whether the page is being activated for the first time.
        /// </summary>
        protected bool FirstActivate
        {
            get { return firstActivate; }
            set { firstActivate = value; }
        }
        /// <summary>
        ///  Indicates whether a load is required previous to editing.
        /// </summary>
        protected bool LoadRequired
        {
            get { return loadRequired; }
            set { loadRequired = value; }
        }
        /// <summary>
        ///  Indicates if loading is taking place.
        /// </summary>
        protected int Loading
        {
            get { return loading; }
            set { loading = value; }
        }

        /// <summary>
        ///  Indicates whether an editor should apply its
        ///  changes before it is deactivated.
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
        ///  Gets or sets the creation parameters for this control.
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
        ///  Gets or sets the icon for this page.
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
        ///
        ///  Gets or sets the title of the page.
        /// </summary>
        public virtual string Title
        {
            get
            {
                return base.Text;
            }
        }

        /// <summary>
        ///  Activates and displays the page.
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
        ///  Applies changes to all the components being edited.
        /// </summary>
        public virtual void ApplyChanges()
        {
            SaveComponent();
        }

        /// <summary>
        ///  Deactivates and hides the page.
        /// </summary>
        public virtual void Deactivate()
        {
            Visible = false;
        }

        /// <summary>
        ///  Increments the loading counter, which determines whether a page
        ///  is in loading mode.
        /// </summary>
        protected void EnterLoadingMode()
        {
            loading++;
        }

        /// <summary>
        ///  Decrements the loading counter, which determines whether a page
        ///  is in loading mode.
        /// </summary>
        protected void ExitLoadingMode()
        {
            loading--;
        }

        /// <summary>
        ///  Gets the control that represents the window for this page.
        /// </summary>
        public virtual Control GetControl()
        {
            return this;
        }

        /// <summary>
        ///  Gets the component that is to be edited.
        /// </summary>
        protected IComponent GetSelectedComponent()
        {
            return component;
        }

        /// <summary>
        ///  Processes messages that could be handled by the page.
        /// </summary>
        public virtual bool IsPageMessage(ref Message msg)
        {
            return PreProcessMessage(ref msg);
        }

        /// <summary>
        ///  Gets a value indicating whether the page is being activated for the first time.
        /// </summary>
        protected bool IsFirstActivate()
        {
            return firstActivate;
        }

        /// <summary>
        ///  Gets a value indicating whether the page is being loaded.
        /// </summary>
        protected bool IsLoading()
        {
            return loading != 0;
        }

        /// <summary>
        ///  Loads the component into the page UI.
        /// </summary>
        protected abstract void LoadComponent();

        /// <summary>
        ///
        ///  Called when the page along with its sibling
        ///  pages have applied their changes.
        /// </summary>
        public virtual void OnApplyComplete()
        {
            ReloadComponent();
        }

        /// <summary>
        ///  Called when the current component may have changed elsewhere
        ///  and needs to be reloded into the UI.
        /// </summary>
        protected virtual void ReloadComponent()
        {
            if (Visible == false)
            {
                loadRequired = true;
            }
        }

        /// <summary>
        ///  Saves the component from the page UI.
        /// </summary>
        protected abstract void SaveComponent();

        /// <summary>
        ///  Sets the page to be in dirty state.
        /// </summary>
        protected virtual void SetDirty()
        {
            if (!IsLoading() && pageSite != null)
            {
                pageSite.SetDirty();
            }
        }

        /// <summary>
        ///  Sets the component to be edited.
        /// </summary>
        public virtual void SetComponent(IComponent component)
        {
            this.component = component;
            loadRequired = true;
        }

        /// <summary>
        ///  Sets the site for this page.
        /// </summary>
        public virtual void SetSite(IComponentEditorPageSite site)
        {
            pageSite = site;
            pageSite?.GetControl()?.Controls.Add(this);
        }

        /// <summary>
        ///
        ///  Provides help information to the help system.
        /// </summary>
        public virtual void ShowHelp()
        {
        }

        /// <summary>
        ///  Gets a value indicating whether the editor supports Help.
        /// </summary>
        public virtual bool SupportsHelp()
        {
            return false;
        }
    }
}
