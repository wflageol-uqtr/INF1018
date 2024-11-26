module Streams

type Stream<'a> = { Source: seq<'a>
                    Index: int }

let stream source = { Source = source; Index = 0}

// Retourne le caractère actuel du stream.
let peek (stream: Stream<'a>) : 'a option =
    let rest = Seq.skip stream.Index stream.Source
    if Seq.isEmpty rest
    then None
    else Seq.head rest |> Some

// Stream<'a> -> 'a option * Stream<'a>
// Retourne le caractère actuel du stream et avance le curseur.
let next (stream: Stream<'a>) : 'a option * Stream<'a> = 
    peek stream, { stream with Index = stream.Index + 1 }
