// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {

    using System.Diagnostics;
    using System;
    using System.ComponentModel;
    using Microsoft.Win32;


    /// <include file='doc\SystemParameter.uex' path='docs/doc[@for="SystemParameter"]/*' />
    /// <devdoc>
    ///    <para>
    ///       Specifies the
    ///       SystemParameterType.
    ///    </para>
    /// </devdoc>
    [System.Runtime.InteropServices.ComVisible(true)]
    public enum SystemParameter {

        /// <include file='doc\SystemParameter.uex' path='docs/doc[@for="SystemParameter.DropShadow"]/*' />
        /// <devdoc>
        ///    <para>
        ///       DropShadow.
        ///    </para>
        /// </devdoc>
        DropShadow = 0,

        /// <include file='doc\SystemParameter.uex' path='docs/doc[@for="SystemParameter.FlatMenu"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Flat Menu.
        ///    </para>
        /// </devdoc>
        FlatMenu = 1,

        /// <include file='doc\SystemParameter.uex' path='docs/doc[@for="SystemParameter.FontSmoothingContrastMetric"]/*' />
        /// <devdoc>
        ///    <para>
        ///       FontSmoothingContrastMetric.
        ///    </para>
        /// </devdoc>
        FontSmoothingContrastMetric = 2,

        /// <include file='doc\SystemParameter.uex' path='docs/doc[@for="SystemParameter.FontSmoothingTypeMetric"]/*' />
        /// <devdoc>
        ///    <para>
        ///       FontSmoothingTypeMetric.
        ///    </para>
        /// </devdoc>
        FontSmoothingTypeMetric = 3,

        /// <include file='doc\SystemParameter.uex' path='docs/doc[@for="SystemParameter.MenuFadeEnabled"]/*' />
        /// <devdoc>
        ///    <para>
        ///       MenuFadeEnabled.
        ///    </para>
        /// </devdoc>
        MenuFadeEnabled = 4,
        
        /// <include file='doc\SystemParameter.uex' path='docs/doc[@for="SystemParameter.SelectionFade"]/*' />
        /// <devdoc>
        ///    <para>
        ///       SelectionFade.
        ///    </para>
        /// </devdoc>
        SelectionFade = 5,
        
        /// <include file='doc\SystemParameter.uex' path='docs/doc[@for="SystemParameter.ToolTipAnimationMetric"]/*' />
        /// <devdoc>
        ///    <para>
        ///       ToolTipAnimationMetric.
        ///    </para>
        /// </devdoc>
        ToolTipAnimationMetric = 6,

        /// <include file='doc\SystemParameter.uex' path='docs/doc[@for="SystemParameter.UIEffects"]/*' />
        /// <devdoc>
        ///    <para>
        ///       UIEffects.
        ///    </para>
        /// </devdoc>
        UIEffects = 7,

        /// <include file='doc\SystemParameter.uex' path='docs/doc[@for="SystemParameter.CaretWidthMetric"]/*' />
        /// <devdoc>
        ///    <para>
        ///       CaretWidthMetric.
        ///    </para>
        /// </devdoc>
        CaretWidthMetric = 8,

        /// <include file='doc\SystemParameter.uex' path='docs/doc[@for="SystemParameter.VerticalFocusThicknessMetric"]/*' />
        /// <devdoc>
        ///    <para>
        ///       VerticalFocusThicknessMetric.
        ///    </para>
        /// </devdoc>
        VerticalFocusThicknessMetric = 9,

        /// <include file='doc\SystemParameter.uex' path='docs/doc[@for="SystemParameter.HorizontalFocusThicknessMetric"]/*' />
        /// <devdoc>
        ///    <para>
        ///       HorizontalFocusThicknessMetric.
        ///    </para>
        /// </devdoc>
        HorizontalFocusThicknessMetric = 10,
    }
}

