module Combinators

open Streams

type Result<'a, 'b> =
    | Ok of seq<'a> * Stream<'b>
    | Error

type Parser<'a, 'b> = Stream<'a> -> Result<'a, 'b>

let one char : Parser<char, char> =
    fun stream ->
        let element, newStream = next stream
        match element with
        | None -> Error
        | Some c -> if c = char
                    then Ok (List.ofSeq [c], newStream)
                    else Error

let bind p1 p2 : Parser<char, char> =
    fun stream ->
        match p1 stream with
        | Ok (r1, newStream) -> match p2 newStream with
                                | Ok (r2, newStream) ->
                                    Ok (Seq.append r1 r2, newStream)
                                | Error -> Error
        | Error -> Error

let (>>=) = bind

let either p1 p2 : Parser<char, char> =
    fun stream ->
        let r1 = p1 stream
        match r1 with
        | Ok _ -> r1
        | Error -> p2 stream
        
let many p : Parser<char, char> =
    fun stream ->
        let rec doMany acc stream =
            match p stream with
            | Ok(r, newStream) -> doMany (Seq.append acc r) newStream
            | Error -> Ok (acc, stream)
        doMany [] stream

let optional p : Parser<char, char> =
    fun stream ->
        let r = p stream
        match r with
        | Ok _ -> r
        | Error -> Ok ([],stream)