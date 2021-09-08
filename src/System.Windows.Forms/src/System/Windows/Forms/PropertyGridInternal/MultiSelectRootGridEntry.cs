// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Windows.Forms.Design;

namespace System.Windows.Forms.PropertyGridInternal
{
    /// <summary>
    ///  Root <see cref="GridEntry"/> for the <see cref="PropertyGrid"/> when there are multiple objects
    ///  in <see cref="PropertyGrid.SelectedObjects"/>.
    /// </summary>
    internal sealed partial class MultiSelectRootGridEntry : SingleSelectRootGridEntry
    {
        private static readonly PropertyDescriptorComparer s_propertyComparer = new();

        internal MultiSelectRootGridEntry(
            PropertyGridView view,
            object obj,
            IServiceProvider baseProvider,
            IDesignerHost host,
            PropertyTab tab,
            PropertySort sortType)
            : base(view, obj, baseProvider, host, tab, sortType)
        {
        }

        internal override bool ForceReadOnly
        {
            get
            {
                if (!_forceReadOnlyChecked)
                {
                    foreach (object obj in (Array)_value)
                    {
                        var readOnlyAttribute = (ReadOnlyAttribute)TypeDescriptor.GetAttributes(obj)[typeof(ReadOnlyAttribute)];
                        if ((readOnlyAttribute is not null && !readOnlyAttribute.IsDefaultAttribute())
                            || TypeDescriptor.GetAttributes(obj).Contains(InheritanceAttribute.InheritedReadOnly))
                        {
                            SetForceReadOnlyFlag();
                            break;
                        }
                    }

                    _forceReadOnlyChecked = true;
                }

                return base.ForceReadOnly;
            }
        }

        protected override bool CreateChildren(bool diffOldChildren = false)
        {
            try
            {
                object[] rgobjs = (object[])_value;

                ChildCollection.Clear();

                MultiPropertyDescriptorGridEntry[] mergedProps = PropertyMerger.GetMergedProperties(rgobjs, this, _propertySort, OwnerTab);

                Debug.WriteLineIf(CompModSwitches.DebugGridView.TraceVerbose && mergedProps is null, "PropertyGridView: MergedProps returned null!");

                if (mergedProps is not null)
                {
                    ChildCollection.AddRange(mergedProps);
                }

                bool fExpandable = Children.Count > 0;
                if (!fExpandable)
                {
                    SetFlag(Flags.ExpandableFailed, true);
                }

                CategorizePropEntries();
                return fExpandable;
            }
            catch (Exception e) when (!ClientUtils.IsCriticalException(e))
            {
                return false;
            }
        }
    }
}
