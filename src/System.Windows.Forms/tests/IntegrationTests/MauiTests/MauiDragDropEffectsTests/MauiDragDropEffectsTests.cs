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
    public class MauiDragDropEffectsTests : ReflectBase
    {
        private Point ptFrom, ptTo;
        private TextBox tb1, tb2;
        private TParams tp;
        private DragDropEffects ddEffect;

        public MauiDragDropEffectsTests(String[] args) : base(args)
        {
            this.BringToForeground();
        }

        [STAThread]
        public static void Main(String[] args)
        {
            Thread.CurrentThread.SetCulture("en-US");
            Application.Run(new MauiDragDropEffectsTests(args));
        }

        private void Setup()
        {
            this.Controls.Clear();
            tb1 = new TextBox();
            tb2 = new TextBox();

            tb1.Location = new Point(0, 0);
            tb2.Location = new Point(0, tb1.Size.Height);

            tb2.AllowDrop = true;
            tb1.Text = tp.ru.GetString(10);
            tb1.MouseDown += new MouseEventHandler(tb_MouseDown);
            tb2.DragEnter += new DragEventHandler(tb_DragEnter);
            tb2.DragDrop += new DragEventHandler(tb_DragDrop);

            this.Controls.Add(tb1);
            this.Controls.Add(tb2);

            ptFrom = this.PointToScreen(tb1.Location);
            ptTo = this.PointToScreen(tb2.Location);
        }
        //==========================================
        // Test Methods
        // Effect Move: Drag Move text between 2 textboxes.
        // Effect Copy: Drag Copy text from one Windows.Forms app to another.
        // Effect Link: Drag Open a file into a textbox/MDI App.
        // Effect None: Drag Drop a file onto a control which won't accept it.
        //==========================================

        private void tb_MouseDown(Object sender, MouseEventArgs e)
        {
            TextBox tb = ((TextBox)sender);
            DragDropEffects dde = tb.DoDragDrop(tb.Text, DragDropEffects.All);
            tp.log.WriteLine("Started DragDrop with text: " + tb.Text);
            if (dde == DragDropEffects.Move)
            {
                tb.Text = "";
            }
        }
        private void tb_DragEnter(Object sender, DragEventArgs e)
        {
            IDataObject data = e.Data;
            if (data.GetDataPresent("Text"))
            {
                e.Effect = ddEffect;
                tp.log.WriteLine("Text Data Present");
            }
        }
        private void tb_DragDrop(object sender, DragEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            IDataObject data = e.Data;
            if (data.GetDataPresent("Text"))
            {
                tb.Text = (string)data.GetData("Text");
                tp.log.WriteLine("Text Set: " + tb.Text);
            }
            else
            {
                tp.log.WriteLine("Text Data Not Present!!!");
            }
        }

        [Scenario(true)]
        public ScenarioResult DragDropCopy(TParams p)
        {
            tp = p;
            Setup();
            ddEffect = DragDropEffects.Copy;
            Mouse.ClickDrag(MouseFlags.LeftButton, ptFrom.X + 5, ptFrom.Y + 5, ptTo.X + 5, ptTo.Y + 5);
            Application.DoEvents();

            return new ScenarioResult(tb1.Text.Equals(tb2.Text), "DragDrop Copy not successful");
        }

        [Scenario(true)]
        public ScenarioResult DragDropNone(TParams p)
        {
            tp = p;
            Setup();
            string str = tb1.Text;
            ddEffect = DragDropEffects.None;
            Mouse.ClickDrag(MouseFlags.LeftButton, ptFrom.X + 5, ptFrom.Y + 5, ptTo.X + 5, ptTo.Y + 5);
            Application.DoEvents();

            return new ScenarioResult(tb1.Text.Equals(str) && tb2.Text.Equals(""), "DragDrop None not successful");
        }

        [Scenario(true)]
        public ScenarioResult DragDropMove(TParams p)
        {
            tp = p;
            Setup();
            String str = tb1.Text;
            ddEffect = DragDropEffects.Move;
            Mouse.ClickDrag(MouseFlags.LeftButton, ptFrom.X + 5, ptFrom.Y + 5, ptTo.X + 5, ptTo.Y + 5);
            Application.DoEvents();
            return new ScenarioResult(tb1.Text.Equals("") && tb2.Text.Equals(str), "DragDrop Movenot successful");
        }

        [Scenario(true)]
        public ScenarioResult DragDropMoveBetweenForms(TParams p)
        {
            tp = p;
            Setup();
            Form f = new Form();
            this.Controls.Remove(tb2);
            f.Controls.Add(tb2);
            f.Show();
            f.Location = new Point(this.Location.X + this.Size.Width, this.Location.Y);
            ptTo = f.PointToScreen(tb2.Location);
            String str = tb1.Text;
            ddEffect = DragDropEffects.Move;
            Mouse.ClickDrag(MouseFlags.LeftButton, ptFrom.X + 5, ptFrom.Y + 5, ptTo.X + 5, ptTo.Y + 5);
            Application.DoEvents();
            return new ScenarioResult(tb1.Text.Equals("") && tb2.Text.Equals(str), "DragDrop Move between forms not successful");
        }
    }
}
