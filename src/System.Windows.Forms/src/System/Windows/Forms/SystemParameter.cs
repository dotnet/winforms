// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {

    using System.Diagnostics;
    using System;
    using System.ComponentModel;
    using Microsoft.Win32;



    /// <devdoc>
    ///    <para>
    ///       Specifies the
    ///       SystemParameterType.
    ///    </para>
    /// </devdoc>
    [System.Runtime.InteropServices.ComVisible(true)]
    public enum SystemParameter {


        /// <devdoc>
        ///    <para>
        ///       DropShadow.
        ///    </para>
        /// </devdoc>
        DropShadow = 0,


        /// <devdoc>
        ///    <para>
        ///       Flat Menu.
        ///    </para>
        /// </devdoc>
        FlatMenu = 1,


        /// <devdoc>
        ///    <para>
        ///       FontSmoothingContrastMetric.
        ///    </para>
        /// </devdoc>
        FontSmoothingContrastMetric = 2,


        /// <devdoc>
        ///    <para>
        ///       FontSmoothingTypeMetric.
        ///    </para>
        /// </devdoc>
        FontSmoothingTypeMetric = 3,


        /// <devdoc>
        ///    <para>
        ///       MenuFadeEnabled.
        ///    </para>
        /// </devdoc>
        MenuFadeEnabled = 4,
        

        /// <devdoc>
        ///    <para>
        ///       SelectionFade.
        ///    </para>
        /// </devdoc>
        SelectionFade = 5,
        

        /// <devdoc>
        ///    <para>
        ///       ToolTipAnimationMetric.
        ///    </para>
        /// </devdoc>
        ToolTipAnimationMetric = 6,


        /// <devdoc>
        ///    <para>
        ///       UIEffects.
        ///    </para>
        /// </devdoc>
        UIEffects = 7,


        /// <devdoc>
        ///    <para>
        ///       CaretWidthMetric.
        ///    </para>
        /// </devdoc>
        CaretWidthMetric = 8,


        /// <devdoc>
        ///    <para>
        ///       VerticalFocusThicknessMetric.
        ///    </para>
        /// </devdoc>
        VerticalFocusThicknessMetric = 9,


        /// <devdoc>
        ///    <para>
        ///       HorizontalFocusThicknessMetric.
        ///    </para>
        /// </devdoc>
        HorizontalFocusThicknessMetric = 10,
    }
}

