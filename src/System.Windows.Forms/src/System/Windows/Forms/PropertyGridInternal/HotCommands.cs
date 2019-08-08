// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel.Design;
using System.Drawing;
using System.Text;

namespace System.Windows.Forms.PropertyGridInternal
{
    internal class HotCommands : PropertyGrid.SnappableControl
    {
        private object component;
        private DesignerVerb[] verbs;
        private LinkLabel label;
        private bool allowVisible = true;
        private int optimalHeight = -1;

        internal HotCommands(PropertyGrid owner) : base(owner)
        {
            Text = "Command Pane";
        }

        public virtual bool AllowVisible
        {
            get
            {
                return allowVisible;
            }
            set
            {
                if (allowVisible != value)
                {
                    allowVisible = value;
                    if (value && WouldBeVisible)
                    {
                        Visible = true;
                    }
                    else
                    {
                        Visible = false;
                    }
                }
            }
        }

        /// <summary>
        ///  Constructs the new instance of the accessibility object for this control.
        /// </summary>
        /// <returns>The accessibility object for this control.</returns>
        protected override AccessibleObject CreateAccessibilityInstance()
        {
            return new HotCommandsAccessibleObject(this, ownerGrid);
        }

        public override Rectangle DisplayRectangle
        {
            get
            {
                Size sz = ClientSize;
                return new Rectangle(4, 4, sz.Width - 8, sz.Height - 8);
            }
        }

        public LinkLabel Label
        {
            get
            {
                if (label == null)
                {
                    label = new LinkLabel
                    {
                        Dock = DockStyle.Fill,
                        LinkBehavior = LinkBehavior.AlwaysUnderline,

                        // use default LinkLabel colors for regular, active, and visited
                        DisabledLinkColor = SystemColors.ControlDark
                    };
                    label.LinkClicked += new LinkLabelLinkClickedEventHandler(LinkClicked);
                    Controls.Add(label);
                }
                return label;
            }
        }

        public virtual bool WouldBeVisible
        {
            get
            {
                return (component != null);
            }
        }

        public override int GetOptimalHeight(int width)
        {
            if (optimalHeight == -1)
            {
                int lineHeight = (int)(1.5 * Font.Height);
                int verbCount = 0;
                if (verbs != null)
                {
                    verbCount = verbs.Length;
                }
                optimalHeight = verbCount * lineHeight + 8;
            }
            return optimalHeight;
        }
        public override int SnapHeightRequest(int request)
        {
            return request;
        }

        /// <summary>
        ///  Indicates whether or not the control supports UIA Providers via
        ///  IRawElementProviderFragment/IRawElementProviderFragmentRoot interfaces.
        /// </summary>
        internal override bool SupportsUiaProviders => true;

        private void LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                if (!e.Link.Enabled)
                {
                    return;
                }

