namespace INF1018_Devoir_2

open System
open System.Collections.Generic
open Streams
open Combinators

(* 
Construisez un automate d'analyse lexicale pour cet alphabet
d'un langage d'arithmétique simple :

<var> := A | B | C
<int> := Séquence de chiffre représentant un entier valide
<op> := + | - | * | /

Basez votre automate sur l'approche procédurale 
basée sur les diagrammes d'états-transitions.
*)

module Lex =
    type Var = A | B | C
    type Op = Plus | Minus | Multiply | Divide
    type LexicalUnit = Var of Var | Int of int | Op of Op

    let convertToVar result =
        match result with
        | Ok (chars, stream) -> 
            let lexicalVar = match List.ofSeq chars with
                             | ['A'] -> Var A
                             | ['B'] -> Var B
                             | ['C'] -> Var C
                             | _ -> failwith "Ne devrais pas arriver."
            Ok ([lexicalVar], stream)
        | Error -> Error

    let convertToOp result =
        match result with
        | Ok (chars, stream) ->
            let lexicalOp = match List.ofSeq chars with
                            | ['+'] -> Op Plus
                            | ['-'] -> Op Minus
                            | ['*'] -> Op Multiply
                            | ['/'] -> Op Divide
                            | _ -> failwith "Ne devrais pas arriver."
            Ok ([lexicalOp], stream)
        | Error -> Error

    let convertToInt result =
        match result with
        | Ok (chars, stream) ->
            let lexicalOp = let charStrings = Seq.map string chars
                            String.Concat charStrings
                            |> Int32.Parse |> Int
            Ok ([lexicalOp], stream)
        | Error -> Error

    let chars list =
        let parsers = List.map one list
        List.reduce either parsers

    let varp = chars ['A'; 'B'; 'C'] >> convertToVar
    let opp = chars ['+'; '-'; '*'; '/'] >> convertToOp
    let integerp = chars ['0'..'9']
    let intp = integerp >>= many integerp >> convertToInt
    let spacep = skip (one ' ')

    let lexp = List.reduce either [varp; opp; intp] >>= many spacep

    let parser = many lexp


    // Prend une chaîne de caractère en entrée et retourne une liste de lexèmes
    // si la chaîne en entrée est une expression de la grammaire valide.
    let parse string =
       let stream = stream string

       let result = parser stream

       match result with
       | Ok (seq, _) -> List.ofSeq seq
       | Error -> failwith "Impossible de valider la phrase."