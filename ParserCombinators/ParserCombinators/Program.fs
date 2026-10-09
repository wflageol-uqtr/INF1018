open System
open Streams
open Combinators

let chars string = List.map one string
let lettre = chars (List.append ['A'..'Z'] ['a'..'z'])
             |> List.reduce either
let chiffre = chars ['0'..'9'] |> List.reduce either
let chiffres = chiffre >>= many chiffre
let id = lettre >>= many (either lettre chiffre)
let nonzero = chars ['1'..'9'] |> List.reduce either
let entier = either (optional (one '-') >>= nonzero >>= many chiffre)
                    (optional (one '-') >>= one '0')
let nombre = entier >>= optional (one '.' >>= chiffres >>= optional (one 'E' >>= entier))

let parser = nombre

let stream = stream "1.0E2"

parser stream
|> Console.WriteLine