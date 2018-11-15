// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System {
    using System;
#if DRAWINGDESIGN_NAMESPACE
    using System.Drawing.Design;
#elif WINFORMS_NAMESPACE
    using System.Windows.Forms;
#endif
    using System.Runtime.CompilerServices;

    /// <summary>
    /// This class maps accessibility quirks set in the app.config file or based on the target framework moniker 
    /// to accessibility levels that are referenced in the source code.
    /// </summary>
    internal static class AccessibilityImprovements {
        private static bool levelsValidated;
        private static int  useLegacyAccessibilityFeatures;
        private static int  useLegacyAccessibilityFeatures2;
        private static int  useLegacyAccessibilityFeatures3;

        private static int  useLegacyToolTipDisplayBehavior;

        // We assume that accessibility quirks have the same names in different assemblies.
        internal const string UseLegacyAccessibilityFeaturesSwitchName = @"Switch.UseLegacyAccessibilityFeatures";
        internal const string UseLegacyAccessibilityFeatures2SwitchName = @"Switch.UseLegacyAccessibilityFeatures.2";
        internal const string UseLegacyAccessibilityFeatures3SwitchName = @"Switch.UseLegacyAccessibilityFeatures.3";

        internal const string UseLegacyToolTipDisplaySwitchName = @"Switch.System.Windows.Forms.UseLegacyToolTipDisplay";

        /// <summary>
        /// False - opt-out of the accessibility improvements introduced in 4.7.1.
        /// This is the case of application that target .NET frameworks below 4.7.1 or 
        /// applications that target 4.7.1 and above and have the following  
        /// <AppContextSwitchOverrides value="Switch.UseLegacyAccessibilityFeatures=true" />
        /// in the <runtime> section in the app.config file.
        /// 
        /// True - opt-in into the accessibility improvements introduced in 4.7.1.
        /// </summary>
        internal static bool Level1 {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get {
                if (useLegacyAccessibilityFeatures < 0) return true;
                if (useLegacyAccessibilityFeatures > 0) return false;
                ValidateLevels();

                return (useLegacyAccessibilityFeatures < 0);
            }
        }

        /// <summary>
        /// Opt in or out of the accessibility improvements introduced in .NET Framework versions 4.7.2 and 4.7.1.
        /// </summary>
        internal static bool Level2 {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get {
                if (useLegacyAccessibilityFeatures2 < 0) return true;
                if (useLegacyAccessibilityFeatures2 > 0) return false;
                ValidateLevels();

                return (useLegacyAccessibilityFeatures2 < 0);
            }
        }

        /// <summary>
        /// Opt in or out of the accessibility improvements introduced in .NET Framework version 4.7.3.
        /// </summary>
        internal static bool Level3 {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get {
                if (useLegacyAccessibilityFeatures3 < 0) return true;
                if (useLegacyAccessibilityFeatures3 > 0) return false;
                ValidateLevels();

                return (useLegacyAccessibilityFeatures3 < 0);
            }
        }

        /// <summary>
        /// Opt in or out of the keyboard tooltips triggering
        /// </summary>
        internal static bool UseLegacyToolTipDisplay {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get {
                if (useLegacyToolTipDisplayBehavior > 0) return true;
                if (useLegacyToolTipDisplayBehavior < 0) return false;
                ValidateLevels();

                return (useLegacyToolTipDisplayBehavior > 0);
            }
        }

        internal static void ValidateLevels() {
            if (levelsValidated) {
                return;
            }

            // Level[N+1] accessibility improvements are building upon the infrastructure introduced in Level[N],
            // thus the application has to opt-in into Level[N] level in order to get the Level[N+1] level of support. 
            Tuple<string, Action<int>>[] levelNamesAndSetters = {
                Tuple.Create<string, Action<int>>(UseLegacyAccessibilityFeaturesSwitchName, (switchValue) => useLegacyAccessibilityFeatures = switchValue),
                Tuple.Create<string, Action<int>>(UseLegacyAccessibilityFeatures2SwitchName, (switchValue) => useLegacyAccessibilityFeatures2 = switchValue),
                Tuple.Create<string, Action<int>>(UseLegacyAccessibilityFeatures3SwitchName, (switchValue) => useLegacyAccessibilityFeatures3 = switchValue)
            };
            bool higherLevelsAreExpectedToBeDisabled = false;
            bool invalidSwitchesCombination = false;
            bool[] levelState = new bool[levelNamesAndSetters.Length];
            for (int i = 0; i < levelNamesAndSetters.Length; i++) {
                string levelName = levelNamesAndSetters[i].Item1;
                Action<int> setLevelSwitchValue = levelNamesAndSetters[i].Item2;

                int levelSwitchValue = 0;
                bool levelIsDisabled = LocalAppContext.GetCachedSwitchValue(levelName, ref levelSwitchValue);

                if (levelIsDisabled) {
                    higherLevelsAreExpectedToBeDisabled = true;
                }
                else if (higherLevelsAreExpectedToBeDisabled) {
                    invalidSwitchesCombination = true;
                }

                setLevelSwitchValue(levelSwitchValue);

                levelState[i] = levelIsDisabled;
            }

            if (invalidSwitchesCombination) {
#if DEPLOYMENT_NAMESPACE
                throw new NotSupportedException(System.Deployment.Application.Resources.GetString("CombinationOfAccessibilitySwitchesNotSupported"));
#else
                throw new NotSupportedException(SR.CombinationOfAccessibilitySwitchesNotSupported);
#endif
            }

            // Get other improvements values
            bool useLegacyToolTipDisplay = LocalAppContext.GetCachedSwitchValue(UseLegacyToolTipDisplaySwitchName, ref useLegacyToolTipDisplayBehavior);

            // If new ToolTip display behavior is enabled, Level3 should be enabled
            if (!useLegacyToolTipDisplay && levelState[2]) {
#if DEPLOYMENT_NAMESPACE
                throw new NotSupportedException(System.Deployment.Application.Resources.GetString("KeyboardToolTipDisplayBehaviorRequiresAccessibilityImprovementsLevel3"));
#else
                throw new NotSupportedException(SR.KeyboardToolTipDisplayBehaviorRequiresAccessibilityImprovementsLevel3);
#endif
            }

            // If this code is executed concurrently, in the worst case we'll throw the same exception on each thread.
            levelsValidated = true;
        }
    }
}
