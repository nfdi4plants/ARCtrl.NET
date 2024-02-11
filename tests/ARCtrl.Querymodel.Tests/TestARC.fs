module TestARC.Tests

open Expecto
open System.Text.Json
open ARCtrl
open ARCtrl.NET
open ARCtrl.QueryModel
open ARCtrl.ISA
let testArcPath = __SOURCE_DIRECTORY__ + @"\TestObjects\TestArc"
let testArc = ARC.load(testArcPath)


let getNodes =
    let isa = testArc.ISA.Value
    testList "GetNodes" [
        testCase "LastData" (fun () -> 
            let nodes = isa.ArcTables.LastData
            let nodeNames = nodes |> List.map (fun n -> n.Name)
            let expected = ["sampleOutCold.txt"; "sampleOutHeat.txt"]
            Expect.sequenceEqual nodeNames expected "LastData of full sequence"            
        )
        testCase "LastSamples" (fun () ->
            let nodes = isa.ArcTables.LastSamples
            let nodeNames = nodes |> List.map (fun n -> n.Name)
            let expected = ["CC1_prep"; "CC2_prep"; "CC3_prep"; "Co1_prep"; "Co2_prep"; "Co3_prep"; "C1_prep"; "C2_prep"; "C3_prep"; "H1_prep"; "H2_prep"; "H3_prep"]
            Expect.sequenceEqual expected nodeNames "LastSamples of full sequence"                 
        )
        testCase "LastNodes" (fun () ->
            let nodes = isa.ArcTables.LastNodes
            let nodeNames = nodes |> Seq.map (fun n -> n.Name)
            let expected = ["sampleOutCold.txt"; "sampleOutHeat.txt"]
            Expect.sequenceEqual nodeNames expected "LastData of full sequence"    
        )
    ]



[<Tests>]
let main = testList "TestArcTests" [
    getNodes
]