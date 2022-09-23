namespace arcIO.NET

open ISADotNet
open ISADotNet.XLSX
open System.IO

module Investigation =

    let investigationFileName = "isa.investigation.xlsx"

    let fromArcFolder (arc : string) =
        let ip = Path.Combine(arc,investigationFileName)
        let i = Investigation.fromFile ip 
        match i.Studies with
        | Some ss -> 
            let ps,studies =
                ss
                |> List.fold (fun (ps,studies) study ->
                    let ps',study' = 
                        match study.Assays with
                        | Some assays ->
                            let contacts,ps' = 
                                assays
                                |> List.fold (fun (contacts,processSequence) a -> 
                                    let c,a = Assay.readByFileName arc a.FileName.Value               
                                    contacts @ c, processSequence @ (a.ProcessSequence |> Option.defaultValue [])
                                ) (study.Contacts |> Option.defaultValue [],study.ProcessSequence |> Option.defaultValue [])
                    
                            let study = 
                                {study with                        
                                    Contacts = Option.fromValueWithDefault [] (contacts |> List.distinct)
                                }
                            ps', study
                        | None -> 
                            study.ProcessSequence |> Option.defaultValue [],study
                    ps @ ps', studies @ [study']
                ) ([],[])
            let ref = ps |> ProcessSequence.updateByItself
            let studies' =
                studies
                |> List.map (fun study ->
                    {study with
                        Assays = study.Assays |> Option.map (List.map (fun a -> {a with ProcessSequence = a.ProcessSequence |> Option.map (ProcessSequence.updateByRef ref)}))
                        ProcessSequence = study.ProcessSequence |> Option.map (ProcessSequence.updateByRef ref)
                    }
                )
            {i with Studies = Some studies'}
        | None -> 
            i