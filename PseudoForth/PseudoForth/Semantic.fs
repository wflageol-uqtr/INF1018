module Semantic

open Streams
open Combinators
open Lexer

type AstToken = Scalar of Token
              | List of AstToken list

let rec parseAstToken stream : Result<Token, AstToken> =
    let nextToken, stream = next stream
    match nextToken with
    | None -> Error
    | Some (Sep '[') -> parseList stream
    | Some token -> Ok ([Scalar token], stream)

and parseList stream : Result<Token, AstToken> =
    let rec doParseList acc stream =
        let nextToken = peek stream
        match nextToken.Value with
        | Sep ']' -> let _, stream = next stream
                     Ok ([List (List.rev acc)], stream)
        | _ -> match parseAstToken stream with
               | Ok (token, stream) -> doParseList (Seq.head token :: acc) stream
               | _ -> Error
    doParseList [] stream

let parse = many parseAstToken >> unwrap