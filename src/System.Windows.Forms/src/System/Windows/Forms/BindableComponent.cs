// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Runtime.Versioning;

namespace System.Windows.Forms
{
    [RequiresPreviewFeatures]
    public abstract class BindableComponent : Component, IBindableComponent
    {
        internal static readonly object s_bindingContextChangedEvent = new();

        private ControlBindingsCollection? _dataBindings;
        private BindingContext? _bindingContext;

        public BindableComponent()
        {
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [SRCategory(nameof(SR.CatData))]
        [SRDescription(nameof(SR.ToolStripItemBindingContextDescr))]
        public BindingContext BindingContext
        {
            [RequiresPreviewFeatures]
            get => _bindingContext ??= new BindingContext();

            [RequiresPreviewFeatures]
            set
            {
                if (!Equals(_bindingContext, value))
                {
                    _bindingContext = value;
                    OnBindingContextChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Occurs when the binding context has changed
        /// </summary>
        [SRCategory(nameof(SR.CatData))]
        [SRDescription(nameof(SR.ToolStripItemOnBindingContextChangedDescr))]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public event EventHandler BindingContextChanged
        {
            add => Events.AddHandler(s_bindingContextChangedEvent, value);
            remove => Events.RemoveHandler(s_bindingContextChangedEvent, value);
        }

        /// <summary>
        ///  Raises the <see cref="BindableComponent.BindingContextChanged"/> event.
        ///  Inheriting classes should override this method to handle this event.
        ///  Call base.OnBindingContextChanged to send this event to any registered event listeners.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnBindingContextChanged(EventArgs e)
        {
            if (_bindingContext is not null)
            {
                for (int i = 0; i < DataBindings.Count; i++)
                {
                    BindingContext.UpdateBinding(BindingContext, DataBindings[i]);
                }
            }

            RaiseEvent(s_bindingContextChangedEvent, e);
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [RefreshProperties(RefreshProperties.All)]
        [ParenthesizePropertyName(true)]
        public ControlBindingsCollection DataBindings
            => _dataBindings ??= new ControlBindingsCollection(this);

        private protected void RaiseEvent(object key, EventArgs e)
            => ((EventHandler?)Events[key])?.Invoke(this, e);
    }
}
