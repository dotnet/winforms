// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <summary>
    /// Provides operating-system specific feature queries.
    /// </summary>
    public class OSFeature : FeatureSupport
    {
        /// <summary>
        /// Represents the layered, top-level windows feature. This <see langword='static'/> field
        /// is read-only.
        /// </summary>
        public static readonly object LayeredWindows = new object();

        /// <summary>
        /// Determines if the OS supports themes
        /// </summary>
        public static readonly object Themes = new object();

        private static OSFeature _feature = null;

        private static bool _themeSupportTested = false;
        private static bool _themeSupport = false;

        /// <summary>
        /// Initializes a new instance of the <see cref='System.Windows.Forms.OSFeature'/> class.
        /// </summary>
        protected OSFeature()
        {
        }

        /// <summary>
        /// Represents the <see langword='static'/> instance of <see cref='System.Windows.Forms.OSFeature'/>
        // to use for feature queries. This property is read-only.
        /// </summary>
        public static OSFeature Feature => _feature ?? (_feature = new OSFeature());

        /// <summary>
        /// Retrieves the version of the specified feature currently available on the system.
        /// </summary>
        public override Version GetVersionPresent(object feature)
        {
            if (feature == LayeredWindows)
            {
                return new Version(0, 0, 0, 0);
            }
            else if (feature == Themes)
            {
                if (!_themeSupportTested)
                {
                    try
                    {
                        SafeNativeMethods.IsAppThemed();
                        _themeSupport = true;
                    }
                    catch
                    {
                        _themeSupport = false;
                    }

                    _themeSupportTested = true;
                }

                if (_themeSupport)
                {
                    return new Version(0, 0, 0, 0);
                }
            }

            return null;
        }

        internal bool OnXp
        {
            get
            {
                bool onXp = false;
                if (Environment.OSVersion.Platform == System.PlatformID.Win32NT)
                {
                    onXp = Environment.OSVersion.Version.CompareTo(new Version(5, 1, 0, 0)) >= 0;
                }
                return onXp;
            }
        }

        internal bool OnWin2k
        {
            get
            {
                bool onWin2k = false;
                if (Environment.OSVersion.Platform == System.PlatformID.Win32NT)
                {
                    onWin2k = Environment.OSVersion.Version.CompareTo(new Version(5, 0, 0, 0)) >= 0;
                }
                return onWin2k;
            }
        }

        /// <summary>
        /// Retrieves whether SystemParameterType is supported on the Current OS version.
        /// </summary>
        public static bool IsPresent(SystemParameter enumVal)
        {
            switch (enumVal)
            {
                case SystemParameter.DropShadow:
                    return Feature.OnXp;
                    

                case SystemParameter.FlatMenu:
                    return Feature.OnXp;
                    

                case SystemParameter.FontSmoothingContrastMetric:
                    return Feature.OnXp;
                    
                        
                case SystemParameter.FontSmoothingTypeMetric:
                    return Feature.OnXp;
                    

                case SystemParameter.MenuFadeEnabled:
                    return Feature.OnWin2k;
                    
               
                case SystemParameter.SelectionFade:
                    return Feature.OnWin2k;
                    

                case SystemParameter.ToolTipAnimationMetric:
                    return Feature.OnWin2k;
                    

                case SystemParameter.UIEffects:
                    return Feature.OnWin2k;
                    

                case SystemParameter.CaretWidthMetric:
                    return Feature.OnWin2k;
                    

                case SystemParameter.VerticalFocusThicknessMetric:
                    return Feature.OnXp;
                    

                case SystemParameter.HorizontalFocusThicknessMetric:
                    return Feature.OnXp;
            }

            return false;
        }
    }
}
