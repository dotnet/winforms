// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.ComponentModel.Design
{
    /// <summary>
    ///     DesignerActionList is the abstract base class which control authors inherit from to create a task sheet.
    ///     Typical usage is to add properties and methods and then implement the abstract
    ///     GetSortedActionItems method to return an array of DesignerActionItems in the order they are to be displayed.
    /// </summary>
    public class DesignerActionList
    {
        /// <summary>
        ///     takes the related component as a parameter
        /// </summary>
        public DesignerActionList(IComponent component)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        public virtual bool AutoShow
        {
            get => throw new NotImplementedException(SR.NotImplementedByDesign);
            set => throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///     this will be null for list created from upgraded verbs collection...
        /// </summary>
        public IComponent Component => throw new NotImplementedException(SR.NotImplementedByDesign);

        public object GetService(Type serviceType)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        public virtual DesignerActionItemCollection GetSortedActionItems()
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }
    }
}
