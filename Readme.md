# CoAP Explorer 

[![Build status](https://ci.appveyor.com/api/projects/status/njym61gix1mygnqg/branch/master?svg=true)](https://ci.appveyor.com/project/NZSmartie/coapexplorer/branch/master)

Work in Progress App for interacting with CoAP devices. Soon to be cross platform, for now, is targeting Windows.

Thanks To:
 - [ReactiveUI](https://github.com/reactiveui/ReactiveUI/) - Reactive Style UI
 - [Material Deisgn Toolkit](https://github.com/ButchersBoy/MaterialDesignInXamlToolkit) Google's Material Design for WIndows Presentation Framework
 - [AvalonEdit](https://github.com/icsharpcode/AvalonEdit) - Text Highlighter for WPF
 - [CoAP.Net](https://github.com/NZSmartie/CoAP.Net/) - My very own CoAP library

Latest nightly builds for Windows can be downloaded straight from AppVeyor - https://ci.appveyor.com/project/NZSmartie/coapexplorer/build/artifacts

## Goals

 - Cross Platform
   - Using the same concepts from Xamarin Apps, the core functionality is in the schared project (Targeting .Net Standard)
 - Device Discovery
   - [X] UDP Multicast Discovery.
   - [ ] TODO: Suport more transports.
 - Send Messages

## Screen Grabs

![Device Discovery](Media/2018-03-15_11-37-33.gif)