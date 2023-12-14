# ARCtrl.NET

> **ARCtrl.NET** is the .NET IO implementation of [ARCtrl](https://github.com/nfdi4plants/ARCtrl)
 
| Version | Downloads |
| :--------|-----------:|
|<a href="https://www.nuget.org/packages/ARCtrl.NET/"><img alt="Nuget" src="https://img.shields.io/nuget/vpre/ARCtrl.NET?logo=nuget&color=%234fb3d9"></a>|<a href="https://www.nuget.org/packages/ARCtrl/"><img alt="Nuget" src="https://img.shields.io/nuget/dt/ARCtrl?color=%234FB3D9"></a>|

```fsharp
#r "nuget: ARCtrl.NET, 1.0.0-beta.2"

open ARCtrl.NET
open ARCtrl

let arc = ARC.load(myArcPath)

// work work work

arc.Write(myArcPath)
```

For documentation on manipulationh of the datamodel, see https://github.com/nfdi4plants/ARCtrl/tree/main/docs
