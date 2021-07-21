// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using System.Windows.Forms.IntegrationTests.Common;
using ReflectTools;
using WFCTestLib.Log;

namespace System.Windows.Forms.IntegrationTests.MauiTests
{
    public class MauiMDITests : ReflectBase
    {
        public MauiMDITests(string[] args) : base(args)
        {
            this.BringToForeground();
            IsMdiContainer = true;
            ClientSize = new Size(640, 480);
        }

        public static void Main(string[] args)
        {
            Thread.CurrentThread.SetCulture("en-US");
            Application.Run(new MauiMDITests(args));
        }

        [Scenario(true)]
        public ScenarioResult Resize_When_MdiChildrenMinimizedAnchorBottom_Default(TParams p)
        {
            // Currently Maui test scenarios run on the same instance and this flag might have been set to a non-default values in the previous tests. So, explicitly setting it to `default' value `true` to make tests reliable.
            MdiChildrenMinimizedAnchorBottom = true;

            using Form childForm = new()
            {
                MdiParent = this,
                WindowState = FormWindowState.Minimized
            };
            childForm.Show();

            int childFormMinimizedYPositionFromBottom = ClientSize.Height - childForm.Top;
            Height += 100;

            if (childFormMinimizedYPositionFromBottom != ClientSize.Height - childForm.Top)
            {
                return new ScenarioResult(false, $"MDI child changed bottom relative position with {nameof(MdiChildrenMinimizedAnchorBottom)} == {MdiChildrenMinimizedAnchorBottom}!", p.log);
            }

            return ScenarioResult.Pass;
        }

        [Scenario(true)]
        public ScenarioResult Resize_When_MdiChildrenMinimizedAnchorBottom_False(TParams p)
        {
            MdiChildrenMinimizedAnchorBottom = false;

            using Form childForm = new()
            {
                MdiParent = this,
                WindowState = FormWindowState.Minimized
            };
            childForm.Show();

            int childFormMinimizedTop = childForm.Top;
            Height += 100;

            if (childFormMinimizedTop != childForm.Top)
            {
                return new ScenarioResult(false, $"MDI child changed top relative position with {nameof(MdiChildrenMinimizedAnchorBottom)} == {MdiChildrenMinimizedAnchorBottom}!", p.log);
            }

            return ScenarioResult.Pass;
        }
    }
}
