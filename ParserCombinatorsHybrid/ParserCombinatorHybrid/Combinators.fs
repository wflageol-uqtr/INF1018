module Combinators

open Streams

type Result<'a, 'b> = Ok of seq<'b> * Stream<'a>
                    | Error
type Parser<'a, 'b> = Stream<'a> -> Result<'a, 'b>

// Exécute le prédicat sur le caractère actuel du stream.
let one (predicate: 'a -> bool) : Parser<'a, 'a> =
    fun stream ->
        let element, newStream = next stream
        if element.IsSome && predicate element.Value
        then Ok (List.toSeq [element.Value], newStream)
        else Error

// Combine séquentiellement deux parsers en un seul. Les résulats sont concaténés.
let bind (p1: Parser<'a, 'b>) (p2: Parser<'a, 'b>) : Parser<'a, 'b> =
    fun stream ->
        match p1 stream with
        | Ok (result, stream) -> 
            match p2 stream with
            | Ok (result2, stream) -> Ok (Seq.append result result2, stream)
            | Error -> Error
        | Error -> Error

// Opérateur infix pour bind.
let (>>=) = bind

// Crée un parser qui applique le parser spécifié autant de fois que possible. 
// Le parser résultant ne peut pas retourner Error.
// Il retourne un Ok vide si le parser spécifié ne s'applique pas.
let many (p: Parser<'a, 'b>) : Parser<'a, 'b> =
    fun stream -> 
        let rec doMany acc stream =
            match p stream with
            | Ok (r, s) -> doMany (Seq.append acc r) s
            | Error -> Ok (acc, stream)
        doMany [] stream

// Combine une liste de parsers en un seul parser qui appliquera le premier parser valide de la liste.
// Le parser résultant retournera Error si aucun des parsers de sa liste ne s'applique. 
let either (ps: Parser<'a, 'b> list) : Parser<'a, 'b> =
    fun stream -> 
        let rec doEither rest =
            match rest with
            | (first::rest) -> match first stream with
                                | Error -> doEither rest 
                                | ok -> ok
            | [] -> Error
        doEither ps            

// Crée un parser qui appliquera la transformation fn au résultat du parser spécifié.
let convert (fn: 'b seq -> 'c seq) (p: Parser<'a, 'b>) : Parser<'a, 'c> =
    fun stream -> 
        match p stream with
        | Ok (r, s) -> Ok(fn r, s)
        | Error -> Error

// Crée un parser qui ignore le résultat du parser spécifié.
let skip (p: Parser<'a, 'b>) : Parser<'a, 'c> =
    fun stream ->
        match p stream with
        | Ok (r, s) -> Ok ([], s)
        | Error -> Error

// Extrait du type Result la valeur obtenue. 
// Lance une exception si le Result est une Error.
let unwrap (result: Result<'a, 'b>) : 'b seq =
    match result with
    | Ok (r, s) ->
        if s.Index < (Seq.length s.Source)
        then failwith "Input stream was not parsed completely."
        else r
    | Error -> failwith "An error occurred while parsing the stream."