                ((DesignerVerb)e.Link.LinkData).Invoke();
            }
            catch (Exception ex)
            {
                RTLAwareMessageBox.Show(this, ex.Message, SR.PBRSErrorTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning,
                        MessageBoxDefaultButton.Button1, 0);
            }
        }

        private void OnCommandChanged(object sender, EventArgs e)
        {
            SetupLabel();
        }

        protected override void OnGotFocus(EventArgs e)
        {
            Label.Focus();
            Label.Invalidate();
        }

        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
            optimalHeight = -1;
        }

        internal void SetColors(Color background, Color normalText, Color link, Color activeLink, Color visitedLink, Color disabledLink)
        {
            Label.BackColor = background;
            Label.ForeColor = normalText;
            Label.LinkColor = link;
            Label.ActiveLinkColor = activeLink;
            Label.VisitedLinkColor = visitedLink;
            Label.DisabledLinkColor = disabledLink;
        }

        public void Select(bool forward)
        {
            Label.Focus();
        }

        public virtual void SetVerbs(object component, DesignerVerb[] verbs)
        {
            if (this.verbs != null)
            {
                for (int i = 0; i < this.verbs.Length; i++)
                {
                    this.verbs[i].CommandChanged -= new EventHandler(OnCommandChanged);
                }
                this.component = null;
                this.verbs = null;
            }

            if (component == null || verbs == null || verbs.Length == 0)
            {
                Visible = false;
                Label.Links.Clear();
                Label.Text = null;
            }
            else
            {
                this.component = component;
                this.verbs = verbs;

                for (int i = 0; i < verbs.Length; i++)
                {
                    verbs[i].CommandChanged += new EventHandler(OnCommandChanged);
                }

                if (allowVisible)
                {
                    Visible = true;
                }
                SetupLabel();
            }

            optimalHeight = -1;
        }

        private void SetupLabel()
        {
            Label.Links.Clear();
            StringBuilder sb = new StringBuilder();
            Point[] links = new Point[verbs.Length];
            int charLoc = 0;
            bool firstVerb = true;

            for (int i = 0; i < verbs.Length; i++)
            {
                if (verbs[i].Visible && verbs[i].Supported)
                {
                    if (!firstVerb)
                    {
                        sb.Append(Application.CurrentCulture.TextInfo.ListSeparator);
                        sb.Append(' ');
                        charLoc += 2;
                    }
                    string name = verbs[i].Text;

                    links[i] = new Point(charLoc, name.Length);
                    sb.Append(name);
                    charLoc += name.Length;
                    firstVerb = false;
                }
            }

            Label.Text = sb.ToString();

            for (int i = 0; i < verbs.Length; i++)
            {
                if (verbs[i].Visible && verbs[i].Supported)
                {
                    LinkLabel.Link link = Label.Links.Add(links[i].X, links[i].Y, verbs[i]);
                    if (!verbs[i].Enabled)
                    {
                        link.Enabled = false;
                    }
                }
            }
        }

    }

    /// <summary>
    ///  Represents the hot commands control accessible object.
    /// </summary>
    [Runtime.InteropServices.ComVisible(true)]
    internal class HotCommandsAccessibleObject : Control.ControlAccessibleObject
    {
        private readonly PropertyGrid _parentPropertyGrid;

        /// <summary>
        ///  Initializes new instance of DocCommentAccessibleObject.
        /// </summary>
        /// <param name="owningHotCommands">The owning HotCommands control.</param>
        /// <param name="parentPropertyGrid">The parent PropertyGrid control.</param>
        public HotCommandsAccessibleObject(HotCommands owningHotCommands, PropertyGrid parentPropertyGrid) : base(owningHotCommands)
        {
            _parentPropertyGrid = parentPropertyGrid;
        }

        /// <summary>
        ///  Request to return the element in the specified direction.
        /// </summary>
        /// <param name="direction">Indicates the direction in which to navigate.</param>
        /// <returns>Returns the element in the specified direction.</returns>
        internal override UnsafeNativeMethods.IRawElementProviderFragment FragmentNavigate(UnsafeNativeMethods.NavigateDirection direction)
        {
            if (_parentPropertyGrid.AccessibilityObject is PropertyGridAccessibleObject propertyGridAccessibleObject)
            {
                UnsafeNativeMethods.IRawElementProviderFragment navigationTarget = propertyGridAccessibleObject.ChildFragmentNavigate(this, direction);
                if (navigationTarget != null)
                {
                    return navigationTarget;
                }
            }

            return base.FragmentNavigate(direction);
        }

        /// <summary>
        ///  Request value of specified property from an element.
        /// </summary>
        /// <param name="propertyId">Identifier indicating the property to return</param>
        /// <returns>Returns a ValInfo indicating whether the element supports this property, or has no value for it.</returns>
        internal override object GetPropertyValue(int propertyID)
        {
            if (propertyID == NativeMethods.UIA_ControlTypePropertyId)
            {
                return NativeMethods.UIA_PaneControlTypeId;
            }
            else if (propertyID == NativeMethods.UIA_NamePropertyId)
            {
                return Name;
            }

            return base.GetPropertyValue(propertyID);
        }
    }
}
