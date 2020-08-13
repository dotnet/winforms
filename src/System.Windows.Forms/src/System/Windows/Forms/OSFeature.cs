// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

namespace System.Windows.Forms
{
    /// <summary>
    ///  Provides operating-system specific feature queries.
    /// </summary>
    public class OSFeature : FeatureSupport
    {
        /// <summary>
        ///  Represents the layered, top-level windows feature. This <see langword='static'/> field
        ///  is read-only.
        /// </summary>
        public static readonly object LayeredWindows = new object();

        /// <summary>
        ///  Determines if the OS supports themes
        /// </summary>
        public static readonly object Themes = new object();

        private static OSFeature _feature;

        /// <summary>
        ///  Initializes a new instance of the <see cref='OSFeature'/> class.
        /// </summary>
        protected OSFeature()
        {
        }

        /// <summary>
        ///  Represents the <see langword='static'/> instance of <see cref='OSFeature'/>
        ///  to use for feature queries. This property is read-only.
        /// </summary>
        public static OSFeature Feature => _feature ?? (_feature = new OSFeature());

        /// <summary>
        ///  Retrieves the version of the specified feature currently available on the system.
        /// </summary>
        public override Version GetVersionPresent(object feature)
        {
            // These are always supported on platforms that .NET Core supports.
            if (feature == LayeredWindows || feature == Themes)
            {
                return new Version(0, 0, 0, 0);
            }

            return null;
        }

        /// <summary>
        ///  Retrieves whether SystemParameterType is supported on the Current OS version.
        /// </summary>
        public static bool IsPresent(SystemParameter enumVal)
        {
            switch (enumVal)
            {
                case SystemParameter.DropShadow:
                case SystemParameter.FlatMenu:
                case SystemParameter.FontSmoothingContrastMetric:
                case SystemParameter.FontSmoothingTypeMetric:
                case SystemParameter.MenuFadeEnabled:
                case SystemParameter.SelectionFade:
                case SystemParameter.ToolTipAnimationMetric:
                case SystemParameter.UIEffects:
                case SystemParameter.CaretWidthMetric:
                case SystemParameter.VerticalFocusThicknessMetric:
                case SystemParameter.HorizontalFocusThicknessMetric:
                    return true;
            }

            return false;
        }
    }
}
