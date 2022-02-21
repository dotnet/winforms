# How to troubleshoot the Windows Forms .NET Framework designer

Whilst the Windows Forms Out-Of-Process Designer (or the OOP designer) is by far and large is a black box to the users, it is still possible to find what more information on its state.

## Logging 

Once the OOP designer is started, its log is available in the Visual Studio Output window:

![image](https://user-images.githubusercontent.com/4403806/154604523-081f6197-d597-4416-9877-72fb61d12b8a.png)

The default verbosity is set to  _Warnings_, which includes Warnings and Errors (prior to VS 2022 17.2 was _None_, i.e. logging disabled). You can change the logging level to terser or more verbose under **Tools -> Options -> Windows Forms Designer -> General -> Logging Level**, e.g.:

![image](https://user-images.githubusercontent.com/4403806/154607481-44fa404a-894e-4656-95b8-efdeced2ace0.png)

> :warning: Some log events are not correctly classified. This is known, and the re-classification work is under way.

## Connection Timeouts

When the OOP designer starts it launches the designer server process (aka DesignToolsServer.exe). If the server process fails to launch and connect to the Visual Studio, you may see the following error:

```
[02:59:20.7198643] [MyProj]: Timed out waiting for the design tools server process launch to complete.
[02:59:20.7668637] Microsoft.DotNet.DesignTools.Client.ServerException: Timed out waiting for the design tools server process launch to complete.
```

The default connection timeout is set to _2 minutes_ (prior to VS 2022 17.1p5 was _10 seconds_). VS 2022 17.2 introduces the ability to increase the connection timeout, if necessary. You can change it under **Tools -> Options -> Windows Forms Designer -> General -> Connection timeout**, e.g.:

![image](https://user-images.githubusercontent.com/4403806/154607521-8665597b-b2e5-448b-adcb-a2c147570501.png)

There can be various reasons why the server process may be taking longer than the allocated time. For example, some users have reported to have solutions with a large number of projects. The designer performs a shadow copying of the necessary files and folders before it can render your form or control, and if you have a slow hard drive, or the project files are located on a network drive, it can have a significant effect on how fast files are copied to the shadow cache folder.

Another common reason for the connection timeouts is AV services, and adding exclusion rules for devevn.exe and DesignToolsServer.exe can increase the I/O of the shadow cache. For example, to add an exception to the Windows Defender so that it is not slowed down by the system call hooks Defender uses to provide real-time protection can be done with the following PowerShell command:

```powershell
Add-MpPreference -ExclusionProcess 'devenv.exe'
Add-MpPreference -ExclusionProcess 'DesignToolsServer.exe'
```
