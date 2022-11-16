// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using System.Windows.Forms.Primitives;
using System.Reflection;

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
            SetSwitch(LocalAppContextSwitches.AnchorLayoutV2SwitchName, LocalAppContextSwitchValue.True);

            (Form form, Button button) = GetFormWithAnchoredButton();

            DefaultLayout.AnchorInfo anchorInfo = DefaultLayout.GetAnchorInfo(button);
            Assert.Null(anchorInfo);

            // Unparent button and resume layout.
            form.Controls.Remove(button);
            form.ResumeLayout(false);

            anchorInfo = DefaultLayout.GetAnchorInfo(button);
            Assert.Null(anchorInfo);

            SetSwitch(LocalAppContextSwitches.AnchorLayoutV2SwitchName, LocalAppContextSwitchValue.Unknown);
            Dispose(form, button);
        }

        [WinFormsFact]
        public void Control_SuspendedLayout_AnchorsNotComputed()
        {
            // TargetFramework on the testhost.exe is NetCoreApp2.1. AppContext.TargetFrameworkName return this value
            // while running unit tests. To avoid using this invalid target framework for unit tests, we are
            // explicitly setting and unsetting the switch.
            SetSwitch(LocalAppContextSwitches.AnchorLayoutV2SwitchName, LocalAppContextSwitchValue.True);

            (Form form, Button button) = GetFormWithAnchoredButton();

            DefaultLayout.AnchorInfo anchorInfo = DefaultLayout.GetAnchorInfo(button);
            Assert.Null(anchorInfo);

            form.Controls.Add(button);
            anchorInfo = DefaultLayout.GetAnchorInfo(button);
            Assert.Null(anchorInfo);

            SetSwitch(LocalAppContextSwitches.AnchorLayoutV2SwitchName, LocalAppContextSwitchValue.Unknown);
            Dispose(form, button);
        }

        [WinFormsFact]
        public void Control_ResumedLayout_AnchorsComputed()
        {
            // TargetFramework on the testhost.exe is NetCoreApp2.1. AppContext.TargetFrameworkName return this value
            // while running unit tests. To avoid using this invalid target framework for unit tests, we are
            // explicitly setting and unsetting the switch.
            SetSwitch(LocalAppContextSwitches.AnchorLayoutV2SwitchName, LocalAppContextSwitchValue.True);

            (Form form, Button button) = GetFormWithAnchoredButton();

            DefaultLayout.AnchorInfo anchorInfo = DefaultLayout.GetAnchorInfo(button);
            Assert.Null(anchorInfo);

            form.Controls.Add(button);
            anchorInfo = DefaultLayout.GetAnchorInfo(button);
            Assert.Null(anchorInfo);

            form.ResumeLayout(false);
            anchorInfo = DefaultLayout.GetAnchorInfo(button);
            Assert.NotNull(anchorInfo);

            SetSwitch(LocalAppContextSwitches.AnchorLayoutV2SwitchName, LocalAppContextSwitchValue.Unknown);
            Dispose(form, button);
        }

        [WinFormsFact]
        public void ConfigSwitch_Disabled_SuspendedLayout_AnchorsComputed()
        {
            // TargetFramework on the testhost.exe is NetCoreApp2.1. AppContext.TargetFrameworkName return this value
            // while running unit tests. To avoid using this invalid target framework for unit tests, we are
            // explicitly setting and unsetting the switch.
            SetSwitch(LocalAppContextSwitches.AnchorLayoutV2SwitchName, LocalAppContextSwitchValue.False);

            (Form form, Button button) = GetFormWithAnchoredButton();
            form.Controls.Add(button);

            DefaultLayout.AnchorInfo anchorInfo = DefaultLayout.GetAnchorInfo(button);
            Assert.NotNull(anchorInfo);

            Dispose(form, button);

            SetSwitch(LocalAppContextSwitches.AnchorLayoutV2SwitchName, LocalAppContextSwitchValue.Unknown);
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

        /// <summary>
        ///  Sets <see cref="LocalAppContextSwitches"/> switch value via reflection.
        /// </summary>
        /// <param name="switchName"> AppContext switch name to be set</param>
        /// <param name="value">AppContext switch value to be set</param>
        private static void SetSwitch(string switchName, LocalAppContextSwitchValue value)
        {
            switch (switchName)
            {
                case LocalAppContextSwitches.AnchorLayoutV2SwitchName:
                    FieldInfo anchorLayoutV2 = typeof(LocalAppContextSwitches).GetField("s_AnchorLayoutV2",
                            BindingFlags.Static |
                            BindingFlags.NonPublic);

                    if (value == LocalAppContextSwitchValue.True)
                    {
                        anchorLayoutV2.SetValue(null, 1);
                    }
                    else if (value == LocalAppContextSwitchValue.False)
                    {
                        anchorLayoutV2.SetValue(null, -1);
                    }
                    else if (value == LocalAppContextSwitchValue.Unknown)
                    {
                        anchorLayoutV2.SetValue(null, 0);
                    }

                    break;
            }
        }

        private static void Dispose(Form form, Button button)
        {
            button?.Dispose();
            form?.Dispose();
        }
    }
}
