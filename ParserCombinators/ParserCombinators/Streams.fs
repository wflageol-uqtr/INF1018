module Streams

type Stream = { Source: seq<char>
                Index: int }

let stream source = { Source = source; Index = 0}

let peek stream = 
    let rest = Seq.skip stream.Index stream.Source
    if Seq.isEmpty rest
    then char 0
    else Seq.head rest

let next stream = peek stream, { stream with Index = stream.Index + 1 }