namespace arcIO.NET

open ISADotNet
open ISADotNet.XLSX
open System.IO

module Investigation =

    let investigationFileName = "isa.investigation.xlsx"

    /// Creates an investigation file in the ARC from the given investigation metadata contained in cliArgs that contains no studies or assays.
    let write (arc : string) (investigation : ISADotNet.Investigation) =
           
        let log = Logging.createLogger "InvestigationCreateLog"
        
        log.Info("Start Investigation Create")

        if System.IO.File.Exists(Path.Combine(arc,investigationFileName)) then
            log.Error("Investigation file does already exist.")

        else 
            let investigationFilePath = Path.Combine(arc,investigationFileName)    
            Investigation.toFile investigationFilePath investigation

    let fromArcFolder (arc : string) =
        let log = Logging.createLogger "InvestigationFromArcFolderLog"

        // read investigation from investigation file
        let ip = Path.Combine(arc,investigationFileName).Replace(@"\","/")
        let i = Investigation.fromFile ip

        // get study list from study files and assay files
        let istudies = 
            i.Studies
            |> Option.map (List.map (fun study -> 
                // read study from file
                match study.Identifier with
                | Some id ->
                    let studyFromFile = Study.readByIdentifier arc id
                    // update study assays and contacts with information from assay files
                    match study.Assays with
                    | Some assays ->
                        let scontacts,sassays = 
                            assays
                            |> List.fold (fun (cl,al) assay ->
                                match assay.FileName with
                                | Some fn ->
                                    let contactsFromFile,assayFromFile = Assay.readByFileName arc assay.FileName.Value
                                    cl @ contactsFromFile, al @ [assayFromFile]
                                | None ->
                                    log.Warn("Study \'" + id + "\' contains Assay without filename.")
                                    cl, al @ [assay]
                            ) (studyFromFile.Contacts |> Option.defaultValue [],[])
                        {studyFromFile with                        
                            Contacts = Some (scontacts |> List.distinct)
                            Assays = Some sassays 
                        }
                    | None -> 
                        studyFromFile
                | None ->
                    log.Warn("Investigation file contains study without identifier.")
                    study
            ))
        
        // construct complete process list from studies and assays, then update by itself
        let iprocesses = 
            istudies
            |> Option.map (List.fold (fun pl study ->
                let sprocesses = study.ProcessSequence |> Option.defaultValue []
                let aprocesses =
                    study.Assays
                    |> Option.map (List.fold (fun spl assay ->
                        let ap = assay.ProcessSequence |> Option.defaultValue []
                        spl @ ap
                    ) [] )
                    |> Option.defaultValue []
                pl @ sprocesses @ aprocesses
            ) [] )
            |> Option.defaultValue []
        let ref = iprocesses |> ProcessSequence.updateByItself

        // update investigation processes
        let istudies' =
            istudies
            |> Option.map (List.map (fun study ->
                {study with
                    Assays = study.Assays |> Option.map (List.map (fun a -> {a with ProcessSequence = a.ProcessSequence |> Option.map (ProcessSequence.updateByRef ref)}))
                    ProcessSequence = study.ProcessSequence |> Option.map (ProcessSequence.updateByRef ref)
                }
            ))

        // fill investigation with information from study files and assay files
        {i with Studies = istudies'}