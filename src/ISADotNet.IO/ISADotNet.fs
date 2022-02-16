namespace arcIO.NET

open ISADotNet
open ISADotNet.API

module ISADotNet =
    type SourceSinkPair = 
        {
            Protocol: string
            ProcessInput: string
            ProcessOutput: string
        }
    
        static member create protocol pi po = 
            {
                Protocol = protocol
                ProcessInput = pi
                ProcessOutput = po
            }

    let createInOutMap (assay: Assay) =
        assay.ProcessSequence.Value
        |> List.map (fun x -> 
            List.map2 ( fun i o ->
                let iNameOpt = (API.ProcessInput.getName i)
                let oNameOpt = (API.ProcessOutput.getName o)
                match iNameOpt, oNameOpt with
                | None, _ -> None
                | _, None -> None
                | Some iName, Some oName ->
                    Some (SourceSinkPair.create x.ExecutesProtocol.Value.Name.Value iName oName)
            ) x.Inputs.Value x.Outputs.Value
            |> List.choose id
        )
        |> List.concat
        |> List.map (fun ssp -> ssp.ProcessOutput,(ssp.ProcessInput,ssp.ProcessOutput,ssp.Protocol))
        |> List.groupBy fst
        |> List.map (fun (a,b) -> a, b |> List.map snd)
        |> Map.ofList

    let findSourcesOfOutputInProtocol (protocol: string) (outputName: string) (inOutMap: Map<string,(string*string*string)list>) =
        let rec loop out l =
            match l with
            | h::t ->
                match Map.tryFind h inOutMap with
                | Some ins ->
                    let ofProtocol = ins |> List.choose (fun (source,sample,prot) -> if prot = protocol then Some sample else None)
                    let notOfProtocol = ins |> List.choose (fun (source,sample,prot) -> if prot = protocol then None else Some source)
                    loop (List.append ofProtocol out) (notOfProtocol)
                | None -> loop out t
    
            | [] -> out
        loop [] [outputName]

    let tryRetrieveParameterValue (protocolName: string) (parameterName: string) (sampleName: string) (assay:Assay) =
        match assay.ProcessSequence with
        | Some ps -> 
            ProcessSequence.filterByProtocolName protocolName ps
            |> ProcessSequence.getOutputsWithParameterBy (fun (p:ProtocolParameter) -> ProtocolParameter.getNameAsStringWithNumber p = parameterName)
            |> List.tryPick (fun (o,p) -> 
                match ProcessOutput.getName o with
                | Some o when o = sampleName -> Some (ProcessParameterValue.getValueAsString p)
                | _ -> None
            )
        | None -> None

    let tryRetrieveCharacteristicValue (protocolName: string) (characteristicName: string) (sampleName: string) (assay:Assay) =
        match assay.ProcessSequence with
        | Some ps -> 
            ProcessSequence.filterByProtocolName protocolName ps
            |> ProcessSequence.getOutputsWithCharacteristicBy (fun (ma : MaterialAttribute) -> MaterialAttribute.getNameAsStringWithNumber ma = characteristicName)
            |> List.tryPick (fun (o,p) -> 
                match ProcessOutput.getName o with
                | Some o when o = sampleName -> Some (MaterialAttributeValue.getValueAsString p)
                | _ -> None
            )
        | None -> None

    let tryGetCharacteristic (inOutMap: Map<string,(string*string*string)list>) (protocolName: string) (characteristicName: string) (sampleName: string) (assay:Assay) =
        let sName =
            findSourcesOfOutputInProtocol protocolName sampleName inOutMap
            |> fun x -> 
                if x.Length <> 1 then printfn "Warning, more than one corresponding name found. Your naming is not unambiguous: found %i matching entries" x.Length
                x
            |> List.head
        tryRetrieveCharacteristicValue protocolName characteristicName sName assay

    let tryGetParameter (inOutMap: Map<string,(string*string*string)list>) (protocolName: string) (characteristicName: string) (sampleName: string) (assay:Assay) =
        let sName = 
            findSourcesOfOutputInProtocol protocolName sampleName inOutMap
            |> fun x -> 
                if x.Length <> 1 then printfn "Warning, more than one corresponding name found. Your naming is not unambiguous: found %i matching entries" x.Length
                x
            |> List.head
        tryRetrieveParameterValue protocolName characteristicName sName assay
