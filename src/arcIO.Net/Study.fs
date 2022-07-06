namespace arcIO.Net

open ISADotNet
open ISADotNet.XLSX
open System.IO 


module Study = 

    let studyFileName = "isa.study.xlsx"

    let readFromFolder (arc : string) (folderPath : string) =
        let sp = Path.Combine (folderPath,studyFileName)
        let study = StudyFile.Study.fromFile sp
        match study.Assays with
        | Some assays ->
            let contacts,ps,assays = 
                assays
                |> List.fold (fun (contacts,processSequence,assays) a -> 
                    let c,a = Assay.readByFileName arc a.FileName.Value               
                    contacts @ c, processSequence @ (a.ProcessSequence |> Option.defaultValue []), assays @ [a]
                ) (study.Contacts |> Option.defaultValue [],study.ProcessSequence |> Option.defaultValue [],[])
            let ref = ps |> ProcessSequence.updateByItself
            let updatedAssays =
                assays
                |> List.map (fun a ->
                    {a with ProcessSequence = a.ProcessSequence |> Option.map (ProcessSequence.updateByRef ref)}
                )
            {study with 
                ProcessSequence = study.ProcessSequence |> Option.map (ProcessSequence.updateByRef ref)
                Assays = Some updatedAssays
                Contacts = Option.fromValueWithDefault [] (contacts |> List.distinct)
            }
        | None -> 
            {study with ProcessSequence = study.ProcessSequence |> Option.map ProcessSequence.updateByItself}
