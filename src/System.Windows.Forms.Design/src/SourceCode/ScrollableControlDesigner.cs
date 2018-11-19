// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;

namespace System.Windows.Forms.Design
{
    /// <summary>
    ///     The ScrollableControlDesigner class builds on the ParentControlDesigner, and adds the implementation
    ///     of IWinFormsDesigner so that the designer can be hosted as a document.
    /// </summary>
    public class ScrollableControlDesigner : ParentControlDesigner
    {
        /// <summary>
        ///     Overrides the base class's GetHitTest method to determine regions of the
        ///     control that should always be UI-Active.  For a form, if it has autoscroll
        ///     set the scroll bars are always UI active.
        /// </summary>
        protected override bool GetHitTest(Point pt)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///     We override our base class's WndProc to monitor certain messages.
        /// </summary>
        protected override void WndProc(ref Message m)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }
    }
}
