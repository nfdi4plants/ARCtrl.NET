// #r "nuget: ARCtrl.NET, 1.0.0-beta.2"
#r @"bin\Debug\net6.0\ARCtrl.dll"
#r @"bin\Debug\net6.0\ARCtrl.NET.dll"
#r @"bin\Debug\net6.0\ARCtrl.CWL.dll"
#r @"bin\Debug\net6.0\ARCtrl.Contract.dll"
#r @"bin\Debug\net6.0\ARCtrl.FileSystem.dll"
#r @"bin\Debug\net6.0\ARCtrl.ISA.dll"
#r @"bin\Debug\net6.0\ARCtrl.ISA.Json.dll"
#r @"bin\Debug\net6.0\ARCtrl.ISA.Spreadsheet.dll"
#r @"bin\Debug\net6.0\ARCtrl.Querymodel.dll"
#r @"bin\Debug\net6.0\Fable.Core.dll"


open ARCtrl
open ARCtrl.NET
open ARCtrl.QueryModel
open ARCtrl.ISA
let testArcPath = __SOURCE_DIRECTORY__ + @"\TestObjects\TestArc"
let testArc = ARC.load(testArcPath)

let i = testArc.ISA.Value
i.AssayCount
i.StudyCount
i.ArcTables.TableCount


i.AssayIdentifiers
let lastsamples = i.GetAssay("SamplePreparation_Cold").LastSamples
lastsamples
|> List.map (fun x -> x.Name)

