// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing.Design;
using System.Windows.Forms.PropertyGridInternal;

namespace System.Windows.Forms.Design;

/// <summary>
///  Provides an interface for a <see cref="UITypeEditor"/> to display Windows Forms or to display a control in a
///  drop-down area from a <see cref="PropertyGrid"/> control in design mode.
/// </summary>
/// <remarks>
///  <para>
///   The default <see cref="IWindowsFormsEditorService"/> is provided to <see cref="UITypeEditor"/>s through
///   the service provider given to the <see cref="UITypeEditor.EditValue(IServiceProvider, object)"/> and
///   <see cref="UITypeEditor.EditValue(System.ComponentModel.ITypeDescriptorContext, IServiceProvider, object)"/>
///   methods.
///  </para>
/// </remarks>
/// <devdoc>
///  <see cref="PropertyGridView"/> implements this interface and it provides it via the
///  <see cref="PropertyGrid.PropertyGridServiceProvider"/> in the <see cref="SingleSelectRootGridEntry.GetService(Type)"/>
///  override.
///
///  <see cref="PropertyGridView.PopupEditor(int)"/> calls <see cref="GridEntry.EditPropertyValue(PropertyGridView)"/>
///  which invokes <see cref="UITypeEditor"/>'s EditValue methods.
/// </devdoc>
public interface IWindowsFormsEditorService
{
    /// <summary>
    ///  Closes a previously opened drop down list.
    /// </summary>
    void CloseDropDown();

    /// <summary>
    ///  Modally displays the specified control in a drop down area below a value field of the
    ///  <see cref="PropertyGrid"/> that provides this service.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   The EditValue methods of a <see cref="UITypeEditor"/> can call this method to display a specified control
    ///   in a drop down area over the <see cref="PropertyGrid"/> hosting the editor which uses this service.
    ///  </para>
    ///  <para>
    ///   When possible, the dimensions of the control will be maintained. If this is not possible due to the screen
    ///   layout, the control may be resized. To ensure that the control resizes neatly, you should implement docking
    ///   and anchoring, and possibly any resize event-handler update code. If the user performs an action that
    ///   causes the drop down to close, the control will be hidden and disposed by garbage collection if there is
    ///   no other stored reference to the control.
    ///  </para>
    /// </remarks>
    void DropDownControl(Control control);

    /// <summary>
    ///  Shows the specified <see cref="Form"/> as a dialog and returns its result. You should always use this
    ///  method rather than showing the dialog directly, as this will properly position the dialog and provide
    ///  it a dialog owner.
    /// </summary>
    DialogResult ShowDialog(Form dialog);
}
