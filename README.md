# ARCtrl.Querymodel

> [!NOTE]  
> Filesystem Access has been implemented in ARCtrl since v2.3.0, even transpilable to JS and Python. ARCtrl.NET is therefore deprecated and this repository hosts only ARCtrl.Querymodel.


<a href="https://www.nuget.org/packages/ARCtrl.Querymodel/"><img alt="Nuget" src="https://img.shields.io/nuget/v/ARCtrl.Querymodel?logo=nuget&color=%234fb3d9"></a>

Adds querying functionality to the core [ARCtrl](https://github.com/nfdi4plants/ARCtrl) package in .NET.

The documentation for the actual functions for manipulating the ARC datamodel can be found [here](https://github.com/nfdi4plants/ARCtrl/tree/main/docs/scripts_fsharp).

## Usage

```fsharp
open ARCtrl
open ARCtrl.QueryModel
open ARCtrl.ISA

let i = ArcInvestigation("Dummy Investigation")

i.ArcTables.Values().WithName("Dummy Header").First.ValueText

i.GetAssay("Dummy Assay").LastSamples
```

## Development

#### Requirements

- [.NET SDK](https://dotnet.microsoft.com/en-us/download)
    - verify with `dotnet --version` (Tested with 7.0.306)

#### Local Setup

- Setup dotnet tools `dotnet tool restore`

- Verify correct setup with  `./build.cmd runtests` ✨