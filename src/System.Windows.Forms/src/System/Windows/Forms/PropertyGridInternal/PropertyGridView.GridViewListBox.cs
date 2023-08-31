// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.PropertyGridInternal;

internal partial class PropertyGridView
{
    internal class GridViewListBox : ListBox
    {
        private bool _inSetSelectedIndex;
        private readonly PropertyGridView _owningPropertyGridView;

        public GridViewListBox(PropertyGridView gridView)
        {
            ArgumentNullException.ThrowIfNull(gridView);

            IntegralHeight = false;
            _owningPropertyGridView = gridView;
            base.BackColor = gridView.BackColor;
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.Style &= ~(int)WINDOW_STYLE.WS_BORDER;
                cp.ExStyle &= ~(int)WINDOW_EX_STYLE.WS_EX_CLIENTEDGE;
                return cp;
            }
        }

        /// <summary>
        ///  Gets the owning PropertyGridView.
        /// </summary>
        internal PropertyGridView OwningPropertyGridView
            => _owningPropertyGridView;

        /// <summary>
        ///  Indicates whether or not the control supports UIA Providers via
        ///  IRawElementProviderFragment/IRawElementProviderFragmentRoot interfaces
        /// </summary>
        internal override bool SupportsUiaProviders => true;

        /// <summary>
        ///  Constructs the new instance of the accessibility object for this control.
        /// </summary>
        /// <returns>The accessibility object instance.</returns>
        protected override AccessibleObject CreateAccessibilityInstance()
            => new GridViewListBoxAccessibleObject(this);

        public virtual bool InSetSelectedIndex() => _inSetSelectedIndex;

        protected override void OnSelectedIndexChanged(EventArgs e)
        {
            _inSetSelectedIndex = true;
            base.OnSelectedIndexChanged(e);
            _inSetSelectedIndex = false;
        }
    }
}
