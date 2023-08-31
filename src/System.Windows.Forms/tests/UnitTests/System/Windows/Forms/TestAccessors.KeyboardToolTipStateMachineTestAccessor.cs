// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System;

public static partial class TestAccessors
{
    internal class KeyboardToolTipStateMachineTestAccessor : TestAccessor<KeyboardToolTipStateMachine>
    {
        public KeyboardToolTipStateMachineTestAccessor(KeyboardToolTipStateMachine instance)
            : base(instance) { }

        internal bool IsToolTracked(IKeyboardToolTip sender) => (bool)Dynamic.IsToolTracked(sender);
    }

    internal static KeyboardToolTipStateMachineTestAccessor TestAccessor(this KeyboardToolTipStateMachine instance)
        => new(instance);
}
