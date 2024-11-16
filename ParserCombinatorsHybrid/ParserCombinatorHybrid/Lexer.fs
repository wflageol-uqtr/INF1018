module Lexer

open System
open Combinators

type Operator = Plus | Minus | Multiply | Divide

type Token = Integer of int
           | Op of Operator

// Parser qui valide que le prochain caractère fait partie de la liste fournie.
let chars (list: char list) : Parser<char, char> =
    one (fun c -> List.contains c list)

// Parser qui valide que le prochain mot correspond au mot fourni.
let word (string: String) : Parser<char, char> =
    Seq.map (fun e -> chars [e]) string
    |> Seq.reduce bind

// Converti une séquence de caractère en Token de type Integer.
let convertToInt (seq: char seq) : Token seq =
    let string = Seq.toArray seq |> String
    [Int32.Parse string |> Integer]

// Converti une séquence de caractère en Token de type Op.
let convertToOp (seq: char seq) : Token seq =
    [match Seq.toList seq with
     | ['+'] -> Plus
     | ['-'] -> Minus
     | ['/'] -> Divide
     | ['*'] -> Multiply
     | _ -> failwith "Convert failed."
     |> Op]

// Définition du lexique du langage :
let digit = chars ['0'..'9']
let op = chars ['-'; '+'; '*'; '/'] |> convert convertToOp
let spaces = chars [' '] >>= many (chars [' '])
let integer = digit >>= (many digit) |> convert convertToInt
let skipSpaces = skip spaces

// Parser lexical combinant tous les parsers définis.
let lex = either [integer; op; skipSpaces]
          |> many
          >> unwrap