namespace System.Windows.Forms;

public partial class Popup
{
    // A helper class that provides methods and properties for system information
    private static class SystemInfoHelper
    {
        // A property that returns the grip symbol height
        public static int GripSymbolHeight
        {
            get { return (int)(SystemInformation.HorizontalScrollBarThumbWidth * ScalingFactor); }
        }

        // A property that returns the status bar height
        public static int StatusBarHeight
        {
            get { return (int)(SystemInformation.CaptionHeight * ScalingFactor); }
        }

        // A property that returns the scroll bar width
        public static int ScrollBarWidth
        {
            get { return (int)(SystemInformation.VerticalScrollBarWidth * ScalingFactor); }
        }

        // A property that returns the scroll bar height
        public static int ScrollBarHeight
        {
            get { return (int)(SystemInformation.HorizontalScrollBarHeight * ScalingFactor); }
        }

        // A property that returns the scaling factor based on the current DPI mode
        public static double ScalingFactor =>
            DpiHelper.IsScalingRequired
                ? DpiHelper.LogicalToDeviceUnitsScalingFactor
                : 1.0;
    }
}
