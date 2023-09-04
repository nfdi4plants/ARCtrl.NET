module Contract.Tests

open Expecto
open ARCtrl.Path
open System.Text.Json


let testRead =

    testList "Read" [
        testCase "Implement" (fun () -> 
            Expect.isTrue false "ImplementTest"           
        )
    ]


let testWrite =

    testList "Write" [
        testCase "Implement" (fun () -> 
            Expect.isTrue false "ImplementTest"           
        )
    ]

[<Tests>]
let main = 
    testList "ContractTests" [
        //testRead
        //testWrite
    ]
