// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;

namespace System.Windows.Forms.Design
{
    /// <summary>
    ///     Provides access to get and set option values for a designer.
    /// </summary>
    [ComVisible(true)]
    public class DesignerOptions
    {
        /// <summary>
        ///     Public GridSize property.
        /// </summary>
        [SRCategory(nameof(SR.DesignerOptions_LayoutSettings))]
        [SRDisplayName(nameof(SR.DesignerOptions_GridSizeDisplayName))]
        [SRDescription(nameof(SR.DesignerOptions_GridSizeDesc))]
        public virtual Size GridSize
        {
            get => throw new NotImplementedException(SR.NotImplementedByDesign);
            set => throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///     Public ShowGrid property.
        /// </summary>
        [SRCategory(nameof(SR.DesignerOptions_LayoutSettings))]
        [SRDisplayName(nameof(SR.DesignerOptions_ShowGridDisplayName))]
        [SRDescription(nameof(SR.DesignerOptions_ShowGridDesc))]
        public virtual bool ShowGrid
        {
            get => throw new NotImplementedException(SR.NotImplementedByDesign);
            set => throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///     Public SnapToGrid property.
        /// </summary>
        [SRCategory(nameof(SR.DesignerOptions_LayoutSettings))]
        [SRDisplayName(nameof(SR.DesignerOptions_SnapToGridDisplayName))]
        [SRDescription(nameof(SR.DesignerOptions_SnapToGridDesc))]
        public virtual bool SnapToGrid
        {
            get => throw new NotImplementedException(SR.NotImplementedByDesign);
            set => throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///     This property enables or disables snaplines in the designer.
        /// </summary>
        [SRCategory(nameof(SR.DesignerOptions_LayoutSettings))]
        [SRDescription(nameof(SR.DesignerOptions_UseSnapLines))]
        public virtual bool UseSnapLines
        {
            get => throw new NotImplementedException(SR.NotImplementedByDesign);
            set => throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///     This property enables or disables smart tags in the designer.
        /// </summary>
        [SRCategory(nameof(SR.DesignerOptions_LayoutSettings))]
        [SRDescription(nameof(SR.DesignerOptions_UseSmartTags))]
        public virtual bool UseSmartTags
        {
            get => throw new NotImplementedException(SR.NotImplementedByDesign);
            set => throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///     This property enables or disables smart tags in the designer.
        /// </summary>
        [SRDisplayName(nameof(SR.DesignerOptions_ObjectBoundSmartTagAutoShowDisplayName))]
        [SRCategory(nameof(SR.DesignerOptions_ObjectBoundSmartTagSettings))]
        [SRDescription(nameof(SR.DesignerOptions_ObjectBoundSmartTagAutoShow))]
        public virtual bool ObjectBoundSmartTagAutoShow
        {
            get => throw new NotImplementedException(SR.NotImplementedByDesign);
            set => throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///     This property enables or disables the component cache
        /// </summary>
        [SRDisplayName(nameof(SR.DesignerOptions_CodeGenDisplay))]
        [SRCategory(nameof(SR.DesignerOptions_CodeGenSettings))]
        [SRDescription(nameof(SR.DesignerOptions_OptimizedCodeGen))]
        public virtual bool UseOptimizedCodeGeneration
        {
            get => throw new NotImplementedException(SR.NotImplementedByDesign);
            set => throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///     This property enables or disables the InSitu Editing for ToolStrips
        /// </summary>
        [SRDisplayName(nameof(SR.DesignerOptions_EnableInSituEditingDisplay))]
        [SRCategory(nameof(SR.DesignerOptions_EnableInSituEditingCat))]
        [SRDescription(nameof(SR.DesignerOptions_EnableInSituEditingDesc))]
        [Browsable(false)]
        public virtual bool EnableInSituEditing
        {
            get => throw new NotImplementedException(SR.NotImplementedByDesign);
            set => throw new NotImplementedException(SR.NotImplementedByDesign);
        }
    }
}
