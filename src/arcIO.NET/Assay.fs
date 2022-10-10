namespace arcIO.NET

open ISADotNet.XLSX
open ISADotNet
open System.IO

open FSharpSpreadsheetML

module Assay =

    let assayFileName = "isa.assay.xlsx"

    let readFromFolder (folderPath : string) =
        let ap = Path.Combine (folderPath,assayFileName)
        let c,a = AssayFile.Assay.fromFile ap
        c,a

    let readByFileName (arc : string) (assayFileName : string) =
        let ap = Path.Combine ([|arc;"assays";assayFileName|])
        let c,a = AssayFile.Assay.fromFile ap
        c,a

    let readByName (arc : string) (assayName : string) =
        Path.Combine ([|arc;"assays";assayName|])
        |> readFromFolder

    /// Initializes a new empty assay file and associated folder structure in the ARC.
    let init (name : string) =

        let log = Logging.createLogger "AssayInitLog"
        
        log.Info("Start Assay Init")
        
        let assayFilePath = Path.Combine([|"./assays/";name;assayFileName|])

        let assay = ISADotNet.Assay.create(FileName=Path.Combine(name,assayFileName))

        if System.IO.Directory.Exists (Path.Combine("./assays",name)) then
            log.Error($"Assay folder with identifier {name} already exists.")
        else
            [|"dataset";"protocols"|]
            |> Array.iter (
                Directory.CreateDirectory 
                >> fun dir -> File.Create(Path.Combine(dir.FullName, ".gitkeep")) |> ignore 
            )

            // ToDo: is this correct usage??
            ISADotNet.XLSX.AssayFile.Assay.init (Some assay) None name assayFilePath

            [|"README.md"|]
            |> Array.iter (File.Create >> ignore)

    /// Updates an existing assay file in the ARC with the given assay metadata contained in cliArgs.
    let update (assay : ISADotNet.Assay) (studyIdentifier : string) updateOption =
        
        let log = Logging.createLogger "AssayUpdateLog"

        log.Info("Start Assay Update")

        // ToDo: what does this do?
        //let updateOption = if containsFlag "ReplaceWithEmptyValues" assayArgs then API.Update.UpdateAll else API.Update.UpdateByExisting            

        let assayFilePath =
            match assay.FileName with
            | Some f ->
                Path.Combine("./assays",f)
            | None ->
                log.Error("Assay not valid, filename missing.")
                raise (InvalidAssay("No Filename"))

        let assayName =
            match assay.FileName with
            | Some f ->
                Path.GetDirectoryName(f)
            | None ->
                log.Error("Assay not valid, filename missing.")
                raise (InvalidAssay("No Filename"))

        // let studyIdentifier = 
        //     match getFieldValueByName "StudyIdentifier" assayArgs with
        //     | "" -> assayIdentifier
        //     | s -> 
        //         log.Trace("No Study Identifier given, use Assay Identifier instead.")
        //         s

        let investigationFilePath = "./isa.investigation.xlsx"
        
        let investigation = Investigation.fromFile investigationFilePath
        
        let doc = Spreadsheet.fromFile assayFilePath true

        // part that writes assay metadata into the assay file
        try 
            AssayFile.MetaData.overwriteWithAssayInfo "Assay" assay doc
            
        finally
            Spreadsheet.close doc

        // part that writes assay metadata into the investigation file
        match investigation.Studies with
        | Some studies -> 
            match API.Study.tryGetByIdentifier studyIdentifier studies with
            | Some study -> 
                match study.Assays with
                | Some assays -> 
                    if API.Assay.existsByFileName assayFileName assays then
                        API.Assay.updateByFileName updateOption assay assays
                        |> API.Study.setAssays study
                    else
                        let msg = $"Assay with the identifier {assayName} does not exist in the study with the identifier {studyIdentifier}."
                        log.Error($"{msg}")
                        log.Trace("AddIfMissing argument can be used to register assay with the update command if it is missing.")
                        study
                | None -> 
                    let msg = $"The study with the identifier {studyIdentifier} does not contain any assays."
                    log.Error($"{msg}")
                    log.Trace("AddIfMissing argument can be used to register assay with the update command if it is missing.")
                    study
                |> fun s -> API.Study.updateByIdentifier API.Update.UpdateAll s studies
                |> API.Investigation.setStudies investigation
            | None -> 
                log.Error($"Study with the identifier {studyIdentifier} does not exist in the investigation file.")
                investigation
        | None -> 
            log.Error("The investigation does not contain any studies.")
            investigation
        |> Investigation.toFile investigationFilePath
    
    /// Deletes an assay's folder and underlying file structure from the ARC.
    let delete (identifier : string) isForced =

        let log = Logging.createLogger "AssayDeleteLog"

        log.Info("Start Assay Delete")

        let assayFolderPath = Path.Combine([|"./assays";identifier;"isa.assay.xlsx"|])

        /// Standard files that should be always present in an assay.
        let standard = [|
            "isa.assay.xlsx"
            yield!
                [|"README.md"|]
            yield!
                [|"dataset";"protocols"|]
                |> Array.map (
                    fun p -> Path.Combine(p, ".gitkeep")
                )
        |]

        /// Actual files found.
        let allFiles =
            Directory.GetFiles(assayFolderPath, "*", SearchOption.AllDirectories)
            |> Array.map (fun p -> Path.GetRelativePath(identifier,p))

        /// A check if there are no files in the folder that are not standard.
        let isStandard = Array.forall (fun t -> Array.contains t standard) allFiles

        match isForced, isStandard with
        | true, _
        | false, true ->
            try Directory.Delete(assayFolderPath, true) with
            | err -> log.Error($"Cannot delete assay:\n {err.ToString()}")
        | _ ->
            log.Error "Assay contains user-specific files. Deletion aborted."
            log.Info "Run the command with `--force` to force deletion."