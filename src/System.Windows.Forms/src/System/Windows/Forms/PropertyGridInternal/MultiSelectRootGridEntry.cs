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
    internal partial class MultiSelectRootGridEntry : SingleSelectRootGridEntry
    {
        private static readonly PDComparer s_propertyComparer = new();

        internal MultiSelectRootGridEntry(PropertyGridView view, object obj, IServiceProvider baseProvider, IDesignerHost host, PropertyTab tab, PropertySort sortType)
        : base(view, obj, baseProvider, host, tab, sortType)
        {
        }

        internal override bool ForceReadOnly
        {
            get
            {
                if (!forceReadOnlyChecked)
                {
                    bool anyRO = false;
                    foreach (object obj in (Array)objValue)
                    {
                        ReadOnlyAttribute readOnlyAttr = (ReadOnlyAttribute)TypeDescriptor.GetAttributes(obj)[typeof(ReadOnlyAttribute)];
                        if ((readOnlyAttr is not null && !readOnlyAttr.IsDefaultAttribute()) || TypeDescriptor.GetAttributes(obj).Contains(InheritanceAttribute.InheritedReadOnly))
                        {
                            anyRO = true;
                            break;
                        }
                    }

                    if (anyRO)
                    {
                        _flags |= FLAG_FORCE_READONLY;
                    }

                    forceReadOnlyChecked = true;
                }

                return base.ForceReadOnly;
            }
        }

        protected override bool CreateChildren()
        {
            return CreateChildren(false);
        }

        protected override bool CreateChildren(bool diffOldChildren)
        {
            try
            {
                object[] rgobjs = (object[])objValue;

                ChildCollection.Clear();

                MultiPropertyDescriptorGridEntry[] mergedProps = PropertyMerger.GetMergedProperties(rgobjs, this, PropertySort, CurrentTab);

                Debug.WriteLineIf(CompModSwitches.DebugGridView.TraceVerbose && mergedProps is null, "PropertyGridView: MergedProps returned null!");

                if (mergedProps is not null)
                {
                    ChildCollection.AddRange(mergedProps);
                }

                bool fExpandable = Children.Count > 0;
                if (!fExpandable)
                {
                    SetFlag(GridEntry.FL_EXPANDABLE_FAILED, true);
                }

                CategorizePropEntries();
                return fExpandable;
            }
            catch
            {
                return false;
            }
        }
    }
}
