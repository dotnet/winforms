Can you please implement an additional Analyzer unit test according to 
#file:'AnalyzerTests-Copilot-Instructions.md' for the 
#class:'System.Windows.Forms.CSharp.Analyzers.MissingPropertySerializationConfiguration.MissingPropertySerializationConfigurationAnalyzer':436-4056 ? 

We need to a test class which tests that makes sure
* No static Properties get flagged.
* No properties get flagged in side of classes which are inherited/implemented based of 
  `IComponent` alright, but not the `System.ComponentModel` versions.
* No Properties with a private setting get flagged.
* We have at least one test case, where we have an inherited property which has 
  been correctly attributed, so, the overwritten one should or should not cause 
  the Analyzer to trigger.

These four cases can be combined using one additional test class, and one additional 
test data folder.

This is for C#.
