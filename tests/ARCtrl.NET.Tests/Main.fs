module arcIO.NET.Tests

open Expecto


let all = testSequenced <| testList "All" [
        Path.Tests.main
    ]

[<EntryPoint>]
let main argv =

    Tests.runTestsWithCLIArgs [] argv all
