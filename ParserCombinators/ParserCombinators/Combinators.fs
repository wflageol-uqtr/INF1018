module Combinators

open Streams

type Result<'a> = Ok of seq<'a> * Stream
                | Error

/// Exécute le prédicat sur le caractère actuel du stream.
let one predicate stream =
    let element, newStream = next stream
    if predicate element
    then Ok (List.toSeq [element], newStream)
    else Error

/// Combine séquentiellement deux parsers en un seul. Les résulats sont concaténés.
let bind p1 p2 stream =
    match p1 stream with
    | Ok (result, stream) -> 
        match p2 stream with
        | Ok (result2, stream) -> Ok (Seq.append result result2, stream)
        | Error -> Error
    | Error -> Error

/// Opérateur infix pour bind.
let (>>=) = bind

/// Applique un parser autant de fois que possible. Ne peut pas retourner Error.
let many p stream =
    let rec doMany acc stream =
        match p stream with
        | Ok (r, s) -> doMany (Seq.append acc r) s
        | Error -> Ok (acc, stream)
    doMany [] stream

/// Vérifie que le caractère actual fait partie de la liste fournie.
let chars list stream = one (fun c -> List.contains c list) stream