#r "nuget: ARCtrl"
#r @"bin\Release\net6.0\ARCtrl.NET.dll"
#r @"bin\Release\net6.0\FSSpreadsheet.dll"
#r @"bin\Release\net6.0\ARCtrl.Contract.dll"
#r @"bin\Release\net6.0\ARCtrl.Querymodel.dll"
#r @"bin\Release\net6.0\Fable.Core.dll"
#r @"bin\Release\net6.0\OBO.NET.dll"


open ARCtrl
open ARCtrl.NET
open ARCtrl.QueryModel
open ARCtrl.ISA
let testArcPath = __SOURCE_DIRECTORY__ + @"\TestObjects\TestArc"
let testArc = ARC.load(testArcPath)

let i = testArc.ISA.Value

i.ArcTables.Data
|> Seq.map (fun x -> x.Name)


i.AssayCount
i.StudyCount

i.ArcTables.TableCount

i.ArcTables.TableNames

i.ArcTables.FirstRawData
|> List.map (fun x -> x.Name)

i.ArcTables.LastSamples
|> List.map (fun x -> x.Name)

i.ArcTables.FirstSamples
|> List.map (fun x -> x.Name)

i.ArcTables.LastRawData
|> List.map (fun x -> x.Name)

i.ArcTables.FirstProcessedData
|> List.map (fun x -> x.Name)

i.ArcTables.LastProcessedData
|> List.map (fun x -> x.Name)

i.ArcTables.Nodes
|> List.map (fun x -> x.Name)
|> List.length

i.ArcTables.LastSamples
|> List.map (fun x -> x.Name)

i.ArcTables.LastNodes
|> List.ofSeq
|> List.map (fun x -> x.Name)

let studies = i.StudyIdentifiers
let heatAssays = i.Studies[0].RegisteredAssayIdentifiers |> List.ofSeq 

let coldAssays = i.Studies[1].RegisteredAssayIdentifiers |> List.ofSeq

let lastsamplesOfFirstAssayH = i.GetAssay("SamplePreparation_Heat").LastSamples |> List.map (fun x -> x.Name)
let lastsamplesOfFirstAssayC = i.GetAssay("SamplePreparation_Cold").LastSamples |> List.map (fun x -> x.Name)

let lastNodesHeat = i.GetAssay("MSEval_Heat").LastNodes |> Seq.map (fun x -> x.Name) |> Array.ofSeq
let lastSamplesHeat = i.GetAssay("MSEval_Heat").LastSamples |> List.map (fun x -> x.Name) |> Array.ofSeq

let exampleLastSampleHeat = i.GetAssay("MSEval_Heat").LastSamples[0] 
let exampleLastSampleHeatName = i.GetAssay("MSEval_Heat").LastSamples[0].Name 

let prevNodes = exampleLastSampleHeat.Sources |> List.ofSeq |> List.map (fun x -> x.Name)

exampleLastSampleHeat.PreviousParameters
|> Seq.map (fun x -> x.Value)
|> List.ofSeq

//  i.GetAssay("MSEval_Heat") |> List.map (fun x -> x.Name) |> Array.ofSeq
let allValuesOfExampleAssayAndExampleNode = 
    i.GetAssay("MSEval_Heat").ValuesOf("sampleOutHeat.txt")
    // |> Seq.toList
    // |> List.map (fun x -> x.NameText)
allValuesOfExampleAssayAndExampleNode
|> Seq.map (fun x -> x.NameText)
|> List.ofSeq

exampleLastSampleHeat.Parameters
|> Seq.toList
|> List.map (fun x -> x.NameText)

i.GetAssay("MS_Heat").ValuesOf("sampleOutHeat.txt")
|> Seq.map (fun x -> x.NameText)
|> List.ofSeq

i.ArcTables.ValuesOf("sampleOutHeat.txt","MS")
|> Seq.toList
|> List.map (fun x -> x.NameText)

i.GetAssay("MS_Heat").Values("MS")
|> Seq.map (fun x -> x.NameText)
|> List.ofSeq
// let getBioRep (fN:QNode) = 
//     match qi.ValuesOf(fN,ProtocolName = "Growth").WithName("biological replicate").Values.Head with
//     | QueryModel.ISAValue.Parameter x -> x.Value.Value.AsString 
//     | _ -> failwith "no biorep please add"

exampleLastSampleHeat.ParentProcessSequence.Nodes
|> List.map (fun x -> x.Name)

exampleLastSampleHeat.Nodes
|> Seq.toList 
|> List.map (fun x -> x.Name)

// exampleLastSampleHeat.
// |> Seq.toList 
// |> List.map (fun x -> x.Name)
