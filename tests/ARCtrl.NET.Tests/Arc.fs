module Arc.Tests

open Expecto
open ARCtrl.NET
open System.Text.Json
open ARCtrl

let testLoad =

    testList "Load" [
        testCase "simpleARC" (fun () -> 
            let p = System.IO.Path.Combine(__SOURCE_DIRECTORY__,@"TestObjects\ARC_SimpleARC")
            let result = ARC.load(p)
            
            Expect.isSome result.ISA "Should contain an ISA part"
            Expect.isNone result.CWL "Should not contain a CWL part"

            let isa = result.ISA.Value
            Expect.equal isa.StudyCount 1 "Should contain 1 study"
            Expect.equal isa.AssayCount 1 "Should contain 1 assay"
            Expect.equal isa.RegisteredStudies.Count 1 "Should contain 1 registered study"
            
            let s = isa.Studies.[0]
            Expect.equal s.RegisteredAssayCount 1 "Should contain 1 registered assay"
            Expect.equal s.TableCount 3 "Study should contain 3 tables"

            let a = s.RegisteredAssays.[0]
            Expect.equal a.TableCount 4 "Assay should contain 4 tables"
            
        )
    ]

[<Tests>]
let main = 
    testList "ARC_Tests" [
        testLoad
    ]