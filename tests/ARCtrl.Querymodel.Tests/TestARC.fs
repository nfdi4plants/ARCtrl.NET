﻿module TestARC.Tests

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
    testList "ArcTable_Values" [
        testCase "ValuesOf_SpecificTable" (fun () ->
            let nodeName = "sampleOutHeat.txt"
            let protocolName =  "MS"            
            let values = isa.ArcTables.ValuesOf(nodeName,protocolName)
            let expected = 
                [
                    ISAValue.Parameter (
                        ProcessParameterValue.create(
                            ProtocolParameter.fromString("technical replicate","MS","MS:1001808"), 
                            Value.Ontology (OntologyAnnotation.fromString("1"))
                        )
                    )
                    ISAValue.Parameter (
                        ProcessParameterValue.create(
                            ProtocolParameter.fromString("injection volume setting","AFR","AFR:0001577"), 
                            Value.Int 20,
                            OntologyAnnotation.fromString("microliter","UO","http://purl.obolibrary.org/obo/UO_0000101")
                        )
                    )
                ]
            Expect.sequenceEqual values expected "Did not return correct values for specific table"
        )
        testCase "ValuesOf" (fun () ->
            let nodeName = "sampleOutHeat.txt"

            let valueHeaders = 
                isa.ArcTables.ValuesOf(nodeName)
                |> Seq.map (fun x -> x.NameText)
            let expected = 
                ["biological replicate";"organism";"temperature day";"pH";"technical replicate"; "injection volume setting";"analysis software"]
            Expect.sequenceEqual valueHeaders expected "Did not return correct values for all table"
        )
        testCase "GetSpecificValue" (fun () ->
            let rep1 = isa.ArcTables.ValuesOf("C1_measured").WithName("biological replicate").First.ValueText
            Expect.equal rep1 "1" "Did not return correct value for specific table"
            let rep2 = isa.ArcTables.ValuesOf("C2_measured").WithName("biological replicate").First.ValueText
            Expect.equal rep2 "2" "Did not return correct value for specific table"
        )
    ]





[<Tests>]
let main = testList "TestArcTests" [
    ArcTables_getNodes
    Assay_getNodes
    ArcTables_ValueOf
]