open System

open Streams
open Combinators

type Token = Lettre of char
           | Chiffre of int

let word string =
    Seq.map (fun e -> chars [e]) string
    |> Seq.reduce bind

let digit = chars ['0'..'9']

let integer = digit >>= (many digit)


let str = stream "1234"

integer str |> Console.WriteLine