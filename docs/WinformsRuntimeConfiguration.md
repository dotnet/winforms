**Runtime configuration in legacy .NET framework applications**

.NET framework winforms applications use app.config to define application wide settings used by Winforms applications and runtime config option like AppContext switches to opt-in or opt out of the new features released in the latest .NET framework versions. Following are the various sections in the app.config that define Winforms application's and their runtime behavior.

**AppContext switches**

These settings are used to opt-in or opt-out of a particular feature from WinForms runtime. Please see [AppContext Switches](https://docs.microsoft.com/en-us/dotnet/framework/configure-apps/file-schema/runtime/appcontextswitchoverrides-element) for more information

```XML
<configuration>
   <runtime>
      <AppContextSwitchOverrides value="Switch.System.Globalization.NoAsyncCurrentCulture=true" />
   </runtime>
</configuration>
```
**System.Windows.Forms.ApplicationConfigurationSection**

This was introduced in .NET framework 4.7 and is primarily used by Winforms runtime to enable HighDpi and other accessibility improvements made in the .NET framework 4.7 and above versions. Please refer to [ApplicationConfigurationSection](https://docs.microsoft.com/en-us/dotnet/framework/configure-apps/file-schema/winforms/windows-forms-add-configuration-element) for more information.

```XML
<configuration>
  <System.Windows.Forms.ApplicationConfigurationSection>
  ...
  </System.Windows.Forms.ApplicationConfigurationSection>
</configuration>
```

**App settings from Settings designer/editor page**

Unlike above, these settings are used by the user application. These are commonly defined via settings designer in Visual Studio which intern serialize them into app.config file. Please refer to [Application Settings](https://docs.microsoft.com/en-us/dotnet/desktop/winforms/advanced/using-application-settings-and-user-settings?view=netframeworkdesktop-4.8) for more information.
```XML
 <userSettings>
        <WinFormsApp2.Properties.Settings>
            <setting name="Settingdfsd" serializeAs="String">
                <value>dfds</value>
            </setting>
        </WinFormsApp2.Properties.Settings>
    </userSettings>
    <applicationSettings>
        <WinFormsApp2.Properties.Settings>
            <setting name="dfsd" serializeAs="String">
                <value>sdfsdgs</value>
            </setting>
        </WinFormsApp2.Properties.Settings>
    </applicationSettings>
```


**Winforms runtime configuration in .NET (Core) applications**

.NET Winforms applications currently have [limited application
configurations](https://docs.microsoft.com/en-us/dotnet/desktop/winforms/whats-new/net60?view=netdesktop-6.0#new-application-bootstrap) defined at build time via project file that are then emitted into source code using source
generators at compile time. This document outlines expansion of those application wide configurations further to cover runtime config options.

**runtimeconfig.json for Winforms .NET applications.**

app.config has limited support in .NET and goal is to move away from using it in .NET for
performance and reliability reasons. .NET runtime and other .NET teams use runtimeconfig.json to define .NET runtime configurations and appsettings.json to define application-level settings. In this proposal, we are leveraging runtimeconfig.json to define WinForms runtime configurations.

While this proposal is focusing on providing an alternative solution for existing configuration sections `AppContextSwitchOverrides` and `System.Windows.Forms.ApplicationConfigurationSection` that are primarily used for specifying feature flags impacting winforms runtime behavior, we will be looking into alternatives for `Application Settings` that doesn't require app.config in the upcoming releases of .NET.


**Goals:**

-   Replacement for `AppContextSwitchOverrides` and `System.Windows.Forms.ApplicationConfigurationSection` of app.config.

-   Users should be able to update/modify Winforms applications runtime configurations without recompiling the application

-   Existing applications should be able to seamlessly upgrade to this
    new model when targeting to latest .NET.
-   Existing [build time properties]((https://docs.microsoft.com/en-us/dotnet/desktop/winforms/whats-new/net60?view=netdesktop-6.0#new-application-bootstrap)) defined via project file continue to work.

**Out of scope:**

-  App settings that are serialized from Settings designer/editor page. Applications should continue to use current model until we come up with an alternative solution for this.
-  Dynamic/real-time loading of configuration values from runtimeconfig.json.
-  Unification of Build/Runtime configurations into one place, runtimeconfig.json.
    -   We keep current build time configurations in the project file while adding new runtime configurations into runtimeconfig.json. We may revisit build time properties for unification in the upcoming releases of .NET.


**Syntax of runtimeConfig.template.Json**.

```xml
{
  "configProperties": {
      "System.Globalization.UseNls": true,
      "System.Net.DisableIPv6": true,
      "System.GC.Concurrent": false,
      "System.Threading.ThreadPool.MinThreads": 4,
      "System.Threading.ThreadPool.MaxThreads": 25
    }
}
```

Given this section is [common across .NET products](https://docs.microsoft.com/en-us/dotnet/core/runtime-config), Winforms specific switches will have unique names for its feature flags to avoid conflicts with other .NET teams. Following is the syntax that winforms adapt while defining a new feature switch/flag.

   Winforms runtime flags/settings: "System.Windows.Forms.<runtime_flag/setting_name>"

ex: 
```xml
{
  "configProperties": {
      "System.Globalization.UseNls": true,
      "System.Net.DisableIPv6": true,
      "System.GC.Concurrent": false,
      "System.Threading.ThreadPool.MinThreads": 4,
      "System.Threading.ThreadPool.MaxThreads": 25,
      "System.Windows.Forms.ScaleTopLevelFormMinMaxSize": true,
      "System.Windows.Forms.<CustomEnumProperty>": "EnumValue"
    }
}
```


**Reading Winforms runtime configurations:**

Build system [generate](https://docs.microsoft.com/en-us/dotnet/core/runtime-config) `<AppName>.runtimeconfig.json` file in the output directory with the content specified in the `runtimeconfig.template.json` file along with application target framework information. .NET runtime recognizes this file and get it loaded while running Winforms applications.

In this proposal, we will be focusing primarily on enabling only runtime configurations for WinForms applications. Support for analyzers and source generators are considered in later .NET releases. We will revisit this implementation to improve user-experience further, as we make progress.

Ex: Content of <AppName>.runtimeconfig.json generated from above content.

```XML
{
  "runtimeOptions": {
    "tfm": "net7.0",
    "frameworks": [
      {
        "name": "Microsoft.NETCore.App",
        "version": "7.0.0-preview.7.22375.6"
      },
      {
        "name": "Microsoft.WindowsDesktop.App",
        "version": "7.0.0-preview.7.22377.1"
      }
    ],
    "configProperties": {
      "System.Globalization.UseNls": true,
      "System.Net.DisableIPv6": true,
      "System.GC.Concurrent": false,
      "System.Threading.ThreadPool.MinThreads": 4,
      "System.Threading.ThreadPool.MaxThreads": 25,
      "Switch.System.Windows.Forms.ScaleTopLevelFormMinMaxSize": true,
      "System.Windows.Forms.<StringProperty>": "string"
      "System.Windows.Forms.<CustomProperty>": "CustomValue"
    }
  }
}
```

Target framework information added to <AppName>.runtimeconfig.json file always match with application target framework irrespective of runtime/SDK installed on the machine or runtime used (Roll-forward scenarios) by the WinForms application.

**.NET runtime support in reading runtimeconfig.json:**

WinForms leveraging [the support](https://github.com/dotnet/runtime/blob/5098d45cc1bf9649fab5df21f227da4b80daa084/src/native/corehost/runtime_config.cpp) provided by .NET runtime in reading the `<AppName>.runtimeconfig.json` file and thus get all plumbing required for various hosting and environment scenarios. 

WinForms add following wrapper on top of runtime implementation to improve performance by caching configuration options and also to define custom defaults values for the WinForms specific feature flags. This is very similar to the [implementation](https://github.com/dotnet/runtime/blob/04dac7b0fede29d44f896c5fd793754f83974175/src/libraries/System.Private.CoreLib/src/System/AppContextConfigHelper.cs) used in various .NET runtime components.

Below example illustrate making the feature flag `ScaleTopLevelFormMinMaxSizeForDpi` default opt-in if application is running on Windows 10 and targeting .NET 8.

```cs
 internal static partial class LocalAppContextSwitches
    {
        // Property to cache feature flag "Switch.System.Windows.Forms.ScaleTopLevelFormMinMaxSize" value. 
        private static int s_scaleTopLevelFormMinMaxSize;
        public static bool ScaleTopLevelFormMinMaxSize
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => GetCachedSwitchValue("Switch.System.Windows.Forms.ScaleTopLevelFormMinMaxSizeForDpi", ref s_scaleTopLevelFormMinMaxSize);
        }

        private static bool GetCachedSwitchValueInternal(string switchName, ref int cachedSwitchValue)
        {
            bool hasSwitch = AppContext.TryGetSwitch(switchName, out bool isSwitchEnabled);
            if (!hasSwitch)
            {
                isSwitchEnabled = GetSwitchDefaultValue(switchName);
            }

            // Is caching switches disabled?.
            AppContext.TryGetSwitch("TestSwitch.LocalAppContext.DisableCaching", out bool disableCaching);
            if (!disableCaching)
            {
                cachedSwitchValue = isSwitchEnabled ? 1 /*true*/ : -1 /*false*/;
            }

            return isSwitchEnabled;
        }

        // Provides default values for switches if they're not always false by default
        private static bool GetSwitchDefaultValue(string switchName)
        {
            if (OsVersion.IsWindows10_1703OrGreater)
            {
                var tfm = RuntimeTargetFramework.Framework;
                if (tfm is not null && tfm.Name == "Microsoft.NETCore.App" && string.Compare(tfm.Version,"8.0", StringComparison.OrdinalIgnoreCase) >=0)
                {
                    if (switchName == "Switch.System.Windows.Forms.ScaleTopLevelFormMinMaxSizeForDpi")
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
```


WinForms runtime then use the static `LocalAppContextSwitches` class to access runtime configurations. Below is sample accessing feature switch `Switch.System.Windows.Forms.ScaleTopLevelFormMinMaxSize`. 

Ex: Use of feature switch in OnDpiChanged() method in Form.cs to scale Min/Max size of the Form.

```cs
 protected virtual void OnDpiChanged(DpiChangedEventArgs e)
        {
            if (e.DeviceDpiNew != e.DeviceDpiOld)
            {
                CommonProperties.xClearAllPreferredSizeCaches(this);
                _oldDeviceDpi = e.DeviceDpiOld;

                // call any additional handlers
                ((DpiChangedEventHandler?)Events[EVENT_DPI_CHANGED])?.Invoke(this, e);

                if (!e.Cancel)
                {
                  if(LocalAppContextSwitch.ScaleTopLevelFormMinMaxSize)
                  {
                     ScaleMinMaxSize();
                  }
                  
                  ScaleContainerForDpi(e.DeviceDpiNew, e.DeviceDpiOld, e.SuggestedRectangle);
                }
            }
        }
```
![OptinSwitch](images/RuntimeOptionsDemo.gif)