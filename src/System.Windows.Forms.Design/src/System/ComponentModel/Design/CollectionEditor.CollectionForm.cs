// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace System.ComponentModel.Design;

public partial class CollectionEditor
{
    /// <summary>
    ///  The <see cref="CollectionForm"/> provides a modal dialog for editing the contents of a collection.
    /// </summary>
    protected abstract class CollectionForm : Form
    {
        private readonly CollectionEditor _editor;
        private object? _value;
        private short _editableState = EditableDynamic;

        private const short EditableDynamic = 0;
        private const short EditableYes = 1;
        private const short EditableNo = 2;

        /// <summary>
        ///  Initializes a new instance of the <see cref="CollectionForm"/> class.
        /// </summary>
        public CollectionForm(CollectionEditor editor)
        {
            _editor = editor.OrThrowIfNull();
        }

        /// <summary>
        ///  Gets or sets the data type of each item in the collection.
        /// </summary>
        protected Type CollectionItemType => _editor.CollectionItemType;

        /// <summary>
        ///  Gets or sets the type of the collection.
        /// </summary>
        protected Type CollectionType => _editor.CollectionType;

        internal virtual bool CollectionEditable
        {
            get
            {
                if (_editableState != EditableDynamic)
                {
                    return _editableState == EditableYes;
                }

                bool editable = typeof(IList).IsAssignableFrom(_editor.CollectionType);

                if (editable)
                {
                    if (EditValue is IList list)
                    {
                        return !list.IsReadOnly;
                    }
                }

                return editable;
            }
            set => _editableState = value ? EditableYes : EditableNo;
        }

        /// <summary>
        ///  Gets or sets a type descriptor that indicates the current context.
        /// </summary>
        protected ITypeDescriptorContext? Context => _editor.Context;

        /// <summary>
        ///  Gets or sets the value of the item being edited.
        /// </summary>
        public object? EditValue
        {
            get => _value;
            set
            {
                _value = value;
                OnEditValueChanged();
            }
        }

        /// <summary>
        ///  Gets or sets the array of items this form is to display.
        /// </summary>
        [AllowNull]
        protected object[] Items
        {
            get => _editor.GetItems(EditValue);
            set
            {
                // Request our desire to make a change.
                bool canChange = false;
                try
                {
                    if (Context is not null)
                    {
                        canChange = Context.OnComponentChanging();
                    }
                }
                catch (Exception ex) when (!ex.IsCriticalException())
                {
                    DisplayError(ex);
                }

                if (canChange)
                {
                    object? newValue = _editor.SetItems(EditValue, value);
                    if (newValue != EditValue)
                    {
                        EditValue = newValue;
                    }

                    Context!.OnComponentChanged();
                }
            }
        }

        /// <summary>
        ///  Gets or sets the available item types that can be created for this collection.
        /// </summary>
        protected Type[] NewItemTypes => _editor.NewItemTypes;

        /// <summary>
        ///  Gets or sets a value indicating whether original members of the collection can be removed.
        /// </summary>
        protected bool CanRemoveInstance(object value) => _editor.CanRemoveInstance(value);

        /// <summary>
        ///  Gets or sets a value indicating whether multiple collection members can be selected.
        /// </summary>
        protected virtual bool CanSelectMultipleInstances() => _editor.CanSelectMultipleInstances();

        /// <summary>
        ///  Creates a new instance of the specified collection item type.
        /// </summary>
        protected object CreateInstance(Type itemType) => _editor.CreateInstance(itemType);

        /// <summary>
        ///  Destroys the specified instance of the object.
        /// </summary>
        protected void DestroyInstance(object instance) => _editor.DestroyInstance(instance);

        /// <summary>
        ///  Displays the given exception to the user.
        /// </summary>
        protected virtual void DisplayError(Exception e)
        {
            if (GetService(typeof(IUIService)) is IUIService uis)
            {
                uis.ShowError(e);
            }
            else
            {
                string message = e.Message;
                if (string.IsNullOrEmpty(message))
                {
                    message = e.ToString();
                }

                RTLAwareMessageBox.Show(
                    owner: null,
                    message,
                    caption: null,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation,
                    MessageBoxDefaultButton.Button1,
                    0);
            }
        }

        /// <summary>
        ///  Gets the requested service, if it is available.
        /// </summary>
        protected override object? GetService(Type serviceType) => _editor.GetService(serviceType);

        /// <summary>
        ///  Called to show the dialog via the IWindowsFormsEditorService
        /// </summary>
        protected internal virtual DialogResult ShowEditorDialog(IWindowsFormsEditorService edSvc)
        {
            ArgumentNullException.ThrowIfNull(edSvc);

            return edSvc.ShowDialog(this);
        }

        /// <summary>
        ///  This is called when the value property in the <see cref="CollectionForm"/> has changed.
        /// </summary>
        protected abstract void OnEditValueChanged();
    }
}
