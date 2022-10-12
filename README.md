# Custom root designer demo

This sample shows how to build custom root designer for the Windows Forms out-of-process designer.

![Screenshot](screenshot.png)

https://user-images.githubusercontent.com/4403806/195274386-f27314db-d4c5-48cd-bb5c-c1726c34a6f5.mp4


:exclamation: This sample requires Visual Studio 17.5.

## Solution structure

* `/src`
    * `CustomControl` project contains implementations of the custom root designers.
        * `SampleRootComponentDesigner` a custom root designer based on [`ComponentDesigner`](https://docs.microsoft.com/dotnet/api/system.componentmodel.design.componentdesigner).
        * `SampleRootComponentDocumentDesigner` a custom root designer based on [`ComponentDocumentDesigner`](https://docs.microsoft.com/dotnet/api/system.windows.forms.design.componentdocumentdesigner).
    * `CustomControl.Client` project contains implementations of the proxies for the custom root designers used by Visual Studio.
        * `SampleRootComponentProxyDesigner` a proxy for `SampleRootComponentDesigner`.
        * `SampleRootComponentDocumentProxyDesigner` a proxy for `SampleRootComponentDocumentDesigner`.
        * `DesignerRoutingDefinitionProvider` facilitates mapping of the proxies to their respective concrete designers.
    * `CustomControl.Package` project generates a NuGet package that can be used by the Windows Forms out-of-process designer.
* `/demo`
    * `NetFxApp` project targets .NET Framework 4.7.2 and shows the basic custom root designer functionality as described in https://docs.microsoft.com/dotnet/api/system.componentmodel.design.irootdesigner.
    * `NetApp` project targets .NET 6.0 and shows the same custom root designer functionality in the Windows Forms out-of-process designer referencing CustomControl.Package NuGet package.


## Build instructions

```cmd
dotnet build CustomRootDesigners.sln
start Demo.sln
```
