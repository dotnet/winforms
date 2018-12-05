// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace System.ComponentModel.Design
{
    /// <summary>
    ///     <para>
    ///         The default designer for all components.
    ///     </para>
    /// </summary>
    public class ComponentDesigner : ITreeDesigner, IDesignerFilter, IComponentInitializer
    {
        /// <summary>
        ///     <para>
        ///         Gets the design-time actionlists supported by the component associated with the designer.
        ///     </para>
        /// </summary>
        public virtual DesignerActionListCollection ActionLists =>
            throw new NotImplementedException(SR.NotImplementedByDesign);

        /// <summary>
        ///     <para>
        ///         Retrieves a list of associated components.  These are components that should be incluced in a cut or copy
        ///         operation on this component.
        ///     </para>
        /// </summary>
        public virtual ICollection AssociatedComponents =>
            throw new NotImplementedException(SR.NotImplementedByDesign);

        /// <summary>
        ///     <para>
        ///         Gets or sets a value
        ///         indicating whether or not this component is being inherited.
        ///     </para>
        /// </summary>
        protected bool Inherited => throw new NotImplementedException(SR.NotImplementedByDesign);

        /// <summary>
        ///     This property provides a generic mechanism for discovering parent relationships within designers,
        ///     and is used by ComponentDesigner's ITreeDesigner interface implementation.  This property
        ///     should only return null when this designer is the root component.  Otherwise, it should return
        ///     the parent component.  The default implementation of this property returns the root component
        ///     for all components that are not the root component, and it returns null for the root component.
        /// </summary>
        protected virtual IComponent ParentComponent =>
            throw new NotImplementedException(SR.NotImplementedByDesign);

        /// <summary>
        ///     <para>
        ///         Gets or sets the inheritance attribute for this component.
        ///     </para>
        /// </summary>
        protected virtual InheritanceAttribute InheritanceAttribute =>
            throw new NotImplementedException(SR.NotImplementedByDesign);

        /// <summary>
        ///     Gets a collection that houses shadow properties.  Shadow properties. are properties that fall
        ///     through to the underlying component before they are set, but return their set values once they
        ///     are set.
        /// </summary>
        protected ShadowPropertyCollection ShadowProperties =>
            throw new NotImplementedException(SR.NotImplementedByDesign);

        /// <summary>
        ///     This method is called when an existing component is being re-initialized.  This may occur after
        ///     dragging a component to another container, for example.  The defaultValues
        ///     property contains a name/value dictionary of default values that should be applied
        ///     to properties. This dictionary may be null if no default values are specified.
        ///     You may use the defaultValues dictionary to apply recommended defaults to proeprties
        ///     but you should not modify component properties beyond what is stored in the
        ///     dictionary, because this is an existing component that may already have properties
        ///     set on it.
        ///     The default implemenation of this method does nothing.
        /// </summary>
        public virtual void InitializeExistingComponent(IDictionary defaultValues)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///     This method is called when a component is first initialized, typically after being first added
        ///     to a design surface.  The defaultValues property contains a name/value dictionary of default
        ///     values that should be applied to properties.  This dictionary may be null if no default values
        ///     are specified.  You may perform any initialization of this component that you like, and you
        ///     may even ignore the defaultValues dictionary altogether if you wish.
        ///     The default implemenation of this method does nothing.
        /// </summary>
        public virtual void InitializeNewComponent(IDictionary defaultValues)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        void IDesignerFilter.PostFilterAttributes(IDictionary attributes)
        {
            throw new NotImplementedException();
        }

        void IDesignerFilter.PostFilterEvents(IDictionary events)
        {
            throw new NotImplementedException();
        }

        void IDesignerFilter.PostFilterProperties(IDictionary properties)
        {
            throw new NotImplementedException();
        }

        void IDesignerFilter.PreFilterAttributes(IDictionary attributes)
        {
            throw new NotImplementedException();
        }

        void IDesignerFilter.PreFilterEvents(IDictionary events)
        {
            throw new NotImplementedException();
        }

        void IDesignerFilter.PreFilterProperties(IDictionary properties)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     <para>
        ///         Gets or sets the component this designer is designing.
        ///     </para>
        /// </summary>
        public IComponent Component => throw new NotImplementedException(SR.NotImplementedByDesign);

        /// <summary>
        ///     <para>
        ///         Gets the design-time verbs supported by the component associated with the designer.
        ///     </para>
        /// </summary>
        public virtual DesignerVerbCollection Verbs => throw new NotImplementedException(SR.NotImplementedByDesign);

        ICollection ITreeDesigner.Children => throw new NotImplementedException();

        IDesigner ITreeDesigner.Parent => throw new NotImplementedException();

        /// <summary>
        ///     <para>
        ///         Disposes of the resources (other than memory) used
        ///         by the <see cref='System.ComponentModel.Design.ComponentDesigner' />.
        ///     </para>
        /// </summary>
        public void Dispose()
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///     <para>
        ///         Creates a method signature in the source code file for the default event on the component and navigates
        ///         the user's cursor to that location in preparation to assign
        ///         the default action.
        ///     </para>
        /// </summary>
        public virtual void DoDefaultAction()
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///     <para>
        ///         Initializes a new instance of the <see cref='System.ComponentModel.Design.ComponentDesigner' />
        ///         class using the specified component.
        ///     </para>
        /// </summary>
        public virtual void Initialize(IComponent component)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///     <para>
        ///         Invokes the get inheritance attribute of the specified ComponentDesigner.
        ///     </para>
        /// </summary>
        protected InheritanceAttribute InvokeGetInheritanceAttribute(ComponentDesigner toInvoke)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///     <para>
        ///         Disposes of the resources (other than memory) used
        ///         by the <see cref='System.ComponentModel.Design.ComponentDesigner' />.
        ///     </para>
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///     <para>
        ///         Called when the designer has been associated with a control that is not in it's default state,
        ///         such as one that has been pasted or drag-dropped onto the designer.  This is an opportunity
        ///         to fixup any shadowed properties in a different way than for default components.  This is called
        ///         after the other initialize functions.
        ///     </para>
        /// </summary>
        [Obsolete(
            "This method has been deprecated. Use InitializeExistingComponent instead.  http://go.microsoft.com/fwlink/?linkid=14202")]
        public virtual void InitializeNonDefault()
        {
        }

        /// <summary>
        ///     <para>
        ///         Provides
        ///         a way for a designer to get services from the hosting
        ///         environment.
        ///     </para>
        /// </summary>
        protected virtual object GetService(Type serviceType)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///     <para>
        ///         Raises the SetComponentDefault event.
        ///     </para>
        /// </summary>
        [Obsolete(
            "This method has been deprecated. Use InitializeNewComponent instead.  http://go.microsoft.com/fwlink/?linkid=14202")]
        public virtual void OnSetComponentDefaults()
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///     Called when the context menu should be displayed
        /// </summary>
        internal virtual void ShowContextMenu(int x, int y)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///     <para>
        ///         Allows a
        ///         designer to filter the set of member attributes the
        ///         component it is designing will expose through the
        ///         TypeDescriptor object.
        ///     </para>
        /// </summary>
        protected virtual void PostFilterAttributes(IDictionary attributes)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///     <para>
        ///         Allows
        ///         a designer to filter the set of events the
        ///         component it is designing will expose through the
        ///         TypeDescriptor object.
        ///     </para>
        /// </summary>
        protected virtual void PostFilterEvents(IDictionary events)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///     <para>
        ///         Allows
        ///         a designer to filter the set of properties the
        ///         component it is designing will expose through the
        ///         TypeDescriptor object.
        ///     </para>
        /// </summary>
        protected virtual void PostFilterProperties(IDictionary properties)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///     <para>
        ///         Allows a designer
        ///         to filter the set of member attributes the component
        ///         it is designing will expose through the TypeDescriptor
        ///         object.
        ///     </para>
        /// </summary>
        protected virtual void PreFilterAttributes(IDictionary attributes)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///     <para>
        ///         Allows a
        ///         designer to filter the set of events the component
        ///         it is designing will expose through the TypeDescriptor
        ///         object.
        ///     </para>
        /// </summary>
        protected virtual void PreFilterEvents(IDictionary events)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///     <para>
        ///         Allows a
        ///         designer to filter the set of properties the component
        ///         it is designing will expose through the TypeDescriptor
        ///         object.
        ///     </para>
        /// </summary>
        protected virtual void PreFilterProperties(IDictionary properties)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///     <para>
        ///         Notifies the <see cref='System.ComponentModel.Design.IComponentChangeService' /> that this component
        ///         has been changed. You only need to call this when you are
        ///         affecting component properties directly and not through the
        ///         MemberDescriptor's accessors.
        ///     </para>
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate")]
        protected void RaiseComponentChanged(MemberDescriptor member, object oldValue, object newValue)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///     <para>
        ///         Notifies the <see cref='System.ComponentModel.Design.IComponentChangeService' /> that this component is
        ///         about to be changed. You only need to call this when you are
        ///         affecting component properties directly and not through the
        ///         MemberDescriptor's accessors.
        ///     </para>
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate")]
        protected void RaiseComponentChanging(MemberDescriptor member)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///     Collection that holds shadow properties.
        /// </summary>
        protected sealed class ShadowPropertyCollection
        {
            internal ShadowPropertyCollection(ComponentDesigner designer)
            {
                throw new NotImplementedException(SR.NotImplementedByDesign);
            }
            /// <summary>
            ///     Accesses the given property name.  This will throw an exception if the property does not exsit on the
            ///     base component.
            /// </summary>
            public object this[string propertyName]
            {
                get => throw new NotImplementedException(SR.NotImplementedByDesign);
                set => throw new NotImplementedException(SR.NotImplementedByDesign);
            }

        /// <summary>
            ///     Returns true if this shadow properties object contains the given property name.
            /// </summary>
            public bool Contains(string propertyName)
            {
                throw new NotImplementedException(SR.NotImplementedByDesign);
            }

        /// <summary>
            ///     Returns true if the given property name should be serialized, or false
            ///     if not.  This is useful in implementing your own ShouldSerialize* methods
            ///     on shadowed properties.
            /// </summary>
            internal bool ShouldSerializeValue(string propertyName, object defaultValue)
            {
                throw new NotImplementedException(SR.NotImplementedByDesign);
            }
        }
    }
}
