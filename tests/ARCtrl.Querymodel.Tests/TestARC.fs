﻿module TestARC.Tests

open Expecto
open System.Text.Json
open ARCtrl
open ARCtrl.NET
open ARCtrl.Process
open ARCtrl.QueryModel
let testArcPath = __SOURCE_DIRECTORY__ + @"\TestObjects\TestArc"
let testArc = ARC.load(testArcPath)

open ARCtrl.QueryModel.ArcInvestigationExtensions


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
        //testCase "RawData" (fun () ->
        //    let nodes = isa.ArcTables.RawData
        //    let nodeNames = nodes |> Seq.map (fun n -> n.Name)        
        //    let expected = ["CC1_measured";"CC2_measured";"CC3_measured";"Co1_measured";"Co2_measured";"Co3_measured";"C1_measured";"C2_measured";"C3_measured";"H1_measured";"H2_measured";"H3_measured"]
        //    Expect.sequenceEqual nodeNames expected "RawData of full sequence"    
        //)
        //testCase "LastRawData" (fun () ->
        //    let nodes = isa.ArcTables.LastRawData
        //    let nodeNames = nodes |> Seq.map (fun n -> n.Name)        
        //    let expected = ["CC1_measured";"CC2_measured";"CC3_measured";"Co1_measured";"Co2_measured";"Co3_measured";"C1_measured";"C2_measured";"C3_measured";"H1_measured";"H2_measured";"H3_measured"]
        //    Expect.sequenceEqual nodeNames expected "RawData of full sequence"    
        //)
        //testCase "FirstRawData" (fun () ->
        //    let nodes = isa.ArcTables.FirstRawData
        //    let nodeNames = nodes |> Seq.map (fun n -> n.Name)        
        //    let expected = ["CC1_measured";"CC2_measured";"CC3_measured";"Co1_measured";"Co2_measured";"Co3_measured";"C1_measured";"C2_measured";"C3_measured";"H1_measured";"H2_measured";"H3_measured"]
        //    Expect.sequenceEqual nodeNames expected "RawData of full sequence"    
        //)
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
let Assay_ValuesOf =
    let isa = testArc.ISA.Value
    testList "Assay_ValuesOf" [
        
        testCase "ValuesOfOutput_PooledOutput" (fun () ->
            let values = isa.GetAssay("MSEval_Heat").ValuesOf("sampleOutHeat.txt").WithName("Column")
            let valueValues = values |> Seq.map (fun n -> n.ValueText)
            let expected = ["C1 Intensity";"C2 Intensity";"C3 Intensity";"H1 Intensity";"H2 Intensity";"H3 Intensity"]
            Expect.sequenceEqual valueValues expected "Did not return all values correctly"    
        )
        testCase "SucceedingValuesOfInput_PooledOutput" (fun () ->
            let values = isa.GetAssay("MSEval_Heat").SucceedingValuesOf("C2_measured").WithName("Column")
            let valueValues = values |> Seq.map (fun n -> n.ValueText)
            let expected = ["C2 Intensity"]
            Expect.sequenceEqual valueValues expected "Did not return the single value correctly"    
        )
        testCase "PreviousValuesOfInput_PooledOutput" (fun () ->
            let values = isa.GetAssay("MSEval_Heat").PreviousValuesOf("C2_measured").WithName("Column")
            let valueValues = values |> Seq.map (fun n -> n.ValueText)
            let expected = []
            Expect.sequenceEqual valueValues expected "Should return no values"    
        )
        testCase "ValuesOfInput_PooledOutput" (fun () ->
            let values = isa.GetAssay("MSEval_Heat").ValuesOf("C2_measured").WithName("Column")
            let valueValues = values |> Seq.map (fun n -> n.ValueText)
            let expected = ["C2 Intensity"]
            Expect.sequenceEqual valueValues expected "Did not return the single value correctly"    
        )

    ]

let ArcTables_ValueOf =
    let isa = testArc.ISA.Value
    testList "ArcTable_Values" [
        testCase "ValuesOf_SpecificTable" (fun () ->
            let nodeName = "sampleOutHeat.txt"
            let protocolName =  "MS_Heat"            
            let values = isa.ArcTables.ValuesOf(nodeName,protocolName)
            let expectedTechRep =
                ISAValue.Parameter (
                        ProcessParameterValue.create(
                            ProtocolParameter.fromString("technical replicate","MS","MS:1001808"), 
                            Value.Ontology (OntologyAnnotation("1"))
                        )
                    )
            let expectedInjVol =
                ISAValue.Parameter (
                        ProcessParameterValue.create(
                            ProtocolParameter.fromString("injection volume setting","AFR","AFR:0001577"), 
                            Value.Int 20,
                            OntologyAnnotation("microliter","UO","http://purl.obolibrary.org/obo/UO_0000101")
                        )
                    )
            let expected = 
                [
                    expectedTechRep;expectedInjVol
                    expectedTechRep;expectedInjVol
                    expectedTechRep;expectedInjVol
                    expectedTechRep;expectedInjVol
                    expectedTechRep;expectedInjVol
                    expectedTechRep;expectedInjVol
                ]
            Expect.sequenceEqual values expected "Did not return correct values for specific table"
        )
        testCase "ValuesOf" (fun () ->
            let nodeName = "sampleOutHeat.txt"

            let valueHeaders = 
                isa.ArcTables.ValuesOf(nodeName).DistinctHeaderCategories()
                |> Seq.map (fun x -> x.NameText)
            let expected = 
                ["biological replicate";"organism";"temperature day";"pH";"technical replicate"; "injection volume setting";"analysis software";"Column"]
            Expect.sequenceEqual valueHeaders expected "Did not return correct values for all table"
        )
        testCase "GetSpecificValue" (fun () ->
            let rep1 = isa.ArcTables.ValuesOf("C1_measured").WithName("biological replicate").First.ValueText
            Expect.equal rep1 "1" "Did not return correct value for specific table"
            let rep2 = isa.ArcTables.ValuesOf("C2_measured").WithName("biological replicate").First.ValueText
            Expect.equal rep2 "2" "Did not return correct value for specific table"
        )
        testCase "ValuesOf_SpecificTable_PooledOutput" (fun () ->
            let vals = isa.ArcTables.ValuesOf("sampleOutHeat.txt","Growth_Heat").WithName("biological replicate").Values |> List.map (fun v -> v.ValueText)         
            Expect.sequenceEqual vals ["1";"2";"3";"1";"2";"3"] "Did not return correct values"
        )
        testCase "SpecificValue_SpecificTable_PooledOutput" (fun () ->
            let vals = isa.ArcTables.ValuesOf("C2_prep","Growth_Heat").WithName("biological replicate").First.ValueText
            Expect.equal vals "2" "Did not return correct value"
        )
    ]





[<Tests>]
let main = testList "TestArcTests" [
    ArcTables_getNodes
    Assay_getNodes
    Assay_ValuesOf
    ArcTables_ValueOf
]