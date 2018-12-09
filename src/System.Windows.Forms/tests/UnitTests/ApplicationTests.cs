using Xunit;

namespace System.Windows.Forms.Tests
{
    public class ApplicationTests
    {
        [Fact]
        public void Application_EnsureVisualStylesAreEnabled()
        {
            Application.EnableVisualStyles();
            Assert.True(Application.UseVisualStyles, "New Visual Styles will not be applied on Winforms app. This is a high priority bug and must be looked into");            
        }

        [Fact]
        public void Application_TestHighDpiMode()
        {

            // This test should have the same outcome from Windows 7 to Windows {current}.
            HighDpiMode tempHighDpiMode = Application.HighDpiMode;
            Assert.False(Application.SetHighDpiMode(HighDpiMode.Uninitialized),
                "Passing HighDpiMode.Uninitialized should always return false.");
            Assert.True(tempHighDpiMode == Application.HighDpiMode,
                "Passing HighDpiMode.Uninitialized should never actually change the HighDpiMode");

            // This test will only succeed on Windows 10 >= 1607.
            if (Environment.OSVersion.Version.Build >= 14393)
            {
                Assert.True(Application.SetHighDpiMode(HighDpiMode.DpiUnaware));
                Assert.True(Application.HighDpiMode == HighDpiMode.DpiUnaware);

                Assert.True(Application.SetHighDpiMode(HighDpiMode.SystemAware));
                Assert.True(Application.HighDpiMode == HighDpiMode.SystemAware);

                Assert.True(Application.SetHighDpiMode(HighDpiMode.PerMonitor));
                Assert.True(Application.HighDpiMode == HighDpiMode.PerMonitor);

                Assert.True(Application.SetHighDpiMode(HighDpiMode.PerMonitorV2));
                Assert.True(Application.HighDpiMode == HighDpiMode.PerMonitorV2);
            }
            else if(Environment.OSVersion.Version.Build >= 9600)
            {
                Assert.True(Application.SetHighDpiMode(HighDpiMode.DpiUnaware));
                Assert.True(Application.HighDpiMode == HighDpiMode.DpiUnaware);

                Assert.True(Application.SetHighDpiMode(HighDpiMode.SystemAware));
                Assert.True(Application.HighDpiMode == HighDpiMode.SystemAware);

                Assert.True(Application.SetHighDpiMode(HighDpiMode.PerMonitor));
                Assert.True(Application.HighDpiMode == HighDpiMode.PerMonitor);

                Assert.True(Application.SetHighDpiMode(HighDpiMode.PerMonitorV2));
                Assert.True(Application.HighDpiMode == HighDpiMode.PerMonitor);

            }
            else if (Environment.OSVersion.Version.Build >= 7600)
            {
                Assert.True(Application.SetHighDpiMode(HighDpiMode.DpiUnaware));
                Assert.True(Application.HighDpiMode == HighDpiMode.DpiUnaware);

                Assert.True(Application.SetHighDpiMode(HighDpiMode.SystemAware));
                Assert.True(Application.HighDpiMode == HighDpiMode.SystemAware);

                Assert.True(Application.SetHighDpiMode(HighDpiMode.PerMonitor));
                Assert.True(Application.HighDpiMode == HighDpiMode.SystemAware);

                Assert.True(Application.SetHighDpiMode(HighDpiMode.PerMonitorV2));
                Assert.True(Application.HighDpiMode == HighDpiMode.SystemAware);
            }

            Assert.False(true, "This test cannot succeed on this OS.");
        }
    }
}
