module ArcTable.Tests

open Expecto
open System.Text.Json

[<Tests>]
let testStuff =

    testList "ArcTable" [
        testCase "WillNotFail" (fun () -> 
            Expect.isTrue true "Test if the test will test."
            
        )
    ]