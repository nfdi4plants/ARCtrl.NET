namespace arcIO.NET

open System.IO
open ISADotNet

module Arc =

    let subFolderPaths = [".arc";Study.rootFolderName;Assay.rootFolderName;Workflow.rootFolderName;Run.rootFolderName]

    /// Initializes the ARC-specific folder structure.
    let initFolders (arcPath) =        
        subFolderPaths
        |> List.iter (fun n ->
            let dp = Path.Combine(arcPath,n)
            let dir = Directory.CreateDirectory(dp)
            File.Create(Path.Combine(dir.FullName, ".gitkeep")).Close()
        )

    /// Initializes the ISA part of the arc (ARC-specific folder structure and ISA files) from an existing investgation object.
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

    /// Initializes the ARC-specific git repository.
    let initGit workDir (repositoryAddress : string option) (branch : string option) =

        let log = Logging.createLogger "ArcInitGitLog"

        log.Trace("Init Git repository")

        let branch = branch |> Option.defaultValue "main"

        try

            GitHelper.executeGitCommand workDir $"init -b {branch}"
            //GitHelper.executeGitCommand workDir $"add ."
            //GitHelper.executeGitCommand workDir $"commit -m \"Initial commit\""

            log.Trace("Add remote repository")
            match repositoryAddress with
            | None -> ()
            | Some remote ->
                GitHelper.executeGitCommand workDir $"remote add origin {remote}"
                //GitHelper.executeGitCommand workDir $"branch -u origin/{branch} {branch}"

        with 
        | e -> 

            log.Error($"Git could not be set up. Please try installing Git cli and run `arc git init`.\n\t{e}")

    /// Initializes the ARC-specific folder structure, investigation file and git repository.
    let init (workDir : string) (identifier : string) (repositoryAddress : string option) (branch : string option) =

        let log = Logging.createLogger "ArcInitLog"
        
        log.Info("Start Arc Init")

        log.Trace("Create Directory")

        Directory.CreateDirectory workDir |> ignore

        log.Trace("Initiate folder structure")
        
        initFolders workDir

        log.Trace("Initiate investigation file")

        let inv = ISADotNet.Investigation.create(Identifier=identifier)
        Investigation.write workDir inv

        initGit workDir repositoryAddress branch
