module Lexer

open System
open Combinators

type Token = Integer of int
           | Ident of string
           | Text of string
           | Sep of char

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

let convertToIdent seq : Token seq =
    let string = Seq.toArray seq |> String
    [Ident string]

let convertToText seq : Token seq =
    let string = Seq.toArray seq |> String
    [Text string]

let convertToListSep seq : Token seq =
    [Sep <| Seq.head seq]

// Définition du lexique du langage :
let digit = chars ['0'..'9']
let spaces = chars [' '; '\r'; '\n'; '\t'] >>= many (chars [' '; '\r'; '\n'; '\t'])
let integer = digit >>= (many digit) |> convert convertToInt
let alpha = List.reduce List.append [['A'..'Z'];['a'..'z'];['+';'-';'*';'/';'_';':']]
            |> chars
let alphanum = either [alpha; digit]
let ident = alpha >>= many alphanum |> convert convertToIdent
let text = chars ['"'] >>. until (chars ['"']) |> convert convertToText
let sep = chars [';'] |> convert convertToListSep
let skipSpaces = skip spaces

// Parser lexical combinant tous les parsers définis.
let lex = either [text; integer; ident; sep; skipSpaces]
          |> many
          >> unwrap