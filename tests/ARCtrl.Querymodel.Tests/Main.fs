﻿module arcIO.NET.Tests

open Expecto


let all = testSequenced <| testList "All" [
        TestARC.Tests.main     
    ]

[<EntryPoint>]
let main argv =

    Tests.runTestsWithCLIArgs [] argv all
