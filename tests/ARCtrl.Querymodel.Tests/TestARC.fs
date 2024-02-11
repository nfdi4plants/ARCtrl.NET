module TestARC.Tests

open Expecto
open System.Text.Json
open ARCtrl
open ARCtrl.NET
open ARCtrl.QueryModel
open ARCtrl.ISA
let testArcPath = __SOURCE_DIRECTORY__ + @"\TestObjects\TestArc"
let testArc = ARC.load(testArcPath)


let ArcTables_getNodes =
    let isa = testArc.ISA.Value
    testList "ARCTables_GetNodes" [
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

let Assay_getNodes =
    let isa = testArc.ISA.Value
    testList "Assay_GetNodes" [
        
        testCase "LastNodes" (fun () ->
            let nodes = isa.GetAssay("MSEval_Heat").LastNodes
            let nodeNames = nodes |> Seq.map (fun n -> n.Name)
            let expected = ["sampleOutHeat.txt"]
            Expect.sequenceEqual nodeNames expected "LastData of full sequence"    
        )
    ]


let ArcTables_ValueOf =
    let isa = testArc.ISA.Value
    testList "ArcTable_ValueOf" [
        testCase "Values" (fun () ->
            let nodeName = "sampleOutHeat.txt"
            let protocolName =  "MS"

            let values = 
                isa.ArcTables.ValuesOf(nodeName,protocolName)
                |> Seq.toList
                |> List.map (fun x -> x.NameText)
            Expect.isTrue false ""
        )
    ]





[<Tests>]
let main = testList "TestArcTests" [
    ArcTables_getNodes
    Assay_getNodes
    ArcTables_ValueOf
]