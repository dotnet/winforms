// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms.PropertyGridInternal
{
    internal partial class PropertyGridView
    {
        private class GridViewListBox : ListBox
        {
            internal bool fInSetSelectedIndex = false;
            private readonly PropertyGridView _owningPropertyGridView;

            public GridViewListBox(PropertyGridView gridView)
            {
                base.IntegralHeight = false;
                _owningPropertyGridView = gridView;
                base.BackColor = gridView.BackColor;
            }

            protected override CreateParams CreateParams
            {
                get
                {
                    CreateParams cp = base.CreateParams;
                    cp.Style &= ~NativeMethods.WS_BORDER;
                    cp.ExStyle &= ~NativeMethods.WS_EX_CLIENTEDGE;
                    return cp;
                }
            }

            /// <summary>
            ///  Gets the owning PropertyGridView.
            /// </summary>
            internal PropertyGridView OwningPropertyGridView
            {
                get
                {
                    return _owningPropertyGridView;
                }
            }

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
            {
                return new GridViewListBoxAccessibleObject(this);
            }

            public virtual bool InSetSelectedIndex()
            {
                return fInSetSelectedIndex;
            }

            protected override void OnSelectedIndexChanged(EventArgs e)
            {
                fInSetSelectedIndex = true;
                base.OnSelectedIndexChanged(e);
                fInSetSelectedIndex = false;

                if (AccessibilityObject is GridViewListBoxAccessibleObject gridViewListBoxAccessibleObject)
                {
                    gridViewListBoxAccessibleObject.SetListBoxItemFocus();
                }
            }

        }
    }
}
