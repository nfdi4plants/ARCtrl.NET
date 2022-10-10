namespace arcIO.NET

open System
open System.IO

module Arc =

    // TODO TO-DO TO DO: how about args??
    // do we need/want branch as input?
    /// Initializes the ARC-specific folder structure.
    let init (workDir : string) (identifier : string) (repositoryAddress : string option) =

        let log = Logging.createLogger "ArcInitLog"
        
        log.Info("Start Arc Init")

        // let gitLFSThreshold     = tryGetFieldValueByName "GitLFSByteThreshold"  arcArgs
        // let branch              = tryGetFieldValueByName "Branch"               arcArgs |> Option.defaultValue "main"
        // let repositoryAddress   = tryGetFieldValueByName "RepositoryAddress"    arcArgs 


        log.Trace("Create Directory")

        Directory.CreateDirectory workDir |> ignore

        log.Trace("Initiate folder structure")
        
        [|".arc";"studies";"assays";"workflows";"runs"|]
        |> Array.map (fun rf -> Path.Combine(workDir, rf))
        |> Array.iter (fun x ->
            Directory.CreateDirectory x
            |> fun dir -> File.Create(Path.Combine(dir.FullName, ".gitkeep")) |> ignore 
        )

        log.Trace("Initiate investigation file")

        let inv = ISADotNet.Investigation.create(Identifier=identifier)
        Investigation.create inv workDir

        log.Trace("Init Git repository")

        try

            GitHelper.executeGitCommand workDir $"init"
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

