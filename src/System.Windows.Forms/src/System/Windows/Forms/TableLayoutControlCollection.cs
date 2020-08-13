// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.ComponentModel;
using System.ComponentModel.Design.Serialization;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Represents a collection of controls on the TableLayoutPanel.
    /// </summary>
    [ListBindable(false)]
    [DesignerSerializer("System.Windows.Forms.Design.TableLayoutControlCollectionCodeDomSerializer, " + AssemblyRef.SystemDesign, "System.ComponentModel.Design.Serialization.CodeDomSerializer, " + AssemblyRef.SystemDesign)]
    public class TableLayoutControlCollection : Control.ControlCollection
    {
        public TableLayoutControlCollection(TableLayoutPanel container) : base(container ?? throw new ArgumentNullException(nameof(container)))
        {
            Container = container;
        }

        //the container of this TableLayoutControlCollection
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
}
