module arcIO.NET.Tests

open Expecto


let all = testSequenced <| testList "All" [
        ArcTable.Tests.testStuff     
    ]

[<EntryPoint>]
let main argv =

    Tests.runTestsWithCLIArgs [] argv all
