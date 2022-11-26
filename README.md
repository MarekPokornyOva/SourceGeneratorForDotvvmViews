# SourceGeneratorForDotvvmViews

### Introduction
This is naive attempt to investigate posibility to compile DotVVM views as .NET types/methods. The idea is to compile the views similar way as Razor pages.

### Motivation
Design time compilation could:
- speed up application start or page/control first rendering
- ability to use/render the views out of route handler
- ability to manage the views for any purpose
- allow other C# features (partial classes) usage

### High level plan
This solution creates Source Generator which generates C# types/methods and adds them to the application project's compilation.

### Investigation and found issues
The first issue is DotVVM framework is written targetting .NET FW 4.7 and .NET Standard 2.1 whereas Source Generator needs to target .NET Standard 2.0. Therefore its needed to backport the framework - see the modified files in /Original directory.  
Another issue is the framework is complex and I was able to bring support of static texts/html only. Model binding was outscoped.

### VS Solution organization
- /Original - files modified to add .NET Standard 2.0 support. Other files can be downloaded from the original repository - I used https://github.com/riganti/dotvvm/tree/b9bb6f7d8f99e1fbc2942f56048ccca5bafb6730.
- /DotvvmApplication3 - sample application which uses compiled views.
- /Generator - the Source Generator.
- /ViewParserInvestigation - project used to investigate the framework.

### Conclusion
The attempt revealed it's possible to bring design time compilation however it seems the code refactor would be massive - probably separate the framework services to design time (parser/lexer/...) and runtime ones as a first step.
