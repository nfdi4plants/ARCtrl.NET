module ArcTable.Tests

open Expecto
open System.Text.Json
open ARCtrl
open ARCtrl.NET
open ARCtrl.QueryModel
open ARCtrl.ISA
let testArcPath = __SOURCE_DIRECTORY__ + @"\TestObjects\TestArc"
let testArc = ARC.load(testArcPath)
[<Tests>]
let testStuff =
    
    testList "ArcTable" [
        testCase "WillNotFail" (fun () -> 
            Expect.isTrue true "Test if the test will test."
            
        )
    ]