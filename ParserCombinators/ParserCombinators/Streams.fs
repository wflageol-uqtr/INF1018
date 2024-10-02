module Streams

type Stream = { Source: seq<char>
                Index: int }

let stream source = { Source = source; Index = 0}

/// Retourne le caractère actuel du stream.
let peek stream = 
    let rest = Seq.skip stream.Index stream.Source
    if Seq.isEmpty rest
    then char 0
    else Seq.head rest

/// Retourne le caractère actuel du stream et avance le curseur.
let next stream = peek stream, { stream with Index = stream.Index + 1 }