module arcIO.NET.Tests

open Expecto

[<EntryPoint>]
let main argv =

    //Regex Test
    Tests.runTestsWithCLIArgs [Tests.CLIArguments.Sequenced] argv AssayTests.testComponentCasting |> ignore

    0