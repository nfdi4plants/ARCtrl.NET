# ARCtrl.NET

This library functions as an IO wrapper for the [ARCtrl](https://github.com/nfdi4plants/ARCtrl) library in .NET. 

The documentation for the actual functions for manipulating the ARC datamodel can be found [here](https://github.com/nfdi4plants/ARCtrl/tree/main/docs/scripts_fsharp).

## Usage

```fsharp
#r "nuget: ARCtrl.NET, 1.0.0-beta.2"

open ARCtrl.NET
open ARCtrl


let arcPath = ""

let arc = ARC.load(arcPath)

let isa = arc.ISA.Value

isa.InitStudy("MyStudy")

arc.Write(arcPath)
```

## Development

`./build.cmd runtests`

## ARCtrl.Querymodel

```fsharp
open ARCtrl
open ARCtrl.QueryModel
open ARCtrl.ISA

let i = ArcInvestigation("Dummy Investigation")

i.ArcTables.Values().WithName("Dummy Header").First.ValueText

i.GetAssay("Dummy Assay").LastSamples
```