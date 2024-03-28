// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

/// <summary>
///  Translates between WinForms text-based <see cref="Clipboard"/>
///  formats and Win32 32-bit signed integer-based clipboard
///  formats. Provides <see langword="static"/> methods to create new
/// <see cref="Clipboard"/> formats and add them to the Windows Registry.
/// </summary>
public static partial class DataFormats
{
    /// <summary>
    ///  Represents a format type.
    /// </summary>
    public class Format
    {
        /// <summary>
        ///  Initializes a new instance of the <see cref="Format"/> class and
        ///  specifies whether a Win32 handle is expected with this format.
        /// </summary>
        public Format(string name, int id)
        {
            Name = name;
            Id = id;
        }

        /// <summary>
        ///  Specifies the name of this format.
        /// </summary>
        public string Name { get; }

        /// <summary>
        ///  Specifies the ID number for this format.
        /// </summary>
        public int Id { get; }
    }
}
