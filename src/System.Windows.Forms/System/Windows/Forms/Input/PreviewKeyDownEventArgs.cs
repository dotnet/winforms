// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

/// <summary>
///  Provides data for the PreviewKeyDownEvent
/// </summary>
public class PreviewKeyDownEventArgs : EventArgs
{
    public PreviewKeyDownEventArgs(Keys keyData)
    {
        KeyData = keyData;
    }

    /// <summary>
    ///  Gets the key data for a <see cref="Control.KeyDown"/>
    ///  or <see cref="Control.KeyUp"/> event.
    /// </summary>
    public Keys KeyData { get; }

    public bool Alt => (KeyData & Keys.Alt) == Keys.Alt;

    /// <summary>
    ///  Gets a value indicating whether the CTRL key was pressed.
    /// </summary>
    public bool Control => (KeyData & Keys.Control) == Keys.Control;

    /// <summary>
    ///  Gets the keyboard code for a <see cref="Control.KeyDown"/>
    ///  or <see cref="Control.KeyUp"/> event.
    /// </summary>
    public Keys KeyCode
    {
        get
        {
            Keys keyGenerated = KeyData & Keys.KeyCode;
            if (!Enum.IsDefined(keyGenerated))
            {
                return Keys.None;
            }

            return keyGenerated;
        }
    }

    /// <summary>
    ///  Gets the keyboard value for a <see cref="Control.KeyDown"/>
    ///  or <see cref="Control.KeyUp"/> event.
    /// </summary>
    public int KeyValue => (int)(KeyData & Keys.KeyCode);

    /// <summary>
    ///  Gets the modifier flags for a <see cref="Control.KeyDown"/>
    ///  or <see cref="Control.KeyUp"/> event.
    ///  This indicates which modifier keys (CTRL, SHIFT, and/or ALT) were pressed.
    /// </summary>
    public Keys Modifiers => KeyData & Keys.Modifiers;

    /// <summary>
    ///  Gets a value indicating whether the SHIFT key was pressed.
    /// </summary>
    public bool Shift => (KeyData & Keys.Shift) == Keys.Shift;

    public bool IsInputKey { get; set; }
}
