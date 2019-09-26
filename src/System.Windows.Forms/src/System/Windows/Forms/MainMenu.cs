// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using static Interop;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Represents a menu structure for a form.
    /// </summary>
    [ToolboxItemFilter("System.Windows.Forms.MainMenu")]
    public class MainMenu : Menu
    {
        internal Form _form;
        internal Form ownerForm;  // this is the form that created this menu, and is the only form allowed to dispose it.
        private RightToLeft _rightToLeft = RightToLeft.Inherit;
        private EventHandler _onCollapse;

        /// <summary>
        ///  Creates a new MainMenu control.
        /// </summary>
        public MainMenu() : base(null)
        {
        }

        /// <summary>
        ///  Initializes a new instance of the <see cref='MainMenu'/> class with the specified container.
        /// </summary>
        public MainMenu(IContainer container) : this()
        {
            if (container == null)
            {
                throw new ArgumentNullException(nameof(container));
            }

            container.Add(this);
        }

        /// <summary>
        ///  Creates a new MainMenu control with the given items to start with.
        /// </summary>
        public MainMenu(MenuItem[] items) : base(items)
        {
        }

        [SRDescription(nameof(SR.MainMenuCollapseDescr))]
        public event EventHandler Collapse
        {
            add => _onCollapse += value;
            remove => _onCollapse -= value;
        }

        /// <summary>
        ///  This is used for international applications where the language is written from RightToLeft.
        ///  When this property is true, text alignment and reading order will be from right to left.
        /// </summary>
        [Localizable(true)]
        [AmbientValue(RightToLeft.Inherit)]
        [SRDescription(nameof(SR.MenuRightToLeftDescr))]
        public virtual RightToLeft RightToLeft
        {
            get
            {
                if (_rightToLeft == RightToLeft.Inherit)
                {
                    if (_form != null)
                    {
                        return _form.RightToLeft;
                    }
                    
                    return RightToLeft.Inherit;
                }
                
                return _rightToLeft;
            }
            set
            {
                if (!ClientUtils.IsEnumValid(value, (int)value, (int)RightToLeft.No, (int)RightToLeft.Inherit))
                {
                    throw new InvalidEnumArgumentException(nameof(RightToLeft), (int)value, typeof(RightToLeft));
                }

                if (_rightToLeft != value)
                {
                    _rightToLeft = value;
                    UpdateRtl((value == RightToLeft.Yes));
                }
            }
        }

        internal override bool RenderIsRightToLeft => (RightToLeft == RightToLeft.Yes && (_form == null || !_form.IsMirrored));

        /// <summary>
        ///  Creates a new MainMenu object which is a dupliate of this one.
        /// </summary>
        public virtual MainMenu CloneMenu()
        {
            var newMenu = new MainMenu();
            newMenu.CloneMenu(this);
            return newMenu;
        }

        protected override IntPtr CreateMenuHandle() => User32.CreateMenu();

        /// <summary>
        ///  Clears out this MainMenu object and discards all of it's resources.
        ///  If the menu is parented in a form, it is disconnected from that as
        ///  well.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_form != null && (ownerForm == null || _form == ownerForm))
                {
                    _form.Menu = null;
                }
            }

            base.Dispose(disposing);
        }

        /// <summary>
        ///  Indicates which form in which we are currently residing [if any]
        /// </summary>
        public Form GetForm() => _form;

        internal override void ItemsChanged(int change)
        {
            base.ItemsChanged(change);
            _form?.MenuChanged(change, this);
        }

        internal virtual void ItemsChanged(int change, Menu menu) => _form?.MenuChanged(change, menu);

        /// <summary>
        ///  Fires the collapse event
        /// </summary>
        protected internal virtual void OnCollapse(EventArgs e) => _onCollapse?.Invoke(this, e);

        /// <summary>
        ///  Returns true if the RightToLeft should be persisted in code gen.
        /// </summary>
        internal virtual bool ShouldSerializeRightToLeft() => _rightToLeft == RightToLeft.Inherit;

        /// <summary>
        ///  Returns a string representation for this control.
        /// </summary>
        public override string ToString() => base.ToString();
    }
}
