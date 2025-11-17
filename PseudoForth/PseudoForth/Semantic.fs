module Semantic

open Streams
open Combinators
open Lexer

type ScalarType = ScalarInt of int
                | ScalarText of string

type DeclarationType = { WordName: string
                         Definition: Instruction list }

and Instruction = Scalar of ScalarType
                  | Call of string
                  | Assignation of string
                  | Operation of string
                  | Declaration of DeclarationType

let operators = [ "+"; "-"; "/"; "*"
                  "dup"; "swap"; "print" ]

let reservedKeywords = [ "to"; ";"; ":" ]

let checkValidIdent id =
    if List.contains id (List.append operators reservedKeywords)
    then failwith "Un nom de variable ne peut pas être un mot-clé."

let rec parseIdent stream id =
    match id with
    | "to" -> let varName, stream = next stream
              match varName with
              | Some (Ident s) ->
                    checkValidIdent s
                    Assignation s, stream
              | _ -> failwith "Impossible de créer une variable avec ce nom."
    | ":" -> let nameToken, stream = next stream
             let wordName = match nameToken with
                            | Some (Ident s) ->
                               checkValidIdent s
                               s
                            | _ -> failwith "Nom de mot invalide."

             let rec accContent acc stream =
                let nextToken, stream = next stream
                match nextToken with
                | Some (Sep ';') -> acc, stream
                | Some token -> accContent (token :: acc) stream
                | None -> failwith "Aucune fin à la définition de mot."

             let tokens, stream = accContent [] stream
             let tokenStream = Streams.stream (List.rev tokens)

             Declaration { WordName = wordName
                           Definition = generateAst tokenStream }, stream

    | op when List.contains op operators ->
              Operation op, stream
    | id -> Call id, stream

and generateAst stream =
    let rec doGenerate acc stream =
        let (token, stream) = next stream
        match token with
        | None -> acc
        | Some t -> match t with
                    | Integer i -> let scalar = Scalar (ScalarInt i)
                                   doGenerate (scalar :: acc) stream
                    | Text s -> let scalar = Scalar (ScalarText s)
                                doGenerate (scalar :: acc) stream
                    | Sep _ -> failwith "Séparateur unique."
                    | Ident id -> let r, stream = parseIdent stream id
                                  doGenerate (r :: acc) stream

    doGenerate [] stream
    |> List.rev