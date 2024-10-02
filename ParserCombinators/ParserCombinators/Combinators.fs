module Combinators

open Streams

type Result<'a> = Ok of seq<'a> * Stream
                | Error

// Exécute le prédicat sur le caractère actuel du stream, 
let one predicate stream =
    let element, newStream = next stream
    if predicate element
    then Ok (List.toSeq [element], newStream)
    else Error

let bind p1 p2 stream =
    match p1 stream with
    | Ok (result, stream) -> 
        match p2 stream with
        | Ok (result2, stream) -> Ok (Seq.append result result2, stream)
        | Error -> Error
    | Error -> Error

/// Applies a parser as many times as possible. This cannot return Error.
let many p stream =
    let rec doMany acc stream =
        match p stream with
        | Ok (r, s) -> doMany (Seq.append acc r) s
        | Error -> Ok (acc, stream)
    doMany [] stream

let (>>=) = bind

let chars list stream = one (fun c -> List.contains c list) stream