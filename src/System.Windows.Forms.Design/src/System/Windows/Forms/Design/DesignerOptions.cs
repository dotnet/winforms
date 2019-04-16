// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;

namespace System.Windows.Forms.Design
{
    /// <summary>
    /// Provides access to get and set option values for a designer.
    /// </summary>
    [ComVisible(true)]
    public class DesignerOptions
    {
        private const int MinGridSize = 2;
        private const int MaxGridSize = 200;
        private bool _showGrid = true;
        private bool _snapToGrid = true;
        private Size _gridSize = new Size(8, 8);

        private bool _useSnapLines = false;
        private bool _useSmartTags = false;
        private bool _objectBoundSmartTagAutoShow = true;
        private bool _enableComponentCache = false;
        private bool _enableInSituEditing = true;

        /// <summary>
        /// Public GridSize property.
        /// </summary>
        [SRCategory(nameof(SR.DesignerOptions_LayoutSettings))]
        [SRDisplayName(nameof(SR.DesignerOptions_GridSizeDisplayName))]
        [SRDescription(nameof(SR.DesignerOptions_GridSizeDesc))]
        public virtual Size GridSize
        {
            get => _gridSize;
            set
            {
                //do some validation checking here
                if (value.Width < MinGridSize) value.Width = MinGridSize;
                if (value.Height < MinGridSize) value.Height = MinGridSize;
                if (value.Width > MaxGridSize) value.Width = MaxGridSize;
                if (value.Height > MaxGridSize) value.Height = MaxGridSize;
                _gridSize = value;
            }
        }

        /// <summary>
        /// Public ShowGrid property.
        /// </summary>
        [SRCategory(nameof(SR.DesignerOptions_LayoutSettings))]
        [SRDisplayName(nameof(SR.DesignerOptions_ShowGridDisplayName))]
        [SRDescription(nameof(SR.DesignerOptions_ShowGridDesc))]
        public virtual bool ShowGrid
        {
            get => _showGrid;
            set => _showGrid = value;
        }

        /// <summary>
        /// Public SnapToGrid property.
        /// </summary>
        [SRCategory(nameof(SR.DesignerOptions_LayoutSettings))]
        [SRDisplayName(nameof(SR.DesignerOptions_SnapToGridDisplayName))]
        [SRDescription(nameof(SR.DesignerOptions_SnapToGridDesc))]
        public virtual bool SnapToGrid
        {
            get => _snapToGrid;
            set => _snapToGrid = value;
        }

        /// <summary>
        /// This property enables or disables snaplines in the designer.
        /// </summary>
        [SRCategory(nameof(SR.DesignerOptions_LayoutSettings))]
        [SRDescription(nameof(SR.DesignerOptions_UseSnapLines))]
        public virtual bool UseSnapLines
        {
            get => _useSnapLines;
            set => _useSnapLines = value;
        }

        /// <summary>
        /// This property enables or disables smart tags in the designer.
        /// </summary>
        [SRCategory(nameof(SR.DesignerOptions_LayoutSettings))]
        [SRDescription(nameof(SR.DesignerOptions_UseSmartTags))]
        public virtual bool UseSmartTags
        {
            get => _useSmartTags;
            set => _useSmartTags = value;
        }

        /// <summary>
        /// This property enables or disables smart tags in the designer.
        /// </summary>
        [SRDisplayName(nameof(SR.DesignerOptions_ObjectBoundSmartTagAutoShowDisplayName))]
        [SRCategory(nameof(SR.DesignerOptions_ObjectBoundSmartTagSettings))]
        [SRDescription(nameof(SR.DesignerOptions_ObjectBoundSmartTagAutoShow))]
        public virtual bool ObjectBoundSmartTagAutoShow
        {
            get => _objectBoundSmartTagAutoShow;
            set => _objectBoundSmartTagAutoShow = value;
        }

        /// <summary>
        /// This property enables or disables the component cache
        /// </summary>
        [SRDisplayName(nameof(SR.DesignerOptions_CodeGenDisplay))]
        [SRCategory(nameof(SR.DesignerOptions_CodeGenSettings))]
        [SRDescription(nameof(SR.DesignerOptions_OptimizedCodeGen))]
        public virtual bool UseOptimizedCodeGeneration
        {
            get => _enableComponentCache;
            set => _enableComponentCache = value;
        }

        /// <summary>
        /// This property enables or disables the InSitu Editing for ToolStrips
        /// </summary>
        [SRDisplayName(nameof(SR.DesignerOptions_EnableInSituEditingDisplay))]
        [SRCategory(nameof(SR.DesignerOptions_EnableInSituEditingCat))]
        [SRDescription(nameof(SR.DesignerOptions_EnableInSituEditingDesc))]
        [Browsable(false)]
        public virtual bool EnableInSituEditing
        {
            get => _enableInSituEditing;
            set => _enableInSituEditing = value;
        }
    }
}
