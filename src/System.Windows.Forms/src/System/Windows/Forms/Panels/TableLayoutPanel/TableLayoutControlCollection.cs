// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.ComponentModel.Design.Serialization;

namespace System.Windows.Forms;

/// <summary>
///  Represents a collection of controls on the TableLayoutPanel.
/// </summary>
[ListBindable(false)]
[DesignerSerializer($"System.Windows.Forms.Design.TableLayoutControlCollectionCodeDomSerializer, {AssemblyRef.SystemDesign}", $"System.ComponentModel.Design.Serialization.CodeDomSerializer, {AssemblyRef.SystemDesign}")]
public class TableLayoutControlCollection : Control.ControlCollection
{
    public TableLayoutControlCollection(TableLayoutPanel container) : base(container.OrThrowIfNull())
    {
        Container = container;
    }

    /// <summary>
    ///  Gets the parent <see cref="TableLayoutPanel"/> that contains the controls in the collection.
    /// </summary>
    public TableLayoutPanel Container { get; }

    /// <summary>
    ///  Add control to cell (x, y) on the table. The control becomes absolutely positioned if neither x nor y is equal to -1
    /// </summary>
    public virtual void Add(Control control, int column, int row)
    {
        base.Add(control);
        Container.SetColumn(control, column);
        Container.SetRow(control, row);
    }
}
