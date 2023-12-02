// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace System.Windows.Forms;

public partial class ToolStripPanel
{
    // The FeedbackRectangle happens to encapsulate a toolstripdropdown
    // with a special region. The feedback rectangle exposes the minimum
    // API so the underlying implementation can be replaced if necessary.
    private partial class FeedbackRectangle : IDisposable
    {
        private FeedbackDropDown? _dropDown;

        public FeedbackRectangle(Rectangle bounds)
        {
            _dropDown = new FeedbackDropDown(bounds);
        }

        public bool Visible
        {
            get
            {
                if (_dropDown is not null && !_dropDown.IsDisposed)
                {
                    return _dropDown.Visible;
                }

                return false;
            }
            set
            {
                if (_dropDown is not null && !_dropDown.IsDisposed)
                {
                    _dropDown.Visible = value;
                }
            }
        }

        public void Show(Point newLocation)
        {
            _dropDown?.Show(newLocation);
        }

        public void Move(Point newLocation)
        {
            _dropDown?.MoveTo(newLocation);
        }

        protected void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_dropDown is not null)
                {
                    Visible = false;
                    _dropDown.Dispose();
                    _dropDown = null;
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }
}
