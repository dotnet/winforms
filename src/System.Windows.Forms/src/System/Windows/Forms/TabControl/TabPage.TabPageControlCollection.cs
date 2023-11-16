// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

public partial class TabPage
{
    /// <summary>
    ///  Our control collection will throw an exception if you try to add other tab pages.
    /// </summary>
    public class TabPageControlCollection : ControlCollection
    {
        /// <summary>
        ///  Creates a new TabPageControlCollection.
        /// </summary>
        public TabPageControlCollection(TabPage owner) : base(owner)
        {
        }

        /// <summary>
        ///  Adds a child control to this control. The control becomes the last control
        ///  in the child control list. If the control is already a child of another
        ///  control it is first removed from that control. The tab page overrides
        ///  this method to ensure that child tab pages are not added to it, as these
        ///  are illegal.
        /// </summary>
        public override void Add(Control? value)
        {
            if (value is TabPage)
            {
                throw new ArgumentException(SR.TabControlTabPageOnTabPage);
            }

            base.Add(value);
        }
    }
}
