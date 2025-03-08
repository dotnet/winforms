// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "VSTHRD200:Use \"Async\" suffix for async methods")]
[assembly: SuppressMessage("Usage", "VSTHRD100:Avoid async void methods", Justification = "On purpose in test.", Scope = "member", Target = "~M:System.Windows.Forms.UITests.DragDropTests.DragDrop_RTF_FromExplorer_ToRichTextBox_ReturnsExpected_Async~System.Threading.Tasks.Task")]
[assembly: SuppressMessage("Usage", "VSTHRD003:Avoid awaiting foreign Tasks", Justification = "On purpose in test.", Scope = "member", Target = "~M:System.Windows.Forms.UITests.ControlTestBase.RunFormAsync``1(System.Func{System.ValueTuple{System.Windows.Forms.Form,``0}},System.Func{System.Windows.Forms.Form,``0,System.Threading.Tasks.Task})~System.Threading.Tasks.Task")]
[assembly: SuppressMessage("Usage", "VSTHRD003:Avoid awaiting foreign Tasks", Justification = "On purpose in test.", Scope = "member", Target = "~M:System.Windows.Forms.UITests.ControlTestBase.RunFormWithoutControlAsync``1(System.Func{``0},System.Func{``0,System.Threading.Tasks.Task})~System.Threading.Tasks.Task")]
