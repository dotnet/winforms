// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.ComponentModel;

namespace System.Windows.Forms
{
    /// <summary>
    ///  BindingsCollection is a collection of bindings for a Control. It has Add/Remove capabilities,
    ///  as well as an All array property, enumeration, etc.
    /// </summary>
    [DefaultEvent(nameof(CollectionChanged))]
    internal class ListManagerBindingsCollection : BindingsCollection
    {
        private readonly BindingManagerBase _bindingManagerBase;

        /// <summary>
        ///  ColumnsCollection constructor.  Used only by DataSource.
        /// </summary>
        internal ListManagerBindingsCollection(BindingManagerBase bindingManagerBase) : base()
        {
            Debug.Assert(bindingManagerBase != null, "How could a listmanagerbindingscollection not have a bindingManagerBase associated with it!");
            _bindingManagerBase = bindingManagerBase;
        }

        protected override void AddCore(Binding dataBinding)
        {
            if (dataBinding == null)
            {
                throw new ArgumentNullException(nameof(dataBinding));
            }
            if (dataBinding.BindingManagerBase == _bindingManagerBase)
            {
                throw new ArgumentException(SR.BindingsCollectionAdd1, nameof(dataBinding));
            }
            if (dataBinding.BindingManagerBase != null)
            {
                throw new ArgumentException(SR.BindingsCollectionAdd2, nameof(dataBinding));
            }

            dataBinding.BindingManagerBase = _bindingManagerBase;
            base.AddCore(dataBinding);
        }

        protected override void ClearCore()
        {
            int numLinks = Count;
            for (int i = 0; i < numLinks; i++)
            {
                this[i].BindingManagerBase = null;
            }

            base.ClearCore();
        }

        protected override void RemoveCore(Binding dataBinding)
        {
            if (dataBinding == null)
            {
                throw new ArgumentNullException(nameof(dataBinding));
            }
            if (dataBinding.BindingManagerBase != _bindingManagerBase)
            {
                throw new ArgumentException(SR.BindingsCollectionForeign, nameof(dataBinding));
            }

            dataBinding.BindingManagerBase = null;
            base.RemoveCore(dataBinding);
        }
    }
}
