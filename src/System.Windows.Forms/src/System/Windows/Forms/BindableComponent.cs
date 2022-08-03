// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Runtime.Versioning;

namespace System.Windows.Forms
{
    /// <summary>
    /// Base class for components which provide properties which can be 
    /// data bound with the WinForms Designer. 
    /// </summary>
    [RequiresPreviewFeatures]
    public abstract class BindableComponent : Component, IBindableComponent
    {
        internal static readonly object s_bindingContextChangedEvent = new();

        private ControlBindingsCollection? _dataBindings;
        private BindingContext? _bindingContext;

        /// <summary>
        /// Gets or sets the <see cref="BindingContext"/> for this bindable <see cref="Component"/>.
        /// </summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [SRCategory(nameof(SR.CatData))]
        [SRDescription(nameof(SR.BindingComponentBindingContextDescr))]
        public BindingContext? BindingContext
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
        /// Occurs when the binding context has changed.
        /// </summary>
        [SRCategory(nameof(SR.CatData))]
        [SRDescription(nameof(SR.BindableComponentBindingContextChangedDescr))]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public event EventHandler BindingContextChanged
        {
            add => Events.AddHandler(s_bindingContextChangedEvent, value);
            remove => Events.RemoveHandler(s_bindingContextChangedEvent, value);
        }

        /// <summary>
        ///  Raises the <see cref="BindableComponent.BindingContextChanged"/> event.
        /// </summary>
        /// <remarks>
        ///   <para>
        ///     Raising an event invokes the event handler through a delegate. For more information,
        ///     see <see href="https://docs.microsoft.com/dotnet/standard/events/">Handling and Raising Events</see>.
        ///   </para>
        ///   <para>
        ///     The <see cref="OnBindingContextChanged"/> method also allows derived classes to handle the event
        ///     without attaching a delegate. This is the preferred technique for handling the event in a derived class.
        ///   </para>
        ///   <para>
        ///    <block subset = "none" type="overrides">
        ///     Override this method to handle the event. In the overriding method, call ...
        ///    </block>
        ///   </para>
        /// </remarks>
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

        /// <summary>
        /// Gets the <see cref="ControlBindingsCollection"/> for this bindable <see cref="Component"/>.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [RefreshProperties(RefreshProperties.All)]
        [ParenthesizePropertyName(true)]
        public ControlBindingsCollection DataBindings
            => _dataBindings ??= new ControlBindingsCollection(this);

        private protected void RaiseEvent(object key, EventArgs e)
            => ((EventHandler?)Events[key])?.Invoke(this, e);
    }
}
