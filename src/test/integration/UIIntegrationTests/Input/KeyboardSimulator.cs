// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.UI.Input.KeyboardAndMouse;

namespace System.Windows.Forms.UITests.Input;

internal class KeyboardSimulator
{
    private readonly InputSimulator _inputSimulator;

    public KeyboardSimulator(InputSimulator inputSimulator)
    {
        _inputSimulator = inputSimulator;
    }

    public MouseSimulator Mouse => _inputSimulator.Mouse;

    internal KeyboardSimulator KeyDown(VIRTUAL_KEY key)
    {
        Span<INPUT> inputs =
        [
            InputBuilder.KeyDown(key),
        ];

        InputSimulator.Send(inputs);
        return this;
    }

    internal KeyboardSimulator KeyUp(VIRTUAL_KEY key)
    {
        Span<INPUT> inputs =
        [
            InputBuilder.KeyUp(key),
        ];

        InputSimulator.Send(inputs);
        return this;
    }

    internal KeyboardSimulator KeyPress(VIRTUAL_KEY key)
    {
        Span<INPUT> inputs =
        [
            InputBuilder.KeyDown(key),
            InputBuilder.KeyUp(key),
        ];

        InputSimulator.Send(inputs);
        return this;
    }

    internal KeyboardSimulator TextEntry(char character)
    {
        Span<INPUT> inputs =
        [
            InputBuilder.CharacterDown(character),
            InputBuilder.CharacterUp(character),
        ];

        InputSimulator.Send(inputs);
        return this;
    }

    internal KeyboardSimulator TextEntry(string text)
    {
        if (text.Length == 0)
        {
            return this;
        }

        Span<INPUT> inputs = stackalloc INPUT[text.Length * 2];
        for (int i = 0; i < text.Length; i++)
        {
            inputs[i * 2] = InputBuilder.CharacterDown(text[i]);
            inputs[i * 2 + 1] = InputBuilder.CharacterUp(text[i]);
        }

        InputSimulator.Send(inputs);
        return this;
    }

    internal KeyboardSimulator ModifiedKeyStroke(VIRTUAL_KEY modifierKeyCode, VIRTUAL_KEY keyCode)
    {
        return ModifiedKeyStroke(new[] { modifierKeyCode }, new[] { keyCode });
    }

    internal KeyboardSimulator ModifiedKeyStroke(IEnumerable<VIRTUAL_KEY> modifierKeyCodes, IEnumerable<VIRTUAL_KEY> keyCodes)
    {
        var modifierArray = modifierKeyCodes.ToArray();
        var keyArray = keyCodes.ToArray();
        if (modifierArray.Length == 0 && keyArray.Length == 0)
        {
            return this;
        }

        Span<INPUT> inputs = stackalloc INPUT[modifierArray.Length * 2 + keyArray.Length * 2];
        for (int i = 0; i < modifierArray.Length; i++)
        {
            inputs[i] = InputBuilder.KeyDown(modifierArray[i]);
            inputs[^(i + 1)] = InputBuilder.KeyUp(modifierArray[i]);
        }

        for (int i = 0; i < keyArray.Length; i++)
        {
            inputs[modifierArray.Length + i] = InputBuilder.KeyDown(keyArray[i]);
            inputs[modifierArray.Length + i + 1] = InputBuilder.KeyUp(keyArray[i]);
        }

        InputSimulator.Send(inputs);
        return this;
    }

    internal KeyboardSimulator Sleep(TimeSpan time)
    {
        Thread.Sleep(time);
        return this;
    }
}
