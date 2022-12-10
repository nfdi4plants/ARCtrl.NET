namespace arcIO.NET

open ISADotNet
open ISADotNet.XLSX
open System.IO

module Investigation =

    let investigationFileName = "isa.investigation.xlsx"

    let fromArcFolder (arc : string) =
        // read investigation from investigation file
        let ip = Path.Combine(arc,investigationFileName)
        let i = Investigation.fromFile ip

        // get study list from study files and assay files
        let istudies = 
            i.Studies
            |> Option.map (List.map (fun study -> 
                // read study from file
                let studyFromFile = Study.readByIdentifier arc study.Identifier.Value
                // update study assays and contacts with information from assay files
                match study.Assays with
                | Some assays ->
                    let scontacts,sassays = 
                        assays
                        |> List.fold (fun (cl,al) assay ->
                            let contactsFromFile,assayFromFile = Assay.readByFileName arc assay.FileName.Value
                            cl @ contactsFromFile, al @ [assayFromFile]
                        ) (studyFromFile.Contacts |> Option.defaultValue [],[])
                    {studyFromFile with                        
                        Contacts = Some scontacts
                        Assays = Some sassays 
                    }
                | None -> 
                    studyFromFile
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

    let write (arc : string) (investigation : Investigation) =
        let p = Path.Combine(arc,investigationFileName)
        ISADotNet.XLSX.Investigation.toFile p investigation