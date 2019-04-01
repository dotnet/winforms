// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Windows.Forms.Tests
{
    public class UpDownBaseTests
    {
        [Fact]
        public void UpDownBase_OnTextBoxTextChanged_Invoke_CallsOnTextChanged()
        {
            var control = new SubUpDownBase();
            var eventArgs = new EventArgs();

            // No handler.
            control.OnTextBoxTextChanged(null, eventArgs);

            // Handler.
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Equal(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            control.TextChanged += handler;
            control.OnTextBoxTextChanged(null, eventArgs);
            Assert.Equal(1, callCount);

            // Should not call if the handler is removed.
            control.TextChanged -= handler;
            control.OnTextBoxTextChanged(null, eventArgs);
            Assert.Equal(1, callCount);
        }

        [Fact]
        public void UpDownBase_OnTextBoxTextChanged_Invoke_CallsOnChanged()
        {
            var control = new OnChangedHandlerUpDownBase();
            var originalSource = new object();
            var eventArgs = new EventArgs();

            int callCount = 0;
            control.OnChangedAction += (source, e) =>
            {
                Assert.Same(originalSource, source);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };

            control.OnTextBoxTextChanged(originalSource, eventArgs);
            Assert.Equal(1, callCount);
        }

        private class OnChangedHandlerUpDownBase : UpDownBase
        {
            public override void DownButton()
            {
            }

            public override void UpButton()
            {
            }

            protected override void UpdateEditText()
            {
            }

            public Action<object, EventArgs> OnChangedAction { get; set; }

            public new void OnTextBoxTextChanged(object source, EventArgs e)
            {
                base.OnTextBoxTextChanged(source, e);
            }

            protected override void OnChanged(object source, EventArgs e)
            {
                OnChangedAction(source, e);
            }
        }

        private class SubUpDownBase : UpDownBase
        {
            public override void DownButton()
            {
            }

            public override void UpButton()
            {
            }

            protected override void UpdateEditText()
            {
            }

            public new void OnTextBoxTextChanged(object source, EventArgs e)
            {
                base.OnTextBoxTextChanged(source, e);
            }

            public new void OnChanged(object source, EventArgs e)
            {
                base.OnChanged(source, e);
            }
        }
    }
}
