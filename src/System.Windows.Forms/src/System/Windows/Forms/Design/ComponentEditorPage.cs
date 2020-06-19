// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using static Interop;

namespace System.Windows.Forms.Design
{
    /// <summary>
    ///  Provides a base implementation for a <see cref='ComponentEditorPage'/>.
    /// </summary>
    public abstract class ComponentEditorPage : Panel
    {
        private Icon _icon;

        /// <summary>
        ///  Initializes a new instance of the <see cref='ComponentEditorPage'/> class.
        /// </summary>
        public ComponentEditorPage() : base()
        {
            Visible = false;
        }

        /// <summary>
        ///  Hide the property
        /// </summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override bool AutoSize
        {
            get => base.AutoSize;
            set => base.AutoSize = value;
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new event EventHandler AutoSizeChanged
        {
            add => base.AutoSizeChanged += value;
            remove => base.AutoSizeChanged -= value;
        }

        /// <summary>
        ///  Gets or sets the page site
        /// </summary>
        protected IComponentEditorPageSite PageSite { get; set; }

        /// <summary>
        ///  Gets or sets the component to edit
        /// </summary>
        protected IComponent Component { get; set; }

        /// <summary>
        ///  Indicates whether the page is being activated for the first time
        /// </summary>
        protected bool FirstActivate { get; set; } = true;

        /// <summary>
        ///  Indicates whether a load is required previous to editing
        /// </summary>
        protected bool LoadRequired { get; set; }

        /// <summary>
        ///  Indicates if loading is taking place
        /// </summary>
        protected int Loading { get; set; }

        /// <summary>
        ///  Indicates whether an editor should apply its changes before it is deactivated
        /// </summary>
        public bool CommitOnDeactivate { get; set; }

        /// <summary>
        ///  Gets or sets the creation parameters for this control
        /// </summary>
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.Style &= ~(int)(User32.WS.BORDER | User32.WS.OVERLAPPED | User32.WS.DLGFRAME);
                return cp;
            }
        }

        /// <summary>
        ///  Gets or sets the icon for this page
        /// </summary>
        public Icon Icon
        {
            get =>_icon ??= new Icon(typeof(ComponentEditorPage), "ComponentEditorPage");
            set => _icon = value;
        }

        /// <summary>
        ///  Gets or sets the title of the page
        /// </summary>
        public virtual string Title => base.Text;

        /// <summary>
        ///  Activates and displays the page.
        /// </summary>
        public virtual void Activate()
        {
            if (LoadRequired)
            {
                EnterLoadingMode();
                LoadComponent();
                ExitLoadingMode();

                LoadRequired = false;
            }

            Visible = true;
            FirstActivate = false;
        }

        /// <summary>
        ///  Applies changes to all the components being edited
        /// </summary>
        public virtual void ApplyChanges() => SaveComponent();

        /// <summary>
        ///  Deactivates and hides the page
        /// </summary>
        public virtual void Deactivate()
        {
            Visible = false;
        }

        /// <summary>
        ///  Increments the loading counter, which determines whether a page is in loading mode.
        /// </summary>
        protected void EnterLoadingMode() => Loading++;

        /// <summary>
        ///  Decrements the loading counter, which determines whether a page is in loading mode.
        /// </summary>
        protected void ExitLoadingMode()
        {
            if (Loading == 0)
            {
                return;
            }

            Loading--;
        }

        /// <summary>
        ///  Gets the control that represents the window for this page
        /// </summary>
        public virtual Control GetControl() => this;

        /// <summary>
        ///  Gets the component that is to be edited
        /// </summary>
        protected IComponent GetSelectedComponent() => Component;

        /// <summary>
        ///  Processes messages that could be handled by the page
        /// </summary>
        public virtual bool IsPageMessage(ref Message msg) => PreProcessMessage(ref msg);

        /// <summary>
        ///  Gets a value indicating whether the page is being activated for the first time
        /// </summary>
        protected bool IsFirstActivate() => FirstActivate;

        /// <summary>
        ///  Gets a value indicating whether the page is being loaded
        /// </summary>
        protected bool IsLoading() => Loading != 0;

        /// <summary>
        ///  Loads the component into the page UI
        /// </summary>
        protected abstract void LoadComponent();

        /// <summary>
        ///  Called when the page along with its sibling pages have applied their changes
        /// </summary>
        public virtual void OnApplyComplete() => ReloadComponent();

        /// <summary>
        ///  Called when the current component may have changed elsewhere and needs to be reloded into the UI
        /// </summary>
        protected virtual void ReloadComponent()
        {
            if (!Visible)
            {
                LoadRequired = true;
            }
        }

        /// <summary>
        ///  Saves the component from the page UI
        /// </summary>
        protected abstract void SaveComponent();

        /// <summary>
        ///  Sets the page to be in dirty state
        /// </summary>
        protected virtual void SetDirty()
        {
            if (!IsLoading() && PageSite != null)
            {
                PageSite.SetDirty();
            }
        }

        /// <summary>
        ///  Sets the component to be edited
        /// </summary>
        public virtual void SetComponent(IComponent component)
        {
            Component = component;
            LoadRequired = true;
        }

        /// <summary>
        ///  Sets the site for this page.
        /// </summary>
        public virtual void SetSite(IComponentEditorPageSite site)
        {
            PageSite = site;
            site?.GetControl()?.Controls.Add(this);
        }

        /// <summary>
        ///  Provides help information to the help system
        /// </summary>
        public virtual void ShowHelp()
        {
        }

        /// <summary>
        ///  Gets a value indicating whether the editor supports Help
        /// </summary>
        public virtual bool SupportsHelp() => false;
    }
}
