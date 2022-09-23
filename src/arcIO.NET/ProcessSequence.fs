namespace arcIO.NET

open ISADotNet
open ISADotNet.XLSX
open System.IO


module ProcessSequence =

    let updateByRef (ref : Process list) (ps : Process list) =
        AssayFile.AnnotationTable.updateSamplesByReference ref ps 
        |> Seq.toList

    let updateByItself (ps : Process list) =
        AssayFile.AnnotationTable.updateSamplesByThemselves ps 
        |> Seq.toList
