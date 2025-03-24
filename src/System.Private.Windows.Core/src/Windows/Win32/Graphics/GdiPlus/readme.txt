GDI+ interop should primarily live in System.Private.Windows.Core.GdiPlus. Only initialization and handles
should be defined here to allow WPF to use targeted GDI+ functionality without having to take the entirety of
System.Private.Windows.Core.GdiPlus.