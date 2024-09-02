# WinForms Libraries Feature Switches

Publicly documented [feature-switches](https://github.com/dotnet/designs/blob/master/accepted/2020/feature-switch.md) can be found on the [official docs](https://learn.microsoft.com/en-us/dotnet/core/deploying/trimming/trimming-options#trimming-framework-library-features). Additional details on feature switches usage can be found on the [runtime repo](https://github.com/LakshanF/runtime/blob/main/docs/workflow/trimming/feature-switches.md). Non-public feature switches that impact WinForms libraries can be found in the following table.

## Available Feature Switches

| MSBuild Property Name | AppContext Setting | Description |
|-|-|-|
| _ActiveXImplSupport | System.Windows.Forms.ActiveXImpl.IsSupported | WinForms ActiveX support is trimmed when set to false. |
| _MdiWindowDialogSupport | System.Windows.Forms.MdiWindowDialog.IsSupported | WinForms MdiWindowDialog support is trimmed when set to false. |
| _UseComponentModelRegisteredTypes | System.Windows.Forms.Control.UseComponentModelRegisteredTypes | Uses ComponentModel type registration feature when set to true. |
| _WinFormsBindingSupport | System.Windows.Forms.Binding.IsSupported | WinForms binding support is trimmed when set to false. |
| _WinFormsDesignTimeFeaturesSupport | System.Windows.Forms.Control.AreDesignTimeFeaturesSupported | WinForms design time support is trimmed when set to false. |
| _WinFormsImageIndexConverterSupport | System.Windows.Forms.ImageIndexConverter.IsSupported | WinForms ImageIndexConverter support is trimmed when set to false. |
| _WinFormsUITypeEditorSupport | System.Drawing.Design.UITypeEditor.IsSupported | WinForms UITypeEditor support is trimmed when set to false. |
