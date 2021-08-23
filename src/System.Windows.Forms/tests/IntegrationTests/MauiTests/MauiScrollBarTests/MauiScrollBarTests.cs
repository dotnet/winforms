// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using System.Windows.Forms.IntegrationTests.Common;
using ReflectTools;

namespace System.Windows.Forms.IntegrationTests.MauiTests
{
    public partial class MauiScrollBarTests : ReflectBase
    {
        private readonly HScrollBar _hScrollBar;
        private readonly VScrollBar _vScrollBar;

        public MauiScrollBarTests(string[] args) : base(args)
        {
            this.BringToForeground();

            _vScrollBar = new VScrollBar()
            {
                Size = new Size(50, 200)
            };

            _hScrollBar = new HScrollBar()
            {
                Size = new Size(200, 50)
            };

            Controls.Add(_hScrollBar);
            Controls.Add(_vScrollBar);
        }

        public static void Main(string[] args)
        {
            Thread.CurrentThread.SetCulture("en-US");
            Application.Run(new MauiScrollBarTests(args));
        }
    }
}
