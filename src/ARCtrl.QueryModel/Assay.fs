namespace ARCtrl.QueryModel

open ARCtrl.ISA
open System.Text.Json.Serialization
open System.Text.Json
open System.IO

open System.Collections.Generic
open System.Collection

[<AutoOpen>]
module ArcAssayExtensions = 

    /// Queryable representation of an ISA Assay. Implements the ArcTables interface
    type ArcAssay with 

        member this.FileName = FileName
        member this.MeasurementType = MeasurementType
        member this.TechnologyType = TechnologyType
        member this.TechnologyPlatform = TechnologyPlatform

        /// get the protocol or sheet (in ISATab logic) with the given name
        member this.Protocol (sheetName : string) =
            base.Protocol(sheetName, $"Assay \"{this.FileName}\"")

        /// get the nth protocol or sheet (in ISATab logic) 
        member this.Protocol (index : int) =
            base.Protocol(index, $"Assay \"{this.FileName}\"")

        /// Returns the initial inputs final outputs of the assay, to which no processPoints
        static member getRootInputs (assay : QAssay) = ArcTables.getRootInputs assay

        /// Returns the final outputs of the assay, which point to no further nodes
        static member getFinalOutputs (assay : QAssay) = ArcTables.getFinalOutputs assay

        /// Returns the initial inputs final outputs of the assay, to which no processPoints
        static member getRootInputOf (assay : QAssay) (sample : string) = ArcTables.getRootInputsOfBy (fun _ -> true) sample assay 
        
        /// Returns the final outputs of the assay, which point to no further nodes
        static member getFinalOutputsOf (assay : QAssay) (sample : string) = ArcTables.getFinalOutputsOfBy (fun _ -> true) sample assay
