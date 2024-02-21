namespace ARCtrl.QueryModel

open ARCtrl.ISA
open System.Text.Json.Serialization
open System.Text.Json
open System.IO

open System.Collections.Generic
open System.Collections

[<AutoOpen>]
module ArcInvestigationExtensions = 

    /// Queryable representation of an ISA Investigation. Implements the QProcessSequence interface
    type ArcInvestigation with

        /// Returns the QStudy with the given name
        member this.ArcTables
            with get() : ArcTables = 
                seq {
                    for s in this.Studies do yield! s.Tables
                    for a in this.Assays do yield! a.Tables
                }
                |> ResizeArray
                |> ArcTables

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
