// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Drawing;
using System.Windows.Forms;
using WFCTestLib.Util;
using WFCTestLib.Log;
using ReflectTools;
using System.Threading;
using Maui.Core;
using System.Windows.Forms.IntegrationTests.Common;

namespace System.Windows.Forms.IntegrationTests.MauiTests
{
    public class MauiContextMenuTests : ReflectBase
    {
        public MauiContextMenuTests(string[] args) : base(args)
        {
            this.BringToForeground();
        }

        /**
        * Calls static method LaunchTest to start the test
        */
        public static void Main(string[] args)
        {
            Thread.CurrentThread.SetCulture("en-US");
            Application.Run(new MauiContextMenuTests(args));
        }

        bool bClicked;
        private Point relativeLocation;
        private void MenuItem_OnClick(Object sender, EventArgs e)
        {
            bClicked = true;
        }
        private void OnPopup(Object sender, EventArgs e)
        {
            Point cmLocation = this.PointToScreen(relativeLocation);
            Mouse.Click(MouseFlags.LeftButton, cmLocation.X + 5, cmLocation.Y + 5);

        }
        private void OnPopup2(Object sender, EventArgs e)
        {
            Point cmLocation = this.PointToScreen(relativeLocation);
            Mouse.Click(MouseFlags.LeftButton, cmLocation.X - 10, cmLocation.Y + 5);
        }
        //==========================================
        // Test Methods
        //==========================================

        [Scenario(true)]
        public ScenarioResult ShowContextMenu(TParams p)
        {
            this.ContextMenu = new ContextMenu();
            String menuItemText = "MenuItem1";
            this.ContextMenu.MenuItems.Add(menuItemText);
            this.ContextMenu.MenuItems[0].Click += (new EventHandler(MenuItem_OnClick));
            this.ContextMenu.Popup += (new EventHandler(OnPopup2));
            relativeLocation = new Point(this.Size.Width / 2, this.Size.Height / 2);

            bClicked = false;
            this.ContextMenu.Show(this, relativeLocation, LeftRightAlignment.Left);
            Application.DoEvents();
            return new ScenarioResult(bClicked, "MenuItem not clicked!");
        }

        [Scenario(true)]
        public ScenarioResult ShowContextMenuByRightClick(TParams p)
        {
            this.ContextMenu = new ContextMenu();
            String menuItemText = "MenuItem1";
            this.ContextMenu.MenuItems.Add(menuItemText);
            this.ContextMenu.MenuItems[0].Click += (new EventHandler(MenuItem_OnClick));
            this.ContextMenu.Popup += (new EventHandler(OnPopup));
            relativeLocation = new Point(this.Size.Width / 2, this.Size.Height / 2);

            bClicked = false;
            Point cmLocation = this.PointToScreen(relativeLocation);
            Mouse.Click(MouseFlags.RightButton, cmLocation.X, cmLocation.Y);
            Application.DoEvents();
            return new ScenarioResult(bClicked, "MenuItem not clicked!");
        }

        [Scenario(true)]
        public ScenarioResult ShowContextMenuOnButtonByRightClick(TParams p)
        {
            Button btn = new Button();
            btn.ContextMenu = new ContextMenu();
            btn.ContextMenu.MenuItems.Add("MenuItem");
            btn.ContextMenu.MenuItems[0].Click += (new EventHandler(MenuItem_OnClick));
            btn.ContextMenu.Popup += (new EventHandler(OnPopup));
            btn.Location = new Point(100, 100);
            this.Controls.Clear();
            this.Controls.Add(btn);

            // We should never pop-up the form's context menu
            this.ContextMenu = new ContextMenu();
            this.ContextMenu.MenuItems.Add("****");
            this.ContextMenu.Popup += (new EventHandler(OnPopup));

            relativeLocation = new Point(btn.Size.Width / 2 + btn.Location.X, btn.Size.Height / 2 + btn.Location.Y);

            bClicked = false;
            Point cmLocation = this.PointToScreen(relativeLocation);
            //Thread.Sleep(1000);
            Mouse.Click(MouseFlags.RightButton, cmLocation.X, cmLocation.Y);
            Application.DoEvents();
            return new ScenarioResult(bClicked, "MenuItem not clicked!");
        }

        [Scenario(true)]
        public ScenarioResult ShowContextMenuOnTextboxByRightClick(TParams p)
        {
            TextBox tb = new TextBox();
            tb.ContextMenu = new ContextMenu();
            tb.ContextMenu.MenuItems.Add("MenuItem4");
            tb.ContextMenu.MenuItems[0].Click += (new EventHandler(MenuItem_OnClick));
            tb.ContextMenu.Popup += (new EventHandler(OnPopup));
            tb.Location = new Point(100, 100);
            this.Controls.Clear();
            this.Controls.Add(tb);

            // We should never pop-up the form's context menu
            this.ContextMenu = new ContextMenu();
            this.ContextMenu.MenuItems.Add("****");
            this.ContextMenu.Popup += (new EventHandler(OnPopup));

            relativeLocation = new Point(tb.Size.Width / 2 + tb.Location.X, tb.Size.Height / 2 + tb.Location.Y);

            bClicked = false;
            Point cmLocation = this.PointToScreen(relativeLocation);
            //Thread.Sleep(1000);
            Mouse.Click(MouseFlags.RightButton, cmLocation.X, cmLocation.Y);
            Application.DoEvents();
            return new ScenarioResult(bClicked, "MenuItem not clicked!");
        }
    }
}

// [Scenarios]
//@ 1) ShowContextMenu
//@ 2) ShowContextMenuByRightClick
//@ 3) ShowContextMenuOnButtonByRightClick
//@ 4) ShowContextMenuOnTextboxByRightClick
