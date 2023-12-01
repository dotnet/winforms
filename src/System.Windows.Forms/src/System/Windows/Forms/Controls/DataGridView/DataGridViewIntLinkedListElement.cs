// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

/// <summary>
///  Represents an element in a <see cref="DataGridViewIntLinkedList"/> linked list.
/// </summary>
internal class DataGridViewIntLinkedListElement
{
    public DataGridViewIntLinkedListElement(int integer) => Int = integer;

    public int Int { get; set; }

    public DataGridViewIntLinkedListElement? Next { get; set; }
}
