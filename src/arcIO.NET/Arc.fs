namespace arcIO.NET

open System.IO
open ISADotNet

module Arc =

    let initFolders (arcPath) =        
        let rootFolders =
            [
                ".arc";Study.rootFolderName;Assay.rootFolderName;Workflow.rootFolderName;Run.rootFolderName
            ]
        rootFolders
        |> List.iter (fun n ->
            let dp = Path.Combine(arcPath,n)
            let dir = Directory.CreateDirectory(dp)
            File.Create(Path.Combine(dir.FullName, ".gitkeep")).Close()
        )

    let importFromInvestigation (arcPath) (investigation : Investigation)= 
        initFolders arcPath
        Investigation.write arcPath investigation
        investigation.Studies
        |> Option.defaultValue []
        |> List.collect (fun s -> 
            Study.init arcPath s
            s.Assays
            |> Option.defaultValue []
            )
        |> List.iter (fun a ->
            Assay.init arcPath a
        )
