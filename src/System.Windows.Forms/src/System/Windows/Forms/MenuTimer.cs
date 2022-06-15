// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Diagnostics;

namespace System.Windows.Forms
{
    internal class MenuTimer
    {
        private readonly Timer autoMenuExpandTimer = new Timer();

        // consider - weak reference?
        private ToolStripMenuItem currentItem;
        private ToolStripMenuItem fromItem;
        private bool inTransition;

        private readonly int quickShow = 1;

        private readonly int slowShow;

        public MenuTimer()
        {
            // MenuShowDelay can be set to 0.  In this case, set to something low so it's imperceptible.
            autoMenuExpandTimer.Tick += new EventHandler(OnTick);

            // since MenuShowDelay is registry tweakable we've gotta make sure we've got some sort
            // of interval
            slowShow = Math.Max(quickShow, SystemInformation.MenuShowDelay);
        }

        // the current item to autoexpand.
        private ToolStripMenuItem CurrentItem
        {
            get
            {
                return currentItem;
            }
            set
            {
                Debug.WriteLineIf(ToolStrip.s_menuAutoExpandDebug.TraceVerbose && currentItem != value, "[MenuTimer.CurrentItem] changed: " + ((value is null) ? "null" : value.ToString()));
                currentItem = value;
            }
        }

        public bool InTransition
        {
            get { return inTransition; }
            set { inTransition = value; }
        }

        public void Start(ToolStripMenuItem item)
        {
            if (InTransition)
            {
                return;
            }

            StartCore(item);
        }

        private void StartCore(ToolStripMenuItem item)
        {
            if (item != CurrentItem)
            {
                Cancel(CurrentItem);
            }

            CurrentItem = item;
            if (item is not null)
            {
                CurrentItem = item;
                autoMenuExpandTimer.Interval = item.IsOnDropDown ? slowShow : quickShow;
                autoMenuExpandTimer.Enabled = true;
            }
        }

        public void Transition(ToolStripMenuItem fromItem, ToolStripMenuItem toItem)
        {
            Debug.WriteLineIf(ToolStrip.s_menuAutoExpandDebug.TraceVerbose, "[MenuTimer.Transition] transitioning items " + fromItem.ToString() + " " + toItem.ToString());

            if (toItem is null && InTransition)
            {
                Cancel();
                // in this case we're likely to have hit an item that's not a menu item
                // or nothing is selected.
                EndTransition(/*forceClose*/ true);
                return;
            }

            if (this.fromItem != fromItem)
            {
                this.fromItem = fromItem;
                CancelCore();
                StartCore(toItem);
            }

            // set up the current item to be the toItem so it will be auto expanded when complete.
            CurrentItem = toItem;
            InTransition = true;
        }

        public void Cancel()
        {
            if (InTransition)
            {
                return;
            }

            CancelCore();
        }

        /// <summary> cancels if and only if this item was the one that
        ///  requested the timer
        /// </summary>
        public void Cancel(ToolStripMenuItem item)
        {
            if (InTransition)
            {
                return;
            }

            if (item == CurrentItem)
            {
                CancelCore();
            }
        }

        private void CancelCore()
        {
            autoMenuExpandTimer.Enabled = false;
            CurrentItem = null;
        }

        private void EndTransition(bool forceClose)
        {
            ToolStripMenuItem lastSelected = fromItem;
            fromItem = null; // immediately clear BEFORE we call user code.
            if (InTransition)
            {
                InTransition = false;

                // we should rollup if the current item has changed and is selected.
                bool rollup = forceClose || (CurrentItem is not null && CurrentItem != lastSelected && CurrentItem.Selected);
                if (rollup && lastSelected is not null && lastSelected.HasDropDownItems)
                {
                    lastSelected.HideDropDown();
                }
            }
        }

        internal void HandleToolStripMouseLeave(ToolStrip toolStrip)
        {
            if (InTransition && toolStrip == fromItem.ParentInternal)
            {
                // restore the selection back to CurrentItem.
                // we're about to fall off the edge of the toolstrip, something should be selected
                // at all times while we're InTransition mode - otherwise it looks really funny
                // to have an auto expanded item
                if (CurrentItem is not null)
                {
                    CurrentItem.Select();
                }
            }
            else
            {
                // because we've split up selected/pressed, we need to make sure
                // that onmouseleave we make sure there's a selected menu item.
                if (toolStrip.IsDropDown && toolStrip.ActiveDropDowns.Count > 0)
                {
                    ToolStripMenuItem menuItem = (!(toolStrip.ActiveDropDowns[0] is ToolStripDropDown dropDown)) ? null : dropDown.OwnerItem as ToolStripMenuItem;
                    if (menuItem is not null && menuItem.Pressed)
                    {
                        menuItem.Select();
                    }
                }
            }
        }

        private void OnTick(object sender, EventArgs e)
        {
            autoMenuExpandTimer.Enabled = false;

            if (CurrentItem is null)
            {
                return;
            }

            EndTransition(/*forceClose*/false);
            if (CurrentItem is not null && !CurrentItem.IsDisposed && CurrentItem.Selected && CurrentItem.Enabled && ToolStripManager.ModalMenuFilter.InMenuMode)
            {
                Debug.WriteLineIf(ToolStrip.s_menuAutoExpandDebug.TraceVerbose, "[MenuTimer.OnTick] calling OnMenuAutoExpand");
                CurrentItem.OnMenuAutoExpand();
            }
        }
    }
}
