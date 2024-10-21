module TestTasks

open BlackFox.Fake
open Fake.DotNet
open Fake.IO
open Fake.IO.Globbing.Operators

open ProjectInfo
open BasicTasks

let runTestClean = BuildTask.create "CleanTestResults" [clean] {
    !! "tests/**/TestResults"
    |> Shell.cleanDirs
}

let runTests = BuildTask.create "RunTests" [clean; runTestClean; build] {
    printfn $"Testprojectcount: {Seq.length testProjects}"
    testProjects
    |> Seq.iter (fun testProject ->
        Fake.DotNet.DotNet.test(fun testParams ->
            let msBuildParams =
                {testParams.MSBuildParams with 
                    DisableInternalBinLog = true
            }
            {
                testParams with
                    Logger = Some "console;verbosity=detailed"
                    Configuration = DotNet.BuildConfiguration.fromString configuration
                    NoBuild = true
                    MSBuildParams = msBuildParams
            }
        ) testProject
    )
}

    //|> DotNet.build (fun p ->
    //    let msBuildParams =
    //        {p.MSBuildParams with 
    //            DisableInternalBinLog = true
    //        }
    //    {
    //        p with 
    //            MSBuildParams = msBuildParams
    //    }
    //    |> DotNet.Options.withCustomParams (Some "-tl")
    //)

// to do: use this once we have actual tests
let runTestsWithCodeCov = BuildTask.create "RunTestsWithCodeCov" [clean; runTestClean; build] {
    let standardParams = Fake.DotNet.MSBuild.CliArguments.Create ()
    testProjects
    |> Seq.iter(fun testProject -> 
        Fake.DotNet.DotNet.test(fun testParams ->
            {
                testParams with
                    MSBuildParams = {
                        standardParams with
                            Properties = [
                                "AltCover","true"
                                "AltCoverCobertura","../../codeCov.xml"
                                "AltCoverForce","true"
                            ]
                    };
                    Logger = Some "console;verbosity=detailed"
            }
        ) testProject
    )
}