namespace ARCtrl.QueryModel

open ARCtrl.ISA
open System.Text.Json.Serialization
open System.Text.Json
open System.IO

open System.Collections.Generic
open System.Collections

[<AutoOpen>]
module ArcInvestigationExtensions = 

    ///// Queryable representation of an ISA Investigation. Implements the QProcessSequence interface
    //type ArcInvestigation with

    //    /// Returns the QStudy with the given name
    //    member this.Study(studyName : string) = 
    //        this.Studies
    //        |> List.find (fun s -> s.Identifier.Value = studyName)
        
    //    /// Returns the nth QStudy
    //    member this.Study(i : int) = 
    //        this.Studies
    //        |> List.item i 

    //    /// Returns the QAssay with the given name (registered in the study with the given study name)
    //    member this.Assay(assayName : string, ?StudyName : string) = 
    //        match StudyName with
    //        | Some sn ->
    //            this.Study(sn).Assay(assayName)
    //        | None ->
    //            this.Studies
    //            |> List.collect (fun s -> s.Assays)
    //            |> List.find (fun a -> a.FileName.Value.Contains assayName)

    //    /// get the protocol or sheet (in ISATab logic) with the given name
    //    member this.Protocol (sheetName : string) =
    //        base.Protocol(sheetName, $"Assay \"{this.FileName}\"")

    //    /// get the nth protocol or sheet (in ISATab logic) 
    //    member this.Protocol (index : int) =
    //        base.Protocol(index, $"Assay \"{this.FileName}\"")

    //    /// Returns the initial inputs final outputs of the assay, to which no processPoints
    //    static member getRootInputs (investigation : QInvestigation) = QProcessSequence.getRootInputs investigation

    //    /// Returns the final outputs of the investigation, which point to no further nodes
    //    static member getFinalOutputs (investigation : QInvestigation) = QProcessSequence.getFinalOutputs investigation

    //    /// Returns the initial inputs final outputs of the investigation, to which no processPoints
    //    static member getRootInputOf (investigation : QInvestigation) (sample : string) = QProcessSequence.getRootInputsOfBy (fun _ -> true) sample investigation 
        
    //    /// Returns the final outputs of the investigation, which point to no further nodes
    //    static member getFinalOutputsOf (investigation : QInvestigation) (sample : string) = QProcessSequence.getFinalOutputsOfBy (fun _ -> true) sample investigation

    //    static member toString (rwa : QInvestigation) =  JsonSerializer.Serialize<QInvestigation>(rwa,JsonExtensions.options)

    //    static member toFile (path : string) (rwa:QInvestigation) = 
    //        File.WriteAllText(path,QInvestigation.toString rwa)

    //    static member fromString (s:string) = 
    //        JsonSerializer.Deserialize<QInvestigation>(s,JsonExtensions.options)

    //    static member fromFile (path : string) = 
    //        File.ReadAllText path 
    //        |> QInvestigation.fromString

    module Investigation =

        open Errors

        //let fileName (i : ArcInvestigation) =
        //    match i.FileName with
        //    | Some v -> (v)
        //    | None -> raise InvestigationHasNoFileNameException
        let identifier (i : ArcInvestigation) =
            i.Identifier
        let title (i : ArcInvestigation) =
            match i.Title with
            | Some v -> (v)
            | None -> raise InvestigationHasNoTitleException
        let description (i : ArcInvestigation) =
            match i.Description with
            | Some v -> (v)
            | None -> raise InvestigationHasNoDescriptionException
        let submissionDate (i : ArcInvestigation) =
            match i.SubmissionDate with
            | Some v -> (v)
            | None -> raise InvestigationHasNoSubmissionDateException
        let publicReleaseDate (i : ArcInvestigation) =
            match i.PublicReleaseDate with
            | Some v -> (v)
            | None -> raise InvestigationHasNoPublicReleaseDateException
        let ontologySourceReferences (i : ArcInvestigation) =
            match i.OntologySourceReferences with
            | [||] -> raise InvestigationHasNoOntologySourceReferencesException
            | v -> v
        let publications (i : ArcInvestigation) =
            match i.Publications with
            | [||] -> raise InvestigationHasNoPublicationsException
            | v -> (v)
        let contacts (i : ArcInvestigation) =
            match i.Contacts with
            | [||] -> raise InvestigationHasNoContactsException
            | v -> (v)
