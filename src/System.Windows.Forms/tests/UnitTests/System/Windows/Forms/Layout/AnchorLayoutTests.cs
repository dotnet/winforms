// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using System.Windows.Forms.Primitives;

namespace System.Windows.Forms.Layout.Tests
{
    public partial class AnchorLayoutTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void Control_NotParented_AnchorsNotComputed()
        {
            // TargetFramework on the testhost.exe is NetCoreApp2.1. AppContext.TargetFrameworkName return this value
            // while running unit tests. To avoid using this invalid target framework for unit tests, we are
            // explicitly setting and unsetting the switch.
            // Switch value has 3 states: 0 - unknown, 1 - true, -1 - false
            SetAnchorLayoutV2Switch(1);

            (Form form, Button button) = GetFormWithAnchoredButton();

            DefaultLayout.AnchorInfo anchorInfo = DefaultLayout.GetAnchorInfo(button);
            Assert.Null(anchorInfo);

            // Unparent button and resume layout.
            form.Controls.Remove(button);
            form.ResumeLayout(false);

            anchorInfo = DefaultLayout.GetAnchorInfo(button);
            Assert.Null(anchorInfo);

            // Reset switch to Unknown.
            SetAnchorLayoutV2Switch(0);
            Dispose(form, button);
        }

        [WinFormsFact]
        public void Control_SuspendedLayout_AnchorsNotComputed()
        {
            // TargetFramework on the testhost.exe is NetCoreApp2.1. AppContext.TargetFrameworkName return this value
            // while running unit tests. To avoid using this invalid target framework for unit tests, we are
            // explicitly setting and unsetting the switch.
            // Switch value has 3 states: 0 - unknown, 1 - true, -1 - false
            SetAnchorLayoutV2Switch(1);

            (Form form, Button button) = GetFormWithAnchoredButton();

            DefaultLayout.AnchorInfo anchorInfo = DefaultLayout.GetAnchorInfo(button);
            Assert.Null(anchorInfo);

            form.Controls.Add(button);
            anchorInfo = DefaultLayout.GetAnchorInfo(button);
            Assert.Null(anchorInfo);

            // Reset switch to Unknown.
            SetAnchorLayoutV2Switch(0);
            Dispose(form, button);
        }

        [WinFormsFact]
        public void Control_ResumedLayout_AnchorsComputed()
        {
            // TargetFramework on the testhost.exe is NetCoreApp2.1. AppContext.TargetFrameworkName return this value
            // while running unit tests. To avoid using this invalid target framework for unit tests, we are
            // explicitly setting and unsetting the switch.
            // Switch value has 3 states: 0 - unknown, 1 - true, -1 - false
            SetAnchorLayoutV2Switch(1);

            (Form form, Button button) = GetFormWithAnchoredButton();

            DefaultLayout.AnchorInfo anchorInfo = DefaultLayout.GetAnchorInfo(button);
            Assert.Null(anchorInfo);

            form.Controls.Add(button);
            anchorInfo = DefaultLayout.GetAnchorInfo(button);
            Assert.Null(anchorInfo);

            form.ResumeLayout(false);
            anchorInfo = DefaultLayout.GetAnchorInfo(button);
            Assert.NotNull(anchorInfo);

            // Reset switch to Unknown.
            SetAnchorLayoutV2Switch(0);
            Dispose(form, button);
        }

        [WinFormsFact]
        public void ConfigSwitch_Disabled_SuspendedLayout_AnchorsComputed()
        {
            // TargetFramework on the testhost.exe is NetCoreApp2.1. AppContext.TargetFrameworkName return this value
            // while running unit tests. To avoid using this invalid target framework for unit tests, we are
            // explicitly setting and unsetting the switch.
            // Switch value has 3 states: 0 - unknown, 1 - true, -1 - false
            SetAnchorLayoutV2Switch(-1);

            (Form form, Button button) = GetFormWithAnchoredButton();
            form.Controls.Add(button);

            DefaultLayout.AnchorInfo anchorInfo = DefaultLayout.GetAnchorInfo(button);
            Assert.NotNull(anchorInfo);

            // Reset switch to Unknown.
            SetAnchorLayoutV2Switch(0);
            Dispose(form, button);
        }

        private static (Form, Button) GetFormWithAnchoredButton()
        {
            Form form = new();
            form.SuspendLayout();
            form.Size = new(200, 300);

            Button button = new()
            {
                Location = new(20, 30),
                Size = new(20, 30),
                Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom
            };

            return (form, button);
        }

        private static void Dispose(Form form, Button button)
        {
            button?.Dispose();
            form?.Dispose();
        }

        private static void SetAnchorLayoutV2Switch(int value)
        {
            // Switch value has 3 states: 0 - unknown, 1 - true, -1 - false
            dynamic localAppContextSwitches = typeof(LocalAppContextSwitches).TestAccessor().Dynamic;
            localAppContextSwitches.s_AnchorLayoutV2 = value;
        }
    }
}
